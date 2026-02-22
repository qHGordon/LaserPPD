using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    private void Awake()
    {
        instance = this;
    }
    [Header("Server Settings")]
    string serverIP = "127.0.0.1";
    int serverPort = 40000;

    [Header("Connection Settings")]
    public float reconnectInterval = 5f; // 断线重连间隔(秒)
    public float heartbeatInterval = 30f; // 心跳间隔(秒)
    int level_Index = 0;
    string levelName = "";
    // 网络状态
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private Thread _receiveThread;
    private bool _isConnected;
    private bool _isConnecting;
    private float _lastReconnectTime = -1f;
    private float _lastHeartbeatTime;

    // 消息队列（用于跨线程处理）
    private Queue<Action> _mainThreadActions = new Queue<Action>();

    // 游戏状态
    private string _currentLevelName;
    private List<User> _currentUsers = new List<User>();

    // 消息类定义（与协议一致）
    public class Message
    {
        public string action;
        public string content;
    }

    // OpenLevel指令内容
    public class OpenLevelContent
    {
        public string levelName;
        public User[] users;
    }

    // 用户信息
    public class User
    {
        public string userId;
        public string userNickName;
    }
    [System.Serializable]
    public class LevelResult
    {
        public string userId;         // 用户唯一标识
        public string score;          // 本局游戏得分（字符串类型）
        public string timeStamp;      // 游戏结束时间戳
    }

    // 游戏结果集类
    [System.Serializable]
    public class LevelResults
    {
        public LevelResult[] levelResults; // 包含多个游戏结果
    }
    void Start()
    {
        ConnectToServer();
    }

    void Update()
    {
        // 处理主线程任务
        while (_mainThreadActions.Count > 0)
        {

            Action action = _mainThreadActions.Dequeue();
            action?.Invoke();
        }

        // 断线重连逻辑
        if (!_isConnected && !_isConnecting && Time.time - _lastReconnectTime > reconnectInterval)
        {
            _lastReconnectTime = Time.time;
            ConnectToServer();
        }

        // 心跳包发送
        if (_isConnected && Time.time - _lastHeartbeatTime > heartbeatInterval)
        {
            _lastHeartbeatTime = Time.time;
            SendHeartbeat();
        }
    }

    void OnDestroy()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        if (_isConnecting || _isConnected) return;

        _isConnecting = true;
        Debug.Log("尝试连接服务器...");

        // 使用Thread连接，避免阻塞主线程
        Thread connectThread = new Thread(() =>
        {
            try
            {
                _tcpClient = new TcpClient();
                _tcpClient.Connect(serverIP, serverPort);
                _networkStream = _tcpClient.GetStream();
                _isConnected = true;
                _isConnecting = false;
                Debug.Log("连接服务器成功！");

                // 发送GameRunning消息
                SendGameRunning();

                // 开始接收消息
                _receiveThread = new Thread(ReceiveMessages);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();
            }
            catch (Exception e)
            {
               // Debug.LogError($"连接失败: {e.Message}");
                _isConnecting = false;
                _isConnected = false;
            }
        });

        connectThread.IsBackground = true;
        connectThread.Start();
    }

    public void Disconnect()
    {
        _isConnected = false;
        _isConnecting = false;

        try
        {
            // 发送关闭消息
            SendMessage("GameClosing", "");

            // 关闭线程
            if (_receiveThread != null && _receiveThread.IsAlive)
                _receiveThread.Abort();

            // 关闭网络连接
            _networkStream?.Close();
            _tcpClient?.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"断开连接时出错: {e.Message}");
        }
    }

    private void ReceiveMessages()
    {
        byte[] buffer = new byte[4096];

        while (_isConnected)
        {
            try
            {
                if (_networkStream == null || !_networkStream.CanRead)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // 直接检查是否有可用数据
                if (!_networkStream.DataAvailable)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // 读取可用数据
                int bytesRead = _networkStream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    HandleDisconnection();
                    return;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log($"收到消息: {message}");

                // 在主线程处理消息
                _mainThreadActions.Enqueue(() => HandleServerMessage(message));
            }
            catch (Exception e)
            {
                Debug.LogError($"接收消息时出错: {e.Message}");
                HandleDisconnection();
                return;
            }
        }
    }
    private void HandleDisconnection()
    {
        _isConnected = false;
        Debug.LogWarning("与服务器断开连接");
    }

    private void HandleServerMessage(string json)
    {
        try
        {
            Debug.Log($"收到服务器消息: {json}");
            Message msg = JsonConvert.DeserializeObject<Message>(json);

            if (msg == null)
            {
                Debug.LogWarning("无法解析消息");
                return;
            }

            switch (msg.action)
            {
                case "OpenLevel":
                    HandleOpenLevel(msg.content);
                    break;

                case "LevelProgress":
                    // 服务器询问进度，立即返回当前进度
                    SendLevelProgress(GetCurrentLevelProgress());
                    break;

                case "CloseGame":
                    // 服务器要求关闭游戏
                    SendGameClosing();
                    // 退出游戏
                    Application.Quit();
                    break;

                case "LevelOpening_OK":
                case "LevelOpened_OK":
                case "LevelResults_OK":
                case "Heartbeat_OK":
                    // 确认消息，可记录日志
                    Debug.Log($"收到确认: {msg.action}");
                    break;

                default:
                    Debug.LogWarning($"未知消息类型: {msg.action}");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"处理服务器消息时出错: {e.Message}");
        }
    }
    private int ExtractLevelNumber(string levelName)
    {
        // 方法1: 使用正则表达式直接匹配数字
        Match match = Regex.Match(levelName, @"\d+");
        if (match.Success && int.TryParse(match.Value, out int number))
        {
            return number;
        }

        // 方法2: 尝试从常见格式中提取
        if (levelName.Contains("level"))
        {
            int startIndex = levelName.IndexOf("level") + 5;
            if (startIndex < levelName.Length)
            {
                string numberPart = levelName.Substring(startIndex).Trim();
                if (int.TryParse(numberPart, out int num))
                {
                    return num;
                }
            }
        }

        return -1; // 提取失败
    }

    private void HandleOpenLevel(string content)
    {
        try
        {
            Debug.Log($"收到OpenLevel消息: {content}");

            // 解析嵌套的JSON内容
            OpenLevelContent openLevel = JsonConvert.DeserializeObject<OpenLevelContent>(content);

            if (openLevel == null)
            {
                Debug.LogError("解析OpenLevel内容失败");
                return;
            }

            // 解析关卡名称
            string _levelName = openLevel.levelName;
            levelName = _levelName;
            if (string.IsNullOrEmpty(levelName))
            {
                Debug.LogWarning("关卡名称为空，使用默认关卡");
                levelName = "默认关卡";
            }
            level_Index = ExtractLevelNumber(levelName);
            Debug.Log($"加载关卡: {level_Index}");

            // 解析玩家列表
            List<User> players = new List<User>();
            if (openLevel.users != null)
            {
                foreach (User user in openLevel.users)
                {
                    if (!string.IsNullOrEmpty(user.userId) && !string.IsNullOrEmpty(user.userNickName))
                    {
                        players.Add(user);
                        Debug.Log($"玩家: ID={user.userId}, 昵称={user.userNickName}");
                    }
                }
            }

            Debug.Log($"玩家数量: {players.Count}");

            // 保存关卡信息
            _currentLevelName = levelName;
            _currentUsers.Clear();
            _currentUsers.AddRange(players);

            // 发送LevelOpening响应
            SendLevelOpening();

            // 实际加载关卡
            LoadLevel(level_Index, _currentUsers);
        }
        catch (JsonException ex)
        {
            Debug.LogError($"JSON解析错误: {ex.Message}\n原始内容: {content}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"处理OpenLevel消息时出错: {ex.Message}");
        }
    }

    private void LoadLevel(int _index, List<User> users)
    {
        Debug.Log($"开始加载关卡: {_index}");

        Main.MapIndex = _index;
        Main.MapID = _index;
        Main.gameLevel = _index % 10;
        Main.playerNum = users.Count;
        Game97_Main.instance.EnterGame(_index);
    }

    public void SendGameResults()
    {
        // 确保有玩家数据
        if (_currentUsers == null || _currentUsers.Count == 0)
        {
            Debug.LogWarning("没有玩家数据，无法发送游戏结果");
            return;
        }

        // 创建结果数组
        LevelResult[] results = new LevelResult[_currentUsers.Count];

        // 获取当前时间戳
        string timestamp = GetCurrentTimestamp();

        // 为每个玩家生成结果（这里使用模拟数据）
        for (int i = 0; i < _currentUsers.Count; i++)
        {
            // 实际项目中应从游戏逻辑中获取真实得分
            int playerScore = FjData.g_Fj[0].Scores;

            results[i] = new LevelResult
            {
                userId = _currentUsers[i].userId,
                score = playerScore.ToString(),
                timeStamp = timestamp
            };

            Debug.Log($"玩家结果: ID={_currentUsers[i].userId}, 得分={playerScore}");
        }

        // 创建结果集
        LevelResults levelResults = new LevelResults
        {
            levelResults = results
        };

        // 序列化结果
        string jsonContent = JsonConvert.SerializeObject(levelResults,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            });

        // 发送结果
        SendMessage("LevelResults", jsonContent);
        Debug.Log("游戏结果已发送");
    }

    // 获取当前时间戳（格式：年-月-日 时:分:秒.毫秒）
    private string GetCurrentTimestamp()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    // 获取当前关卡进度（0-1之间的浮点数）
    private float GetCurrentLevelProgress()
    {
        if (Game01_Main.instance == null)
        {
            return 0;
        }
        else
        {
            if (Game01_Main.instance.statue < en_Game01_Sta.Play)
            {
                return 0;

            }
            else if (Game01_Main.instance.statue == en_Game01_Sta.Play)
            {
                return 0.5f;

            }
            else
            {
                return 1;

            }
        }

    }

    // 发送心跳包
    private void SendHeartbeat()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        SendMessage("Heartbeat", timestamp);
    }

    private void SendGameRunning()
    {
        SendMessage("GameRunning", "");
    }

    public void SendLevelOpening()
    {
        SendMessage("LevelOpening", levelName);
    }

    public void SendLevelOpened()
    {
        SendMessage("LevelOpened", levelName);
    }

    private void SendLevelProgress(float progress)
    {
        SendMessage("LevelProgress", progress.ToString());
    }

    private void SendGameClosing()
    {
        SendMessage("GameClosing", "");
    }

    // 通用消息发送方法
    private void SendMessage(string action, string content)
    {
        if (!_isConnected || _networkStream == null || !_networkStream.CanWrite)
        {
            Debug.LogWarning("发送失败：未连接到服务器");
            return;
        }

        try
        {
            // 创建消息对象
            Message msg = new Message { action = action, content = content };

            // 序列化JSON（不使用长度前缀）
            string json = JsonConvert.SerializeObject(msg, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            });

            // 转换为UTF-8字节
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // 调试：记录发送的原始JSON
            Debug.Log($"发送原始JSON: {json}");

            // 直接发送JSON数据（不添加长度前缀）
            _networkStream.Write(jsonBytes, 0, jsonBytes.Length);
            _networkStream.Flush();

            Debug.Log($"已发送: {action}");
        }
        catch (Exception e)
        {
            Debug.LogError($"发送消息({action})时出错: {e.Message}");
            HandleDisconnection();
        }
    }
}