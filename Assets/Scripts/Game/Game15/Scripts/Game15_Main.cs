using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 测试窗口
/// </summary>
public class TestingNoSkWin : BaseWin
{
    public bool GetIsOpen
    {
        get
        {
            return IsOpen;
        }
    }

    public TestingNoSkWin(Transform root, Button graph_item_btn, Button open_btn, Dictionary<Vector2Int, ColorTable.ColorType> color_dic, Action open_event, Action<Vector2Int> on_click_graph_item_btn, Func<int> get_width, Func<int> get_height) : base(root, open_event, () => { })
    {
        Text open_btn_text = open_btn.transform.GetChild(0).GetComponent<Text>();
        open_btn_text.text = "点击打开";
        open_btn.onClick.AddListener(() =>
        {
            if (IsOpen) Close();
            else Open();
            open_btn_text.text = !IsOpen ? "点击打开" : "点击关闭";
        });

        GraphItemBtn = graph_item_btn;
        GraphItemBtnRect = GraphItemBtn.GetComponent<RectTransform>();
        OnClickGraphItemBtn = on_click_graph_item_btn;
        GetWidth = get_width;
        GetHeight = get_height;
        ColorDic = color_dic;
        Close();
    }

    /// <summary>
    /// 打开测试时创造按钮同时更新和添加事件
    /// </summary>
    public override void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        Vector3 graph_btn_pos = GraphItemBtn.transform.localPosition;
        for (int x = 0; x < GetWidth(); x++)
        {
            for (int y = 0; y < GetHeight(); y++)
            {
                int xpos = x, ypos = y;
                Button btn = GameObject.Instantiate(GraphItemBtn, Root);
                btn.name = "Btn_Game13_GraphItemBtn" + " (" + x.ToString() + ',' + y.ToString() + ')';
                btn.transform.localPosition = graph_btn_pos + new Vector3(GraphItemBtnRect.rect.width * x, GraphItemBtnRect.rect.height * y, 0);
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnClickGraphItemBtn(new Vector2Int(xpos, ypos)));
                GraphItemBtnDic[new Vector2Int(x, y)] = btn;
            }
        }
        base.Open();

    }

    public override void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        foreach (var item in GraphItemBtnDic)
        {
            GameObject.Destroy(item.Value.gameObject);
        }
        GraphItemBtnDic.Clear();
        base.Close();
    }

    public override void Update()
    {
        base.Update();
        for (int y = 0; y < GetHeight(); y++)
        {
            for (int x = 0; x < GetWidth(); x++)
            {
                Vector2Int vec = new Vector2Int(x, y);
                if (!GraphItemBtnDic.ContainsKey(vec)) continue;
                if (!ColorDic.ContainsKey(vec)) continue;
                Vector4 vec4 = ColorTable.GetRGBA(ColorDic[vec]);
                GraphItemBtnDic[new Vector2Int(x, y)].image.color = new Color(vec4.x, vec4.y, vec4.z, vec4.w);
            }
        }
    }

    protected bool IsOpen = true;

    protected Button GraphItemBtn;

    protected RectTransform GraphItemBtnRect;

    protected Dictionary<Vector2Int, Button> GraphItemBtnDic = new Dictionary<Vector2Int, Button>();

    protected Dictionary<Vector2Int, ColorTable.ColorType> ColorDic = new Dictionary<Vector2Int, ColorTable.ColorType>();

    protected Action<Vector2Int> OnClickGraphItemBtn = (Vector2Int vec2) => { };

    protected Func<int> GetWidth = () => 0, GetHeight = () => 0;
}

public class Game15_ScoresWin : BaseWin
{
    public Game15_ScoresWin(Transform root, Image[] score_imgs, Sprite[] num_sprites, Func<int>[] scores) : base(root, () => { }, () => { })
    {
        ScoreImgs = score_imgs;
        NumSprites = num_sprites;
        Scores = scores;
    }

    public override void Update()
    {
        for (int i = 0; i < Mathf.Min(Mathf.Min(ScoreImgs.Length, Scores.Length), NumSprites.Length); i++)
        {
            ScoreImgs[i].sprite = NumSprites[Scores[i]()];
        }
    }

    protected Image[] ScoreImgs;

    protected Sprite[] NumSprites;

    protected Func<int>[] Scores;
}

public class Game15_Main : MonoBehaviour
{

    #region 测试窗口
    [Header("[测试窗口]")]
    public Transform TestingWinRoot;

    public Button TestingWinGraphItemBtn, TestingWinOpenBtn;
    #endregion

    #region 提示窗口
    [Space(1)]
    [Header("[提示窗口]")]
    public Transform TipsWinRoot;

    public Button TipsWinConfirmBtn, TipsWinCancelBtn;

    public Text TipsWinText;
    #endregion

    #region 得分窗口
    [Space(1)]
    [Header("得分窗口")]
    public Transform ScoresWinRoot;

    public Image[] ScoresWinScoreImgs;

    public Sprite[] ScoresWinNumSprites;
    #endregion

    #region 音乐相关
    [Space(1)]
    [Header("音乐相关")]
    public AudioClip ChangeClip;

    public AudioClip CollisionClip;

    public AudioClip BgmClip;

    public AudioClip[] ReadyClip = new AudioClip[2];
    #endregion

    [Space(10)]
    public Button BackBtn;

    public void Awake0(Main main, int id)
    {

    }

    public void GameStart()
    {
        GameSt = GameState.Ready;
        ReadyTime = 4f;
    }

    // 地图最大值
    protected int XMax = 0, YMax = 0;

    protected int[] Scores = new int[2];

    // 游戏更新时间
    protected float UpdateTime = 0.5f, MaxUpdateTime = 0.5f;

    // 游戏准备时间
    protected float ReadyTime = 3f;

    // 游戏下一关倒计时
    protected float NextLevelTime = 5f;

    // UI文本
    protected string BackLabel = "", NextLevelLabel = "";

    // 音乐
    protected AudioSource AudioSource;

    // 球
    protected Vector2Int[] Ball = new Vector2Int[5];

    // 球的运动方向，四种可能方向分别为 (-1, 1), (1, 1), (1, -1), (-1, -1), 都能将原向量转化成 Vector3 并与 (0, 0, -1) 叉积得到 
    protected Vector2Int Dir = new Vector2Int(-1, -1);

    // 平台, 0 为下方玩家, 1为上方玩家
    protected Vector2Int[,] Platforms = new Vector2Int[2, 6];

    // 判断是否包含边界的字典
    protected Dictionary<Vector2Int, bool> BorderDic = new Dictionary<Vector2Int, bool>();

    // 测试窗口
    protected TestingNoSkWin TestingWin;

    // 提示窗口
    protected TipsWin TipsWin;

    // 得分窗口
    protected Game15_ScoresWin ScoresWin;

    // 用于记录地图上颜色的字典栈
    protected Dictionary<Vector2Int, ColorTable.ColorType> ColorMapDic = new Dictionary<Vector2Int, ColorTable.ColorType>();

    // 游戏状态
    protected GameState GameSt = GameState.Idle;

    protected enum GameState
    {
        Ready,
        Idle,
        Playing,
        End,
        Result
    }

    protected void Init()
    {
        XMax = Set.setVal.Width;
        YMax = Set.setVal.Height;

        for (int x = 0; x < XMax; x++)
        {
            for (int y = 0; y < YMax; y++)
            {
                DrawItem(new Vector2Int(x, y), ColorTable.ColorType.Color_Black);
            }
        }

        /* 假设对于一张 10 * 10 的图：
		 * 0 0 0 0 0 0 0 0 0 0
		 * 1 0 0 0 0 0 0 0 0 1
		 * 1 0 0 0 0 0 0 0 0 1
		 * 1 0 0 0 0 0 0 0 0 1
		 * 1 0 0 0 0 0 0 0 0 1
		 * 1 0 0 0 0 0 0 0 0 1
		 * 1 0 0 0 0 0 0 0 0 1
		 * 1 0 0 0 0 0 0 0 0 1
		 * 1 0 0 0 0 0 0 0 0 1
		 * 0 0 0 0 0 0 0 0 0 0
		 * 图中 1 的地方即为球可能反弹的地方
		 */
        BorderDic.Clear();
        for (int y = 1; y < YMax - 1; y++)
        {
            BorderDic[new Vector2Int(0, y)] = true;
            BorderDic[new Vector2Int(XMax - 1, y)] = true;
        }

        // 生成球
        for (int i = 4, x = XMax / 2 + 1, y = YMax - 3; i >= 0; x--, y--, i--)
        {
            Ball[i] = new Vector2Int(x, y);
            DrawItem(Ball[i], ColorTable.ColorType.Color_Green);
        }

        // 生成平台
        for (int i = 0, y = 1, d_vec = 1; i < 2; i++, y = YMax - y - 1, d_vec *= -1)
        {
            for (int j = 0; j < 4; j++)
            {
                Platforms[i, j] = new Vector2Int(j, y);
                DrawItem(Platforms[i, j], ColorTable.ColorType.Color_Red);

                if (j < 1 || j > 2) continue;
                Platforms[i, j + 3] = new Vector2Int(j, y - d_vec);
                DrawItem(Platforms[i, j + 3], ColorTable.ColorType.Color_Red);
            }
        }

        TestingWin = new TestingNoSkWin(TestingWinRoot, TestingWinGraphItemBtn, TestingWinOpenBtn, ColorMapDic, () => { }, (Vector2Int vec) =>
        {
            if (vec.y >= 2 && vec.y < YMax - 2) return;
            int idx = vec.y < 2 ? 0 : 1;
            for (int x = 0; x < XMax; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    DrawItem(new Vector2Int(x, idx != 0 ? YMax - y - 1 : y), ColorTable.ColorType.Color_Black);
                }
            }
            for (int i = 0, p_x = vec.x < XMax - 4 ? vec.x : XMax - 4, p_y = idx == 0 ? 1 : YMax - 2; i < 4; i++, p_x++)
            {
                Platforms[idx, i] = new Vector2Int(p_x, p_y);
                DrawItem(Platforms[idx, i], ColorTable.ColorType.Color_Red);

                if (i < 1 || i > 2) continue;
                Platforms[idx, i + 3] = new Vector2Int(p_x, p_y + (idx == 0 ? -1 : 1));
                DrawItem(Platforms[idx, i + 3], ColorTable.ColorType.Color_Red);
            }

        }, () => XMax, () => YMax);

        UpdateTime = MaxUpdateTime;
    }

    /// <summary>
    /// 下一步需要移动的位置
    /// </summary>
    /// <returns></returns>
    protected Vector2Int NextStepPos()
    {
        bool is_collision = false;
        if (BorderDic.ContainsKey(Ball[0]) && !BorderDic.ContainsKey(Ball[1]))
        {
            is_collision = true;
            Dir.x *= -1;
        }
        for (int i = 0, d = 1; i < 2; i++, d *= -1)
        {
            bool is_break = false;
            for (int j = 0; j < 4; j++)
            {
                if (Ball[0] != Platforms[i, j] + new Vector2Int(0, d)) continue;
                Dir.y *= -1;
                is_break = is_collision = true;
                break;
            }
            if (is_break) break;
        }
        if (is_collision)
        {
            // 碰撞音乐
            AudioSource.PlayOneShot(CollisionClip);
        }

        Vector2Int res_vec = BorderDic.ContainsKey(Ball[0]) && !BorderDic.ContainsKey(Ball[1]) ? new Vector2Int(0, Dir.y) : Dir;
        return res_vec;
    }

    protected void CheckLedKey()
    {
        for (int x = 0; x < XMax; x++)
        {
            for (int y = 2; y < YMax - 2; y++)
            {
                DrawItem(new Vector2Int(x, y), ColorTable.ColorType.Color_Black);
            }
        }
        for (int x = 0; x < XMax; x++)
        {
            /*int idx = -1, */
            int idx1 = -1, idx2 = -1;

            for (int i = 0, y = 1, d_vec = 1; i < 2; i++, y = YMax - y - 1, d_vec *= -1)
            {
                int id1 = Framebuffer.tab_Mapping[y * XMax + x], id2 = Framebuffer.tab_Mapping[(y - d_vec) * XMax + x];
                if (!LedKey.KeyStatus(id1) && !LedKey.KeyStatus(Framebuffer.tab_Mapping[id2])) continue;
                // 踩到平台的音乐
                AudioSource.PlayOneShot(ChangeClip);
                if (y <= 1) idx1 = 0;
                if (y > 1) idx2 = 1;
            }
            if (idx1 != -1)
            {
                for (int pos_x = 0; pos_x < XMax; pos_x++)
                {
                    for (int pos_y = 0; pos_y < 2; pos_y++)
                    {
                        DrawItem(new Vector2Int(pos_x, pos_y), ColorTable.ColorType.Color_Black);
                    }
                }
                for (int i = 0, p_x = x < XMax - 4 ? x : XMax - 4, p_y = 1; i < 4; i++, p_x++)
                {
                    Platforms[idx1, i] = new Vector2Int(p_x, p_y);
                    DrawItem(Platforms[idx1, i], ColorTable.ColorType.Color_Red);

                    if (i < 1 || i > 2) continue;
                    Platforms[idx1, i + 3] = new Vector2Int(p_x, p_y - 1);
                    DrawItem(Platforms[idx1, i + 3], ColorTable.ColorType.Color_Red);
                }
            }

            if (idx2 != -1)
            {
                for (int pos_x = 0; pos_x < XMax; pos_x++)
                {
                    for (int pos_y = 0; pos_y < 2; pos_y++)
                    {
                        DrawItem(new Vector2Int(pos_x, YMax - pos_y - 1), ColorTable.ColorType.Color_Black);
                    }
                }
                for (int i = 0, p_x = x < XMax - 4 ? x : XMax - 4, p_y = YMax - 2; i < 4; i++, p_x++)
                {
                    Platforms[idx2, i] = new Vector2Int(p_x, p_y);
                    DrawItem(Platforms[idx2, i], ColorTable.ColorType.Color_Red);

                    if (i < 1 || i > 2) continue;
                    Platforms[idx2, i + 3] = new Vector2Int(p_x, p_y + 1);
                    DrawItem(Platforms[idx2, i + 3], ColorTable.ColorType.Color_Red);
                }
            }
        }
    }

    void DrawItem(Vector2Int vec, ColorTable.ColorType color)
    {
        DrawPic.DrawPointId(Framebuffer.tab_Mapping[vec.y * XMax + vec.x], (uint)color, enPointSta.None);
        ColorMapDic[vec] = color;
    }

    protected void OnClickBackMenu()
    {
        Time.timeScale = 1;
        TipsWin.Close();
        Main.instance.ChangeStatue(en_MainStatue.Game_97);
        Main.instance.game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
    }

    protected void OnClickInNextLevel()
    {
        TipsWin.Close();


        Main.MapIndex = 9;// (++Main.MapIndex - 100) % 5 + 100;
        Game97_Main.instance.EnterGame( Main.MapIndex);
    }

    // Use this for initialization
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();

        TipsWin = new TipsWin(TipsWinRoot, TipsWinConfirmBtn, TipsWinCancelBtn, TipsWinText);

        ScoresWin = new Game15_ScoresWin(ScoresWinRoot, ScoresWinScoreImgs, ScoresWinNumSprites, new Func<int>[] { () => Scores[0], () => Scores[1] });

        BackLabel = Set.setVal.Language == (int)en_Language.Chinese ? "是否返回" : "Do you want to back";

        BackBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 0;
            TipsWin.Open(() => BackLabel, new List<Action> { OnClickBackMenu }, new List<Action> { TipsWin.Close });
        });
    }

    // Update is called once per frame
    void Update()
    {

        switch (GameSt)
        {
            case GameState.Ready:
                if ((int)ReadyTime > 0 && ReadyTime - (int)ReadyTime < Time.deltaTime) AudioSource.PlayOneShot(ReadyClip[0]);
                GameLedControl.playerControl[0].ShowReadyTime((int)ReadyTime);
                ReadyTime = ReadyTime > 0 ? ReadyTime - Time.deltaTime : 0;
                if (ReadyTime > 0) break;
                Init();
                AudioSource.PlayOneShot(ReadyClip[1]);
                AudioSource.clip = BgmClip;
                AudioSource.Play();
                AudioSource.loop = true;
                ReadyTime = 3f;
                GameSt = GameState.Playing;
                break;
            case GameState.Idle:
                break;
            case GameState.Playing:
                TestingWin.Update();
                UpdateTime = UpdateTime > 0 ? UpdateTime - Time.deltaTime : 0;
                CheckLedKey();
                if (UpdateTime > 0) break;
       
                UpdateTime = 0.5f;

                // 更新第一个球的位置
                Vector2Int tmp_ball_vec = Ball[0];
                Ball[0] += NextStepPos();

                DrawItem(Ball[0], ColorTable.ColorType.Color_Green);

                // 更新后面球的位置
                for (int i = 1; i < Ball.Length; i++)
                {
                    Vector2Int v = Ball[i];
                    Ball[i] = tmp_ball_vec;
                    DrawItem(Ball[i], ColorTable.ColorType.Color_Green);
                    tmp_ball_vec = v;
                }

                if (Ball[0].y > Platforms[0, 0].y && Ball[0].y < Platforms[1, 0].y) break;
                if (Ball[0].y <= 1) Scores[1]++;
                else Scores[0]++;
                ScoresWin.Update();
                if (Scores[0] == Scores[1] || Scores[0] + Scores[1] < 2) GameSt = GameState.Ready;
                else GameSt = GameState.End;
                break;
            case GameState.End:
                if (Scores[0] > Scores[1])
                {
                    NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "玩家一獲勝\n是否進入下一關 " : "Player1 Win\nNext Level ";
                }
                else
                {
                    NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "玩家二獲勝\n是否進入下一關 " : "Player2 Win\nNext Level ";
                }
                TipsWin.Open(() => NextLevelLabel + ((int)NextLevelTime).ToString(), new List<Action> { OnClickInNextLevel }, new List<Action> { OnClickBackMenu });
                GameSt = GameState.Result;
                break;
            case GameState.Result:
                NextLevelTime = NextLevelTime - Time.deltaTime > 0 ? NextLevelTime - Time.deltaTime : 0;
                TipsWin.Update();
                if (NextLevelTime > 0) break;
                NextLevelTime = 5f;
                OnClickInNextLevel();
                break;
        }
    }
}
