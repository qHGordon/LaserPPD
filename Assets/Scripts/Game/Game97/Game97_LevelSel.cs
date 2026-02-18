using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// [Game97] 关卡选择界面。
/// 管理关卡索引（MapIndex）到地图ID（MapID）的映射表，
/// 并在 Game97 Hub 中作为 GameSelect 状态对应的 UI 组件使用。
/// </summary>
public class Game97_LevelSel : MonoBehaviour
{
    public static Game97_LevelSel instance;

    /// <summary>
    /// 关卡索引到地图ID的映射表（可在 Inspector 中配置）。
    /// Level[MapIndex] 返回对应的 MapID，长度至少 30 以支持挑战模式。
    /// </summary>
    [Header("关卡映射配置 (Level[MapIndex] = MapID)")]
    public int[] Level = new int[30]
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
        10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
        20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
    };

    Game97_Main game97_Main;
    en_GameSelSta statue;
    float runTime;

    /// <summary>由 Game97_Main.Awake0 调用，完成初始化并注册单例。</summary>
    public void Awake0(Game97_Main game)
    {
        game97_Main = game;
        instance = this;
    }

    /// <summary>进入 GameSelect 状态时由 Game97_Main.ChangeStatue 调用。</summary>
    public void GameStart()
    {
        ChangeStatue(en_GameSelSta.Selecting);
    }

    void Update()
    {
        switch (statue)
        {
            case en_GameSelSta.EnterGame:
                runTime += Time.deltaTime;
                if (runTime >= 0.5f)
                {
                    game97_Main.EnterGame(0);
                }
                break;
        }
    }

    void ChangeStatue(en_GameSelSta sta)
    {
        statue = sta;
        runTime = 0;
        Key.Clear();
    }

    /// <summary>
    /// 选中指定关卡索引，同步更新 Main.MapIndex 与 Main.MapID。
    /// 在闯关/挑战模式中，由游戏主逻辑调用以切换到下一关。
    /// </summary>
    /// <param name="id">关卡索引（Level[] 数组下标，从 0 开始）</param>
    public void OnClick_Level(int id)
    {
        if (id < 0 || id >= Level.Length)
            return;
        Main.MapIndex = id;
        Main.MapID = Level[id];
    }
}
