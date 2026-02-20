using System;

namespace LaserPPD.Core
{


/// <summary>
/// [Core.Hardware] 硬件协议接口
/// 定义硬件通信的核心方法，用于解耦硬件通信层
/// </summary>
public interface IHardwareProtocol
{
    /// <summary>
    /// 初始化硬件协议
    /// </summary>
    /// <param name="sendData">数据发送委托</param>
    void Init(SendData sendData);

    /// <summary>
    /// 检查硬件连接状态
    /// </summary>
    void CheckConnect();

    /// <summary>
    /// 获取连接状态
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 发送LED单个命令（最常用的方法）
    /// </summary>
    /// <param name="channel">频道数（0-5）</param>
    /// <param name="color">灯的颜色值</param>
    /// <param name="indices">LED索引数组</param>
    /// <param name="length">数组长度</param>
    void SendLedOne(int channel, uint color, uint[] indices, int length);

    /// <summary>
    /// 发送LED全屏命令
    /// </summary>
    /// <param name="color">全屏颜色值</param>
    void SendLedAll(uint color);

    /// <summary>
    /// 发送通道长度配置
    /// </summary>
    void SendLine();

    /// <summary>
    /// 发送协议配置
    /// </summary>
    void SendProtocol();
}
}
