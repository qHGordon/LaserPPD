namespace LaserPPD.Core
{

/// <summary>
/// [Core.Config] 游戏内设置目标注册表，解耦 SettingInGame 与具体 Game 的静态引用。
/// 当前激活的 Game 在启动时注册，SettingInGame 通过此处获取目标。
/// </summary>
public static class SettingInGameRegistry
{
    public static ISettingInGameTarget CurrentTarget { get; set; }
}
}
