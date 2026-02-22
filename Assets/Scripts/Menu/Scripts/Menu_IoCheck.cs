using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_IoCheck : MonoBehaviour
{
    const int MAX_ACC_ONEPAGE = 6;
    int MAX_ACC;
    const int MAX_SET_SEL = 1;
    const int MAX_BUTTON = 1;
    const int MAX_SEL = MAX_SET_SEL + MAX_BUTTON;

    const int MAX_PAGE = Main.MAX_PLAYER + 1;

    // 要显示的内容(中英文切换)    
    public Text text_Title;
    public Menu_Button[] button;
    public Menu_Button[] button_Out;
    public AccValue[] accValue_In;
    //public AccValue[] accValue_Adc;
    public Transform ledOne_Layer;
    public GameObject ledOne_Prefab;

    List<Menu_LedOne> list_LedOne = new List<Menu_LedOne>();


    float[] runTime = new float[MAX_SET_SEL];
    int[] runCnt;
    //
    int postIndex;
    int page;
    int keyStartId;
    int targetKeyId;
    float clearTotalTime;
    bool pressedLeft;
    bool pressedRight;
    bool pressedOK;

    //
    // 传入的变量
    Menu menu;
    Menu_PasswordManager passwordManager;
    Menu_Tips menuTips;
    //
    public void Awake0(Menu mmenu)
    {
        //
        menu = mmenu;
        passwordManager = menu.passwordManager;
        menuTips = menu.menuTips;

        for (int i = 0; i < button_Out.Length; i++)
        {
            button_Out[i].Init(i, OnClick_Button);
        }
        for (int i = 0; i < button.Length; i++)
        {
            button[i].Init(MAX_SET_SEL + i, OnClick_Button);
        }
        button[1].Init(MAX_SET_SEL + 1, NextLedTest);
        runCnt = new int[accValue_In.Length];
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (Menu.statue != en_MenuStatue.MenuSta_IoCheck)
            return;
        if (menuTips.gameObject.activeSelf)
            return;
        if (passwordManager.gameObject.activeSelf)
            return;
        Framebuffer.Update_PointColor(Ledcnt, 0x60, enPointSta.None);
        Framebuffer.Update_TransmitLedColor(Ledcnt, 0x60, enPointSta.Target);
        // 输入检测
        if (Key.KEYFJ_CinPressed(0))
        {
            runCnt[0]++;
            UpdateAccData(0);
        }
        if (Key.KEYFJ_CoutPressed(0))
        {
            runCnt[1]++;
            UpdateAccData(1);
        }
        if (LedKey.KeyPressed(targetKeyId))
        {
            runCnt[2]++;
            UpdateAccData(2);
        }

        for (int i = 0; i < list_LedOne.Count; i++)
        {
            list_LedOne[i].image_Pic.color = Color.red;
        }
        for (int i = 0; i < list_LedOne.Count; i++)
        {

            //   
            if (LedKey.GetKeyStatus(keyStartId + i) == 1)
            {
                int lednum = 0;
                for (int n = 0; n < Set.setVal.Width * Set.setVal.Height; n++)
                {
                    if (Framebuffer.isNewLeiShe)
                    {
                        if (GameLeiSheBase.tab_Point[n] == i)
                        {
                            //                        Debug.LogError(i + "   " +n);
                            list_LedOne[n].image_Pic.color = Color.green; ;
                        }
                    }
                    else
                    {
                        Debug.LogError(i + "  " + keyStartId);
                        list_LedOne[i].image_Pic.color = Color.green; ;
                    }

                }


            }


        }


        if (Key.MENU_OkPressed() || Key.KEYFJ_Menu_OkPressed() || pressedOK)
        {
            pressedOK = false;
            switch (postIndex)
            {
                case 0: //SSR1

                    IO.Out_SSR(0, 1);
                    break;

                case MAX_SET_SEL:
                    //返回
                    OnButton_AccBack_Pressed();
                    break;
                case 2:
                    NextLedTest(0);
                    break;

            }
            if (postIndex < MAX_SET_SEL)
            {
                runTime[postIndex] = 0.6f;
                button_Out[postIndex].image_BackG.color = Color.blue;
            }
        }
    }

    // 更新光标坐标和大小
    void UpdateCursor()
    {
        //设置项
        for (int i = 0; i < button_Out.Length; i++)
        {
            if (i == postIndex)
            {
                button_Out[i].SetSelect(true);
            }
            else
            {
                button_Out[i].SetSelect(false);
            }
        }
        //三个按键
        for (int i = 0; i < button.Length; i++)
        {
            if (i + MAX_SET_SEL == postIndex)
            {
                button[i].SetSelect(true);
            }
            else
            {
                button[i].SetSelect(false);
            }
        }
    }

    void Update_LedNum()
    {
        int len = Set.setVal.Width * Set.setVal.Height;
        if (len == list_LedOne.Count)
            return;
        // 去掉原来的：
        for (int i = 0; i < list_LedOne.Count; i++)
        {
            if (list_LedOne[i] != null)
            {
                Destroy(list_LedOne[i].gameObject);
            }
        }
        list_LedOne.Clear();
        // 新的：
        for (int i = 0; i < len; i++)
        {
            Menu_LedOne ledOne = Instantiate(ledOne_Prefab, ledOne_Layer).GetComponent<Menu_LedOne>();
            ledOne.Init(-1, i);
            list_LedOne.Add(ledOne);
        }
    }

    const int CH_ONE_WIDTH = 60;
    void Update_Led()
    {
        Menu_SizeSet.Update_LedPos(list_LedOne, (en_ConnectOrder)Set.setVal.Index_AnZhuang, CH_ONE_WIDTH, Set.setVal.Width, Set.setVal.Height, en_ArryType.Center);

        int width = CH_ONE_WIDTH * Set.setVal.Width;
        int height = CH_ONE_WIDTH * Set.setVal.Height;
        // Scale: 600 x 500
        float scalex = 1550f / width;
        float scaley = 550f / height;
        float scale = Mathf.Clamp(Mathf.Min(scalex, scaley), 0, 1);
        ledOne_Layer.transform.localScale = Vector3.one * scale;

    }

    void Update_LedStatue(int no, Color color)
    {
        if (no >= list_LedOne.Count)
            return;
        //   byte ledKeySta = LedKey.GetKeyStatus(i);
        //  Color color = Color.green;
        //   list_LedOne[GameLeiSheBase.tab_Point[no]].text_ID.text = "Yes~~!";
        // if (ledKeySta == 0)
        {
            //   color = Color.red;
            //  list_LedOne[GameLeiSheBase.tab_Point[no]].text_ID.text = "NO~~!";
        }
        //  list_LedOne[GameLeiSheBase.tab_Point[no]].image_Pic.enabled = false;

        list_LedOne[no].image_Pic.color = color;
    }
    int Ledcnt = 0;
    void NextLedTest(int id)
    {

        Framebuffer.Update_ColorFull(0, enPointSta.None);

        Ledcnt++;
        if (Ledcnt >= Set.setVal.Width * Set.setVal.Height)
        {
            Ledcnt = 0;
        }
    }
    // 1.进入初始化'

    public void GameStart()
    {
        Ledcnt = 0;
        keyStartId = LedKey.GetLeiSheKeyStartId();
        targetKeyId = LedKey.GetLeiSheTargetKeyId();

        Framebuffer.Update_ColorFull(0x60, enPointSta.None);
        UpdateLanguage();
        //
        pressedLeft = false;
        pressedRight = false;
        pressedOK = false;

        page = 0;
        postIndex = MAX_SET_SEL;		// 下一页
        for (int i = 0; i < runCnt.Length; i++)
        {
            runCnt[i] = 0;
            UpdateAccData(i);
        }


        clearTotalTime = 0;
        UpdateCursor();

        //
        Update_LedNum();
        Update_Led();

        //for (int i = 0; i < list_LedOne.Count; i++)
        //{
        //    Update_LedStatue(i);
        //}
    }


    // 2.更新显示中英文
    public void UpdateLanguage()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            //中文
            //按键
            text_Title.text = "IO检测";
            button[0].GetComponentInChildren<Text>().text = "返回";
            button[1].GetComponentInChildren<Text>().text = " 测试";
        }
        else
        {
            //英文
            //按键
            text_Title.text = "IO Check";
            button[0].GetComponentInChildren<Text>().text = "Back";
            button[1].GetComponentInChildren<Text>().text = "Test";
        }
        AccVal_UpdataLanguage();
    }

    //按键 ------
    public void OnClick_Button(int id)
    {
        postIndex = id;
        UpdateCursor();
        pressedOK = true;
    }
    public void OnButton_AccBack_Pressed()
    {
        IO.Init();
        Framebuffer.Update_ColorFull(0, enPointSta.None);
        menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
    }

    // 需要修改的内容 -------------------------------------------------------------------------------------------------------

    // 2.更新显示中英文
    void AccVal_UpdataLanguage()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            //中文
            //账目

        }
        else
        {
            //英文
            //账目

        }
    }

    // 更新账目
    void UpdateAccData(int no)
    {
        if (no >= accValue_In.Length)
            return;
        accValue_In[no].SetData(runCnt[no]);
    }
}
