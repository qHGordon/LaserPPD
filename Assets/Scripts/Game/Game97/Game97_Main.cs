using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum en_Game97_Sta
{
    Idle = 0,
    GameSelect,
    PlayerModeSelect,
    DiffcultySelect,
    GameOver,
    UserManager,
    TheEnd,
}
/// <summary>[Hub] 游戏入口 Hub，选择场景、难度、玩家模式，调度各游戏模块。</summary>
public class Game97_Main : MonoBehaviour
{
    public Game97_Idle gameIdle;
    public Game97_LevelSel gameSel;
    public Game97_PlayerModeSel playerModeSel;
    public Game97_DiffcultySel diffcultySel;
    public Game97_UserManager userManager;
    public AudioSource audioSource_BackG;//待机界面音乐


    Main main;
    public en_Game97_Sta statue;
    float runTime;
    float sendStaTime;
    float time;
    int showTime;
    int scenceNum;
    int playerNum;
    public const int OpenSceneNum = 6;//展示六个场景
    public static Game97_Main instance;
    public void Awake0(Main mainf)
    {
        main = mainf;
        instance = this;
        gameIdle.Awake0(this);
        gameSel.Awake0(this);
        playerModeSel.Awake0(this);
        diffcultySel.Awake0(this);
        userManager.Awake0(this);
    }

    bool IsPlayerPlaying()
    {
        for (int i = 0; i < Main.MAX_PLAYER; i++)
        {
            if (FjData.g_Fj[i].Playing)
            {
                return true;
            }
        }
        return false;
    }

    // Use this for initialization
    public void GameStart()
    {

        playerNum = Main.MAX_PLAYER;
        // playerUI[0].GameStart(0);

#if FPS_TEST
        FjData.g_Fj[0].Coins = 99;
        FjData.g_Fj[1].Coins = 99;
#endif
        if (Main.isRestart)
        {
            Main.isRestart = false;


            //
            //Set.LoadChannelLength ();
            //if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu) {
            //    Set.ChannelLength[2] = Set.ChannelLength[0];
            //    Set.ChannelLength[3] = Set.ChannelLength[1];
            //    Set.ChannelLength[4] = Set.setVal.TargetLedNum + 1;
            //}
            Set.LoadGameSetting();
            FjData.rankList = RankList.LoadRankList(Set.gameName[0]);
            Main.VerifySpeedFromSize();
            // 玩家数据清零
            PlayerDataClear();
            //
            PAction.Init();
            //
            Framebuffer.MapTabInit();

            Framebuffer.Init_LEDNum();
        }

        Main.currUser = null;

        Main.settingError = en_ErrorCode.None;

        int maxSetting = Set.gameSetting.Length;
        //设置每个通道的功能
        Set.CheckLED_Return = 0;
        Set.LedProtocol[0] = (int)en_LedProtocol.Liang;
        Set.LedProtocol[1] = (int)en_LedProtocol.Liang;
        Set.LedProtocol[2] = (int)en_LedProtocol.Zhu;
        Set.LedProtocol[3] = (int)en_LedProtocol.Zhu;
        Set.LedProtocol[4] = (int)en_LedProtocol.Zhu;
        Set.LedProtocol[5] = (int)en_LedProtocol.Zhu;
        ///
        for (int i = 0; i < Set.gameSetting.Length && i < maxSetting; i++)
        {
            if (Set.gameSetting[i] == null)
            {
                //   Debug.LogError(Set.gameSetting.Length+"    "+i);
                //   Main.settingError = en_ErrorCode.GameSetting;
                break;
            }
            else
            {
                for (int j = 0; j < Set.gameSetting[i].maxLevel && j < Set.gameSetting[i].gameLevelSetting.Length; j++)
                {
                    if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu)
                    {
                        if (Set.gameSetting[i].gameLevelSetting[j].picSetting == null)
                        {
                            Main.settingError = en_ErrorCode.PicSetting;
                            break;
                        }
                    }
                }
            }
        }

        //
        Main.playerNum = 1;
        Main.gameSetting = Set.gameSetting[0];
        Main.IsDemo = false;
        IO.Init();
        ChangeStatue(en_Game97_Sta.Idle);
    }

    // Update is called once per frame
    void Update()
    {

        //        Debug.LogError(MainRun.uartYDGZ.receiveDataNum);
        // 发送状态到控制板
        if (sendStaTime > 0)
        {
            sendStaTime -= Time.deltaTime;
        }
        else
        {
            sendStaTime = 1;
            //    CmdIO_YDGZ.CMD0_SendCmd_GameStatue (0, 0, ioGameSta);
        }
        audioSource_BackG.volume = (float)Set.setVal.MainSoundVolume / 10;
        switch (statue)
        {
            case en_Game97_Sta.Idle:
                if (runTime < 2)
                {
                    runTime += Time.deltaTime;
                    break;
                }
                //for (int i = 0; i < playerNum; i++) {
                //    if (Main.IsCanGamePlay (i)) {
                //        ChangeStatue (en_Game97_Sta.PlayerModeSelect);
                //    }
                //}
                break;

            case en_Game97_Sta.PlayerModeSelect:

                break;
            case en_Game97_Sta.DiffcultySelect:
                break;

            case en_Game97_Sta.TheEnd:

                break;
        }
    }

    public void ChangeStatue(en_Game97_Sta sta)
    {
        Key.Clear();
        statue = sta;
        runTime = 0;
        sendStaTime = 0;

        gameIdle.gameObject.SetActive(false);
        gameSel.gameObject.SetActive(false);
        playerModeSel.gameObject.SetActive(false);
        diffcultySel.gameObject.SetActive(false);
        userManager.gameObject.SetActive(false);

        //		image_Cover.gameObject.SetActive(true);
        //Debug.Log (statue);
        if (statue != en_Game97_Sta.Idle && audioSource_BackG.isPlaying == false)
        {
            audioSource_BackG.Play();
        }

        switch (statue)
        {

            case en_Game97_Sta.Idle:
                gameIdle.gameObject.SetActive(true);
                gameIdle.GameStart();

                audioSource_BackG.Stop();
                audioSource_BackG.clip = MusicManager.instance.GetAudioClip_Idle();
                if (Set.setVal.DeskMusic != 0)
                {
                    audioSource_BackG.Play();
                }
                break;

            case en_Game97_Sta.GameSelect:

                gameSel.gameObject.SetActive(true);
                gameSel.GameStart();
                break;

            case en_Game97_Sta.PlayerModeSelect:
                playerModeSel.gameObject.SetActive(true);
                playerModeSel.GameStart();
                break;
            case en_Game97_Sta.DiffcultySelect:
                diffcultySel.gameObject.SetActive(true);
                diffcultySel.GameStart();
                break;

            case en_Game97_Sta.UserManager:
                userManager.gameObject.SetActive(true);
                userManager.GameStart();
                break;
        }
    }

    public void EnterGame(int gameno)//选的场景
    {
        if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu)
        {
            Main.playerMode = (en_PlayerMode)Mathf.Clamp((int)Main.playerMode, 0, Set.gameSetting.Length - 1);
            Main.gameSetting = Set.gameSetting[(int)Main.playerMode];
        }
        else
        {
            Main.gameSetting = Set.gameSetting[0];
        }
        //
        gameno = Set.setVal.GameChoose;
        //main.ChangeScene ((en_MainStatue)gameno);
        main.ChangeStatue((en_MainStatue)gameno);
    }

    public static void PlayerDataClear()
    {
        for (int i = 0; i < Main.MAX_PLAYER; i++)
        {
            FjData.g_Fj[i].Scores = 0;
            FjData.g_Fj[i].JsScores = 0;
            FjData.g_Fj[i].Blood = 0;
            FjData.g_Fj[i].GameTime = 0;
            FjData.g_Fj[i].Playing = false;
            FjData.g_Fj[i].Played = false;
        }
    }

    public bool IsCanPlay()
    {

        for (int i = 0; i < Main.MAX_PLAYER; i++)
        {
            if (Main.IsCanGamePlay(i))
                return true;
            if (FjData.g_Fj[i].Blood > 0)
                return true;
        }
        return false;
    }
}
