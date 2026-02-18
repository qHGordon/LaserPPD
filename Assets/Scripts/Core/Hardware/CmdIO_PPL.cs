using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdIO_PPL {
#if IO_PPL || IO_PPLOC
    //
    static SendData sendData;
    public static bool connectStatue = true;
    public static float connectTimeout = 0;
    public static float connectSendTime = 0;
    // 指令部分
    const int CMDBUF_SIZE = 32;
    const byte CMD_HEAD = 0xd3;
    const byte CMD_VERIFYCODE = 0x32;

    //
    public static byte[] CmdBuf0 = new byte[CMDBUF_SIZE];       //???????????
    static int CmdIn0 = 0;
    //
    static byte[] OutBuf0 = new byte[CMDBUF_SIZE];

    public static int cmdReceiveCnt = 0;
    //
    static byte[] c_Parms = {
        0,  //CMDIO_LINE = 0,         // ??
	    3,  //CMDIO_VERSION = 1,  //	
	    1,  //CMDIO_KEY_STA = 2,
	    1,  //CMDIO_SSR_OUT = 3,
	    2,  //CMDIO_LED_ONE = 4,	
	    0,	//CMDIO_GET_UID = 5,	
	    14,	//CMDIO_SEND_UID = 6,
        2,	//CMDIO_SSR_CARD = 7,
	    0,	//CMDIO_CARD_OUT = 8,
	    //CMDIO_COUNT = 5,
    };

    public static void CheckConnect() {
        connectTimeout += Time.deltaTime;
        if (connectTimeout >= 1.5f) {
            connectTimeout = 0;
            connectStatue = false;
        }
        connectSendTime += Time.deltaTime;
        if (connectSendTime >= 0.5f) {
            connectSendTime = 0;
            CMD0_SendCmd_Line();
        }
    }
    //
    static void LAN_SendCmd(byte cmd, byte[] buf) {
        LAN_SendCmd(cmd, buf, c_Parms[cmd]);
    }
    static void LAN_SendCmd(byte cmd, byte[] buf, int len) {
        int i;

        OutBuf0[0] = CMD_HEAD;
        OutBuf0[1] = cmd;
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
        Debug.Log("CC: " + value.ToString("X2"));
        if (value == CMD_HEAD) {
            CmdBuf0[0] = value;
            CmdIn0 = 1;
        } else if (value >= 0x80) {
            CmdIn0 = 0;
        } else if (CmdIn0 > 0) {
            CmdBuf0[CmdIn0] = value;
            CmdIn0++;
            sc_cmdno = CmdBuf0[1];	// 指令号            
            if (sc_cmdno >= c_Parms.Length) {
                CmdIn0 = 0;
                return;
            }
            sc_len = (byte)(c_Parms[sc_cmdno] + 3); // 长度
            if (CmdIn0 >= sc_len && CmdIn0 >= 3) {
                CmdIn0 = 0;
                sc_len--;
                sc_c4 = CMD_VERIFYCODE;
                for (sc_c3 = 0; sc_c3 < sc_len; sc_c3++) {
                    sc_c4 += CmdBuf0[sc_c3];
                }
                // 判断检验结果
                if ((sc_c4 & 0x7f) == CmdBuf0[sc_len]) {
                    // 收到正确指令
                    //Main.Log("Cmd: " + (en_CMD0)sc_cmdno);
                    switch ((en_CMD0)sc_cmdno) {
                    case en_CMD0.CMDIO_LINE: // 连线
                        connectTimeout = 0;
                        connectStatue = true;
                        break;

                    case en_CMD0.CMDIO_SEND_UID:
                        break;

                    case en_CMD0.CMDIO_VERSION:
                        break;

                    case en_CMD0.CMDIO_KEY_STA:
                        ulong l1 = 0;
                        l1 = CmdBuf0[2];
                        
                        Key.KEY_Update(l1);
                        if (Main.statue <= en_MainStatue.Game_97) {
                            PAction.Check();
                        }
                        break;

                    case en_CMD0.CMDIO_CARD_OUT:
                        //PAction.outCardFlag[0] = true;
                        break;
                    }
                }
            }
        }
    }


    // 用户协议 --------------------------------------------------------------------------------------
    enum en_CMD0 {
        CMDIO_LINE = 0,         // ??
        CMDIO_VERSION = 1,  //	
        CMDIO_KEY_STA = 2,
        CMDIO_SSR_OUT = 3,
        CMDIO_LED_ONE = 4,
        CMDIO_GET_UID = 5,
        CMDIO_SEND_UID = 6,
        CMDIO_SSR_CARD = 7,
        CMDIO_CARD_OUT = 8,
        CMDIO_COUNT = 9,
    }

    public static void Init(SendData funSendData) {
        sendData = funSendData;
    }

    // 发送区
    const int OUT_BUF_SIZE = 64;
    static byte[] CMD0_OutBuf = new byte[OUT_BUF_SIZE];
    static byte[] CMD0_OutBufTemp = new byte[OUT_BUF_SIZE];

    public static void CMD0_SendCmd_Line() {
        //
        LAN_SendCmd((byte)en_CMD0.CMDIO_LINE, CMD0_OutBuf, 0);
    }
    public static void CMD0_SendCmd_GetUID() {
        //
        LAN_SendCmd((byte)en_CMD0.CMDIO_GET_UID, CMD0_OutBuf, 0);
    }
    public static void CMD0_SendCmd_GetVersion() {
        //
        LAN_SendCmd((byte)en_CMD0.CMDIO_VERSION, CMD0_OutBuf, 0);
    }
    public static void CMD0_SendCmd_SSR(byte no, byte enable) {
        //CMD0_OutBuf[0] = no;
        CMD0_OutBuf[0] = enable;
        //
        LAN_SendCmd((byte)en_CMD0.CMDIO_SSR_OUT, CMD0_OutBuf, 1);
    }
#if IO_PPLOC
    public static void CMD0_SendCmd_CardSSR(byte enable) {
        //CMD0_OutBuf[0] = no;
        CMD0_OutBuf[0] = (byte)Set.setVal.OutorMode;
        CMD0_OutBuf[1] = enable;
        //
        LAN_SendCmd((byte)en_CMD0.CMDIO_SSR_CARD, CMD0_OutBuf, 2);
    }
#endif
    public static void CMD0_SendCmd_LedOne(byte no, byte enable) {
        CMD0_OutBuf[0] = no;
        CMD0_OutBuf[1] = enable;
        //
        LAN_SendCmd((byte)en_CMD0.CMDIO_LED_ONE, CMD0_OutBuf, 2);
    }
#endif  // IO_PPL
    }
