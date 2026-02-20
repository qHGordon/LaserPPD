using UnityEngine;

namespace LaserPPD.Core
{


/// <summary>
/// 基础游戏点结构体
/// 包含所有游戏点的共同字段：状态和颜色
/// </summary>
public struct BaseGamePoint
{
    /// <summary>
    /// 点的状态
    /// </summary>
    public enPointSta statue;

    /// <summary>
    /// 点的颜色值（uint格式）
    /// </summary>
    public uint color;

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public BaseGamePoint(enPointSta statue, uint color)
    {
        this.statue = statue;
        this.color = color;
    }

    /// <summary>
    /// 重置为默认值
    /// </summary>
    public void Reset()
    {
        statue = enPointSta.None;
        color = 0;
    }
}
}
