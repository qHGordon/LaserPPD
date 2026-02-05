using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum en_GameOverSta
{
    Run = 0,
}

public class Game97_GameOver : MonoBehaviour
{
    public Image image_TitleBackG;
    public Image image_TitleText;
    // 声音
    public AudioSource audioSource_BackG;       // 背景音乐

    Game97_Main game97_Main;

    Sprite[] sprite_PlayerScoreResult;
    Sprite[] sprite_Title;
    public Sprite allpass, gameOver;
    en_GameOverSta statue;
    public float runTime;

    int[] nowScore = new int[Main.MAX_PLAYER];
    int[] nowWins = new int[Main.MAX_PLAYER];


    //
    public int playerNum;
    //
    public void Awake0(Game97_Main game)
    {
        //print("111");
        game97_Main = game;
        playerNum = 0;
        sprite_Title = Resources.LoadAll<Sprite>("Company_00/Game97/GameOver/Title");
        sprite_PlayerScoreResult = Resources.LoadAll<Sprite>("Company_00/Game97/GameOver/Plsyer");
    }

    public void GameStart()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            allpass = Resources.Load<Sprite>("Company_00/Game97/GameOver/cn/win_cn");
            gameOver = Resources.Load<Sprite>("Company_00/Game97/GameOver/cn/lose_cn");
        }
        else
        {
            allpass = Resources.Load<Sprite>("Company_00/Game97/GameOver/en/win_en");
            gameOver = Resources.Load<Sprite>("Company_00/Game97/GameOver/en/lose_en");
        }

        /*
        else
        {
             // 游戏结束
            //  image_TitleBackG.sprite = sprite_Title[0];
            image_TitleText.sprite = sprite_Title[2 + Set.setVal.Language];
        }
        Main.FormatImageSizeFollowSprite(image_TitleBackG);
        Main.FormatImageSizeFollowSprite(image_TitleText);
   */
        playerNum = 0;
        for (int i = 0; i < Main.MAX_PLAYER; i++)
        {
            if (FjData.g_Fj[i].Played)
            {
                playerNum++;
            }
        }

     

        for (int i = 0; i < Main.MAX_PLAYER; i++)
        {
            nowScore[i] = FjData.g_Fj[i].JsScores;

          
            Main.JieSuanScore(i);
        }
        //
        
        //
        ChangeStatue(en_GameOverSta.Run);
    }
    //// 得分结果结算, 清零
    //public void GetResultScore() {
    //    int winBl = 10;
    //    if (Set.setVal.OutMode == (int)en_OutMode.OutTicket) {
    //        winBl = Set.setVal.TicketBl;
    //    } else if (Set.setVal.OutMode == (int)en_OutMode.OutGift) {
    //        winBl = Set.setVal.GiftBl;
    //    }
    //    for (int i = 0; i < Main.MAX_PLAYER; i++) {
    //        nowWins[i] = 0;
    //        if (FjData.g_Fj[i].Played) {
    //            if (FjData.g_Fj[i].Scores >= winBl) {
    //                nowWins[i] = FjData.g_Fj[i].Scores / winBl;
    //                FjData.g_Fj[i].Wins += nowWins[i];
    //                FjData.SaveData_Wins(i);
    //            }
    //            //print("Player_" + i + " Wins: " + FjData.g_Fj[i].Wins);
    //        }
    //        FjData.g_Fj[i].Scores = 0;
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        runTime += Time.deltaTime;
        if (runTime >= 8)
        {

			game97_Main.GameStart ();
        }
    }

    void ChangeStatue(en_GameOverSta sta)
    {
        statue = sta;
        runTime = 0;

        Key.Clear();

    }
}
