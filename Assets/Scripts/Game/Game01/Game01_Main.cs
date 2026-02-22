using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum en_Game01_Sta
{
    None = 0,
    Idle,
    Tips,
    WaitStart,
    ShowLevel,
    ShowPlayer,
    Ready,
    Play,           // 固定点
    Continue,
    ShowResult,
    ShowResultScore,
    WaitNextLevel,
    ShowWins,       // 单人结算奖励
    ShowWiner,      // 对战结算奖励
    InputName,
    RankList,
    End,
    Out,
    OutEnd,
}


public class Game01_Main : MonoBehaviour
{
    public int index_JieDuan = 0;
    public Game01_GameUI gameUI;
    public GameObject shakeMain_Obj;
    public Game01_Player player;
    public GameObject lifeOne_Prefab;
    // Prefab
    // 声音
    public AudioSource audioSource_BackG;       // 背景音乐
    public AudioSource audioSource_Others;
    //public AudioClip[] audioClip_BGM;    
    public AudioClip audioClip_ReadyTime;
    public AudioClip audioClip_ReadyGo;
    public AudioClip audioClip_Timeout;
    public AudioClip audioClip_TimesUp;
    public AudioClip audioClip_Pass;
    public AudioClip audioClip_Loss;



    Main main;
    public en_Game01_Sta statue;
    public float runTime = 0;
    int runCnt;
    float sendTime;

    int bgmIndex = 0;
    public int gameId = 56;
    public int setIndex;
    public int setCount;
    bool cmdRetSucess;
    bool isGameOver;

    int playerNum = Main.MAX_PLAYER;
    int playerId;
    public int gameLevel;
    int maxLevel;
    public int maxLife;
    int readyTime;
    public float gameTime;

    float remainGameTime;
    int coins;
    int startButtonId;
    RankOne rankOne;
    public int bigGameLevel;
    public int smallGameLevel;

    public static Game01_Main instance;
    public void Awake0(Main mainn)
    {
        instance = this;
        main = mainn;
        player.Awake0(this);
        gameUI.Awake0();
    }

    readonly int[] tab_PlayerId_Left = { 0, 1 };
    readonly int[] tab_PlayerId_Right = { 1, 0 };
    // Use this for initialization
    public void GameStart()
    {
        Game_Map01.instance.protectTime = 5;
        startButtonId = LedKey.GetLeiSheStartButtonId();
        playerNum = 1;
        index_JieDuan = 0;
        //FjData.rankList = RankList.LoadRankList (Set.gameLeiSheName[(int)Main.playerMode]);
        //if (Main.playerMode == (int)en_PlayerMode.Free) {
        //    playerNum = 1;
        //} else {
        //    playerNum = 2;
        //}
        // playerNum = Main.playerNum;
        player.playerUI = gameUI.playerUI;
        //
        gameUI.gameObject.SetActive(true);
        gameUI.GameStart();
        Game_Map01.instance.Initmap();
        //
        if (Main.IsDemo == false)
        {
            if (Set.setVal.BraceletMode != 0 && Main.currUser != null)
            {
                Main.currUser.playCnt++;
                UserManager.SaveData(Main.currUser.cardId, Main.currUser);
            }
            FjData.acc[0].PlayCnt++;
            FjData.totalAcc[0].PlayCnt++;
            FjData.SaveAcc_PlayCnt(0, false);
            FjData.SaveTotalAcc_PlayCnt(0);
        }

        for (int i = 0; i < Main.MAX_PLAYER; i++)
        {
            FjData.g_Fj[i].Scores = 0;
            FjData.g_Fj[i].Result = 1;
        }
#if UNITY_EDITOR
        Debug.LogError("PlayerNum: " + Main.playerNum);
#endif
        //   remainGameTime = FjData.g_Fj[0].GameTime;
        player.GameStart(0);//GameStart
        maxLevel = Mathf.Min(Main.gameSetting.maxLevel, Main.gameSetting.gameLevelSetting.Length);
        gameLevel = 0;
        gameLevel = Main.gameLevel;

        if (Set.setVal.StartCoins > 0 || Set.setVal.TimeEnable != 0)
        {
            gameUI.gameTime.gameObject.SetActive(true);
        }
        else
        {
            gameUI.gameTime.gameObject.SetActive(false);
        }

        if (Main.IsDemo)
        {
            ChangeStatue(en_Game01_Sta.Play);
        }
        else
        {
            ChangeStatue(en_Game01_Sta.Tips);
        }
    }

    void GameContinue()
    {
        player.Continue();
        statue = en_Game01_Sta.Play;
        //
        gameUI.coinIn.gameObject.SetActive(false);
        gameUI.playerId_Obj.SetActive(false);
        gameUI.continue_Obj.SetActive(false);
        gameUI.time_Obj.SetActive(true);
        bigGameLevel = gameLevel / 10;
        smallGameLevel = gameLevel % 10;

        gameTime = Main.gameSetting.gameLevelSetting[gameLevel].gameTime;

        gameUI.Update_RemainTime((int)gameTime);
        audioSource_BackG.Play();
        PlaySound(audioClip_ReadyGo);
    }

    int rTime = 0;

    void Update()
    {


        remainGameTime = Main.PlayTime;
        if (Main.IsDemo == false)
        {
            if (Set.setVal.StartCoins > 0 || Set.setVal.TimeEnable != 0)
            {
                if (remainGameTime > 0)
                {

                }
                else if (statue <= en_Game01_Sta.Play)
                {

                    ChangeStatue(en_Game01_Sta.ShowResult);
                }
            }
        }

        switch (statue)
        {
            case en_Game01_Sta.Tips:
                runTime += Time.deltaTime;
                if (runTime >= 2f)
                {
                    runTime = 0;
                    ChangeStatue(en_Game01_Sta.WaitStart);
                }
                break;

            case en_Game01_Sta.WaitStart:
                StartButtonLed_Run();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //player.ledControl.LedInit(Main.gameSetting.gameLevelSetting[gameLevel]);
                }
                //
                if (LedKey.KeyPressed(startButtonId) || gameUI.levelStarted || Input.GetKeyDown(KeyCode.O))
                {
                    ChangeStatue(en_Game01_Sta.ShowLevel);
                }
                break;

            case en_Game01_Sta.ShowLevel:
                runTime += Time.deltaTime;
                if (runTime >= 1.5f)
                {
                    runTime = 0;
#if UNITY_EDITOR && false
                ChangeStatue (en_Game01_Sta.Play);
#else
                    if (playerNum > 1)
                    {
                        ChangeStatue(en_Game01_Sta.ShowPlayer);
                    }
                    else
                    {
                        ChangeStatue(en_Game01_Sta.Ready);
                    }
#endif
                }
                break;

            case en_Game01_Sta.ShowPlayer:
                runTime += Time.deltaTime;
                if (runTime >= 2f)
                {
                    runTime = 0;
                    ChangeStatue(en_Game01_Sta.Ready);
                }
                break;

            case en_Game01_Sta.Ready:
                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    if (readyTime != (int)runTime)
                    {
                        readyTime = (int)runTime;
                        gameUI.Update_ReadyTime(readyTime);
                        if (readyTime > 0)
                        {
                            //CmdIO_YDGZ.CMD0_SendCmd_ReadyTime (gameId, gameLevel, readyTime);
                            PlaySound(audioClip_ReadyTime);
                        }
                        else
                        {
                            //PlaySound (audioClip_ReadyGo);
                        }
                        player.RunLedAnim();
                        if (readyTime == 0)
                        {
                            ChangeStatue(en_Game01_Sta.Play);
                        }
                    }
                }
                else
                {
                    //PlaySound (audioClip_ReadyGo);
                    ChangeStatue(en_Game01_Sta.Play);
                }
                break;

            case en_Game01_Sta.Play:
                if (player.statue >= en_Player01Sta.Pass)
                {
                    Debug.LogError("00");

                    //result = 1;
                    if (Main.IsDemo)
                    {
                        ChangeStatue(en_Game01_Sta.Out);
                    }
                    else
                    {
                        ChangeStatue(en_Game01_Sta.ShowResult);
                    }
                    break;
                }
                if (player.statue > en_Player01Sta.Play)
                {
                    break;
                }//
                if (/*Main.playerMode != en_PlayerMode.Free*/true)
                {
                    if (gameTime > 0)
                    {
                        gameTime -= Time.deltaTime;

                        //
                        gameUI.Update_RemainTime(rTime);
                        //CmdIO_YDGZ.CMD0_SendCmd_GameStatue (gameId, gameLevel, (int)statue);
                        //
                        if (rTime != (int)gameTime)
                        {
                            rTime = (int)gameTime;
                            if (rTime <= 5)
                            {
                                if (rTime > 0)
                                {
                                    PlaySound(audioClip_Timeout);
                                }
                                else
                                {
                                    PlaySound(audioClip_TimesUp);
                                }
                            }
                        }


                    }
                    else
                    {
                        if (Main.IsDemo)
                        {
                            ChangeStatue(en_Game01_Sta.Out);
                        }
                        else
                        {
                            ChangeStatue(en_Game01_Sta.ShowResult);
                        }
                        break;
                    }
                }
                //
                if (player.statue == en_Player01Sta.Play)
                {
                }
                break;

            case en_Game01_Sta.ShowResult:
                runTime += Time.deltaTime;
                if (runTime >= 3)
                {
                    //if (player.statue == en_Player1Sta.Loss && Main.playerMode == (int)en_PlayerMode.Free) {
                    //    ChangeStatue (en_Game01_Sta.Continue);
                    //} else {
                    //    ChangeStatue (en_Game01_Sta.ShowResultScore);
                    //}
                    ChangeStatue(en_Game01_Sta.ShowResultScore);
                }
                break;

            case en_Game01_Sta.Continue:
                if (coins != FjData.g_Fj[0].Coins)
                {
                    Update_CoinsTips();
                }

                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    if (runCnt != (int)runTime)
                    {
                        runCnt = (int)runTime;
                        gameUI.Update_ContinueTime(runCnt);
                    }
                }
                if (runTime <= 0 || gameUI.continueResult == 2)
                {
                    ChangeStatue(en_Game01_Sta.ShowResultScore);
                    break;
                }
                if (gameUI.continueResult == 1)
                {
                    // 确认续玩：
                    if (Main.IsDemo == false)
                    {
                        if (Main.DecStartCoin(0))
                        {
                            if (Set.setVal.BraceletMode != 0 && Main.currUser != null)
                            {
                                Main.currUser.playCnt++;
                                UserManager.SaveData(Main.currUser.cardId, Main.currUser);
                            }
                            FjData.acc[0].PlayCnt++;
                            FjData.totalAcc[0].PlayCnt++;
                            FjData.SaveAcc_PlayCnt(0, false);
                            FjData.SaveTotalAcc_PlayCnt(0);
                            //
                            GameContinue();
                            //ChangeStatue (en_Game01_Sta.ShowResultScore);
                            break;
                        }
                    }
                }
                gameUI.continueResult = 0;
                break;

            case en_Game01_Sta.ShowResultScore:
                if (player.AddScoreFinish() == false)
                    break;
                if (isGameOver && Main.playerMode == en_PlayerMode.Free && runCnt == 0)
                {
                    MusicManager.instance.Play_Talk(4, 0.5f); // "游戏结束"
                    runCnt = 1;
                }
                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    break;
                }

                if (isGameOver)
                {
                    if (Main.playerMode == en_PlayerMode.PassLevel) {
                       ChangeStatue (en_Game01_Sta.ShowWiner);
                    }
                    // 用玩家最高分记录当前分：
                    int maxId = 0;
                    for (int i = 0; i < playerNum && i < Main.MAX_PLAYER; i++)
                    {
                        if (FjData.g_Fj[i].Scores > FjData.g_Fj[maxId].Scores)
                        {
                            maxId = i;
                        }
                    }
                    FjData.g_Fj[0].Scores = FjData.g_Fj[maxId].Scores;
                    //
                    bool hasUser = false;
                    if (Set.setVal.BraceletMode != 0 && Main.currUser != null)
                    {
                        if (FjData.g_Fj[0].Scores > Main.currUser.maxScore)
                        {
                            Main.currUser.maxScore = FjData.g_Fj[0].Scores;
                        }
                        Main.currUser.AddUseRecordOne(gameLevel, FjData.g_Fj[0].Scores);
                        Main.currUser.AddScoresRank(gameLevel, FjData.g_Fj[0].Scores);
                        UserManager.SaveData(Main.currUser.cardId, Main.currUser);
                        gameUI.playerNameInput.playerName = Main.currUser.userName;
                        hasUser = true;
                    }
                    if (FjData.rankList.UpList(FjData.g_Fj[0].Scores))
                    {
                        if (hasUser)
                        {
                            ChangeStatue(en_Game01_Sta.RankList);
                        }
                        else
                        {
                            ChangeStatue(en_Game01_Sta.InputName);
                        }
                        break;
                    }
                    ChangeStatue(en_Game01_Sta.End);
                    //} 
                }
                //else if (playerId < playerNum && playerNum > 1)
                //{
                //    ChangeStatue(en_Game01_Sta.ShowPlayer);
                //}
                else
                {
                    ChangeStatue(en_Game01_Sta.WaitNextLevel);
                }
                break;

            case en_Game01_Sta.WaitNextLevel:
                StartButtonLed_Run();
                //
                if (LedKey.KeyPressed(startButtonId))
                {
                    Debug.Log("等待进入下一关");
                    runTime = 0;
                }
                if (runTime > 0 && gameUI.levelStarted == false)
                {
                    runTime -= Time.deltaTime;
                    if (readyTime != (int)runTime)
                    {
                        readyTime = (int)runTime;
                        gameUI.Update_LevelWaitTime(readyTime);
                    }
                }
                else
                {

                    gameLevel++;
                    if (gameLevel >= 10)
                    {
                        gameLevel = 0;
                        Debug.LogError(Main.playerMode);
                        Main.playerMode++;
                        if (Main.playerMode > en_PlayerMode.Challenge)
                        {
                            Main.playerMode = 0;
                        }
                        Game97_LevelSel.instance.OnClick_Level(1);

                    }
                    ChangeStatue(en_Game01_Sta.ShowLevel);
                }
                break;

            case en_Game01_Sta.ShowWins:
                runTime += Time.deltaTime;
                if (runTime >= 4)
                {
                    if (FjData.rankList.UpList(FjData.g_Fj[0].Scores))
                    {
                        ChangeStatue(en_Game01_Sta.InputName);
                        break;
                    }
                    ChangeStatue(en_Game01_Sta.End);
                }
                break;

            case en_Game01_Sta.ShowWiner:
                runTime += Time.deltaTime;
                if (runTime >= 8)
                {
                    ChangeStatue(en_Game01_Sta.Out);
                }
                break;

            case en_Game01_Sta.InputName:
                if (gameUI.playerNameInput.gameObject.activeSelf)
                    break;
                ChangeStatue(en_Game01_Sta.RankList);
                break;

            case en_Game01_Sta.RankList:
                runTime += Time.deltaTime;
                if (runTime >= 8)
                {
                    ChangeStatue(en_Game01_Sta.Out);
                }
                break;

            case en_Game01_Sta.End:
                runTime += Time.deltaTime;
                if (runTime >= 1)
                {
                    ChangeStatue(en_Game01_Sta.Out);
                }
                break;


            case en_Game01_Sta.Out:

                runTime += Time.deltaTime;
                if (runTime >= 0.3f)
                {
                    //Game97_Main_01.gameOver.Awake0(this);
                    //Game97_Main_01.gameOver.gameObject.SetActive(true);
                    //GameOver();
                    ChangeStatue(en_Game01_Sta.OutEnd);
                    //Game97_Main_01.gameOver.Awake0(this);
                    Main.instance.ChangeStatue(en_MainStatue.Game_97);
                    if (Main.settingError != en_ErrorCode.None || Main.PlayTime < 0)
                    {
                        return;
                    }
                    Main.instance.game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
                }
                break;
        }
    }

    public void ChangeStatue(en_Game01_Sta sta)
    {
#if UNITY_EDITOR
        Debug.Log("GameSta: " + sta);
#endif
        statue = sta;
        runTime = 0;
        runCnt = 0;
        sendTime = 0;
        setIndex = 0;
        setCount = 0;
        cmdRetSucess = false;

        //CmdIO_YDGZ.CMD0_SendCmd_GameStatue (gameId, gameLevel, (int)statue);


        //gameUI.time_Obj.SetActive (false);
        gameUI.coinIn.gameObject.SetActive(false);
        gameUI.pleaseCoin_Obj.SetActive(false);
        gameUI.tips_Obj.SetActive(false);
        gameUI.button_LevelStart.gameObject.SetActive(false);
        gameUI.showLevel_Obj.SetActive(false);
        gameUI.showPlayer_Obj.SetActive(false);
        gameUI.image_ReadyTime.gameObject.SetActive(false);
        gameUI.continue_Obj.SetActive(false);
        gameUI.levelWaitTime_Obj.SetActive(false);
        gameUI.resultWinner.gameObject.SetActive(false);
        gameUI.resultWins.gameObject.SetActive(false);
        gameUI.levelStarted = false;

        StartButtonLedOut(0);//

        switch (statue)
        {
            case en_Game01_Sta.Idle:
                gameUI.time_Obj.SetActive(false);
                player.ChangeStatue(en_Player01Sta.Idle);
                break;

            case en_Game01_Sta.Tips:
                gameUI.tips_Obj.SetActive(true);
                break;

            case en_Game01_Sta.WaitStart:
                gameUI.tips_Obj.SetActive(true);
                gameUI.button_LevelStart.gameObject.SetActive(true);
                break;

            case en_Game01_Sta.ShowLevel:
                gameUI.time_Obj.SetActive(false);
                gameUI.level_Obj.SetActive(false);
                gameUI.playerId_Obj.SetActive(false);
                gameUI.showLevel_Obj.SetActive(true);
                gameUI.Update_ShowLevel(gameLevel);

                //for (int i = 0; i < playerNum && i < player.Length; i++) {
                //    player[i].GameStart (maxLife, playerNum);
                //}

                playerId = 0;
                if (Main.playerMode == en_PlayerMode.PassLevel && gameLevel > 0)
                {
                    for (int i = 0; i < playerNum; i++)
                    {
                        if (FjData.g_Fj[i].Result > 0)
                        {
                            playerId = i;
                            break;
                        }
                    }
                }
                player.GameStart(playerId);
                //
                audioSource_BackG.Stop();
                //PlaySound (audioClip_ShowLevel);
                MusicManager.instance.Play_ShowLevel();
                break;

            case en_Game01_Sta.ShowPlayer:
                gameUI.time_Obj.SetActive(false);
                gameUI.level_Obj.SetActive(false);
                gameUI.playerId_Obj.SetActive(false);
                gameUI.showPlayer_Obj.SetActive(true);
                gameUI.Update_ShowPlayer(playerId);
                //
                player.GameStart(playerId);
                break;

            case en_Game01_Sta.Ready:
                gameUI.level_Obj.SetActive(true);
                gameUI.Update_Level(gameLevel);
                if (playerNum > 1)
                {
                    gameUI.playerId_Obj.SetActive(true);
                    gameUI.Update_PlayerId(playerId);
                }
                else
                {
                    gameUI.playerId_Obj.SetActive(false);
                }
                runTime = 4;
                readyTime = 3;
                gameUI.Update_ReadyTime(readyTime);
                PlaySound(audioClip_ReadyTime);
                break;

            case en_Game01_Sta.Play:
                gameUI.level_Obj.SetActive(true);
                gameUI.Update_Level(gameLevel);
                if (playerNum > 1)
                {
                    gameUI.playerId_Obj.SetActive(true);
                    gameUI.Update_PlayerId(playerId);
                }
                else
                {
                    gameUI.playerId_Obj.SetActive(false);
                }
                if (/*Main.playerMode != en_PlayerMode.Free*/true)
                {
                    gameUI.time_Obj.SetActive(true);
                    //if (playerNum <= 1)
                    //{
                    //    // 不显示玩家号：
                    //    gameUI.time_Obj.transform.localPosition = new Vector3(0, 0);
                    //}
                    //else if (Main.gameSetting.gameLevelSetting[gameLevel].life <= 10)
                    //{
                    //    // 1排
                    //    gameUI.time_Obj.transform.localPosition = new Vector3(0, -70);
                    //}
                    //else
                    //{
                    //    // 2排
                    //    gameUI.time_Obj.transform.localPosition = new Vector3(0, -180);
                    //}
                }

                gameTime = 0;
                gameTime = Main.gameSetting.gameLevelSetting[gameLevel].gameTime;
                //  gameTime = Set.gameSetting[bigGameLevel].gameLevelSetting[smallGameLevel].gameTime;

                gameUI.Update_RemainTime((int)gameTime);

                player.PlayStart();
                MusicManager.instance.Play_Talk(0, 1.0f); // "挑战开始"
                                                          //
                if (++bgmIndex >= MusicManager.instance.audioClip_Leishe_BGM.Length)
                {
                    bgmIndex = 0;
                }
                audioSource_BackG.Stop();
                //audioSource_BackG.clip = MusicManager.instance.audioClip_BGM[bgmIndex];
                audioSource_BackG.clip = MusicManager.instance.GetAudioClip_LeiShe_Game();
                audioSource_BackG.PlayDelayed(2);
                //
                PlaySound(audioClip_ReadyGo);
                break;

            case en_Game01_Sta.ShowResult:
                gameUI.time_Obj.SetActive(false);
                gameUI.gameTime.gameObject.SetActive(false);
                player.ShowResult();
                //
                audioSource_BackG.Stop();

                if (Main.playerMode == (int)en_PlayerMode.Free)
                {
                    if (player.result == 0)
                    {
                        PlaySound(audioClip_Loss);
                    }
                    else
                    {
                        PlaySound(audioClip_Pass);
                    }
                }
                break;

            case en_Game01_Sta.Continue:
                gameUI.continue_Obj.SetActive(true);
                if (Set.setVal.StartCoins > 0)
                {
                    gameUI.coinIn.gameObject.SetActive(true);
                }
                Update_CoinsTips();
                //
                runCnt = 20;
                runTime = runCnt + 1;
                gameUI.Update_ContinueTime(runCnt);
                gameUI.continueResult = 0;
                break;

            case en_Game01_Sta.ShowResultScore:
                gameUI.level_Obj.SetActive(false);
                gameUI.playerId_Obj.SetActive(false);
                player.ShowResultScore();

                isGameOver = false;
                runTime = 1.5f;
                NextPlayerId();
                if (IsGameOver())
                {
                    isGameOver = true;
                    runTime = 3.0f;
                }
                break;

            case en_Game01_Sta.WaitNextLevel:
                gameUI.levelWaitTime_Obj.SetActive(true);
                gameUI.button_LevelStart.gameObject.SetActive(true);
                readyTime = Set.setVal.LevelWaitTime;
                runTime = readyTime + 0.9f;
                gameUI.Update_LevelWaitTime(readyTime);

                MusicManager.instance.Play_ShowLevel();
                audioSource_BackG.PlayDelayed(1.5f);
                MusicManager.instance.Play_Talk(3, 0.5f); // "准备进入下一关"
                break;

            case en_Game01_Sta.ShowWins:
                gameUI.resultWins.gameObject.SetActive(true);
                int win = Main.JieSuanScore(playerId);
                gameUI.resultWins.Update_Result(gameLevel, FjData.g_Fj[playerId].Scores, win);
                player.ChangeStatue(en_Player01Sta.GameOver);
                MusicManager.instance.Play_Talk(4, 2); // "游戏结束"
                break;

            case en_Game01_Sta.ShowWiner:
                gameUI.playerId_Obj.SetActive(false);
                gameUI.resultWinner.gameObject.SetActive(true);
                int[] wins = new int[Main.MAX_PLAYER];
                for (int i = 0; i < wins.Length; i++)
                {
                    wins[i] = Main.JieSuanScore(i);
                }
                gameUI.resultWinner.Update_Value();
                player.ChangeStatue(en_Player01Sta.GameOver);
                MusicManager.instance.Play_Talk(4, 1.5f); // "游戏结束"
                break;

            case en_Game01_Sta.InputName:
                gameUI.playerNameInput.gameObject.SetActive(true);
                gameUI.playerNameInput.GameStart(6, null);

                audioSource_BackG.clip = MusicManager.instance.audioClip_EndEff;
                audioSource_BackG.Play();
                break;

            case en_Game01_Sta.RankList:
                gameUI.rankList.gameObject.SetActive(true);
                rankOne.playerName = gameUI.playerNameInput.GetName();
                rankOne.score = FjData.g_Fj[0].Scores;
                rankOne.level = gameLevel;
                //
                int rank = FjData.rankList.AddOne(rankOne);
                RankList.SaveRankList(Set.gameName[(int)Main.playerMode], FjData.rankList);
                //
                gameUI.rankList.UpdateValue(rank, FjData.rankList);

                if (audioSource_BackG.clip != MusicManager.instance.audioClip_EndEff)
                {
                    audioSource_BackG.clip = MusicManager.instance.audioClip_EndEff;
                    audioSource_BackG.Stop();
                }
                if (audioSource_BackG.isPlaying == false)
                {
                    audioSource_BackG.Play();
                }
                break;

            case en_Game01_Sta.End:
                break;

            case en_Game01_Sta.Out:
                break;
        }
    }

    void PlaySound(AudioClip audioClip)
    {
        audioSource_Others.Stop();
        audioSource_Others.volume = (float)Set.setVal.MainSoundVolume / 10;
        audioSource_Others.clip = audioClip;
        audioSource_Others.Play();
    }



    // -----------------------------------------------------------------------------
    void NextPlayerId()
    {
        for (; ; )
        {
            playerId++;
            if (playerId >= FjData.g_Fj.Length)
                break;
            if (playerId >= playerNum)
                break;
            if (Main.playerMode != en_PlayerMode.PassLevel)
            {
                break;
            }
            if (FjData.g_Fj[playerId].Result > 0)
            {
                break;
            }
        }
    }
    bool IsGameOver()
    {
        //if (Main.playerMode != en_PlayerMode.Free && gameLevel + 1 >= maxLevel)
        //    return true;
        // 有1个玩家失败，游戏结束
        for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
        {
            if (FjData.g_Fj[i].Result == 0)
            {
                return true;
            }
        }
        if (playerId < playerNum)
            return false;
        return false;
    }
    // 对战模式下：
    bool IsGameOver_ByBatlle()
    {
        if (gameLevel + 1 >= maxLevel)
            return true;
        if (Main.playerMode == en_PlayerMode.PassLevel)
        {
            if (playerId < playerNum)
                return false;
            // 剩余玩家少于2，游戏结束
            int remainPlayer = 0;
            for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
            {
                if (FjData.g_Fj[i].Result > 0)
                {
                    remainPlayer++;
                }
            }
            if (remainPlayer < 2)
            {
                return true;
            }
        }
        else
        {
            // 有1个玩家失败，游戏结束
            for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
            {
                if (FjData.g_Fj[i].Result == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool AllPlayerPass()
    {
        for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
        {
            if (FjData.g_Fj[i].Result == 0)
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

    static float startButtonLedTime = 0;
    static uint startButtonLedSta = 0;

    public static void StartButtonLedOut(uint color)
    {
        Framebuffer.Update_TargetLedColor(Set.ChannelLength[4] - 2, color);
    }
    public static void StartButtonLed_Run()
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

    void Update_CoinsTips()
    {
        coins = FjData.g_Fj[0].Coins;
        if (coins >= Set.setVal.StartCoins)
        {
            gameUI.pleaseCoin_Obj.SetActive(false);
        }
        else
        {
            gameUI.pleaseCoin_Obj.SetActive(true);
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

    public void CheckPass()
    {


        Framebuffer.Update_TargetLedColor(0, 0xff0000);

      

         
    }
}
