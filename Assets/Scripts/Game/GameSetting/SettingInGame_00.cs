using UnityEngine;
using LaserPPD.Core;

/// <summary>
/// 将游戏00的设置定义为 ScriptableObject。
/// 这样可以在 Unity Inspector 中方便地配置游戏参数。
/// </summary>
[CreateAssetMenu(fileName = "Game00Settings", menuName = "Game Settings/Game 00 Settings")]
public class SettingInGame_00 : ScriptableObject
{
    [Header("配置选项")]
    [Tooltip("蓝色方块数量的可选值。")]
    /// <summary>
    /// 蓝色方块数量的可选值数组。
    /// </summary>
    [SerializeField] public int[] tab_BlueNum = { 20, 25, 30, 40, 50, 60, 70, 80, 90, 100 };

    [Tooltip("目标数量的可选值。")]
    /// <summary>
    /// 目标数量的可选值数组。
    /// </summary>
    [SerializeField] public int[] tab_TarageNum = { 20, 30, 40, 50, 60, 70, 80, 90, 100 };

    [Tooltip("玩家生命值的可选值。")]
    /// <summary>
    /// 玩家生命值的可选值数组。
    /// </summary>
    [SerializeField] public int[] tab_LifeNum = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

    [Tooltip("关卡时间的可选值（秒）。")]
    /// <summary>
    /// 关卡时间的可选值数组（秒）。
    /// </summary>
    [SerializeField] public int[] tab_LevelTime = { 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 150, 180, 210, 240, 270, 300, 330, 360, 390, 420, 450, 480, 510, 540, 570, 600 };

    [Tooltip("阶段时间的可选值（秒）。")]
    /// <summary>
    /// 阶段时间的可选值数组（秒）。
    /// </summary>
    [SerializeField] public int[] tab_JieDuanTime = { 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 150, 180 };

    [Tooltip("移动速度的可选值。")]
    /// <summary>
    /// 移动速度的可选值数组。
    /// </summary>
    [SerializeField] public int[] table_MoveSpeed = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    [Header("当前游戏设置")]
    [Tooltip("默认的蓝色方块数量。")]
    /// <summary>
    /// 默认的蓝色方块数量。
    /// </summary>
    public int set_BlueNum = 20;

    [Tooltip("默认的玩家生命值。")]
    /// <summary>
    /// 默认的玩家生命值。
    /// </summary>
    public int set_LifeNum = 10;

    [Tooltip("默认的关卡时间（秒）。")]
    /// <summary>
    /// 默认的关卡时间（秒）。
    /// </summary>
    public int set_LevelTime = 60;

    [Tooltip("不同阶段的默认移动速度。")]
    /// <summary>
    /// 不同阶段的默认移动速度。
    /// </summary>
    [SerializeField] public int[] set_MoveSpeed = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    [Tooltip("不同阶段的默认目标数量。")]
    /// <summary>
    /// 不同阶段的默认目标数量。
    /// </summary>
    [SerializeField] public int[] set_TarageNum = { 20, 30, 40, 50, 60, 70, 80, 90, 100 };
}