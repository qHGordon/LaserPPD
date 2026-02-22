using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Serialization;

// 基础数据结构（严格遵循协议规范）
//public class User
//{
//    [JsonProperty("userId")]
//    public string UserId { get; set; }

//    [JsonProperty("userNickName")]
//    public string UserNickName { get; set; }
//}

//public class OpenLevel
//{
//    [JsonProperty("users")]
//    public List<User> Users { get; set; } = new List<User>();

//    [JsonProperty("levelName")]
//    public string LevelName { get; set; }

//    [JsonProperty("levelDuration")]
//    public string LevelDuration { get; set; } // 单位：秒
//}

//public class GameResult
//{
//    [JsonProperty("userId")]
//    public string UserId { get; set; }

//    [JsonProperty("score")]
//    public string Score { get; set; }

//    [JsonProperty("timeStamp")]
//    public string TimeStamp { get; set; } // 格式：yyyy-MM-dd HH:mm:ss
//}

//public class GameResults
//{
//    [JsonProperty("results")]
//    public List<GameResult> Results { get; set; } = new List<GameResult>();
//}

//public class Message
//{
//    [JsonProperty("action")]
//    public string Action { get; set; } // "OpenLevel" 或 "GameResults"

//    [JsonProperty("content")]
//    public string Content { get; set; } // 嵌套对象的JSON
//}

//// 刷卡程序核心类
//public class CardSwipingClient
//{
//    private const int GAME_PORT = 40001;
//    private const int TIMEOUT = 3000; // 3秒超时
//    private const int MAX_RETRY = 2;  // 最大重试次数

//    // JSON序列化设置（严格遵循协议）
//    private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
//    {
//        ContractResolver = new CamelCasePropertyNamesContractResolver(),
//        NullValueHandling = NullValueHandling.Ignore,
//        Formatting = Formatting.None
//    };

//    /// <summary>
//    /// 发送选关指令到游戏程序
//    /// </summary>
//    public bool SendOpenLevel(string gameServerIP, OpenLevel openLevel)
//    {
//        var message = new Message
//        {
//            Action = "OpenLevel",
//            Content = JsonConvert.SerializeObject(openLevel, _jsonSettings)
//        };

//        return SendWithRetry(gameServerIP, GAME_PORT, message);
//    }

//    /// <summary>
//    /// 接收游戏结果（刷卡程序需监听40000端口）
//    /// </summary>
//    public GameResults ReceiveGameResults(TcpClient client)
//    {
//        using (var stream = client.GetStream())
//        {
//            stream.ReadTimeout = TIMEOUT;

//            // 读取数据
//            byte[] buffer = new byte[4096];
//            int bytesRead = stream.Read(buffer, 0, buffer.Length);
//            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

//            // 解析消息
//            var message = JsonConvert.DeserializeObject<Message>(json);
//            if (message?.Action == "GameResults")
//            {
//                return JsonConvert.DeserializeObject<GameResults>(message.Content);
//            }
//            throw new ProtocolViolationException("Invalid message action");
//        }
//    }

//    // 带重试机制的TCP发送
//    private bool SendWithRetry(string ip, int port, Message message, int retryCount = 0)
//    {
//        try
//        {
//            using (var client = new TcpClient())
//            {
//                client.ConnectAsync(ip, port).Wait(TIMEOUT);

//                string json = JsonConvert.SerializeObject(message, _jsonSettings);
//                byte[] data = Encoding.UTF8.GetBytes(json);

//                using (var stream = client.GetStream())
//                {
//                    stream.Write(data, 0, data.Length);

//                    // 等待OpenLevel确认（协议要求）
//                    if (message.Action == "OpenLevel")
//                    {
//                        byte[] ackBuffer = new byte[1024];
//                        int ackSize = stream.Read(ackBuffer, 0, ackBuffer.Length);
//                        // 此处可添加ACK内容验证逻辑
//                    }
//                }
//            }
//            return true;
//        }
//        catch (Exception ex) when (retryCount < MAX_RETRY)
//        {
//            Console.WriteLine($"Error: {ex.Message}. Retrying...");
//            return SendWithRetry(ip, port, message, retryCount + 1);
//        }
//        catch
//        {
//            return false;
//        }
//    }
//}
