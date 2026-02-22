using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    /// <summary>
    /// 镭射矩阵
    /// </summary>
    private int[,] laserMatrix;
    public void InitMatrix(int width, int height)
    {
        laserMatrix = new int[height, width];
    }
    /// <summary>
    /// 发送目标的镭射矩阵数据到镭射设备
    /// </summary>
    /// <param name="targetMatrix"></param>
    public void SendTargetMatrix(int[,] targetMatrix)
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
        uint[] data =  MatrixToIndexArray(targetMatrix);
        if (data == null)
        {
            Debug.LogError("转换矩阵为索引数组失败.");
            return;
        }
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(0, 63, data, data.Length);
    }
    /// <summary>
    /// 将矩阵转换为索引数组,从矩阵的左上角开始，S形向下
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
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
    public void SendLaserData(int laserId, bool isOpen, uint[] data, int length)
    {
        if (laserId < 0 || laserId > 63)
        {
            Debug.LogError("Invalid laser ID.");
            return;
        }
        if (isOpen)
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne(laserId, 63, data, length);
        }
        else
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne(laserId, 0, data, length);
        }
    }
}
