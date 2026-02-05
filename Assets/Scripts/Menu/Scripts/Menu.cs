using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;		//调用库

public enum en_MenuStatue
{
    MenuSta_CheckPassword = 0,
    MenuSta_SysSet,
    MenuSta_GameSet,
    MenuSta_WinSet,
    MenuSta_PresetPicSet,
    MenuSta_AnimSet,
    MenuSta_GameLevelSet,
    MenuSta_GameSelect,
    MenuSta_GameDownLoad,
    MenuSta_Acc,
    MenuSta_AdcVerify,
    MenuSta_SizeSet,
    MenuSta_UpdateVideo,
    MenuSta_UpdateIoApp,
    MenuSta_IoCheck,
    MenuSta_Dna,
    MenuSta_Enc,
    MenuSta_MusicMenu,
    MenuSta_MusicManager,
    MenuSta_Language,
    MenuSta_LevelSet,
};

public class Menu : MonoBehaviour
{
    //
    public const bool SCREEN_H = true;      // 横屏

    // 坐标,大小定义 X: 从左到右, Y: 从下到上
    public const float DEFAULT_SCREEN_WIDTH = 1280;
    public const float DEFAULT_SCREEN_HEIGHT = 720;

    public const float SCREEN_WIDTH = 1280; // Main.DEFAULT_SCREEN_WIDTH;
    public const float SCREEN_HEIGHT = 720; // Main.DEFAULT_SCREEN_HEIGHT;

    public static float MENU_CODE_START_Y = 630;                //C1开始坐标 Y
    public static float MENU_CODE_DISTANCE_Y = 90;          //C1开始坐标 Y
    public static float MENU_CODE_BUTTON_Y = 60;                //C1按键坐标 Y
    public static float MENU_CODE_BUTTON_DISTANCE_X = 330;  //C1按键 X 间距
    public static float MENU_CURSOR_CODE_START_Y = 630;     //C1开始坐标 Y
    public static float MENU_CURSOR_CODE_DISTANCE_Y = 90;   //C1开始坐标 Y

    // 页面:
    Main gmain;
    public Text text_VersionText;
    public Text text_IoVersion;
    public Menu_CreateTips createTips;
    public Menu_Tips menuTips;
    public Menu_PasswordManager passwordManager;
    public Menu_SysSet menu_SysSet;
    public Menu_GameSet menu_GameSet;
    //public Menu_WinSet menu_WinSet;
    public Menu_GameSelect menu_GameSelect;
    public Menu_FileDownLoad menu_GameDownLoad;
    public Menu_UpdateIoApp menu_UpdateIoApp;
    public Menu_AdcVerify menu_AdcVerify;
    public Menu_Acc menu_Acc;
    //public Menu_UpdateVideo menu_UpdateVideo;
    public Menu_IoCheck menu_IoCheck;
    public Menu_SizeSet menu_SizeSet;
    public Menu_AnimSet menu_AnimSet;
    public Menu_GameLevelSet menu_GameLevelSet;
    public Menu_PresetPicSet menu_PresetPicSet;
    public Menu_Dna menu_Dna;
    public Menu_Enc menu_Enc;
    public Menu_Language menu_Language;
    public Menu_LevelSet menu_LevelSet;

    public Menu_MusicMenu menu_MusicMenu;
    public Menu_MusicManager menu_MusicManager;

    static GameObject planeOld;
    static GameObject planeCurr;

    public static en_MenuStatue statue;

    float planePosX;
    int ioVersion;
    //    
    public void Awake0(Main main)
    {
        //public void Awake0() { 
        gmain = main;
        //初始化接口
        planeOld = null;
        planeCurr = null;

        Menu_Tips.instance = menuTips;
        createTips.Init(menuTips);
        passwordManager.Init(this);
        menu_SysSet.Awake0(this);
        menu_GameSet.Awake0(this);
        //menu_WinSet.Awake0(this);
        menu_GameSelect.Awake0(this);
        menu_GameDownLoad.Awake0(this);
        menu_UpdateIoApp.Awake0(this);
        menu_AdcVerify.Awake0(this);
        menu_Acc.Awake0(this);
        menu_MusicManager.Awake0(this);

        menu_MusicMenu.Awake0(this);
        menu_IoCheck.Awake0(this);
        menu_SizeSet.Awake0(this);
        menu_AnimSet.Awake0(this);
        menu_GameLevelSet.Awake0(this);
        menu_PresetPicSet.Awake0(this);
        menu_LevelSet.Awake0(this);

        //menu_UpdateVideo.Awake0(this);
        //menu_Dna.Awake0(this);
        //menu_Enc.Awake0(this);
        menu_Language.Awake0(this);

        //
        // 大小调整
        //menu_SysSet.transform.parent.localScale = new Vector3 ((float)Screen.width / DEFAULT_SCREEN_WIDTH, (float)Screen.height / DEFAULT_SCREEN_HEIGHT, 1);
        // 版本显示
        text_VersionText.text = "Version: " + Main.VERSION;
    }

    public void Enter()
    {
        Update_IoVersion();
        transform.localPosition = new Vector3(0, 0, 0);
        //
        //if (Main.VER_DNA && Game_Enc.MACHINE_NO < 0) {
        //	ChangeStatue (en_MenuStatue.MenuSta_Dna);
        //} else if (Main.VER_ENC && Game_Enc.mactime <= 0) {
        //	ChangeStatue (en_MenuStatue.MenuSta_Enc);
        //} else {
        //	ChangeStatue (en_MenuStatue.MenuSta_SysSet);
        //}
        ChangeStatue(en_MenuStatue.MenuSta_SysSet);
        ChangeStatue(en_MenuStatue.MenuSta_CheckPassword);

    }

    // Update is called once per frame
    void Update()
    {
        if (ioVersion != Main.ioVersion)
        {
            Update_IoVersion();
        }

        //if (Main.statue != en_MainStatue.Game_98)
        //	return;
        //界面移动
        if (statue == en_MenuStatue.MenuSta_CheckPassword)
        {
            if (passwordManager.gameObject.activeSelf == false)
            {
                if (passwordManager.optionType == en_PasswordOptionType.Check && passwordManager.checkResult == false)
                {
                    Quit();
                }
                else
                {
                    ChangeStatue(en_MenuStatue.MenuSta_SysSet);
                }
            }
        }
        if (planePosX > 0)
        {
            planePosX -= Time.deltaTime * 6605;
            if (planePosX < 0)
                planePosX = 0;
            ShowPlane();
        }
        else if (planePosX < 0)
        {
            planePosX += Time.deltaTime * 6605;
            if (planePosX > 0)
                planePosX = 0;
            ShowPlane();
        }
    }

    public void ChangeStatue(en_MenuStatue sta)
    {
        //Debug.Log("Menu 状态：" + sta);
        statue = sta;
        Key.Clear();

        //  CmdIO_YDGZ.CMD0_SendCmd_GameStatue (0, 0, 0);
        menu_MusicMenu.gameObject.SetActive(false);
        menu_MusicManager.gameObject.SetActive(false);

        createTips.gameObject.SetActive(false);
        passwordManager.gameObject.SetActive(false);
        menuTips.gameObject.SetActive(false);
        menu_SysSet.gameObject.SetActive(false);
        menu_GameSet.gameObject.SetActive(false);
        //menu_WinSet.gameObject.SetActive(false);
        menu_GameSelect.gameObject.SetActive(false);
        menu_GameDownLoad.gameObject.SetActive(false);
        menu_UpdateIoApp.gameObject.SetActive(false);
        menu_AdcVerify.gameObject.SetActive(false);
        menu_Acc.gameObject.SetActive(false);
        //menu_UpdateVideo.gameObject.SetActive(false);
        menu_IoCheck.gameObject.SetActive(false);
        menu_SizeSet.gameObject.SetActive(false);
        menu_AnimSet.gameObject.SetActive(false);
        menu_GameLevelSet.gameObject.SetActive(false);
        menu_PresetPicSet.gameObject.SetActive(false);
        menu_Dna.gameObject.SetActive(false);
        menu_Enc.gameObject.SetActive(false);
        menu_Language.gameObject.SetActive(false);
        menu_LevelSet.gameObject.SetActive(false);
        passwordManager.waitCheck = false;

        planeOld = planeCurr;
#if SHOOT_BEAD || SHOOT_WATER
        GameMain.GunMotorStatue_Clear();
#endif
        switch (statue)
        {
            case en_MenuStatue.MenuSta_CheckPassword:
                passwordManager.gameObject.SetActive(true);
                passwordManager.GameStart(en_PasswordType.Menu, en_PasswordOptionType.Check);
                //planeCurr = passwordManager.gameObject;
                //planePosX = -SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_SysSet:
                menu_SysSet.gameObject.SetActive(true);
                menu_SysSet.GameStart();
                planeCurr = menu_SysSet.gameObject;
                planePosX = -SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_GameSet:
                menu_GameSet.gameObject.SetActive(true);
                menu_GameSet.GameStart();
                planeCurr = menu_GameSet.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_MusicManager:
                menu_MusicManager.gameObject.SetActive(true);
                //menu_MusicManager.GameStart ();
                planeCurr = menu_MusicManager.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_MusicMenu:
                menu_MusicMenu.gameObject.SetActive(true);
                menu_MusicMenu.GameStart();
                planeCurr = menu_MusicMenu.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            //case en_MenuStatue.MenuSta_WinSet:
            //    menu_WinSet.gameObject.SetActive(true);
            //    menu_WinSet.GameStart();
            //    planeCurr = menu_WinSet.gameObject;
            //    planePosX = SCREEN_WIDTH;
            //    break;
            case en_MenuStatue.MenuSta_GameSelect:
                menu_GameSelect.gameObject.SetActive(true);
                menu_GameSelect.GameStart();
                planeCurr = menu_GameSelect.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_GameDownLoad:
                menu_GameDownLoad.gameObject.SetActive(true);
                //menu_GameDownLoad.GameStart ();
                planeCurr = menu_GameDownLoad.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_UpdateIoApp:
                menu_UpdateIoApp.gameObject.SetActive(true);
                menu_UpdateIoApp.GameStart();
                planeCurr = menu_UpdateIoApp.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_AdcVerify:
                menu_AdcVerify.gameObject.SetActive(true);
                menu_AdcVerify.GameStart();
                planeCurr = menu_AdcVerify.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_LevelSet:

                menu_LevelSet.gameObject.SetActive(true);
                menu_LevelSet.GameStart();
                planeCurr = menu_LevelSet.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_Acc:
                menu_Acc.gameObject.SetActive(true);
                menu_Acc.GameStart();
                planeCurr = menu_Acc.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            //case en_MenuStatue.MenuSta_UpdateVideo:
            //    menu_UpdateVideo.gameObject.SetActive(true);
            //    menu_UpdateVideo.GameStart();
            //    planeCurr = menu_UpdateVideo.gameObject;
            //    planePosX = SCREEN_WIDTH;
            //    break;
            case en_MenuStatue.MenuSta_IoCheck:
                menu_IoCheck.gameObject.SetActive(true);
                menu_IoCheck.GameStart();
                planeCurr = menu_IoCheck.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_SizeSet:
                menu_SizeSet.gameObject.SetActive(true);
                menu_SizeSet.GameStart();
                planeCurr = menu_SizeSet.gameObject;
                planePosX = SCREEN_WIDTH;
                break;

            case en_MenuStatue.MenuSta_AnimSet:
                menu_AnimSet.gameObject.SetActive(true);
                menu_AnimSet.GameStart();
                planeCurr = menu_AnimSet.gameObject;
                planePosX = SCREEN_WIDTH;
                break;

            case en_MenuStatue.MenuSta_GameLevelSet:
                menu_GameLevelSet.gameObject.SetActive(true);
                //menu_GameLevelSet.GameStart ();
                planeCurr = menu_GameLevelSet.gameObject;
                planePosX = SCREEN_WIDTH;
                break;

            case en_MenuStatue.MenuSta_PresetPicSet:
                menu_PresetPicSet.gameObject.SetActive(true);
                menu_PresetPicSet.GameStart();
                planeCurr = menu_PresetPicSet.gameObject;
                planePosX = SCREEN_WIDTH;
                break;

            case en_MenuStatue.MenuSta_Enc:
                menu_Enc.gameObject.SetActive(true);
                menu_Enc.GameStart();
                planeCurr = menu_Enc.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_Dna:
                menu_Dna.gameObject.SetActive(true);
                menu_Dna.GameStart();
                planeCurr = menu_Dna.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
            case en_MenuStatue.MenuSta_Language:
                menu_Language.gameObject.SetActive(true);
                menu_Language.GameStart();
                planeCurr = menu_Language.gameObject;
                planePosX = SCREEN_WIDTH;
                break;
        }
        ShowPlane();
    }
    //
    void ShowPlane()
    {
        if (planeOld != null)
        {
            planeOld.transform.localPosition = new Vector3(planePosX - SCREEN_WIDTH, 0, 0);
        }
        if (planeCurr != null)
        {
            planeCurr.transform.localPosition = new Vector3(planePosX, 0, 0);
        }
    }

    void Update_IoVersion()
    {
        ioVersion = Main.ioVersion;
        text_IoVersion.text = "IoVersion: " + ioVersion.ToString("X4");
    }
    public void Quit()
    {
        //
        gmain.ChangeStatue_To_GameIdle();
    }
}
