using UnityEngine;

namespace LaserPPD.Core
{


/// <summary>
/// [Core.Config] 游戏内设置目标接口，用于解耦 SettingInGame 与具体 Game_Map、Game_Main 的静态引用。
/// 由 Game00_Main、Game02_Main 等实现，在游戏激活时注册到 SettingInGameRegistry。
/// </summary>
public interface ISettingInGameTarget
{
    /// <summary>预设图层的父级 GameObject，用于显示/隐藏对照图</summary>
    GameObject PresetPicLayerParent { get; }

    /// <summary>下一阶段：重置 TarageNum_now，根据 MapID 执行对应 Init 或设置 isClearAll</summary>
    void OnSettingNextLevel();

    /// <summary>切换预设图显示状态</summary>
    void TogglePresetPicShow();
}
}
