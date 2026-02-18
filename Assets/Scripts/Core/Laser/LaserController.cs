using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 激光控制器实现类
/// 负责激光矩阵管理和数据发送
/// </summary>
public class LaserController : MonoBehaviour, ILaserController
{
    /// <summary>
    /// 硬件协议接口（通过依赖注入或自动创建）
    /// </summary>
    private IHardwareProtocol hardwareProtocol;

    /// <summary>
    /// 是否使用默认的 YDGZProtocol（在Inspector中可配置）
    /// </summary>
    [SerializeField]
    private bool useDefaultProtocol = true;

    /// <summary>
    /// 镭射矩阵
    /// </summary>
    private int[,] laserMatrix;

    /// <summary>
    /// 设置硬件协议（用于依赖注入）
    /// </summary>
    /// <param name="protocol">硬件协议实例</param>
    public void SetHardwareProtocol(IHardwareProtocol protocol)
    {
        hardwareProtocol = protocol;
    }

    /// <summary>
    /// 获取硬件协议实例
    /// </summary>
    public IHardwareProtocol GetHardwareProtocol()
    {
        return hardwareProtocol;
    }

    /// <summary>
    /// 初始化硬件协议（如果未设置，则使用默认的 YDGZProtocol）
    /// </summary>
    void Awake()
    {
        if (hardwareProtocol == null && useDefaultProtocol)
        {
            // 创建默认的 YDGZProtocol 实例
            hardwareProtocol = new YDGZProtocol();
            // 注意：CmdIO_YDGZ.Init() 应该在 MainRun 中已经调用
            // 这里我们只是创建协议包装器，实际的发送逻辑仍然通过静态类
        }
    }

    /// <summary>
    /// 初始化激光矩阵
    /// </summary>
    /// <param name="width">矩阵宽度</param>
    /// <param name="height">矩阵高度</param>
    public void InitMatrix(int width, int height)
    {
        laserMatrix = new int[height, width];
    }

    /// <summary>
    /// 发送目标矩阵数据到激光设备（实现 ILaserController.SendMatrix）
    /// </summary>
    /// <param name="targetMatrix">目标矩阵数据</param>
    public void SendMatrix(int[,] targetMatrix)
    {
        if (laserMatrix == null)
        {
            Debug.LogError("镭射矩阵未初始化.");
            return;
        }

        if (targetMatrix.GetLength(0) != laserMatrix.GetLength(0) || targetMatrix.GetLength(1) != laserMatrix.GetLength(1))
        {
            Debug.LogError("目标矩阵与镭射矩阵的维度不匹配.");
            return;
        }
        uint[] data = MatrixToIndexArray(targetMatrix);
        if (data == null)
        {
            Debug.LogError("转换矩阵为索引数组失败.");
            return;
        }
        
        if (hardwareProtocol == null)
        {
            Debug.LogError("硬件协议未初始化，无法发送数据.");
            return;
        }
        
        hardwareProtocol.SendLedOne(0, 63, data, data.Length);
    }

    /// <summary>
    /// 发送目标的镭射矩阵数据到镭射设备（保留向后兼容，内部调用 SendMatrix）
    /// </summary>
    /// <param name="targetMatrix">目标矩阵数据</param>
    public void SendTargetMatrix(int[,] targetMatrix)
    {
        SendMatrix(targetMatrix);
    }
    /// <summary>
    /// 将矩阵转换为索引数组,从矩阵的左上角开始，S形向下（实现 ILaserController.MatrixToIndexArray）
    /// </summary>
    /// <param name="matrix">输入矩阵</param>
    /// <returns>索引数组，如果转换失败返回null</returns>
    public uint[] MatrixToIndexArray(int[,] matrix)
    {
        if (matrix == null)
        {
            Debug.LogError("Matrix is null.");
            return null;
        }
        int height = matrix.GetLength(0);
        int width = matrix.GetLength(1);
        uint[] indexArray = new uint[height * width];
        int index = 0;
        for (int i = 0; i < height; i++)
        {
            // S形向下
            if (i % 2 == 0)
            {
                for (int j = 0; j < width; j++)
                {
                    indexArray[index++] = (uint)matrix[i, j];
                }
            }
            else
            {
                for (int j = width - 1; j >= 0; j--)
                {
                    indexArray[index++] = (uint)matrix[i, j];
                }
            }
        }
        return indexArray;
    }
    /// <summary>
    /// 发送激光数据（实现 ILaserController.SendLaserData）
    /// </summary>
    /// <param name="laserId">激光ID (0-63)</param>
    /// <param name="isOpen">是否开启</param>
    /// <param name="data">数据数组</param>
    /// <param name="length">数据长度</param>
    public void SendLaserData(int laserId, bool isOpen, uint[] data, int length)
    {
        if (laserId < 0 || laserId > 63)
        {
            Debug.LogError("Invalid laser ID.");
            return;
        }
        
        if (hardwareProtocol == null)
        {
            Debug.LogError("硬件协议未初始化，无法发送数据.");
            return;
        }
        
        if (isOpen)
        {
            hardwareProtocol.SendLedOne(laserId, 63, data, length);
        }
        else
        {
            hardwareProtocol.SendLedOne(laserId, 0, data, length);
        }
    }
}
