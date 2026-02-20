using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{


public class PAction
{
    const float OUT_TICKET_TIMEOUT = 3.0f;	//退币超时时间
    const float OUT_GIFT_TIMEOUT = 10.0f;	//退币超时时间
    const float OUT_COIN_TIMEOUT = 6.0f;	//退币超时时间

    public static bool[] outing = new bool[AppConst.MAX_PLAYER];        // 退币/彩票/礼品 正在退状态
    public static bool[] outEnable = new bool[AppConst.MAX_PLAYER];     // 退币/彩票/礼品 开关
    public static bool[] outError = new bool[AppConst.MAX_PLAYER];
    static float[] outTimeout = new float[AppConst.MAX_PLAYER];         // 退币/彩票/礼品超时时间    
    static float[] outDcTime = new float[AppConst.MAX_PLAYER];         // 退币/彩票/礼品间隔时间    
    static int[] outCount = new int[AppConst.MAX_PLAYER];                   // 单次已退币个数


    //
    //float[] codeTablePowerTime_CoinIn = new float[AppConst.MAX_PLAYER];
    //float[] codeTablePowerTime_CoinOut = new float[AppConst.MAX_PLAYER];
    //bool[] codeTableStatue_CoinIn = new bool[AppConst.MAX_PLAYER];
    //bool[] codeTableStatue_CoinOut = new bool[AppConst.MAX_PLAYER];

    // Use this for initialization
    public static void Init () {
        for (int i = 0; i < AppConst.MAX_PLAYER; i++) {
            outCount[i] = 0;
            outTimeout[i] = Time.time;
            outDcTime[i] = Time.time;
            outing[i] = false;
            outError[i] = false;
            IO.Out_SSR (i, 0);

            //CodeTable_CoinIn_Stop (i);
            //CodeTable_CoinOut_Stop (i);

            if (FjData.g_Fj[i].Wins <= 0) {    // 枪神
                outEnable[i] = false;
            } else {
                outEnable[i] = true;
            }
        }
    }
    // Update is called once per frame
    public static void Check () {
        return;
        if (IoAppDownload.mainStatue >= en_MainStatue.Game_98 && IoAppDownload.mainStatue != en_MainStatue.LoadScene)
            return;

        int coins;
        //int playerNum = AppConst.MAX_PLAYER;
        //if (Set.setVal.PlayerMode == (int)en_PlayerMode.One) {
        //    playerNum = 1;
        //}
        int playerNum = 1;
        // 娱乐 --------------------------------------------------------------------------
        for (int i = 0; i < playerNum; i++) {
            coins = FjData.g_Fj[i].Coins;
            //投币信号
            if (Key.KEYFJ_CinPressed (i)) {
                if (i == 0 || Set.setVal.InOutMode != (int)en_InOutMode.OneInOneOut) {
                    FjData.acc[i].CoinIn++;
                    FjData.totalAcc[i].CoinIn++;
                    coins++;    //+= (uint)Set.setVal.CoinBl;
                    FjData.SaveAcc_CoinIn (i);
                    FjData.SaveTotalAcc_CoinIn (i);
                    AppConst.onCoinIn?.Invoke();
                }
            }

            //退币信号
            if (Key.KEYFJ_CoutPressed (i)) {
                if (Time.time - outDcTime[i] >= (float)Set.setVal.OutDcTime / 1000) {
                    outDcTime[i] = Time.time;
                    if (Set.setVal.InOutMode == (int)en_InOutMode.TwoInTwoOut ||
                        Set.setVal.PlayerMode == (int)en_PlayerMode.Free) {
                        // 双退
                        if (outing[i]) {
                            outCount[i]++;
                            outTimeout[i] = Time.time;
                            OutOne_ForYule (i);
                        }
                    } else if (i == 0) {
                        // 单退
                        for (int j = 0; j < AppConst.MAX_PLAYER; j++) {
                            if (outing[j]) {
                                outCount[j]++;
                                outTimeout[j] = Time.time;
                                OutOne_ForYule (j);
                                break;
                            }
                        }
                    }
                }
            }

            //退币按键 
            
            if (outing[i] == true) {
                // 正在退
                //	outTimeout[i] += Time.deltaTime;
                switch ((en_OutMode)Set.setVal.OutMode) {
                case en_OutMode.OutTicket:
                    if (Time.time - outTimeout[i] >= OUT_TICKET_TIMEOUT) {
                        SSR_Stop (i);
                        outError[i] = true;
                    }
                    // 剩余个数不足
                    if (FjData.g_Fj[i].Wins <= 0) {
                        SSR_Stop (i);
                    }
                    break;
                case en_OutMode.OutGift:
                    if (Time.time - outTimeout[i] >= OUT_GIFT_TIMEOUT) {
                        SSR_Stop (i);
                        outError[i] = true;
                    }
                    // 剩余个数不足
                    if (FjData.g_Fj[i].Wins <= 0) {
                        SSR_Stop (i);
                    }
                    break;
                }
            } else if (FjData.g_Fj[i].Wins > 0 && outEnable[i]) {
                if (Set.setVal.InOutMode == (int)en_InOutMode.TwoInTwoOut ||
                    Set.setVal.PlayerMode == (int)en_PlayerMode.Free) {
                    // 自动退奖励: 双退
                    if (outError[i] == false) { // && Main.enableAutoOut[i]) {
                        switch ((en_OutMode)Set.setVal.OutMode) {
                        case en_OutMode.OutTicket:
                        case en_OutMode.OutGift:
                            SSR_Start (i);
                            break;
                        }
                    }
                } else {
                    // (单退)自动退币检测
                    int i1;
                    for (i1 = 0; i1 < AppConst.MAX_PLAYER; i1++) {
                        if (outError[i1] || outing[i1]) {
                            break;  // 不能退
                        }
                    }
                    if (i1 >= AppConst.MAX_PLAYER) {
                        //if (Main.enableAutoOut[i]) {
                        // 自动退奖励: 
                        switch ((en_OutMode)Set.setVal.OutMode) {
                        case en_OutMode.OutTicket:
                        case en_OutMode.OutGift:
                            SSR_Start (i);       // 1退口
                            return;
                        }
                        //}
                    }
                }
            }

            // 保存
            if (coins != FjData.g_Fj[i].Coins) {
                FjData.g_Fj[i].Coins = coins;
                FjData.SaveData_Coins (i);

            }
        }

        // 修复
        if (Key.KEYFJ_ResetPressed (0)) {
            for (int i = 0; i < AppConst.MAX_PLAYER; i++) {
                outError[i] = false;
            }
        }
    }

    public static bool IsOuting (int playerno) {
        return outing[playerno];
    }

    static void OutOne_ForYule (int playerno) {
        switch ((en_OutMode)Set.setVal.OutMode) {
        case en_OutMode.OutTicket:
            if (FjData.g_Fj[playerno].Wins > 0) {
                FjData.g_Fj[playerno].Wins--;
                FjData.SaveData_Wins (playerno);

            }
            FjData.acc[playerno].TicketOut++;
            FjData.totalAcc[playerno].TicketOut++;
            FjData.SaveAcc_TicketOut (playerno);
            FjData.SaveTotalAcc_TicketOut (playerno);
            break;
        case en_OutMode.OutGift:
            if (FjData.g_Fj[playerno].Wins > 0) {
                FjData.g_Fj[playerno].Wins--;
                FjData.SaveData_Wins (playerno);

            }
            FjData.acc[playerno].GiftOut++;
            FjData.totalAcc[playerno].GiftOut++;
            FjData.SaveAcc_GiftOut (playerno);
            FjData.SaveTotalAcc_GiftOut (playerno);
            break;
        }
    }

    //
    public static void SSR_Start (int playerno) {
        outTimeout[playerno] = Time.time;
        outing[playerno] = true;
        if (Set.setVal.InOutMode == (int)en_InOutMode.TwoInTwoOut) {
            IO.Out_SSR (playerno, 1);    // 双退
        } else {
            IO.Out_SSR (0, 1);           // 单退
        }
    }

    public static void SSR_Stop (int playerno) {
        outCount[playerno] = 0;
        outTimeout[playerno] = Time.time;
        outing[playerno] = false;
        if (Set.setVal.InOutMode == (int)en_InOutMode.TwoInTwoOut) {
            IO.Out_SSR (playerno, 0);    // 双退
        } else {
            IO.Out_SSR (0, 0);           // 单退
        }
        outEnable[playerno] = false;
    }

}
}
