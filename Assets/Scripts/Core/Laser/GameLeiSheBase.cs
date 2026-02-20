using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{

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

/// <summary>[Core.Laser] 镭射游戏点阵基础逻辑：管理通道点位状态、颜色同步与闪灯检测。</summary>
public class GameLeiSheBase
{
    public const int MAX_LED_CH = 2;

    /// <summary>
    /// 游戏点数组（使用统一的 GamePoint 结构体）
    /// </summary>
    public static GamePoint[] gamePoint = new GamePoint[AppConst.MAX_LED_ONE * MAX_LED_CH];
    public static int[] tab_Point = new int[500];

    /// <summary>
    /// 目标LED数组（使用统一的 GamePoint 结构体）
    /// </summary>
    public static GamePoint[] targetLed = new GamePoint[AppConst.MAX_LED_ONE];

    public static void Update_ColorFull(uint color, enPointSta sta)
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

}
