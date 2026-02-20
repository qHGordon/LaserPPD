using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{


/// <summary>
/// 激光控制器接口
/// 定义激光控制的核心功能，用于解耦硬件通信层
/// </summary>
public interface ILaserController
{
    /// <summary>
    /// 初始化激光矩阵
    /// </summary>
    /// <param name="width">矩阵宽度</param>
    /// <param name="height">矩阵高度</param>
    void InitMatrix(int width, int height);

    /// <summary>
    /// 发送目标矩阵数据到激光设备
    /// </summary>
    /// <param name="targetMatrix">目标矩阵数据</param>
    void SendMatrix(int[,] targetMatrix);

    /// <summary>
    /// 发送激光数据
    /// </summary>
    /// <param name="laserId">激光ID (0-63)</param>
    /// <param name="isOpen">是否开启</param>
    /// <param name="data">数据数组</param>
    /// <param name="length">数据长度</param>
    void SendLaserData(int laserId, bool isOpen, uint[] data, int length);

    /// <summary>
    /// 将矩阵转换为索引数组（S形扫描）
    /// </summary>
    /// <param name="matrix">输入矩阵</param>
    /// <returns>索引数组，如果转换失败返回null</returns>
    uint[] MatrixToIndexArray(int[,] matrix);
}
}
