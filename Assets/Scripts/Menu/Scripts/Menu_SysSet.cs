

#define GAMESELECT
//#define GAMELEVELSET
//#define GAMEDOWNLOAD
#define ANIMTSET
//#define ADCVERIFY


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum en_ButtonId : int
{
    GameSet = 0,
    //WinSet,
    SizeSet,
    PresetPicSet,
#if ANIMTSET
    AnimSet,
#endif
#if GAMELEVELSETS
    GameLevelSet,
#endif
#if GAMESELECT
    GameSelect,
#endif

#if GAMEDOWNLOAD
    GameDownLoad,
#endif
#if ACC
    Acc,
#endif
    UpdateIoApp,
#if ADCVERIFY
    AdcVerify,
#endif
    //UpdateVideo,
#if DEF_ENC
    Enc,
#endif
    IoCheck,

    MusicManager,
    ChangePassword,
    ClearRankList,
#if LANGUAGE_ALL
    Language,
#endif
    LevelSet,
    Back,
    Count,
}

public class Menu_SysSet : MonoBehaviour
{

    const int BUTTON_ID_GAMESET = (int)en_ButtonId.GameSet;
    const int BUTTON_ID_LEVELSET = (int)en_ButtonId.LevelSet;


    const int BUTTON_ID_SIZESET = (int)en_ButtonId.SizeSet;
    const int BUTTON_ID_PRESETPICSET = (int)en_ButtonId.PresetPicSet;
#if ANIMTSET
    const int BUTTON_ID_ANIMSET = (int)en_ButtonId.AnimSet;
#endif
#if GAMELEVELSET
    const int BUTTON_ID_GAMELEVELSET = (int)en_ButtonId.GameLevelSet;
#endif
    //const int BUTTON_ID_WINSET = (int)en_ButtonId.WinSet;
#if GAMESELECT
    const int BUTTON_ID_GAMESELECT = (int)en_ButtonId.GameSelect;

#endif
#if GAMEDOWNLOAD
    const int BUTTON_ID_GAMEDOWNLOAD = (int)en_ButtonId.GameDownLoad;
#endif
#if ACC
    const int BUTTON_ID_ACC = (int)en_ButtonId.Acc;
#endif

    const int BUTTON_ID_UPDATEIOAPP = (int)en_ButtonId.UpdateIoApp;
    const int BUTTON_ID_MUSICMANAGER = (int)en_ButtonId.MusicManager;
    const int BUTTON_ID_CHANGEPSW = (int)en_ButtonId.ChangePassword;
#if ADCVERIFY
    const int BUTTON_ID_ADCVERIFY = (int)en_ButtonId.AdcVerify;
#endif
    //const int BUTTON_ID_UPDATEVIDEO = (int)en_ButtonId.UpdateVideo;
#if DEF_ENC
    const int BUTTON_ID_ENC = (int)en_ButtonId.Enc;
#endif
    const int BUTTON_ID_IOCHECK = (int)en_ButtonId.IoCheck;
#if LANGUAGE_ALL
    const int BUTTON_ID_LANGUAGE = (int)en_ButtonId.Language;
#endif
    const int BUTTON_ID_ClearRankList = (int)en_ButtonId.ClearRankList;
    const int BUTTON_ID_BACK = (int)en_ButtonId.Back;
    const int MAX_BUTTON = (int)en_ButtonId.Count;

    // 要显示的内容(中英文切换)    
    public Text text_SysSet;
    public Menu_Button[] button;
    public Menu_Button button_Back;
    static int postIndex;
    bool pressedButton;

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
        //        
        for (int i = 0; i < button.Length; i++)
        {
            button[i].gameObject.SetActive(false);
        }
        button[BUTTON_ID_BACK] = button_Back;
        for (int i = 0; i < MAX_BUTTON; i++)
        {
            button[i].Init(i, OnClick_Button);
            button[i].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Menu.statue != en_MenuStatue.MenuSta_SysSet)
            return;
        if (menuTips.gameObject.activeSelf)
            return;
        if (passwordManager.gameObject.activeSelf)
            return;
        if (passwordManager.waitCheck)
        {
            passwordManager.waitCheck = false;
            if (passwordManager.checkResult)
            {
#if ACC
      if (postIndex == BUTTON_ID_ACC) {
                    OnButton_Acc_Pressed ();
                    return;
                }
#endif

            }
        }


        if (Key.MENU_LeftPressed() || Key.KEYFJ_Menu_LeftPressed())
        {
            postIndex = (postIndex + MAX_BUTTON - 1) % MAX_BUTTON;
            UpdateCursor();
        }
        if (Key.MENU_RightPressed() || Key.KEYFJ_Menu_RightPressed())
        {
            postIndex = (postIndex + 1) % MAX_BUTTON;
            UpdateCursor();
        }
        if (Key.MENU_OkPressed() || Key.KEYFJ_Menu_OkPressed() || pressedButton)
        {
            pressedButton = false;
            switch (postIndex)
            {
                case BUTTON_ID_GAMESET: OnButton_GameSet_Pressed(); break;
                case BUTTON_ID_ClearRankList: OnButton_ClearRankList_Pressed(); break;
                case BUTTON_ID_SIZESET: OnButton_SizeSet_Pressed(); break;
                case BUTTON_ID_PRESETPICSET: OnButton_PresetPicSet_Pressed(); break;
                case BUTTON_ID_MUSICMANAGER: OnButton_MusicManager_Pressed(); break;
#if GAMELEVELSET
            case BUTTON_ID_GAMELEVELSET: OnButton_GameLevelSet_Pressed (); break;
#endif
#if ANIMTSET
                case BUTTON_ID_ANIMSET: OnButton_AnimSet_Pressed(); break;
#endif
                //case BUTTON_ID_WINSET: OnButton_WinSet_Pressed(); break;
#if GAMESELECT
                case BUTTON_ID_GAMESELECT: OnButton_GameSelect_Pressed(); break;
#endif
#if GAMEDOWNLOAD
            case BUTTON_ID_GAMEDOWNLOAD: OnButton_GameDownLoad_Pressed (); break;
#endif
#if ADCVERIFY
                case BUTTON_ID_ADCVERIFY: OnButton_CursorVerify_Pressed(); break;
#endif
#if ACC
       case BUTTON_ID_ACC:
                    passwordManager.GameStart(en_PasswordType.Acc, en_PasswordOptionType.Check);
                    break;
#endif
                case BUTTON_ID_LEVELSET: OnButton_LevelSet_Pressed(); break;

                case BUTTON_ID_UPDATEIOAPP: OnButton_UpdateIoApp_Pressed(); break;
                case BUTTON_ID_CHANGEPSW: OnButton_ChangePassword_Pressed(); break;

                //case BUTTON_ID_UPDATEVIDEO: OnButton_UpdateVideo_Pressed(); break;
#if DEF_ENC
            case BUTTON_ID_ENC: OnButton_Enc_Pressed(); break;
#endif
                   case BUTTON_ID_IOCHECK: OnButton_IoCheck_Pressed (); break;
#if LANGUAGE_ALL
                case BUTTON_ID_LANGUAGE: OnButton_Language_Pressed(); break;
#endif
                case BUTTON_ID_BACK: OnButton_Back_Pressed(); break;
            }
        }
    }

    // 更新光标坐标和大小
    void UpdateCursor()
    {
        for (int i = 0; i < MAX_BUTTON; i++)
        {
            if (i == postIndex)
            {
                button[i].SetSelect(true);
            }
            else
            {
                button[i].SetSelect(false);
            }
        }
    }

    // 1.进入初始化
    public void GameStart()
    {
        pressedButton = false;
        UpdateLanguage();
        postIndex = 0;
        UpdateCursor();
        if (Set.setVal.GameChoose != 0)
        {
            if (button[BUTTON_ID_LEVELSET].gameObject.activeSelf)
            {
                button[BUTTON_ID_LEVELSET].gameObject.SetActive(false);

            }
        }
        else
        {
            if (!button[BUTTON_ID_LEVELSET].gameObject.activeSelf)
            {
                button[BUTTON_ID_LEVELSET].gameObject.SetActive(true);

            }
        }

    }
    // 2.更新显示中英文
    public void UpdateLanguage()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            //中文
            text_SysSet.text = "系 统 设 置";
            button[BUTTON_ID_GAMESET].GetComponentInChildren<Text>().text = "参 数 设 置";
            button[BUTTON_ID_ClearRankList].GetComponentInChildren<Text>().text = "清空排行榜";
            button[BUTTON_ID_SIZESET].GetComponentInChildren<Text>().text = "布 局 设 置";
            button[BUTTON_ID_LEVELSET].GetComponentInChildren<Text>().text = "自 选 关 卡";

            button[BUTTON_ID_PRESETPICSET].GetComponentInChildren<Text>().text = "图 案 设 置";
#if GAMELEVELSET
            button[BUTTON_ID_GAMELEVELSET].GetComponentInChildren<Text> ().text = "关 卡 设 置";
#endif
#if ANIMTSET
            button[BUTTON_ID_ANIMSET].GetComponentInChildren<Text>().text = "动 画 设 置";
#endif
            //button[BUTTON_ID_WINSET].GetComponentInChildren<Text>().text = "礼品设置";
#if GAMESELECT
            button[BUTTON_ID_GAMESELECT].GetComponentInChildren<Text>().text = "游 戏 选 择";
#endif
#if ADCVERIFY
            button[BUTTON_ID_ADCVERIFY].GetComponentInChildren<Text>().text = "地板硬件检测";
#endif
#if ACC
            button[BUTTON_ID_ACC].GetComponentInChildren<Text> ().text = "账 目 查 询";
#endif
            button[BUTTON_ID_UPDATEIOAPP].GetComponentInChildren<Text>().text = "控 制 板 程 序";
            //button[BUTTON_ID_UPDATEVIDEO].GetComponentInChildren<Text>().text = "更新视频";
#if DEF_ENC
            button[BUTTON_ID_ENC].GetComponentInChildren<Text>().text = "报账";
#endif
              button[BUTTON_ID_IOCHECK].GetComponentInChildren<Text> ().text = "IO 检 测";
#if LANGUAGE_ALL
            button[BUTTON_ID_LANGUAGE].GetComponentInChildren<Text>().text = "语言";
#endif
            button[BUTTON_ID_CHANGEPSW].GetComponentInChildren<Text>().text = "修 改 密 码";

            button[BUTTON_ID_MUSICMANAGER].GetComponentInChildren<Text>().text = " 音乐管理";
            button[BUTTON_ID_BACK].GetComponentInChildren<Text>().text = "返 回";
        }
        else
        {
            //英文
            text_SysSet.text = "System Set";
            button[BUTTON_ID_GAMESET].GetComponentInChildren<Text>().text = "Game Setting";
            button[BUTTON_ID_SIZESET].GetComponentInChildren<Text>().text = "Size Setting";
            button[BUTTON_ID_PRESETPICSET].GetComponentInChildren<Text>().text = "Graphic Setting";
#if ANIMTSET
            button[BUTTON_ID_ANIMSET].GetComponentInChildren<Text>().text = "Animation Setting";
#endif
            //button[BUTTON_ID_WINSET].GetComponentInChildren<Text>().text = "Gift Set";
#if GAMESELECT
            button[BUTTON_ID_GAMESELECT].GetComponentInChildren<Text>().text = "Game Select";
#endif
#if ADCVERIFY
            button[BUTTON_ID_ADCVERIFY].GetComponentInChildren<Text>().text = "HardWork Verify";
#endif
#if ACC
            button[BUTTON_ID_ACC].GetComponentInChildren<Text> ().text = "Acc";
#endif
            button[BUTTON_ID_UPDATEIOAPP].GetComponentInChildren<Text>().text = "Update IO App";
            //button[BUTTON_ID_UPDATEVIDEO].GetComponentInChildren<Text>().text = "Update Video";
#if DEF_ENC
            button[BUTTON_ID_ENC].GetComponentInChildren<Text>().text = "Report";
#endif
            //   button[BUTTON_ID_IOCHECK].GetComponentInChildren<Text> ().text = "IO Check";
#if LANGUAGE_ALL
            button[BUTTON_ID_LANGUAGE].GetComponentInChildren<Text>().text = "Language";
#endif
            button[BUTTON_ID_BACK].GetComponentInChildren<Text>().text = "Back";
            button[BUTTON_ID_CHANGEPSW].GetComponentInChildren<Text>().text = "Modify Password";

        }
    }

    //按键 ------
    public void OnClick_Button(int id)
    {
        postIndex = id;
        UpdateCursor();
        pressedButton = true;
    }

    void SaveSet(bool isOk)
    {
        if (isOk == false)
            return;
        RankList.ClearAll();
    }
    void OnButton_ClearRankList_Pressed()
    {
        if(Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("是否清空排行榜?", SaveSet);
        }
        else
        {
            Menu_Tips.instance.Init("Do you want to clear the leaderboard?", SaveSet);
        }

    }
    public void OnButton_GameSet_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_GameSet);
    }
    public void OnButton_SizeSet_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_SizeSet);
    }
    public void OnButton_PresetPicSet_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_PresetPicSet);
    }
    public void OnButton_AnimSet_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_AnimSet);
    }
    public void OnButton_GameLevelSet_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_GameLevelSet);
    }
    public void OnButton_WinSet_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_WinSet);
    }
    public void OnButton_GameSelect_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_GameSelect);
    }
    public void OnButton_GameDownLoad_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_GameDownLoad);
    }
    public void OnButton_LevelSet_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_LevelSet);
    }
    public void OnButton_CursorVerify_Pressed()
    {
        switch ((en_GameId)Set.setVal.GameChoose)
        {
            default:
            case en_GameId.YueDongGeZi:
                menu.ChangeStatue(en_MenuStatue.MenuSta_AdcVerify);
                break;
            case en_GameId.LeiSheWu:
                menu.ChangeStatue(en_MenuStatue.MenuSta_IoCheck);
                break;
            case en_GameId.PanYan:
                menu.ChangeStatue(en_MenuStatue.MenuSta_AdcVerify);
                break;

        }

    }
    public void OnButton_Acc_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_Acc);
    }
    public void OnButton_UpdateIoApp_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_UpdateIoApp);
    }
    public void OnButton_UpdateVideo_Pressed()
    {
        //    menu.ChangeStatue(en_MenuStatue.MenuSta_UpdateVideo);
    }
    public void OnButton_IoCheck_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_IoCheck);
    }
    public void OnButton_Enc_Pressed()
    {
        //   menu.ChangeStatue(en_MenuStatue.MenuSta_Enc);
    }
    public void OnButton_Language_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_Language);
    }
    public void OnButton_Back_Pressed()
    {
        //	Application.LoadLevel ("Loading");
        menu.Quit();
    }
    public void OnButton_MusicManager_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_MusicMenu);
    }
    public void OnButton_ChangePassword_Pressed()
    {
        menu.ChangeStatue(en_MenuStatue.MenuSta_CheckPassword);
        menu.passwordManager.GameStart(en_PasswordType.Menu, en_PasswordOptionType.Modify);
    }
}
