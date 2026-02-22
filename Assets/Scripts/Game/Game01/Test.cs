using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    uint color;
    [Header("Red")]
    [Range(0, 255)]
    public byte r;
    [Range(0, 255)]
    public byte g;
    [Range(0, 255)]
    public byte b;
    [ContextMenu("亮镭射")]
    public void LaserAll()
    {
        uint[] data = new uint[80];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (uint)i;
        }
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(0, 63, data, data.Length);

    }
    [ContextMenu("亮拍拍灯")]
    public void PPD_ALL()
    {
        uint[] data = new uint[62];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (uint)i;
        }
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(4, Color2Uint(r, g, b), data, data.Length);
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(5, Color2Uint(r, g, b), data, data.Length);
    }
    public uint Color2Uint(byte r, byte g, byte b)
    {
        int temp = 0;
        temp = ((r & 0xFF) << 17) | ((g & 0xFF) << 9) | ((b & 0xFF) << 1);
        return (uint)temp;
    }
    public Color Uint2Color(uint color)
    {
        byte r = (byte)((color >> 17) & 0xFF);
        byte g = (byte)((color >> 9) & 0xFF);
        byte b = (byte)((color >> 1) & 0xFF);
        return new Color32(r, g, b, 255);
    }
}
