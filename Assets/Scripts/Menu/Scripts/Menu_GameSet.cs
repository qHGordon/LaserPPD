using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Menu_GameSet : MonoBehaviour
{
    const int MAX_SET_ONEPAGE = 12;
    int MAX_SET_SEL;// = 12;
    int MAX_SEL;// = MAX_SET_SEL + 3;
    public GameObject volumeTips_Obj;
    public Image image_VolumeValue;
    public Text[] text_VolumeTips;
    // 要显示的内容(中英文切换)    
    public Text text_GameSet;
    public Text text_TiShi;
    public Text text_DescriptTitle;
    public Text text_Descript;
    public GameObject setVal_Layer;
    public Menu_SelectFlag selectFlag;  //光标选中标志
    public Menu_Button[] button;
    public GameObject setVal_Prefab;
    // 设置项	
    //SetValue[] setVal = new SetValue[MAX_ALL_SET_SEL];
    //List<SetValue> list_SetVal = new List<SetValue>();
    Menu_SetOne[] setVal = new Menu_SetOne[MAX_ALL_SET_SEL];
    List<Menu_SetOne> list_SetVal = new List<Menu_SetOne>();
    //
    static int postIndex;
    static bool selectSta;
    float runTime_SaveText;
    bool pressedButton;
    float num_time;
    int cnt = 1;
    //public static int SET_ID_StartCoins_sta;

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

        for (int i = 0; i < button.Length; i++)
        {
            button[i].Init(i, OnClick_Button);
        }
        //
        GameObject obj;
        for (int i = 0; i < MAX_ALL_SET_SEL; i++)
        {
            obj = Instantiate(setVal_Prefab, setVal_Layer.transform);
            setVal[i] = obj.GetComponent<Menu_SetOne>();
            setVal[i].Init(i, OnClick_SetOne);
        }
        SetVal_Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Menu.statue != en_MenuStatue.MenuSta_GameSet)
            return;

        if (menuTips.gameObject.activeSelf)
            return;
        if (num_time > 0)
        {
            num_time -= Time.deltaTime;

        }
        else
        {


            num_time = 1f;
            cnt++;
            if (cnt > 3)
            {
                cnt = 1;
            }
            Framebuffer.MapTabInit();
            GameLedControl.ReadyStart(null, false);
            //GameLedControl.playerControl[0].Init(0, en_PlayerMode.One, 0);

            GameLedControl.playerControl[0].ShowReadyTime(cnt);


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
                if (postIndex < MAX_SET_SEL)
                {
                    GameSetVal_Left();
                }
            }
            else
            {
                postIndex = (postIndex + MAX_SEL - 1) % MAX_SEL;
                UpdateSetValPos();
                UpdateCursor();
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
                if (postIndex < MAX_SET_SEL)
                {
                    GameSetVal_Right();
                }
            }
            else
            {
                postIndex = (postIndex + 1) % MAX_SEL;
                UpdateSetValPos();
                UpdateCursor();
                //	if (postIndex >= MAX_SET_SEL) {
                //		postIndex = (postIndex + 1) % MAX_SEL;
                //		UpdateSetValPos();
                //		UpdateCursor ();
                //	}
            }
        }
        if (Key.MENU_OkPressed() || Key.KEYFJ_Menu_OkPressed() || pressedButton)
        {
            pressedButton = false;
            if (postIndex < MAX_SET_SEL)
            {
                //设置参数
                if (selectSta == true)
                {
                    UpdataCursor_Select(false);
                }
                else
                {
                    //if (Main.VER_ENC && Game_Enc.IsActive() == false && CanChange() == false)   //
                    if (CanChange() == false)
                        return;
                    UpdataCursor_Select(true);
                }
            }
            //三个按键
            else if (postIndex == MAX_SET_SEL + 0)
            {
                //保存并退出
                OnButton_SetSave_Pressed();
            }
            else if (postIndex == MAX_SET_SEL + 1)
            {
                //默认值
                OnButton_SetDefault_Pressed();
            }
            else if (postIndex == MAX_SET_SEL + 2)
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
        //selectFlag.gameObject.SetActive(false);
        //if (postIndex < list_SetVal.Count) {
        //    selectFlag.transform.position = list_SetVal[postIndex].image_ValueBackG.transform.position;
        //    selectFlag.Init(list_SetVal[postIndex].image_ValueBackG.rectTransform.sizeDelta.x);
        //}
    }
    // 更新光标坐标和大小
    void UpdateCursor()
    {
        //设置项
        for (int i = 0; i < list_SetVal.Count; i++)
        {
            if (i == postIndex)
            {
                list_SetVal[i].SetSelect(true);
            }
            else
            {
                list_SetVal[i].SetSelect(false);
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
        UpdateDescript();
    }
    //
    void UpdateSetValPos()
    {
        int id = postIndex;
        if (id >= MAX_SET_SEL)
            id = MAX_SET_SEL - 1;
        int page = id / MAX_SET_ONEPAGE;
        int pindex;

        for (int i = 0; i < MAX_SET_SEL; i++)
        {
            if ((i / MAX_SET_ONEPAGE) == page)
            {
                list_SetVal[i].gameObject.SetActive(true);
                //list_SetVal[i].transform.localPosition = new Vector3(0, 220 - (i % MAX_SET_ONEPAGE) * 70, 0);
                pindex = i % MAX_SET_ONEPAGE;
                list_SetVal[i].transform.localPosition = new Vector3((pindex % 3) * 560 - 560, 180 - (pindex / 3) * 100, 0);
            }
            else
            {
                list_SetVal[i].gameObject.SetActive(false);
            }
        }
    }


    // 1.进入初始化 : 放到中英文切换前
    int oldOutMode;
    public void GameStart()
    {
        volumeTips_Obj.SetActive(false);
        //
        oldOutMode = Set.setVal.OutMode;
        pressedButton = false;
        UpdateLanguage();
        UpdataGameSet();
        UpdateOrder();
        postIndex = MAX_SEL - 1;    //返回        
        UpdataCursor_Select(false);
        UpdateCursor();
    }
    // 2.更新显示中英文
    public void UpdateLanguage()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            //中文
            //通用
            text_GameSet.text = "游 戏 设 置";
            text_TiShi.text = "温馨提示:修改奖励模式后,请清零账目!";
            button[0].GetComponentInChildren<Text>().text = "保 存 并 退 出";
            button[1].GetComponentInChildren<Text>().text = "恢 复 默 认 值";
            button[2].GetComponentInChildren<Text>().text = "不 保 存 退 出";
        }
        else
        {
            //英文
            //通用
            text_GameSet.text = "Game Setting";
            text_TiShi.text = "Tips: please clear the account after modifying the Out Mode!";
            button[0].GetComponentInChildren<Text>().text = "Save Back";
            button[1].GetComponentInChildren<Text>().text = "Default";
            button[2].GetComponentInChildren<Text>().text = "Not Save Back";
        }
        SetVal_UpdataLanguage();
    }





    // 右键设置++
    void GameSetVal_Right()
    {
        if (postIndex < list_SetVal.Count)
        {
            if (CanChange() == false)
                return;
            list_SetVal[postIndex].ValueAdd();
            UpdateDescript();
            ModeCheck();
        }
    }

    // 左键设置--
    void GameSetVal_Left()
    {
        if (postIndex < list_SetVal.Count)
        {
            if (CanChange() == false)
                return;
            list_SetVal[postIndex].ValueDec();
            UpdateDescript();
            ModeCheck();
        }
    }

    bool CanChange()
    {
        //if (Main.VER_ENC) {
        //    if (Game_Enc.IsActive() == false) {
        //        if (tab_IdBuf[postIndex] == SET_ID_CardValue)
        //            return false;
        //    }
        //}
        return true;
    }

    public void OnClick_SetOne(int id)
    {
        for (int i = 0; i < list_SetVal.Count; i++)
        {
            if (setVal[id] == list_SetVal[i])
            {
                OnClick_Button(i);
                break;
            }
        }
    }
    public void OnClick_Button(int id)
    {
        postIndex = id;
        UpdateCursor();
        pressedButton = true;
    }

    // 需要修改的部分 --------------------------------------------------------------------------------------------------------------------------------------------
    // 设置项名字
    const int SET_ID_GameMode = 0;
    const int SET_ID_PlayerMode = 1;
    const int SET_ID_GunMode = 2;
    const int SET_ID_InOutMode = 3;
    const int SET_ID_LedProtocol = 4;
    //	
    const int SET_ID_OutMode = 5;
    const int SET_ID_StartCoins = 6;
    const int SET_ID_TicketBl = 7;
    const int SET_ID_GiftBl = 8;
    const int SET_ID_GameTime = 9;
    const int SET_ID_ScoreTtl = 10;
    //const int SET_ID_MonsterNum_1 = 8;
    //const int SET_ID_MonsterNum_2 = 9;
    //const int SET_ID_MonsterNum_3 = 10;
    //
    const int SET_ID_TicketsOneCoin = 11;
    const int SET_ID_EditerOneCoin = 12;
    const int SET_ID_MinTickets = 13;
    const int SET_ID_OutDcTime = 14;
    const int SET_ID_KeyDelay = 15;
    const int SET_ID_PlayerOrder = 16;
    //
    const int SET_ID_DeskMusic = 17;
    const int SET_ID_MainSoundVolume = 18;
    const int SET_ID_AnZhuang = 19;
    const int SET_ID_SafePlace = 20;
    const int SET_ID_GameChoose = 21;
    const int SET_ID_LeiShe_LMD = 22;
    const int SET_ID_IsContinue = 23;
    const int SET_ID_TimeMode = 24;
    const int SET_ID_ReadyTime = 25;
    const int SET_ID_ShowIdle =26;

    const int MAX_ALL_SET_SEL = 27;


    // 娱乐模式ID列表
    int[] tab_IdBuf_OutNone = {
        //SET_ID_PlayerMode,
        
        //SET_ID_GunMode,
        SET_ID_OutMode,
        //SET_ID_LedProtocol,
      //  SET_ID_StartCoins,
        SET_ID_AnZhuang,
      //  SET_ID_SafePlace,
        //SET_ID_TicketBl,
        //SET_ID_GiftBl,
        SET_ID_GameTime,
        //SET_ID_OutDcTime,
        SET_ID_KeyDelay,
        //SET_ID_PlayerOrder,
        SET_ID_GameMode,
      SET_ID_IsContinue,
      SET_ID_TimeMode,
        SET_ID_MainSoundVolume,
        SET_ID_ShowIdle,
     //  SET_ID_ReadyTime,
    //    SET_ID_GameChoose, 

  #if UNITY_EDITOR 
           
    #endif
  
        SET_ID_DeskMusic
    };
    // 娱乐模式ID列表
    int[] tab_IdBuf_LeiShe  = {
        //SET_ID_PlayerMode,
        
        //SET_ID_GunMode,
        SET_ID_OutMode,
        //SET_ID_LedProtocol,
     //   SET_ID_StartCoins,
        SET_ID_AnZhuang,
      //  SET_ID_SafePlace,
        //SET_ID_TicketBl,
        //SET_ID_GiftBl,
        SET_ID_GameTime,
        //SET_ID_OutDcTime,
        SET_ID_KeyDelay,
        SET_ID_TimeMode,
        //SET_ID_PlayerOrder,
        SET_ID_GameMode,
      
        SET_ID_MainSoundVolume,
          SET_ID_LeiShe_LMD,
                // SET_ID_GameChoose,  

  #if UNITY_EDITOR 
    #endif
  
        SET_ID_DeskMusic
    };
    int[] tab_IdBuf_OutTicket = {
        //SET_ID_PlayerMode,
        
        //SET_ID_InOutMode,
        //SET_ID_GunMode,
        SET_ID_OutMode,
      //  SET_ID_LedProtocol,
        SET_ID_StartCoins,
        SET_ID_TicketBl,
        //SET_ID_GiftBl,
        //SET_ID_GameTime,
        //SET_ID_OutDcTime,
        SET_ID_KeyDelay,
        SET_ID_PlayerOrder,
        SET_ID_GameMode,
        // SET_ID_GameChoose,
        SET_ID_DeskMusic
    };
    //
    void SetVal_Init()
    {
        //** 1.设置项初始化(代替在界面里直接真写) : 初始化全部可选择列表
        //setVal[SET_ID_InOutMode].SetValueInit(Set.SET_c_InOutMode);
        setVal[SET_ID_GameMode].SetValueInit(Set.SET_c_GameMode);
        setVal[SET_ID_IsContinue].SetValueInit(Set.SET_c_isContinue);
        setVal[SET_ID_TimeMode].SetValueInit(Set.SET_c_TimeMode);
        setVal[SET_ID_ReadyTime].SetValueInit(Set.SET_c_ReadyTime);
        setVal[SET_ID_ShowIdle].SetValueInit(Set.SET_c_ShowIdle);
        setVal[SET_ID_GameChoose].SetValueInit(Set.SET_c_GameChoose);
        setVal[SET_ID_InOutMode].SetValueInit(Set.SET_c_InOutMode);
        setVal[SET_ID_LedProtocol].SetValueInit(Set.SET_c_LedProtocol);
        setVal[SET_ID_GunMode].SetValueInit(Set.SET_c_GunMode);
        setVal[SET_ID_PlayerMode].SetValueInit(Set.SET_c_PlayerMode);
        //
        setVal[SET_ID_OutMode].SetValueInit(Set.SET_c_OutMode);
        setVal[SET_ID_TicketBl].SetValueInit(Set.SET_c_TicketBl);
        setVal[SET_ID_GiftBl].SetValueInit(Set.SET_c_GiftBl);
        setVal[SET_ID_StartCoins].SetValueInit(Set.SET_c_StartCoins);
        setVal[SET_ID_GameTime].SetValueInit(Set.SET_c_GameTime);
        //setVal[SET_ID_MonsterNum_1].SetValueInit(Set.SET_c_MonsterNum_1);
        //setVal[SET_ID_MonsterNum_2].SetValueInit(Set.SET_c_MonsterNum_2);
        //setVal[SET_ID_MonsterNum_3].SetValueInit(Set.SET_c_MonsterNum_3);
        setVal[SET_ID_ScoreTtl].SetValueInit(Set.SET_c_ScoreTtl);
        setVal[SET_ID_DeskMusic].SetValueInit(Set.SET_c_DeskMusic);
        setVal[SET_ID_LeiShe_LMD].SetValueInit(Set.SET_c_LeiShe_LMD);
        setVal[SET_ID_MainSoundVolume].SetValueInit(Set.SET_c_MainSoundVolume);
        setVal[SET_ID_AnZhuang].SetValueInit(Set.SET_c_AnZhuang);
        setVal[SET_ID_SafePlace].SetValueInit(Set.SET_c_SaftPlace);
        //
        setVal[SET_ID_TicketsOneCoin].SetValueInit(Set.SET_c_TicketsOneCoin);
        setVal[SET_ID_EditerOneCoin].SetValueInit(Set.SET_c_EditerOneCoin);
        setVal[SET_ID_MinTickets].SetValueInit(Set.SET_c_MinTickets);
        setVal[SET_ID_OutDcTime].SetValueInit(Set.SET_c_OutDcTime);
        setVal[SET_ID_KeyDelay].SetValueInit(Set.SET_c_KeyDelay);
        setVal[SET_ID_PlayerOrder].SetValueInit(Set.SET_c_PlayerOrder);
    }
    // 模式检测，当模式发生变化时，设置项目排列不同
    void ModeCheck()
    {
        if (list_SetVal[postIndex] == setVal[SET_ID_PlayerMode])
        {
            UpdateOrder();
        }
    }
    // 排序， 不同模式
    void UpdateOrder()
    {
        int[] tab_IdBuf;

        if (setVal[SET_ID_GameChoose].GetValue() ==1)
        {
            // 娱乐模式
            tab_IdBuf = tab_IdBuf_LeiShe;
        }
        else
        {
            // 中性模式 
            tab_IdBuf = tab_IdBuf_OutNone;
        }

        MAX_SET_SEL = tab_IdBuf.Length;
        MAX_SEL = MAX_SET_SEL + 3;
        // 当前要操作的选项
        list_SetVal.Clear();
        for (int i = 0; i < tab_IdBuf.Length; i++)
        {
            list_SetVal.Add(setVal[tab_IdBuf[i]]);
        }
        // 关闭所有选项
        for (int i = 0; i < setVal.Length; i++)
        {
            setVal[i].gameObject.SetActive(false);
        }
        // 重新排列
        UpdateSetValPos();
        //
        for (int i = 0; i < button.Length; i++)
        {
            button[i].Id = list_SetVal.Count + i;
        }
    }
    // 2.更新显示中英文
    void SetVal_UpdataLanguage()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            // 要显示的设置项 名称
            setVal[SET_ID_GameMode].SetName("游戏模式");
            setVal[SET_ID_IsContinue].SetName("是否自动续玩");
            setVal[SET_ID_TimeMode].SetName("计时模式");
            setVal[SET_ID_ReadyTime].SetName("准备时间");
            setVal[SET_ID_GameChoose].SetName("游戏选择");
            setVal[SET_ID_ShowIdle].SetName("待机花样开关");

            setVal[SET_ID_InOutMode].SetName("投退模式");
            setVal[SET_ID_LedProtocol].SetName("地砖灯类型");
            setVal[SET_ID_GunMode].SetName("枪模式");
            setVal[SET_ID_PlayerMode].SetName("玩家模式");
            // 娱乐
            setVal[SET_ID_OutMode].SetName("连线模式");
            setVal[SET_ID_StartCoins].SetName("几币一玩");
            setVal[SET_ID_TicketBl].SetName("彩票比率");
            setVal[SET_ID_GiftBl].SetName("扭蛋比率");
            setVal[SET_ID_GameTime].SetName("游戏时间");
            //setVal[SET_ID_MonsterNum_1].SetName("第1关怪物数量");
            //setVal[SET_ID_MonsterNum_2].SetName("第2关怪物数量");
            //setVal[SET_ID_MonsterNum_3].SetName("第3关怪物数量");
            setVal[SET_ID_ScoreTtl].SetName("是否积累");
            setVal[SET_ID_DeskMusic].SetName("待机音乐");
            // 中性模式
            setVal[SET_ID_TicketsOneCoin].SetName("1币兑票数");
            setVal[SET_ID_EditerOneCoin].SetName("1币分数");
            setVal[SET_ID_MinTickets].SetName("安慰票数");
            setVal[SET_ID_OutDcTime].SetName("退检测时间");
            setVal[SET_ID_KeyDelay].SetName("灯按键延迟");
            setVal[SET_ID_PlayerOrder].SetName("玩家顺序");
            setVal[SET_ID_AnZhuang].SetName("灯安装方向");
            setVal[SET_ID_SafePlace].SetName("游戏总时间");
            // 共用
            setVal[SET_ID_MainSoundVolume].SetName("游戏音量");
            setVal[SET_ID_LeiShe_LMD].SetName("镭射灯灵敏度");

            // 安装方向
            setVal[SET_ID_AnZhuang].SetValueName(0, "左下往右");
            setVal[SET_ID_AnZhuang].SetValueName(1, "左下往上");
            setVal[SET_ID_AnZhuang].SetValueName(2, "左上往右");
            setVal[SET_ID_AnZhuang].SetValueName(3, "左上往下");
            setVal[SET_ID_AnZhuang].SetValueName(4, "右上往下");
            setVal[SET_ID_AnZhuang].SetValueName(5, "右上往左");
            setVal[SET_ID_AnZhuang].SetValueName(6, "右下往上");
            setVal[SET_ID_AnZhuang].SetValueName(7, "右下往左");
            setVal[SET_ID_AnZhuang].SetValueName(8, "镭射1拖8");

            // 设置项值: 
            // 游戏模式
            setVal[SET_ID_GameMode].SetValueName((int)en_GameMode.Normal, "普通模式");
            setVal[SET_ID_GameMode].SetValueName((int)en_GameMode.CardId, "手环模式");
            setVal[SET_ID_IsContinue].SetValueName(0, "否");
            setVal[SET_ID_IsContinue].SetValueName(1, "是");
 
            setVal[SET_ID_TimeMode].SetValueName(0, "有时间限制");
            setVal[SET_ID_TimeMode].SetValueName(1, "无限时间游玩");
            
            // 投退模式
            setVal[SET_ID_GameChoose].SetValueName(0, "地板砖");
            setVal[SET_ID_GameChoose].SetValueName(1, "镭射灯");
            setVal[SET_ID_GameChoose].SetValueName(2, "攀岩");
            setVal[SET_ID_GameChoose].SetValueName(4, "篮球机");
            setVal[SET_ID_GameChoose].SetValueName(3, "投掷");
            setVal[SET_ID_GameChoose].SetValueName(5, "恶魔之眼");
            setVal[SET_ID_GameChoose].SetValueName(6, "拍拍灯");
            setVal[SET_ID_GameChoose].SetValueName(7, "镭射拍拍灯");
            // 投退模式
            setVal[SET_ID_InOutMode].SetValueName(0, "双投双退");
            setVal[SET_ID_InOutMode].SetValueName(1, "双投单退");
            setVal[SET_ID_InOutMode].SetValueName(2, "单投单退");


            //setVal[SET_ID_SafePlace].SetValueName(0, "0圈");
            //setVal[SET_ID_SafePlace].SetValueName(1, "1圈");
            //setVal[SET_ID_SafePlace].SetValueName(2, "2圈");

            // 枪模式
            setVal[SET_ID_GunMode].SetValueName(0, "枪");
            setVal[SET_ID_GunMode].SetValueName(1, "射珠");
            setVal[SET_ID_GunMode].SetValueName(2, "射水");

            // 玩家模式
            setVal[SET_ID_PlayerMode].SetValueName(0, "单人");
            setVal[SET_ID_PlayerMode].SetValueName(1, "双人");

            // 奖励模式
            setVal[SET_ID_OutMode].SetValueName(0, "离线");
            setVal[SET_ID_OutMode].SetValueName(1, "TCP"); 
            //
            setVal[SET_ID_ScoreTtl].SetValueName(0, "否");
            setVal[SET_ID_ScoreTtl].SetValueName(1, "是");
            //  
            //
            setVal[SET_ID_ShowIdle].SetValueName(0, "关");
            setVal[SET_ID_ShowIdle].SetValueName(1, "开");
            //
            setVal[SET_ID_DeskMusic].SetValueName(0, "关");
            setVal[SET_ID_DeskMusic].SetValueName(1, "开");
            //
            setVal[SET_ID_PlayerOrder].SetValueName(0, "从左到右");
            setVal[SET_ID_PlayerOrder].SetValueName(1, "从右到左");

            //
            //setVal[SET_ID_StartCoins].SetDescript("设置多少分玩一次\r\n举例：1.设置为0时，免费玩游戏\r\n           2.设置为2，每局游戏需要2分才能开始游戏");
            ////
            //setVal[SET_ID_PlayerMode].SetDescript("单人：一个人\r\n双人：两个人");

            //setVal[SET_ID_LedProtocol].SetDescript("枪伤害：\r\n 1: 1\r\n 2: 1.5\r\n 3: 2\r\n 4: 2.5\r\n 5: 3\r\n");

            //setVal[SET_ID_GunMode].SetDescript("枪模式：射珠、射水、枪");

            //setVal[SET_ID_InOutMode].SetDescript("退票模式：双投双退、单投单退、双投单退");

            //setVal[SET_ID_StartCoins].SetDescript("多少分可以玩一次");

            //setVal[SET_ID_TicketBl].SetDescript("彩票比率");

            //setVal[SET_ID_GiftBl].SetDescript("扭蛋比率");

            //setVal[SET_ID_OutMode].SetDescript("1:无奖励\r\n2:退扭蛋\r\n3:退彩票");

            //setVal[SET_ID_OutDcTime].SetDescript("退检测时间：20-600，检测两个礼品的间隔时间（毫秒）");

            //setVal[SET_ID_KeyDelay].SetDescript ("灯按键延迟：0-29，检测灯按键的延迟帧数");


            //setVal[SET_ID_GameTime].SetDescript("投一个比大概玩多长时间");

            //setVal[SET_ID_DeskMusic].SetDescript("关 = 关闭待机音乐\r\n开 = 打开待机音乐");

            ////
        }
        else
        {
            // 要显示的设置项 名称
            setVal[SET_ID_AnZhuang].SetName("Light MoveWay");
            setVal[SET_ID_AnZhuang].SetValueName(0, "Bottom left to right");
            setVal[SET_ID_AnZhuang].SetValueName(1, "Bottom left to Top");
            setVal[SET_ID_AnZhuang].SetValueName(2, "Top left to right");
            setVal[SET_ID_AnZhuang].SetValueName(3, "Top left to Down");
            setVal[SET_ID_AnZhuang].SetValueName(4, "Top Right to Down");
            setVal[SET_ID_AnZhuang].SetValueName(5, "Top Right to Left");
            setVal[SET_ID_AnZhuang].SetValueName(6, "Bottom Right to Top");
            setVal[SET_ID_AnZhuang].SetValueName(7, "Bottom Right to Left");
            //
            setVal[SET_ID_GameMode].SetName("Game Mode");
            setVal[SET_ID_SafePlace].SetName("Number of safe zone laps");
            setVal[SET_ID_InOutMode].SetName("Input And Output Mode");
            setVal[SET_ID_LedProtocol].SetName("Led Type");
            setVal[SET_ID_GunMode].SetName("Gun Mode");
            setVal[SET_ID_PlayerMode].SetName("Player Mode");
            setVal[SET_ID_StartCoins].SetName("Start coins");
            setVal[SET_ID_GameTime].SetName("Game Time");
            //setVal[SET_ID_MonsterNum_1].SetName("Number Of Monster In Level 1");
            //setVal[SET_ID_MonsterNum_2].SetName("Number Of Monster In Level 2");
            //setVal[SET_ID_MonsterNum_3].SetName("Number Of Monster In Level 3");

            setVal[SET_ID_GameChoose].SetName("Choose Game");
            setVal[SET_ID_GameChoose].SetValueName(0, "Floor");
            setVal[SET_ID_GameChoose].SetValueName(1, "Laser Maze");
            setVal[SET_ID_GameChoose].SetValueName(2, "Climb");
            setVal[SET_ID_GameChoose].SetValueName(3, "Arena");
            setVal[SET_ID_GameChoose].SetValueName(4, "Hoops");
            setVal[SET_ID_GameChoose].SetValueName(5, "Hide ");
            setVal[SET_ID_GameChoose].SetValueName(6, "Touch Button");
            setVal[SET_ID_GameChoose].SetValueName(7, "Laser Touch Button");

            setVal[SET_ID_ScoreTtl].SetName("Wheher The Cumulative");
            setVal[SET_ID_DeskMusic].SetName("Dest Music");
            setVal[SET_ID_MainSoundVolume].SetName("Sound Volume");
            setVal[SET_ID_LeiShe_LMD].SetName("Light Sensitivity");


            setVal[SET_ID_OutMode].SetName("Link Mode");
            setVal[SET_ID_TicketBl].SetName("Ticket Ratio");
            setVal[SET_ID_GiftBl].SetName("Gift Ratio");
            // 中性模式
            setVal[SET_ID_TicketsOneCoin].SetName("Tickets Per Coin");
            setVal[SET_ID_EditerOneCoin].SetName("Editers Per Coin");
            setVal[SET_ID_MinTickets].SetName("Min Tickets");
            setVal[SET_ID_OutDcTime].SetName("Out Check Time");

            setVal[SET_ID_KeyDelay].SetName("LED Key Delay");
            setVal[SET_ID_PlayerOrder].SetName("Player Order");

            // 设置项值: 
            // 游戏模式
            setVal[SET_ID_GameMode].SetValueName((int)en_GameMode.Normal, "Normal mode");
            setVal[SET_ID_GameMode].SetValueName((int)en_GameMode.CardId, "Bracelet mode");
            // 投退模式
            setVal[SET_ID_InOutMode].SetValueName(0, "Double Inupt Double Return");
            setVal[SET_ID_InOutMode].SetValueName(1, "Double Inupt Single Return");
            setVal[SET_ID_InOutMode].SetValueName(2, "Single Input Single Return");

            //setVal[SET_ID_SafePlace].SetValueName(0, "0 lap");
            //setVal[SET_ID_InOutMode].SetValueName(1, "1 lap");
            //setVal[SET_ID_InOutMode].SetValueName(2, "2 lap");
            setVal[SET_ID_IsContinue].SetName("Auto Continue");
            setVal[SET_ID_IsContinue].SetValueName(0, "No");
            setVal[SET_ID_IsContinue].SetValueName(1, "Yes");
            //
            setVal[SET_ID_TimeMode].SetName("Time Limit");
            setVal[SET_ID_TimeMode].SetValueName(0, "Have TimeControl");
            setVal[SET_ID_TimeMode].SetValueName(1, "Have No TimeControl");
            // 枪模式
            setVal[SET_ID_GunMode].SetValueName(0, "Gun");
            setVal[SET_ID_GunMode].SetValueName(1, "Shoot Bead");
            setVal[SET_ID_GunMode].SetValueName(2, "Shoot Water");
            // 投退模式
            setVal[SET_ID_PlayerMode].SetValueName(0, "One Player");
            setVal[SET_ID_PlayerMode].SetValueName(1, "Two Player");
            // 奖励模式
            setVal[SET_ID_OutMode].SetValueName(0, "OffLine");
            setVal[SET_ID_OutMode].SetValueName(1, "TCP"); 
            //
            setVal[SET_ID_ScoreTtl].SetValueName(0, "No");
            setVal[SET_ID_ScoreTtl].SetValueName(1, "Yes");
            //
            setVal[SET_ID_DeskMusic].SetValueName(0, "OFF");
            setVal[SET_ID_DeskMusic].SetValueName(1, "ON");
            //
            setVal[SET_ID_PlayerOrder].SetValueName(0, "Left To Right");
            setVal[SET_ID_PlayerOrder].SetValueName(1, "Right To Left");
            //
            setVal[SET_ID_ShowIdle].SetName("ShowIdle");
            setVal[SET_ID_ShowIdle].SetValueName(0, "OFF");
            setVal[SET_ID_ShowIdle].SetValueName(1, "ON");
            //setVal[SET_ID_StartCoins].SetDescript("Set how many points to play once\r\nExample：1. When set to 0, play the game for free\r\n            2. Set it to 2, and each game needs 2 points to start ");
            ////
            //setVal[SET_ID_PlayerMode].SetDescript("Single：One People\r\ndouble：Two People");

            //setVal[SET_ID_LedProtocol].SetDescript(" Gun damage ：\r\n 1: 1\r\n 2: 1.5\r\n 3: 2\r\n 4: 2.5\r\n 5: 3\r\n");

            //setVal[SET_ID_GunMode].SetDescript(" Gun mode: bead shooting, water shooting, gun ");

            //setVal[SET_ID_InOutMode].SetDescript(" Refund mode: double vote and double refund\r\n single vote and single refund\r\n double vote and single refund ");

            //setVal[SET_ID_StartCoins].SetDescript(" How many points can you play once ");

            //setVal[SET_ID_TicketBl].SetDescript(" Lottery ratio ");

            //setVal[SET_ID_GiftBl].SetDescript(" Egg twist ratio ");

            //setVal[SET_ID_OutMode].SetDescript("1: No reward\r\n 2: Back twisted egg \r\n3: Return the lottery ticket");

            //setVal[SET_ID_OutDcTime].SetDescript(" Return detection time: 20-600, interval between detecting two gifts (MS) ");

            //setVal[SET_ID_KeyDelay].SetDescript (" LED Key Delay: 0-29, the frames of led key delay ");


            //setVal[SET_ID_GameTime].SetDescript(" How long does it take to throw a game ");

            //setVal[SET_ID_DeskMusic].SetDescript("shut = Turn off standby music\r\nopen = Turn on standby music");

            ////
            //setVal[SET_ID_MainSoundVolume].SetDescript("Adjust the game volume. The higher the value, the louder the sound");
        }

    }

    void UpdateDescript()
    {
        text_DescriptTitle.gameObject.SetActive(false);
        text_Descript.gameObject.SetActive(false);
        ////
        //if (postIndex >= list_SetVal.Count)
        //{
        //    volumeTips_Obj.SetActive(false);
        //    return;
        //}
        //bool volumeTips = false;
        //for (int i = 0; i < setVal.Length; i++)
        //{
        //    if (list_SetVal[postIndex] == setVal[i])
        //    {
        //        text_DescriptTitle.gameObject.SetActive(true);
        //        text_Descript.gameObject.SetActive(true);
        //        text_DescriptTitle.text = setVal[i].text_Name.text + "：";
        //        //text_Descript.text = setVal[i].text_SetValue.text + setVal[i].GetDescript(setVal[i].GetValue());
        //        text_Descript.text = setVal[i].GetDescript(setVal[i].GetValue());
        //        //
        //        if (i == SET_ID_MainSoundVolume)
        //        {
        //            volumeTips = true;
        //            int value = setVal[i].GetValue();
        //            image_VolumeValue.fillAmount = (float)value / Set.MAX_SOUND_VOLUME;
        //            AudioListener.volume = (float)value / Set.MAX_SOUND_VOLUME;
        //        }
        //        break;
        //    }
        //}
        //volumeTips_Obj.SetActive(volumeTips);
    }

    // 更新设置内容 :
    void UpdataGameSet()
    {
        //setVal[SET_ID_InOutMode].UpdateValue(Set.setVal.InOutMode);
        setVal[SET_ID_GameMode].UpdateValue(Set.setVal.GameMode);
        setVal[SET_ID_IsContinue].UpdateValue(Set.setVal.isContinue);
        setVal[SET_ID_TimeMode].UpdateValue(Set.setVal.TimeMode);
        setVal[SET_ID_ReadyTime].UpdateValue(Set.setVal.ReadyTime);
        setVal[SET_ID_ShowIdle].UpdateValue(Set.setVal.ShowIdle);
        setVal[SET_ID_InOutMode].UpdateValue(Set.setVal.InOutMode);
        setVal[SET_ID_GameChoose].UpdateValue(Set.setVal.GameChoose);
        setVal[SET_ID_ShowIdle].UpdateValue(Set.setVal.ShowIdle);
        setVal[SET_ID_LedProtocol].UpdateValue(Set.setVal.LedProtocol);
        setVal[SET_ID_GunMode].UpdateValue(Set.setVal.GunMode);
        setVal[SET_ID_PlayerMode].UpdateValue(Set.setVal.PlayerMode);
        //
        setVal[SET_ID_OutMode].UpdateValue(Set.setVal.OutMode);
        setVal[SET_ID_TicketBl].UpdateValue(Set.setVal.TicketBl);
        setVal[SET_ID_GiftBl].UpdateValue(Set.setVal.GiftBl);
        setVal[SET_ID_StartCoins].UpdateValue(Set.setVal.StartCoins);
        setVal[SET_ID_GameTime].UpdateValue(Set.setVal.GameTime);
        //setVal[SET_ID_MonsterNum_1].UpdateValue(Set.setVal.MonsterNum_1);
        //setVal[SET_ID_MonsterNum_2].UpdateValue(Set.setVal.MonsterNum_2);
        //setVal[SET_ID_MonsterNum_3].UpdateValue(Set.setVal.MonsterNum_3);
        setVal[SET_ID_ScoreTtl].UpdateValue(Set.setVal.ScoreTtl);
        setVal[SET_ID_DeskMusic].UpdateValue(Set.setVal.DeskMusic);
        //
        setVal[SET_ID_TicketsOneCoin].UpdateValue(Set.setVal.TicketsOneCoin);
        setVal[SET_ID_EditerOneCoin].UpdateValue(Set.setVal.EditerOneCoin);
        setVal[SET_ID_MinTickets].UpdateValue(Set.setVal.MinTickets);
        setVal[SET_ID_OutDcTime].UpdateValue(Set.setVal.OutDcTime);
        setVal[SET_ID_KeyDelay].UpdateValue(Set.setVal.KeyDelay);
        setVal[SET_ID_PlayerOrder].UpdateValue(Set.setVal.PlayerOrder);
        //
        setVal[SET_ID_MainSoundVolume].UpdateValue(Set.setVal.MainSoundVolume);
        setVal[SET_ID_LeiShe_LMD].UpdateValue(Set.setVal.LeiShe_LMD);
        setVal[SET_ID_AnZhuang].UpdateValue(Set.setVal.Index_AnZhuang);
        setVal[SET_ID_SafePlace].UpdateValue(Set.setVal.Num_SafePlace);

    }


    //按键 ------
    void DefaultSetting(bool isOk)
    {
        if (isOk)
        {
            Set.Default();
            GameStart();
            //
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
    void Back(bool isOk)
    {
        if (isOk)
        {
            menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
        }
    }
    public void OnButton_SetBack_Pressed()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            menuTips.Init("是否不保存退出?", Back);
        }
        else
        {
            menuTips.Init("Exit without saving?", Back);
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
    public void SaveSetting(bool isOk)
    {
        if (isOk == false)
            return;
        // 切换模式要清零
        //if (Set.setVal.OutMode != setVal[SET_ID_OutMode].GetValue()) {
        //    FjData.Clear();    // 清零当前账目
        //}
        //// 中性模式：切换兑票比率时，清零账目(当前账目)和几率
        //if (setVal[SET_ID_GameMode].GetValue() == (int)en_GameMode.Zhongxing) {
        //    if (Set.setVal.TicketsOneCoin != setVal[SET_ID_TicketsOneCoin].GetValue()) {
        //        FjData.Clear();    // 清零当前账目
        //        JL.Clear();
        //    }
        //}

        //保存设置
        //Set.setVal.InOutMode = setVal[SET_ID_InOutMode].GetValue();
        Set.setVal.GameMode = setVal[SET_ID_GameMode].GetValue();
        Set.setVal.ReadyTime = setVal[SET_ID_ReadyTime].GetValue();
        Set.setVal.ShowIdle = setVal[SET_ID_ShowIdle].GetValue();
        Set.setVal.isContinue = setVal[SET_ID_IsContinue].GetValue();
        Set.setVal.TimeMode = setVal[SET_ID_TimeMode].GetValue();
        Set.setVal.GameChoose = setVal[SET_ID_GameChoose].GetValue();
        Set.setVal.InOutMode = setVal[SET_ID_InOutMode].GetValue();
        Set.setVal.LedProtocol = setVal[SET_ID_LedProtocol].GetValue();
        Set.setVal.GunMode = setVal[SET_ID_GunMode].GetValue();
        Set.setVal.PlayerMode = setVal[SET_ID_PlayerMode].GetValue();
        //
        Set.setVal.OutMode = setVal[SET_ID_OutMode].GetValue();
        Set.setVal.TicketBl = setVal[SET_ID_TicketBl].GetValue();
        Set.setVal.GiftBl = setVal[SET_ID_GiftBl].GetValue();
        Set.setVal.StartCoins = setVal[SET_ID_StartCoins].GetValue();
        Set.setVal.GameTime = setVal[SET_ID_GameTime].GetValue();
     
        //Set.setVal.MonsterNum_1 = setVal[SET_ID_MonsterNum_1].GetValue();
        //Set.setVal.MonsterNum_2 = setVal[SET_ID_MonsterNum_2].GetValue();
        //Set.setVal.MonsterNum_3 = setVal[SET_ID_MonsterNum_3].GetValue();
        Set.setVal.ScoreTtl = setVal[SET_ID_ScoreTtl].GetValue();
        Set.setVal.DeskMusic = setVal[SET_ID_DeskMusic].GetValue();
        //
        Set.setVal.TicketsOneCoin = setVal[SET_ID_TicketsOneCoin].GetValue();
        Set.setVal.EditerOneCoin = setVal[SET_ID_EditerOneCoin].GetValue();
        Set.setVal.MinTickets = setVal[SET_ID_MinTickets].GetValue();
        Set.setVal.OutDcTime = setVal[SET_ID_OutDcTime].GetValue();
        Set.setVal.KeyDelay = setVal[SET_ID_KeyDelay].GetValue();
        Set.setVal.LeiShe_LMD = setVal[SET_ID_LeiShe_LMD].GetValue();
        Set.setVal.PlayerOrder = setVal[SET_ID_PlayerOrder].GetValue();
        //
        Set.setVal.MainSoundVolume = setVal[SET_ID_MainSoundVolume].GetValue();

        Set.setVal.Num_SafePlace = setVal[SET_ID_SafePlace].GetValue();
        Set.setVal.Index_AnZhuang = setVal[SET_ID_AnZhuang].GetValue();

        Set.SaveAll();

        Main.PlayTime= Set.setVal.GameTime  ;
        Main.CanSend_Score = true;
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
        //Debug.Log("保存成功");
    }
}
