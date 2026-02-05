using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Menu_GameLevelSet : MonoBehaviour
{

    public Text text_Title;
    public Text text_Level;
    public Text text_Tips;
    public Text text_AnimDescript;

    public Menu_SetOne setOne_MaxLevel;
    public Menu_SetOne setOne_PlayerMode;
    public AccValue accValue_GameId;
    public Text text_DescriptTitle;
    public InputField intputField_Descript;
    public GameObject gameLevelSetOne_Prefab;
    public GameObject picOne_Prefab;
    public Transform gameLevelSet_Layer;
    public Transform presetPic_Layer;

    public Button button_Preview;
    public Button button_Save;
    public Button button_Default;
    public Button button_Back;

    const float GAMELEVE_ONE_HEIGHT = 675;

    string gameName;
    GameSetting gameSetting;
    PresetPic presetPic;
    AnimSet animSet;
    Menu_GameLevelSetOne[] gameLevelSetOne = new Menu_GameLevelSetOne[Main.MAX_LEVEL];
    GameLevelSetting levelSettingPreview = new GameLevelSetting();
    GameLeiSheLedControl ledControl = new GameLeiSheLedControl();
    string[] picNames;
    string[] animNames;

    int selectId;
    string currLevelPicName;
    string currLevelAnimName;
    float gameLevelSetLayerLimitUp;

    public en_Game00_Sta statue;
    float runTime;
    int gameId = 20;
    public int setIndex;
    public int setCount;
    bool cmdRetSucess;
    int playerMode;
    int gameLevel;
    public int maxLevel;
    int gameChoose;

    //en_PlayerMode playerMode = en_PlayerMode.One;

    public static Menu_GameLevelSet instance;
    void Awake()
    {
        instance = this;
    }

    void OnDisable()
    {
        ChangeStatue(en_Game00_Sta.None);
    }

    Menu menu;
    public void Awake0(Menu mmenu)
    {
        menu = mmenu;

        button_Preview.onClick.AddListener(OnClick_Preview);
        button_Save.onClick.AddListener(OnClick_Save);
        button_Default.onClick.AddListener(OnClick_Default);
        button_Back.onClick.AddListener(OnClick_Back);
        //
        gameChoose = Mathf.Clamp(Set.setVal.GameChoose, 0, GameSetting.tab_MaxLevel.Length - 1);
        setOne_MaxLevel.SetValueInit(GameSetting.tab_MaxLevel[gameChoose]);
        setOne_PlayerMode.SetValueInit(Set.SET_c_PlayerMode);
        //

        //for (int i = 0; i < gameLevelSetting.Length; i++) {
        //	gameLevelSetting[i] = new GameLevelSetting ();
        //}
        for (int i = 0; i < gameLevelSetOne.Length; i++)
        {
            gameLevelSetOne[i] = Instantiate(gameLevelSetOne_Prefab, gameLevelSet_Layer).GetComponent<Menu_GameLevelSetOne>();
            gameLevelSetOne[i].Awake0(i, OnClick_GameLevelSettingOne);
        }
    }

    // Use this for initialization	
    public void GameStart(int playermode, string gamename, GameSetting setting)
    {
        //
        playerMode = playermode;
        Update_Languae();
        if (gameChoose != Set.setVal.GameChoose)
        {
            gameChoose = Mathf.Clamp(Set.setVal.GameChoose, 0, GameSetting.tab_MaxLevel.Length - 1);
            setOne_MaxLevel.SetValueInit(GameSetting.tab_MaxLevel[gameChoose]);
        }
        //
        gameName = gamename;
        accValue_GameId.SetValueName(gameName);
        gameSetting = setting;
        //
        //picNames = GetFileNames(PresetPic.GetDirectory());
        //animNames = GetFileNames(AnimSet.GetDirectory());
        picNames = PresetPic.GetAllFiles();
        animNames = AnimSet.GetAllFiles();
        //settingValue:
        setOne_MaxLevel.UpdateValue(gameSetting.maxLevel);
        setOne_PlayerMode.UpdateValue((int)playerMode);
        setOne_PlayerMode.gameObject.SetActive(false);
        intputField_Descript.text = gameSetting.descript;
        //        
        PresetPic_Init();
        // 保存当前值

        Update_GameLevelSettingList();

        gameLevel = 0;
        selectId = -1;
        Update_SelectId();

        ChangeStatue(en_Game00_Sta.None);
    }


    // Update is called once per frame
    void Update()
    {
        switch (statue)
        {
            case en_Game00_Sta.Idle:
                break;

            case en_Game00_Sta.Play:
                ledControl.Run();
                break;
        }

        if (gameLevelSet_Layer.transform.localPosition.y < 0)
        {
            gameLevelSet_Layer.transform.localPosition = new Vector3(0, 0);
        }
        else if (gameLevelSet_Layer.transform.localPosition.y > gameLevelSetLayerLimitUp)
        {
            gameLevelSet_Layer.transform.localPosition = new Vector3(0, gameLevelSetLayerLimitUp);
        }

        //
        if (selectId >= 0 && selectId < gameLevelSetOne.Length)
        {
            if (currLevelPicName != gameLevelSetOne[selectId].GetCurrPicName())
            {
                Update_CurrPresetPic();
            }
            if (currLevelAnimName != gameLevelSetOne[selectId].GetCurrAnimName())
            {
                Update_CurrAnimDescript();
            }
        }

        if (maxLevel != setOne_MaxLevel.GetValue())
        {
            Update_GameLevelSettingList();
        }
    }

    void ChangeStatue(en_Game00_Sta sta)
    {
        statue = sta;
        runTime = 0;
        setIndex = 0;
        setCount = 0;
        cmdRetSucess = false;

        //       CmdIO_YDGZ.CMD0_SendCmd_GameStatue(gameId, gameLevel, (int)statue);

        switch (statue)
        {
            case en_Game00_Sta.None:
            case en_Game00_Sta.Idle:
                //CmdIO_YDGZ.CMD0_SendCmd_GameStatue (0, 0, 0);
                break;
            case en_Game00_Sta.Play:
                if (++gameId >= 127)
                {
                    gameId = 1;
                }
                ledControl.LedInit(levelSettingPreview);
                ledControl.RunStart();
                break;
        }
    }
    bool TimePassed(float passTime)
    {
        if (runTime > 0)
        {
            runTime -= Time.deltaTime;
        }
        else
        {
            runTime = passTime;
            return true;
        }
        return false;
    }

    void SendPresetPoint()
    {
        PresetPic.SendPresetPoint(gameId, levelSettingPreview.picSetting, ref setIndex);
    }

    //readonly enPointSta[] tab_PointSta = { enPointSta.Rest, enPointSta.Die, enPointSta.Target };
    //void SendPresetPoint() {
    //    if (setIndex >= tab_PointSta.Length)
    //        return;
    //    byte[] buf = new byte[128];
    //    byte pointType = (byte)tab_PointSta[setIndex];
    //    int bufLen = Mathf.Min(Set.setVal.Width * Set.setVal.Height, levelSettingPreview.picSetting.dataBuff.Length);
    //    int pointLen = 0;
    //    int bitn = 0;
    //    int len = 0;
    //    for (int i = 0; i < bufLen; i++) {
    //        if (bitn == 0) {
    //            buf[len] = 0;
    //        }
    //        if (levelSettingPreview.picSetting.dataBuff[i] == pointType) {
    //            buf[len] |= (byte)(1 << bitn);
    //            pointLen++;
    //        }
    //        if (++bitn >= 7) {
    //            bitn = 0;
    //            len++;
    //        }
    //    }
    //    if (bitn > 0) {
    //        len++;
    //    }
    //    if (pointLen == 0) {
    //        setIndex++;
    //        return;
    //    }
    //    //
    //    if (CmdIO_YDGZ.CMD0_SendCmd_PresetPoint(gameId, setIndex, pointType, buf, (byte)len) == false) {
    //        setIndex++;
    //    }
    //}


    void Update_Languae()
    {
        setOne_PlayerMode.gameObject.SetActive(false);
        text_Tips.gameObject.SetActive(false);
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            text_Title.text = "关 卡 设 置";
            //text_Tips.text = "(速度值越小移动越快)";
            text_DescriptTitle.text = "备 注:";
            accValue_GameId.SetName("游 戏 选 择");
            button_Preview.GetComponentInChildren<Text>().text = "预 览";
            button_Save.GetComponentInChildren<Text>().text = "保 存 设 置";
            button_Default.GetComponentInChildren<Text>().text = "默 认 值";
            button_Back.GetComponentInChildren<Text>().text = "返 回";

            //
            setOne_MaxLevel.SetName("最大关数");
            //
            setOne_PlayerMode.SetName("玩家模式");
            //setOne_PlayerMode.SetValueName ((int)en_PlayerMode.One, "闯关模式");
            //setOne_PlayerMode.SetValueName ((int)en_PlayerMode.Two, "对战模式");
        }
        else
        {
            //
            text_Title.text = "Level setting";
            //text_Tips.text = "(The smaller the speed value, the faster the movement)";
            text_DescriptTitle.text = "Note:";
            accValue_GameId.SetName("Game");
            button_Preview.GetComponentInChildren<Text>().text = "Preview";
            button_Save.GetComponentInChildren<Text>().text = "Save";
            button_Default.GetComponentInChildren<Text>().text = "Default";
            button_Back.GetComponentInChildren<Text>().text = "Return";

            //
            setOne_MaxLevel.SetName("Maximum level");
            //
            setOne_PlayerMode.SetName("Play mode");
            //setOne_PlayerMode.SetValueName ((int)en_PlayerMode.One, "Pass game");
            //setOne_PlayerMode.SetValueName ((int)en_PlayerMode.Two, "Battle game");
        }
    }

    void Update_SelectId()
    {
        for (int i = 0; i < gameLevelSetOne.Length; i++)
        {
            if (i == selectId)
            {
                gameLevelSetOne[i].SetSelectSta(true);
            }
            else
            {
                gameLevelSetOne[i].SetSelectSta(false);
            }
        }
        if (selectId >= 0 && selectId < gameLevelSetOne.Length)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                text_Level.text = "第 " + (selectId + 1) + " 关";
            }
            else
            {
                text_Level.text = "Level " + (selectId + 1);
            }
        }
        else
        {
            text_Level.text = "";
        }
        Update_CurrLevelInfo();
    }

    void Update_GameLevelSettingList()
    {
        maxLevel = setOne_MaxLevel.GetValue();
        int levelCount = Mathf.Min(maxLevel, gameSetting.gameLevelSetting.Length);

        for (int i = 0; i < gameLevelSetOne.Length; i++)
        {
            if (i < levelCount)
            {
                gameLevelSetOne[i].gameObject.SetActive(true);
                gameLevelSetOne[i].GameStart(playerMode, gameSetting.gameLevelSetting[i], picNames, animNames);
            }
            else
            {
                gameLevelSetOne[i].gameObject.SetActive(false);
            }
        }

        gameLevelSetLayerLimitUp = levelCount * GAMELEVE_ONE_HEIGHT - 700 + 50;
        if (gameLevelSetLayerLimitUp < 0)
        {
            gameLevelSetLayerLimitUp = 0;
        }
    }
    void Update_CurrLevelInfo()
    {
        Update_CurrPresetPic();
        Update_CurrAnimDescript();
    }

    // 更新当前选中关卡的图案
    void Update_CurrPresetPic()
    {
        if (selectId >= 0 && selectId < gameLevelSetOne.Length && selectId < setOne_MaxLevel.GetValue())
        {
            currLevelPicName = gameLevelSetOne[selectId].GetCurrPicName();
#if UNITY_EDITOR
            //  Debug.LogError("picName: " + currLevelPicName);
#endif
            presetPic = PresetPic.LoadSetting(currLevelPicName);
            Update_PresetPic(presetPic.dataBuff);
        }
        else
        {
            Update_PresetPic(null);
        }
    }
    // 更新当前选中关卡的动画备注
    void Update_CurrAnimDescript()
    {
        if (selectId >= 0 && selectId < gameLevelSetOne.Length && selectId < setOne_MaxLevel.GetValue())
        {
            currLevelAnimName = gameLevelSetOne[selectId].GetCurrAnimName();
#if UNITY_EDITOR
            Debug.Log("animName: " + currLevelAnimName);
#endif
            animSet = AnimSet.LoadSetting(currLevelAnimName);
            if (animSet == null)
            {
                if (Set.setVal.Language == (int)en_Language.Chinese)
                {
                    text_AnimDescript.text = "未选择";
                }
                else
                {
                    text_AnimDescript.text = "Not selected";
                }
            }
            else
            {
                text_AnimDescript.text = animSet.descript;
            }
        }
        else
        {
            text_AnimDescript.text = "";
        }
    }


    List<Image> list_PresetPic = new List<Image>();
    void PresetPic_Init()
    {
        int len = Set.setVal.Width * Set.setVal.Height;
        // 去除多余的
        for (; list_PresetPic.Count > len;)
        {
            Destroy(list_PresetPic[0].gameObject);
            list_PresetPic.RemoveAt(0);
        }
        // 添加不足的
        for (int i = list_PresetPic.Count; i < len; i++)
        {
            Image image = Instantiate(picOne_Prefab, presetPic_Layer).GetComponent<Image>();
            list_PresetPic.Add(image);
        }
        int widthOne = 50;
        //if (Set.setVal.Width > 0) {
        //    widthOne = 400 / Set.setVal.Width;
        //}
        //int heightOne = 10;
        //if (Set.setVal.Height > 0) {
        //    heightOne = 500 / Set.setVal.Height;
        //}
        //if (widthOne > heightOne) {
        //    widthOne = heightOne;
        //}
        //if (widthOne > 50) {
        //    widthOne = 50;
        //}
        int offsetY = (Set.setVal.Height - 1) * widthOne;
        int width = widthOne * Set.setVal.Width;
        int height = widthOne * Set.setVal.Height;
        int x = 0;
        int y = 0;
        for (int i = 0; i < list_PresetPic.Count; i++)
        {
            //list_PresetPic[i].rectTransform.sizeDelta = new Vector2(widthOne, widthOne);
            //list_PresetPic[i].transform.localPosition = new Vector3(x * widthOne - width * 0.5f, y * widthOne + height * 0.5f - offsetY);
            list_PresetPic[i].transform.localPosition = new Vector3(x * widthOne - (width - widthOne) * 0.5f, y * widthOne + (height - widthOne) * 0.5f - offsetY);
            if (++x >= Set.setVal.Width)
            {
                x = 0;
                y++;
            }
        }
        // Scale: 600 x 500
        float scalex = 610f / width;
        float scaley = 450f / height;
        float scale = Mathf.Clamp(Mathf.Min(scalex, scaley), 0, 1);
        presetPic_Layer.transform.localScale = Vector3.one * scale;
    }

    readonly Color[] tab_PointColor = { Color.black, Color.blue, Color.red, Color.green };
    //void Update_PresetPic (byte[,] dataBuf) {
    void Update_PresetPic(byte[] dataBuf)
    {
        if (dataBuf == null)
        {

            presetPic_Layer.gameObject.SetActive(false);
            return;
        }

        presetPic_Layer.gameObject.SetActive(true);
        int id;
        int bufId;
        for (int i = 0; i < Set.setVal.Height && i < PresetPic.PIC_HEIGHT; i++)
        {
            for (int j = 0; j < Set.setVal.Width && j < PresetPic.PIC_WIDTH; j++)
            {
                id = i * Set.setVal.Width + j;
                bufId = i * PresetPic.PIC_WIDTH + j;
                if (bufId >= dataBuf.Length)
                    continue;
                if (id >= list_PresetPic.Count)
                    return;
                //                Debug.LogError(dataBuf[bufId]);
                if (dataBuf[bufId] == 0)
                {
                    list_PresetPic[id].color = Color.black;
                }
                else
                {
                    list_PresetPic[id].color = Color.blue;
                }
            }
        }

        //for (int i = 0; i < Set.setVal.Width && i < PresetPic.PIC_WIDTH; i++) {
        //    for (int j = 0; j < Set.setVal.Height && j < PresetPic.PIC_HEIGHT; j++) {
        //        id = j * Set.setVal.Width + i;
        //        bufId = j * PresetPic.PIC_WIDTH + i;
        //        if (bufId >= dataBuf.Length)
        //            continue;
        //        if (id >= list_PresetPic.Count)
        //            return;
        //        if (dataBuf[bufId] >= tab_PointColor.Length) {
        //            list_PresetPic[id].color = Color.black;
        //        } else if (id < dataBuf.Length) {
        //            list_PresetPic[id].color = tab_PointColor[dataBuf[bufId]];
        //        } else {
        //            list_PresetPic[id].color = Color.black;
        //        }
        //    }
        //}
    }
    //void Update_PresetPic(byte[] dataBuf) {
    //    if (dataBuf == null) {                
    //        presetPic_Layer.gameObject.SetActive(false);
    //        return;
    //    }
    //    presetPic_Layer.gameObject.SetActive(true);
    //    for (int i = 0; i < list_PresetPic.Count; i++) {
    //        if (i >= dataBuf.Length) {
    //            list_PresetPic[i].color = Color.black;
    //        } else if (dataBuf[i] < tab_PointColor.Length) {
    //            list_PresetPic[i].color = tab_PointColor[dataBuf[i]];
    //        } else {
    //            list_PresetPic[i].color = Color.black;
    //        }
    //    }
    //}

    public void OnClick_GameLevelSettingOne(int id)
    {
        if (selectId == id)
            return;
        selectId = id;
        Update_SelectId();
    }

    public void OnClick_Preview()
    {
        //Debug.Log("预览");
        if (statue == en_Game00_Sta.None)
        {
            if (gameSetting == null)
                return;
            if (selectId >= 0 && selectId < gameLevelSetOne.Length)
            {
                gameLevel = selectId;
                gameLevelSetOne[gameLevel].GetSetting(levelSettingPreview);

                levelSettingPreview.picSetting = PresetPic.LoadSetting(levelSettingPreview.presetPicName);
                levelSettingPreview.animSetting = AnimSet.LoadSetting(levelSettingPreview.animInfoName);
                if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu)
                {
                    if (levelSettingPreview.picSetting == null)
                        return;
                }
                if (Set.setVal.GameChoose == (int)en_GameId.YueDongGeZi)
                {
                    if (levelSettingPreview.animSetting == null)
                        return;
                }
                //for (int i = 0; i < levelSettingPreview.picSetting.dataBuff.Length; i++) {
                //    levelSettingPreview.picSetting.dataBuff[i] = gameSetting.gameLevelSetting[selectId].picSetting.dataBuff[i];
                //}
                //playerMode = (en_PlayerMode)setOne_PlayerMode.GetValue();
                ChangeStatue(en_Game00_Sta.Play);
            }
        }
        else if (statue == en_Game00_Sta.Play)
        {
            ChangeStatue(en_Game00_Sta.None);
        }
    }

    void SaveSet(bool isOk)
    {
        if (isOk == false)
            return;
        if (string.IsNullOrEmpty(gameName))
            return;
        if (GameSetting.IsDefaultFile(gameName))
            return;
        for (int i = 0; i < gameSetting.gameLevelSetting.Length && i < gameLevelSetOne.Length; i++)
        {
            gameLevelSetOne[i].GetSetting(gameSetting.gameLevelSetting[i]);
            //
            //gameLevelSetting[i].CopyTo (gameSetting.gameLevelSetting[i]);
        }
        gameSetting.maxLevel = setOne_MaxLevel.GetValue();
        gameSetting.descript = intputField_Descript.text;
        //Debug.Log ("备注：" + gameSetting.descript);
        GameSetting.SaveSetting(gameName, gameSetting);
        //
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("保存成功", 3, true);
        }
        else
        {
            Menu_Tips.instance.Init("Saved successfully", 3, true);
        }
    }
    public void OnClick_Save()
    {
        if (string.IsNullOrEmpty(gameName))
            return;
        if (GameSetting.IsDefaultFile(gameName))
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                Menu_Tips.instance.Init("默认游戏，不能修改", 3, true);
            }
            else
            {
                Menu_Tips.instance.Init("Default game, cannot be modified", 3, true);
            }
            return;
        }
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("是否保存设置", SaveSet);
        }
        else
        {
            Menu_Tips.instance.Init("Save setting?", SaveSet);
        }
    }

    void DefaultSet(bool isOk)
    {
        if (isOk == false)
            return;
        if (string.IsNullOrEmpty(gameName))
            return;
        if (GameSetting.IsDefaultFile(gameName))
            return;
        gameSetting.Default(Set.setVal.GameChoose, playerMode);
        GameSetting.SaveSetting(gameName, gameSetting);
        setOne_MaxLevel.UpdateValue(gameSetting.maxLevel);
        Update_GameLevelSettingList();
        //
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("恢复默认值成功", 3, true);
        }
        else
        {
            Menu_Tips.instance.Init("Restore Default Success", 3, true);
        }
    }
    public void OnClick_Default()
    {
        if (string.IsNullOrEmpty(gameName))
            return;
        if (GameSetting.IsDefaultFile(gameName))
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                Menu_Tips.instance.Init("默认游戏，不能修改", 3, true);
            }
            else
            {
                Menu_Tips.instance.Init("Default game, cannot be modified", 3, true);
            }
            return;
        }
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
        gameObject.SetActive(false);
        //menu.ChangeStatue (en_MenuStatue.MenuSta_SysSet);
        //menu.ChangeStatue (en_MenuStatue.MenuSta_GameSelect);
    }

}
