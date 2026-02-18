
#define TEST_IN_WINDOW

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Profiling;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.EventSystems;

public enum en_MainStatue
{
    Restart = -1,       // 开机
    Game_00 = 0,
    Game_01,
    Game_02,
    Game_03,
    Game_04,
    Game_05,
    Game_06,
    Game_07,
    Game_97 = 97,
    Game_98,

    LoadScene,          // 加载游戏场景中..
    Menu,
    Game,
}

enum en_LoadStatue
{
    Load_FjData = 0,
    Load_JL,
    Load_Dna,
    Load_Enc,
}




/// <summary>[Core.Bootstrap] 主流程控制器，负责场景切换、游戏加载与全局状态。</summary>
public class Main : MonoBehaviour
{
    // 分辨率设置
    public static bool FREE_SIZE = false;	// 自适应分辨率
    //默认分辨率
    public const float DEFAULT_SCREEN_WIDTH = 1920;
    public const float DEFAULT_SCREEN_HEIGHT = 1080;
    //目标分辨率
    public const float TARGET_SCREEN_WIDTH = 1920;
    public const float TARGET_SCREEN_HEIGHT = 1080;


    //------------------------------------------------------------------------------------------
    // 版本(固定语言)
    /*
    为了单独输出游戏,屏蔽了游戏设置报错
    KuPao7_2.13,修改拍拍灯计时过快
    KuPao7_2.14
    1、修改镭射血条统一；
    2、关卡里点击显示退出按钮；
    3、游戏失败后如果还有游戏时间就退出到关卡选择，没有时间就退到待机。
    4、修复第一次进入游戏时如果不进入后台就进入关卡会卡死报错的问题
    KuPao7_2.15
    1、修改拍拍灯关卡中一些bug
    2、修复拍拍灯进入demo会使关卡图案不对的问题
    3、修复第一次进入游戏时如果不进入后台就进入关卡会报错的问题。点击开始后先进入后台然后瞬间退出，在进入选关
    KuPao_LaserBtn_v01.01.00
    1、加入了客户要求的tcp网络系统。
    KuPao_LaserBtn_v01.01.01
    1、修复选择页面和进入游戏提示的若干UI错误
    KuPao_LaserBtn_v01.01.02
    1、修复上个版本中的后台设置选项，选择按钮和内容页面错位的问题
    KuPao_LaserBtn_v01.01.03
    1、修复上个版本中打开的游戏错误导致灯不亮，修复提示和结束页面有中文UI的问题

    */
    public static string VERSION = "KuPao_LaserBtn_v01.01.04";

    //
    public const bool VER_DNA = false;
    public const bool VER_ENC = false;

    // 
    public const bool KEY_ESC_QUITE = false; // 按"ESC"键可退出程序
    //	public const bool 
    //公司类别 
    //public const int COMPANY_NUM = 0;  //公版"跳跃风暴"
    //public const int COMPANY_NUM = 1;  //世博"舞动魔方"(H6)
    //public const int COMPANY_NUM = 2;  //世博"跳跃风暴"(H6)
    //public const int COMPANY_NUM = 3;   //悦动光格
    public const int COMPANY_NUM = 4;   //麦克斯


    public readonly static int[] tab_GameId = { 0 };
    //通用

    public const int MAX_CH = 6;        // 通道个数
    public const int MAX_LED_ONE = 192;
    public const int MAX_LED = MAX_LED_ONE * MAX_CH;

    //
    public const int MAX_WALLLED = 20;
    public const int MAX_PLAYER = 2;
    public const int MAX_LEVEL = 15;
    public const int MAX_ANIM = 10;
    // -----------
    public Canvas gameUI_Panel;
    public ErrorTips errorTips;
    public Game00_Main game00_Main;
    public Game01_Main game01_Main;
    public Game02_Main game02_Main;
    public Game03_Main game03_Main;
    public Game04_Main game04_Main;
    public Game05_Main game05_Main;
    public Game06_Main game06_Main;
    public Game07_Main game07_Main;

    public Game13_Main game13_Main;
    public GameObject obj_13;
    public GameObject obj_14;
    public GameObject obj_15;
    public Game14_Main game14_Main;

    public Game15_Main game15_Main;

    public Game97_Main game97_Main;
    //public Game97_01_Main game97_01_Main;
    public Menu menu;
    public Game_LoadScene game_LoadScene;
    public GameObject volumeCotroy_Prefab;

    // sound
    public AudioSource audioSource_CoinIn;
    public Image img_SuoPing;

    // 全局变量 
    public static int ioVersion = 0;
    public static int MapID = 0;
    public static int MapIndex = 0;
    public static float PlayTime;
    public static bool CanSend_Score = true;

    // 主程序状态
    public static en_MainStatue statue = en_MainStatue.Restart;
    public static en_MainStatue nextStatue = en_MainStatue.Restart;

    // 游戏状态
    // 游戏ID（场景）
    public static bool isRestart = true;  // 重启动标志
    public static int gameId = -1;      // 开机加载界面
    public static bool IsDemo = false;
    public static int verifySpeed = 0;
    public static en_ErrorCode settingError;
    public static UserOne currUser;
    public static int ledCheckErrorCount;
    public static bool ledCheckFinish;
    public static en_PlayerMode playerMode;
    public static int playerNum;
    public static int gameLevel;
    public static int gameLevel_challenge_Index;
    public static bool Go_challeng;

    public static GameSetting gameSetting;
    //
    float runTime;
    int sceneNum = 0;
    string sceneName = "";


    // 界面切换
    public static int receiveDataNum = 0;       // 接收数据个数
    public static byte readAddr;
    public static byte addr;
    GameObject volumeCotroy;

    // 游戏参数设置



    //
    public static Main instance;
    void Awake()
    {

        instance = this;
        CanSend_Score = true;
        if (game00_Main != null)
        {
            game00_Main.Awake0(this);
        }
        if (game97_Main != null)
        {
            game97_Main.Awake0(this);
        }
        menu.Awake0(this);
    }

    void Start()//开始事件
    {
        Application.targetFrameRate = 60;


        GameLedControl.Init();

        Set.LoadAll();

        Key.Init();
        IO.Init();
#if NEW_IO || IO_PPL
        AudioListener.volume = (float)Set.setVal.SysVolume / 100;
#endif
        if (nextStatue == en_MainStatue.Restart)
        {
            statue = en_MainStatue.Game_97;
            ChangeStatue(en_MainStatue.Game_97);
        }
        else
        {
            ChangeStatue(nextStatue);
        }
#if UNITY_EDITOR || FPS_TEST
        //FjData.g_Fj[0].Wins = 20;
        //FjData.g_Fj[0].Coins = 20;
        //FjData.g_Fj[1].Coins = 20;

        //FjData.g_Fj[0].Wins = 30;
        //FjData.g_Fj[1].Wins = 30;
#else
         //FjData.g_Fj[0].Coins = 20;
#endif

        PlayTime = Set.setVal.GameTime;
        CanSend_Score = true;
        //    Sock_Main.instance.Sock_Init();
        //   Debug.LogError(Set.setVal.GameTime);
    }

    int fpscnt;
    int fps;
    float fpstime;

    public static long AllMemory;
    public static long UseMemory;
    public static long UnuseMemory;
    byte ledValue = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Framebuffer.Update_TargetLedColor(4, 0, 0x0000ff);
            //Framebuffer.Update_TargetLedColorAll(0xff0000);
            //Framebuffer.Update_TransmitLedColor(0, 0x60, enPointSta.Target);
            //Framebuffer.Update_PointColor(200, 0x60, enPointSta.Target);
            //int pointId = Framebuffer.tab_Mapping[1];
            //for (int i = 0; i < 4; i++)
            //{
            //    pointId += Set.ChannelLength[i];
            //}
            //Debug.LogError("p" + pointId);
            //DrawPic.DrawPointId(0, 0x0000ff, enPointSta.Target); 
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            //Framebuffer.Update_TargetLedColor(1, 0);
            //Framebuffer.Update_TargetLedColor(4, 0, 0);
            //int pointId = Framebuffer.tab_Mapping[0];
            //for (int i = 0; i < 4; i++)
            //{
            //    pointId += Set.ChannelLength[i];
            //}
            //DrawPic.DrawPointId(pointId, 0, enPointSta.None);
            //Framebuffer.Update_TransmitLedColor(200, 0, enPointSta.Target);
        }
        if (Set.setVal.Index_AnZhuang == 8)
        {
            if (!Framebuffer.isNewLeiShe)
            {
                Framebuffer.isNewLeiShe = true;
            }
        }
        else
        {
            if (Framebuffer.isNewLeiShe)
            {
                Framebuffer.isNewLeiShe = false;
            }
        }
        //if (Set.setVal.GameChoose != (int)en_GameId.LeiSheWu)
        //{
        //    Set.setVal.GameChoose = (int)en_GameId.LeiSheWu;
        //    Set.SaveAll();
        //}

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Framebuffer.Update_TargetLedColor_DianZhen(0, 9, 0x040004);

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Framebuffer.Update_TargetLedColor_DianZhen(0, 31, 0xff0000);
            //    Framebuffer.Update_TargetLedColor_DianZhen(0, 127, 0);

        }
        if (!Main.IsDemo && Set.setVal.TimeMode == 0) { Main.PlayTime -= Time.deltaTime; }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayTime = 0;
        }

#if DEBUG_TEST || UNITY_EDITOR || UNITY_STANDALONE_WIN
        AllMemory = Profiler.GetTotalReservedMemory() / 1000000;
        UseMemory = Profiler.GetTotalAllocatedMemory() / 1000000;
        UnuseMemory = Profiler.GetTotalUnusedReservedMemory() / 1000000;
      
        //帧数检测
        fpscnt++;
        fpstime += Time.deltaTime;
        if (fpstime >= 1.0f)
        {
            fpstime -= 1.0f;
            fps = fpscnt;
            fpscnt = 0;
        }
        // 输出口测试
        //     OutIO_Test();

        if (Input.GetKeyDown(KeyCode.L))
        {
            //if (ledValue == 0) {
            //    ledValue = 0xff0000;
            //} else {
            //    ledValue = 0;
            //}
            if (++ledValue >= 5)
            {
                ledValue = 0;
            }
            IO.WallLED_One(0, (byte)(1 << ledValue));
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (ledValue == 0)
            {
                ledValue = 0x1f;
            }
            else
            {
                ledValue = 0;
            }
            IO.WallLED_All(ledValue);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DrawPic.DrawRol(0, 0, Set.setVal.Width, 0xfc00, enPointSta.None);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            DrawPic.DrawCol(0, 0, Set.setVal.Height, 0xfc00, enPointSta.None);
        }
#endif
        //
        IO.CheckSend();

        //
        switch (statue)
        {

            case en_MainStatue.LoadScene:
                if (async == null || async.isDone)
                {
                    if (nextStatue == en_MainStatue.Game_97 || nextStatue == en_MainStatue.Game_98)
                    {
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game_97"));
                    }
                    else
                    {
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Game_" + ((int)nextStatue).ToString("D2")));
                    }
                    ChangeStatue(nextStatue);
                    break;
                }

                // 重新做的整个界面
                game_LoadScene.Update_ProgressValue(async.progress);

                break;

            case en_MainStatue.Game_98:
                break;
            default:
                //设置
                if (Key.MENU_OkPressed())
                {    // || Key.KEYFJ_Menu_OkPressed ()
                    IO.Init();
                    PAction.Init();
                    //    ChangeScene(en_MainStatue.Game_98);
                    ChangeStatue(en_MainStatue.Game_98);
                }
#if NEW_IO || IO_PPL
            if (Key.MENU_Statue_Left() || Key.MENU_Statue_Right())
            {
                if (volumeCotroy == null)
                {
                    volumeCotroy = Instantiate(volumeCotroy_Prefab, gameUI_Panel.transform);
                    volumeCotroy.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
#endif
                break;
        }
    }

    void LateUpdate()
    {
#if !LiuGuang
        if (Set.setVal.WallNum_Height != 0 || Set.setVal.WallNum_Width != 0)
        {
            Set.setVal.WallNum_Height = 0; Set.setVal.WallNum_Width = 0;

        }
#endif
        if (img_SuoPing.gameObject.activeSelf)
        {
            img_SuoPing.gameObject.SetActive(false);
        }
    
        if (Set.setVal.TimeMode==1)
        {
            if (PlayTime<=10)

            {
                PlayTime = 3600;

            }
        }
        if (Main.PlayTime <= 0 && CanSend_Score)
        {
            CanSend_Score = false;


            //   CmdIO_WeChat.CMD0_SendCmd_ReadySendScore();
            CmdIO_WeChat.CMD0_SendCmd_Score(FjData.g_Fj[0].Scores);

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CmdIO_WeChat.CMD0_SendCmd_ReadySendScore();
            CmdIO_WeChat.CMD0_SendCmd_Score(100);
        }
        if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu)
        {
            LedKey.Check();
            GameLeiSheBase.Check();
        }
    

        Framebuffer.MapBuffer();

   
    }


    readonly string[] tab_Language = { "CN", "EN" };
    int language = -1;
    void CheckLanguage()
    {

        if (gameId != Set.setVal.GameChoose || language != Set.setVal.Language)
        {
            gameId = Set.setVal.GameChoose;
            language = Set.setVal.Language;
            GameObject prefab;

            if (game97_Main != null)
            {
                Destroy(game97_Main.gameObject);
            }
            //if (game97_01_Main != null) {
            //    Destroy (game97_01_Main.gameObject);
            //}
            //if (game97_02_Main != null) {
            //    Destroy (game97_02_Main.gameObject);
            //}
            string companyPath = "";
            if (COMPANY_NUM == 4)
            {
                companyPath = "Company_" + COMPANY_NUM.ToString("D2") + "/";
            }
            prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game97_" + tab_Language[Set.setVal.Language]);
            game97_Main = Instantiate(prefab).GetComponent<Game97_Main>();
            game97_Main.Awake0(this);

            // Game00-game02
            if (game00_Main != null)
            {
                Destroy(game00_Main.gameObject);
            }
            if (game01_Main != null)
            {
                Destroy(game01_Main.gameObject);
            }
            if (game02_Main != null)
            {
                Destroy(game02_Main.gameObject);
            }

            switch ((en_MainStatue)gameId)
            {
                case en_MainStatue.Game_00:
                    //prefab = Resources.Load<GameObject> ("Prefabs/Game97_00_" + tab_Language[Set.setVal.Language]);
                    //game97_00_Main = Instantiate (prefab).GetComponent<Game97_Main> ();
                    //game97_00_Main.Awake0 (this);
                    //
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game00_" + tab_Language[language]);
                    game00_Main = Instantiate(prefab).GetComponent<Game00_Main>();
                    game00_Main.Awake0(this);
                    break;
                case en_MainStatue.Game_01:
                    //prefab = Resources.Load<GameObject> ("Prefabs/Game97_01_" + tab_Language[Set.setVal.Language]);
                    //game97_01_Main = Instantiate (prefab).GetComponent<Game97_01_Main> ();
                    //game97_01_Main.Awake0 (this);
                    //
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game01_" + tab_Language[language]);
                    game01_Main = Instantiate(prefab).GetComponent<Game01_Main>();
                    game01_Main.Awake0(this);
                    break;
                case en_MainStatue.Game_02:
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game02_" + tab_Language[language]);
                    game02_Main = Instantiate(prefab).GetComponent<Game02_Main>();
                    game02_Main.Awake0(this);
                    break;
                case en_MainStatue.Game_03:
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game03_" + tab_Language[language]);
                    game03_Main = Instantiate(prefab).GetComponent<Game03_Main>();
                    game03_Main.Awake0(this);
                    break;
                case en_MainStatue.Game_04:
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game04_" + tab_Language[language]);
                    game04_Main = Instantiate(prefab).GetComponent<Game04_Main>();
                    game04_Main.Awake0(this);
                    break;
                case en_MainStatue.Game_05:
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game05_" + tab_Language[language]);
                    game05_Main = Instantiate(prefab).GetComponent<Game05_Main>();
                    game05_Main.Awake0(this);
                    break;
                case en_MainStatue.Game_06:
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game06_" + tab_Language[language]);
                    game06_Main = Instantiate(prefab).GetComponent<Game06_Main>();
                    game06_Main.Awake0(this);
                    break;
                case en_MainStatue.Game_07:
                    prefab = Resources.Load<GameObject>(companyPath + "Prefabs/Game07_" + tab_Language[language]);
                    game07_Main = Instantiate(prefab).GetComponent<Game07_Main>();
                    game07_Main.Awake0(this);
                    break;
            }
        }
    }


    public void ChangeStatue(en_MainStatue sta)
    {

        statue = sta;
        Key.Clear();
        IO.WallLED_All(0);
        Framebuffer.Clear();

        CheckLanguage();

        if (game_LoadScene != null)
        {
            game_LoadScene.gameObject.SetActive(false);
        }
        if (game00_Main != null)
        {
            game00_Main.gameObject.SetActive(false);
        }
        if (game01_Main != null)
        {
            game01_Main.gameObject.SetActive(false);
        }
        if (game02_Main != null)
        {
            game02_Main.gameObject.SetActive(false);
        }
        if (game03_Main != null)
        {
            game03_Main.gameObject.SetActive(false);
        }
        if (game04_Main != null)
        {
            game04_Main.gameObject.SetActive(false);
        }
        if (game05_Main != null)
        {
            game05_Main.gameObject.SetActive(false);
        }
        if (game06_Main != null)
        {
            game06_Main.gameObject.SetActive(false);
        }
        if (game07_Main != null)
        {
            game07_Main.gameObject.SetActive(false);
        }
        if (game97_Main != null)
        {
            game97_Main.gameObject.SetActive(false);
        }
        if (obj_13 != null)
        {
            Destroy(obj_13);
        
        }
        obj_13= Instantiate(game13_Main.gameObject);
        obj_13.gameObject.SetActive(false);
        //
        if (obj_14 != null)
        {
            Destroy(obj_14);

        }
        obj_14= Instantiate(game14_Main.gameObject);
        obj_14.gameObject.SetActive(false);
        //

        if (obj_15 != null)
        {
            Destroy(obj_15);
        }
        obj_15= Instantiate(game15_Main.gameObject);
        obj_15.gameObject.SetActive(false);

        if (menu != null)
        {
            menu.gameObject.SetActive(false);
        }

        errorTips.gameObject.SetActive(true);

        switch (statue)
        {
            case en_MainStatue.Game_00:
                //game00_Main = GameObject.Find("GameMain").GetComponentInChildren<Game00_Main>();
                //game00_Main.Awake0(this);
                if (MapIndex == 6)
                {
                    if (obj_13 != null)
                    {
                        obj_13.gameObject.SetActive(true);
                        obj_13 .GetComponent<Game13_Main>().GameStart();
                    }


                }
                else if (MapIndex == 7)
                {
                    if (obj_14 != null)
                    {
                        obj_14.gameObject.SetActive(true);
                        obj_14.GetComponent<Game14_Main>().GameStart();
                    }
                }

                else if (MapIndex == 8)
                {
                    if (obj_15 != null)
                    {
                        obj_15.gameObject.SetActive(true);
                        obj_15.GetComponent<Game15_Main>().GameStart();

                    }
                }
                else
                {
                    game00_Main.gameObject.SetActive(true);
                    game00_Main.GameStart();

                }


                break;
            case en_MainStatue.Game_01:
                game01_Main.gameObject.SetActive(true);
                game01_Main.GameStart();
                break;
            case en_MainStatue.Game_02:
                game02_Main.gameObject.SetActive(true);
                game02_Main.GameStart();
                break;

            case en_MainStatue.Game_03:
                game03_Main.gameObject.SetActive(true);
                game03_Main.GameStart();
                break;
            case en_MainStatue.Game_04:
                game04_Main.gameObject.SetActive(true);
                game04_Main.GameStart();
                break;

            case en_MainStatue.Game_05:
                game05_Main.gameObject.SetActive(true);
                game05_Main.GameStart();
                break;

            case en_MainStatue.Game_06:
                game06_Main.gameObject.SetActive(true);
                game06_Main.GameStart();
                break;

            case en_MainStatue.Game_07:
              
                game07_Main.gameObject.SetActive(true);
                game07_Main.GameStart();
                break;

            case en_MainStatue.Game_97:
                game97_Main.gameObject.SetActive(true);
                game97_Main.GameStart();

                   
                break;
            case en_MainStatue.Game_98:

                errorTips.gameObject.SetActive(false);
                PAction.Init();
                menu.gameObject.SetActive(true);
                menu.Enter();
                isRestart = true;
                break;
            case en_MainStatue.LoadScene:
                errorTips.gameObject.SetActive(false);
                // 重新做的整个界面
                game_LoadScene.GameStart();
                break;
        }
    }


    static AsyncOperation async;
    public void ChangeScene(en_MainStatue gameno)   //切换场景
    {
        //Debug.Log ("流"+gameno);
        Resources.UnloadUnusedAssets();
        switch (gameno)
        {
            case en_MainStatue.Game_00:
            case en_MainStatue.Game_01:
            case en_MainStatue.Game_02:
            case en_MainStatue.Game_03:
            case en_MainStatue.Game_04:
                async = SceneManager.LoadSceneAsync("Game_" + ((int)gameno).ToString("D2"), LoadSceneMode.Additive);
                //Debug.Log("Game_" + ((int)gameno).ToString("D2"));
                break;
            case en_MainStatue.Game_97:
            case en_MainStatue.Game_98:
                Scene scene;
                async = null;
                for (int i = 0; i < tab_GameId.Length; i++)
                {
                    scene = SceneManager.GetSceneByName("Game_" + tab_GameId[i].ToString("D2"));
                    if (scene.IsValid())
                    {
                        async = SceneManager.UnloadSceneAsync("Game_" + tab_GameId[i].ToString("D2"));
                    }
                }
                // Game_97 常驻内存
                //async = SceneManager.LoadSceneAsync("Game_97", LoadSceneMode.Single);
                break;
        }
        game_LoadScene.Update_ProgressValue(0);
        nextStatue = gameno;
        ChangeStatue(en_MainStatue.LoadScene);
    }

    public void ChangeStatue_To_GameIdle()
    {
        ChangeStatue(en_MainStatue.Game_97);
        //ChangeStatue(en_MainStatue.Game);
        //gameMain.ChangeStatue(en_GameStatue.Idle);
    }

    public void PlaySound_CoinIn()
    {
        if (audioSource_CoinIn != null)
        {
            audioSource_CoinIn.Play();
        }
    }

    // Load data ---------------------------------------------------------------------------------
    // 用在加载进度条之前，读出所有数据
    // 加载数据程序状态
    static en_LoadStatue loadStatue;
    public static void Load_Start()
    {
        if (VER_DNA)
        {
            Load_ChangeStatue(en_LoadStatue.Load_Dna);
        }
        else if (VER_ENC)
        {
            Load_ChangeStatue(en_LoadStatue.Load_Enc);
        }
        else
        {
            Load_ChangeStatue(en_LoadStatue.Load_FjData);
        }
    }
    public static bool Load_Run()
    {
        //#if UNITY_EDITOR
        //        return true;
        //#endif
        switch (loadStatue)
        {
            case en_LoadStatue.Load_Dna:
                if (Game_Dna.Load())
                {
                    Game_Dna.Init();
                    Game_Dna.CheckDnaCode();
                    if (Game_Enc.MACHINE_NO <= 0)
                    {
                        return true;    // 打码失败:加载完备，直接进入打码
                    }
                    if (VER_ENC)
                    {
                        Load_ChangeStatue(en_LoadStatue.Load_Enc);
                    }
                    else
                    {
                        Load_ChangeStatue(en_LoadStatue.Load_FjData);
                    }
                }
                break;
            case en_LoadStatue.Load_Enc:
                if (Game_Enc.Load())
                {
                    Load_ChangeStatue(en_LoadStatue.Load_FjData);
                }
                break;
            case en_LoadStatue.Load_FjData:
                if (FjData.Load())
                {
                    Load_ChangeStatue(en_LoadStatue.Load_JL);
                }
                break;
            case en_LoadStatue.Load_JL:
                if (JL.Load())
                {
                    return true;    // 加载完备
                }
                break;
        }
        return false;
    }
    static void Load_ChangeStatue(en_LoadStatue sta)
    {
        loadStatue = sta;
        switch (loadStatue)
        {
            case en_LoadStatue.Load_Dna:
                Game_Dna.LoadStart();
                break;
            case en_LoadStatue.Load_Enc:
                Game_Enc.LoadStart();
                break;
            case en_LoadStatue.Load_FjData:
                FjData.LoadStart();
                break;
            case en_LoadStatue.Load_JL:
                JL.LoadStart();
                break;
        }
        //print("Load_ChangeStatue : " + loadStatue.ToString());
    }



    public static bool IsOnButton(Image cursor, Image image)
    {
        if (cursor.transform.position.x < image.rectTransform.position.x - image.rectTransform.sizeDelta.x * image.transform.lossyScale.x / 2)
            return false;
        if (cursor.transform.position.x > image.rectTransform.position.x + image.rectTransform.sizeDelta.x * image.transform.lossyScale.x / 2)
            return false;
        if (cursor.transform.position.y < image.rectTransform.position.y - image.rectTransform.sizeDelta.y * image.transform.lossyScale.y / 2)
            return false;
        if (cursor.transform.position.y > image.rectTransform.position.y + image.rectTransform.sizeDelta.y * image.transform.lossyScale.y / 2)
            return false;
        return true;
    }




#if (DEBUG_TEST || FPS_TEST || UNITY_EDITOR) && false
    void OnGUI ()//打印桌面  修改
    {
        GUI.color = Color.red;
        GUI.Label (new Rect (300, 0, 300, 20), "分辨率: " + Screen.width.ToString () + "x" + Screen.height.ToString ());
        GUI.Label (new Rect (50, 20, 300, 20), "VER: " + VERSION);
        GUI.Label (new Rect (50, 50, 200, 20), "FPS: " + fps.ToString ());
        ////	GUI.Label (new Rect(50, 100, 200, 20), "exeStatue: " + exeStatue.ToString() );
        //GUI.Label (new Rect (300, 50, 200, 20), "AllMemory: " + AllMemory.ToString ());
        //GUI.Label (new Rect (300, 80, 200, 20), "UseMemory: " + UseMemory.ToString ());
        //GUI.Label (new Rect (300, 110, 200, 20), "UnuseMemory: " + UnuseMemory.ToString ());

        //GUI.Label(new Rect(300, 150, 200, 20), "testTime : " + testTime.ToString());
        //GUI.Label(new Rect(300, 200, 200, 20), "sceneNum : " + sceneNum.ToString());
        //GUI.Label(new Rect(300, 230, 200, 20), "sceneName : " + sceneName);

        //// Uart
        ////GUI.Label(new Rect(50, 80, 400, 20), "Uart: " + uartIO.portFd.ToString() + 
        ////    " ReceveCount: "+ uartIO.receiveCnt.ToString() +
        ////    " cmdCnt: "+ cmdIO.cmdReceiveCnt.ToString());
        GUI.Label (new Rect (50, 150, 200, 20), "ledValue: " + ledValue);
        //KEY:`
        //
        GUI.Label (new Rect (50, 250, 200, 20), "KEY: ");
        for (int i = 0; i < 10; i++) {
            GUI.Label (new Rect (90 + i * 20, 250, 50, 20), Key.Key_Old[i].ToString ("X2"));
        }
        

        //GUI.Label(new Rect(50, 250, 200, 20), "Win_0: " + FjData.g_Fj[0].Wins);
        //GUI.Label(new Rect(50, 280, 200, 20), "Win_0: " + FjData.g_Fj[1].Wins);

        //if (gamePlay != null) {
        //    GUI.Label(new Rect(50, 210, 200, 20), "AliveMonsterNum: " + gamePlay.aliveMonsterNum.ToString());
        //    GUI.Label(new Rect(50, 240, 200, 20), "exsitMonsterNum: " + gamePlay.exsitMonsterNum.ToString());
        //    GUI.Label(new Rect(50, 270, 200, 20), "monsterFreshPosId: " + gamePlay.monsterFreshPosId_Out.ToString());
        //}

        //JL:
        //GUI.Label(new Rect(1000, 50, 300, 20), "JL_TTL : " + JL.ttl.ToString());
        //GUI.Label(new Rect(1000, 170, 300, 20), "JL_waveJk : " + JL.waveJk.ToString());
        //GUI.Label(new Rect(1000, 200, 300, 20), "JL_fpbl : " + JL.fpbl.ToString());
        //GUI.Label(new Rect(1000, 230, 300, 20), "JL_killJL : " + JL.killJL.ToString());

    }
#endif

    public static void Log(string str)
    { 
    }
    public static int IndexOfArry(int[] arry, int value)
    {
        for (int i = 0; i < arry.Length; i++)
        {
            if (arry[i] == value)
            {
                return i;
            }
        }
        return -1;
    }

    // 判断分数是否够玩
    public static bool IsCanGamePlay(int playerno)
    {
        if (Set.setVal.PlayerMode == (int)en_PlayerMode.Free && playerno > 0)
            return false;
        if (Set.setVal.InOutMode == (int)en_InOutMode.OneInOneOut &&
            Set.setVal.PlayerMode == (int)en_PlayerMode.PassLevel)
        {
            // 单投模式
            playerno = 0;
        }
        //		Debug.Log ("StartCoins:"+S
        //		Debug.Log ("Coins:"+FjData.g_Fj [playerno].Coins);
        if (FjData.g_Fj[playerno].Coins >= Set.setVal.StartCoins)
        {
            return true;
        }
        return false;
    }

    // 扣币是否成功
    public static bool DecStartCoin(int playerno)
    {
        if (Set.setVal.PlayerMode == (int)en_PlayerMode.Free && playerno > 0)
            return false;
        int no;
        if (Set.setVal.InOutMode == (int)en_InOutMode.OneInOneOut &&
            Set.setVal.PlayerMode == (int)en_PlayerMode.PassLevel)
        {
            // 单投模式
            no = 0;
        }
        else
        {
            no = playerno;
        }
        if (FjData.g_Fj[no].Coins >= Set.setVal.StartCoins)
        {
            FjData.g_Fj[no].Coins -= Set.setVal.StartCoins;
            FjData.SaveData_Coins(no);
            //
            int coin = Set.setVal.StartCoins;
         
            JL.PushCoin(playerno, coin);
            return true;
        }
        return false;
    }
    // 结算奖励
    public static int JieSuanScore(int playerno)
    {
        int win = 0;
        if (Set.setVal.OutMode == (int)en_OutMode.OutTicket)
        {
            if (FjData.g_Fj[playerno].Scores >= Set.setVal.TicketBl && Set.setVal.TicketBl > 0)
            {
                win = FjData.g_Fj[playerno].Scores / Set.setVal.TicketBl;
            }
        }
        else if (Set.setVal.OutMode == (int)en_OutMode.OutGift)
        {
            if (FjData.g_Fj[playerno].Scores >= Set.setVal.GiftBl && Set.setVal.GiftBl > 0)
            {
                win = FjData.g_Fj[playerno].Scores / Set.setVal.GiftBl;
                if (win > 1)
                {
                    win = 1;
                }
            }
        }
        if (win > 0)
        {
            FjData.g_Fj[playerno].Wins += win;
            FjData.SaveData_Wins(playerno);
        }
        //if (FjData.g_Fj[playerno].Scores > 0) {
        //    FjData.g_Fj[playerno].Scores = 0;
        //    //FjData.SaveData_Scores(playerno);
        //}
        //FjData.g_Fj[playerno].Scores = 0;

        if (FjData.g_Fj[playerno].Wins > 0 && PAction.outError[playerno] == false)
        {
            PAction.outEnable[playerno] = true;
        }

        return win;
    }

    public static void VerifySpeedFromSize()
    {
        int size = Mathf.Min(Set.setVal.Width, Set.setVal.Height);
        if (size >= 16)
        {
            verifySpeed = 0;
        }
        else
        {
            verifySpeed = (int)((16 - size) * 1.7f);
        }
    }
    public static void Pass_Challenge(bool isPass)
    {
        if (isPass)
        {
            gameLevel_challenge_Index++;
            Debug.LogError(Menu_LevelSet.instance.OpenIndex + "  OpenIndex  " );
            Debug.LogError(gameLevel_challenge_Index + "    gameLevel_challenge_Index  ");

            if (Main.gameLevel_challenge_Index >= Menu_LevelSet.instance.OpenIndex)
            {
                Main.Go_challeng = false;
                 
                Game00_Main.instance.ChangeStatue(en_Game00_Sta.ShowResultScore);

            }
            else
            {
                Main.Go_challeng = true;
                int a = PlayerPrefs.GetInt("NowOpenLevel" +
             gameLevel_challenge_Index.ToString());
                Debug.LogError(  "  Index  " + a);
                Main.MapIndex = a;
                if (a < 30)
                {
                    a = Game97_LevelSel.instance. Level[a];
                }

                Main.MapID = a;
           
                Game97_Main.instance.EnterGame(0);
            }
        }
        else
        {

            Main.gameLevel_challenge_Index = 0;
            Main.Go_challeng = false;
            Game97_Main.instance.ChangeStatue(en_Game97_Sta.GameSelect);

        }


    }
}
