using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum en_Player01Sta
{
    Idle = 0,           // 空闲
    ReadyTargetLed,     // 准备目标灯(从远到近依次点亮)
    PlayTargetLed,      // 目标灯(打完所有目标灯后，开始正式游戏)
    ReadyPlayLed,       // 准备游戏灯(从近到远依次显示游戏图案)
    Play,               // 游戏中
    WaitPass,
    Die,                // 死亡    
    Pass,
    Loss,
    ResultScore,
    ShowWinner,
    GameOver,
}

public class Game01_Player : MonoBehaviour
{
    //#if UNITY_EDITOR
    //    const int MAX_PLAYER_BLOOD = 2000;
    //#else
    //    const int MAX_PLAYER_BLOOD = 1000;
    //#endif
    const float MAX_SPECIALATTCK_TIME = 3;

    public Canvas canvas;
    public Game01_PlayerUI playerUI;

    public Game01_Main gameMain;
    public int Id;
    public int startx;
    public int starty;
    public int width;
    public int height;
    public int result;
    public int gameLevel;
    bool isWiner;
    public static int rxKeyStartId;
    static int targetKeyId;
    static int wallKeyId;

    public en_Player01Sta statue;
    public en_Player01Sta oldStatue;
    float runTime;
    public int runCnt;
    public int[] tarageLED_List = new int[300];
    public GameLeiSheLedControl ledControl = new GameLeiSheLedControl();
    int ch = 4;
    float errorCD = 1;
    public int maxJieDuan = 4;
    public int currJieDuan = 0;
    int targetNum;
    public void Awake0(Game01_Main gmain)
    {
        gameMain = gmain;
        if (playerUI != null)
        {
            playerUI.Awake0(Id);
        }
    }
    // Use this for initialization
    public void GameStart(int no)
    {
        currJieDuan = 0;

        Id = no;
        //
        startx = 0;
        starty = 0;
        width = Set.setVal.Width;
        height = Set.setVal.Height;
        //
        FjData.g_Fj[Id].Level = gameMain.gameLevel;
        FjData.g_Fj[Id].Result = 0;
        FjData.g_Fj[Id].Life = 100;// Main.gameSetting.gameLevelSetting[gameMain.gameLevel].life;
        //FjData.g_Fj[Id].RemainPoint = Set.gameSetting.gameLevelSetting[gameMain.gameLevel].targetPoint;
        FjData.g_Fj[Id].RemainPoint = 1;
        FjData.g_Fj[Id].TargetLed = Mathf.Min(Set.setVal.WallLedNum, Main.gameSetting.gameLevelSetting[gameMain.gameLevel].wallLedNum);
        //FjData.g_Fj[Id].GameTime = Set.gameSetting.gameLevelSetting[gameMain.gameLevel].gameTime;
        //
        if (playerUI != null)
        {
            playerUI.GameStart(Id);
            playerUI.Update_MaxLife(FjData.g_Fj[Id].Life);
        }

        rxKeyStartId = LedKey.GetLeiSheKeyStartId();
        targetKeyId = LedKey.GetLeiSheTargetKeyId();
        wallKeyId = LedKey.GetLeiSheWallLedKeyId();
        for (int i = 0; i < tarageLED_List.Length; i++)
        {
            tarageLED_List[i] = -1;
        }
        ledControl.LedInit(Main.gameSetting.gameLevelSetting[gameMain.gameLevel]);
        ChangeStatue(en_Player01Sta.Idle);

    }

    public void Ready()
    {
        ChangeStatue(en_Player01Sta.Idle);
    }
    public void Continue()
    {
        //ChangeStatue (en_Player1Sta.Play);
        FjData.g_Fj[Id].Life = Main.gameSetting.gameLevelSetting[gameMain.gameLevel].life;
        PlayStart();
    }

    public void PlayStart()
    {
        for (int i = 0; i < protectTime.Length; i++)
        {
            protectTime[i] = 2f - 0.02f * Set.setVal.LeiShe_LMD;
        }
        GameLeiSheBase.Update_ColorFull(0, enPointSta.None);
        if (FjData.g_Fj[Id].TargetLed > 0)
        {
            Debug.LogError(0);

            ChangeStatue(en_Player01Sta.ReadyTargetLed);
        }
        else
        {
            ChangeStatue(en_Player01Sta.ReadyPlayLed);
        }
    }

    // Update is called once per frame
    void Update()
    {

        Test();

        switch (statue)
        {
            case en_Player01Sta.Idle:
                break;

            case en_Player01Sta.ReadyTargetLed:

                CheckRxKey();
                //if (FjData.g_Fj[Id].Life <= 0)
                //{
                //    ChangeStatue(en_Player01Sta.Die);
                //    break;
                //}
                //
                runTime += Time.deltaTime;
                if (runTime >= 0.03f)
                {
                    runTime = 0;
                    if (runCnt < Set.setVal.Width)
                    {
                        byte value;
                        for (int i = 0; i < Set.setVal.Height; i++)
                        {
                            if ((runCnt % 2) == (i % 2))
                            {
                                value = 63;
                            }
                            else
                            {
                                value = 0;
                            }
                            GameLeiSheBase.Update_PointColor(runCnt, i, value, enPointSta.None);
                        }
                        runCnt++;
                    }
                    else
                    {
                        ChangeStatue(en_Player01Sta.PlayTargetLed);
                    }
                }
                break;

            case en_Player01Sta.PlayTargetLed:

                runTime += Time.deltaTime;
                if (runTime >= 3 || GetCurrTargetLed() == 0)
                {
                    runTime = 0;
                    ShowTargetLed();
                }

                CheckRxKey();
                CheckWallLedKey();
                if (FjData.g_Fj[Id].Life <= 0)
                {
                    ChangeStatue(en_Player01Sta.Die);
                    break;
                }
                if (FjData.g_Fj[Id].TargetLed <= 0)
                {
                    ChangeStatue(en_Player01Sta.ReadyPlayLed);
                    break;
                }

                break;

            case en_Player01Sta.ReadyPlayLed:
                CheckRxKey();
                if (FjData.g_Fj[Id].Life <= 0)
                {
                    ChangeStatue(en_Player01Sta.Die);
                    break;
                }
                float oldAngle = playerUI.target_Obj.transform.eulerAngles.y;
                playerUI.target_Obj.transform.eulerAngles = Vector3.MoveTowards(playerUI.target_Obj.transform.eulerAngles, new Vector3(0, 180), 360 * Time.deltaTime);
                if (oldAngle < 90 && playerUI.target_Obj.transform.eulerAngles.y >= 90)
                {
                    playerUI.remainPoint_Obj.SetActive(false);
                    playerUI.targetButton_Obj.SetActive(true);
                }
                //
                runTime += Time.deltaTime;
                if (runTime >= 0.03f)
                {
                    runTime = 0;
                    if (runCnt >= 0)
                    {
                        for (int i = 0; i < Set.setVal.Height; i++)
                        {
                            if (ledControl.GetPicPointValue(runCnt, i) == 0)
                            {
                                GameLeiSheBase.Update_PointColor(runCnt, i, 0, enPointSta.None);
                            }
                            else
                            {
                                GameLeiSheBase.Update_PointColor(runCnt, i, GameLeiSheLedControl.LEISHE_COLOR, enPointSta.Die);
                            }
                        }
                        runCnt--;
                    }
                    else if (playerUI.target_Obj.transform.eulerAngles == new Vector3(0, 180))
                    {
                        ChangeStatue(en_Player01Sta.Play);
                    }
                }
                break;

            case en_Player01Sta.Play:
                //TODO:游戏状态
                runTime += Time.deltaTime;
                if (runTime < 0.2f)
                {
                    return;
                }
           //     Debug.LogError("currJieDuan: " + currJieDuan+"   "+Game01_Main.instance.index_JieDuan);

                switch (currJieDuan)
                {
                    case 0:
                        UpdateLed(4);
                        ch = 4;
                        for (int i = 0; i < Set.ChannelLength[0]; i++)
                        {
                            Framebuffer.Update_TransmitLedColor(i, 0x60, enPointSta.Target);
                        }
                        CheckLedKey(4);
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            tarageNum = 0;
                        }
                        if (tarageNum <= 0)
                        {
                            MusicManager.instance.Play_Correct();
                            currJieDuan++;
                            Game_Map01.instance.protectTime = 3;
                            // Debug.LogError("currJieDuan: " + currJieDuan);
                            ChangeStatue(en_Player01Sta.WaitPass);
                        }
                        break;
                    case 1:
                        targetKeyId = (Set.ChannelLength[0] + Set.ChannelLength[1] + Set.ChannelLength[2] + Set.ChannelLength[3] + Set.ChannelLength[4]) - 1;
                     //   RunLedAnim();
                        if (LedKey.KeyPressed(targetKeyId) || Input.GetKeyDown(KeyCode.Q))
                        {
                            MusicManager.instance.Play_Correct();
                            currJieDuan++;
                            //   Debug.LogError("currJieDuan: " + currJieDuan);
                            Game_Map01.instance.protectTime = 3;

                            ChangeStatue(en_Player01Sta.WaitPass);
                            break;
                        }
                        break;
                    case 2:
                        switch (Main.gameLevel)
                        {
                            default:
                                UpdateLed(5);

                                CheckLedKey(5);
                                break;
                                //case 1:
                                //    RunMap_FindSame();
                                //    break;
                        }
                        ch = 5;
                        for (int i = 0; i < Set.ChannelLength[0]; i++)
                        {
                            Framebuffer.Update_TransmitLedColor(i, 0x60, enPointSta.Target);
                        }
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            tarageNum = 0;
                        }
                        if (tarageNum <= 0)
                        {
                            MusicManager.instance.Play_Correct();
                            currJieDuan++;
                            Game_Map01.instance.protectTime = 3;

                            ChangeStatue(en_Player01Sta.WaitPass);
                        }
                        break;
                    case 3:
                        targetKeyId = (Set.ChannelLength[0] + Set.ChannelLength[1] + Set.ChannelLength[2] + Set.ChannelLength[3] + Set.ChannelLength[4]) - 2;
                    //    RunLedAnim();
                        if (LedKey.KeyPressed(targetKeyId) || Input.GetKeyDown(KeyCode.Q))
                        {
                            MusicManager.instance.Play_Correct();
                            Game_Map01.instance.protectTime = 3;
                            currJieDuan = 0;
                            ChangeStatue(en_Player01Sta.Pass);
                            break;
                        }
                        break;
                }
                errorCD -= Time.deltaTime;

                TargetLed_Run();
#if UNITY_EDITOR //&& false
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    MusicManager.instance.Play_Correct();
                    ChangeStatue(en_Player01Sta.WaitPass);
                    break;
                }

#endif

                CheckRxKey();
                if (FjData.g_Fj[Id].Life <= 0)
                {
                    ChangeStatue(en_Player01Sta.Die);
                    break;
                }
                break;

            case en_Player01Sta.WaitPass:
                runTime += Time.deltaTime;
                if (runCnt < Set.setVal.Width)
                {
                    if (runTime >= 0.03f)
                    {
                        runTime = 0;
                        for (int i = 0; i < Set.setVal.Height; i++)
                        {
                            GameLeiSheBase.Update_PointColor(runCnt, i, 0, enPointSta.None);
                        }
                        runCnt++;
                    }
                }
                else if (runTime > 0.5f)
                {
                    if (currJieDuan >= maxJieDuan)
                    {
                        ChangeStatue(en_Player01Sta.Pass);
                    }
                    else
                    {
                        ledControl.LedInit(Main.gameSetting.gameLevelSetting[gameMain.gameLevel]);
                        //FjData.g_Fj[Id].Life = Main.gameSetting.gameLevelSetting[gameMain.gameLevel].life;
                        PlayStart();
                    }
                }
                break;

            case en_Player01Sta.Die:
                runTime += Time.deltaTime;
                if (runCnt < Set.setVal.Width)
                {
                    if (runTime >= 0.03f)
                    {
                        runTime = 0;
                        for (int i = 0; i < Set.setVal.Height; i++)
                        {
                            GameLeiSheBase.Update_PointColor(runCnt, i, GameLeiSheLedControl.LEISHE_COLOR, enPointSta.None);
                        }
                        runCnt++;
                    }
                }
                else if (runTime > 0.5f)
                {
                    ChangeStatue(en_Player01Sta.Loss);
                }
                break;

            case en_Player01Sta.Pass:
                break;

            case en_Player01Sta.Loss:
                break;

            case en_Player01Sta.ResultScore:
                if (runTime < 1f)
                {
                    runTime += Time.deltaTime;
                    if (runTime >= 1f)
                    {
                        playerUI.resultScore.RunStart();
                    }
                }
                break;
        }
    }
    public void ChangeStatue(en_Player01Sta sta)
    {
        //Debug.LogError("P_"+ Id + "_Statue: " + sta);

        Key.Clear();
        IO.GunOut(0);
        LedKey.Clear();
        TargetLedOut(0);

        if (sta == en_Player01Sta.Die)
        {
            oldStatue = statue;
        }
        statue = sta;
        runTime = 0;
        runCnt = 0;

        if (playerUI != null)
        {
            playerUI.image_Result.gameObject.SetActive(false);
            playerUI.resultScore.gameObject.SetActive(false);
        }
        switch (statue)
        {
            case en_Player01Sta.Idle:
                // playerUI.gameObject.SetActive(false);
                if (playerUI != null)
                {
                    playerUI.Scores_Obj.SetActive(true);

                    //playerUI.life_Obj.SetActive(true);

                    if (FjData.g_Fj[Id].TargetLed > 0)
                    {
                        playerUI.remainPoint_Obj.SetActive(true);
                        playerUI.targetButton_Obj.SetActive(false);
                        playerUI.target_Obj.transform.eulerAngles = Vector3.zero;
                    }
                    else
                    {
                        playerUI.remainPoint_Obj.SetActive(false);
                        playerUI.targetButton_Obj.SetActive(true);
                        playerUI.target_Obj.transform.eulerAngles = new Vector3(0, 180);
                    }
                }
                break;

            case en_Player01Sta.ReadyTargetLed:
                currJieDuan = 0;
                ledControl.LedInit(Main.gameSetting.gameLevelSetting[gameMain.gameLevel]);
                playerUI.target_Obj.transform.eulerAngles = Vector3.zero;
                playerUI.remainPoint_Obj.SetActive(true);
                playerUI.targetButton_Obj.SetActive(false);
                MusicManager.instance.Play_OpenLed();
                break;

            case en_Player01Sta.PlayTargetLed:
                GameLeiSheBase.Update_TargetLedColorAll(0, enPointSta.None);
                ShowTargetLed();
                IO.GunOut(1);
                break;

            case en_Player01Sta.ReadyPlayLed:
                //playerUI.target_Obj.transform.eulerAngles = new Vector3 (0, 180);
                //playerUI.remainPoint_Obj.SetActive (false);
                //playerUI.targetButton_Obj.SetActive (true);
                runCnt = Set.setVal.Width - 1;
                MusicManager.instance.Play_OpenLed();
                break;

            case en_Player01Sta.Play:
                result = 0; // 未出结果            
                ledControl.RunStart();
                Framebuffer.ClearDelayBuf();
                LedKey.Clear();
                ClearPPD(4);
                ClearPPD(5);
                Framebuffer.Update_TargetLedColor(4, Set.ChannelLength[4] - 1, 0);
                Framebuffer.Update_TargetLedColor(4, Set.ChannelLength[4] - 2, 0);
                switch (currJieDuan)
                {
                    case 0:
                        GetRandom_Tarage(4, 10);
                        GetOtherLed(4);
                        break;
                    case 1:
                        targetKeyId = (Set.ChannelLength[0] + Set.ChannelLength[1] + Set.ChannelLength[2] + Set.ChannelLength[3] + Set.ChannelLength[4]) - 1;
                        break;
                    case 2:
                        GetRandom_Tarage(5, 10);
                        Debug.LogError(Main.gameLevel);
                        switch (Main.gameLevel)
                        {
                            default:
                                GetOtherLed(5);
                                break;
                                //case 1:
                                //    InitMap_FindSame();
                                //    break;
                        }
                        break;
                    case 3:
                        targetKeyId = (Set.ChannelLength[0] + Set.ChannelLength[1] + Set.ChannelLength[2] + Set.ChannelLength[3] + Set.ChannelLength[4]) - 2;
                        break;
                }
                break;
            case en_Player01Sta.WaitPass:
                //FjData.g_Fj[Id].LevelTime = (int)gameMain.gameTime;
                break;
            case en_Player01Sta.Die:
                result = 0;
                break;
            case en_Player01Sta.Pass:
                if (Game01_Main.instance.index_JieDuan < 2)
                {
                    Game01_Main.instance.index_JieDuan++;
                    MusicManager.instance.Play_Correct();
                    ChangeStatue(en_Player01Sta.ReadyTargetLed);
                    break;
                }
                FjData.g_Fj[Id].Result = 1;
                result = 1;
                if (playerUI != null)
                {
                    playerUI.Update_Result(1);
                }
                //
                GameLeiSheBase.Update_ColorFull(0, enPointSta.None);
                MusicManager.instance.PlayOne(gameMain.audioClip_Pass, 0);
                MusicManager.instance.Play_Talk(1, 1.2f); // "恭喜过关"
                FjData.g_Fj[Id].LevelTime = (int)gameMain.gameTime;
                currJieDuan = 0;
                break;

            case en_Player01Sta.Loss:
                if (playerUI != null)
                {
                    playerUI.Update_Result(0);
                }
                //
                GameLeiSheBase.Update_ColorFull(1, enPointSta.None);
                Framebuffer.Update_TargetLedColorAll(4, 0xff0000);
                Framebuffer.Update_TargetLedColorAll(5, 0xff0000);
                //
                MusicManager.instance.PlayOne(gameMain.audioClip_Loss, 0);
                MusicManager.instance.Play_Talk(2, 1.2f); // "挑战失败"
                break;

            case en_Player01Sta.ResultScore:
                if (playerUI != null)
                {
                    //
                    playerUI.Scores_Obj.SetActive(false);
                    playerUI.life_Obj.SetActive(false);
                    playerUI.remainPoint_Obj.SetActive(false);
                    playerUI.targetButton_Obj.SetActive(false);
                    //
                    playerUI.resultScore.gameObject.SetActive(true);
                    if (result == 1)
                    {
                        playerUI.resultScore.GameStart(Id, 1);
                    }
                    else
                    {
                        playerUI.resultScore.GameStart(Id, 0);
                    }
                }
                break;

            case en_Player01Sta.ShowWinner:
                //playerUI.resultWinner.gameObject.SetActive (true);
                //playerUI.resultWinner.Update_Value (isWiner, gameLevel, FjData.g_Fj[Id].Scores);
                break;

            case en_Player01Sta.GameOver:

                break;
        }
    }
    int tarageNum;
    int randomX;
    int randomY;
    int a;
    void CheckLedKey(int ch)
    {
        for (int x = 0; x < Set.setVal.PPDWidth; x++)
        {
            for (int y = 0; y < Set.setVal.PPDHeight; y++)
            {
                int id = x + Set.setVal.PPDWidth * y;
                int pointId = id;
                for (int m = 0; m < ch; m++)
                {
                    id += Set.ChannelLength[m];
                }
                if (LedKey.KeyPressed(id))
                {
                    if (Framebuffer.led[id].statue == enPointSta.Target)
                    {
                        GameLedControl.gamePoint[id].statue = enPointSta.None;
                        Framebuffer.led[id].statue = enPointSta.None;
                        Framebuffer.Update_TargetLedColor(ch, pointId, 0);
                        FjData.g_Fj[0].Scores += 10;
                        MusicManager.instance.Play_Correct();
                        tarageNum--;
                    }
                    if (Framebuffer.led[id].statue == enPointSta.Die)
                    {
                        if (FjData.g_Fj[0].Life > 0)
                        {
                            if (errorCD <= 0)
                            {
                              //  FjData.g_Fj[0].Life--;
                                if (FjData.g_Fj[0].Scores > 0)
                                {
                                    FjData.g_Fj[0].Scores -= 5;
                                }
                                else
                                {
                                    FjData.g_Fj[0].Scores = 0;
                                }
                                errorCD = 1;
                                MusicManager.instance.Play_Fails();
                            }

                        }
                    }
                }
            }
        }
    }
    void GetRandom_Tarage(int ch, int targetNum)
    {
        int cnt = 300;
        if (targetNum > 0)
        {
            while (targetNum > 0 && cnt > 0)
            {
                cnt--;
                randomX = Random.Range(0, Set.setVal.PPDWidth);
                randomY = Random.Range(0, Set.setVal.PPDHeight);
                int id = randomX + randomY * Set.setVal.PPDWidth;
                int pointId = id;
                for (int i = 0; i < ch; i++)
                {
                    id += Set.ChannelLength[i];
                }
                if (GameLedControl.gamePoint[id].statue == enPointSta.None)
                {
                    GameLedControl.gamePoint[id].statue = enPointSta.Target;
                    Framebuffer.led[id].statue = enPointSta.Target;
                    //DrawPic.DrawPointId(pointId, 0, enPointSta.None);
                    Framebuffer.Update_TargetLedColor(ch, pointId, 0x0000ff);
                    Led_Color[id] = 0x0000ff;
                    tarageNum++;
                    targetNum--;
                }
            }
        }
    }
    void ClearPPD(int ch)
    {
        for (int i = 0; i < Set.setVal.PPDWidth; i++)
        {
            for (int k = 0; k < Set.setVal.PPDHeight; k++)
            {
                picId = k * Set.setVal.PPDWidth + i;
                pointId = picId;
                for (int m = 0; m < ch; m++)
                {
                    picId += Set.ChannelLength[m];
                }
                GameLedControl.gamePoint[picId].statue = enPointSta.None;
                DrawPic.DrawPointId(picId, 0, enPointSta.None);
                Framebuffer.Update_TargetLedColor(ch, pointId, 0);
                Led_Color[picId] = 0;
            }
        }
    }
    public readonly uint[] tab_PointColor = { 0x00a0f0, 0xff00ff, 0xffff00, 0x1145cd, 0xF44500 };
    public void GetOtherLed(int ch)
    {
        for (int i = 0; i < Set.setVal.PPDWidth; i++)
        {
            for (int k = 0; k < Set.setVal.PPDHeight; k++)
            {
                picId = k * Set.setVal.PPDWidth + i;
                pointId = picId;
                for (int m = 0; m < ch; m++)
                {
                    picId += Set.ChannelLength[m];
                }
                if (GameLedControl.gamePoint[picId].statue != enPointSta.Target)
                {
                    GameLedControl.gamePoint[picId].statue = enPointSta.Die;
                    int a = Random.Range(0, tab_PointColor.Length);
                    Led_Color[picId] = /*tab_PointColor[a]*/0xff0000;
                    Framebuffer.Update_TargetLedColor(ch, pointId, Led_Color[picId]);
                }
            }
        }
    }


    Vector2Int oldPos = new Vector2Int();
    uint oldColor = 0;
    int startPoint = 0;
    void InitMap_FindSame()
    {

        tarageNum = 20;
        oldPos = Vector2Int.one * -1;
        oldColor = 0;
        int num = 20;
        int startPoint = 0;
        for (int m = 0; m < 5; m++)
        {
            startPoint += Set.ChannelLength[m];
        }
        int x = Random.Range(0, Set.setVal.PPDWidth);
        int y = Random.Range(0, Set.setVal.PPDHeight);
        while (num > 0)
        {

            x = Random.Range(0, Set.setVal.PPDWidth);
            y = Random.Range(0, Set.setVal.PPDHeight);
            pointId = Framebuffer.MappingId(x, y);

            if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
            {
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
                GameLedControl.gamePoint[pointId].Color = 255;
                num--;

            }

        }
        num = 20;
        while (num > 0)
        {

            x = Random.Range(0, Set.setVal.PPDWidth);
            y = Random.Range(0, Set.setVal.PPDHeight);
            pointId = Framebuffer.MappingId(x, y);

            if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
            {
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
                GameLedControl.gamePoint[pointId].Color = 0x00ff00;
                num--;

            }

        }
    }
    void CheckWallLedKey()
    {

        for (int i = 0; i < Set.setVal.WallLedNum; i++)
        {
            if (GameLeiSheBase.targetLed[1 + i].statue == enPointSta.None)
                continue;
            if (LedKey.KeyStatus(wallKeyId + i) == false)
                continue;
            GameLeiSheBase.Update_TargetLedColor(1 + i, 0, enPointSta.None);
            if (FjData.g_Fj[Id].TargetLed > 0)
            {
                FjData.g_Fj[Id].TargetLed--;
                FjData.g_Fj[0].Scores += 5;
                MusicManager.instance.Play_Bomb();
            }
        }
    }
    int picId;
    int pointId;
    public uint[] Led_Color = new uint[Main.MAX_LED];
    void RunMap_FindSame()
    {
        for (int i = 0; i < Set.setVal.PPDWidth; i++)
        {
            for (int k = 0; k < Set.setVal.PPDHeight; k++)
            {
                picId = Set.setVal.PPDWidth * k + i;
                if (LedKey.KeyPressed(startPoint + picId))
                {
                    if (oldPos == Vector2Int.one * -1)
                    {
                        oldPos = new Vector2Int(i, k);
                        oldColor = GameLedControl.gamePoint[Framebuffer.MappingId(i, k)].Color;
                        MusicManager.instance.Play_Correct();
                    }
                    else
                    {
                        if (oldPos != new Vector2Int(i, k))
                        {
                            if (GameLedControl.gamePoint[Framebuffer.MappingId(i, k)].Color == GameLedControl.gamePoint[Framebuffer.MappingId(oldPos.x, oldPos.y)].Color)
                            {
                                GameLedControl.gamePoint[Framebuffer.MappingId(oldPos.x, oldPos.y)].Color = 0;
                                GameLedControl.gamePoint[Framebuffer.MappingId(i, k)].Color = 0;
                                GameLedControl.gamePoint[Framebuffer.MappingId(i, k)].statue = enPointSta.None;
                                GameLedControl.gamePoint[Framebuffer.MappingId(oldPos.x, oldPos.y)].statue = enPointSta.None;
                                MusicManager.instance.Play_Correct();
                                FjData.g_Fj[0].Scores += 20;
                                oldPos = Vector2Int.one * -1;
                                oldColor = 0;
                                tarageNum--;
                            }
                        }
                        else
                        {
                            MusicManager.instance.Play_Fails();
                            oldPos = Vector2Int.one * -1;
                            oldColor = 0;
                        //    FjData.g_Fj[0].Life--;
                        }
                    }
                }
            }
        }
    }
    void UpdateLed(int ch)
    {
        for (int i = 0; i < Set.setVal.PPDWidth; i++)
        {
            for (int k = 0; k < Set.setVal.PPDHeight; k++)
            {
                picId = k * Set.setVal.PPDWidth + i;
                //pointId = Framebuffer.tab_Mapping[picId];
                for (int m = 0; m < ch; m++)
                {
                    picId += Set.ChannelLength[m];
                }
                if (GameLedControl.gamePoint[picId].statue != enPointSta.Target)
                {
                    DrawPic.DrawPointId(picId, Led_Color[picId], GameLedControl.gamePoint[picId].statue);
                }
                else
                {
                    DrawPic.DrawPointId(picId, Led_Color[picId], enPointSta.Target);
                }
            }
        }
    }
    // 当前剩余目标灯个数
    int GetCurrTargetLed()
    {
        int count = 0;
        for (int i = 0; i < Set.setVal.WallLedNum; i++)
        {
            if (GameLeiSheBase.targetLed[i].statue == enPointSta.None)
                continue;
            count++;
        }
        return count;
    }
    // 随机生成N个按键灯：
    void ShowTargetLed()
    {
        if (FjData.g_Fj[Id].TargetLed <= 0)
            return;
        int count = GetCurrTargetLed();
        if (count == 0)
        {
            count = Mathf.Min(FjData.g_Fj[Id].TargetLed, Random.Range(3, 6));
        }
        int[] idBuf = new int[count];
        int len;
        for (int i = 0; i < count; i++)
        {
            len = 0;
            for (int j = 0; j < count; j++)
            {
                if (GameLeiSheBase.targetLed[j].statue == 0)
                {
                    idBuf[len] = j;
                    len++;
                }
            }
            if (len == 0)
                break;
            int id = idBuf[Random.Range(0, len)];
            GameLeiSheBase.Update_TargetLedColor(1 + id, 63, enPointSta.Target);
        }
    }
    float[] protectTime = new float[1000];
    void CheckRxKey()
    {

        int len = Set.setVal.Width * Set.setVal.Height;


        for (int i = 0; i < len && i < GameLeiSheBase.gamePoint.Length; i++)
        {

            if (GameLeiSheBase.gamePoint[i].bindCnt > 0)
                continue;
            if (Framebuffer.led[i].colorOld == 0)
            { continue; }
            int JieShou_ID = i;
            if (Framebuffer.isNewLeiShe)
            { JieShou_ID = GameLeiSheBase.tab_Point[i]; }

            if (GameLeiSheLedControl.PrePic_Copy[i] == 0)
            {
                continue;
            }
            else
            {

                //Debug.LogErrorError("要扣血 " + i + " " + JieShou_ID);//


            }



            if (Framebuffer.led[i].waitTime > 0)
                continue;

            if (Framebuffer.isNewLeiShe)
            {
                if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID))
                {
                    continue;
                }
            }
            else
            {
                if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID))
                {
                    continue;
                }
            }
            if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID) == false && statue == en_Player01Sta.Play)
            {



                protectTime[JieShou_ID] -= Time.deltaTime;

                if (protectTime[JieShou_ID] <= 0)
                {
                    //   Debug.LogErrorError("扣血灯 " + i + " " + JieShou_ID);//
                    if (JieShou_ID == 79)
                    {
                        return;
                    }
                    if (FjData.g_Fj[0].Life > 0)
                    {
                        //TODO 镭射超时
                        //FjData.g_Fj[0].Life--;
#if !UNITY_EDITOR
                    
#endif
                        protectTime[JieShou_ID] = 2f - 0.02f * Set.setVal.LeiShe_LMD;

                        GameLeiSheBase.gamePoint[i].bindCnt = 10;
                        MusicManager.instance.Play_Fails();
                    }
                }

            }
            else
            {
                protectTime[JieShou_ID] = 2f - 0.02f * Set.setVal.LeiShe_LMD;
            }
        }
    }
    void Test()
    {
        return;
        int len = Set.setVal.Width * Set.setVal.Height;


        for (int i = 0; i < len && i < GameLeiSheBase.gamePoint.Length; i++)
        {
            if (GameLeiSheBase.gamePoint[i].bindCnt > 0)
                continue;
            if (Framebuffer.led[i].colorOld == 0)
            {
                //  Debug.LogErrorError("关" + i);
                continue;
            }
            else
            {
                //                   Debug.LogErrorError("开" + i);
            }
            if (Framebuffer.led[i].waitTime > 0)
                continue;




            //                Debug.LogErrorError(protectTime[i]);
            if (LedKey.KeyStatus(rxKeyStartId + i))
            {
                //   int ii = GetIndex_Led(i);
                //     Debug.LogErrorError(i + "   " + GameLeiSheBase.tab_Point[i] + "   sta     " + GameLeiSheBase.tab_Point[51]);
                //      tarageLED_List[ii] = 1;
            }

        }
    }
    int GetIndex_Led(int num)
    {
        for (int i = 0; i < GameLeiSheBase.tab_Point.Length; i++)
        {
            if (GameLeiSheBase.tab_Point[i] == num)
            {
                return i;

            }
        }
        return -1;
    }
    public void RunLedAnim()
    {

        ledControl.Run();

    }

    public void ShowResult()
    {
        if (statue < en_Player01Sta.PlayTargetLed || statue > en_Player01Sta.Die)
            return;
        gameLevel = gameMain.gameLevel;
        if (statue == en_Player01Sta.WaitPass || FjData.g_Fj[Id].RemainPoint <= 0)
        {
            ChangeStatue(en_Player01Sta.Pass);
        }
        else
        {
            ChangeStatue(en_Player01Sta.Loss);
        }
    }
    public void ShowResultScore()
    {
        if (statue == en_Player01Sta.Pass || statue == en_Player01Sta.Loss)
        {
            ChangeStatue(en_Player01Sta.ResultScore);
        }
    }

    public void ShowWinner(bool win)
    {
        isWiner = win;
        gameLevel = gameMain.gameLevel;
        ChangeStatue(en_Player01Sta.ShowWinner);
    }


    public bool AddScoreFinish()
    {
        if (statue != en_Player01Sta.ResultScore)
            return true;
        if (playerUI == null)
            return true;
        if (playerUI.resultScore.statue == en_ResultScoreSta1.End)
            return true;
        return false;
    }

    static float targetLedTime = 0;
    static uint targetLedSta = 0;
    // 切换结束按钮颜色
    public void TargetLedOut(uint color)
    {
        if (currJieDuan == 0 || currJieDuan == 1)
        {
            Framebuffer.Update_TargetLedColor(Set.ChannelLength[4] - 1, color);
        }
        else
        {
            Framebuffer.Update_TargetLedColor(Set.ChannelLength[4] - 2, color);
        }
    }
    public void TargetLed_Run()
    {
        targetLedTime += Time.deltaTime;
        if (targetLedTime >= 0.5f)
        {
            targetLedTime = 0;
            if (targetLedSta == 0)
            {
                targetLedSta = 0xa0a0a0;
            }
            else
            {
                targetLedSta = 0;
            }
            //GameLeiSheBase.Update_PointColor (2, Set.setVal.TargetLedNum, targetLedSta, enPointSta.None);            
            TargetLedOut(targetLedSta);
        }
    }
}
