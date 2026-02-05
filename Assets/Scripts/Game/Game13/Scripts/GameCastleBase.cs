using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 城堡攻防游戏基类
/// 提取 Game13_Main 和 Game23_Main 的公共逻辑
/// </summary>
public abstract class GameCastleBase : MonoBehaviour
{
    // ========== 公共字段 ==========
    public int CaseleWidth = 4, CaseleHeight = 2;
    public int AttackItemCnt;

    #region 测试窗口（子类需要提供具体引用）
    public abstract Transform GetTestingWinRoot();
    public abstract Button GetTestingWinOpenBtn();
    public abstract Button GetTestingWinGraphItemBtn();
    #endregion

    #region 提示窗口（子类需要提供具体引用）
    public abstract Transform GetTipsWinRoot();
    public abstract Button GetTipsWinConfirmBtn();
    public abstract Button GetTipsWinCancelBtn();
    public abstract Text GetTipsWinText();
    #endregion

    #region 玩家窗口（公共字段）
    public Transform PlayerStaWinRoot1, PlayerStaWinRoot2;
    public Image PlayerStaWinLostScroeImg1, PlayerStaWinLostScroeImg2;
    public Sprite PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite;
    public List<Image> PlayerStaWinHpImgList1 = new List<Image>(), PlayerStaWinHpImgList2 = new List<Image>();
    public List<Sprite> PlayerStaWinLostScoreSpriteList = new List<Sprite>();
    #endregion

    public Button BackBtn;

    // ========== 抽象方法：子类需要实现的类型特定逻辑 ==========
    
    /// <summary>
    /// 创建玩家状态窗口1（类型特定）
    /// </summary>
    protected abstract BasePlayerStaWin CreatePlayerStaWin1(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList);
    
    /// <summary>
    /// 创建玩家状态窗口2（类型特定）
    /// </summary>
    protected abstract BasePlayerStaWin CreatePlayerStaWin2(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList);
    
    /// <summary>
    /// 创建城堡1（类型特定）
    /// </summary>
    protected abstract BaseCastle CreateCastle1(List<Vector2Int> posList);
    
    /// <summary>
    /// 创建城堡2（类型特定）
    /// </summary>
    protected abstract BaseCastle CreateCastle2(List<Vector2Int> posList);
    
    /// <summary>
    /// 创建子弹（类型特定）
    /// </summary>
    protected abstract BaseBullet CreateBullet(Vector2Int dirVec, Vector2Int pos, BlockType blockType);
    
    /// <summary>
    /// 获取音频管理器（类型特定）
    /// </summary>
    protected abstract BaseAudioManager GetAudioManager();
    
    /// <summary>
    /// 获取BlockType枚举值（类型特定）
    /// </summary>
    protected abstract BlockType GetBlockTypePlayer1();
    protected abstract BlockType GetBlockTypePlayer2();
    
    /// <summary>
    /// 获取子弹的BlockType（类型特定，用于类型转换）
    /// </summary>
    protected abstract BlockType GetBulletBlockType(BaseBullet bullet);

    // ========== 公共字段 ==========
    protected bool IsBulletRemove = false;
    protected bool[] IsCastleGetHurt = new bool[2];
    protected int XMax = 0, YMax = 0;
    protected int BulletId = 0;
    protected int PlayerBulletCnt1 = 0, MaxPlayerBulletCnt1 = 0, PlayerBullerCnt2 = 0, MaxPlayerBulletCnt2 = 0;
    protected float ReadyTime = 3f;
    protected float GameTime = 0, MaxGameTime = 0.5f;
    protected float NextLevelTime = 5f;
    protected string BackLabel = "", NextLevelLabel = "";
    protected BaseCastle Castle1, Castle2;
    protected Dictionary<Vector2Int, BaseBullet> BulletVec2Dic = new Dictionary<Vector2Int, BaseBullet>();
    protected Dictionary<Vector2Int, bool> RemoveBulletVec2Dic = new Dictionary<Vector2Int, bool>();
    protected List<BaseBullet> BulletList = new List<BaseBullet>();
    protected List<Vector2Int> RemoveBulletList = new List<Vector2Int>();
    protected Dictionary<Vector2Int, Stack<ColorTable.ColorType>> ColorMapDic = new Dictionary<Vector2Int, Stack<ColorTable.ColorType>>();
    protected TestingWin TestingWin;
    protected TipsWin TipsWin;
    protected BasePlayerStaWin PlayerStaWin1, PlayerStaWin2;
    protected GameState GameSt = GameState.Idle;

    protected enum GameState
    {
        Ready,
        Idle,
        Playing,
        End,
        Result
    }

    // ========== 公共方法 ==========
    
    public void Awake0(Main main, int id)
    {
        // 子类可以重写此方法添加特定初始化逻辑
    }

    public void GameStart()
    {
        // 玩家状态窗口
        PlayerStaWin1 = CreatePlayerStaWin1(PlayerStaWinRoot1, PlayerStaWinLostScroeImg1, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList1, PlayerStaWinLostScoreSpriteList);
        PlayerStaWin2 = CreatePlayerStaWin2(PlayerStaWinRoot2, PlayerStaWinLostScroeImg2, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList2, PlayerStaWinLostScoreSpriteList);

        PlayerStaWin1.Updata(8);
        PlayerStaWin2.Updata(8);

        GameSt = GameState.Ready;
        ReadyTime = 3f;
    }

    /// <summary>
    /// 游戏初始化
    /// </summary>
    public void Init(int casele_w, int casele_h)
    {
        List<Vector2Int> casele_pos_list_1 = new List<Vector2Int>(), casele_pos_list_2 = new List<Vector2Int>();

        // 初始化地板最值
        XMax = Set.setVal.Width;
        YMax = Set.setVal.Height;

        ColorMapDic.Clear();

        // 初始化所有地砖的颜色
        for (int i = 0; i < XMax; i++)
        {
            for (int j = 0; j < YMax; j++)
            {
                ColorMapDic.Add(new Vector2Int(i, j), new Stack<ColorTable.ColorType>());
                ColorMapDic[new Vector2Int(i, j)].Push(ColorTable.ColorType.Color_Black);
                DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * j + i], (uint)ColorMapDic[new Vector2Int(i, j)].Peek(), enPointSta.None);
            }
        }

        // 初始化两个城堡
        for (int i = 0; i < casele_w; i++)
        {
            for (int j = 0; j < casele_h; j++)
            {
                Vector2Int pos1 = new Vector2Int(XMax - casele_w + i, YMax - casele_h + j);
                casele_pos_list_1.Add(pos1);
                ColorMapDic[pos1].Push(ColorTable.ColorType.Color_Green);

                Vector2Int pos2 = new Vector2Int(i, j);
                casele_pos_list_2.Add(pos2);
                ColorMapDic[pos2].Push(ColorTable.ColorType.Color_Green);
            }
        }

        Castle1 = CreateCastle1(casele_pos_list_1);
        Castle2 = CreateCastle2(casele_pos_list_2);

        BulletList.Clear();
        CreateBullet();

        // 测试窗口
        TestingWin = new TestingWin(GetTestingWinRoot(), GetTestingWinGraphItemBtn(), GetTestingWinOpenBtn(), ColorMapDic, () => { }, (Vector2Int vec) =>
        {
            for (int i = 0; i < BulletList.Count; i++)
            {
                if (BulletList[i].GetPos != vec) continue;
                GetAudioManager().PlaySound("Hurt");
                ColorMapDic[BulletList[i].GetPos].Pop();
                BulletList[i] = null;
                BulletList.RemoveAt(i);
                return;
            }
            if (!BulletVec2Dic.ContainsKey(vec)) return;
            if (BulletVec2Dic[vec].AllowMove) return;
            if (YMax % 2 != 0 && vec.y == (YMax >> 1)) return;
            GetAudioManager().PlaySound("Attack");
            BlockType bulletType = GetBulletBlockType(BulletVec2Dic[vec]);
            if (bulletType == GetBlockTypePlayer1()) PlayerBulletCnt1++;
            else if (bulletType == GetBlockTypePlayer2()) PlayerBullerCnt2++;
            BulletList.Add(BulletVec2Dic[vec]);
            BulletVec2Dic[vec].AllowMove = true;
            BulletVec2Dic.Remove(vec);
        }, () => XMax, () => YMax);

        // 玩家状态窗口
        PlayerStaWin1 = CreatePlayerStaWin1(PlayerStaWinRoot1, PlayerStaWinLostScroeImg1, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList1, PlayerStaWinLostScoreSpriteList);
        PlayerStaWin2 = CreatePlayerStaWin2(PlayerStaWinRoot2, PlayerStaWinLostScroeImg2, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList2, PlayerStaWinLostScoreSpriteList);
    }

    /// <summary>
    /// 在固定区域随机生成子弹
    /// </summary>
    protected void CreateBullet()
    {
        BulletVec2Dic.Clear();

        PlayerBulletCnt1 = PlayerBullerCnt2 = MaxPlayerBulletCnt1 = MaxPlayerBulletCnt2 = 0;

        // 计算可以生成的子弹数量
        AttackItemCnt = 2 * XMax * (YMax - 2 * CaseleHeight - (YMax & 1)) / 3;

        List<Vector2Int> rand_pos_list_1 = new List<Vector2Int>(), rand_pos_list_2 = new List<Vector2Int>();
        for (int x = 0; x < XMax; x++)
        {
            for (int y = CaseleHeight; y < YMax - CaseleHeight; y++)
            {
                if (YMax % 2 != 0 && y == (YMax >> 1)) continue;
                ColorMapDic[new Vector2Int(x, y)] = new Stack<ColorTable.ColorType>();
                ColorMapDic[new Vector2Int(x, y)].Push(ColorTable.ColorType.Color_Black);
                if (y >= (YMax >> 1)) rand_pos_list_1.Add(new Vector2Int(x, y));
                else rand_pos_list_2.Add(new Vector2Int(x, y));
            }
        }
        int len = Mathf.Min(Mathf.Min(rand_pos_list_1.Count, rand_pos_list_2.Count), AttackItemCnt >> 1);
        for (int i = 0; i < len; i++)
        {
            int idx1 = UnityEngine.Random.Range(0, Mathf.Min(Mathf.Min(rand_pos_list_1.Count, rand_pos_list_2.Count)));
            int idx2 = UnityEngine.Random.Range(0, Mathf.Min(Mathf.Min(rand_pos_list_1.Count, rand_pos_list_2.Count)));
            Vector2Int pos1 = rand_pos_list_1[idx1], pos2 = rand_pos_list_2[idx2];
            rand_pos_list_1.RemoveAt(idx1);
            rand_pos_list_2.RemoveAt(idx2);

            int id1 = Framebuffer.tab_Mapping[XMax * pos1.y + pos1.x], id2 = Framebuffer.tab_Mapping[XMax * pos2.y + pos2.x];

            Vector2Int dir_vec_1 = new Vector2Int(0, -1), dir_vec_2 = new Vector2Int(0, 1);

            ColorMapDic[pos1].Push(ColorTable.ColorType.Color_Red);
            ColorMapDic[pos2].Push(ColorTable.ColorType.Color_Blue);

            BulletVec2Dic[pos1] = CreateBullet(dir_vec_1, pos1, GetBlockTypePlayer1());
            MaxPlayerBulletCnt1++;

            BulletVec2Dic[pos2] = CreateBullet(dir_vec_2, pos2, GetBlockTypePlayer2());
            MaxPlayerBulletCnt2++;

            DrawPic.DrawPointId(id1, (uint)ColorMapDic[pos1].Peek(), enPointSta.Target);
            DrawPic.DrawPointId(id2, (uint)ColorMapDic[pos2].Peek(), enPointSta.Target);
        }
    }

    /// <summary>
    /// 对地砖进行判断检查
    /// </summary>
    protected void CheckLedKey()
    {
        if (BulletVec2Dic.Count == 0) return;

        #region 摧毁子弹
        RemoveBulletVec2Dic.Clear();

        for (int i = 0; i >= 0 && i < BulletList.Count; i++)
        {
            BulletId = Framebuffer.tab_Mapping[BulletList[i].GetPos.y * XMax + BulletList[i].GetPos.x];
            if (!LedKey.KeyStatus(BulletId)) continue;
            BlockType bulletType = GetBulletBlockType(BulletList[i]);
            if (bulletType == GetBlockTypePlayer1() && BulletList[i].GetPos.y >= (YMax >> 1)) continue;
            if (bulletType == GetBlockTypePlayer2() && BulletList[i].GetPos.y < (YMax >> 1)) continue;
            GetAudioManager().PlaySound("Hurt");
            RemoveBulletVec2Dic[BulletList[i].GetPos] = true;
            ColorMapDic[BulletList[i].GetPos].Pop();
            DrawPic.DrawPointId(BulletId, (uint)ColorMapDic[BulletList[i].GetPos].Peek(), enPointSta.None);
            BulletList[i] = null;
            BulletList.RemoveAt(i);
            i--;
        }
        #endregion

        #region 判断子弹是否移动
        RemoveBulletList.Clear();

        foreach (var item in BulletVec2Dic)
        {
            BulletId = Framebuffer.tab_Mapping[item.Value.GetPos.y * XMax + item.Value.GetPos.x];
            DrawPic.DrawPointId(BulletId, (uint)ColorMapDic[item.Value.GetPos].Peek(), enPointSta.Target);
            if (!LedKey.KeyStatus(BulletId) || (RemoveBulletVec2Dic.ContainsKey(item.Value.GetPos) && RemoveBulletVec2Dic[item.Value.GetPos])) continue;
            if (YMax % 2 != 0 && item.Value.GetPos.y == (YMax >> 1)) return;
            BlockType bulletType = GetBulletBlockType(item.Value);
            if (bulletType == GetBlockTypePlayer1()) PlayerBulletCnt1++;
            else if (bulletType == GetBlockTypePlayer2()) PlayerBullerCnt2++;
            GetAudioManager().PlaySound("Attack");
            RemoveBulletList.Add(item.Key);
            BulletList.Add(BulletVec2Dic[item.Key]);
            BulletVec2Dic[item.Key].AllowMove = true;
        }
        for (int i = 0; i < RemoveBulletList.Count; i++) BulletVec2Dic.Remove(RemoveBulletList[i]);
        #endregion
    }

    protected void CastleDelHp(Vector2Int hp_vec)
    {
        ColorMapDic[hp_vec].Clear();
        ColorMapDic[hp_vec].Push(ColorTable.ColorType.Color_Black);
        DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * hp_vec.y + hp_vec.x], (uint)ColorMapDic[hp_vec].Peek(), enPointSta.None);
        GetAudioManager().PlaySound("Hurt");
    }

    protected void CastleUpdataPos(Vector2Int last_pos, Vector2Int now_pos)
    {
        ColorMapDic[last_pos].Pop();
        ColorMapDic[now_pos].Push(ColorTable.ColorType.Color_Green);
        DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * last_pos.y + last_pos.x], (uint)ColorMapDic[last_pos].Peek(), enPointSta.None);
        DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * now_pos.y + now_pos.x], (uint)ColorMapDic[now_pos].Peek(), enPointSta.None);
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
        Main.MapID = 7;
        Main.MapIndex = 7;
        Game97_Main.instance.EnterGame(Main.MapIndex);
    }

    void Start()
    {
        // 提示窗口
        TipsWin = new TipsWin(GetTipsWinRoot(), GetTipsWinConfirmBtn(), GetTipsWinCancelBtn(), GetTipsWinText());

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
                if ((int)ReadyTime > 0 && ReadyTime - (int)ReadyTime < Time.deltaTime) GetAudioManager().PlaySound("Readying");
                GameLedControl.playerControl[0].ShowReadyTime((int)ReadyTime);
                ReadyTime = Mathf.Max(0, ReadyTime - Time.deltaTime);
                if (ReadyTime > 0) break;
                GameSt = GameState.Playing;
                Init(CaseleWidth, CaseleHeight);
                GetAudioManager().PlaySound("ReadyEnd");
                break;
            case GameState.Idle:
                break;
            case GameState.Playing:
                if (Input.GetKeyDown(KeyCode.E) && !TestingWin.GetIsOpen) TestingWin.Open();
                else if (Input.GetKeyDown(KeyCode.E) && TestingWin.GetIsOpen) TestingWin.Close();

                if ((PlayerBulletCnt1 == MaxPlayerBulletCnt1 || PlayerBullerCnt2 == MaxPlayerBulletCnt2) && BulletList.Count == 0) CreateBullet();

                CheckLedKey();

                TestingWin.Update();

                GameTime = Mathf.Min(GameTime + Time.deltaTime, MaxGameTime);
                if (GameTime < MaxGameTime) return;
                GameTime = 0;

                Castle1.Move((Vector2Int last_pos, Vector2Int now_pos) => CastleUpdataPos(last_pos, now_pos));
                Castle2.Move((Vector2Int last_pos, Vector2Int now_pos) => CastleUpdataPos(last_pos, now_pos));

                for (int i = BulletList.Count - 1; i >= 0 && i < BulletList.Count; i--)
                {
                    IsBulletRemove = false;
                    int bulletIndex = i; // 捕获索引
                    BulletList[i].Move((Vector2Int last_vec, Vector2Int now_vec) =>
                    {
                        ColorMapDic[last_vec].Pop();
                        BlockType bulletType = GetBulletBlockType(BulletList[bulletIndex]);
                        ColorTable.ColorType color = bulletType == GetBlockTypePlayer1() ? ColorTable.ColorType.Color_Red : ColorTable.ColorType.Color_Blue;
                        DrawPic.DrawPointId(Framebuffer.tab_Mapping[last_vec.y * XMax + last_vec.x], (uint)ColorMapDic[last_vec].Peek(), enPointSta.None);
                        IsBulletRemove = last_vec == now_vec;
                        if (IsBulletRemove) return;
                        ColorMapDic[now_vec].Push(color);
                        DrawPic.DrawPointId(Framebuffer.tab_Mapping[now_vec.y * XMax + now_vec.x], (uint)ColorMapDic[now_vec].Peek(), enPointSta.None);
                    });
                    BlockType bulletType = GetBulletBlockType(BulletList[i]);
                    IsCastleGetHurt[0] = Castle1.GetHurt(BulletList[i].GetPos, bulletType, CastleDelHp);
                    IsCastleGetHurt[1] = Castle2.GetHurt(BulletList[i].GetPos, bulletType, CastleDelHp);
                    if (!IsBulletRemove && !IsCastleGetHurt[0] && !IsCastleGetHurt[1]) continue;
                    if (ColorMapDic[BulletList[i].GetPos].Count > 1) ColorMapDic[BulletList[i].GetPos].Pop();
                    DrawPic.DrawPointId(Framebuffer.tab_Mapping[BulletList[i].GetPos.y * XMax + BulletList[i].GetPos.x], (uint)ColorMapDic[BulletList[i].GetPos].Peek(), enPointSta.None);
                    BulletList[i] = null;
                    BulletList.RemoveAt(i);
                }

                PlayerStaWin1.Updata(Castle1.GetHp);
                PlayerStaWin2.Updata(Castle2.GetHp);

                if (Castle1.GetHp == 0 || Castle2.GetHp == 0) GameSt = GameState.End;
                break;
            case GameState.End:
                NextLevelTime = 5f;
                if (Castle1.GetHp > 0)
                {
                    NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "紅方獲勝\n是否進入下一關 " : "Red Win\nNext Level ";
                }
                else if (Castle2.GetHp > 0)
                {
                    NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "藍方獲勝\n是否進入下一關 " : "Blue Win\nNext Level ";
                }
                else
                {
                    NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "平局\n是否進入下一關 " : "Draw\nNext Level ";
                }
                FjData.g_Fj[0].Scores += 100;
                TipsWin.Open(() => NextLevelLabel + ((int)NextLevelTime).ToString(), new List<Action> { OnClickInNextLevel }, new List<Action> { OnClickBackMenu });
                GameSt = GameState.Result;
                break;
            case GameState.Result:
                NextLevelTime = Mathf.Max(0, NextLevelTime - Time.deltaTime);
                TipsWin.Update();
                if (NextLevelTime > 0) break;
                OnClickInNextLevel();
                break;
            default:
                break;
        }
    }
}

// ========== 抽象接口和基类 ==========

/// <summary>
/// 玩家状态窗口基类
/// </summary>
public abstract class BasePlayerStaWin : BaseWin
{
    public BasePlayerStaWin(Transform root) : base(root, () => { }, () => { }) { }
    public abstract void Updata(int hp);
}

/// <summary>
/// 城堡基类
/// </summary>
public abstract class BaseCastle
{
    public abstract int GetHp { get; }
    public abstract void Move(Action<Vector2Int, Vector2Int> blockEvent);
    public abstract bool GetHurt(Vector2Int attackVec, BlockType blockTy, Action<Vector2Int> castleLedEvent);
}

/// <summary>
/// 子弹基类
/// </summary>
public abstract class BaseBullet
{
    public abstract Vector2Int GetPos { get; }
    public abstract bool AllowMove { get; set; }
    public abstract BlockType GetBlockTy { get; }
    public abstract void Move(Action<Vector2Int, Vector2Int> blockEvent);
}

/// <summary>
/// 音频管理器基类
/// </summary>
public abstract class BaseAudioManager
{
    public abstract void PlaySound(string soundName);
}

/// <summary>
/// Block类型枚举（统一）
/// </summary>
public enum BlockType
{
    Player1,
    Player2,
}
