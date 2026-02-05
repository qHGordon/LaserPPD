using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Game97_LevelSel : MonoBehaviour
{
    public readonly int[] Level = {
        0,15,3,6, 9 ,//5
        26,17,2,16,25,//10


        5,8,10,7,13 ,//15
        19,12,11,21,24,//20
   //25
        20,18,28,27,22 ,//25
        29,14,23,4,1,//30


        30,31,32,33,34 ,
        35,36,37,38,39
    };
    public static Game97_LevelSel instance;
    public GameObject obj_PageAll;
    public GameObject[] obj_Page;
    public GameButton[] button_GameMode;
    public GameButton[] button_Level;
    public GameObject Img_Challenge;
    public Button button_Changange;
    public Button button_Changange_Back;


    public Button button_Back;

    public GameObject img_LevelNum; //图片预制体
    public Image[] image_LevelPic;
    public Image[] image_LevelName;


    public Image image_BackG;
    public Num num_Time;
    public Image image_PressToStart;
    //public Image StartGame;
    // 声音
    public AudioSource audioSource_BackG;       // 背景音乐


    public Image image_SelectTimeout;

    Game97_Main game97_Main;
    public Sprite spr_NoOpen;
    public Sprite[] sprite_LevelPic;        //关卡地图
    public Sprite[] sprite_LevelName;       //关卡名字

    public GameObject GameLevel_button;
    public GameObject Game13;
    public GameObject Game14;
    public GameObject Game15;
    en_GameSelSta statue;
    public int page_Index = 0;
    public int Map_Index = 0;
    Vector3 page_Pos = Vector3.zero;
    public Transform trans_Level;
    public Text text_TotalScore;
    public void Awake0(Game97_Main main)
    {

        instance = this;

        game97_Main = main;
        page_Index = 0;
        Map_Index = 0;

        for (int i = 0; i < button_GameMode.Length; i++)
        {
            button_GameMode[i].Init(i, OnClick_ButtonMode);
        }

        button_Back.onClick.AddListener(OnClick_Back);
        //在这里生成有X个关卡的第四页
        button_Level = obj_PageAll.GetComponentsInChildren<GameButton>();
        for (int i = 0; i < button_Level.Length; i++)
        {
            button_Level[i].Init(i, OnClick_Level);
        }
        if (Set.setVal.GameChoose == 0)
        {

            //   button_Changange.onClick.AddListener(OnClick_Changlle);
            //  button_Changange_Back.onClick.AddListener(OnClick_Changlle_Back);
        }


        image_LevelPic = new Image[100];
        image_LevelName = new Image[100];

        if (Set.setVal.GameChoose == (7))
        {
            for (int i = 0; i < button_Level.Length; i++)
            {
                image_LevelPic[i] = Instantiate(img_LevelNum, button_Level[i].transform).GetComponent<Image>();
                image_LevelPic[i].transform.localPosition = new Vector3(0, 80);
                image_LevelName[i] = Instantiate(img_LevelNum, button_Level[i].transform).GetComponent<Image>();
                image_LevelName[i].transform.localPosition = new Vector3(0, -100);
            }
        }
        else
        {
            for (int i = 0; i < button_Level.Length; i++)
            {
                image_LevelPic[i] = Instantiate(img_LevelNum, button_Level[i].transform).GetComponent<Image>();
                image_LevelPic[i].transform.localPosition = new Vector3(0, 39);
                image_LevelName[i] = Instantiate(img_LevelNum, button_Level[i].transform).GetComponent<Image>();
                image_LevelName[i].transform.localPosition = new Vector3(0, -89);
            }
        }



    }
    int len = 10;
    int a = 0;
    public void OnClick_Changlle_Back()
    {
        Img_Challenge.gameObject.SetActive(false);
        game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
    }
    public void OnClick_Changlle()
    {

        Main.gameLevel_challenge_Index = 0;
        a = PlayerPrefs.GetInt("NowOpenLevel" + 0.ToString());
        if (a == -1)
        {
            PlayerPrefs.SetInt("NowOpenLevel" + 0.ToString(), 0);
            PlayerPrefs.SetInt("NowOpenLevel" + 1.ToString(), 11);
            PlayerPrefs.SetInt("NowOpenLevel" + 2.ToString(), 23);
            PlayerPrefs.SetInt("NowOpenLevel" + 3.ToString(), 15);
            PlayerPrefs.SetInt("NowOpenLevel" + 4.ToString(), 10);
            PlayerPrefs.SetInt("NowOpenLevel" + 5.ToString(), 20);
            PlayerPrefs.SetInt("NowOpenLevel" + 6.ToString(), 25);
            PlayerPrefs.SetInt("NowOpenLevel" + 7.ToString(), 22);
            PlayerPrefs.SetInt("NowOpenLevel" + 8.ToString(), 29);
            PlayerPrefs.SetInt("NowOpenLevel" + 9.ToString(), 17);
            Menu_LevelSet.instance.OpenIndex = 9;
        }
        a = PlayerPrefs.GetInt("NowOpenLevel" + 0.ToString());
        Main.Go_challeng = true;

        Main.MapIndex = a;

        if (a < 30)
        {
            a = Level[a];
        }

        Main.MapID = a;
        game97_Main.EnterGame(a);
    }
    public void Update_TotalScore()
    {
        if (text_TotalScore.text != FjData.g_Fj[0].Scores.ToString())
        {
            text_TotalScore.text = FjData.g_Fj[0].Scores.ToString();
        }

    }
    public void GameStart()
    {
        //   Invoke("Start_"+1.ToString("D2"),0);
        Main.Go_challeng = false;
        string companyPath = "";
        if (Main.COMPANY_NUM == 4)
        {
            companyPath = "Company_" + Main.COMPANY_NUM.ToString("D2") + "/";
        }
        //关卡地图
        //关卡名字
        Img_Challenge.gameObject.SetActive(false);
        switch (Set.setVal.GameChoose)
        {
            case (int)en_GameId.YueDongGeZi:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/LevelName");
                break;
            case (int)en_GameId.LeiSheWu:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/LeiShe/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/LeiShe/LevelName");
                break;
            case (int)en_GameId.PanYan:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/PanYan/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/PanYan/LevelName");
                break;


            case (int)en_GameId.LanQiu:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/LanQiu/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/LanQiu/LevelName");
                break;
            case (int)en_GameId.TouZhi:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/TouZhi/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/TouZhi/LevelName");
                break;

            case (int)en_GameId.DevEyes:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/DevEyes/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/DevEyes/LevelName");
                break;

            case (int)en_GameId.PaiPaiDeng:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/PaiPaiDeng/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/PaiPaiDeng/LevelName");
                break;
            case (int)en_GameId.LeiShePPD:
                sprite_LevelPic = Resources.LoadAll<Sprite>(companyPath + "UI/LeiShePPD/LevelPic");
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/LeiShePPD/LevelName");
                break;
        }

        GameObject btn_level;

        if (Menu_GameLevelSet.instance != null)
        {
            len = Menu_GameLevelSet.instance.maxLevel;
        }
        for (int i = 0; i < trans_Level.childCount; i++)
        {
            trans_Level.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < len; i++)
        {
            trans_Level.GetChild(i).gameObject.SetActive(true);
        }

        //for (int i = 0; i < len; i++)
        //{
        //    btn_level = Instantiate(GameLevel_button, trans_Level);
        //}




        CheckGameChoose();

        page_Index = (int)Main.playerMode;
        Update_GameModeSelect();
    }


    readonly Vector3[] tab_ModeButtonPos_00 = { new Vector3(-674, -214), new Vector3(-313, -214), new Vector3(48, -214), new Vector3(376, -214), new Vector3(704, -214) };
    readonly Vector3[] tab_ModeButtonPos_01 = { new Vector3(-450, -214), new Vector3(0, -214), new Vector3(450, -214), new Vector3(630, -214), new Vector3(704, -214) };
    readonly Vector3[] tab_ModeButtonPos_02 = { new Vector3(-600, -214), new Vector3(-200, -214), new Vector3(200, -214), new Vector3(600, -214), new Vector3(704, -214) };

    Vector3[] tab_ModeButtonPos;

    int gameChoose = -1;
    void CheckGameChoose()
    {
        //if (gameChoose == Set.setVal.GameChoose)
        //    return;
        gameChoose = Set.setVal.GameChoose;
        //        Debug.LogError("????"+ gameChoose);
        for (int i = 0; i < button_GameMode.Length; i++)
        {
            button_GameMode[i].gameObject.SetActive(true);
        }
        if (gameChoose == (int)en_GameId.YueDongGeZi)
        {
            // 地砖灯：
            //for (int i = 0; i < button_Level.Length; i++) {
            //    button_Level[i].Init (i, OnClick_Level);
            //}
            //
            tab_ModeButtonPos = tab_ModeButtonPos_00;
            // levelPic-name

            for (int i = 0; i < 30 + len; i++)
            {
                image_LevelPic[i].sprite = sprite_LevelPic[i];
                //image_LevelPic[i].SetNativeSize ();

                //if (i==20)
                //{

                //    image_LevelPic[i].sprite = spr_NoOpen;
                //    image_LevelPic[i] .SetNativeSize();
                //  // img_NoOpen.transform.localPosition = new Vector3(0, 40, 0);
                //}
            }
            //      
            for (int i = 0; i < 30 + len; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                //if (i <= 9)
                //{
                //    image_LevelName[i].transform.localScale = Vector3.one * 1.6f;
                //}
            }
            button_GameMode[3].gameObject.SetActive(true);
            button_GameMode[4].gameObject.SetActive(true);
        }
        else if (gameChoose == (int)en_GameId.LeiSheWu)

        {
            // 其它：镭射灯
            // 重新编号：
            //int[] levelId = new int[obj_Page.Length];
            //for (int i = 0; i < levelId.Length; i++) {
            //    levelId[i] = 0;
            //}
            //for (int i = 0; i < button_Level.Length; i++) {
            //    for (int j = 0; j < obj_Page.Length; j++) {
            //        if (button_Level[i].transform.parent == obj_Page[j].transform) {
            //            button_Level[i].Init (levelId[j], OnClick_Level);
            //            levelId[j]++;
            //            break;
            //        }
            //    }
            //}
            //
            tab_ModeButtonPos = tab_ModeButtonPos_01;
            button_GameMode[3].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);
            // levelPic-name
            for (int i = 0; i < sprite_LevelPic.Length; i++)
            {
                image_LevelPic[i].sprite = sprite_LevelPic[i];
                //image_LevelPic[i].SetNativeSize ();
                if (Set.setVal.Language == (int)en_Language.Chinese)
                {
                    image_LevelPic[i].gameObject.SetActive(true);
                }
                else
                {
                    image_LevelPic[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < sprite_LevelName.Length; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                if (Set.setVal.Language == (int)en_Language.Chinese)
                {
                    image_LevelName[i].gameObject.SetActive(true);
                }
                else
                {
                    image_LevelName[i].gameObject.SetActive(false);
                }
            }
            button_GameMode[0].gameObject.SetActive(false);
            obj_Page[1].gameObject.SetActive(false);
            button_GameMode[1].gameObject.SetActive(false);
            obj_Page[2].gameObject.SetActive(false);
            button_GameMode[2].gameObject.SetActive(false);
            obj_Page[3].gameObject.SetActive(false);
            button_GameMode[3].gameObject.SetActive(false);
            obj_Page[4].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);
        }


        else if (gameChoose == (int)en_GameId.PanYan)
        {
            tab_ModeButtonPos = tab_ModeButtonPos_02;
            // levelPic-name

            for (int i = 0; i < 30; i++)
            {
                image_LevelPic[i].sprite = sprite_LevelPic[i];

            }
            //      
            for (int i = 0; i < 30; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                image_LevelName[i].gameObject.SetActive(true);
                if (i >= 20)
                {
                    image_LevelName[i].transform.localScale = Vector3.one * 2;
                }
            }
            obj_Page[4].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);
        }
        else if (gameChoose == (int)en_GameId.TouZhi)
        {

            tab_ModeButtonPos = tab_ModeButtonPos_01;
            // levelPic-name

            for (int i = 0; i < 30; i++)
            {
                image_LevelPic[i].sprite = sprite_LevelPic[i];
                image_LevelPic[i].SetNativeSize();
                image_LevelPic[i].transform.parent.GetComponent<Image>().enabled = false;

            }
            //      
            for (int i = 0; i < 30; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                image_LevelName[i].gameObject.SetActive(true);
                //if (i >= 20)
                //{
                //    image_LevelName[i].transform.localScale = Vector3.one * 2;
                //}
            }
            obj_Page[3].gameObject.SetActive(false);
            button_GameMode[3].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);
        }
        else if (gameChoose == (int)en_GameId.DevEyes)
        {

            tab_ModeButtonPos = tab_ModeButtonPos_01;
            // levelPic-name

            for (int i = 0; i < 30; i++)
            {
                image_LevelPic[i].sprite = sprite_LevelPic[i];
                image_LevelPic[i].SetNativeSize();
                image_LevelPic[i].transform.parent.GetComponent<Image>().enabled = false;
            }
            //      
            for (int i = 0; i < 30; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                image_LevelName[i].gameObject.SetActive(true);
                //if (i >= 20)
                //{
                //    image_LevelName[i].transform.localScale = Vector3.one * 2;
                //}
            }
            obj_Page[3].gameObject.SetActive(false);
            button_GameMode[3].gameObject.SetActive(false);
            obj_Page[4].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);

        }
        else if (gameChoose == (int)en_GameId.LanQiu)
        {

            tab_ModeButtonPos = tab_ModeButtonPos_01;
            // levelPic-name

            for (int i = 0; i < 30; i++)
            {
                image_LevelPic[i].sprite = sprite_LevelPic[i];
                image_LevelPic[i].SetNativeSize();
                image_LevelPic[i].transform.parent.GetComponent<Image>().enabled = false;
            }
            //      
            for (int i = 0; i < 30; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                image_LevelName[i].gameObject.SetActive(true);
                //if (i >= 20)
                //{
                //    image_LevelName[i].transform.localScale = Vector3.one * 2;
                //}
            }
            obj_Page[3].gameObject.SetActive(false);
            button_GameMode[3].gameObject.SetActive(false);
            obj_Page[4].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);
        }
        else if (gameChoose == (int)en_GameId.PaiPaiDeng)
        {

            tab_ModeButtonPos = tab_ModeButtonPos_01;
            // levelPic-name

            for (int i = 0; i < 10; i++)
            {
                image_LevelPic[i].sprite = sprite_LevelPic[i];
                image_LevelPic[i].SetNativeSize();
                image_LevelPic[i].transform.localScale = Vector3.one * 0.6f;
                image_LevelPic[i].transform.parent.GetComponent<Image>().enabled = false;
            }
            //      
            for (int i = 0; i < 10; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                image_LevelName[i].gameObject.SetActive(true);
                //if (i >= 20)
                //{
                //    image_LevelName[i].transform.localScale = Vector3.one * 2;
                //}
            }

            button_GameMode[0].gameObject.SetActive(false);
            obj_Page[1].gameObject.SetActive(false);
            button_GameMode[1].gameObject.SetActive(false);
            obj_Page[2].gameObject.SetActive(false);
            button_GameMode[2].gameObject.SetActive(false);

            obj_Page[3].gameObject.SetActive(false);
            button_GameMode[3].gameObject.SetActive(false);
            obj_Page[4].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);
        }
        else if (gameChoose == (int)en_GameId.LeiShePPD)
        {

            tab_ModeButtonPos = tab_ModeButtonPos_01;
            // levelPic-name

            //for (int i = 0; i < 10; i++)
            //{
            //    image_LevelPic[i].sprite = sprite_LevelPic[i];
            //    image_LevelPic[i].SetNativeSize();
            //    image_LevelPic[i].transform.localScale = Vector3.one * 0.6f;
            //    //image_LevelPic[i].transform.parent.GetComponent<Image>().enabled = false;
            //    if (Set.setVal.Language == (int)en_Language.Chinese)
            //    {
            //        image_LevelPic[i].gameObject.SetActive(true);
            //    }
            //    else
            //    {
            //        image_LevelPic[i].gameObject.SetActive(false);
            //    }
            //}
            //      
            for (int i = 0; i < 10; i++)
            {
                image_LevelName[i].sprite = sprite_LevelName[i];
                image_LevelName[i].SetNativeSize();
                if (Set.setVal.Language == (int)en_Language.Chinese)
                {
                    image_LevelName[i].gameObject.SetActive(true);
                }
                else
                {
                    image_LevelName[i].gameObject.SetActive(false);
                }
                //if (i >= 20)
                //{
                //    image_LevelName[i].transform.localScale = Vector3.one * 2;
                //}
            }

            button_GameMode[0].gameObject.SetActive(false);
            obj_Page[1].gameObject.SetActive(false);
            button_GameMode[1].gameObject.SetActive(false);
            obj_Page[2].gameObject.SetActive(false);
            button_GameMode[2].gameObject.SetActive(false);
            obj_Page[3].gameObject.SetActive(false);
            button_GameMode[3].gameObject.SetActive(false);
            obj_Page[4].gameObject.SetActive(false);
            button_GameMode[4].gameObject.SetActive(false);
        }
        for (int i = 0; i < button_GameMode.Length; i++)
        {

            button_GameMode[i].transform.localPosition = tab_ModeButtonPos[i];

        }

    }


    void Update_GameModeSelect()
    {
        for (int i = 0; i < button_GameMode.Length; i++)
        {
            if (i == page_Index)
            {
                button_GameMode[i].SetSelectFlag(true);
                button_GameMode[i].transform.localScale = Vector3.one * 1.1f;
            }
            else
            {
                button_GameMode[i].SetSelectFlag(false);
                button_GameMode[i].transform.localScale = Vector3.one * 1f;
            }
        }
    }



    public void OnClick_Back()
    {

        game97_Main.ChangeStatue(en_Game97_Sta.Idle);

    }
    public void OnClick_Start()
    {
        if (Main.statue != en_MainStatue.Game_97)
            return;
        if (game97_Main.statue != en_Game97_Sta.GameSelect)
            return;
        if (Main.settingError != en_ErrorCode.None)
            return;
        if (Set.setVal.Width >= 20 || Set.setVal.Height >= 20)
        {
            game97_Main.ChangeStatue(en_Game97_Sta.DiffcultySelect);
        }
        else
        {
            Game97_PlayerModeSel.selectId = (int)en_PlayerMode.Free;
            game97_Main.ChangeStatue(en_Game97_Sta.DiffcultySelect);
        }
    }


    public void OnClick_ButtonMode(int id)
    {
        page_Index = id;
        if (id == 4)
        {
            Img_Challenge.gameObject.SetActive(true);

        }
        Debug.LogError("???");
        Update_GameModeSelect();
    }
    public void OnClick_Level(int id)
    {

        switch (Set.setVal.GameChoose)
        {
            case (int)en_GameId.YueDongGeZi:
                Main.MapIndex = id;
                if (id < 30)
                {
                    id = Level[id];
                }

                Main.MapID = id;
                break;
            case (int)en_GameId.LeiSheWu:
                Main.playerMode = (en_PlayerMode)page_Index;
                Main.gameLevel = Main.MapIndex = id % 10;
                break;
            case (int)en_GameId.PanYan:
                Main.MapIndex = id;
                Main.MapID = id;
                break;
            case (int)en_GameId.LanQiu:
                Main.MapIndex = id;
                Main.MapID = id;
                break;
            case (int)en_GameId.TouZhi:
                Main.MapIndex = id;
                Main.MapID = id;
                break;
            case (int)en_GameId.DevEyes:
                Main.MapIndex = id;
                Main.MapID = id;
                break;
            case (int)en_GameId.PaiPaiDeng:
                Main.MapIndex = id;
                Main.MapID = id;
                break;
            case (int)en_GameId.LeiShePPD:
                Main.MapIndex = id;
                Main.MapID = id;
                break;
        }


        //镭射用：

        Game97_Main.instance.EnterGame(0);
    }


    private void Update()
    {
        if (Main.PlayTime <= 0)
        {
            Main.instance.ChangeStatue(en_MainStatue.Game_97);
        }

    }
}
