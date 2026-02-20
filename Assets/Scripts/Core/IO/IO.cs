using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{




public enum en_PlayerIO
{
    LED1 = 0,
    LED2,
    LED3,
    LED4,
    SSR1,
    SSR2,
}

public class IO
{
    public const int MAX_IO_ONEPLAYER = 5;

    // 玩家按键脚位定义 
    //	public const int IO_PLAYER_LED1 	= (int)en_PlayerIO.LED1;
    //	public const int IO_PLAYER_LED2 	= (int)en_PlayerIO.LED2;
    //	public const int IO_PLAYER_MK1		= (int)en_PlayerIO.MK1;
    //	public const int IO_PLAYER_MK2		= (int)en_PlayerIO.MK2;
    //	public const int IO_PLAYER_SSR		= (int)en_PlayerIO.SSR;

    //
    static byte IO_Statue;
    static bool sendChange;
    static float sendTime;
    public static bool[] gunMotorSta = new bool[AppConst.MAX_PLAYER];
    public static byte[] wallLedValue = new byte[AppConst.MAX_WALLLED];
    //
    public static void Init()
    {
        Update((int)en_PlayerIO.LED1, false);
        Update((int)en_PlayerIO.LED2, false);
        Update((int)en_PlayerIO.LED3, false);
        Update((int)en_PlayerIO.LED4, false);
        Update((int)en_PlayerIO.SSR1, false);
        Update((int)en_PlayerIO.SSR2, false);

        for (int i = 0; i < AppConst.MAX_PLAYER; i++)
        {
            GunMotorStop(i);
            ButtonLED(i, 0);
        }
    }

    static float wallLedSendTime;
    static int wallLedId = 0;
    public static void CheckSend()
    {
        // 200ms 发一次
        if (Time.time - sendTime >= 0.2f)
        {
            sendChange = true;
        }

        //if (sendChange) {
        //    sendChange = false;
        //    CmdIO.CMD0_SendCmd_IO(IO_Statue);
        //    sendTime = Time.time;
        //}

        if (Set.setVal.WallLedNum > 0)
        {
            wallLedSendTime += Time.deltaTime;
            if (wallLedSendTime >= 0.1f)
            {
                wallLedSendTime = 0;
                if (++wallLedId >= Set.setVal.WallLedNum)
                {
                    wallLedId = 0;
                }
                WallLED_One(wallLedId, wallLedValue[wallLedId]);
            }
        }
    }

    public static void Update(int key, bool enable)
    {
        if (key < MAX_IO_ONEPLAYER)
        {
            if (enable == false)
            {
                IO_Statue &= (byte)~(1 << key);
            }
            else
            {
                IO_Statue |= (byte)(1 << key);
            }
            sendChange = true;
        }
    }
    //
    public static void Out_LED(int no, byte enable)
    {
        //Main.Log("LDE_"+no.ToString() + " :" + enable.ToString());
#if NEW_IO
        CmdIO.CMD0_SendCmd_LED((byte)no, enable);
        CmdIO.CMD0_SendCmd_LED((byte)no, enable);
#endif
    }
    //
    public static void Out_SSR(int no, byte enable)
    {
        //       Main.Log("SSR_" + no.ToString() + " :" + enable.ToString());
#if NEW_IO
            CmdIO.CMD0_SendCmd_SSR((byte)no, enable);
            CmdIO.CMD0_SendCmd_SSR((byte)no, enable);
#endif
    }

    //
    /*
    public static void GunMotorStart(int playerno, float time)
    {
        if (gunMotorDcTime[playerno] > 0)
            return;
        if (playerno < AppConst.MAX_PLAYER && time > 0)
        {
            Out_LED(playerno * 2, 1);
            Out_LED(playerno * 2 + 1, 1);
            gunMotorTime[playerno] = time;
            gunMotorDcTime[playerno] = 0.1f;
        }
    }
     * */
    public static void GunMotorStart(int playerno)
    {
        if (playerno >= AppConst.MAX_PLAYER)
            return;
        Out_LED(playerno * 2, 1);
        Out_LED(playerno * 2 + 1, 1);
        gunMotorSta[playerno] = true;
    }
    public static void GunMotorStop(int playerno)
    {
        if (playerno >= AppConst.MAX_PLAYER)
            return;
        Out_LED(playerno * 2, 0);
        Out_LED(playerno * 2 + 1, 0);
        gunMotorSta[playerno] = false;
    }


    public static void ButtonLED(int no, byte power)
    {
#if NEW_IO
        CmdIO.CMD0_SendCmd_ButtonLED((byte)no, power);
        CmdIO.CMD0_SendCmd_ButtonLED((byte)no, power);
#endif
    }

    public static void WallLED_All(byte power)
    {
        for (int i = 0; i < wallLedValue.Length; i++)
        {
            wallLedValue[i] = power;
        }
        CmdIO_PPLJL.CMD0_SendCmd_LedOne(0x7f, power);  //广播地址: 0x7f
        CmdIO_PPLJL.CMD0_SendCmd_LedOne(0x7f, power);
    }
    public static void WallLED_One(int no, byte power)
    {
#if IO_PPLJL
        if (no < wallLedValue.Length)
        {
            wallLedValue[no] = power;
            CmdIO_PPLJL.CMD0_SendCmd_LedOne((byte)no, power);
        }
#endif
    }


    public static void GunOut(byte power)
    {
        uint[] bufId = { 0 };
        uint color = 0;
        if (power != 0)
        {
            color = 63;
        }
        //Framebuffer.Update_PointColor (5, 0, color);
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(5, color, bufId, 1);
    }
}
}
