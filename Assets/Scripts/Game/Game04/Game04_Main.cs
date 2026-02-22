using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


//public enum en_Game00_Sta
//{
//    None = 0,
//    Idle,
//    ShowLevel,
//    Tips,
//    Ready,
//    Play,           // 固定点
//    ChangeMap,//换地图,或者是换阶段
//    WaitContinue,
//    Continue,
//    ShowResult,
//    ShowResultScore,
//    ShowWiner,
//    InputName,
//    RankList,
//    End,
//    Out,
//    OutEnd,
//}

//public enum en_IoGameSta
//{
//    None = 0,
//    Idle,
//    Ready,
//    Play,
//    Pass,
//    Loss,
//    GameOver,
//}

public class Game04_Main : MonoBehaviour
{
    public Game04_GameUIComm gameUIComm;
    public Game04_GameUI gameUI_Single;
    public Game04_GameUI gameUI_MulitPlayer;
    Game04_GameUI gameUI;
    public GameObject shakeMain_Obj;
    public Game04_Player[] player;
    public GameObject lifeOne_Prefab;
    // Prefab
    // 声音
    public AudioSource audioSource_BackG;       // 背景音乐
    public AudioSource audioSource_Others;
    public AudioSource audioSource_Others_tips;
    //public AudioClip[] audioClip_BGM;
    public AudioClip audioClip_ReadyTime;
    public AudioClip audioClip_ReadyGo;
    public AudioClip audioClip_Timeout;
    public AudioClip audioClip_TimesUp;
    public AudioClip audioClip_Pass;
    public AudioClip audioClip_Loss;
    public AudioClip audioClip_YouYu;
    public AudioClip[] audioClip_Rule;
    public Transform presetPic_Layer;
    public GameObject picOne_Prefab;
    PresetPic presetPic;
    public Image image_BackG;

    Main main;

    public en_Game00_Sta statue;
    public float runTime = 0;
    int runCnt;
    float sendTime;

    int id;
    int pointId;
    int bgmIndex = 0;
    public int gameId = 56;
    public int setIndex;
    public int setCount;
    bool cmdRetSucess;

    int playerNum = Main.MAX_PLAYER;
    public int gameLevel;
    int maxLevel;
    public int maxLife;
    int readyTime;
    public int gameTime;

    public int settingNum = 10;//设置中的设置数量


    public float remainTime;
    float freshWallLedTime;
    int wallLedNum;
    public int continueResult;
    public int result;
    en_TargetDieType targetDieType;
    RankOne rankOne;

    public en_IoGameSta gameStatue; // 0
    const byte WALL_LED_OPEN = 0x01;
    public int Index_JieDuan = 0;
    float maxremainTime = 60;
    public bool isClearTarage = true;//是按照时间还是按照,清空地图上所有的目标点来进入下一个阶段
    public int BigGameLevel = 0;
    public int SmallGameLevel = 0;

    public int startButtonId;
    public int targetKeyId;

    public float startButtonLedTime = 0;
    public uint startButtonLedSta = 0;

    public int Score_LinShi = 0;

#if UNITY_EDITOR
    //void Start()
    //{
    //    Main.statue = en_MainStatue.Game_02;
    //    Awake0(null);
    //    GameStart();
    //}
#endif

    public static Game04_Main instance;

    public void Awake0(Main mainn)
    {
        instance = this;
        main = mainn;
        isClearTarage = true;
        //  gameUI_MulitPlayer.Awake0();

    }

    readonly int[] tab_PlayerId_Left = { 0, 1 };
    readonly int[] tab_PlayerId_Right = { 1, 0 };
    // Use this for initialization

    public void GameStart()
    {
        Score_LinShi = 0;
        startButtonId = LedKey.GetLeiSheStartButtonId();
        targetKeyId = LedKey.GetLeiSheTargetKeyId();
        //        Texture2D texture2D = BackgPicManager.LoadLoaclPic_Game();
        //        if (texture2D != null && image_BackG != null)
        //        {
        //#if UNITY_EDITOR
        //            Debug.Log("image_BackG: " + texture2D);
        //#endif
        //            image_BackG.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        //        }
        PresetPic_Init();

        isClearTarage = true;
        playerNum = Main.MAX_PLAYER;

        GameLedControl.Stop();
        GameLedControl.GameStart((en_PlayerMode)Game97_PlayerModeSel.selectId);
        //
        gameUI_Single.gameObject.SetActive(false);
        //gameUI_MulitPlayer.gameObject.SetActive(false);
        gameUI = gameUI_Single;
        playerNum = 1;
        for (int i = 0; i < player.Length; i++)
        {
            if (Set.setVal.PlayerOrder == 0 || Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free)
            {
                player[i].Awake0(this, tab_PlayerId_Left[i]);
            }
            else
            {
                player[i].Awake0(this, tab_PlayerId_Right[i]);
            }
        }
        gameUI.gameObject.SetActive(true);
        gameUI.GameStart();
        gameUIComm.GameStart();


        //
        if (Main.IsDemo == false)
        {
            if (Set.setVal.GameMode == (int)en_GameMode.CardId && Main.currUser != null)
            {
                Main.currUser.playCnt++;
                UserManager.SaveData(Main.currUser.cardId, Main.currUser);
            }
            FjData.acc[0].PlayCnt++;
            FjData.totalAcc[0].PlayCnt++;
            FjData.SaveAcc_PlayCnt(0, false);
            FjData.SaveTotalAcc_PlayCnt(0);
        }


        for (int i = 0; i < player.Length; i++)
        {
            player[i].Ready();
        }
        maxLevel = 8;// Mathf.Min (Main.gameSetting.maxLevel, Main.gameSetting.gameLevelSetting.Length);
        gameLevel = Main.MapID;

        BigGameLevel = gameLevel / 10;
        SmallGameLevel = gameLevel % 10;

        IO.WallLED_All(0);
        ChangeStatue(en_Game00_Sta.ShowLevel);
        if (Main.IsDemo)
        {
            ChangeStatue(en_Game00_Sta.Ready);
            ChangeStatue(en_Game00_Sta.Play);
        }
    }
    public void StartButtonLed_Run()
    {
        startButtonLedTime += Time.deltaTime;
        if (startButtonLedTime >= 0.5f)
        {
            startButtonLedTime = 0;
            if (startButtonLedSta == 0)
            {
                startButtonLedSta = 0xa0a0a0;
            }
            else
            {
                startButtonLedSta = 0;
            }
            StartButtonLedOut(startButtonLedSta);
        }
    }
    public void StartButtonLedOut(uint color)
    {
        Framebuffer.Update_TargetLedColor(0, color);
    }
    bool TimePassed(float passTime)
    {
        if (runTime > 0)
        {
            runTime -= Time.deltaTime;
        }
        else
        {
            runTime = passTime;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {

        audioSource_BackG.volume = (float)Set.setVal.MainSoundVolume / 10;
        if (errorCD > 0)
        {
            errorCD -= Time.deltaTime;
        }
        if (gameUIComm.enabled == false)
        {
            gameUIComm.enabled = true;
        }
        if (statue != en_Game00_Sta.Play)
        {
            if (gameUIComm.image_JieDuan.IsActive())
            {
                gameUIComm.image_JieDuan.gameObject.SetActive(false);
            }
        }

        switch (statue)
        {
            case en_Game00_Sta.ShowLevel:
                runTime += Time.deltaTime;
                if (runTime >= 1.5f)
                {
                    runTime = 0;
                    //if (gameLevel == 0) {
                    //    ChangeStatue (en_Game00_Sta.Tips);
                    //} else {
                    //    ChangeStatue (en_Game00_Sta.Play);
                    //}
                    ChangeStatue(en_Game00_Sta.Tips);
                }
                break;

            case en_Game00_Sta.Tips:
                runTime += Time.deltaTime;
                if (runTime >= 2f)
                {
                    runTime = 0;
                    ChangeStatue(en_Game00_Sta.WaitStart);
                }
                break;
            case en_Game00_Sta.WaitStart:
                StartButtonLed_Run();
                //
                if (LedKey.KeyPressed(startButtonId) || Input.GetKeyDown(KeyCode.O))
                {

                }
                ChangeStatue(en_Game00_Sta.Ready);

                break;
            case en_Game00_Sta.Ready:

                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    if (readyTime != (int)runTime)
                    {
                        readyTime = (int)runTime;
                        gameUIComm.Update_ReadyTime(readyTime);
                        if (readyTime > 0)
                        {
                            //  ShowReadyTime(readyTime);
                            PlaySound(audioClip_ReadyTime);
                        }
                        else
                        {
                            //PlaySound (audioClip_ReadyGo);
                        }
                        if (readyTime == 0)
                        {
                            ChangeStatue(en_Game00_Sta.Play);
                        }
                    }
                }
                else
                {
                    //PlaySound (audioClip_ReadyGo);
                    ChangeStatue(en_Game00_Sta.Play);
                }
                break;

            case en_Game00_Sta.Play:
                if (Input.GetKeyDown(KeyCode.M))
                {
                    remainTime = 0;
                }

                gameUI.Update_RemainTime((int)Main.PlayTime);
                gameUI.Update_RemainTime_JieDuan((int)remainTime);


                if (!presetPic_Layer.transform.parent.gameObject.activeSelf)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        presetPic_Layer.transform.parent.gameObject.SetActive(true);
                        presetPic_Layer.transform.localRotation = new Quaternion(0, 0, 0, 1);
                    }



                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        presetPic_Layer.transform.parent.gameObject.SetActive(false);
                        presetPic_Layer.transform.localRotation = new Quaternion(0, 0, 0, 1);
                    }
                    for (int x = 0; x < Set.setVal.Width; x++)
                    {
                        for (int y = 0; y < Set.setVal.Height; y++)
                        {
                           id= x + Set.setVal.Width * y;
                            pointId = Framebuffer.tab_Mapping[id];
                            switch (Framebuffer.led[pointId].statue)
                            {
                                case enPointSta.None:
                                    list_PresetPic[id].color = Color.white;

                                    break;
                                case enPointSta.Target:
                                    list_PresetPic[id].color = Color.blue;

                                    break;
                                case enPointSta.Die:
                                    list_PresetPic[id].color = Color.red;

                                    break;
                                case enPointSta.Rest:
                                    list_PresetPic[id].color = Color.green;

                                    break;
                                case enPointSta.MoveRest:
                                    break;
                                case enPointSta.MoveDie:
                                    break;
                                case enPointSta.Dieing:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    }
                    if (Main.PlayTime <= 0)
                    {
                        Main.PlayTime = 0;
                        ChangeStatue(en_Game00_Sta.ShowResult);
                        break;
                        //       if (Set.setVal.isContinue == 0)
                        //{

                        //}
                        //else
                        //{
                        //    Main.PlayTime = 1200;
                        //}

                    }
                    if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free && Set.setVal.WallLedNum > 0)
                    {
                        CheckWallLedButton();
                        if (wallLedNum != FjData.g_Fj[0].RemainWallLed)
                        {
                            Update_WallLedNum();
                        }
                        //
                        if (FjData.g_Fj[0].RemainWallLed > 0)
                        {
                            if (CurrRemainWallLed() > 0)
                            {
                            }
                            else if (freshWallLedTime > 0)
                            {
                                freshWallLedTime -= Time.deltaTime;
                            }
                            else
                            {
                                freshWallLedTime = 1f;
                                FreshWallLed();
                            }
                        }
                    }
                    //
                    if (FjData.g_Fj[0].Life <= 0)//AllPlayerPass() || 
                    {
                        runTime += Time.deltaTime;
                        if (runTime >= 0.5f)
                        {
                            //result = 1;
                            if (Main.IsDemo)
                            {
                                ChangeStatue(en_Game00_Sta.Out);
                                break;
                            }


                            ChangeStatue(en_Game00_Sta.WaitContinue);

                        }
                        break;
                    }
                    //
                    if (remainTime > 0)
                    {
                        remainTime -= Time.deltaTime;


                    }

                    //
                    if (Index_JieDuan >= Game_Map04.instance.MaxJieDuan)// 
                    {
                        ChangeStatue(en_Game00_Sta.ShowResult);

                        break;
                    }



                    if (Game_Map04.instance.isClearAll)
                    {
                        if (Main.IsDemo)
                        {
                            ChangeStatue(en_Game00_Sta.Out);
                        }



                        else
                        {


                            Game_Map04.instance.isCleaning = true;


                            if (Game_Map04.instance.Run_MapClear())
                            {


                                NextJieDuan();
                            }
                            GameLedControl.RunCheck();

                            break;


                            //  ChangeStatue(en_Game00_Sta.WaitContinue);
                        }
                        break;


                    }
                //else
                //{
                //    if (Game_Map04.instance.remainPoint <= 0)
                //    {
                //        if (Main.IsDemo)
                //        {
                //            ChangeStatue(en_Game00_Sta.Out);
                //        }
                //        else
                //        {
                //            //Debug.Log("")
                //            Game_Map04.instance.isCleaning = true;


                //            if (Game_Map04.instance.Run_MapClear())
                //            {

                //                NextJieDuan();
                //            }
                //            GameLedControl.RunCheck();
                //            break;


                //            //  ChangeStatue(en_Game00_Sta.WaitContinue);
                //        }
                //    }

                //}

                if (remainTime <= 0)
                {
                    FjData.g_Fj[0].Life = 0;
                    ChangeStatue(en_Game00_Sta.WaitContinue);
                    break;
                }
                //
                if (gameLevel > 9 && gameLevel < 20)
                    {
                        GameLedControl.RunCheck(true);

                    }
                    else
                    {
                        GameLedControl.RunCheck(false);

                    }
                    break;

            case en_Game00_Sta.WaitContinue:
                {
                    if (runTime > 0)
                    {
                        runTime -= Time.deltaTime;
                        if (runCnt != (int)runTime)
                        {
                            runCnt = (int)runTime;
                            gameUIComm.Update_ContinueTime(runCnt);
                        }
                    }


                    if (continueResult == 1)
                    {

                        ChangeStatue(en_Game00_Sta.Continue);
                        break;
                    }


                }
                if (continueResult == 2 || runTime <= 0)// ||
                {
                    Debug.LogError("???");
                    ChangeStatue(en_Game00_Sta.ShowResult);
                }
                break;

            case en_Game00_Sta.Continue:
                // 确认收到后跳转
                int num = gameLevel / 10;
                int num1 = gameLevel % 10;
                FjData.g_Fj[0].Life = 10;
                ChangeStatue(en_Game00_Sta.Ready);
                if (cmdRetSucess)
                {
                    //ChangeStatue (en_Game00_Sta.SetPresetPoint);

                }
                break;

            case en_Game00_Sta.ShowResult:
                if (sendTime > 0)
                {
                    sendTime -= Time.deltaTime;
                }
                else
                {
                    sendTime = 0.5f;
                    //   CmdIO_YDGZ.CMD0_SendCmd_GameOver (gameId, gameLevel, result);
                }
                runTime += Time.deltaTime;
                if (runTime >= 2)
                {
                    if (result == 0)
                    {
                        ChangeStatue(en_Game00_Sta.ShowResultScore);
                        break;
                    }
                    else
                    {
                        // if (LedKey.KeyPressed(Game04_Main.instance.targetKeyId) || Input.GetKeyDown(KeyCode.Q))
                        {
                            Main.MapIndex++;
                            if (Main.MapIndex >= 29)
                            {
                                Main.MapIndex = 0;
                            }
                            if (Main.PlayTime > 0)
                            {
                                FjData.g_Fj[0].Scores += Score_LinShi;
                                Main.MapID = Main.MapIndex;
                                Game97_Main.instance.EnterGame(0);
                            }
                            else
                            {
                                ChangeStatue(en_Game00_Sta.Out);
                            }
                        }


                        break;
                    }
                }
                break;

            case en_Game00_Sta.ShowResultScore:
                if (AllPlayerAddScoreFinish() == false)
                    break;
                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    break;
                }
                if (true)// result == 0|| gameLevel + 1 >= maxLevel
                {
                    if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free)
                    {

                        bool hasUser = false;
                        if (Set.setVal.GameMode == (int)en_GameMode.CardId && Main.currUser != null)
                        {
                            if (FjData.g_Fj[0].Scores > Main.currUser.maxScore)
                            {
                                Main.currUser.maxScore = FjData.g_Fj[0].Scores;
                            }
                            Main.currUser.AddUseRecordOne(gameLevel, FjData.g_Fj[0].Scores);
                            Main.currUser.AddScoresRank(gameLevel, FjData.g_Fj[0].Scores);
                            UserManager.SaveData(Main.currUser.cardId, Main.currUser);
                            gameUIComm.playerNameInput.playerName = Main.currUser.userName;
                            hasUser = true;
                        }

                        if (FjData.rankList.UpList(FjData.g_Fj[0].Scores))
                        {
                            if (hasUser)
                            {
                                ChangeStatue(en_Game00_Sta.RankList);
                            }
                            else
                            {
                                ChangeStatue(en_Game00_Sta.InputName);
                            }
                            break;
                        }
                        ChangeStatue(en_Game00_Sta.End);
                    }
                    else
                    {
                        ChangeStatue(en_Game00_Sta.ShowWiner);
                    }
                }
                else
                {
                    gameLevel++;
                    ChangeStatue(en_Game00_Sta.ShowLevel);
                }
                break;

            case en_Game00_Sta.ShowWiner:
                runTime += Time.deltaTime;
                if (runTime >= 8)
                {
                    ChangeStatue(en_Game00_Sta.Out);
                }
                break;

            case en_Game00_Sta.InputName:
                if (gameUIComm.playerNameInput.gameObject.activeSelf)
                    break;
                ChangeStatue(en_Game00_Sta.RankList);
                break;

            case en_Game00_Sta.RankList:
                runTime += Time.deltaTime;
                if (runTime >= 8)
                {
                    ChangeStatue(en_Game00_Sta.Out);
                }
                break;

            case en_Game00_Sta.End:
                runTime += Time.deltaTime;
                if (runTime >= 1)
                {
                    ChangeStatue(en_Game00_Sta.Out);
                }
                break;


            case en_Game00_Sta.Out:

                runTime += Time.deltaTime;
                if (runTime >= 0.3f)
                {
                    //Game97_Main.gameOver.Awake0(this);
                    //Game97_Main.gameOver.gameObject.SetActive(true);
                    //GameOver();
                    ChangeStatue(en_Game00_Sta.OutEnd);
                    //Game97_Main.gameOver.Awake0(this);
                    main.ChangeStatue(en_MainStatue.Game_97);
                }
                break;
        }
    }

    public void ChangeStatue(en_Game00_Sta sta)
    {


        BigGameLevel = gameLevel / 10;
        SmallGameLevel = gameLevel % 10;
        //   Debug.LogError(gameLevel);
        statue = sta;
        runTime = 0;
        runCnt = 0;
        sendTime = 0;
        setIndex = 0;
        setCount = 0;
        cmdRetSucess = false;
        presetPic_Layer.transform.parent.gameObject.SetActive(false);

        Key.Clear();
        // CmdIO_YDGZ.CMD0_SendCmd_GameStatue (gameId, gameLevel, (int)statue);



        //gameUI.time_Obj.SetActive (false);
        gameUIComm.tips_Obj.SetActive(false);
        gameUIComm.showLevel_Obj.SetActive(false);
        gameUIComm.image_ReadyTime.gameObject.SetActive(false);
        gameUIComm.continueGame_Obj.SetActive(false);

        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            GameLedControl.gamePoint[i].errorTime = 0;
        }

        switch (statue)
        {

            case en_Game00_Sta.Idle:

                presetPic_Layer.transform.parent.gameObject.SetActive(false);
                float sX, sY;

                gameUI.time_Obj_All.SetActive(false);
                gameUI.time_Obj_JieDuan.SetActive(false);


                for (int i = 0; i < player.Length; i++)
                {
                    player[i].ChangeStatue(en_Player00Sta.Idle);
                }
                break;

            case en_Game00_Sta.Tips:

                //  Update_CurrPresetPic();

                break;



            case en_Game00_Sta.ShowLevel:

                gameUI.time_Obj_All.SetActive(false);


                gameUI.time_Obj_JieDuan.SetActive(false);
                gameUIComm.level_Obj.SetActive(false);
                gameUIComm.wallLedNum_Obj.SetActive(false);
                //gameUIComm.showLevel_Obj.SetActive(true);
                gameUIComm.Update_ShowLevel(gameLevel);
                if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free && Set.setVal.WallLedNum > 0)
                {
                    IO.WallLED_All(0);//WALL_LED_OPEN
                }
                //
                if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free && Set.setVal.WallLedNum > 0)
                {
                    gameUIComm.wallLedNum_Obj.SetActive(true);
                    //wallLedNum = Set.setVal.WallLedNum;
                    wallLedNum = Main.gameSetting.gameLevelSetting[gameLevel].wallLedNum;
                }
                else
                {
                    wallLedNum = 0;
                }


                for (int i = 0; i < Main.MAX_PLAYER; i++)
                {

                    FjData.g_Fj[i].Life = 10;


                    FjData.g_Fj[i].RemainPoint = 5000;
                    FjData.g_Fj[i].TargetPoint = FjData.g_Fj[i].RemainPoint;
                    FjData.g_Fj[i].RemainWallLed = wallLedNum;
                }


                Update_WallLedNum();
                if (Main.IsDemo)
                {
                    gameTime = 120;
                }
                else
                {
                    gameTime = (int)Main.PlayTime;
                }
                gameUI.Update_RemainTime((int)Main.PlayTime);
                gameUI.Update_RemainTime_JieDuan((int)remainTime);
                for (int i = 0; i < player.Length; i++)
                {
                    if (i < gameUI.playerUI.Length)
                    {
                        player[i].playerUI = gameUI.playerUI[i];
                    }
                    player[i].GameStart(maxLife);
                }
                break;

            case en_Game00_Sta.Ready:
                //   Debug.LogError("BigGameLevel   " + BigGameLevel);
                if (continueResult == 1)
                {

                }
                else
                {
                    Score_LinShi = 0;
                    Index_JieDuan = 0;
                }
                continueResult = 0;
                GameLedControl.ReadyStart(Main.gameSetting.gameLevelSetting[BigGameLevel], false);


                sX = 20 / (float)Set.setVal.Width;
                sY = 12 / (float)Set.setVal.Height;


                //     presetPic_Layer.transform.localScale = new Vector3(sX, sY, 0);

                //gameUI.time_Obj.SetActive (true);
                runTime = 4;
                readyTime = 3;
                gameUIComm.Update_ReadyTime(readyTime);
                if (Main.IsDemo == false)
                {
                    // ShowReadyTime(readyTime);
                }
                PlaySound(audioClip_ReadyTime);
                if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free && Set.setVal.WallLedNum > 0)
                {
                    IO.WallLED_All(0);//WALL_LED_OPEN
                }
                break;

            case en_Game00_Sta.Play:
                  PlaySound_Tips(audioClip_Rule[gameLevel]);
                gameUIComm.level_Obj.SetActive(true);
                if (Set.setVal.Height > Set.setVal.Width)
                {
                    presetPic_Layer.transform.Rotate(0, 0, 90);

                }


                gameUIComm.Update_Level(gameLevel);


                gameUIComm.Update_JieDuan(Index_JieDuan);

                gameUI.time_Obj_All.SetActive(true);
                gameUI.time_Obj_JieDuan.SetActive(true);



                gameUI.Update_RemainTime(Set.setVal.GameTime);
                gameUI.Update_RemainTime_JieDuan((int)remainTime);
                if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free && Set.setVal.WallLedNum > 0)
                {
                    IO.WallLED_All(0);//WALL_LED_OPEN
                }
                freshWallLedTime = 0;

                for (int i = 0; i < playerNum && i < player.Length; i++)
                {
                    player[i].ChangeStatue(en_Player00Sta.Play);
                }



                targetDieType = (en_TargetDieType)Set.gameSetting[0].gameLevelSetting[0].targetDieType;
                GameLedControl.PlayStart(Set.gameSetting[0].gameLevelSetting[0]);
                //
                if (++bgmIndex >= MusicManager.instance.audioClip_BGM.Length)
                {
                    bgmIndex = 0;
                }
                audioSource_BackG.Stop();

                audioSource_BackG.clip = MusicManager.instance.audioClip_BGM[bgmIndex];
                audioSource_BackG.clip = MusicManager.instance.GetAudioClip_Game();
                audioSource_BackG.volume = 0.6f;
                //   audioSource_BackG.Play();
                //
                PlaySound(audioClip_ReadyGo);

                Game_Map04.instance.InitMap(gameLevel);
                break;

            case en_Game00_Sta.WaitContinue:
                runTime = 10f;
                continueResult = 0;
                gameUIComm.continueGame_Obj.SetActive(true);
                GameLedControl.playerControl[0].ShowColorFull(0xff0000, enPointSta.Die);
                if (Main.IsDemo)
                {
                    continueResult = 2;
                }
                break;
            case en_Game00_Sta.Continue:

                gameUIComm.continueGame_Obj.SetActive(true);
                break;

            case en_Game00_Sta.ShowResult:
                for (int i = 0; i < Game04_Main.instance.gameUIComm.txt_TarageList.Length; i++)
                {
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(false);
                }
                GameLedControl.playerControl[0].ShowColorFull(0, enPointSta.None);
                gameUIComm.level_Obj.SetActive(false);
                gameUI.time_Obj_All.SetActive(false);
                gameUI.time_Obj_JieDuan.SetActive(false);
                if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free && Set.setVal.WallLedNum > 0)
                {
                    IO.WallLED_All(0);
                }
                if (remainTime> 0&&FjData.g_Fj[0].Life>0)
                {
                    result = 1;
                }
                else  
                {
                    result = 0;
                }

                           

                if (result == 0)
                {

                }

                for (int i = 0; i < playerNum && i < player.Length; i++)
                {
                    player[i].ShowResult();
                }

                //
                audioSource_BackG.Stop();

                if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free)
                {
                    if (result == 0)
                    {
                        PlaySound(audioClip_Loss);
                    }
                    else
                    {
                        PlaySound(audioClip_Pass);
                    }
                }
                break;

            case en_Game00_Sta.ShowResultScore:
                gameUIComm.wallLedNum_Obj.SetActive(false);
                for (int i = 0; i < playerNum && i < player.Length; i++)
                {
                    player[i].ShowResultScore();
                }
                if (result == 0 || gameLevel + 1 >= maxLevel)
                {
                    runTime = 4;
                }
                else
                {
                    runTime = 1;
                }
                break;

            case en_Game00_Sta.ShowWiner:
                if (player[0].result == player[1].result)
                {
                    if (FjData.g_Fj[0].Scores > FjData.g_Fj[1].Scores)
                    {
                        player[0].ShowWinner(true);
                        player[1].ShowWinner(false);
                    }
                    else if (FjData.g_Fj[0].Scores < FjData.g_Fj[1].Scores)
                    {
                        player[0].ShowWinner(false);
                        player[1].ShowWinner(true);
                    }
                    else if (player[0].result == 1)
                    {
                        // 平局
                        player[0].ShowWinner(true);
                        player[1].ShowWinner(true);
                    }
                    else
                    {
                        player[0].ShowWinner(false);
                        player[1].ShowWinner(false);
                    }
                }
                else if (player[0].result == 0)
                {
                    player[0].ShowWinner(false);
                    player[1].ShowWinner(true);
                }
                else
                {
                    player[0].ShowWinner(true);
                    player[1].ShowWinner(false);
                }
                break;

            case en_Game00_Sta.InputName:
                gameUIComm.playerNameInput.gameObject.SetActive(true);
                gameUIComm.playerNameInput.GameStart(6, null);
                break;

            case en_Game00_Sta.RankList:
                gameUIComm.rankList.gameObject.SetActive(true);
                rankOne.playerName = gameUIComm.playerNameInput.GetName();
                rankOne.score = FjData.g_Fj[0].Scores;
                rankOne.level = gameLevel;
                //
                int rank = FjData.rankList.AddOne(rankOne);
                RankList.SaveRankList(Set.gameName[0], FjData.rankList);
                //
                gameUIComm.rankList.UpdateValue(rank, FjData.rankList);
                break;

            case en_Game00_Sta.End:
                break;

            case en_Game00_Sta.Out:
                break;
        }
    }

    // 墙灯：
    void CheckWallLedButton()
    {
        bool pressed = false;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            pressed = true;
        }
#endif
        for (int i = 0; i < Main.MAX_WALLLED && i < Set.setVal.WallLedNum; i++)
        {
            if (Key.KEYFJ_OkPressed(i) || pressed)
            {
                if (IO.wallLedValue[i] != 0 && FjData.g_Fj[0].RemainWallLed > 0)
                {
                    FjData.g_Fj[0].RemainWallLed--;
                    FjData.g_Fj[0].Scores += 1;
                    MusicManager.instance.Play_Bomb();
                    pressed = false;
                    //    CmdIO_YDGZ.CMD0_SendCmd_GameStatue(gameId, gameLevel, (int)statue);
                }
                IO.WallLED_One(i, 0);
            }
        }
    }
    int CurrRemainWallLed()
    {
        int count = 0;
        for (int i = 0; i < Main.MAX_WALLLED && i < Set.setVal.WallLedNum; i++)
        {
            if (IO.wallLedValue[i] != 0)
            {
                count++;
            }
        }
        return count;
    }

    void FreshWallLed()
    {
        int min = Set.setVal.WallLedNum / 4;
        int freshNum = Mathf.Clamp(Random.Range(min, min + 2), 1, Set.setVal.WallLedNum);
        if (freshNum > Main.MAX_WALLLED)
            freshNum = Main.MAX_WALLLED;
        if (freshNum > FjData.g_Fj[0].RemainWallLed)
            freshNum = FjData.g_Fj[0].RemainWallLed;
        int len;
        int id;
        int count = 0;
        int[] idBuf = new int[Main.MAX_WALLLED];
        for (; count < freshNum;)
        {
            len = 0;
            for (int i = 0; i < Set.setVal.WallLedNum && i < IO.wallLedValue.Length && i < idBuf.Length; i++)
            {
                if (IO.wallLedValue[i] == 0)
                {
                    idBuf[len] = i;
                    len++;
                }
            }
            if (len == 0)
            {
                break;
            }
            id = idBuf[Random.Range(0, len)];
            IO.WallLED_One(id, WALL_LED_OPEN);
            count++;
        }
    }
    void Update_WallLedNum()
    {
        wallLedNum = FjData.g_Fj[0].RemainWallLed;
        gameUIComm.Update_WallLedNum(wallLedNum);
    }

    void PlaySound(AudioClip audioClip)
    {
        audioSource_Others.Stop();
        audioSource_Others.clip = audioClip;
        audioSource_Others.Play();
    }
    public void PlaySound_YouYu()
    {
        audioSource_Others.Stop();
        audioSource_Others.clip = audioClip_YouYu;
        audioSource_Others.Play();
    }
    void PlaySound_Tips(AudioClip audioClip)
    {
        audioSource_Others_tips.Stop();
        audioSource_Others_tips.clip = audioClip;
        audioSource_Others_tips.Play();
    }

    void ShowReadyTime(int value)
    {
        for (int i = 0; i < GameLedControl.playerControl.Length && i < playerNum; i++)
        {
            GameLedControl.playerControl[i].ShowReadyTime(value);
        }
    }

    bool HasPlayerLoss()
    {
        for (int i = 0; i < playerNum; i++)
        {
            if (FjData.g_Fj[i].Life <= 0)
            {
                return true;
            }
        }
        return false;
    }
    bool AllPlayerPass()
    {
        for (int i = 0; i < playerNum; i++)
        {
            if (FjData.g_Fj[i].RemainPoint > 0 || FjData.g_Fj[0].RemainWallLed > 0)
            {
                return false;
            }
        }
        return true;
    }
    bool AllPlayerAddScoreFinish()
    {
        for (int i = 0; i < playerNum; i++)
        {
            if (player[i].AddScoreFinish() == false)
            {
                return false;
            }
        }
        return true;
    }


    //踩中目标点
    public bool HitTargetPoint(int no, int x, int y)
    {
        if (player[0].statue != en_Player00Sta.Play)
        {
            return false;

        }

        if (GameLedControl.playerControl[no].IsPlayerGamePoint(x, y) == false)
        {
            return false;

        }
        int picid = x + Set.setVal.Width * y;
        int pointid = Framebuffer.tab_Mapping[picid];
        if (GameLedControl.gamePoint[pointid].tarageTime > 0)
        {

            return false;
        }
        if (FjData.g_Fj[no].RemainPoint > 0 && FjData.g_Fj[no].Life > 0)
        {
            GameLedControl.gamePoint[pointid].tarageTime = 0.2f;
            FjData.g_Fj[no].RemainPoint--;
            Score_LinShi += 1;
            MusicManager.instance.Play_Correct();
            return true;
        }

        return false;


    }
    float errorCD = 1;
    // 踩中红点
    public bool HitDiePoint(int no, int x, int y)
    {
        if (player[0].statue != en_Player00Sta.Play)
            return false;
        if (GameLedControl.playerControl[no].IsPlayerGamePoint(x, y) == false)
            return false;




        return true;

        return false;
    }



    // -----------------------------------------------------------------------------
    bool IsAllPlayerEnd()
    {
        for (int i = 0; i < playerNum; i++)
        {
            if (player[0].statue != en_Player00Sta.Idle)
            {
                return false;
            }
        }
        return true;
    }


    // 震屏效果 ----------------------------------------------------
    int shakeCnt;
    float shakeTime;
    float shakePower;
    public void ShakeStart(float power, int cnt)
    {
        shakePower = power;
        shakeCnt = cnt;
    }
    public void ShakeStop()
    {
        shakeMain_Obj.transform.localPosition = new Vector3(0, 0, 0);
        shakeCnt = 0;
    }
    public void ShakeRun()
    {
        if (shakeCnt > 0)
        {
            shakeTime += Time.deltaTime;
            if (shakeTime >= 0.05f)
            {
                shakeTime = 0;
                //
                shakeCnt--;
                if (shakeCnt == 0)
                {
                    shakeMain_Obj.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    shakeMain_Obj.transform.localPosition = new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.8f, 0.8f), 0) * shakePower;
                }
            }
        }
    }


#if DEBUG_TEST
    void OnGUI()
    {
        //GUI.color = Color.yellow;

        //GUI.Label(new Rect(550, 100, 200, 20), "MonsterNum: " + monsterNum.ToString());
        //GUI.Label(new Rect(550, 130, 200, 20), "RemainNum: " + monsterRemainNum.ToString());
        //GUI.Label(new Rect(550, 160, 200, 20), "AliveNum: " + monsterAliveNum.ToString());
        ////GUI.Label(new Rect(550, 270, 200, 20), "monsterFreshPosId: " + gamePlay.monsterFreshPosId_Out.ToString());

        //GUI.Label(new Rect(550, 200, 200, 20), "BloodDcTime: " + daoJuDcTime[DAOJU_ID_BLOOD].ToString());
        //GUI.Label(new Rect(550, 220, 200, 20), "DaoDanDcTime: " + daoJuDcTime[DAOJU_ID_DAODAN].ToString());
    }
#endif
    List<Image> list_PresetPic = new List<Image>();
    void PresetPic_Init()
    {
        //while (list_PresetPic.Count>0)
        //{
        //    Destroy(list_PresetPic[0]);
        //}
        //if (presetPic_Layer.childCount>0)
        //{

        //    Destroy(presetPic_Layer.GetChild(0).gameObject);
        //}
        //   GameObject n = Instantiate(presetPic_Layer.gameObject, presetPic_Layer.transform);
        int len = Set.setVal.Width * Set.setVal.Height;
        // 去除多余的
        //        Debug.Log(list_PresetPic.Count);
        for (; list_PresetPic.Count > len;)
        {
            Destroy(list_PresetPic[0].gameObject);
            list_PresetPic.RemoveAt(0);
        }
        // 添加不足的
        for (int i = list_PresetPic.Count; i < len; i++)
        {
            Image image = Instantiate(picOne_Prefab, presetPic_Layer).GetComponent<Image>();
            list_PresetPic.Add(image);
        }
        int widthOne = 10;
        if (Set.setVal.Width > 0)
        {
            widthOne = 400 / Set.setVal.Width;
        }
        int heightOne = 10;
        if (Set.setVal.Height > 0)
        {
            heightOne = 500 / Set.setVal.Height;
        }
        if (widthOne > heightOne)
        {
            widthOne = heightOne;
        }
        if (widthOne > 50)
        {
            widthOne = 50;
        }
        int width = widthOne * Set.setVal.Width;
        int height = widthOne * Set.setVal.Height;
        int x = 0;
        int y = 0;
        for (int i = 0; i < list_PresetPic.Count; i++)
        {
            list_PresetPic[i].rectTransform.sizeDelta = new Vector2(widthOne, widthOne);
            list_PresetPic[i].transform.localPosition = new Vector3(x * widthOne - width * 0.5f, y * widthOne + height * 0.5f);
            list_PresetPic[i].color = Color.black;

            if (++x >= Set.setVal.Width)
            {
                x = 0;
                y++;
            }

        }
    }



    readonly Color[] tab_PointColor = { Color.black, Color.blue, Color.red, Color.green };
    void Update_PresetPic(byte[] dataBuf)
    {
        if (dataBuf == null)
        {
            presetPic_Layer.gameObject.SetActive(false);
            return;
        }

        presetPic_Layer.gameObject.SetActive(true);

        int id;
        int bufId;
        for (int i = 0; i < Set.setVal.Width && i < PresetPic.PIC_WIDTH; i++)
        {
            for (int j = 0; j < Set.setVal.Height && j < PresetPic.PIC_HEIGHT; j++)
            {
                id = j * Set.setVal.Width + i;
                bufId = j * PresetPic.PIC_WIDTH + i;
                if (bufId >= dataBuf.Length)
                    continue;
                if (id >= list_PresetPic.Count)
                    return;
                if (dataBuf[bufId] >= tab_PointColor.Length)
                {
                    list_PresetPic[id].color = Color.black;
                }
                else if (id < dataBuf.Length)
                {
                    list_PresetPic[id].color = tab_PointColor[dataBuf[bufId]];
                }
                else
                {
                    list_PresetPic[id].color = Color.black;
                }
            }
        }
    }
    public void NextJieDuan()
    {
        if (Main.IsDemo)
        {
            ChangeStatue(en_Game00_Sta.Out);
            return;
        }
        //  Game_Map00.instance.isCleaning = false;
        //   Game_Map00.instance.isClearAll = false;
        Index_JieDuan++;
        //if (player[0].playerUI.Ingame_Setting.set_MoveSpeed > 0.05f)
        //{

        //    player[0].playerUI.Ingame_Setting.list_GameSetOne[2].OnClick_Dec();

        //}
        gameUIComm.Update_JieDuan(Index_JieDuan);


        Game_Map04.instance.isClearAll = false;//是否清理了所有的点
                                               //int a = Random.Range(1, 10);
                                               //targetDieType = (en_TargetDieType)Main.gameSetting.gameLevelSetting[a].targetDieType;


        GameLedControl.ReadyStart(Set.gameSetting[0].gameLevelSetting[0], false);
        GameLedControl.PlayStart(Set.gameSetting[0].gameLevelSetting[0]);


        Game_Map04.instance.InitMap(gameLevel);
        //}
    }
}

