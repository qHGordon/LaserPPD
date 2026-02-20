using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using LaserPPD.Core;

/// <summary>单个激光对象的配置，对应一次 New_LedOne 调用</summary>
[System.Serializable]
public class LaserEntry
{
    /// <summary>激光占据的坐标列表 [列x, 行y]，单点激光只需填1个元素</summary>
    public Vector2Int[] positions;
    /// <summary>激光运动模式</summary>
    public Game01_Ledone.en_Move_Type moveType;
}

/// <summary>单轮次（Round）的激光布局配置</summary>
[System.Serializable]
public class RoundConfig
{
    /// <summary>激光阶段1（阶段 X.R.2，currJieDuan=1）的激光布局</summary>
    public LaserEntry[] lasers_stage2;
    /// <summary>激光阶段2（阶段 X.R.4，currJieDuan=3）的激光布局</summary>
    public LaserEntry[] lasers_stage4;
    /// <summary>激光移动速率（对应 max_runtime），值越大移动越慢</summary>
    public float speed = 1.5f;
}

/// <summary>单关卡（Level）的全部轮次配置，固定 3 轮</summary>
[System.Serializable]
public class LevelConfig
{
    public RoundConfig[] rounds = new RoundConfig[3];
}

public class Game_Map01 : MonoBehaviour
{
    public bool isclearall = false;
    int maxJieduan = 4;
    public int Jieduan_Now = 0;
    public List<Game01_Ledone> list_LightPos = new List<Game01_Ledone>();

    private void Awake()
    {
        instance = this;
    }
    public static Game_Map01 instance;
    public Game01_Ledone obj_Led;
    public float max_runtime = 0;
    public float protectTime = 5;

    /// <summary>
    /// 全部关卡的轮次激光配置（10关 × 3轮），在 Inspector 中手动配置。
    /// 若某轮 lasers 为空，则自动回退使用硬编码的 Initmap_Maze_XX 函数。
    /// </summary>
    [Header("关卡轮次激光配置 (10关 × 3轮)")]
    public LevelConfig[] allLevelsConfig = new LevelConfig[10];

    public void Initmap()
    {
        protectTime = 5;

        for (int i = 0; i < list_LightPos.Count; i++)
        {
            Destroy(list_LightPos[i].gameObject);
        }
        isclearall = false;
        maxJieduan = 4;

        list_LightPos = new List<Game01_Ledone>();
        GameLeiSheLedControl.PrePic_Copy = new int[Set.setVal.Width * Set.setVal.Height];
        GameLeiSheLedControl.LedCoordinate();
        // 关卡开始时，初始化第 0 轮的激光配置
        Initmap_ForRound(0);
    }

    /// <summary>
    /// 按指定轮次和阶段初始化激光对象。
    /// 若 allLevelsConfig 中对应位置有配置则从配置读取；
    /// 否则仅 stage2（isStage4=false）时回退到硬编码迷宫函数。
    /// </summary>
    /// <param name="round">当前轮次（0-indexed，共 maxRound 轮）</param>
    /// <param name="isStage4">false=激光阶段1(X.R.2，currJieDuan=1)；true=激光阶段2(X.R.4，currJieDuan=3)</param>
    public void Initmap_ForRound(int round, bool isStage4 = false)
    {
        // 清除现有激光对象
        for (int i = 0; i < list_LightPos.Count; i++)
            Destroy(list_LightPos[i].gameObject);
        list_LightPos.Clear();
        isclearall = false;

        int level = Game01_Main.instance.gameLevel;

        // 按阶段选取对应的激光配置数组
        LaserEntry[] source = null;
        if (allLevelsConfig != null
            && level < allLevelsConfig.Length
            && allLevelsConfig[level] != null
            && allLevelsConfig[level].rounds != null
            && round < allLevelsConfig[level].rounds.Length
            && allLevelsConfig[level].rounds[round] != null)
        {
            RoundConfig cfg = allLevelsConfig[level].rounds[round];
            LaserEntry[] candidate = isStage4 ? cfg.lasers_stage4 : cfg.lasers_stage2;
            if (candidate != null && candidate.Length > 0)
            {
                source = candidate;
                max_runtime = cfg.speed;
            }
        }

        if (source != null)
        {
            // 【Inspector配置路径】从 lasers_stage2 或 lasers_stage4 读取激光布局
            foreach (LaserEntry laser in source)
            {
                if (laser.positions == null || laser.positions.Length == 0)
                    continue;
                List<Vector2Int> pos = new List<Vector2Int>(laser.positions);
                // 坐标映射：positions[i].x = 列x，positions[i].y = 行y
                if (pos.Count == 1)
                    New_LedOne(pos[0].x, pos[0].y, 0, laser.moveType);
                else
                    New_LedOne(pos, 0, laser.moveType);
            }
        }
        else if (!isStage4)
        {
            // 【回退路径】stage2 无配置时使用硬编码迷宫函数（仅 round=0 推荐）
            Invoke("Initmap_Maze_" + (Main.MapIndex % 12).ToString("D2"), 0);
        }
    }

    /// <summary>
    /// 录入关卡1（Level 0）的默认激光坐标配置。
    /// 在 Unity Inspector 右键 GameObject 选择 "Init Default Laser Data" 调用。
    /// </summary>
    [ContextMenu("Init Default Laser Data")]
    public void InitDefaultLaserData()
    {
        // 安全实例化全部层级，避免 NullReferenceException
        if (allLevelsConfig == null || allLevelsConfig.Length < 10)
            allLevelsConfig = new LevelConfig[10];
        for (int i = 0; i < allLevelsConfig.Length; i++)
        {
            if (allLevelsConfig[i] == null)
                allLevelsConfig[i] = new LevelConfig();
            if (allLevelsConfig[i].rounds == null || allLevelsConfig[i].rounds.Length < 3)
                allLevelsConfig[i].rounds = new RoundConfig[3];
            for (int r = 0; r < 3; r++)
                if (allLevelsConfig[i].rounds[r] == null)
                    allLevelsConfig[i].rounds[r] = new RoundConfig();
        }

        // ── 关卡1（index=0）三轮六组激光坐标 ────────────────────────────────

        // 1.1.2：轮次1，激光阶段1
        allLevelsConfig[0].rounds[0].lasers_stage2 = new LaserEntry[]
        {
            new LaserEntry
            {
                positions = new Vector2Int[] { V(2,1), V(10,1), V(6,4), V(14,4), V(6,5), V(14,5) },
                moveType = Game01_Ledone.en_Move_Type.Stay
            }
        };
        // 1.1.4：轮次1，激光阶段2
        allLevelsConfig[0].rounds[0].lasers_stage4 = new LaserEntry[]
        {
            new LaserEntry
            {
                positions = new Vector2Int[] { V(2,1), V(5,1), V(2,2), V(5,2), V(8,4), V(11,4), V(14,4) },
                moveType = Game01_Ledone.en_Move_Type.Stay
            }
        };

        // 1.2.2：轮次2，激光阶段1
        allLevelsConfig[0].rounds[1].lasers_stage2 = new LaserEntry[]
        {
            new LaserEntry
            {
                positions = new Vector2Int[] { V(12,1), V(13,1), V(2,4), V(8,4), V(2,5), V(3,5), V(7,5), V(8,5), V(16,5) },
                moveType = Game01_Ledone.en_Move_Type.Stay
            }
        };
        // 1.2.4：轮次2，激光阶段2
        allLevelsConfig[0].rounds[1].lasers_stage4 = new LaserEntry[]
        {
            new LaserEntry
            {
                positions = new Vector2Int[] { V(7,1), V(15,3), V(10,4), V(1,5), V(4,5), V(7,5) },
                moveType = Game01_Ledone.en_Move_Type.Stay
            }
        };

        // 1.3.2：轮次3，激光阶段1
        allLevelsConfig[0].rounds[2].lasers_stage2 = new LaserEntry[]
        {
            new LaserEntry
            {
                positions = new Vector2Int[] { V(7,1), V(10,1), V(13,1), V(14,1), V(16,1), V(7,2), V(4,4), V(1,5), V(4,5), V(10,5), V(16,5) },
                moveType = Game01_Ledone.en_Move_Type.Stay
            }
        };
        // 1.3.4：轮次3，激光阶段2
        allLevelsConfig[0].rounds[2].lasers_stage4 = new LaserEntry[]
        {
            new LaserEntry
            {
                positions = new Vector2Int[] { V(1,1), V(4,1), V(12,1), V(7,3), V(8,3), V(9,3), V(15,3), V(16,3), V(1,5), V(4,5), V(12,5) },
                moveType = Game01_Ledone.en_Move_Type.Stay
            }
        };

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("InitDefaultLaserData: 关卡1 全部6组激光配置写入完成。");
#endif
    }

    /// <summary>坐标构造简写辅助方法，x=列，y=行</summary>
    static Vector2Int V(int x, int y) => new Vector2Int(x, y);

    Game01_Ledone New_LedOne(int x, int y, int dir, Game01_Ledone.en_Move_Type move)
    {
        if (x < 0 || x >= Set.setVal.Width || y < 0 || y >= Set.setVal.Height)
        {
            return null;
        }
        Game01_Ledone one = Instantiate(obj_Led, this.transform);
        one.Init(x, y, dir, move);

        list_LightPos.Add(one);
        return one;

    }
    Game01_Ledone New_LedOne(List<Vector2Int> _pos, int dir, Game01_Ledone.en_Move_Type move)
    {
        Game01_Ledone one = Instantiate(obj_Led, this.transform);
        one.Init(_pos, dir, move);

        list_LightPos.Add(one);
        return one;
    }
    void Initmap_Maze_00()
    {
        // ─── Maze_00 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 基础层（全轮次有效）：
        //   Stay: (5,0), (W-2,3), (7,H-2), (9,H-2), (W/2,3)
        // 扩展层 case 1（Jieduan_Now=1，当前为预留）：
        //   OneWay: [(0..2,H-1),(0..2,H-2)]  [(W/2..W/2+2,H-1),(W/2..W/2+2,H-2)]
        //   Stay:   (W/4,0), (3W/4,0)
        // 扩展层 case 2（Jieduan_Now=2，当前为预留）：
        //   Stay:   [x:2..W-3, y:2..H-2]（内矩形区域）
        // 扩展层 case 3（Jieduan_Now=3，当前为预留）：
        //   Stay:   [x:0..W-1, y:3..H-2]（横向宽带）
        //   Shake:  [(0..1,0..2), (W-2..W-1,0..2)]（左右下角各2列×3行）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        //     New_LedOne(0, 0, 0, 0);
        //     New_LedOne(2, 0, 0, Game01_Ledone.en_Move_Type.OneWay);
        New_LedOne(5, 0, 0, 0);
        New_LedOne(Set.setVal.Width - 2, 3, 0, 0);
        New_LedOne(7, Set.setVal.Height - 2, 0, 0);
        New_LedOne(9, Set.setVal.Height - 2, 0, 0);
        New_LedOne(Set.setVal.Width / 2, 3, 0, 0);

        switch (Jieduan_Now)
        {

            case 1:
                List<Vector2Int> pos1 = new List<Vector2Int>();
                ;
                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                        pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(pos1, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
                New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);


                break;
            case 2:
                pos = new List<Vector2Int>();
                for (int i = 2; i < Set.setVal.Width - 2; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                break;
            case 3:
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 3; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                pos = new List<Vector2Int>();

                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                        pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);

                break;
        }

    }
    void Initmap_Maze_01()
    {
        // ─── Maze_01 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 基础层（全轮次有效）：
        //   Stay: [(0..2,H-1),(0..2,H-2)]（左侧3×2块）
        //   Stay: [(W/2..W/2+2,H-1),(W/2..W/2+2,H-2)]（右侧3×2块）
        //   Stay: (W/4,0), (3W/4,0)
        // 扩展层 case 1（Jieduan_Now=1，当前为预留）：
        //   OneWay: 同左右两块 + Stay (W/4,0), (3W/4,0)
        // 扩展层 case 2（Jieduan_Now=2，当前为预留）：
        //   Stay:   [x:2..W-3, y:2..H-2]（内矩形区域）
        // 扩展层 case 3（Jieduan_Now=3，当前为预留）：
        //   Stay:   [x:0..W-1, y:3..H-2]  Shake: [(0..1,0..2), (W-2..W-1,0..2)]
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        List<Vector2Int> pos1 = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

            }
        }
        New_LedOne(pos, 0, 0);
        New_LedOne(pos1, 0, 0);
        New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
        New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);

        switch (Jieduan_Now)
        {

            case 1:

                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                        pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(pos1, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
                New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);


                break;
            case 2:
                pos = new List<Vector2Int>();
                for (int i = 2; i < Set.setVal.Width - 2; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                break;
            case 3:
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 3; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                pos = new List<Vector2Int>();

                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                        pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);

                break;
        }

    }
    void Initmap_Maze_02()
    {
        // ─── Maze_02 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 基础层（全轮次有效）：
        //   OneWay: [(0..2,H-1),(0..2,H-2)]（左侧3×2块，向上移动）
        //   OneWay: [(W/2..W/2+2,H-1),(W/2..W/2+2,H-2)]（右侧3×2块，向上移动）
        //   Stay:   (W/4,0), (3W/4,0)
        // 扩展层 case 1（Jieduan_Now=1，当前为预留，空）
        // 扩展层 case 2（Jieduan_Now=2，当前为预留）：
        //   Stay:   [x:2..W-3, y:2..H-2]（内矩形区域）
        // 扩展层 case 3（Jieduan_Now=3，当前为预留）：
        //   Stay:   [x:0..W-1, y:3..H-2]  Shake: [(0..1,0..2), (W-2..W-1,0..2)]
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        List<Vector2Int> pos1 = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

            }
        }
        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
        New_LedOne(pos1, 0, Game01_Ledone.en_Move_Type.OneWay);
        New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
        New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);

        switch (Jieduan_Now)
        {

            case 1:




                break;
            case 2:
                pos = new List<Vector2Int>();
                for (int i = 2; i < Set.setVal.Width - 2; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                break;
            case 3:
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 3; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                pos = new List<Vector2Int>();

                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                        pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);

                break;
        }

    }
    void Initmap_Maze_03()
    {
        // ─── Maze_03 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 无 switch 扩展，全轮次固定布局：
        //   Stay:  [(0..2,H-1),(0..2,H-2)]（左下角3×2）+ [x:0..W-1, y:3..H-2]（横向宽带）
        //   Shake: [(0..1,0..2), (W-2..W-1,0..2)]（左右各2列×3行的角落区域）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        List<Vector2Int> pos1 = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

            }
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 3; k < Set.setVal.Height - 1; k++)
            {
                pos.Add(new Vector2Int(i, k));
            }
        }
        New_LedOne(pos, 0, 0);
        pos = new List<Vector2Int>();

        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                pos.Add(new Vector2Int(i, k));
                pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
            }
        }
        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);


    }
    void Initmap_Maze_04()
    {
        // ─── Maze_04 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 无 switch 扩展，全轮次固定布局：
        //   Stay:   [x:0..W-1, y:H-2..H-1]（最后2行全宽横条）
        //   OneWay: (0,0), (7,0), (14,0), (21,0), ...（每7列一个点，y=0处，向下移动）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));

            }
        }
        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Stay);
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            if (i % 7 == 0)
            {
                New_LedOne(i, 0, 0, Game01_Ledone.en_Move_Type.OneWay);

            }
        }

    }
    void Initmap_Maze_05()
    {
        // ─── Maze_05 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 无 switch 扩展，全轮次固定布局：
        //   Stay:   [x:0,8,16,...（每8列）, y:0..2]（竖短柱3行）
        //   OneWay: [(W-3..W-1, H-2..H-1)]（右上角3×2块，向上移动）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        pos = new List<Vector2Int>();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            if (i % 8 == 0)
            {
                for (int k = 0; k < 3; k++)
                {
                    pos.Add(new Vector2Int(i, k));

                }

            }
        }

        New_LedOne(pos, 0, 0);
        pos = new List<Vector2Int>();
        for (int i = Set.setVal.Width - 3; i < Set.setVal.Width; i++)
        {

            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));

            }

        }

        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);

    }
    void Initmap_Maze_06()
    {
        // ─── Maze_06 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 无 switch 扩展，全轮次固定布局（速度较快 speed=0.5）：
        //   OneWay: [x:0,8,16,...（每8列）, y:0..H-1]（全高竖条，向下快速移动）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                if (i % 8 == 0)
                {
                    pos.Add(new Vector2Int(i, k));

                }
            }
        }

        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
        max_runtime = 0.5f;
    }
    void Initmap_Maze_07()
    {
        // ─── Maze_07 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 无 switch 扩展，全轮次固定布局（速度 speed=1.0）：
        //   Stay:   [x:0..W-1, y:H-3..H-1]（最后3行全宽横墙，下压式，donw_Y=H-3）
        //   OneWay: (W/2-6,0),(W/2-4,0),(W/2-2,0),(W/2,0),(W/2,0),(W/2+2,0),(W/2+4,0),(W/2+6,0)
        //           （8个点从中心向两侧展开，y=0处，注：i=0时中心点重复创建）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        Game01_Ledone _one;
        pos = new List<Vector2Int>();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = Set.setVal.Height - 3; k < Set.setVal.Height; k++)
            {
                pos.Add(new Vector2Int(i, k));

            }
        }


        _one = New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Stay);
        _one.donw_Y = Set.setVal.Height - 3;
        for (int i = 0; i < 4; i++)
        {
            New_LedOne(Set.setVal.Width / 2 - i * 2, 0, 0, Game01_Ledone.en_Move_Type.OneWay);
            New_LedOne(Set.setVal.Width / 2 + i * 2, 0, 0, Game01_Ledone.en_Move_Type.OneWay);

        }
        max_runtime = 1f;

    }
    void Initmap_Maze_09()
    {
        // ─── Maze_09 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 无 switch 扩展，全轮次固定布局（速度较慢 speed=2.0）：
        //   Shake:  [x:0..W-1, y:H-3..H-1]（最后3行全宽）
        //           + 3列全高竖线 x=0, x=W/2, x=W-1（贯穿全高，与顶部3行重叠）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        Game01_Ledone _one;
        pos = new List<Vector2Int>();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = Set.setVal.Height - 3; k < Set.setVal.Height; k++)
            {
                pos.Add(new Vector2Int(i, k));

            }
        }
        for (int k = 0; k < Set.setVal.Height; k++)
        {
            pos.Add(new Vector2Int(0, k));
            pos.Add(new Vector2Int(Set.setVal.Width / 2, k));
            pos.Add(new Vector2Int(Set.setVal.Width - 1, k));

        }
        _one = New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);
        max_runtime = 2;

    }
    void Initmap_Maze_08()
    {
        // ─── Maze_08 激光布局 (阶段 X.R.2 与 X.R.4，W=Width，H=Height) ───
        // 无 switch 扩展，全轮次固定布局：
        //   OneWay: 3个3×3分块（最后3行）：
        //           [x:0..2, y:H-3..H-1] + [x:8..10, y:H-3..H-1] + [x:16..18, y:H-3..H-1]
        //           （向上移动）
        //   Stay:   (0,0), (8,0), (16,0)（每组对应的y=0起始单点）
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        Game01_Ledone _one;
        for (int n = 0; n < 3; n++)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int k = Set.setVal.Height - 3; k < Set.setVal.Height; k++)
                {

                    pos.Add(new Vector2Int(n * 8 + i, k));

                }
            }
        }
        _one = New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);

        for (int i = 0; i < 3; i++)
        {
            New_LedOne(0 + i * 8, 0, 0, 0);

        }
    }
    float runtime = 0;
    int startId = LedKey.GetLeiSheKeyStartId();
    private void Update()
    {
        if (Game01_Main.instance.player.statue != en_Player01Sta.Play)
        {
            return;
        }
        if (protectTime > 0)
        {
            protectTime -= Time.deltaTime;
        }
        if (Game01_Main.instance.player.currJieDuan== 0 || Game01_Main.instance.player.currJieDuan == 2)
        {
            if (Game01_Main.instance.player.statue==en_Player01Sta.Play)
            {
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 0; k < Set.setVal.Height; k++)
                    {
                        Framebuffer.Update_TransmitLedColor(i, k, 255, enPointSta.Target);
                        if (i == 0)
                        {
                            if (LedKey.KeyStatus(startId + Framebuffer.MappingId(i, k)) == false)
                            {
                                if (protectTime <= 0)
                                {
                                    MusicManager.instance.Play_Fails();
                                    protectTime = 1;
                                    if (FjData.g_Fj[0].Life > 0)
                                    {
                                        Debug.LogError("地面扣血坐标：" +i + "  " + k);

                                        FjData.g_Fj[0].Life--;
                                    }
                                }
                            }
                        }
                     
                    }
                    
                }

            }
        }
        runtime += Time.deltaTime;
        if (runtime > 0.02f)
        {
            runtime = 0;
            Framebuffer.Update_TransmitLedClearAll();
        }


    }
}
