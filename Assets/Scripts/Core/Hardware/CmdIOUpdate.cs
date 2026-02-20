using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{


public class CmdIOUpdate
{

    static SendData sendData;


    // 指令部分
    const int CMD_BUF_SIZE = 2400;
    const byte CMD_HEAD = 0xbf;
    const byte VERIFY_CODE = 0x64;

    //
    static byte[] CmdBuf = new byte[CMD_BUF_SIZE];       //???????????
    static int CmdBufIndex = 0;
    static int CmdLen;
    //
    static byte[] Cmd_OutBuf = new byte[CMD_BUF_SIZE];
    static byte[] Cmd_TempBuf = new byte[CMD_BUF_SIZE];
    //
    public static int errorCode = 0;
    static float connectTimeout = 3;
    public static bool connectStatue = true;
    public static void CheckConnect () {
        if (CmdIO_YDGZ.connectStatue) {
            errorCode = 0;
            return;
        }

        if (connectTimeout > 0) {
            connectTimeout -= Time.deltaTime;
        } else {
            connectStatue = false;
        }
    }


    static void SendCmd (byte[] buf, int len) {
        int i;
        int inid = 0;
        int outid = 0;
        byte cxor;

        if (len > 2360)
            return;
        //Debug.Log("Send Len: " + len);

        Cmd_OutBuf[0] = CMD_HEAD;
        Cmd_OutBuf[1] = (byte)((len >> 7) & 0x7f);
        Cmd_OutBuf[2] = (byte)((len >> 0) & 0x7f);

        // Data: 8->7 ×ª»»
        outid = 3;
        for (; inid < len;) {
            cxor = 0;
            for (i = 0; i < 7 && inid < len; i++) {
                Cmd_OutBuf[outid] = (byte)(buf[inid] & 0x7f);
                if ((buf[inid] & 0x80) != 0) {
                    cxor |= (byte)(1 << i);
                }
                inid++;
                outid++;
            }
            Cmd_OutBuf[outid] = cxor;
            outid++;
        }

        // length	
        len = outid + 1;
        Cmd_OutBuf[1] = (byte)((len >> 7) & 0x7f);
        Cmd_OutBuf[2] = (byte)((len >> 0) & 0x7f);

        // verify
        Cmd_OutBuf[outid] = VERIFY_CODE;
        for (i = 0; i < outid; i++) {
            Cmd_OutBuf[outid] += Cmd_OutBuf[i];
        }
        Cmd_OutBuf[outid] &= 0x7f;

        //
        if (sendData != null) {
            sendData (Cmd_OutBuf, len);
        }
    }

    //
    public static void LAN_SetCode (byte value) {
        int inid;
        int outid;
        int bitLen;
        int i;
        byte cxor;

        if (value == CMD_HEAD) {
            CmdBuf[0] = value;
            CmdBufIndex = 1;
            CmdLen = 10;
        } else if (value >= 0x80) {
            CmdBufIndex = 0;
        } else if (CmdBufIndex > 0) {
            CmdBuf[CmdBufIndex] = value;
            CmdBufIndex++;
            //
            if (CmdBufIndex == 4) {
                CmdLen = (int)(CmdBuf[1] << 7) | CmdBuf[2];
                if (CmdLen < 5 || CmdLen > CMD_BUF_SIZE) {
                    CmdBufIndex = 0;
                    return;
                }
            }
            if (CmdBufIndex >= CmdLen) {
                CmdBufIndex = 0;
                CmdLen--;       // È¥µôÐ£Ñé×Ö½Ú
                                // verify:
                cxor = VERIFY_CODE;
                for (i = 0; i < CmdLen; i++) {
                    cxor += CmdBuf[i];
                }
                if ((cxor & 0x7f) != CmdBuf[CmdLen])
                    return;
                // data: 7->8 ×ª»»
                CmdLen -= 3;    // È¥µôÍ·ºÍ³¤¶È
                inid = 0;   // inid
                outid = 0;  // outid

                for (; inid < CmdLen;) {
                    if (inid + 8 > CmdLen) {
                        bitLen = CmdLen - inid - 1;
                    } else {
                        bitLen = 7;
                    }
                    cxor = CmdBuf[3 + inid + bitLen];
                    for (i = 0; i < bitLen; i++) {
                        Cmd_TempBuf[outid] = CmdBuf[3 + inid];
                        if ((cxor & (1 << i)) != 0) {
                            Cmd_TempBuf[outid] |= 0x80;
                        }
                        outid++;
                        inid++;
                    }
                    inid++;
                }
                //Debug.Log("GetAPICmd: " + (en_CmdIap)Cmd_TempBuf[0]);
                //
                connectTimeout = 3;
                connectStatue = true;

                switch ((en_CmdIap)Cmd_TempBuf[0]) {
                case en_CmdIap.CMDIAP_GetInfo:
                    //
                    IoAppDownload.GetFileInfo ();
                    errorCode = 0;
                    break;

                case en_CmdIap.CMDIAP_GetData:
                    // offet[3]
                    int offset = (Cmd_TempBuf[1] << 24);
                    offset |= (Cmd_TempBuf[2] << 16);
                    offset |= (Cmd_TempBuf[3] << 8);
                    offset |= (Cmd_TempBuf[4] << 0);
                    // len[2]
                    int len = (Cmd_TempBuf[5] << 8);
                    len |= (Cmd_TempBuf[6] << 0);
                    //
                    IoAppDownload.GetFileData (offset, len);
                    errorCode = 0;
                    break;

                case en_CmdIap.CMDIAP_Error:
                    errorCode = Cmd_TempBuf[1];
                    break;
                }
            }
            if (CmdBufIndex >= CMD_BUF_SIZE) {
                CmdBufIndex = 0;
            }
        }
    }

    public static void Init (SendData funSendData) {
        sendData = funSendData;
    }

    // 
    enum en_CmdIap
    {
        CMDIAP_GetInfo = 0,
        CMDIAP_GetInfoRet = 1,
        CMDIAP_GetData = 2,
        CMDIAP_GetDataRet = 3,
        CMDIAP_Error = 4,
    }
    // ---------------------------------------------------------------------------------

    public static void SendCmd_GetInfoRet (byte res, long size) {
        Cmd_TempBuf[0] = (byte)en_CmdIap.CMDIAP_GetInfoRet;
        //
        Cmd_TempBuf[1] = res;
        //
        Cmd_TempBuf[2] = (byte)((size >> 24) & 0xff);
        Cmd_TempBuf[3] = (byte)((size >> 16) & 0xff);
        Cmd_TempBuf[4] = (byte)((size >> 8) & 0xff);
        Cmd_TempBuf[5] = (byte)((size >> 0) & 0xff);
        //
        SendCmd (Cmd_TempBuf, 6);
    }
    public static void SendCmd_GetDataRet (int offset, byte[] buf, int len) {
        Cmd_TempBuf[0] = (byte)en_CmdIap.CMDIAP_GetDataRet;
        //
        Cmd_TempBuf[1] = (byte)((offset >> 24) & 0xff);
        Cmd_TempBuf[2] = (byte)((offset >> 16) & 0xff);
        Cmd_TempBuf[3] = (byte)((offset >> 8) & 0xff);
        Cmd_TempBuf[4] = (byte)((offset >> 0) & 0xff);
        //
        for (int i = 0; i < len; i++) {
            Cmd_TempBuf[5 + i] = buf[i];
        }
        SendCmd (Cmd_TempBuf, len + 5);
    }
    public static void SendCmd_Error (byte error) {
        Cmd_TempBuf[0] = (byte)en_CmdIap.CMDIAP_Error;
        //
        Cmd_TempBuf[1] = error;
        //
        SendCmd (Cmd_TempBuf, 2);
    }
}
}
