namespace LaserPPD.Core
{

/// <summary>
/// [Core.Led] LED 地图回调接口，用于解耦 LedAnim_Struts 与具体 Game_Map 的静态引用。
/// 由 Game_Map00、Game_Map06、Game_Map07 等实现，供 LedAnim_Struts_06/07/Wall 通过注入使用。
/// </summary>
public interface ILedMapCallback
{
    /// <summary>设置指定坐标的 LED 状态</summary>
    /// <param name="x">列坐标</param>
    /// <param name="y">行坐标</param>
    /// <param name="sta">目标状态</param>
    void ChangeLed_Sta(int x, int y, enPointSta sta);

    /// <summary>是否已清除所有目标点</summary>
    bool isClearAll { get; set; }

    /// <summary>剩余目标点数量（用于递减）</summary>
    int tarageNum { get; set; }
}
}
