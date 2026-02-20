using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Menu_LevelSet : MonoBehaviour
{




    public Button button_Save;
    public Button button_Default;
    public Button button_Back;

    const float GAMELEVE_ONE_HEIGHT = 675;

    string gameName;

    string[] picNames;
    string[] animNames;

    int selectId;
    string currLevelPicName;
    string currLevelAnimName;
    float gameLevelSetLayerLimitUp;

    public en_Game00_Sta statue;
    float runTime;
    int gameId = 30;
    public int setIndex;
    public int setCount;
    bool cmdRetSucess;
    int playerMode;
    int gameLevel;
    public int maxLevel;
    int gameChoose;
    Menu menu;
    Menu_Tips menuTips;
    const int MAX_ALL_SET_SEL = 30;
    public GameObject setVal_Layer;
    public GameObject[] List_setVal_Prefab;
      int[] ChooseOpenLevel = new int[30];
      int[] NowOpenLevel = new int[30];


    Sprite[] sprite_LevelName;
    // 设置项	
    //SetValue[] setVal = new SetValue[MAX_ALL_SET_SEL];
    //List<SetValue> list_SetVal = new List<SetValue>();

    // Use this for initialization
   public int OpenIndex = 0;
    public static Menu_LevelSet instance;
    void OnEnable()
    {

        GameStart();
    }
    public void Awake0(Menu mmenu)
    {
        //

        menu = mmenu;

        menuTips = menu.menuTips;

        instance = this;

        //


        button_Save.onClick.AddListener(OnClick_Save);
        button_Default.onClick.AddListener(OnClick_Default);
        button_Back.onClick.AddListener(OnClick_Back);


        GameStart();

    }
    public void GameStart()
    {
        OpenIndex = 0;

        for (int i = 0; i < 30; i++)
        {
          //  ChooseOpenLevel[i] = 2;// PlayerPrefs.GetInt("ChooseOpenLevel" + i.ToString());

            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().txt_level_index.text = "";
            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().UpdateValue(2);
        }


        for (int i = 0; i < NowOpenLevel.Length; i++)
        {
            NowOpenLevel[i] = PlayerPrefs.GetInt("NowOpenLevel" + i.ToString());

            if (NowOpenLevel[i] != -1)
            {

             
                OpenIndex++;
            }
        }

        Update_Languae();
        Update_Choose();
    }
    public void OnClick_Save()
    {
        for (int i = 0; i < 30; i++)
        {

            ChooseOpenLevel[i] = List_setVal_Prefab[i].GetComponent<Menu_SetOne>().GetValue();
            PlayerPrefs.SetInt("ChooseOpenLevel" + i.ToString(), ChooseOpenLevel[i]);


        }

        OnClick_Back();
    }
    public void OnClick_Default()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("是否恢复默认值", DefaultSet);
        }
        else
        {
            Menu_Tips.instance.Init("Restore default setting?", DefaultSet);
        }
    }
    public void OnClick_Back()
    {
        //gameObject.SetActive (false);
        //if (OpenIndex<10)
        //{
        //    if (Set.setVal.Language == (int)en_Language.Chinese)
        //    {
        //        Menu_Tips.instance.Init("请选择10个游戏", 1, true);

        //    }
        //    else
        //    {
        //        Menu_Tips.instance.Init("Please Choose 10 Games", 1, true);

        //    }
        //    return;
        //}
        menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
    }
    void DefaultSet(bool isOk)
    {
        if (isOk == false)
            return;

        //
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("恢复默认值成功", 3, true);
        }
        else
        {
            Menu_Tips.instance.Init("Restore Default Success", 3, true);
        }

       
        for (int i = 0; i <30; i++)
        {
            NowOpenLevel[i] =-1;
            PlayerPrefs.SetInt("NowOpenLevel" + i.ToString(), NowOpenLevel[i]);

        }
       
        OpenIndex = 0;
        Update_Choose();

        for (int i = 0; i < 30; i++)
        {
            ChooseOpenLevel[i] = 2;
            PlayerPrefs.SetInt("ChooseOpenLevel" + i.ToString(), 2);
            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().txt_level_index.text = "";
            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().UpdateValue(2);

        }

    
    }
    void Update_Choose()
    {
        for (int i = 0; i < NowOpenLevel.Length; i++)
        {
            NowOpenLevel[i] = PlayerPrefs.GetInt("NowOpenLevel" + i.ToString());


            if (NowOpenLevel[i] != -1)
            {

                ChooseOpenLevel[NowOpenLevel[i]] = 1;
                List_setVal_Prefab[NowOpenLevel[i]].GetComponent<Menu_SetOne>().txt_level_index.text = (i + 1).ToString();
                List_setVal_Prefab[NowOpenLevel[i]].GetComponent<Menu_SetOne>().UpdateValue(1);


            }

        }
    }
    void Update_Languae()
    {

        string companyPath = "";
        companyPath = "Company_04" + "/";
        
                sprite_LevelName = Resources.LoadAll<Sprite>(companyPath + "UI/LevelName");
     

        if (Set.setVal.Language == (int)en_Language.Chinese)
        {

            button_Save.GetComponentInChildren<Text>().text = "保 存 设 置";
            button_Default.GetComponentInChildren<Text>().text = "默 认 值";
            button_Back.GetComponentInChildren<Text>().text = "返 回";



        }
        else
        {

            button_Save.GetComponentInChildren<Text>().text = "Save";
            button_Default.GetComponentInChildren<Text>().text = "Default";
            button_Back.GetComponentInChildren<Text>().text = "Return";

        }
        for (int i = 0; i < List_setVal_Prefab.Length; i++)
        {
            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.sprite = sprite_LevelName[i];
            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.SetNativeSize();

            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().SetValueInit(Set.SET_c_Open);
            List_setVal_Prefab[i].GetComponent<Menu_SetOne>().UpdateValue(ChooseOpenLevel[i]);
            if (Set.setVal.Language == 0)
            {
                List_setVal_Prefab[i].GetComponent<Menu_SetOne>().SetValueName(2, "关");

                List_setVal_Prefab[i].GetComponent<Menu_SetOne>().SetValueName(1, "开");

            }
            else
            {
                List_setVal_Prefab[i].GetComponent<Menu_SetOne>().SetValueName(2, "Close");
               
                    List_setVal_Prefab[i].GetComponent<Menu_SetOne>().SetValueName(1, "Useing");
              
            }



        }

    }
    void Is_InGroup(int num)
    {

        for (int i = 0; i < NowOpenLevel.Length; i++)
        {

            if (NowOpenLevel[i] == num)
            {
                return;
            }


        }
        //if (OpenIndex >= 10)
        //{
        //    return;
        //}

        NowOpenLevel[OpenIndex] = num;
        PlayerPrefs.SetInt("NowOpenLevel" + OpenIndex.ToString(), num);


        OpenIndex++;
        Update_Choose();

    }
    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < List_setVal_Prefab.Length; i++)
        {

            
            if (List_setVal_Prefab[i].GetComponent<Menu_SetOne>().GetValue() == 1)
            {
                if (List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.color != new Color(0, 1, 0))
                {
                    List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.color = new Color(0, 1, 0);
                    Is_InGroup(i);
                    List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.SetNativeSize();
                }
              
            }
            else
            {
                if (List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.color != new Color(1, 1, 1))
                {
                    List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.color = new Color(1, 1, 1);
                    List_setVal_Prefab[i].GetComponent<Menu_SetOne>().img_levelName.SetNativeSize();
                }
             

            }
           
        }
    }
}
