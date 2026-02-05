using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingInGame_02 : SettingInGame
{
    const int SET_ID_BLUENUM = 0;
    const int SET_ID_LEVELTIME = 1;

    const int SET_ID_MOVESPEED1 = 2;
    const int SET_ID_MOVESPEED2 = 3;
    const int SET_ID_MOVESPEED3 = 4;
    const int SET_ID_MOVESPEED4 = 5;
    const int SET_ID_MOVESPEED5 = 6;

    const int SET_ID_TarageNUM1 = 7;
    const int SET_ID_TarageNUM2 = 8;
    const int SET_ID_TarageNUM3 = 9;
    const int SET_ID_TarageNUM4 = 10;
    const int SET_ID_TarageNUM5 = 11;
    const int SET_ID_LIFENUM = 12;

    const int SET_ID_Time = 13;//暂时用不到


    //测试：
    int[] tab_BlueNum = { 20, 25, 30, 40, 50, 60, 70, 80, 90, 100 };
    int[] tab_TarageNum = { 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    int[] tab_LifeNum = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    int[] tab_LevelTime = { 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 150, 180, 210, 240, 270, 300, 330, 360, 390, 420, 450, 480, 510, 540, 570, 600 };

    int[] tab_JieDuanTime = { 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 150, 180 };
    public readonly int[] table_MoveSpeed = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };


    //   int [] tab_MoveSpeed = new int[16];
    //
    [HideInInspector]
    public int set_BlueNum = 20;
    [HideInInspector]

    public int set_LifeNum = 10;

    [HideInInspector]
    public int set_LevelTime = 60;
    [HideInInspector]
    // public readonly float[] set_MoveSpeed = { 0.02f,0.05f,0.1f,0.15f,0.2f,0.25f,0.3f,0.35f,0.4f,0.45f,0.5f,0.55f,0.6f};
    public readonly int[] set_MoveSpeed = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    [HideInInspector]
    public readonly int[] set_TarageNum = { 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100 };

    string Str_TarageNum;
    string Str_LifeNum;
    string Str_LevelTime;
    string Str_MoveSpeed;
    string Str_ReMainNum;

    public void GetName()
    {

        Str_TarageNum = "Game01_ " + Game01_Main.instance.gameLevel.ToString("D2") + "set_BlueNum";
        Str_LifeNum = "Game01_ " + Game01_Main.instance.gameLevel.ToString("D2") + "set_LifeNum";
        Str_LevelTime = "Game01_ " + Game01_Main.instance.gameLevel.ToString("D2") + "set_LevelTime";
        Str_MoveSpeed = "Game01_ " + Game01_Main.instance.gameLevel.ToString("D2") + "set_MoveSpeed";
        Str_ReMainNum = "Game01_ " + Game01_Main.instance.gameLevel.ToString("D2") + "set_TarageNum";
    }
    public int GetTarageNum()
    {
        int a = PlayerPrefs.GetInt(Str_TarageNum);
        if (a == 0)
        {
            a = set_BlueNum;
        }
        return a;
    }
    public int GetLifeNum()
    {
        int a = PlayerPrefs.GetInt(Str_LifeNum);
        if (a == 0)
        {
            a = set_LifeNum;
        }
        FjData.g_Fj[0].Life = a;

        return a;
    }
    public int GetLevelTime()
    {
        int a = PlayerPrefs.GetInt(Str_LevelTime);
        if (a == 0)
        {
            a = set_LevelTime;
        }
        return a;
    }
    public int GetMoveSpeed(int i)
    {
        int a = PlayerPrefs.GetInt(Str_MoveSpeed + i.ToString());
        if (a == 0)
        {
            a = set_MoveSpeed[i];
        }
        return a;
    }
    public int GetReMainNum(int i)
    {
        int a = PlayerPrefs.GetInt(Str_ReMainNum + i.ToString());
        if (a == 0)
        {
            a = set_TarageNum[i];
        }
        return a;
    }



    public void GGstart()
    {
        GetName();
        UpdateLanguage();
        UpdateSetValue();
    }

    public override void SetInit(int SET_COUNT)
    {
        //base.GameStart();

        //for (int i = 0; i < tab_BlueNum.Length; i++)
        //{
        //    tab_BlueNum[i] = 15 + 5*i;
        //}
        //for (int i = 0; i < tab_LevelTime.Length; i++)
        //{
        //    tab_LevelTime[i] = 60 + i * 20;
        //}
        //for (int i = 0; i < tab_JieDuanTime.Length; i++)
        //{
        //    tab_JieDuanTime[i] = 20 + i * 10;
        //}

        base.Init(SET_COUNT);

        CreateSetOnes(SET_COUNT);

        list_GameSetOne[SET_ID_BLUENUM].SetInit(tab_BlueNum);
        list_GameSetOne[SET_ID_LIFENUM].SetInit(tab_LifeNum);
        list_GameSetOne[SET_ID_BLUENUM].SetInit(tab_BlueNum);
        list_GameSetOne[SET_ID_LEVELTIME].SetInit(tab_LevelTime);
        set_BlueNum = tab_BlueNum[4];
        set_LifeNum = tab_LifeNum[11];
        set_LevelTime = 60;
        if (Set.setVal.Width > 25 || Set.setVal.Height > 25)
        {
            set_BlueNum = tab_BlueNum[5];

        }
        for (int i = 0; i < 5; i++)
        {
            list_GameSetOne[SET_ID_MOVESPEED1 + i].SetInit(table_MoveSpeed);
        }
        for (int i = 0; i < 5; i++)
        {
            list_GameSetOne[SET_ID_TarageNUM1 + i].SetInit(tab_TarageNum);
        }


        gameObject.SetActive(false);


    }
    public static SettingInGame_02 instance;
    private void Awake()
    {
        instance = this;
        SetInit(13);
    }

    public override void UpdateLanguage()
    {
        base.UpdateLanguage();
        //
        text_Title = transform.GetChild(0).GetComponent<Text>();

        if (Set.setVal.Language == 0)
        {
            if (text_Title != null)
            {
                if (Game00_Main.instance != null)
                {
                    text_Title.text = "关卡" + (Game00_Main.instance.gameLevel + 1).ToString("D2");// 

                }

            }
            //
            list_GameSetOne[SET_ID_BLUENUM].SetName("蓝色方块数量");
            list_GameSetOne[SET_ID_BLUENUM].gameObject.SetActive(false);

            list_GameSetOne[SET_ID_LEVELTIME].SetName("每阶段时间");
            list_GameSetOne[SET_ID_LEVELTIME].gameObject.SetActive(false);
            list_GameSetOne[SET_ID_LIFENUM].SetName("玩家总生命值");
            list_GameSetOne[SET_ID_LIFENUM].transform.localPosition = new Vector3(0, -255, 0);

            for (int i = 0; i < 5; i++)
            {
                list_GameSetOne[SET_ID_MOVESPEED1 + i].SetName("第" + (i + 1).ToString("D1") + "阶段速度");
                list_GameSetOne[SET_ID_MOVESPEED1 + i].gameObject.SetActive(false);

            }
            for (int i = 0; i < 5; i++)
            {
                list_GameSetOne[SET_ID_TarageNUM1 + i].SetName("第" + (i + 1).ToString("D1") + "阶段目标数量");
                list_GameSetOne[SET_ID_TarageNUM1 + i].gameObject.SetActive(false);

            }

        }
        else
        {
            if (text_Title != null)
            {
                if (Game00_Main.instance != null)
                {
                    text_Title.text = "Level " + (Game00_Main.instance.gameLevel + 1).ToString("D2");
                }
            }


            list_GameSetOne[SET_ID_BLUENUM].SetName("Numbers of blue ");
            list_GameSetOne[SET_ID_LIFENUM].SetName("Numbers of Life ");
            list_GameSetOne[SET_ID_LEVELTIME].SetName("Level total time");
            for (int i = 0; i < 5; i++)
            {
                //   list_GameSetOne[SET_ID_MOVESPEED1 + i].SetName("第 " + i.ToString("D1") + "阶段移动速度");
                list_GameSetOne[SET_ID_MOVESPEED1 + i].SetName("Speed of " + (i + 1).ToString("D1") + "Level");

            }
            for (int i = 0; i < 5; i++)
            {
                //   list_GameSetOne[SET_ID_MOVESPEED1 + i].SetName("第 " + i.ToString("D1") + "阶段移动速度");
                list_GameSetOne[SET_ID_TarageNUM1 + i].SetName("tarage of " + (i + 1).ToString("D1") + "Level");

            }

        }
    }

    public override void UpdateSetValue()
    {

        int num = 0;

        num = 0;
        num = PlayerPrefs.GetInt(Str_TarageNum);
        if (num != 0)
        {
            set_BlueNum = num;
        }

        num = 0;
        num = PlayerPrefs.GetInt(Str_LevelTime);//
        if (num != 0)
        {
            set_LevelTime = num;
        }



        num = PlayerPrefs.GetInt(Str_LifeNum);

        if (num == 0)
        {
            set_LifeNum = 8;
        }
       
        for (int i = 0; i < 5; i++)
        {
            num = 0;
            num = PlayerPrefs.GetInt(Str_MoveSpeed + i.ToString());
            if (num != 0)
            {
                set_MoveSpeed[i] = num;

            }

            num = 0;
            num = PlayerPrefs.GetInt(Str_TarageNum + i.ToString());

            if (num != 0)
            {
                set_TarageNum[i] = num;

            }

        }
        list_GameSetOne[SET_ID_BLUENUM].UpdateValue(set_BlueNum);
        list_GameSetOne[SET_ID_LIFENUM].UpdateValue(set_LifeNum);
        list_GameSetOne[SET_ID_LEVELTIME].UpdateValue(set_LevelTime);

        for (int i = 0; i < 5; i++)
        {
            // Debug.LogError("??    "+ set_MoveSpeed[i] );
            list_GameSetOne[SET_ID_MOVESPEED1 + i].UpdateValue(set_MoveSpeed[i]);//

        }
        for (int i = 0; i < 5; i++)
        {
            // Debug.LogError("??    "+ set_MoveSpeed[i] );
            list_GameSetOne[SET_ID_TarageNUM1 + i].UpdateValue(set_TarageNum[i]);//

        }

    }

    public override void SaveOk()
    {
        set_BlueNum = list_GameSetOne[SET_ID_BLUENUM].GetValue();
        set_LevelTime = list_GameSetOne[SET_ID_LEVELTIME].GetValue();
        set_LifeNum = list_GameSetOne[SET_ID_LIFENUM].GetValue();


        for (int i = 0; i < 5; i++)
        {
            set_MoveSpeed[i] = list_GameSetOne[SET_ID_MOVESPEED1 + i].GetValue();
            list_GameSetOne[SET_ID_MOVESPEED1 + i].UpdateValue(set_MoveSpeed[i]);

        }
        for (int i = 0; i < 5; i++)
        {
            set_TarageNum[i] = list_GameSetOne[SET_ID_TarageNUM1 + i].GetValue();
            list_GameSetOne[SET_ID_TarageNUM1 + i].UpdateValue(set_TarageNum[i]);

        }
        SaveingLocal();

    }
    public void SaveingLocal()
    {
        PlayerPrefs.SetInt(Str_TarageNum, set_BlueNum);
        PlayerPrefs.SetInt(Str_LifeNum, set_LifeNum);
        Debug.LogError(PlayerPrefs.GetInt(Str_LifeNum));
        PlayerPrefs.SetInt(Str_LevelTime, set_LevelTime);
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt(Str_MoveSpeed + i.ToString(), set_MoveSpeed[i]);
            PlayerPrefs.SetInt(Str_ReMainNum + i.ToString(), set_TarageNum[i]);
        }

        //PlayerPrefs.SetInt("SET_ID_MOVESPEED2", SET_ID_MOVESPEED2);
        //PlayerPrefs.SetInt("SET_ID_MOVESPEED3", SET_ID_MOVESPEED3);
        //PlayerPrefs.SetInt("SET_ID_MOVESPEED4", SET_ID_MOVESPEED4);
        //PlayerPrefs.SetInt("SET_ID_MOVESPEED5", SET_ID_MOVESPEED5);

        //PlayerPrefs.SetInt("SET_ID_TarageNUM1", SET_ID_TarageNUM1);
        //PlayerPrefs.SetInt("SET_ID_TarageNUM2", SET_ID_TarageNUM2);
        //PlayerPrefs.SetInt("SET_ID_TarageNUM3", SET_ID_TarageNUM3);
        //PlayerPrefs.SetInt("SET_ID_TarageNUM4", SET_ID_TarageNUM4);
        //PlayerPrefs.SetInt("SET_ID_TarageNUM5", SET_ID_TarageNUM5);
    }

    public override void DefaultOk()
    {
        // Default_01();

        list_GameSetOne[SET_ID_BLUENUM].SetInit(tab_BlueNum);
        list_GameSetOne[SET_ID_LIFENUM].SetInit(tab_LifeNum);
        list_GameSetOne[SET_ID_BLUENUM].SetInit(tab_BlueNum);
        list_GameSetOne[SET_ID_LEVELTIME].SetInit(tab_LevelTime);
        set_BlueNum = tab_BlueNum[4];
        set_LifeNum = tab_LifeNum[11];
        set_LevelTime = 60;
        if (Set.setVal.Width > 25 || Set.setVal.Height > 25)
        {
            set_BlueNum = tab_BlueNum[5];

        }
        for (int i = 0; i < 5; i++)
        {
            list_GameSetOne[SET_ID_MOVESPEED1 + i].SetInit(table_MoveSpeed);
        }
        for (int i = 0; i < 5; i++)
        {
            list_GameSetOne[SET_ID_TarageNUM1 + i].SetInit(tab_TarageNum);
        }

        UpdateSetValue();
        SaveingLocal();
    }
}
