using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LaserPPD.Core
{


public class CmdIO_WeChat
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
    const int CMD_HEAD = 0xAA;
    const int CMD_END = 0xDD;
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
            AppConst.ioVersion = 0;
        }
        connectSendTime += Time.deltaTime;
        if (connectSendTime >= 1.0f)
        {
            connectSendTime = 0;
            ////CMD0_SendCmd_Line();
            ////CMD0_SendCmd_Score(20);
        }
    }

    static void LAN_SendCmd(byte cmd, byte[] buf, int len)
    {
        int i;
#if UNITY_EDITOR
        //Debug.Log ("SendCmd: " + (en_CMD0)cmd);
#endif
        //CMD_HEAD
        OutBuf[0] = (byte)(CMD_HEAD);

        OutBuf[1] = cmd;
        // data
        for (i = 0; i < len; i++)
        {
            OutBuf[i + 2] = (byte)(buf[i] & 0xff);
        }
        // verify
        byte checkSum = CMD_VERIFYCODE;
        for (i = 0; i < len + 2; i++)
        {
            checkSum += OutBuf[i];
        }
        OutBuf[len + 2] = (byte)(checkSum & 0xff);
        OutBuf[len + 3] = (byte)CMD_END;
        //
    //    for (int k = 0; k < len + 4; k++)
  //      {
//           Debug.LogError("SendCmd: " + OutBuf[k].ToString("x2"));

      //  }
        if (sendData != null)
        {
            sendData(OutBuf, len + 4);
        }
    }

    //
    static byte sc_cmdno;
    static int sc_len;
    static byte sc_c4;
    static int sc_c3;
    static bool NeedSend_Score = false;
    public static void LAN_SetCode(byte value)
    {
//        Debug.LogError (value.ToString ("X2"));
        CmdIOUpdate.LAN_SetCode(value);

        if (value == 0xAA)
        {
            CmdBuf[0] = value;
            CmdIn = 1;
        }
        else if (CmdIn > 0)
        {
            CmdBuf[CmdIn] = value;
            CmdIn++;
            if (CmdIn < 4)
                return;
            sc_len = 6;// (int)CmdBuf[0] | (CmdBuf[1] << 7); // 长度

            if (CmdIn >= sc_len )
            {
                CmdIn = 0;
                if (CmdBuf[5]!=CMD_END)
                {
                    return;
                }
                int cmdLen = sc_len-2 ;
                sc_c4 = CMD_VERIFYCODE;
                for (sc_c3 = 0; sc_c3 < cmdLen; sc_c3++)
                {
                    sc_c4 += CmdBuf[sc_c3];
                }
                // 判断检验结果
                if (sc_c4 == CmdBuf[cmdLen])
                {

                    connectTimeout = 0;
                    // 收到正确指令
                    sc_cmdno = CmdBuf[1];   // 指令号          
#if UNITY_EDITOR
                    Debug.LogError((en_CMDLED)sc_cmdno);
#endif

                    switch ((en_CMDLED)sc_cmdno)
                    {
                        case en_CMDLED.CMDLED_GetReady:
#if UNITY_EDITOR
                            Debug.LogError ("收到第一次打卡 " );
#endif
                            if (NeedSend_Score)
                            {
#if UNITY_EDITOR
                                Debug.LogError("漏打卡，发送分数");
#endif
                                //CMD0_SendCmd_ReadySendScore();
                                CMD0_SendCmd_Score(FjData.g_Fj[0].Scores);
                                FjData.g_Fj[0].Scores = 0;
                                SaveData.Save();
                            }
                            NeedSend_Score = true;
                            CMD0_SendCmd_ReturnReady();
                            break;
                        case en_CMDLED.CMDLED_GetEndPlaying:
#if UNITY_EDITOR
                            Debug.LogError("收到发送分数");
#endif
                            //      CMD0_SendCmd_ReadySendScore();
                            CMD0_SendCmd_Score(FjData.g_Fj[0].Scores);
                            FjData.g_Fj[0].Scores = 0;
                            SaveData.Save();
                            NeedSend_Score = false;
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
        CMDLED_GetReady = 0x0B,
        CMDLED_ReturnReady = 0x8B,
        CMDLED_GetEndPlaying = 0x0E,


     //   CMDLED_ReadySendScore = 0x5B,
        CMDLED_Score = 0x8E,


    }

    public static void Init(SendData funSendData)
    {
        sendData = funSendData;
        //
        connectTimeout = 0;
        connectStatue = true;
        connectSendTime = 1f;
        NeedSend_Score = false;
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
    public static void CMD0_SendCmd_ReadySendScore()
    {
        CMD0_OutBuf[0] = 0x00;  //(byte)((value >> 8) & 0xff);
        CMD0_OutBuf[1] = 0x01;  //(byte)((value >> 0) & 0xff);
        //
     //   LAN_SendCmd((byte)en_CMDLED.CMDLED_ReadySendScore, CMD0_OutBuf, 2);
    }
    
    public static void CMD0_SendCmd_ReturnReady()
    {
        CMD0_OutBuf[0] = 0x01;  //(byte)((value >> 8) & 0xff);
        CMD0_OutBuf[1] = 0x00;  //(byte)((value >> 0) & 0xff);
 
        //
        LAN_SendCmd((byte)en_CMDLED.CMDLED_ReturnReady, CMD0_OutBuf, 2);
    }

    public static void CMD0_SendCmd_Score(int value)
    {
      
        CMD0_OutBuf[0] = (byte)((value >> 8) & 0xff);
        CMD0_OutBuf[1] = (byte)((value >> 0) & 0xff);
        //
        LAN_SendCmd((byte)en_CMDLED.CMDLED_Score, CMD0_OutBuf, 2);
    }
#endif
}
}
