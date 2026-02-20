using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public enum en_Player00Sta
{
    Idle = 0,           // 空闲
    Play,               // 游戏中
    WaitPass,
    Die,                // 死亡
    Pass,
    Loss,
    ResultScore,
    ShowWinner,
    GameOver,
}


public class Game00_Player : MonoBehaviour
{
    //#if UNITY_EDITOR
    //    const int MAX_PLAYER_BLOOD = 2000;
    //#else
    //    const int MAX_PLAYER_BLOOD = 1000;
    //#endif
    const float MAX_SPECIALATTCK_TIME = 3;

    public Canvas canvas;
    public Game00_PlayerUI playerUI;

    public Game00_Main gameMain;
    public int Id;
    public int result;
    public int gameLevel;

    bool isWiner;


    public en_Player00Sta statue;
    public en_Player00Sta oldStatue;
    float runTime;
    int runCnt;
    float btnTime;
    public Button btn_GameSet;



    public void Awake0(Game00_Main gmain, int no)
    {
        gameMain = gmain;
        Id = no;

        if (playerUI != null)
        {
            playerUI.Awake0(Id);
        }


    }
    // Use this for initialization
    public void OnClick_Set()
    {

        if (playerUI.Ingame_Setting != null)
        {


            playerUI.Ingame_Setting.gameObject.SetActive(true);
            playerUI.Ingame_Setting.GGstart();





        }
    }
    public void Ready()
    {
        if (playerUI != null)
        {
            playerUI.GameStart(0, 0);
        }
        ChangeStatue(en_Player00Sta.Idle);
    }
    public void GameStart(int fulllife, int playernum)
    {
        if (btn_GameSet != null)
        {
            btn_GameSet.onClick.AddListener(OnClick_Set);

        }
        if (playerUI != null)
        {
            playerUI.GameStart(fulllife, playernum);
            if (playerUI.Ingame_Setting != null)
            {
                playerUI.Ingame_Setting.gameObject.SetActive(false);
            }

        }
        btnTime = 0;
        if (btn_GameSet != null)
        {
            btn_GameSet.gameObject.SetActive(false);

        }

        ChangeStatue(en_Player00Sta.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        switch (statue)
        {
            case en_Player00Sta.Idle:
                break;

            case en_Player00Sta.Play:
                if (btn_GameSet != null)
                {
                    if (btnTime > 0)
                    {
                        btnTime -= Time.deltaTime;
                        if (btnTime < 0)
                        {
                            btn_GameSet.gameObject.SetActive(false);
                        }
                    }
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    if (Input.GetKeyDown(KeyCode.O))

                    {
                        btnTime = 2;
                        btn_GameSet.gameObject.SetActive(true);

                    }
#else
                     if (Input.GetMouseButtonDown(0))
                    
                    {
                        btnTime = 2;
                        btn_GameSet.gameObject.SetActive(true);

                    }
#endif


                }
                if (playerUI.Ingame_Setting != null)
                {
                    if (playerUI.Ingame_Setting.gameObject.activeSelf)
                    {
                        if (Game00_GameUIComm.instance.image_JieDuan.gameObject.activeSelf)
                        {
                            Game00_GameUIComm.instance.image_JieDuan.gameObject.SetActive(false);

                        }
                        if (gameMain.gameUI_Single.time_Obj_All.gameObject.activeSelf)
                        {
                            gameMain.gameUI_Single.time_Obj_All.gameObject.SetActive(false);
                            gameMain.gameUI_Single.time_Obj_JieDuan.gameObject.SetActive(false);
                            playerUI.text_Scores.transform.parent.gameObject.SetActive(false);

                        }


                    }
                    else
                    {
                        if (!Game00_GameUIComm.instance.image_JieDuan.gameObject.activeSelf)
                        {
                            Game00_GameUIComm.instance.image_JieDuan.gameObject.SetActive(true);

                        }
                        if (!gameMain.gameUI_Single.time_Obj_All.gameObject.activeSelf)
                        {
                          //  gameMain.gameUI_Single.time_Obj_All.gameObject.SetActive(true);


                            gameMain.gameUI_Single.time_Obj_JieDuan.gameObject.SetActive(true);

                        }
                        if (!playerUI.Scores_Obj.gameObject.activeSelf)
                        {
                            playerUI.Scores_Obj.gameObject.SetActive(true);

                        }
                    }


                }

                if (runTime < 1f)
                {
                    runTime += Time.deltaTime;
                    break;
                }
                if (FjData.g_Fj[Id].Life <= 0)
                {
                    ChangeStatue(en_Player00Sta.Die);
                    break;
                }
                if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free)
                {
                    if (FjData.g_Fj[0].RemainWallLed > 0)
                    {
                        break;
                    }
                }
                //if (FjData.g_Fj[Id].RemainPoint <= 0) {
                //    ChangeStatue (en_PlayerSta.WaitPass);
                //    break;
                //}
                break;

            case en_Player00Sta.WaitPass:
                runTime += Time.deltaTime;
                if (runTime >= 0.5f)
                {
                    ChangeStatue(en_Player00Sta.Pass);
                }
                break;

            case en_Player00Sta.Die:
                //runTime += Time.deltaTime;
                //if (runTime >= 0.5f) {
                //    ChangeStatue (en_PlayerSta.Loss);
                //}
                break;

            case en_Player00Sta.Pass:
                runTime += Time.deltaTime;
                if (runTime < 0.015f)
                    break;
                runTime = 0.0f;
                //
                if (runCnt < 250)
                {
                    runCnt = Mathf.Min(runCnt + 10, 250);
                    GameLedControl.playerControl[Id].ShowColorFull((uint)(runCnt << 8), enPointSta.None);
                }
                break;

            case en_Player00Sta.Loss:
                runTime += Time.deltaTime;
                if (runTime < 0.015f)
                    break;
                runTime = 0.0f;
                //
                if (runCnt < 250)
                {
                    runCnt = Mathf.Min(runCnt + 10, 250);
                    GameLedControl.playerControl[Id].ShowColorFull((uint)(runCnt << 16), enPointSta.None);
                }
                break;

            case en_Player00Sta.ResultScore:
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
    public void ChangeStatue(en_Player00Sta sta)
    {

        //print("P_"+ Id + "_Statue: " + sta);
        Key.Clear();

        oldStatue = statue;
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
            case en_Player00Sta.Idle:
                // playerUI.gameObject.SetActive(false);
                if (playerUI != null)
                {
                    //   playerUI.Scores_Obj.SetActive(true);
                    playerUI.life_Obj.SetActive(true);
                    //   playerUI.remainPoint_Obj.SetActive(true);
                }
                break;

            case en_Player00Sta.Play:
                result = 2; // 未出结果

                break;
            case en_Player00Sta.WaitPass:
               gameMain.gameTime = FjData.g_Fj[Id].GameTime;
                break;
            case en_Player00Sta.Die:
                result = 0;
                break;
            case en_Player00Sta.Pass:
                result = 1;
                if (playerUI != null)
                {
                     playerUI.Update_Result(1);
                }
                MusicManager.instance.PlayOne(gameMain.audioClip_Pass, 0);
                FjData.g_Fj[Id].GameTime = gameMain.gameTime;
                break;

            case en_Player00Sta.Loss:
                if (playerUI != null)
                {
                    playerUI.Update_Result(0);
                }
                MusicManager.instance.PlayOne(gameMain.audioClip_Loss, 0);
                break;

            case en_Player00Sta.ResultScore:
                if (playerUI != null)
                {
                    //
                    playerUI.Scores_Obj.SetActive(false);
                    playerUI.life_Obj.SetActive(false);
                    playerUI.remainPoint_Obj.SetActive(false);
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

            case en_Player00Sta.ShowWinner:
                playerUI.resultWinner.gameObject.SetActive(true);
                playerUI.resultWinner.Update_Value(isWiner, gameLevel, FjData.g_Fj[Id].Scores);
                break;
        }
    }

    public void ShowResult()
    {

        if (statue < en_Player00Sta.Play || statue > en_Player00Sta.Die)
            return;
        gameLevel = gameMain.gameLevel;

        //
        if (statue == en_Player00Sta.WaitPass)
        {
            ChangeStatue(en_Player00Sta.Pass);
            return;
        }
        if (Game97_PlayerModeSel.selectId == (int)en_PlayerMode.Free)
        {
            if (FjData.g_Fj[0].Life <= 0)
            {
                ChangeStatue(en_Player00Sta.Loss);
                return;
            }
        }
        ChangeStatue(en_Player00Sta.Pass);
        //if (FjData.g_Fj[Id].RemainPoint <= 0)
        //{

        //}
        //else
        //{
        //    ChangeStatue(en_PlayerSta.Loss);
        //}
    }
    public void ShowResultScore()
    {
        if (statue == en_Player00Sta.Pass || statue == en_Player00Sta.Loss)
        {
            ChangeStatue(en_Player00Sta.ResultScore);
        }
    }

    public void ShowWinner(bool win)
    {
        isWiner = win;
        gameLevel = Main.MapID;
        ChangeStatue(en_Player00Sta.ShowWinner);
    }


    public bool AddScoreFinish()
    {
        if (statue != en_Player00Sta.ResultScore)
            return true;
        if (playerUI == null)
            return true;
        if (playerUI.resultScore.statue == en_ResultScoreSta.End)
            return true;
        return false;
    }
}
