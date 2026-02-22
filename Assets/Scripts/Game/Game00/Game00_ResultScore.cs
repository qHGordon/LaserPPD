using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum en_ResultScoreSta
{
    Idle = 0,
    DecLife,
    DecTime,
    End,
}
public class Game00_ResultScore : MonoBehaviour
{
    public Image image_Result;
    public Text text_Score;
    public Text text_Life;
    public Text text_Time;
    public Sprite[] sprite_Result;

    int Id;
    int remainLife;
    int remainTime;
    bool runStart;

    public en_ResultScoreSta statue;
    float runTime;
    float delayTime;


    readonly string[] tab_Language = { "CN", "EN" };
    int language = -1;
    void CheckLanguage()
    {
        if (language != Set.setVal.Language || sprite_Result == null)
        {
            language = Set.setVal.Language;
            sprite_Result = Resources.LoadAll<Sprite>("UI/Result_" + tab_Language[language]);
        }
    }
    // Use this for initialization
    public void GameStart(int no, int result)
    {
        Id = no;

        CheckLanguage();

        image_Result.sprite = sprite_Result[result];
        image_Result.SetNativeSize();
        remainLife = FjData.g_Fj[Id].Life;
        remainTime = FjData.g_Fj[Id].GameTime;

        runStart = false;
        Update_Score();
        Update_Life();
        Update_Time();

        ChangeStatue(en_ResultScoreSta.Idle);
    }
    public void RunStart()
    {
        runStart = true;
    }

    bool RunTimePassed(float passTime)
    {
        runTime += Time.deltaTime;
        if (runTime >= passTime)
        {
            runTime = 0;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (statue)
        {
            case en_ResultScoreSta.Idle:
                if (FjData.g_Fj[Id].RemainPoint > 0)
                {
                    ChangeStatue(en_ResultScoreSta.End);
                    break;
                }
                if (runStart == false)
                    break;
                if (RunTimePassed(0.5f))
                {
                    ChangeStatue(en_ResultScoreSta.DecLife);
                }
                break;

            case en_ResultScoreSta.DecLife:
                if (remainLife <= 0)
                {
                    if (delayTime > 0)
                    {
                        delayTime -= Time.deltaTime;
                        break;
                    }
                    ChangeStatue(en_ResultScoreSta.DecTime);
                    break;
                }
                if (RunTimePassed(0.05f))
                {
                    if (remainLife > 0)
                    {
                        remainLife--;
                        FjData.g_Fj[Id].Scores += 1;
                        Update_Score();
                        Update_Life();
                        delayTime = 1f;
                    }
                }
                break;

            case en_ResultScoreSta.DecTime:
                if (remainTime <= 0)
                {
                    ChangeStatue(en_ResultScoreSta.End);
                    break;
                }
                if (RunTimePassed(0.05f))
                {
                    if (remainTime > 0)
                    {
                        remainTime--;
                        FjData.g_Fj[Id].Scores += 1;
                        Update_Score();
                        Update_Time();
                    }
                }
                break;

            case en_ResultScoreSta.End:
                break;
            default:
                break;
        }
    }
    void ChangeStatue(en_ResultScoreSta sta)
    {
        statue = sta;
        runTime = 0;
        delayTime = 0;
    }

    void Update_Score()
    {
        text_Score.text = FjData.g_Fj[Id].Scores.ToString();
    }
    void Update_Life()
    {
        text_Life.text = remainLife.ToString();
    }
    void Update_Time()
    {
        text_Time.text = remainTime.ToString();
    }
}
