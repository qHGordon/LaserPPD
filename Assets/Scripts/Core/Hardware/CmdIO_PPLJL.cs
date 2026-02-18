using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public delegate void SendData(byte[] buf, int len);

// IO板串口协议
public class CmdIO_PPLJL {
    static SendData sendData;
    public static bool[] connectStatue = new bool[Main.MAX_PLAYER];
    public static float[] connectTimeout = new float[Main.MAX_PLAYER];
    public static float[] connectSendTime = new float[Main.MAX_PLAYER];
    // 指令部分
    const int CMDBUF_SIZE = 16;
    const byte CMD_HEAD = 0xaa;
    const byte CMD_VERIFYCODE = 0x1a;

    //
    public static byte[] CmdBuf0 = new byte[CMDBUF_SIZE];       //???????????
    static int CmdIn0 = 0;
    //
    static byte[] OutBuf0 = new byte[CMDBUF_SIZE];

    public static int cmdReceiveCnt = 0;
    //
    static byte[] c_Parms = {
        1,  //CMDIOU_LINE = 0,         // ??
	    4,  //CMDIOU_VERSION = 1,  //	
	    2,  //CMDIOU_KEY_STA = 2,
	    1,  //CMDIOU_SSR_OUT = 3,
	    1,  //CMDIOU_LED_ONE = 4,	
	    //CMDIOU_COUNT = 5,
    };

    public static void CheckConnect() {
        for (int i = 0; i < Main.MAX_PLAYER; i++) {
            connectTimeout[i] += Time.deltaTime;
            if (connectTimeout[i] >= 3.5f) {
                connectTimeout[i] = 0;
                connectStatue[i] = false;
            }
            connectSendTime[i] += Time.deltaTime;
            if (connectSendTime[i] >= 1.5f) {
                connectSendTime[i] = 0;
                CMD0_SendCmd_Line((byte)i);
            }
        }
    }
    //    
    static void LAN_SendCmd(byte no, byte[] buf, int len) {
        int i;

        OutBuf0[0] = CMD_HEAD;
        OutBuf0[1] = no;
        // data
        for (i = 0; i < len; i++) {
            OutBuf0[i + 2] = buf[i];
        }
        // verify
        OutBuf0[len + 2] = CMD_VERIFYCODE;
        for (i = 0; i < len + 2; i++) {
            OutBuf0[len + 2] += OutBuf0[i];
        }
        OutBuf0[len + 2] &= 0x7f;
        //
        sendData(OutBuf0, len + 3);
    }

    //
    static byte sc_cmdno;
    static byte sc_len;
    static byte sc_c4;
    static int sc_c3;
    public static void LAN_SetCode(byte value) {
        //Main.Log("CC: " + value.ToString("X2"));
        if (value == CMD_HEAD) {
            CmdBuf0[0] = value;
            CmdIn0 = 1;
        } else if (value >= 0x80) {
            CmdIn0 = 0;
        } else if (CmdIn0 > 0) {
            CmdBuf0[CmdIn0] = value;
            CmdIn0++;
            if (CmdIn0 < 4)
                return; 
            sc_cmdno = CmdBuf0[2];	// 指令号            
            if (sc_cmdno >= c_Parms.Length) {
                CmdIn0 = 0;
                return;
            }
            sc_len = (byte)(c_Parms[sc_cmdno] + 3); // 长度
            if (CmdIn0 >= sc_len) {
                CmdIn0 = 0;
                sc_len--;
                sc_c4 = CMD_VERIFYCODE;
                for (sc_c3 = 0; sc_c3 < sc_len; sc_c3++) {
                    sc_c4 += CmdBuf0[sc_c3];
                }
                // 判断检验结果
                if ((sc_c4 & 0x7f) == CmdBuf0[sc_len]) {
                    // 收到正确指令
                    //Main.Log("Cmd: " + (en_CMDIO_CR)sc_cmdno);
                    switch ((en_CMD0)sc_cmdno) {
                    case en_CMD0.CMDIO_LINE: // 连线
                        if (CmdBuf0[1] < Main.MAX_PLAYER) {
                            connectTimeout[CmdBuf0[1]] = 0;
                            connectStatue[CmdBuf0[1]] = true;
                        }                        
                        break;

                    case en_CMD0.CMDIO_VERSION:
                        break;

                    case en_CMD0.CMDIO_KEY_STA:
                        //
                        Key.KEY_Update(CmdBuf0[1], CmdBuf0[3]);
                        //if (Main.statue <= en_MainStatue.Game_97) {
                        //    PAction.Check();
                        //}
                        break;

                    }
                }
            }
            if(CmdIn0 >= CmdBuf0.Length) {
                CmdIn0 = 0;
            }
        }
    }


    // 用户协议 --------------------------------------------------------------------------------------
    enum en_CMD0 {
        CMDIO_LINE = 0,     
        CMDIO_VERSION = 1,  
        CMDIO_KEY_STA = 2,
        CMDIO_SSR_OUT = 3,
        CMDIO_LED_ONE = 4,
        CMDIO_COUNT = 5,
    }

    public static void Init(SendData funSendData) {
        sendData = funSendData;

        for (int i = 0; i < Main.MAX_PLAYER; i++) {
            connectTimeout[i] = 0;
            connectStatue[i] = true;
            connectSendTime[i] = i * 0.3f;
        }
    }

    // 发送区
    const int OUT_BUF_SIZE = 64;
    static byte[] CMD0_OutBuf = new byte[OUT_BUF_SIZE];
    static byte[] CMD0_OutBufTemp = new byte[OUT_BUF_SIZE];

    public static void CMD0_SendCmd_Line(byte playerno) {
        //
        CMD0_OutBuf[0] = (byte)en_CMD0.CMDIO_LINE;
        LAN_SendCmd(playerno, CMD0_OutBuf, 1);
    }
    public static void CMD0_SendCmd_SSR(byte playerno, byte enable) {
        CMD0_OutBuf[0] = (byte)en_CMD0.CMDIO_SSR_OUT;
        CMD0_OutBuf[1] = enable;
        //
        LAN_SendCmd(playerno, CMD0_OutBuf, 2);
    }
    public static void CMD0_SendCmd_LedOne(byte no, byte ledValue) {
        CMD0_OutBuf[0] = (byte)en_CMD0.CMDIO_LED_ONE;
        CMD0_OutBuf[1] = (byte)(ledValue & 0x7f);
        //
        LAN_SendCmd(no, CMD0_OutBuf, 2);
    }

}
