using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

enum en_IdleSta
{
    ShowVideo = 0,
    ShowLogo,
    ShowRankList,
    Demo,
}

public class Game97_Idle : MonoBehaviour
{
    public Button button_Set;
    public Button button_UserMnanger;

    public Image image_Logo;
    public Image image_Game_Logo;
    public Image image_NeedTime;
    public Sprite[] spr_GameLogo;

    public Game_VideoPlayer gameVideoPlayer;
    public GameRankList gameRankList;
    public Button button_Start;
    public Button button_RankList;
    public Image image_PleaseCoinIn;

    public Game00_CoinIn coinIn;
    public Game97_CurrUserIcon userIcon;
    public Game97_UserInfo userInfo;


    public static Game97_Idle instance;
    Game97_Main game97_Main;
    en_IdleSta statue;
    float runTime;
    float userTime;
    float ShowNeedTime;
    int coins = -1;
    //
    public bool isShow_NewIdleType = true;
    float NewShow_Time = 0;
    int maxHigh = 3;
    Vector2[] newShowIdle_Light1 = new Vector2[300];
    Vector2[] newShowIdle_Light2 = new Vector2[300];
    Vector2[] newShowIdle_Light3 = new Vector2[300];
    Vector2[] newShowIdle_Light4 = new Vector2[300];
    Vector2[] newShowIdle_Light5 = new Vector2[300];
    Vector2[] newShowIdle_Light6 = new Vector2[300];
    uint LED_StartColor = 0x500000;

    const int Show_JianGe = 4;
    int JianGE_Index = 0;
    int startX, StartY = Show_JianGe;
    int NowX = 0;
    int NowY = 0;
    float Show_Lang_Time = 0;
    float MaxShow_Lang_Time = 0.1f;
    //uint[] LL = {
    //    0x0000ff, /*0x0010ff,  0x0030ff, 0x0040ff,0x0050ff, 0x0060ff,  0x00A0ff, 0x00B0ff, 0x00C0ff,*/ 0x000D0ff, 0x00E0ff,0x00ffff,
    //     0x00ffe2,  /* 0x00ffD0,   0x00ffC0,     // 0x00ffA0,   0x00ff90,     0x00ff70,   0x00ff60, */   0x00ff40,   0x00ff30,   0x00ff00,
    //    0x12ff00,  0x22ff00,  0x32ff00,   /*  0x50ff00,  0x70ff00,   */   0xa0ff00,  0xb0ff00,  0xcdff00,  0xdeff00,  0xf0ff00,  0xffff00,
    //      0xffef00,  0xffdb00,  0xffcb00, /* 0xffbb00,  0xff9000,   0xff8000,  0xff6200,  0xff5000,  0xff4300,  0xff3100,*/  0xff2100,  0xff1000,  0xff0000,
    //      0xff001b,0xff0023,0xff0034,/*0xff0046,0xff00700,0xff00750,0xff0080,0xff0090,*/0xff00A1,0xff00B3,0xff00D2,0xff00Ed,
    //};
    uint[] LL = {
        0x0000ff, //0x0010ff,  0x0030ff, 0x0040ff,0x0050ff, 0x0060ff,  0x00A0ff, 0x00B0ff, 0x00C0ff, 0x000D0ff, 0x00E0ff,0x00ffff,
         0x00ffe2,   0x00ffD0,   0x00ffC0,     // 0x00ffA0,   0x00ff90,     0x00ff70,   0x00ff60,    0x00ff40,   0x00ff30,   0x00ff00,
        0x12ff00,  //0x22ff00,  0x32ff00,  
        0x50ff00,  //0x70ff00,0xa0ff00,  0xb0ff00,  0xcdff00,  0xdeff00,  0xf0ff00,  0xffff00,
          0xffef00,//  0xffdb00,  0xffcb00,
        0xffbb00,  0xff9000,   0xff8000, // 0xff6200,  0xff5000,  0xff4300,  0xff3100,  0xff2100,  0xff1000,  0xff0000,
          0xff001b,//,0xff0023,0xff0034,
        0xff0046,0xff00700,//0xff00750,0xff0080,0xff0090,0xff00A1,0xff00B3,0xff00D2,0xff00Ed,*/
    };
    int LL_Index = 0;
    //7个色带,每次走4个格子就   下一个色带启动
    public List<Color_Area> LL_List;
    public Color_Area Color_One;
    public GameObject Color_Layer;

    public void Awake0(Game97_Main game)
    {

        game97_Main = game;
        instance = this;
        Button button = GetComponent<Button>();
        //if (button != null) {
        //    button.onClick.AddListener (OnClick);
        //}
        button_Set.onClick.AddListener(OnClick_Set);
        button_UserMnanger.onClick.AddListener(OnClick_UserMnanger);
        userIcon.GetComponent<Button>().onClick.AddListener(OnClick_UserIcon);
        button_Start.onClick.AddListener(OnClick_Start);
        button_RankList.onClick.AddListener(OnClick_RankList);

        //
        //BackG:
        if (Main.COMPANY_NUM == 7)
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Company_" + Main.COMPANY_NUM.ToString("D2") + "/Pic/Game97/Idle/BackG");
        }
        // Logo:
        if (Main.COMPANY_NUM == 1)
        {
            image_Logo.sprite = Resources.Load<Sprite>("Company_" + Main.COMPANY_NUM.ToString("D2") + "/Logo/Logo_CN");
            image_Logo.SetNativeSize();
            image_Logo.transform.localPosition = new Vector3(0, 130);
        }
        // companyLogo:
        if (Main.COMPANY_NUM == 2)
        {
            GameObject prefab = Resources.Load<GameObject>("Company_" + Main.COMPANY_NUM.ToString("D2") + "/Prefab/CompanyLogo_CN");
            GameObject companyLogo = Instantiate(prefab, transform);
            companyLogo.transform.SetSiblingIndex(0);
            //companyLogo.transform.localPosition = new Vector3 (-800, 500);
        }
    }
    public void GameStart()
    {

       // TcpCommunication.SendGameRunning();
        IO.Init();
        NewShow_Time = 0;
        //
        image_NeedTime.gameObject.SetActive(false);
        ShowNeedTime = 2f;
        if (Set.setVal.StartCoins > 0)
        {
            coinIn.gameObject.SetActive(true);
        }
        else
        {
            coinIn.gameObject.SetActive(false);
        }
        button_Set.gameObject.SetActive(false);
        button_UserMnanger.gameObject.SetActive(false);
        //image_Game_Logo.gameObject.SetActive(false);
        //switch (Set.setVal.GameChoose)
        //{
        //    case 0:

        //        break;
        //    case 1:
        //    case 2:
        //        image_Game_Logo.gameObject.SetActive(true);
        //        image_Game_Logo.sprite = spr_GameLogo[Set.setVal.GameChoose];
        //        break;
        //}
        userInfo.gameObject.SetActive(false);
        if (Set.setVal.GameMode == (int)en_GameMode.CardId)
        {
            userIcon.gameObject.SetActive(true);
            userIcon.Update_UserData(Main.currUser);
        }
        else
        {
            userIcon.gameObject.SetActive(false);
        }
        GIDLE_Start();
        Update_Coins();
        ChangeStatue(en_IdleSta.ShowLogo);
    }
    void GetColorOne()
    {
        Color_Area a = Instantiate(Color_One);
        a.Init(LL[LL_Index]);
        LL_List.Add(a);
        LL_Index++;
        if (LL_Index >= LL.Length)
        {
            LL_Index = 0;
        }

    }
    void Show_Lang()
    {
        Show_Lang_Time += Time.deltaTime;
        if (Show_Lang_Time >= MaxShow_Lang_Time)
        {
            Show_Lang_Time = 0;
            JianGE_Index++;
            for (int i = 0; i < LL_List.Count; i++)
            {
                if (LL_List[i] != null)
                {
                    LL_List[i].NextType();

                }
            }
            if (JianGE_Index > 4)
            {


                JianGE_Index = 0;
                GetColorOne();
                //  LED_StartColor += 10 * 0x0000fc;

            }

            //if (LED_StartColor >= 0xffffac)
            //{

            //    LED_StartColor = 0x0000fc;
            //}
        }



        //NowY = 1;
        //NowX = JianGE_Index * Show_JianGe + startX;
        //for (int k = 0; k < NowY; k++)
        //{
        //    for (int i = NowX; i > NowX - 5; i--)
        //    {
        //        Framebuffer.Update_PointColor(i, k, LED_StartColor + (uint)JianGE_Index * 0x000cfc, enPointSta.Rest);

        //    }
        //    NowX--;
        //    NowY++;
        //    if (NowY > Set.setVal.Height + Set.setVal.WallNum_Height)//NowX < 0 ||
        //    {

        //        break;
        //    }
        //}



    }
    // Update is called once per frame
    void Update()
    {

#if LiuGuang
        //Debug.LogError("width" + Set.setVal.WallNum_Width+ "WallNum_Height" + Set.setVal.WallNum_Height);
        NewShow_Time += Time.deltaTime;
        if (NewShow_Time >= 60)
        {
            NewShow_Time = 0;
            isShow_NewIdleType = !isShow_NewIdleType;
            if (isShow_NewIdleType == false)
            {
                Framebuffer.Clear();
                LL_List.Clear();
            }
        }
        if (isShow_NewIdleType)
        {
            Show_Lang();
        }
        else
        {
            GIDLE_Run();
        }

#else
        GIDLE_Run();
#endif

        ShowNeedTime -= Time.deltaTime;
        if (ShowNeedTime <= 0)
        {
            if (image_NeedTime.gameObject.activeSelf)
            {
                image_NeedTime.gameObject.SetActive(false);
            }
        }
        if (coins != FjData.g_Fj[0].Coins)
        {
            Update_Coins();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            OnClick();
        }
        // 检测放手环
        if (Set.setVal.GameMode == (int)en_GameMode.CardId)
        {
            if (CardInputCheck.cardInputed > 0)
            {
                CardInputCheck.cardInputed = 0;
                UserOne userOne = UserManager.LoadUserData(CardInputCheck.inCardId);
                if (userOne != null)
                {
                    Main.currUser = userOne;
                    userIcon.Update_UserData(Main.currUser);
                    userTime = 30f;
                    //} else if(Main.currUser == null){
                }
                else
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        userIcon.ShowTips("用户不存在");
                    }
                    else
                    {
                        userIcon.ShowTips("User does not exist");
                    }
                }
            }
            if (Main.currUser != null)
            {
                if (userTime > 0)
                {
                    userTime -= Time.deltaTime;
                }
                else
                {
                    Main.currUser = null;
                    userIcon.Update_UserData(Main.currUser);
                }
            }
        }

        switch (statue)
        {
            case en_IdleSta.ShowLogo:
                runTime += Time.deltaTime;
#if UNITY_EDITOR
                if (runTime >= 3)
                {

                    ChangeStatue(en_IdleSta.ShowRankList);
#else
            if (runTime >= 120) {
				ChangeStatue (en_IdleSta.ShowRankList);
#endif
                }
                break;

            case en_IdleSta.ShowVideo:
                runTime += Time.deltaTime;
                if (runTime >= 180)
                {
                    ChangeStatue(en_IdleSta.ShowLogo);
                }
                break;

            case en_IdleSta.ShowRankList:
                runTime += Time.deltaTime;
                if (runTime >= 8)
                {
                    //if (Main.settingError != en_ErrorCode.None) {
                    if (ErrorTips.instance.errorCode != en_ErrorCode.None)
                    {
                        ChangeStatue(en_IdleSta.ShowLogo);
                        break;
                    }
                    ChangeStatue(en_IdleSta.ShowLogo);
                }
                break;
        }
    }

    void ChangeStatue(en_IdleSta sta)
    {
        statue = sta;
        runTime = 0;

        //gameVideoPlayer.gameObject.SetActive(false);
        gameRankList.gameObject.SetActive(false);

        switch (statue)
        {
            case en_IdleSta.ShowVideo:
                //gameVideoPlayer.gameObject.SetActive(true);
                // image_Logo.transform.localPosition = new Vector3(-353, 125, 0);
                // image_Logo.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case en_IdleSta.ShowLogo:
                //image_Logo.transform.localPosition = new Vector3(0, 52, 0);
                // image_Logo.transform.localScale = new Vector3(1, 1, 1);
                break;

            case en_IdleSta.ShowRankList:
                gameRankList.gameObject.SetActive(true);
                gameRankList.UpdateValue(-1, FjData.rankList);
                break;

            case en_IdleSta.Demo:
                Main.IsDemo = true;
                Game97_PlayerModeSel.selectId = (int)en_PlayerMode.Free;
                Main.MapIndex = Random.Range(0, 9);
                if(Set.setVal.GameChoose != (int)en_GameId.PaiPaiDeng || Set.setVal.GameChoose != (int)en_GameId.LeiShePPD)
                {
                    Main.MapID = Game97_LevelSel.instance.Level[Main.MapIndex];
                }
                else
                {
                    Main.MapID = Main.MapIndex;
                }
                game97_Main.EnterGame(0);
                break;
        }
    }


    void Update_Coins()
    {
        coins = FjData.g_Fj[0].Coins;
        //
        if (coins >= Set.setVal.StartCoins)
        {
            image_PleaseCoinIn.gameObject.SetActive(false);
            button_Start.gameObject.SetActive(true);
            if (Set.setVal.StartCoins > 0 && game97_Main.audioSource_BackG.isPlaying == false)
            {
                game97_Main.audioSource_BackG.Play();
            }
        }
        else
        {
            image_PleaseCoinIn.gameObject.SetActive(true);
            button_Start.gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        if (CardInputCheck.GetEnterKeyCardId())
            return;
        if (button_Set.gameObject.activeSelf == false)
        {
            button_Set.gameObject.SetActive(true);
        }
        if (Set.setVal.GameMode == (int)en_GameMode.CardId && button_UserMnanger.gameObject.activeSelf == false)
        {
            button_UserMnanger.gameObject.SetActive(true);
        }


    }

    public void OnClick_Set()
    {
        if (Main.statue >= en_MainStatue.Game_00 && Main.statue <= en_MainStatue.Game_97)
        {

            Main.instance.ChangeStatue(en_MainStatue.Game_98);
        }
    }
    public void OnClick_UserMnanger()
    {
        if (Main.statue == en_MainStatue.Game_97)
        {
            game97_Main.ChangeStatue(en_Game97_Sta.UserManager);
        }
    }
    public void OnClick_UserIcon()
    {
        if (Main.currUser != null)
        {
            userInfo.gameObject.SetActive(true);
            userInfo.ShowUserInfo();
        }
    }
    void OnClick_RankList()
    {
        ChangeStatue(en_IdleSta.ShowRankList);
    }
    public void OnClick_Start()
    {
        if (Main.statue != en_MainStatue.Game_97)
            return;
        if (game97_Main.statue != en_Game97_Sta.Idle)
            return;
        if (Main.settingError != en_ErrorCode.None)
            return;
        if (Main.PlayTime < 0)
        {
            image_NeedTime.gameObject.SetActive(true);
            ShowNeedTime = 2f;
            return;
        }
        Main.instance.ChangeStatue(en_MainStatue.Game_98);
        Main.instance.ChangeStatue_To_GameIdle(); 
        Game97_PlayerModeSel.selectId = (int)en_PlayerMode.Free;
        game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
        //if (Set.setVal.Width >= 20 || Set.setVal.Height >= 20) {



        //} else {
        //    Game97_PlayerModeSel.selectId = (int)en_PlayerMode.Free;
        //    game97_Main.ChangeStatue (en_Game97_Sta.DiffcultySelect);
        //}
    }


    // Idle-LedRun----------------------------------------------------------------------
    public enum enIdleLedSta
    {
        IdleSta_Wait = 0,
        IdleSta_UD,
        IdleSta_LR,
        IdleSta_UD_In,
        IdleSta_LR_In,
        IdleSta_AllBind,
        IdleSta_UD_Out,
        IdleSta_LR_Out,
        IdleSta_End,
    };


    const int MAX_IDLEANIM = 3;
    LedAnim[] LedAnimIdle = new LedAnim[MAX_IDLEANIM];

    public enIdleLedSta GIDLE_Statue;
    float GIDLE_RunTime;
    int GIDLE_Cnt;
    int GIDLE_Cnt2;
    int GIDLE_Cnt3;
    int GIDLE_Mode;
    enCOLOR GIDLE_ColorIndex;
    public uint GIDLE_Color;
    uint GIDLE_ColorA;


    void GIDLE_Start()
    {
        int i;
        for (i = 0; i < LedAnimIdle.Length; i++)
        {
            if (LedAnimIdle[i] == null)
            {
                LedAnimIdle[i] = new LedAnim();
            }
            LedAnimIdle[i].loop = 0;                      // ÊÇ·ñÑ­»·
            LedAnimIdle[i].pointSta = enPointSta.Target;//PointSta_None;		// 
            LedAnimIdle[i].limitLeft = 0; // ¼«ÏÞµã×ó
            LedAnimIdle[i].limitRight = Set.setVal.Width - 1;      // ¼«ÏÞµãÓÒ
            LedAnimIdle[i].limitUp = 0;       // ¼«ÏÞµãÉÏ
            LedAnimIdle[i].limitDown = Set.setVal.Height - 1;  // ¼«ÏÞµãÏÂ
            LedAnimIdle[i].speed = 5;
            //LedAnimIdle[i].stopTime = 0;

        }
        GIDLE_ColorIndex = 0;
        GIDLE_Mode = 0;
        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_Wait);
    }

    void GIDLE_Run()
    {
        if (Set.setVal.ShowIdle == 0)
        {
            return;
        }
        GIDLE_RunTime += Time.deltaTime;

        if (GIDLE_RunTime < 0.015f)
            return;
        GIDLE_RunTime = 0;
        //
        switch (GIDLE_Statue)
        {
            case enIdleLedSta.IdleSta_Wait:
                GIDLE_Cnt++;
                if (GIDLE_Cnt >= 200)
                {
                    if (++GIDLE_Mode >= 4)
                    {
                        GIDLE_Mode = 0;
                    }
                    if (GIDLE_Mode == 0 || GIDLE_Mode == 2)
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_UD);
                        break;
                    }
                    if (++GIDLE_ColorIndex >= enCOLOR.RGB)
                    {
                        GIDLE_ColorIndex = enCOLOR.R;
                    }
                    if (GIDLE_Mode == 1)
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_UD_In);
                    }
                    else
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_LR_In);
                    }
                }
                break;

            case enIdleLedSta.IdleSta_UD:
                Framebuffer.FullScreen(0, enPointSta.None);
                for (int i = 0; i < MAX_IDLEANIM; i++)
                {


                    LedAnimIdle[i].Run();
                }
                if (LedAnimIdle[2].statue == 0)
                {
                    GIDLE_ChangeStatue(enIdleLedSta.IdleSta_LR);
                }
                break;

            case enIdleLedSta.IdleSta_LR:
                Framebuffer.FullScreen(0, enPointSta.None);
                for (int i = 0; i < MAX_IDLEANIM; i++)
                {
                    LedAnimIdle[i].Run();
                }
                if (LedAnimIdle[2].statue == 0)
                {
                    GIDLE_ChangeStatue(enIdleLedSta.IdleSta_End);
                }
                break;

            case enIdleLedSta.IdleSta_UD_In:
                if (++GIDLE_Cnt >= 4)
                {
                    GIDLE_Cnt = 0;
                    if (GIDLE_Cnt2 > Set.setVal.Height)
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_AllBind);
                        break;
                    }
                    if (GIDLE_Cnt2 * 2 > Set.setVal.Height)
                    {
                        GIDLE_Cnt2++;
                        break;
                    }
                    DrawPic.DrawRol(0, GIDLE_Cnt2, Set.setVal.Width, GIDLE_Color, enPointSta.None);
                    DrawPic.DrawRol(0, Set.setVal.Height - 1 - GIDLE_Cnt2, Set.setVal.Width, GIDLE_Color, enPointSta.None);
                    GIDLE_Cnt2++;
                }
                break;

            case enIdleLedSta.IdleSta_LR_In:     // ´Ó×óÓÒ½ø
                if (++GIDLE_Cnt >= 5)
                {
                    GIDLE_Cnt = 0;
                    if (GIDLE_Cnt2 > Set.setVal.Width)
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_AllBind);
                        break;
                    }
                    if (GIDLE_Cnt2 * 2 > Set.setVal.Width)
                    {
                        GIDLE_Cnt2++;
                        break;
                    }
                    DrawPic.DrawCol(GIDLE_Cnt2, 0, Set.setVal.Height, GIDLE_Color, enPointSta.None);
                    DrawPic.DrawCol(Set.setVal.Width - 1 - GIDLE_Cnt2, 0, Set.setVal.Height, GIDLE_Color, enPointSta.None);
                    GIDLE_Cnt2++;
                }
                break;

            case enIdleLedSta.IdleSta_AllBind:
                //
                if (GIDLE_Cnt == 0)
                {
                    if (GIDLE_ColorA > 2)
                    {
                        GIDLE_ColorA -= 3;
                    }
                    else
                    {
                        GIDLE_Cnt++;
                    }
                }
                else if (GIDLE_ColorA < 250)
                {
                    GIDLE_ColorA += 3;
                }
                else if (GIDLE_Cnt2 < 20)
                {
                    if (GIDLE_Cnt2 == 0)
                    {
                        GIDLE_Cnt = 0;
                    }
                    GIDLE_Cnt2++;
                }
                else
                {
                    if (GIDLE_Mode == 1)
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_LR_Out);
                    }
                    else
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_UD_Out);
                    }
                    break;
                }
                GIDLE_UpdateColor(GIDLE_ColorIndex);
                GIDLE_FullScreenColor();
                break;

            case enIdleLedSta.IdleSta_UD_Out:        // ÉÏÏÂ³ö
                if (++GIDLE_Cnt >= 4)
                {
                    GIDLE_Cnt = 0;
                    if (GIDLE_Cnt2 == 0)
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_End);
                        break;
                    }
                    DrawPic.DrawRol(0, GIDLE_Cnt2, Set.setVal.Width, GIDLE_Color, enPointSta.None);
                    DrawPic.DrawRol(0, Set.setVal.Height - 1 - GIDLE_Cnt2, Set.setVal.Width, GIDLE_Color, enPointSta.None);
                    GIDLE_Cnt2--;
                }
                break;

            case enIdleLedSta.IdleSta_LR_Out:        // ×óÓÒ³ö
                if (++GIDLE_Cnt >= 5)
                {
                    GIDLE_Cnt = 0;
                    if (GIDLE_Cnt2 == 0)
                    {
                        GIDLE_ChangeStatue(enIdleLedSta.IdleSta_End);
                        break;
                    }
                    DrawPic.DrawCol(GIDLE_Cnt2, 0, Set.setVal.Height, GIDLE_Color, enPointSta.None);
                    DrawPic.DrawCol(Set.setVal.Width - 1 - GIDLE_Cnt2, 0, Set.setVal.Height, GIDLE_Color, enPointSta.None);
                    GIDLE_Cnt2--;
                }
                break;

            case enIdleLedSta.IdleSta_End:
                GIDLE_Cnt++;
                if (GIDLE_Cnt >= 10)
                {
                    GIDLE_ChangeStatue(enIdleLedSta.IdleSta_Wait);
                }
                break;
        }
    }

    void GIDLE_ChangeStatue(enIdleLedSta sta)
    {
        int i;
        GIDLE_Statue = sta;
        GIDLE_RunTime = 0;
        GIDLE_Cnt = 0;
        GIDLE_Cnt2 = 0;
        GIDLE_Cnt3 = 0;

        switch (GIDLE_Statue)
        {
            case enIdleLedSta.IdleSta_Wait:
                Framebuffer.Clear();
                break;

            case enIdleLedSta.IdleSta_UD:
                Framebuffer.Clear();
                for (i = 0; i < MAX_IDLEANIM; i++)
                {
                    LedAnimIdle[i].animMode = enAnimMode.UpToDown;
                    LedAnimIdle[i].picType = enPicType.Rol;
                    LedAnimIdle[i].color = enCOLOR.R + i;
                    LedAnimIdle[i].startRunPos = 0;
                    LedAnimIdle[i].endRunPos = Set.setVal.Height - 1;
                    LedAnimIdle[i].width = Set.setVal.Width;       // ¿í(/³¤¶È/°ë¾¶)
                    LedAnimIdle[i].height = 1;        // ¸ß
                    LedAnimIdle[i].delayTime = 5 * i;
                    LedAnimIdle[i].x = 0;
                    LedAnimIdle[i].y = 0;
                    //
                    LedAnimIdle[i].RunStart();
                }
                break;

            case enIdleLedSta.IdleSta_LR:
                Framebuffer.Clear();
                for (i = 0; i < MAX_IDLEANIM; i++)
                {
                    LedAnimIdle[i].animMode = enAnimMode.LeftToRight;
                    LedAnimIdle[i].picType = enPicType.Col;
                    LedAnimIdle[i].color = enCOLOR.R + i;
                    LedAnimIdle[i].startRunPos = 0;
                    LedAnimIdle[i].endRunPos = Set.setVal.Width - 1;
                    LedAnimIdle[i].width = 1;     // ¿í(/³¤¶È/°ë¾¶)
                    LedAnimIdle[i].height = Set.setVal.Height;     // ¸ß
                    LedAnimIdle[i].delayTime = 5 * i;
                    LedAnimIdle[i].x = 0;
                    LedAnimIdle[i].y = 0;
                    //
                    LedAnimIdle[i].RunStart();
                }
                break;

            case enIdleLedSta.IdleSta_UD_In:     // ´ÓÉÏÏÂ½ø
                Framebuffer.Clear();
                GIDLE_ColorA = 250;
                GIDLE_UpdateColor(GIDLE_ColorIndex);
                break;

            case enIdleLedSta.IdleSta_LR_In:     // ´Ó×óÓÒ½ø
                Framebuffer.Clear();
                GIDLE_ColorA = 250;
                GIDLE_UpdateColor(GIDLE_ColorIndex);
                break;

            case enIdleLedSta.IdleSta_AllBind:
                GIDLE_ColorA = 250;
                break;

            case enIdleLedSta.IdleSta_UD_Out:        // ÉÏÏÂ³ö
                GIDLE_Cnt2 = (Set.setVal.Height - 1) / 2;
                GIDLE_ColorA = 250;
                GIDLE_UpdateColor(GIDLE_ColorIndex);
                GIDLE_FullScreenColor();
                GIDLE_UpdateColor(enCOLOR.NONE);
                break;

            case enIdleLedSta.IdleSta_LR_Out:        // ×óÓÒ³ö
                GIDLE_Cnt2 = (Set.setVal.Width - 1) / 2;
                GIDLE_ColorA = 250;
                GIDLE_UpdateColor(GIDLE_ColorIndex);
                GIDLE_FullScreenColor();
                GIDLE_UpdateColor(enCOLOR.NONE);
                break;

            case enIdleLedSta.IdleSta_End:
                Framebuffer.Clear();
                break;
        }
    }

    void GIDLE_UpdateColor(enCOLOR color)
    {

        switch (color)
        {
            case enCOLOR.NONE:
                GIDLE_Color = 0;
                break;
            case enCOLOR.R:
                GIDLE_Color = (uint)(GIDLE_ColorA << 16);
                break;
            case enCOLOR.G:
                GIDLE_Color = (uint)(GIDLE_ColorA << 8);
                break;
            case enCOLOR.B:
                GIDLE_Color = GIDLE_ColorA;
                break;
            case enCOLOR.RG:
                GIDLE_Color = (uint)((GIDLE_ColorA << 16) | (GIDLE_ColorA << 8));
                break;
            case enCOLOR.RB:
                GIDLE_Color = (uint)((GIDLE_ColorA << 16) | GIDLE_ColorA);
                break;
            case enCOLOR.GB:
                GIDLE_Color = (uint)((GIDLE_ColorA << 8) | GIDLE_ColorA);
                break;
            case enCOLOR.RGB:
                GIDLE_Color = (uint)((GIDLE_ColorA << 16) | (GIDLE_ColorA << 8) | GIDLE_ColorA);
                break;
        }
    }

    void GIDLE_FullScreenColor()
    {
        //int i;
        //int len;

        //len = Set.setVal.Width * Set.setVal.Height;
        //for (i = 0; i < len; i++) {
        //	DrawPic.DrawPointId (i, GIDLE_Color, enPointSta.None);
        //}
        Framebuffer.Update_ColorFull(GIDLE_Color, enPointSta.None);
        //Framebuffer.FullScreen (GIDLE_Color, enPointSta.None);
    }
}
