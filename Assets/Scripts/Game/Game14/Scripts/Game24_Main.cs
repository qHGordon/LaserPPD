using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game24_Main : MonoBehaviour {

    public SettingInGame_01 setting;

	#region 测试窗口
	public Transform Game24_TestingWinRoot;

	public Button Game24_TestingOpenBtn, Game24_TestingWinGraphItemBtn;
	#endregion

	#region 提示窗口
	public Transform Game24_TipsWinRoot;

	public Button Game24_TipsWinConfirmBtn, Game24_TipsWinCancelBtn;

	public Text Game24_TipsWinText;
	public Text Game24_Score_Player0;
	public Text Game24_Score_Player1;
	#endregion

	#region 倒计时窗口
	public Transform Game24_TimeWinRoot;

	public Text Game24_TimeWinTimeText;
	#endregion
	
	public Button BackBtn;

	public void Awake0(Main main, int id)
	{

	}

	public void GameStart()
	{
       
     
        GameSt = GameState.Ready;
		ReadyTime = 3f;
	}

	public void Init()
	{
        setting.gameObject.SetActive(false);
       setting.GGstart();

        XMax = Set.setVal.Width;
		YMax = Set.setVal.Height;

		foreach (ColorTable.ColorType color in Enum.GetValues(typeof(ColorTable.ColorType)))
		{
			ScoreDic[color] = 0;
		}

		for (int x = 0; x < XMax; x++)
		{
			for (int y = 0; y < YMax; y++)
			{
				MapDic[new Vector2Int(x, y)] = ColorTable.ColorType.Color_Black;
				StMapDic[new Vector2Int(x, y)] = 0;
				ColorListMapDic[new Vector2Int(x, y)] = new List<ColorTable.ColorType>();
				TestingColorDic[new Vector2Int(x, y)] = new Stack<ColorTable.ColorType>();
				TestingColorDic[new Vector2Int(x, y)].Push(MapDic[new Vector2Int(x, y)]);
				DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * y + x], (uint)MapDic[new Vector2Int(x, y)], enPointSta.Target);
				ScoreDic[MapDic[new Vector2Int(x, y)]]++;
                ColorListMapDic[new Vector2Int(x, y)].Add(ColorTable.ColorType.Color_Red);
                ColorListMapDic[new Vector2Int(x, y)].Add(ColorTable.ColorType.Color_Blue);
    //            foreach (ColorTable.ColorType color in Enum.GetValues(typeof(ColorTable.ColorType)))
				//{
				//	if (color == ColorTable.ColorType.Color_Black) continue;
				//	ColorListMapDic[new Vector2Int(x, y)].Add(color);
				//}
			}
		}

		TestingWin = new TestingWin(Game24_TestingWinRoot, Game24_TestingWinGraphItemBtn, Game24_TestingOpenBtn, TestingColorDic, () => { }, (Vector2Int vec) =>
		{
			if (GameTime <= 0) return;
            transform.GetComponent<Game24_AudioManger>().AudioDic["Change"]();
			ScoreDic[MapDic[vec]]--;
			MapDic[vec] = ColorListMapDic[vec][UnityEngine.Random.Range(0, ColorListMapDic[vec].Count)];
			ScoreDic[MapDic[vec]]++;
			ColorListMapDic[vec].Clear();
			DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * vec.y + vec.x], (uint)MapDic[vec], enPointSta.None);
            ColorListMapDic[vec].Add(ColorTable.ColorType.Color_Red);
            ColorListMapDic[vec].Add(ColorTable.ColorType.Color_Blue);
            //foreach (ColorTable.ColorType color in Enum.GetValues(typeof(ColorTable.ColorType)))
            //{
            //	if (color == ColorTable.ColorType.Color_Black || color == MapDic[vec]) continue;
            //	ColorListMapDic[vec].Add(color);
            //}
        }, () => XMax, () => YMax);

        GameTime = setting.GetLevelTime() ;
	}

	// 地图最值
	protected int XMax, YMax;

	// 游戏开始的倒计时
	protected float ReadyTime = 3f;

	// 游戏时间，即倒计时
	protected float GameTime = 60f;

	// 游戏时间，进入下一关的倒计时
	protected float NextLevelTime = 5f;

	// UI文本
	protected string BackLabel = "", NextLevelLabel = "";

	// 玩家1和玩家2所代表的的颜色
	protected ColorTable.ColorType PlayerColor1 = ColorTable.ColorType.Color_Red, PlayerColor2 = ColorTable.ColorType.Color_Blue;

	// 地图字典，用来判断颜色
	protected Dictionary<Vector2Int, ColorTable.ColorType> MapDic = new Dictionary<Vector2Int, ColorTable.ColorType>();

	// 状态 0为未被踩 1为第一次踩 2为正在踩
	protected Dictionary<Vector2Int, int> StMapDic = new Dictionary<Vector2Int, int>();

	// 地图颜色列表字典，用来处理地砖被踩的时候的地图字典的判定
	protected Dictionary<Vector2Int, List<ColorTable.ColorType>> ColorListMapDic = new Dictionary<Vector2Int, List<ColorTable.ColorType>>();

	// 得分字典
	protected Dictionary<ColorTable.ColorType, int> ScoreDic = new Dictionary<ColorTable.ColorType, int>();

	// 测试窗口的颜色字典
	protected Dictionary<Vector2Int, Stack<ColorTable.ColorType>> TestingColorDic = new Dictionary<Vector2Int, Stack<ColorTable.ColorType>>();

	// 测试窗口
	protected TestingWin TestingWin;

	// 提示窗口
	protected TipsWin TipsWin;

	// 倒计时窗口
	protected Game24_TimeWin TimeWin;

	// 游戏状态
	protected GameState GameSt = GameState.Idle;


	protected enum GameState
	{
		Ready,
		Idle,
		Playing,
		End,
		Result,
	}

	protected void CheckLedKey()
	{
       
		for (int x = 0; x < XMax; x++)
		{
			for (int y = 0; y < YMax; y++)
			{
				switch (StMapDic[new Vector2Int(x, y)])
				{
					case 0:
						if (!LedKey.KeyStatus(Framebuffer.tab_Mapping[XMax * y + x])) break;
						StMapDic[new Vector2Int(x, y)] = 1;
						break;
					case 1:
                        transform.GetComponent<Game24_AudioManger>().AudioDic["Change"]();
						ScoreDic[MapDic[new Vector2Int(x, y)]]--;
						MapDic[new Vector2Int(x, y)] = ColorListMapDic[new Vector2Int(x, y)][UnityEngine.Random.Range(0, ColorListMapDic[new Vector2Int(x, y)].Count)];
						ScoreDic[MapDic[new Vector2Int(x, y)]]++;
						ColorListMapDic[new Vector2Int(x, y)].Clear();
						DrawPic.DrawPointId(Framebuffer.tab_Mapping[XMax * y + x], (uint)MapDic[new Vector2Int(x, y)], enPointSta.None);
                        //foreach (ColorTable.ColorType color in Enum.GetValues(typeof(ColorTable.ColorType)))
                        //{
                        //	if (color == ColorTable.ColorType.Color_Black || color == MapDic[new Vector2Int(x, y)]) continue;
                        //	ColorListMapDic[new Vector2Int(x, y)].Add(color);
                        //}
                        ColorListMapDic[new Vector2Int(x, y)].Add(ColorTable.ColorType.Color_Red);
                        ColorListMapDic[new Vector2Int(x, y)].Add(ColorTable.ColorType.Color_Blue);
                        StMapDic[new Vector2Int(x, y)] = 2;
						break;
					case 2:
						if (LedKey.KeyStatus(Framebuffer.tab_Mapping[XMax * y + x])) break;
						StMapDic[new Vector2Int(x, y)] = 0;
						break;
					default:
						break;
				}
			}
		}
	}

	protected void OnClickBackMenu()
	{
		Time.timeScale = 0;
		TipsWin.Close();
		Main.instance.ChangeStatue(en_MainStatue.Game_97);
		Main.instance.game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
	}

	public void OnClickNextLevel()
	{
		TipsWin.Close();
        Main.MapIndex = 8;// (++Main.MapIndex - 100) % 5 + 100;
		Game97_Main.instance.EnterGame(Main.MapIndex);
	}

	// Use this for initialization
	void Start () {

		TipsWin = new TipsWin(Game24_TipsWinRoot, Game24_TipsWinConfirmBtn, Game24_TipsWinCancelBtn, Game24_TipsWinText);

		TimeWin = new Game24_TimeWin(Game24_TimeWinRoot, Game24_TimeWinTimeText, () => ((int)GameTime).ToString());

		BackLabel = Set.setVal.Language == (int)en_Language.Chinese ? "是否返回" : "Do you want to back";

		BackBtn.onClick.AddListener(() =>
		{
			Time.timeScale = 1;
			TipsWin.Open(() => BackLabel, new List<Action> { OnClickBackMenu }, new List<Action> { TipsWin.Close });
		});
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (setting.gameObject.activeSelf == false)
            {
                setting.gameObject.SetActive(true);
            }
        }
        switch (GameSt)
		{
			case GameState.Ready:
				if ((int)ReadyTime > 0 && ReadyTime - (int)ReadyTime < Time.deltaTime) transform.GetComponent<Game24_AudioManger>().AudioDic["Readying"]();
				ReadyTime = Mathf.Max(0, ReadyTime - Time.deltaTime);
				if (ReadyTime > 0) break;
				GameSt = GameState.Playing;
				Init();
                transform.GetComponent<Game24_AudioManger>()    .AudioDic["ReadyEnd"]();
				break;
			case GameState.Idle:
                Game24_Score_Player0.text = 0.ToString();
                Game24_Score_Player1.text = 0.ToString();

                break;
			case GameState.Playing:

				if (Input.GetKeyDown(KeyCode.E) && !TestingWin.GetIsOpen) TestingWin.Open();
				else if (Input.GetKeyDown(KeyCode.E) && TestingWin.GetIsOpen) TestingWin.Close();
				CheckLedKey();
				foreach (var item in MapDic)
				{
					TestingColorDic[item.Key].Clear();
					TestingColorDic[item.Key].Push(item.Value);
				}
				TestingWin.Update();
				TimeWin.Update();
				GameTime = Mathf.Max(0, GameTime - Time.deltaTime);
                Game24_Score_Player0.text = ScoreDic[ColorTable.ColorType.Color_Red].ToString();
                Game24_Score_Player1.text = ScoreDic[ColorTable.ColorType.Color_Blue].ToString();
                if (GameTime == 0) GameSt = GameState.End;
				break;
			case GameState.End:
				if (ScoreDic[ColorTable.ColorType.Color_Red] > ScoreDic[ColorTable.ColorType.Color_Blue])
				{
					NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "紅方獲勝\n是否進入下一關 " : "Red Win\nNext Level ";
				}
				else if (ScoreDic[ColorTable.ColorType.Color_Red] < ScoreDic[ColorTable.ColorType.Color_Blue])
				{
					NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "藍方獲勝\n是否進入下一關 " : "Blue Win\nNext Level ";
				}
				else
				{
					NextLevelLabel = Set.setVal.Language == (int)en_Language.Chinese ? "平局\n是否進入下一關 " : "Draw\nNext Level ";
				}
				NextLevelTime = 5f;
				TipsWin.Open(() => NextLevelLabel + ((int)NextLevelTime).ToString(), new List<Action> { OnClickNextLevel }, new List<Action> { OnClickBackMenu });
				GameSt = GameState.Result;
				break;
			case GameState.Result:
				NextLevelTime = Mathf.Max(0, NextLevelTime - Time.deltaTime);
				TipsWin.Update();
				if (NextLevelTime > 0) break;
				OnClickNextLevel();
				break;
			default:
				break;
		}
	}
    public static Game24_Main instance;
    private void Awake()
    {
        instance = this;
    }
}
