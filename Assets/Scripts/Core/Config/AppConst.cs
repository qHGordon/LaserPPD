namespace LaserPPD.Core
{

/// <summary>[Core.Config] 全局游戏常量，集中管理原 Main 类中的 const 字段，供 Core 程序集内部使用。</summary>
public static class AppConst
{
    public const int MAX_CH       = 6;
    public const int MAX_LED_ONE  = 192;
    public const int MAX_LED      = MAX_LED_ONE * MAX_CH;
    public const int MAX_WALLLED  = 20;
    public const int MAX_PLAYER   = 2;
    public const int MAX_LEVEL    = 15;
    public const int MAX_ANIM     = 10;

    /// <summary>可用游戏场景 ID 列表（与 Main.tab_GameId 保持一致）。</summary>
    public static int[] tab_GameId = { 0 };

    /// <summary>IO 板连接版本号，由 Main 或 CmdIO 更新。</summary>
    public static int ioVersion = 0;

    /// <summary>接收数据计数，由 SaveIO.ReceiveLong 更新，UI 层读取。</summary>
    public static int receiveDataNum = 0;

    /// <summary>最近接收到的数据地址，由 SaveIO.ReceiveLong 更新。</summary>
    public static byte addr = 0;

    /// <summary>最近发送的读取地址，由 SaveIO.ReadLongStart 更新。</summary>
    public static byte readAddr = 0;

    /// <summary>投币音效回调，由 Main 在 Awake 中注册，Core 层触发。</summary>
    public static System.Action onCoinIn;
}

/// <summary>[Core.Config] 主流程状态枚举（原定义在 Main.cs，迁移至 Core 以解耦程序集依赖）。</summary>
public enum en_MainStatue
{
    Restart  = -1,
    Game_00  = 0,
    Game_01,
    Game_02,
    Game_03,
    Game_04,
    Game_05,
    Game_06,
    Game_07,
    Game_97  = 97,
    Game_98,
    LoadScene,
    Menu,
    Game,
}
}
