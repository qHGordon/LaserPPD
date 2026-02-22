using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game23_Main : MonoBehaviour
{


    public int CaseleWidth = 4, CaseleHeight = 2;

    public int AttackItemCnt;

    #region 测试窗口
    public Transform Game23_TestingWinRoot;

    public Button Game23_TestingWinOpenBtn, Game23_TestingWinGraphItemBtn;
    #endregion

    #region 提示窗口
    public Transform Game23_TipsWinRoot;

    public Button Game23_TipsWinConfirmBtn, Game23_TipsWinCancelBtn;

    public Text Game23_TipsWinText;
    #endregion

    #region 玩家窗口
    public Transform PlayerStaWinRoot1, PlayerStaWinRoot2;

    public Image PlayerStaWinLostScroeImg1, PlayerStaWinLostScroeImg2;

    public Sprite PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite;

    public List<Image> PlayerStaWinHpImgList1 = new List<Image>(), PlayerStaWinHpImgList2 = new List<Image>();

    public List<Sprite> PlayerStaWinLostScoreSpriteList = new List<Sprite>();
    #endregion


    public Button BackBtn;

    public void Awake0(Main main, int id)
    {

    }

    public void GameStart()
    {
        // 玩家状态窗口
        PlayerStaWin1 = new Game23_PlayerStaWin(PlayerStaWinRoot1, PlayerStaWinLostScroeImg1, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList1, PlayerStaWinLostScoreSpriteList);
        PlayerStaWin2 = new Game23_PlayerStaWin(PlayerStaWinRoot2, PlayerStaWinLostScroeImg2, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList2, PlayerStaWinLostScoreSpriteList);

        PlayerStaWin1.Updata(8);
        PlayerStaWin2.Updata(8);

        GameSt = GameState.Ready;
        ReadyTime = 3f;
    }

    /// <summary>
    /// 游戏初始化
    /// </summary>
    /// <param name="casele_w">城堡长度</param>
    /// <param name="casele_h">城堡高度</param>
    /// <param name="rand_attack_item_cnt">子弹数量</param>
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

        // 初始化两个城堡，即设置两个城堡的初始位置和移动方向
        for (int i = 0; i < casele_w; i++)
        {
            for (int j = 0; j < casele_h; j++)
            {
                // 初始化两个城堡
                Vector2Int pos1 = new Vector2Int(XMax - casele_w + i, YMax - casele_h + j);
                casele_pos_list_1.Add(pos1);
                ColorMapDic[pos1].Push(ColorTable.ColorType.Color_Green);
                //DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * pos1.y + pos1.x], (uint)ColorMapDic[pos1].Peek(), enPointSta.Rest);

                Vector2Int pos2 = new Vector2Int(i, j);
                casele_pos_list_2.Add(pos2);
                ColorMapDic[pos2].Push(ColorTable.ColorType.Color_Green);
                //DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * pos2.y + pos2.x], (uint)ColorMapDic[pos2].Peek(), enPointSta.Rest);
            }
        }

        Castle1 = new Game23_Castle(true, false, new Vector2Int(-1, 0), Game23_Block.BlockType.Player1, casele_pos_list_1, () => new Vector2Int(XMax - 1, YMax - 1));
        Castle2 = new Game23_Castle(true, false, new Vector2Int(1, 0), Game23_Block.BlockType.Player2, casele_pos_list_2, () => new Vector2Int(XMax - 1, YMax - 1));

        BulletList.Clear();
        // 生成子弹
        CreateBullet();

        // 测试窗口
        TestingWin = new TestingWin(Game23_TestingWinRoot, Game23_TestingWinGraphItemBtn, Game23_TestingWinOpenBtn, ColorMapDic, () => { }, (Vector2Int vec) =>
        {
            for (int i = 0; i < BulletList.Count; i++)
            {
                if (BulletList[i].GetPos != vec) continue;
                transform.GetComponent<Game23_AudioManger>().AudioDic["Hurt"]();
                ColorMapDic[BulletList[i].GetPos].Pop();
                BulletList[i] = null;
                BulletList.RemoveAt(i);
                return;
            }
            if (!BulletVec2Dic.ContainsKey(vec)) return;
            if (BulletVec2Dic[vec].AllowMove) return;
            if (YMax % 2 != 0 && vec.y == (YMax >> 1)) return;
            transform.GetComponent<Game23_AudioManger>().AudioDic["Attack"]();
            if (BulletVec2Dic[vec].GetBlockTy == Game23_Block.BlockType.Player1) PlayerBulletCnt1++;
            else if (BulletVec2Dic[vec].GetBlockTy == Game23_Block.BlockType.Player2) PlayerBullerCnt2++;
            BulletList.Add(BulletVec2Dic[vec]);
            BulletVec2Dic[vec].AllowMove = true;
            BulletVec2Dic.Remove(vec);
        }, () => XMax, () => YMax);

        // 玩家状态窗口
        PlayerStaWin1 = new Game23_PlayerStaWin(PlayerStaWinRoot1, PlayerStaWinLostScroeImg1, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList1, PlayerStaWinLostScoreSpriteList);
        PlayerStaWin2 = new Game23_PlayerStaWin(PlayerStaWinRoot2, PlayerStaWinLostScroeImg2, PlayerStaWinBloodSprite, PlayerStaWinNullBloodSprite, PlayerStaWinHpImgList2, PlayerStaWinLostScoreSpriteList);

    }

    // 判断子弹是否需要被移除
    protected bool IsBulletRemove = false;

    // 判断两个城堡是否受到攻击
    protected bool[] IsCastleGetHurt = new bool[2];

    // 最值
    protected int XMax = 0, YMax = 0;

    // 子弹地砖的ID
    protected int BulletId = 0;

    protected int PlayerBulletCnt1 = 0, MaxPlayerBulletCnt1 = 0, PlayerBullerCnt2 = 0, MaxPlayerBulletCnt2 = 0;

    // 游戏时间
    protected float ReadyTime = 3f;

    protected float GameTime = 0, MaxGameTime = 0.5f;

    protected float NextLevelTime = 5f;

    // UI文本
    protected string BackLabel = "", NextLevelLabel = "";

    // 两个城堡
    protected Game23_Castle Castle1, Castle2;

    // 子弹字典，用于确定初始的子弹
    protected Dictionary<Vector2Int, Game23_Bullet> BulletVec2Dic = new Dictionary<Vector2Int, Game23_Bullet>();

    // 销毁子弹字典，用于处理移动中的子弹，如果该子弹位置和初始子弹字典中的子弹重合则优先销毁再发射子弹
    protected Dictionary<Vector2Int, bool> RemoveBulletVec2Dic = new Dictionary<Vector2Int, bool>();

    // 子弹列表，用于处理移动中的子弹
    protected List<Game23_Bullet> BulletList = new List<Game23_Bullet>();

    // 子弹列表，用于处理需要从子弹字典中移除的子弹
    protected List<Vector2Int> RemoveBulletList = new List<Vector2Int>();


    // 地图颜色字典，用于记录当前地图上的地砖的颜色
    protected Dictionary<Vector2Int, Stack<ColorTable.ColorType>> ColorMapDic = new Dictionary<Vector2Int, Stack<ColorTable.ColorType>>();

    // 测试窗口
    protected TestingWin TestingWin;

    // 提示窗口
    protected TipsWin TipsWin;

    // 玩家状态窗口
    protected Game23_PlayerStaWin PlayerStaWin1, PlayerStaWin2;

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

    /// <summary>
    /// 在固定区域随机生成子弹
    /// </summary>
    protected void CreateBullet()
    {
        BulletVec2Dic.Clear();

        PlayerBulletCnt1 = PlayerBullerCnt2 = MaxPlayerBulletCnt1 = MaxPlayerBulletCnt2 = 0;

        // 计算可以生成的子弹数量，数量为：可生成区域地砖数 * （2/3）
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

            BulletVec2Dic[pos1] = new Game23_Bullet(dir_vec_1, pos1, Game23_Block.BlockType.Player1, () => new Vector2Int(XMax - 1, YMax - 1));
            MaxPlayerBulletCnt1++;

            BulletVec2Dic[pos2] = new Game23_Bullet(dir_vec_2, pos2, Game23_Block.BlockType.Player2, () => new Vector2Int(XMax - 1, YMax - 1));
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
            if (BulletList[i].GetBlockTy == Game23_Block.BlockType.Player1 && BulletList[i].GetPos.y >= (YMax >> 1)) continue;
            if (BulletList[i].GetBlockTy == Game23_Block.BlockType.Player2 && BulletList[i].GetPos.y < (YMax >> 1)) continue;
           transform.GetComponent< Game23_AudioManger>().AudioDic["Hurt"]();
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
            if (item.Value.GetBlockTy == Game23_Block.BlockType.Player1) PlayerBulletCnt1++;
            else if (item.Value.GetBlockTy == Game23_Block.BlockType.Player2) PlayerBullerCnt2++;
            transform.GetComponent<Game23_AudioManger>().AudioDic["Attack"]();
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
        transform.GetComponent<Game23_AudioManger>().AudioDic["Hurt"]();
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
        Main.MapID = 7;// (++Main.MapIndex - 100) % 5 + 100;
        Main.MapIndex = 7;// (++Main.MapIndex - 100) % 5 + 100;
        Game97_Main.instance.EnterGame(Main.MapIndex);
    }

    void Start()
    {

        // 提示窗口
        TipsWin = new TipsWin(Game23_TipsWinRoot, Game23_TipsWinConfirmBtn, Game23_TipsWinCancelBtn, Game23_TipsWinText);

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
                if ((int)ReadyTime > 0 && ReadyTime - (int)ReadyTime < Time.deltaTime) transform.GetComponent<Game23_AudioManger>().AudioDic["Readying"]();
                GameLedControl.playerControl[0].ShowReadyTime((int)ReadyTime);
                ReadyTime = Mathf.Max(0, ReadyTime - Time.deltaTime);
                if (ReadyTime > 0) break;
                GameSt = GameState.Playing;
                Init(CaseleWidth, CaseleHeight);
                transform.GetComponent<Game23_AudioManger>().AudioDic["ReadyEnd"]();
                break;
            case GameState.Idle:
                break;
            case GameState.Playing:
                if (Input.GetKeyDown(KeyCode.E) && !TestingWin.GetIsOpen) TestingWin.Open();
                else if (Input.GetKeyDown(KeyCode.E) && TestingWin.GetIsOpen) TestingWin.Close();

                //if (BulletVec2Dic.Count == 0 && BulletList.Count == 0) CreateBullet();
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
                    BulletList[i].Move((Vector2Int last_vec, Vector2Int now_vec) =>
                    {
                        ColorMapDic[last_vec].Pop();
                        ColorTable.ColorType color = BulletList[i].GetBlockTy == Game23_Block.BlockType.Player1 ? ColorTable.ColorType.Color_Red : ColorTable.ColorType.Color_Blue;
                        DrawPic.DrawPointId(Framebuffer.tab_Mapping[last_vec.y * XMax + last_vec.x], (uint)ColorMapDic[last_vec].Peek(), enPointSta.None);
                        IsBulletRemove = last_vec == now_vec;
                        if (IsBulletRemove) return;
                        ColorMapDic[now_vec].Push(color);
                        DrawPic.DrawPointId(Framebuffer.tab_Mapping[now_vec.y * XMax + now_vec.x], (uint)ColorMapDic[now_vec].Peek(), enPointSta.None);
                    });
                    IsCastleGetHurt[0] = Castle1.GetHurt(BulletList[i].GetPos, BulletList[i].GetBlockTy, CastleDelHp);
                    IsCastleGetHurt[1] = Castle2.GetHurt(BulletList[i].GetPos, BulletList[i].GetBlockTy, CastleDelHp);
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
