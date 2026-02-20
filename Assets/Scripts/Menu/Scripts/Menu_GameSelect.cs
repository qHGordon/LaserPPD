using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Menu_GameSelect : MonoBehaviour
{
    // 操作类型：
    enum en_OptionType
    {
        None = 0,
        Delete,
        Create,
        Setting,
        GameDownload,
        Save,
        Default,
        Back,
    }

    const int MAX_SET_SEL = 3;
    const int MAX_SEL = MAX_SET_SEL + 8;

    const int BUTTONID_SETTING0 = 0;
    const int BUTTONID_SETTING1 = 1;
    const int BUTTONID_SETTING2 = 2;
    const int BUTTONID_DELETE = 3;
    const int BUTTONID_CREATE = 4;
    const int BUTTONID_DOWNLOAD = 5;
    const int BUTTONID_EXPORT = 6;
    const int BUTTONID_SAVE = 7;
    const int BUTTONID_DEFAULT = 8;
    const int BUTTONID_BACK = 9;
    const int BUTTONID_SETTING3 = 10;

    // 要显示的内容(中英文切换)    
    public Menu_GameLevelSet gameLevelSet;
    public Menu_FileDownLoad gameDownLoad;
    public Text text_GameSet;
    //
    public GameObject gameName_Prefab;
    public Transform gameName_Layer;

    //
    public Menu_Button[] button;
    // 设置项    
    public Menu_SetOne[] setOne_GamePath;
    public Text text_DescriptTitle;
    public Text text_GameDescript;
    //
    static int selectId;
    static bool selectSta;
    string gameName;
    bool firstShow;

    List<Menu_Button> list_FileName = new List<Menu_Button>();
    float gameLayerLimitUp;
    int gameSelectId;

    GameSetting gameSetting;
    en_OptionType optionType;

    // 传入的变量
    Menu menu;
    Menu_PasswordManager passwordManager;
    Menu_Tips menuTips;
    Menu_CreateTips createTips;
    //
    public void Awake0(Menu mmenu)
    {
        //
        menu = mmenu;
        passwordManager = menu.passwordManager;
        menuTips = menu.menuTips;
        createTips = menu.createTips;
        gameLevelSet = menu.menu_GameLevelSet;
        gameDownLoad = menu.menu_GameDownLoad;
        //
        SetVal_Init();
        // UpdateSetValPos();

        for (int i = 0; i < setOne_GamePath.Length; i++)
        {

            setOne_GamePath[i].Init(i, OnClick_SetOne);
        }
        for (int i = 0; i < button.Length; i++)
        {
            button[i].Init(i, OnClick_Button);
        }
    }

    void Update()
    {
        if (gameName_Layer.transform.localPosition.y < 0)
        {
            gameName_Layer.transform.localPosition = new Vector3(0, 0);
        }
        else if (gameName_Layer.transform.localPosition.y > gameLayerLimitUp)
        {
            gameName_Layer.transform.localPosition = new Vector3(0, gameLayerLimitUp);
        }
        if (firstShow == false)
        {
            firstShow = true;
            for (int i = 0; i < setOne_GamePath.Length; i++)
            {
                setOne_GamePath[i].UpdateValue(Set.gameName[i]);
            }
            UpdateGameSetting();
        }
        //for (int i = 0; i < setOne_GamePath.Length; i++) {
        //    if (setOne_GamePath[i].IsChanged ()) {
        //        UpdateGameSetting ();
        //    }
        //}
        if (Menu.statue != en_MenuStatue.MenuSta_GameSelect)
            return;
        if (gameLevelSet.gameObject.activeSelf)
            return;
        if (gameDownLoad.gameObject.activeSelf)
            return;
        if (optionType == en_OptionType.GameDownload)
        {
            optionType = en_OptionType.None;
            UpdataGameNameList();
        }
        //if (optionType == en_OptionType.Setting) {
        //    optionType = en_OptionType.None;
        //    UpdateGameSetting ();
        //}

        if (optionType != en_OptionType.None)
        {
            optionType = en_OptionType.None;
        }


        /*	if (Key.MENU_UpPressed ()) {
                if (selectSta == false) {
                    if (postIndex >= MAX_SET_SEL) {
                        postIndex = MAX_SET_SEL - 1;
                    } else {
                        postIndex = (postIndex + MAX_SEL - 1) % MAX_SEL;
                        UpdateSetValPos();
                    }
                    UpdateCursor ();
                }
            }
            if (Key.MENU_DownPressed ()) {
                if (selectSta == false) {
                    postIndex = (postIndex + 1) % MAX_SEL;
                    UpdateSetValPos();
                    UpdateCursor ();
                }
            } */
        if (Key.MENU_LeftPressed() || Key.KEYFJ_Menu_LeftPressed())
        {
            if (selectSta == true)
            {
                if (selectId < MAX_SET_SEL)
                {
                    GameSetVal_Left();
                }
            }
            else
            {
                selectId = (selectId + MAX_SEL - 1) % MAX_SEL;
                UpdateSelectId();
                //	if (postIndex > MAX_SET_SEL) {
                //		postIndex--;
                //		UpdateSetValPos();
                //		UpdateCursor ();
                //	}
            }
        }
        if (Key.MENU_RightPressed() || Key.KEYFJ_Menu_RightPressed())
        {
            if (selectSta == true)
            {
                if (selectId < MAX_SET_SEL)
                {
                    GameSetVal_Right();
                }
            }
            else
            {
                selectId = (selectId + 1) % MAX_SEL;
                UpdateSelectId();
                //	if (postIndex >= MAX_SET_SEL) {
                //		postIndex = (postIndex + 1) % MAX_SEL;
                //		UpdateSetValPos();
                //		UpdateCursor ();
                //	}
            }
        }
        if (Key.MENU_OkPressed() || Key.KEYFJ_Menu_OkPressed())
        {
            if (selectId < MAX_SET_SEL)
            {
                //设置参数
                if (selectSta == true)
                {
                    UpdataCursor_Select(false);
                }
                else
                {
                    UpdataCursor_Select(true);
                }
            }
            //三个按键
            else if (selectId == MAX_SET_SEL + 0)
            {
                //保存并退出
                OnButton_SetSave_Pressed();
            }
            else if (selectId == MAX_SET_SEL + 1)
            {
                //默认值
                OnButton_SetDefault_Pressed();
            }
            else if (selectId == MAX_SET_SEL + 2)
            {
                // 不保存退出
                OnButton_SetBack_Pressed();
            }
        }
    }

    // 更新选中标志
    void UpdataCursor_Select(bool sta)
    {
        selectSta = sta;

        for (int i = 0; i < button.Length; i++)
        {
            if (i == selectId)
            {
                //selectFlag
            }
            else
            {

            }
        }
    }
    // 更新光标坐标和大小
    void UpdateSelectId()
    {
        //设置项
        for (int i = 0; i < setOne_GamePath.Length; i++)
        {
            if (selectId == i)
            {
                setOne_GamePath[i].SetSelect(true);
            }
            else
            {
                setOne_GamePath[i].SetSelect(false);
            }
        }
        //三个按键
        for (int i = 0; i < button.Length; i++)
        {
            if (i + MAX_SET_SEL == selectId)
            {
                button[i].SetSelect(true);
            }
            else
            {
                button[i].SetSelect(false);
            }
        }
    }
    //


    // 1.进入初始化 : 放到中英文切换前
    public void GameStart()
    {
        UpdateLanguage();
        UpdataGameNameList();
        UpdateGameSetting();

        firstShow = false;
        optionType = en_OptionType.None;
        selectId = MAX_SEL - 1;    //返回        
        UpdataCursor_Select(false);
        UpdateSelectId();
    }
    // 2.更新显示中英文
    public void UpdateLanguage()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            //中文
            //通用
            text_GameSet.text = "游 戏 选 择";
            text_DescriptTitle.text = "备 注";
            button[BUTTONID_SETTING0].GetComponentInChildren<Text>().text = "调 整";
            button[BUTTONID_SETTING1].GetComponentInChildren<Text>().text = "调 整";
            button[BUTTONID_SETTING2].GetComponentInChildren<Text>().text = "调 整";
            button[BUTTONID_DELETE].GetComponentInChildren<Text>().text = "删 除";
            button[BUTTONID_CREATE].GetComponentInChildren<Text>().text = "创 建";
            button[BUTTONID_DOWNLOAD].GetComponentInChildren<Text>().text = "下 载";
            button[BUTTONID_EXPORT].GetComponentInChildren<Text>().text = "导 出";
            button[BUTTONID_SAVE].GetComponentInChildren<Text>().text = "保 存 并 退 出";
            button[BUTTONID_DEFAULT].GetComponentInChildren<Text>().text = "恢 复 默 认 值";
            button[BUTTONID_BACK].GetComponentInChildren<Text>().text = "不 保 存 退 出";
            button[BUTTONID_SETTING3].GetComponentInChildren<Text>().text = "调 整";
        }
        else
        {
            //英文
            //通用
            text_GameSet.text = "Game Select";
            text_DescriptTitle.text = "Note";
            button[BUTTONID_SETTING0].GetComponentInChildren<Text>().text = "Setting";
            button[BUTTONID_SETTING1].GetComponentInChildren<Text>().text = "Setting";
            button[BUTTONID_SETTING2].GetComponentInChildren<Text>().text = "Setting";
            button[BUTTONID_DELETE].GetComponentInChildren<Text>().text = "Delete";
            button[BUTTONID_CREATE].GetComponentInChildren<Text>().text = "Create";
            button[BUTTONID_DOWNLOAD].GetComponentInChildren<Text>().text = "Download";
            button[BUTTONID_EXPORT].GetComponentInChildren<Text>().text = "Export";
            button[BUTTONID_SAVE].GetComponentInChildren<Text>().text = "Save Back";
            button[BUTTONID_DEFAULT].GetComponentInChildren<Text>().text = "Default";
            button[BUTTONID_BACK].GetComponentInChildren<Text>().text = "Not Save Back";
            button[BUTTONID_SETTING3].GetComponentInChildren<Text>().text = "Adjust";
        }
        SetVal_UpdataLanguage();
    }

    // 右键设置++
    void GameSetVal_Right()
    {
        if (selectId >= 0 && selectId < setOne_GamePath.Length)
        {
            setOne_GamePath[selectId].ValueAdd();
        }
    }

    // 左键设置--
    void GameSetVal_Left()
    {
        if (selectId >= 0 && selectId < setOne_GamePath.Length)
        {
            setOne_GamePath[selectId].ValueDec();
        }
    }

    //
    void SetVal_Init()
    {
        //** 1.设置项初始化(代替在界面里直接真写) : 初始化全部可选择列表
        //setValue_GameId.Init
    }
    // 排序， 不同模式

    // 2.更新显示中英文
    void SetVal_UpdataLanguage()
    {
        for (int i = 0; i < setOne_GamePath.Length; i++)
        {
            if (i == 0 || (Set.setVal.GameChoose != (int)en_GameId.YueDongGeZi))
            {
                setOne_GamePath[i].gameObject.SetActive(true);
                button[BUTTONID_SETTING0 + i].gameObject.SetActive(true);
            }
            else
            {
                setOne_GamePath[i].gameObject.SetActive(false);
                button[BUTTONID_SETTING0 + i].gameObject.SetActive(false);
            }
        }

        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            // 要显示的设置项 名称
            switch ((en_GameId)Set.setVal.GameChoose)
            {
                case en_GameId.YueDongGeZi:
                    setOne_GamePath[0].SetName("游戏选择(传统模式)");
                    break;
                case en_GameId.LeiSheWu:
                    setOne_GamePath[(int)en_PlayerMode.Free].SetName("游戏选择(休闲模式)");
                    setOne_GamePath[(int)en_PlayerMode.PassLevel].SetName("游戏选择(闯关模式)");
                    setOne_GamePath[(int)en_PlayerMode.Challenge].SetName("游戏选择(挑战模式)");
                    break;
                case en_GameId.PanYan:
                    setOne_GamePath[(int)en_PlayerMode.Free].SetName("游戏选择(休闲模式)");
                    setOne_GamePath[(int)en_PlayerMode.PassLevel].SetName("游戏选择(闯关模式)");
                    setOne_GamePath[(int)en_PlayerMode.Challenge].SetName("游戏选择(挑战模式)");
                    setOne_GamePath[(int)en_PlayerMode.XiaoHai].SetName("游戏选择(小孩模式)");
                    break;

            }

        }
        else
        {
            // 英文
            switch ((en_GameId)Set.setVal.GameChoose)
            {
                case en_GameId.YueDongGeZi:
                    setOne_GamePath[0].SetName("Game Select(Traditional mode)");
                    break;
                case en_GameId.LeiSheWu:
                    setOne_GamePath[(int)en_PlayerMode.Free].SetName("Game Select(Arcade mode)");
                    setOne_GamePath[(int)en_PlayerMode.PassLevel].SetName("Game Select(Pass game)");
                    setOne_GamePath[(int)en_PlayerMode.Challenge].SetName("Game Select(Challenge)");
                    break;
                case en_GameId.PanYan:
                    setOne_GamePath[(int)en_PlayerMode.Free].SetName("Game Select(Arcade mode)");
                    setOne_GamePath[(int)en_PlayerMode.PassLevel].SetName("Game Select(Pass game)");
                    setOne_GamePath[(int)en_PlayerMode.Challenge].SetName("Game Select(Challenge)");
                    break;
            }
        }
    }
    //
    void Update_GameSelectId()
    {
        for (int i = 0; i < list_FileName.Count; i++)
        {
            if (i == gameSelectId)
            {
                list_FileName[i].SetSelect(true);
            }
            else
            {
                list_FileName[i].SetSelect(false);
            }
        }
    }
    //更新游戏名字列表
    void Update_FileNameList()
    {
        string[] files;
        string drectory = GameSetting.GetDirectory();
        if (Directory.Exists(drectory))
        {
            files = Directory.GetFiles(drectory);
        }
        else
        {
            files = new string[0];
        }
        //
        for (; list_FileName.Count > 0;)
        {
            if (list_FileName[0] != null)
            {
                Destroy(list_FileName[0].gameObject);
            }
            list_FileName.RemoveAt(0);
        }
        list_FileName.Clear();
        //
        for (int i = 0; i < files.Length; i++)
        {
            Menu_Button animName = Instantiate(gameName_Prefab, gameName_Layer).GetComponent<Menu_Button>();
            animName.Init(i, OnClick_GameName);
            animName.SetName(Path.GetFileName(files[i]));
            list_FileName.Add(animName);
        }
        //		
        gameLayerLimitUp = list_FileName.Count * 70 - 500 + 50;
        if (gameLayerLimitUp < 0)
        {
            gameLayerLimitUp = 0;
        }
        //
        gameSelectId = -1;
        Update_GameSelectId();
    }
    // 更新设置内容 :
    int UpdataGameNameList()
    {
        Update_FileNameList();
        //
        string[] filename = GameSetting.GetAllFiles();
        for (int i = 0; i < setOne_GamePath.Length; i++)
        {
            setOne_GamePath[i].SetValueInit(filename);
            //Debug.Log ("CurrPath: " + Set.gameName);
            setOne_GamePath[i].UpdateValue(Set.gameName[i]);
        }
        return filename.Length;
    }
    void UpdateGameSetting()
    {
        //gameName = setOne_GamePath.GetValueName ();
        if (gameSelectId >= 0 && gameSelectId < list_FileName.Count)
        {
            gameName = list_FileName[gameSelectId].text.text;
        }
        else
        {
            gameName = "";
        }
        if (gameName == "")
        {
            gameSetting = null;
            text_GameDescript.text = "";
        }
        else
        {
            gameSetting = GameSetting.LoadGameSetting(gameName);
            if (gameSetting != null)
            {
                text_GameDescript.text = gameSetting.descript;
            }
        }
    }


    // 按键操作:---------------------------------------------
    public void OnClick_GameName(int id)
    {
        if (id >= 0 && id < list_FileName.Count)
        {
            gameSelectId = id;
            Update_GameSelectId();
        }
    }
    public void OnClick_SetOne(int id)
    {
        selectId = id;
        UpdateSelectId();
    }
    public void OnClick_Button(int id)
    {
        switch (id)
        {
            case BUTTONID_SETTING0: //setting
                OnButton_Setting(0);
                break;
            case BUTTONID_SETTING1: //setting
                OnButton_Setting(1);
                break;
            case BUTTONID_SETTING2: //setting
                OnButton_Setting(2);
                break;
            case BUTTONID_SETTING3: //setting
                OnButton_Setting(3);
                break;
            case BUTTONID_DELETE: //delete
                OnClick_Delete();
                break;
            case BUTTONID_CREATE: //create
                OnClick_Create();
                break;
            case BUTTONID_DOWNLOAD:
                OnButton_GameDownLoad();
                break;
            case BUTTONID_EXPORT:
                OnButton_GameExport();
                break;
            case BUTTONID_SAVE: //save
                OnButton_SetSave_Pressed();
                break;
            case BUTTONID_DEFAULT: //default
                OnButton_SetDefault_Pressed();
                break;
            case BUTTONID_BACK: //back
                OnButton_SetBack_Pressed();
                break;
        }
        selectId = MAX_SET_SEL + id;
        UpdateSelectId();
    }
    public void OnClick_Delete()
    {
        if (gameSelectId < 0 || gameSelectId >= list_FileName.Count)
            return;
        gameName = list_FileName[gameSelectId].text.text;
        //gameName = setOne_GamePath.GetValueName ();
        if (string.IsNullOrEmpty(gameName))
            return;
        if (GameSetting.IsDefaultFile(gameName))
            return;

        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            menuTips.Init("是否删除 " + gameName + " ?", DeleteFile);
        }
        else
        {
            menuTips.Init("Dos delete " + gameName + " ?", DeleteFile);
        }
        optionType = en_OptionType.Delete;
    }
    void DeleteFile(bool isOk)
    {
        if (isOk == false)
            return;
        if (gameSelectId < 0 || gameSelectId >= list_FileName.Count)
            return;
        gameName = list_FileName[gameSelectId].text.text;
        //
        //gameName = setOne_GamePath.GetValueName ();
        GameSetting.DeleteGameSetting(gameName);
        RankList.DeleteOne(gameName);

        int count = UpdataGameNameList();
        for (int i = 0; i < setOne_GamePath.Length; i++)
        {
            int index = setOne_GamePath[i].GetValueIndex();
            if (index >= count)
                index--;
            setOne_GamePath[i].UpdateValue(index);
        }
        //
        gameSelectId = -1;
        Update_GameSelectId();
        UpdateGameSetting();
    }
    public void OnClick_Create()
    {
        createTips.GameStart(CreateFile);
    }

    public void CreateFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }
        fileName = "U-" + fileName;
        string filePath = GameSetting.GetDirectory() + fileName;
        if (File.Exists(filePath))
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("游戏 " + fileName + " 已经存在!", 3, true);
            }
            else
            {
                menuTips.Init("Game " + fileName + " already exists!", 3, true);
            }
            return;
        }
        //
        GameSetting setting = new GameSetting();
        setting.Default(Set.setVal.GameChoose, 0);
        GameSetting.SaveSetting(fileName, setting);
        //
        UpdataGameNameList();
        //for (int i = 0; i < setOne_GamePath.Length; i++) {
        //    setOne_GamePath[i].UpdateValue (fileName);
        //}
        UpdateGameSetting();
    }

    public void OnButton_Setting(int id)
    {
        UpdateGameSetting();
        if (id < 0 || id >= setOne_GamePath.Length)
            return;
        string gamename = setOne_GamePath[id].GetValueName();
        if (string.IsNullOrEmpty(gamename))
            return;
        GameSetting gameSettingOne = GameSetting.LoadGameSetting(gamename);
        if (gameSettingOne == null)
            return;
        //menu.ChangeStatue (en_MenuStatue.MenuSta_AnimSet);
        //menu.menu_AnimSet.gameObject.SetActive (true);
        //menu.menu_AnimSet.GameStart (gameName, gameSetting);
        gameLevelSet.gameObject.SetActive(true);
        gameLevelSet.GameStart(id, gamename, gameSettingOne);
        optionType = en_OptionType.Setting;
    }

    public void OnButton_GameDownLoad()
    {
        gameDownLoad.GameStart(en_LoadType.Download, en_FileType.GameSetting);
        optionType = en_OptionType.GameDownload;
    }
    public void OnButton_GameExport()
    {
        gameDownLoad.GameStart(en_LoadType.Export, en_FileType.GameSetting);
    }


    void SaveSetting(bool isOk)
    {
        if (isOk)
        {
            for (int i = 0; i < setOne_GamePath.Length; i++)
            {
                Set.gameName[i] = setOne_GamePath[i].GetValueName();
#if UNITY_EDITOR
                Debug.Log("SavePath: " + Set.gameName);
#endif
            }
            Set.SaveGameName();
            menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
            //
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("保存成功", 3, false);
            }
            else
            {
                menuTips.Init("Saved successfully", 3, false);
            }
        }
    }
    public void OnButton_SetSave_Pressed()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            menuTips.Init("是否保存并退出?", SaveSetting);
        }
        else
        {
            menuTips.Init("Save and exit?", SaveSetting);
        }
    }

    void DefaultSetting(bool isOk)
    {
        if (isOk)
        {
            Set.DefaultGameName();
            GameStart();

            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("恢复默认值成功", 3, false);
            }
            else
            {
                menuTips.Init("Restore default successfully", 3, false);
            }
        }
    }
    public void OnButton_SetDefault_Pressed()
    {
        //默认值
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            menuTips.Init("是否恢复默认值?", DefaultSetting);
        }
        else
        {
            menuTips.Init("Restore defaults?", DefaultSetting);
        }
    }

    //void Back(bool isOk) {
    //    if (isOk) {
    //        menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
    //    }
    //}
    public void OnButton_SetBack_Pressed()
    {
        // 不保存退出
        //if (Set.setVal.Language == (int)en_Language.Chinese) {
        //    menuTips.Init("是否不保存退出?", Back);
        //} else {
        //    menuTips.Init("Exit without saving?", Back);
        //}
        menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
    }



}
