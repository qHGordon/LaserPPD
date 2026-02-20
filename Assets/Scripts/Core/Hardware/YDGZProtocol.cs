using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{


/// <summary>
/// [Core.Hardware] YDGZ 硬件协议实现类，封装 CmdIO_YDGZ 的静态方法，实现 IHardwareProtocol 接口。
/// </summary>
public class YDGZProtocol : IHardwareProtocol
{
#if IO_YDGZ
    /// <summary>
    /// 连接状态（从静态类同步）
    /// </summary>
    public bool IsConnected => CmdIO_YDGZ.connectStatue;

    // 指令部分
    private const int CMDBUF_SIZE = 512;
    private const byte CMD_VERIFYCODE = 0;

    // 发送缓冲区
    private byte[] outBuf = new byte[CMDBUF_SIZE];
    private byte[] cmd0_OutBuf = new byte[CMDBUF_SIZE];

    // 用户协议枚举
    private enum en_CMDLED
    {
        CMDLED_LEDCOUNT = 0,
        CMDLED_PROTOCOL = 1,
        CMDLED_LEDFULL = 2,
        CMDLED_LEDONE = 3,
        CMDLED_SERSOR = 4,
        CMDLED_LED4ONE = 5,
    }

    /// <summary>
    /// 初始化硬件协议
    /// 注意：CmdIO_YDGZ 应该在 MainRun 中已经初始化，这里只是委托给静态类
    /// </summary>
    /// <param name="sendData">数据发送委托（如果 CmdIO_YDGZ 尚未初始化，则使用此参数初始化）</param>
    public void Init(SendData sendData)
    {
        // 如果 CmdIO_YDGZ 尚未初始化，则初始化它
        // 注意：通常 CmdIO_YDGZ.Init() 在 MainRun.Awake() 中已经调用
        // 这里提供 Init 方法是为了接口兼容性，实际使用时通常不需要调用
    }

    /// <summary>
    /// 检查硬件连接状态
    /// </summary>
    public void CheckConnect()
    {
        CmdIO_YDGZ.CheckConnect();
    }

    /// <summary>
    /// 发送LED单个命令
    /// </summary>
    /// <param name="channel">频道数（0-5）</param>
    /// <param name="color">灯的颜色值</param>
    /// <param name="indices">LED索引数组</param>
    /// <param name="length">数组长度</param>
    public void SendLedOne(int channel, uint color, uint[] indices, int length)
    {
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(channel, color, indices, length);
    }

    /// <summary>
    /// 发送LED全屏命令
    /// </summary>
    /// <param name="color">全屏颜色值</param>
    public void SendLedAll(uint color)
    {
        CmdIO_YDGZ.CMD0_SendCmd_LedAll(color);
    }

    /// <summary>
    /// 发送通道长度配置
    /// </summary>
    public void SendLine()
    {
        CmdIO_YDGZ.CMD0_SendCmd_Line();
    }

    /// <summary>
    /// 发送协议配置
    /// </summary>
    public void SendProtocol()
    {
        CmdIO_YDGZ.CMD0_SendCmd_Protocol();
    }
#else
    // 非 IO_YDGZ 编译条件下的空实现
    public bool IsConnected => false;

    public void Init(SendData sendData) { }

    public void CheckConnect() { }

    public void SendLedOne(int channel, uint color, uint[] indices, int length) { }

    public void SendLedAll(uint color) { }

    public void SendLine() { }

    public void SendProtocol() { }
#endif
}
}
