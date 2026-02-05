using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 镭射游戏点结构体（已废弃，请使用统一的 GamePoint）
/// 保留此类型定义仅用于向后兼容，实际使用 GamePoint
/// </summary>
[System.Obsolete("GameLeiShePoint 已废弃，请使用统一的 GamePoint 结构体")]
public struct GameLeiShePoint
{
    // 内部使用 GamePoint 存储数据
    private GamePoint point;

    /// <summary>
    /// 点的颜色（访问基础点的颜色）
    /// </summary>
    public uint color
    {
        get => point.color;
        set => point.color = value;
    }

    /// <summary>
    /// 点的状态（访问基础点的状态）
    /// </summary>
    public enPointSta statue
    {
        get => point.statue;
        set => point.statue = value;
    }

    /// <summary>
    /// 绑定计数（闪烁次数）
    /// </summary>
    public int bindCnt
    {
        get => point.bindCnt;
        set => point.bindCnt = value;
    }

    /// <summary>
    /// 绑定时间（闪烁间隔）
    /// </summary>
    public float bindTime
    {
        get => point.bindTime;
        set => point.bindTime = value;
    }

    /// <summary>
    /// 错误标志
    /// </summary>
    public bool error
    {
        get => point.error;
        set => point.error = value;
    }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public GameLeiShePoint(enPointSta statue, uint color, int bindCnt = 0, float bindTime = 0, bool error = false)
    {
        point = new GamePoint(statue, color, 0, 0, 0);
        point.bindCnt = bindCnt;
        point.bindTime = bindTime;
        point.error = error;
    }

    /// <summary>
    /// 隐式转换为 GamePoint
    /// </summary>
    public static implicit operator GamePoint(GameLeiShePoint leiShePoint)
    {
        return leiShePoint.point;
    }

    /// <summary>
    /// 从 GamePoint 隐式转换
    /// </summary>
    public static implicit operator GameLeiShePoint(GamePoint point)
    {
        GameLeiShePoint result = new GameLeiShePoint(point.statue, point.color, point.bindCnt, point.bindTime, point.error);
        result.point = point;
        return result;
    }
}

public class GameLeiSheBase
{
    //public static GamePoint[,] gamePoint = new GamePoint[Main.MAX_CH, Main.MAX_LED_ONE];

    //public static void Update_ColorFull (uint color, enPointSta sta) {
    //    for (int i = 0; i < Main.MAX_CH; i++) {
    //        for (int j = 0; j < Main.MAX_LED_ONE; j++) {
    //            gamePoint[i, j].color = color;
    //            gamePoint[i, j].statue = sta;
    //        }
    //    }
    //    Framebuffer.Update_ColorFull (color);
    //}
    //public static void Update_ColorFull (int ch, uint color, enPointSta sta) {
    //    for (int i = 0; i < Main.MAX_LED_ONE; i++) {
    //        gamePoint[ch, i].color = color;
    //        gamePoint[ch, i].statue = sta;
    //        Framebuffer.Update_PointColor (ch, i, color);
    //    }
    //    //Framebuffer.Update_ColorFull (color);
    //}
    //public static void Update_PointColor (int ch, int id, uint color, enPointSta sta) {
    //    if (ch >= Main.MAX_CH)
    //        return;
    //    if (id >= Main.MAX_LED_ONE)
    //        return;
    //    gamePoint[ch, id].color = color;
    //    gamePoint[ch, id].statue = sta;
    //    Framebuffer.Update_PointColor (ch, id, color);
    //}
    //public static void Update_PointColor (int ch, int x, int y, uint color, enPointSta sta) {
    //    //int id = x * Set.setVal.Height + y;
    //    if (x < 0 || y < 0)
    //        return;
    //    if (x >= Set.setVal.Width || y >= Set.setVal.Height)
    //        return;
    //    int id = y * Set.setVal.Width + x;
    //    id = Framebuffer.tab_Mapping[id];
    //    Update_PointColor (ch, id, color, sta);
    //}


    //public static void Clear () {
    //    for (int i = 0; i < Main.MAX_CH; i++) {
    //        for (int j = 0; j < Main.MAX_LED_ONE; j++) {
    //            gamePoint[i, j].bindCnt = 0;
    //            gamePoint[i, j].bindTime = 0;
    //        }
    //    }
    //}
    //public static void Check () {
    //    for (int i = 0; i < Main.MAX_CH; i++) {
    //        for (int j = 0; j < Main.MAX_LED_ONE; j++) {
    //            // 闪灯
    //            if (gamePoint[i, j].bindCnt > 0) {
    //                if (gamePoint[i, j].bindTime > 0) {
    //                    gamePoint[i, j].bindTime -= Time.deltaTime;
    //                } else {
    //                    gamePoint[i, j].bindCnt--;
    //                    gamePoint[i, j].bindTime = 0.2f;
    //                }
    //                if (gamePoint[i, j].bindCnt == 0) {
    //                    Framebuffer.Update_PointColor (i, j, gamePoint[i, j].color);
    //                } else if ((gamePoint[i, j].bindCnt % 2) == 0) {
    //                    Framebuffer.Update_PointColor (i, j, 0);
    //                } else {
    //                    Framebuffer.Update_PointColor (i, j, 0x1001);
    //                }
    //            }
    //        }
    //    }
    //}

    public const int MAX_LED_CH = 2;

    /// <summary>
    /// 游戏点数组（使用统一的 GamePoint 结构体）
    /// </summary>
    public static GamePoint[] gamePoint = new GamePoint[Main.MAX_LED_ONE * MAX_LED_CH];
    public static int[] tab_Point = new int[500];
    //    {0,31,32,63,64,
    //                                1,30,33,62,65,
    //                                2,29,34,61,66,
    //                                3,28,35,60,67,
    //                                4,27,36,59,68,
    //                                5,26,37,58,69,
    //                                6,25,38,57,70,
    //                                7,24,39,56,71,
    //                                8,23,40,55,72,
    //                                9,22,41,54,73,
    //                                10,21,42,53,74,
    //                                11,20,43,52,75,
    //                                12,19,44,51,76,
    //                                13,18,45,50,77,
    //                                14,17,46,49,78,
    //                                15,16,47,48,79,


    //};
    /// <summary>
    /// 目标LED数组（使用统一的 GamePoint 结构体）
    /// </summary>
    public static GamePoint[] targetLed = new GamePoint[Main.MAX_LED_ONE];

    public static void Update_ColorFull(uint color, enPointSta sta)//镭射灯全部变颜色
    {
        for (int i = 0; i < gamePoint.Length; i++)
        {
            gamePoint[i].color = color;
            gamePoint[i].statue = sta;
            Framebuffer.Update_TransmitLedColor(i, color, sta);
        }
    }
    public static void Update_PointColor(int id, uint color, enPointSta sta)
    {
        if (id >= gamePoint.Length)
            return;
        gamePoint[id].color = color;
        gamePoint[id].statue = sta;
        Framebuffer.Update_TransmitLedColor(id, color, sta);
    }
    public static void Update_PointColor(int x, int y, uint color, enPointSta sta)
    {
        if (x < 0 || y < 0)
            return;
        if (x >= Set.setVal.Width || y >= Set.setVal.Height)
            return;
        int id = y * Set.setVal.Width + x;
        id = Framebuffer.tab_Mapping[id];
        Update_PointColor(id, color, sta);
    }

    public static void Update_TargetLedColorAll(uint color, enPointSta sta)
    {
        for (int i = 0; i < targetLed.Length; i++)
        {
            targetLed[i].color = color;
            targetLed[i].statue = sta;
            Framebuffer.Update_TargetLedColor(i, color);
        }
    }
    public static void Update_TargetLedColor(int id, uint color, enPointSta sta)
    {
        if (id >= targetLed.Length)
            return;
        targetLed[id].color = color;
        targetLed[id].statue = sta;
        Framebuffer.Update_TargetLedColor(id, color);
    }


    public static void Clear()
    {
        for (int i = 0; i < gamePoint.Length; i++)
        {
            gamePoint[i].color = 0;
            gamePoint[i].statue = enPointSta.None;
            gamePoint[i].bindCnt = 0;
            gamePoint[i].bindTime = 0;
        }
    }
    public static void Check()
    {
        for (int i = 0; i < gamePoint.Length; i++)
        {
            // 闪灯
            if (gamePoint[i].bindCnt > 0)
            {
                if (gamePoint[i].bindTime > 0)
                {
                    gamePoint[i].bindTime -= Time.deltaTime;
                }
                else
                {
                    gamePoint[i].bindCnt--;
                    gamePoint[i].bindTime = 0.2f;
                }
                if (gamePoint[i].bindCnt == 0)
                {
                    Framebuffer.Update_TransmitLedColor(i, gamePoint[i].color, enPointSta.None);
                }
                else if ((gamePoint[i].bindCnt % 2) == 0)
                {
                    Framebuffer.Update_TransmitLedColor(i, 0, enPointSta.None);
                }
                else
                {
                    Framebuffer.Update_TransmitLedColor(i, 0x1001, enPointSta.None);
                }
            }
        }
    }
}
