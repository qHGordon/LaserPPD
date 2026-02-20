using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserPPD.Core;

/// <summary>
/// 硬件模拟器管理器
/// 负责模拟硬件连接状态、心跳包机制，并拦截 LED 数据发送
/// </summary>
public class SimulatorManager : MonoBehaviour
{
    [Header("模拟器设置")]
    [Tooltip("是否启用硬件模拟器")]
    public bool enableSimulator = true;
    
    [Tooltip("模拟心跳包间隔（秒）")]
    public float heartbeatInterval = 1.0f;
    
    [Tooltip("强制连接状态（模拟硬件已连接）")]
    public bool forceConnected = true;

    private float lastHeartbeatTime;
    private static SimulatorManager instance;

    public static SimulatorManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (enableSimulator)
        {
            InitializeSimulator();
        }
    }

    void Update()
    {
        if (!enableSimulator) return;

        // 模拟心跳包机制
        if (Time.time - lastHeartbeatTime >= heartbeatInterval)
        {
            lastHeartbeatTime = Time.time;
            SimulateHeartbeat();
        }

        // 强制保持连接状态
        if (forceConnected)
        {
            ForceConnectionStatus();
        }
    }

    /// <summary>
    /// 初始化模拟器
    /// </summary>
    private void InitializeSimulator()
    {
        Debug.Log("[硬件模拟器] 初始化硬件模拟器...");
        
        // 强制设置连接状态
        ForceConnectionStatus();
        
        // 初始化心跳时间
        lastHeartbeatTime = Time.time;
        
        Debug.Log("[硬件模拟器] 硬件模拟器已启动");
    }

    /// <summary>
    /// 强制设置连接状态为已连接
    /// </summary>
    private void ForceConnectionStatus()
    {
#if IO_YDGZ
        // 强制设置 YDGZ 连接状态
        CmdIO_YDGZ.connectStatue = true;
        CmdIO_YDGZ.connectTimeout = 0; // 重置超时计时器
        
        // 设置 IO 版本（模拟硬件已连接）
        Main.ioVersion = 1;
#endif
    }

    /// <summary>
    /// 模拟心跳包（防止游戏检测不到心跳而断开连接）
    /// </summary>
    private void SimulateHeartbeat()
    {
#if IO_YDGZ
        // 重置超时计时器，模拟收到心跳响应
        CmdIO_YDGZ.connectTimeout = 0;
        CmdIO_YDGZ.connectStatue = true;
        
        // 可选：发送模拟心跳响应
        // 这里不需要实际发送数据，只需要重置超时计时器即可
#endif
    }

    /// <summary>
    /// 启用/禁用模拟器
    /// </summary>
    public void SetSimulatorEnabled(bool enabled)
    {
        enableSimulator = enabled;
        if (enabled)
        {
            InitializeSimulator();
        }
        else
        {
            Debug.Log("[硬件模拟器] 硬件模拟器已禁用");
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
