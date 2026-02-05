using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CmdIO_YDGZ
{
#if IO_YDGZ
    static SendData sendData;
    public static bool connectStatue;
    public static float connectTimeout;
    public static float connectSendTime;

    // ========== 硬件模拟器事件 ==========
    /// <summary>
    /// LED 数据发送事件（用于硬件模拟器）
    /// 参数：channel(通道), color(颜色), indices(索引数组), length(长度)
    /// </summary>
    public static event Action<int, uint, uint[], int> OnLedDataSent;

    // 指令部分
    const int CMDBUF_SIZE = 512;
    const byte CMD_VERIFYCODE = 0;

    //
    public static byte[] CmdBuf = new byte[CMDBUF_SIZE];       //???????????
    static int CmdIn = 0;
    //
    static byte[] OutBuf = new byte[CMDBUF_SIZE];

    public static int cmdReceiveCnt = 0;

    public static void CheckConnect()
    {
        connectTimeout += Time.deltaTime;
        if (connectTimeout >= 3.5f)
        {
            connectTimeout = 0;
            connectStatue = false;
            Main.ioVersion = 0;
        }
        connectSendTime += Time.deltaTime;
        if (connectSendTime >= 1.0f)
        {
            connectSendTime = 0;
            CMD0_SendCmd_Line();
            CMD0_SendCmd_Protocol();
        }
    }

    static void LAN_SendCmd(byte cmd, byte[] buf, int len)
    {
        int i;
#if UNITY_EDITOR
        //Debug.Log ("SendCmd: " + (en_CMD0)cmd);
#endif
        //CMD_HEAD
        OutBuf[0] = (byte)(0x80 | (len & 0x7f));
        OutBuf[1] = (byte)((len >> 7) & 0x7f);
        OutBuf[2] = cmd;
        // data
        for (i = 0; i < len; i++)
        {
            OutBuf[i + 3] = (byte)(buf[i] & 0x7f);
        }
        // verify
        byte checkSum = CMD_VERIFYCODE;
        for (i = 0; i < len + 3; i++)
        {
            checkSum += OutBuf[i];
        }
        OutBuf[len + 3] = (byte)(checkSum & 0x7f);
        //
        if (sendData != null)
        {
            string str = "";
            for (i = 0; i < len + 4; i++)
            {
                str += OutBuf[i].ToString("D2") + " ";
            }
            //Debug.Log (str);
            sendData(OutBuf, len + 4);
        }
    }

    //
    static byte sc_cmdno;
    static int sc_len;
    static byte sc_c4;
    static int sc_c3;
    public static void LAN_SetCode(byte value)
    {
        //Debug.Log (value.ToString ("X2"));
        CmdIOUpdate.LAN_SetCode(value);

        if (value >= 0x80)
        {
            CmdBuf[0] = (byte)(value & 0x7f);
            CmdIn = 1;
        }
        else if (CmdIn > 0)
        {
            CmdBuf[CmdIn] = value;
            CmdIn++;
            if (CmdIn < 4)
                return;
            sc_len = (int)CmdBuf[0] | (CmdBuf[1] << 7); // 长度

            if (CmdIn >= sc_len + 4)
            {
                CmdIn = 0;
                int cmdLen = sc_len + 3;
                sc_c4 = CMD_VERIFYCODE;
                for (sc_c3 = 0; sc_c3 < cmdLen; sc_c3++)
                {
                    sc_c4 += CmdBuf[sc_c3];
                }
                // 判断检验结果
                if ((sc_c4 & 0x7f) == CmdBuf[cmdLen])
                {

                    connectTimeout = 0;
                    // 收到正确指令
                    sc_cmdno = CmdBuf[2];	// 指令号          
                    //if((en_CMD0)sc_cmdno != en_CMD0.CMDIO_KEY_STA && (en_CMD0)sc_cmdno != en_CMD0.CMDIO_ADC_VALUE) 
                    //Debug.Log ("DZD_Cmd: " + (en_CMDLED)sc_cmdno);
                    switch ((en_CMDLED)sc_cmdno)
                    {
                        case en_CMDLED.CMDLED_SERSOR:
                            if (sc_len < 2)
                                return;
                            int rxLen = sc_len - 2;
                            // channel:
                            int ch = CmdBuf[3] | (CmdBuf[4] << 7);
                            //
                            if (ch >= Set.StartPos.Length)
                                break;
                            int id;
                            for (int i = 0; i < rxLen; i++)
                            {
                                for (int j = 0; j < 7; j++)
                                {
                                    id = i * 7 + j;
                                    if (id >= Set.ChannelLength[ch])
                                        break;
                                    id += Set.StartPos[ch];
                                    if ((CmdBuf[5 + i] & (1 << j)) == 0)
                                    {
                                        LedKey.Update_KeyValue(id, 0);
                                    }
                                    else
                                    {
                                        LedKey.Update_KeyValue(id, 1);
                                    }
                                }
                            }
                            connectTimeout = 0;
                            connectStatue = true;
                            break;
                    }
                }
            }
            if (CmdIn >= CmdBuf.Length)
            {
                CmdIn = 0;
            }
        }
    }


    // 用户协议 --------------------------------------------------------------------------------------
    enum en_CMDLED
    {
        CMDLED_LEDCOUNT = 0,
        CMDLED_PROTOCOL = 1,
        CMDLED_LEDFULL = 2,
        CMDLED_LEDONE = 3,
        CMDLED_SERSOR = 4,
        CMDLED_LED4ONE = 5,
    }

    public static void Init(SendData funSendData)
    {
        sendData = funSendData;
        //
        connectTimeout = 0;
        connectStatue = true;
        connectSendTime = 1f;
    }

    // 发送区
    static byte[] CMD0_OutBuf = new byte[CMDBUF_SIZE];
    //
    public static void CMD0_SendCmd_Line()
    {
        //布局(每个通道长度)[6],
        for (int i = 0; i < Set.ChannelLength.Length; i++)
        {
            CMD0_OutBuf[0 + i * 2] = (byte)((Set.ChannelLength[i] >> 0) & 0x7f);
            CMD0_OutBuf[1 + i * 2] = (byte)((Set.ChannelLength[i] >> 7) & 0x7f);
        }
        //
        LAN_SendCmd((byte)en_CMDLED.CMDLED_LEDCOUNT, CMD0_OutBuf, 12);
    }
    public static void CMD0_SendCmd_Protocol()
    {
        for (int i = 0; i < 6; i++)
        {
            CMD0_OutBuf[i] = (byte)(Set.LedProtocol[i] & 0x7f);
        }
        //
        LAN_SendCmd((byte)en_CMDLED.CMDLED_PROTOCOL, CMD0_OutBuf, 6);
    }
    //
    public static void CMD0_SendCmd_LedAll(uint color)
    {
        CMD0_OutBuf[0] = (byte)((color >> 17) & 0x7f);
        CMD0_OutBuf[1] = (byte)((color >> 9) & 0x7f);
        CMD0_OutBuf[2] = (byte)((color >> 1) & 0x7f);
        //
        LAN_SendCmd((byte)en_CMDLED.CMDLED_LEDFULL, CMD0_OutBuf, 3);
        
        // 触发硬件模拟器事件（全屏命令，通道设为 -1 表示所有通道）
        OnLedDataSent?.Invoke(-1, color, null, 0);
    }
    /// <summary>
    /// 向控制板发送信号
    /// </summary>
    /// <param name="ch">频道数0到5</param>
    /// <param name="color">灯的颜色</param>
    /// <param name="bufId">命令数据</param>
    /// <param name="len">命令长度</param>
    public static void CMD0_SendCmd_LedOne(int ch, uint color, uint[] bufId, int len)
    {
        // ch:
        CMD0_OutBuf[0] = (byte)(ch & 0x7f);
        // idBuf:
        for (int i = 0; i < len; i++)
        {
            CMD0_OutBuf[1 + i * 2] = (byte)((bufId[i] >> 0) & 0x7f);
            CMD0_OutBuf[2 + i * 2] = (byte)((bufId[i] >> 7) & 0x7f);
        }
        // color:
        CMD0_OutBuf[1 + len * 2] = (byte)((color >> 17) & 0x7f);
        CMD0_OutBuf[2 + len * 2] = (byte)((color >> 9) & 0x7f);
        CMD0_OutBuf[3 + len * 2] = (byte)((color >> 1) & 0x7f);
        LAN_SendCmd((byte)en_CMDLED.CMDLED_LEDONE, CMD0_OutBuf, 4 + len * 2);
        
        // 触发硬件模拟器事件
        OnLedDataSent?.Invoke(ch, color, bufId, len);
    }
    public static void CMD0_SendCmd_LedOne_DianZhen(int ch, uint color,  uint[] bufId, int len)
    {
        // ch:
        CMD0_OutBuf[0] = (byte)(ch & 0x7f);
        // idBuf:
        for (int i = 0; i < len; i++)
        {
            CMD0_OutBuf[1 + i * 2] = (byte)((bufId[i] >> 0) & 0x7f);
            CMD0_OutBuf[2 + i * 2] = (byte)((bufId[i] >> 7) & 0x7f);
        }
        // color:
        CMD0_OutBuf[1 + len * 2] = (byte)((color >> 17) & 0x7f);
        CMD0_OutBuf[2 + len * 2] = (byte)((color >> 9) & 0x7f);
        CMD0_OutBuf[3 + len * 2] = (byte)((color >> 1) & 0x7f);
        CMD0_OutBuf[4 + len * 2] = (byte)((color >> 25) & 0x7f);
        //
        LAN_SendCmd((byte)en_CMDLED.CMDLED_LED4ONE, CMD0_OutBuf, 5 + len * 2);
    }
#endif
}
