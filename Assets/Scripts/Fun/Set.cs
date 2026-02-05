using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public enum en_Language
{
    Chinese = 0,		// 中文
    English,			// 英文
}

public enum en_GameMode
{
    Normal = 0,         // 普通模式
    CardId,             // 手环模式
}
public enum en_GunMode
{
    Gun = 0,            // 枪
    ShootBead,          // 射珠
    ShootWater,         // 射水
}
public enum en_PlayerMode
{
    //One = 0,            // 单人
    //Two,                // 双人
    Free = 0,           // 休闲模式
    PassLevel,          // 闯关模式
    Challenge,          // 挑战模式
    XiaoHai,          // 挑战模式
}
public enum en_InOutMode
{
    TwoInTwoOut = 0,    // 双投双退
    TwoInOneOut,        // 双投单退
    OneInOneOut,        // 单投单退
}
public enum en_OutMode
{
    OutNone = 0,		// 不退
    OutTicket,			// 退彩票模式
    OutGift,            // 退扭蛋
    //	OutCoin,			// 退币模式
}

public enum en_GameId
{
    YueDongGeZi = 0,    // 地砖灯
    LeiSheWu,           // 镭射屋
    PanYan,             // 攀岩
    TouZhi,             //投掷
    LanQiu,             //篮球机

    DevEyes,            //恶魔之眼
    PaiPaiDeng,         //拍拍灯
    LeiShePPD,          //镭射拍拍灯
}

public enum en_ConnectOrder
{
    LeftDown_ToRight = 0,
    LeftDown_ToUp,
    LeftUp_ToRight,
    LeftUp_ToDown,
    RightUp_ToDown,
    RightUp_ToLeft,
    RightDown_ToUp,
    RightDown_ToLeft,
    LeftUp_ToDown_LEIShe
}

public enum en_LedProtocol
{
    Liang = 0x01,
    Zhu = 0x02,
    DianZhen = 0x03,
}

public struct GameSet
{
    public int Language;		// 语言: 0:中文; 1:英文
    public int GameMode;		// 游戏模式: 0: 娱乐 1:中性模式
    public int isContinue;		// 是否自动续玩
    public int ReadyTime;		// 是否自动续玩
    public int ShowIdle;		// 是否 
    public int BraceletMode;    // 手环模式: 开/关
    public int PlayerMode;      // 玩家数量: 0: 单人 1: 双人
    public int GunMode;         // 枪模式：0: 枪  1:射珠   2:射水
    public int InOutMode;       // 投退模式：0: 双投双退； 1：双投单退； 2：单投单退
    public int GameChoose;       
    public int LedProtocol;
    //娱乐模式
    public int OutMode;			// 奖励模式: 不退,退票,退币    
    public int TicketBl;        // 彩票比率
    public int GiftBl;          // 扭蛋比率
    public int StartCoins;		// 开始分数: 几分玩一次
    public int GameTime;		// 游戏时间
    public int LevelWaitTime;	// 游戏时间
    public int TimeEnable;		// 游戏时间限制(仅免费模式有效)
    public int TouchStart;      // 实体开始按键    
    public int CanErrorLedNum;	// 容错灯数
    public int OutDcTime;       // 退检测时间
    public int MonsterNum_1;    // 怪物数量(第一关)
    public int MonsterNum_2;    // 怪物数量(第二关)
    public int MonsterNum_3;    // 怪物数量(第三关)
    public int ScoreTtl;        // 是否累积
    // 中性模式
    public int TicketsOneCoin;  // 1币兑票数
    public int EditerOneCoin;   // 1币分数(子弹数)
    public int MinTickets;      // 最小退票(安慰奖)
    // 其它没用到的
    //public int UpCentBl;		// 上下分比率
    //public int MinOut;			// 最小退币
    //public int MaxOut;			// 最大退币
    //public int OverflowCent;	// 爆机分数
    //public int Odds;			// 难度
    //public int Wave;			// 波动
    public int DeskMusic;       // 背景音乐
    public int MainSoundVolume;	// 主机音量
    public int Index_AnZhuang;	// 安装方向
    public int Num_SafePlace;	// 安全区圈数
    public int SysVolume;       // 系统音量(直接左右设置按键调整)
    public int Password;

    public int KeyDelay;        // 按键延
    public int PlayerOrder;
    public int Width;
    public int Height;
    public int PPDWidth;        //镭射拍拍灯中拍拍灯的长宽，可能与镭射长宽不同
    public int PPDHeight;
    public int WallLedNum;      // 按键灯个数      
    public int LeiShe_LMD;      // 镭射灯灵敏度     
    public int TargetLedNum;
    public int AccPassword;
    public int MenuPassword;
    public int TimeMode;

    public int WallNum_Width;
    public int WallNum_Height;
}

public class Set
{
    //----------------------------
    const string SET_AccPassword = "SETAccPassword";
    const string SET_Language = "SETLanguage";
    const string SET_GameMode = "SETGameMode";
    const string SET_isContinue = "SET_isContinue";
    const string SET_TimeMode = "SET_TimeMode";
    const string SET_ShowIdle = "SET_ShowIdle";
    const string SET_ReadyTime = "SET_ReadyTime";
    const string SET_BraceletMode = "SETBraceletMode";
    const string SET_GameChoose = "SETGameChoose";
    const string SET_InOutMode = "SETInOutMode";
    const string SET_LedProtocol = "SETLedProtocol";
    const string SET_GunMode = "SETGunMode";
    const string SET_PlayerMode = "SETPlayerMode";
    const string SET_StartCoins = "SETStartCoins";
    const string SET_GameTime = "SETGameTime";
    const string SET_LevelWaitTime = "SETLevelWaitTime";
    const string SET_TimeEnable = "SETTimeEnable";
    const string SET_TouchStart = "SETTouchStart";
    const string SET_CanErrorLedNum = "SETCanErrorLedNum";
    const string SET_OutDcTime = "SETOutDcTime";
    const string SET_MonsterNum_1 = "SETMonsterNum_1";
    const string SET_MonsterNum_2 = "SETMonsterNum_2";
    const string SET_MonsterNum_3 = "SETMonsterNum_3";
    const string SET_ScoreTtl = "SETScoreTtl";
    const string SET_DeskMusic = "SETDeskMusic";
    const string SET_KeyDelay = "SETKeyDelay";
    const string SET_PlayerOrder = "SETPlayerOrder";
    const string SET_MainSoundVolume = "SETMainSoundVolume";
    const string SET_AnZhuang = "SETAnZhuang";
    const string SET_SafePlace = "SET_SafePlace";
    const string SET_IoSoundVolume = "SETIoSoundVolume";
    const string SET_SysVolume = "SETSysVolume";
    const string SET_Password = "SETPassword";

    const string SET_OutMode = "SETOutMode";         // 退模式: 不退,退票,退币
    const string SET_TicketBl = "SETTicketBl";        // 彩票比率
    const string SET_GiftBl = "SETGiftBl";		    // 彩票比率
    const string SET_UpCentBl = "SETUpCentBl";		// 上下分比率
    const string SET_MinOut = "SETMinOut";			// 最小退币
    const string SET_MaxOut = "SETMaxOut";			// 最大退币
    const string SET_OverflowCent = "SETOverflowCent";	// 爆机分数
    const string SET_Odds = "SETOdds";			// 难度
    const string SET_Wave = "SETWave";			// 波动
    const string SET_LeiShe_LMD = "SET_LeiShe_LMD";			//  
    // 中性
    const string SET_TicketsOneCoin = "SETTicketsOneCoin";  // 1币兑票数
    const string SET_EditerOneCoin = "SETEditerOneCoin";   // 1币分数(子弹数)
    const string SET_MinTickets = "SETMinTickets";      // 最小退票(安慰奖)
    const string SET_GameSelect = "SETGameSelect";  //

    const string SET_Width = "SETWidth";  //
    const string SET_Height = "SETHeight";  //
    const string SET_PPDWidth = "SETPPDWidth";  //
    const string SET_PPDHeight = "SETPPDHeight";  //
    const string SET_WallLedNum = "SETWallLedNum";  //
    const string SET_TargetLedNum = "SETTargetLedNum";  //
    const string SET_ChannelLength = "SETChannelLength";  //
    const string SET_MenuPassword = "SETMenuPassword";
    const string SET_WallNum_Width = "SET_WallNum_Width";  //
    const string SET_WallNum_Height = "SET_WallNum_Height";  //


    //---------------------------
    public static int[] SET_c_Language = { 1 };
    public static int[] SET_c_GameMode = { 0, 1 };
    public static int[] SET_c_isContinue = { 0, 1 };
    public static int[] SET_c_TimeMode = { 0, 1 };
    public static int[] SET_c_ReadyTime = { 4, 5, 6, 7, 8, 9 };
    public static int[] SET_c_ShowIdle = { 0, 1 };
    public static int[] SET_c_BraceletMode = { 0, 1 };
    public static int[] SET_c_GameChoose = { 0, 1, 2, 3, 4, 5, 6, 7 };
    public static int[] SET_c_InOutMode = { 0, 1, 2 };
    public static int[] SET_c_LedProtocol = { (int)en_LedProtocol.Liang, (int)en_LedProtocol.Zhu };
    public static int[] SET_c_CoinBl = { 0, 1, 2, 3, 4, 5, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
    public static int[] SET_c_StartCoins = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 25, 30, 35, 40, 45, 50 };
    public static int[] SET_c_GameTime = { 30, 60, 120, 300, 600, 720, 900, 1200, 1500, 1800, 2100, 2400, 2700, 3000, 3600, 4800, 7200 };
    public static int[] SET_c_LevelWaitTime = { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60 };
    public static int[] SET_c_TimeEnable = { 0, 1 };
    public static int[] SET_c_TouchStart = { 0, 1 };
    public static int[] SET_c_CanErrorLedNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public static int[] SET_c_OutDcTime = { 0, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 180, 210, 250, 300, 350, 400, 450, 500, 600 };
    public static int[] SET_c_MonsterNum_1 = { 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 200, 300 };
    public static int[] SET_c_MonsterNum_2 = { 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 200, 300 };
    public static int[] SET_c_MonsterNum_3 = { 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 200, 300 };

    public static int[] SET_c_ScoreTtl = { 0, 1 };
    public static int[] SET_c_DeskMusic = { 0, 1 };
    public static int[] SET_c_LeiShe_LMD = { 0, 1, 2, 3, 4, 5, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
    public static int[] SET_c_KeyDelay = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 40, 50, 60, 70, 80, 90, 100 };
    public static int[] SET_c_PlayerOrder = { 0, 1 };
    public static int[] SET_c_MainSoundVolume = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public static int[] SET_c_AnZhuang = { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
    public static int[] SET_c_SaftPlace = { 300, 600, 900, 1200, 1500, 1800, 2100, 2400, 2700, 3000, 3600, 4800, 7200 };
    public static int[] SET_c_Open = { 2, 1 };

    public static int[] SET_c_OutMode = { 0, 1 };            //  tcp or not
    public static int[] SET_c_GunMode = { 0, 1, 2 };            //枪  射珠 射水
    public static int[] SET_c_PlayerMode = { 0, 1 };
    public static int[] SET_c_TicketBl = { 0, 1, 2, 3, 4, 5, 10, 15, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };		// 彩票比率
    public static int[] SET_c_GiftBl = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public static int[] SET_c_UpCentBl = { 0, 10, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };     // 上下分比率
    public static int[] SET_c_MinOut = { 0, 10, 20, 30, 40, 50, 100, 200, 300, 400, 500 };         // 最小退币
    public static int[] SET_c_MaxOut = { 0, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };			// 最大退币
    public static int[] SET_c_OverflowCent = { 0, 5000, 7000, 10000, 20000, 30000, 40000, 50000, 100000, 200000, 300000, 400000, 500000, 1000000 }; // 爆机分数
    public static int[] SET_c_Odds = { 0, 1, 2, 3, 4, 5, 6, 7 };           // 难度
    public static int[] SET_c_Wave = { 0, 1, 2, 3, 4, 5, 6, 7 };            // 波动
    //
    public static int[] SET_c_TicketsOneCoin = { 10, 20, 30, 40, 50, 100, 200, 300, 400, 500 };
    public static int[] SET_c_EditerOneCoin = { 10, 20, 30, 40, 50, 100, 200, 300, 400, 500 };
    public static int[] SET_c_MinTickets = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    public static int[] SET_c_GameSelect = { 0, 1 };
    public static float MAX_SOUND_VOLUME = 10;		// 最大声音量

    public const int SET_c_MinSysVolume = 0;
    public const int SET_c_MaxSysVolume = 100;
    public const int MIN_PASSWORD = 0;
    public const int MAX_PASSWORD = 999999;
    public const int DEF_PASSWORD = 888;
    public const int SUP_PASSWORD = 461835;

    public const int DEF_ACCPASSWORD = 0;
    public const int MAX_ACCPASSWORD = 999999;
    public const int SUP_ACCPASSWORD = 461835;
    public const int MAX_MENUPASSWORD = 999;
    public const int DEF_MENUPASSWORD = 888;
    public const int SUP_MENUPASSWORD = 627;
    public static int CheckLED_Return = 0;
    public static int[] LedProtocol = { 0, 0, 0, 0, 0, 0 };
    //public const int MIN_Width = 1;
    //public const int MAX_Width = 125;
    //public const int MIN_Height = 1;
    //public const int MAX_Height = 125;
    //public const int MIN_ChannelLength = 2;
    //public const int MAX_ChannelLength = 125;

    public readonly static int[] SET_c_Size = {
        1,2,3,4,5,6,7,8,9,10,
        11,12,13,14,15,16,17,18,19,20,
        21,22,23,24,25,26,27,28,29,30,
        31,32,33,34,35,36,37,38,39,40,
        41,42,43,44,45,46,47,48,49,50,
        51,52,53,54,55,56,57,58,59,60,
        61,62,63,64,65,66,67,68,69,70,
        71,72,73,74,75,76,77,78,79,80,
        81,82,83,84,85,86,87,88,89,90,
        91,92,93,94,95,96,97,98,99,100
    };
    public readonly static int[] SET_c_WallLedNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public static int[] SET_c_TargetLedNum = {
        0, 1, 2,3,4,5,6,7,8,9,10,
        11,12,13,14,15,16,17,18,19,20,
        21,22,23,24,25,26,27,28,29,30,
        31,32,33,34,35,36,37,38,39,40,
        41,42,43,44,45,46,47,48,49,50,
        51,52,53,54,55,56,57,58,59,60,
        61,62,63,64,65,66,67,68,69,70
    };
    public readonly static int[] SET_c_ChannelLength = {
        0, 1, 2,3,4,5,6,7,8,9,10,
        11,12,13,14,15,16,17,18,19,20,
        21,22,23,24,25,26,27,28,29,30,
        31,32,33,34,35,36,37,38,39,40,
        41,42,43,44,45,46,47,48,49,50,
        51,52,53,54,55,56,57,58,59,60,
        61,62,63,64,65,66,67,68,69,70,
        71,72,73,74,75,76,77,78,79,80,
        81,82,83,84,85,86,87,88,89,90,
        91,92,93,94,95,96,97,98,99,100,
        101,102,103,104,105,106,107,108,109,110,
        111,112,113,114,115,116,117,118,119,120,
        121,122,123,124,125,126,127,128,129,130,
        131,132,133,134,135,136,137,138,139,140,
        141,142,143,144,145,146,147,148,149,150,
        151,152,153,154,155,156,157,158,159,160,
        161,162,163,164,165,166,167,168,169,170,
        171,172,173,174,175,176,177,178,179,180,
        181,182,183,184,185,186,187,188,189,190,
        191,192,
    };
    //
    public static GameSet setVal;
    public static int[] ChannelLength = new int[Main.MAX_CH];
    public static int[] StartPos = new int[Main.MAX_CH];
    public static int[] GameSelect = new int[Main.tab_GameId.Length];
    public static GameSetting[] gameSetting = new GameSetting[8];
    public static string[] gameName = new string[8];
    //public static string[] gameLeiSheName = new string[3];
    //public static GameLevelSetting[] gameLevelSetting = new GameLevelSetting[Main.MAX_LEVEL];

    static bool CheckLoad(int val, int[] tab)
    {
        for (int i = 0; i < tab.Length; i++)
        {
            if (val == tab[i])
            {
                return true;
            }
        }
        return false;
    }


    // fun
    public static void LoadAll()
    {
        int l1;


        // 第一次??
        if (PlayerPrefs.HasKey(SET_Language) == false)
        {
            DefaultLanguage();
            DefaultPassword();
            Default();
            DefaultGameSelect();
            DefaultSize();
        }

        LoadGameSetting();

        
        l1 = PlayerPrefs.GetInt(SET_Language);
        if (!CheckLoad(l1, SET_c_Language))
        {
            l1 = SET_c_Language[0];
            PlayerPrefs.SetInt(SET_Language, l1);
        }
        // setVal.Language = l1;
        // 固定语言：
        setVal.Language = (int)en_Language.English;

        //
        l1 = PlayerPrefs.GetInt(SET_GameMode);
        if (!CheckLoad(l1, SET_c_GameMode))
        {
            l1 = SET_c_GameMode[0];
            PlayerPrefs.SetInt(SET_GameMode, l1);
        }
        //setVal.GameMode = (int)en_GameMode.Normal;
        setVal.GameMode = l1;
        //

        //
        l1 = PlayerPrefs.GetInt(SET_isContinue);
        if (!CheckLoad(l1, SET_c_isContinue))
        {
            l1 = SET_c_isContinue[0];
            PlayerPrefs.SetInt(SET_isContinue, l1);
        }
        Set.setVal.isContinue = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_TimeMode);
        if (!CheckLoad(l1, SET_c_TimeMode))
        {
            l1 = SET_c_TimeMode[0];
            PlayerPrefs.SetInt(SET_TimeMode, l1);
        }
        //setVal.GameMode = (int)en_GameMode.Normal;
        setVal.TimeMode = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_ShowIdle);
        if (!CheckLoad(l1, SET_c_ShowIdle))
        {
            l1 = SET_c_ShowIdle[1];
            PlayerPrefs.SetInt(SET_ShowIdle, l1);
        }
        setVal.ShowIdle = l1;

        //
        l1 = PlayerPrefs.GetInt(SET_ReadyTime);
        if (!CheckLoad(l1, SET_c_ReadyTime))
        {
            l1 = SET_c_ReadyTime[0];
            PlayerPrefs.SetInt(SET_ReadyTime, l1);
        }
        //setVal.GameMode = (int)en_GameMode.Normal;
        setVal.ReadyTime = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_BraceletMode);
        if (!CheckLoad(l1, SET_c_BraceletMode))
        {
            l1 = SET_c_BraceletMode[0];
            PlayerPrefs.SetInt(SET_BraceletMode, l1);
        }
        setVal.BraceletMode = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_GameChoose);
        if (!CheckLoad(l1, SET_c_GameChoose))
        {
            l1 = SET_c_GameChoose[1];
            PlayerPrefs.SetInt(SET_GameChoose, l1);
        }
        setVal.GameChoose = (int)en_GameId.LeiSheWu;
        // setVal.GameChoose = l1;

        //
        l1 = PlayerPrefs.GetInt(SET_InOutMode);
        if (!CheckLoad(l1, SET_c_InOutMode))
        {
            l1 = SET_c_InOutMode[0];
            PlayerPrefs.SetInt(SET_InOutMode, l1);
        }
        setVal.InOutMode = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_LedProtocol);
        if (!CheckLoad(l1, SET_c_LedProtocol))
        {
            l1 = SET_c_LedProtocol[0];
            PlayerPrefs.SetInt(SET_LedProtocol, l1);
        }
        setVal.LedProtocol = l1;
        setVal.LedProtocol = (int)en_LedProtocol.Zhu;
#if UNITY_EDITOR
        setVal.LedProtocol = (int)en_LedProtocol.Liang;

#else
#endif
        //
        l1 = PlayerPrefs.GetInt(SET_AccPassword);
        if (l1 < MIN_PASSWORD || l1 > MAX_ACCPASSWORD)
        {
            l1 = DEF_ACCPASSWORD;
            PlayerPrefs.SetInt(SET_AccPassword, l1);
        }
        setVal.AccPassword = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_OutMode);
        if (!CheckLoad(l1, SET_c_OutMode))
        {
            l1 = SET_c_OutMode[0];
            PlayerPrefs.SetInt(SET_OutMode, l1);
        }
        setVal.OutMode = l1;
        //

        //
        l1 = PlayerPrefs.GetInt(SET_GunMode);
        if (!CheckLoad(l1, SET_c_GunMode))
        {
            l1 = SET_c_GunMode[0];
            PlayerPrefs.SetInt(SET_GunMode, l1);
        }
        setVal.GunMode = l1;
        //

        l1 = PlayerPrefs.GetInt(SET_PlayerMode);
        if (!CheckLoad(l1, SET_c_PlayerMode))
        {
            l1 = SET_c_PlayerMode[0];
            PlayerPrefs.SetInt(SET_PlayerMode, l1);
        }
        setVal.PlayerMode = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_MenuPassword);
        if (l1 < MIN_PASSWORD || l1 > MAX_MENUPASSWORD)
        {
            l1 = DEF_MENUPASSWORD;
            PlayerPrefs.SetInt(SET_MenuPassword, l1);
        }
        setVal.MenuPassword = l1;

        l1 = PlayerPrefs.GetInt(SET_TicketBl);
        if (!CheckLoad(l1, SET_c_TicketBl))
        {
            l1 = SET_c_TicketBl[0];
            PlayerPrefs.SetInt(SET_TicketBl, l1);
        }
        setVal.TicketBl = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_GiftBl);
        if (!CheckLoad(l1, SET_c_GiftBl))
        {
            l1 = SET_c_GiftBl[1];
            PlayerPrefs.SetInt(SET_GiftBl, l1);
        }
        setVal.GiftBl = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_StartCoins);
        if (!CheckLoad(l1, SET_c_StartCoins))
        {
            l1 = SET_c_StartCoins[0];
            PlayerPrefs.SetInt(SET_StartCoins, l1);
        }
        setVal.StartCoins = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_GameTime);
        if (!CheckLoad(l1, SET_c_GameTime))
        {
            l1 = SET_c_GameTime[4];
            PlayerPrefs.SetInt(SET_GameTime, l1);
        }
        setVal.GameTime = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_LevelWaitTime);
        if (!CheckLoad(l1, SET_c_LevelWaitTime))
        {
            l1 = SET_c_LevelWaitTime[4];
            PlayerPrefs.SetInt(SET_LevelWaitTime, l1);
        }
        setVal.LevelWaitTime = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_TimeEnable);
        if (!CheckLoad(l1, SET_c_TimeEnable))
        {
            l1 = SET_c_TimeEnable[1];
            PlayerPrefs.SetInt(SET_TimeEnable, l1);
        }
        setVal.TimeEnable = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_TouchStart);
        if (!CheckLoad(l1, SET_c_TouchStart))
        {
            l1 = SET_c_TouchStart[1];
            PlayerPrefs.SetInt(SET_TouchStart, l1);
        }
        setVal.TouchStart = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_CanErrorLedNum);
        if (!CheckLoad(l1, SET_c_CanErrorLedNum))
        {
            l1 = SET_c_CanErrorLedNum[0];
            PlayerPrefs.SetInt(SET_CanErrorLedNum, l1);
        }
        setVal.CanErrorLedNum = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_OutDcTime);
        if (!CheckLoad(l1, SET_c_OutDcTime))
        {
            l1 = SET_c_OutDcTime[1];
            PlayerPrefs.SetInt(SET_OutDcTime, l1);
        }
        setVal.OutDcTime = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_MonsterNum_1);
        if (!CheckLoad(l1, SET_c_MonsterNum_1))
        {
            l1 = SET_c_MonsterNum_1[0];
            PlayerPrefs.SetInt(SET_MonsterNum_1, l1);
        }
        setVal.MonsterNum_1 = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_MonsterNum_2);
        if (!CheckLoad(l1, SET_c_MonsterNum_2))
        {
            l1 = SET_c_MonsterNum_2[0];
            PlayerPrefs.SetInt(SET_MonsterNum_2, l1);
        }
        setVal.MonsterNum_2 = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_MonsterNum_3);
        if (!CheckLoad(l1, SET_c_MonsterNum_3))
        {
            l1 = SET_c_MonsterNum_3[0];
            PlayerPrefs.SetInt(SET_MonsterNum_3, l1);
        }
        setVal.MonsterNum_3 = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_ScoreTtl);
        if (!CheckLoad(l1, SET_c_ScoreTtl))
        {
            l1 = SET_c_ScoreTtl[0];
            PlayerPrefs.SetInt(SET_ScoreTtl, l1);
        }
        setVal.ScoreTtl = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_DeskMusic);
        if (!CheckLoad(l1, SET_c_DeskMusic))
        {
            l1 = 10;
            PlayerPrefs.SetInt(SET_DeskMusic, l1);
        }
        setVal.DeskMusic = l1;

        //  
        //
        l1 = PlayerPrefs.GetInt(SET_LeiShe_LMD);
        if (!CheckLoad(l1, SET_c_LeiShe_LMD))
        {
            l1 = SET_c_LeiShe_LMD[5];
            PlayerPrefs.SetInt(SET_LeiShe_LMD, l1);
        }
        setVal.LeiShe_LMD = l1;

        //
        l1 = PlayerPrefs.GetInt(SET_KeyDelay);
        if (!CheckLoad(l1, SET_c_KeyDelay))
        {
            l1 = SET_c_KeyDelay[0];
            PlayerPrefs.SetInt(SET_KeyDelay, l1);
        }
        setVal.KeyDelay = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_PlayerOrder);
        if (!CheckLoad(l1, SET_c_PlayerOrder))
        {
            l1 = SET_c_PlayerOrder[0];
            PlayerPrefs.SetInt(SET_PlayerOrder, l1);
        }
        setVal.PlayerOrder = l1;

        // 中性模式
        //
        l1 = PlayerPrefs.GetInt(SET_TicketsOneCoin);
        if (!CheckLoad(l1, SET_c_TicketsOneCoin))
        {
            l1 = SET_c_TicketsOneCoin[0];
            PlayerPrefs.SetInt(SET_TicketsOneCoin, l1);
        }
        setVal.TicketsOneCoin = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_EditerOneCoin);
        if (!CheckLoad(l1, SET_c_EditerOneCoin))
        {
            l1 = SET_c_EditerOneCoin[0];
            PlayerPrefs.SetInt(SET_EditerOneCoin, l1);
        }
        setVal.EditerOneCoin = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_MinTickets);
        if (!CheckLoad(l1, SET_c_MinTickets))
        {
            l1 = SET_c_MinTickets[0];
            PlayerPrefs.SetInt(SET_MinTickets, l1);
        }
        setVal.MinTickets = l1;

        //
        l1 = PlayerPrefs.GetInt(SET_MainSoundVolume);
        if (!CheckLoad(l1, SET_c_MainSoundVolume))
        {
            l1 = SET_c_MainSoundVolume[50];
            PlayerPrefs.SetInt(SET_MainSoundVolume, l1);
        }
        setVal.MainSoundVolume = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_AnZhuang);
        if (!CheckLoad(l1, SET_c_AnZhuang))
        {
            l1 = SET_c_AnZhuang[0];
            PlayerPrefs.SetInt(SET_AnZhuang, l1);
        }
        setVal.Index_AnZhuang = l1;
        //
        l1 = PlayerPrefs.GetInt(SET_SafePlace);
        if (!CheckLoad(l1, SET_c_SaftPlace))
        {
            l1 = SET_c_SaftPlace[2];
            PlayerPrefs.SetInt(SET_SafePlace, l1);
        }
        setVal.Num_SafePlace = l1;


        //
        l1 = PlayerPrefs.GetInt(SET_SysVolume, 50);
        if (l1 < SET_c_MinSysVolume || l1 > SET_c_MaxSysVolume)
        {
            l1 = 50;
            PlayerPrefs.SetInt(SET_SysVolume, l1);
        }
        setVal.SysVolume = l1;//l1   默认值  音量为50
                              //Debug.Log (setVal.SysVolume);
                              //
        l1 = PlayerPrefs.GetInt(SET_Password);
        if (l1 < MIN_PASSWORD || l1 > MAX_PASSWORD)
        {
            l1 = DEF_PASSWORD;
            PlayerPrefs.SetInt(SET_Password, l1);
        }
        setVal.Password = l1;

        //

        for (int i = 0; i < GameSelect.Length; i++)
        {
            l1 = PlayerPrefs.GetInt(SET_GameSelect + i);
            if (!CheckLoad(l1, SET_c_GameSelect))
            {
                l1 = 1;
                PlayerPrefs.SetInt(SET_GameSelect + i, l1);
            }
            GameSelect[i] = l1;
        }


        ////
        //l1 = PlayerPrefs.GetInt(SET_Odds) ;
        //if (!CheckLoad (l1, SET_c_Odds)) {
        //	l1 = SET_c_Odds [0];
        //	PlayerPrefs.SetInt(SET_Odds, l1) ;
        //}
        //setVal.Odds = l1;
        ////
        //l1 = PlayerPrefs.GetInt(SET_Wave) ;
        //if (!CheckLoad (l1, SET_c_Wave)) {
        //	l1 = SET_c_Wave [0];
        //	PlayerPrefs.SetInt(SET_Wave, l1) ;
        //}
        //setVal.Wave = l1;



        //
        setVal.Width = LoadOne(SET_Width, SET_c_Size, 12);
        setVal.PPDWidth = LoadOne(SET_PPDWidth, SET_c_Size, 12);
        //setVal.Width = 12;
        //
        setVal.Height = LoadOne(SET_Height, SET_c_Size, 12);
        setVal.PPDHeight = LoadOne(SET_PPDHeight, SET_c_Size, 12);
        //setVal.Height = 12;
        //
        setVal.WallLedNum = LoadOne(SET_WallLedNum, SET_c_WallLedNum, 20);
        //
        setVal.TargetLedNum = LoadOne(SET_TargetLedNum, SET_c_TargetLedNum, 0);
        //

        setVal.WallNum_Width = LoadOne(SET_WallNum_Width, SET_c_Size, 12);
        setVal.WallNum_Height = LoadOne(SET_WallNum_Height, SET_c_Size, 12);
        LoadChannelLength();

        // 测试 -----------------------------------------
        //		setVal.GameMode = 2;
        ////
        //setVal.MainSoundVolume = 1;
        //setVal.IoSoundVolume = 1;
        //setVal.Odds = 0;
    }
    public static void LoadChannelLength()
    {
        for (int i = 0; i < ChannelLength.Length; i++)
        {
            //ChannelLength[i] = LoadOne(SET_ChannelLength + i, MIN_ChannelLength, MAX_ChannelLength, 24);
            ChannelLength[i] = LoadOne(SET_ChannelLength + i, SET_c_ChannelLength, 24);
            //ChannelLength[i] = 24;
        }
        Update_StartPos();
    }
    public static void SaveOddsAndWave()
    {
        //PlayerPrefs.SetInt (SET_Odds, setVal.Odds);
        //PlayerPrefs.SetInt (SET_Wave, setVal.Wave);
        //PlayerPrefs.Save ();
    }
    public static void SaveAccPassword()
    {
        PlayerPrefs.SetInt(SET_AccPassword, setVal.AccPassword);
        PlayerPrefs.Save();
    }
    public static void SaveAll()
    {
        PlayerPrefs.SetInt(SET_GameMode, setVal.GameMode);
        PlayerPrefs.SetInt(SET_BraceletMode, setVal.BraceletMode);
        PlayerPrefs.SetInt(SET_InOutMode, setVal.InOutMode);
        PlayerPrefs.SetInt(SET_LedProtocol, setVal.LedProtocol);
        PlayerPrefs.SetInt(SET_GunMode, setVal.GunMode);
        PlayerPrefs.SetInt(SET_PlayerMode, setVal.PlayerMode);
        PlayerPrefs.SetInt(SET_OutMode, setVal.OutMode);
        PlayerPrefs.SetInt(SET_GameChoose, setVal.GameChoose);
        PlayerPrefs.SetInt(SET_StartCoins, setVal.StartCoins);
        PlayerPrefs.SetInt(SET_TicketBl, setVal.TicketBl);
        PlayerPrefs.SetInt(SET_GiftBl, setVal.GiftBl);
        PlayerPrefs.SetInt(SET_GameTime, setVal.GameTime);
        PlayerPrefs.SetInt(SET_ReadyTime, setVal.ReadyTime);
        PlayerPrefs.SetInt(SET_ShowIdle, setVal.ShowIdle);
        PlayerPrefs.SetInt(SET_LevelWaitTime, setVal.LevelWaitTime);
        PlayerPrefs.SetInt(SET_TimeEnable, setVal.TimeEnable);
        PlayerPrefs.SetInt(SET_TouchStart, setVal.TouchStart);
        PlayerPrefs.SetInt(SET_CanErrorLedNum, setVal.CanErrorLedNum);
        PlayerPrefs.SetInt(SET_OutDcTime, setVal.OutDcTime);
        PlayerPrefs.SetInt(SET_MonsterNum_1, setVal.MonsterNum_1);
        PlayerPrefs.SetInt(SET_MonsterNum_2, setVal.MonsterNum_2);
        PlayerPrefs.SetInt(SET_MonsterNum_3, setVal.MonsterNum_3);
        PlayerPrefs.SetInt(SET_ScoreTtl, setVal.ScoreTtl);
        PlayerPrefs.SetInt(SET_DeskMusic, setVal.DeskMusic);
        PlayerPrefs.SetInt(SET_isContinue, setVal.isContinue);
        PlayerPrefs.SetInt(SET_TimeMode, setVal.TimeMode);


        PlayerPrefs.SetInt(SET_KeyDelay, setVal.KeyDelay);
        PlayerPrefs.SetInt(SET_PlayerOrder, setVal.PlayerOrder);
        //
        PlayerPrefs.SetInt(SET_TicketsOneCoin, setVal.TicketsOneCoin);
        PlayerPrefs.SetInt(SET_EditerOneCoin, setVal.EditerOneCoin);
        PlayerPrefs.SetInt(SET_MinTickets, setVal.MinTickets);
        //
        PlayerPrefs.SetInt(SET_MainSoundVolume, setVal.MainSoundVolume);
        PlayerPrefs.SetInt(SET_LeiShe_LMD, setVal.LeiShe_LMD);
        PlayerPrefs.SetInt(SET_AnZhuang, setVal.Index_AnZhuang);
        PlayerPrefs.SetInt(SET_SafePlace, setVal.Num_SafePlace);//


        //PlayerPrefs.SetInt(SET_UpCentBl, setVal.UpCentBl);
        //PlayerPrefs.SetInt(SET_MinOut, setVal.MinOut);
        //PlayerPrefs.SetInt(SET_MaxOut, setVal.MaxOut);
        //PlayerPrefs.SetInt(SET_OverflowCent, setVal.OverflowCent);
        //PlayerPrefs.SetInt(SET_Odds, setVal.Odds);
        //PlayerPrefs.SetInt(SET_Wave, setVal.Wave);

        PlayerPrefs.Save();
    }

    public static void SaveLanguage()
    {
        PlayerPrefs.SetInt(SET_Language, setVal.Language);
        Debug.LogError(setVal.Language);
        PlayerPrefs.Save();
    }
    public static void SavePassword()
    {
        PlayerPrefs.SetInt(SET_Password, setVal.Password);
        PlayerPrefs.Save();
    }
    public static void SaveSysVolume()
    {

        PlayerPrefs.SetInt(SET_SysVolume, setVal.SysVolume);
        PlayerPrefs.Save();
    }
    public static void SaveGameSelect()
    {
        for (int i = 0; i < GameSelect.Length; i++)
        {
            PlayerPrefs.SetInt(SET_GameSelect + i, GameSelect[i]);
        }
        PlayerPrefs.Save();
    }

    public static void SaveSize()
    {
        PlayerPrefs.SetInt(SET_Width, setVal.Width);
        PlayerPrefs.SetInt(SET_Height, setVal.Height);
        PlayerPrefs.SetInt(SET_PPDWidth, setVal.PPDWidth);
        PlayerPrefs.SetInt(SET_PPDHeight, setVal.PPDHeight);
        PlayerPrefs.SetInt(SET_WallLedNum, setVal.WallLedNum);
        PlayerPrefs.SetInt(SET_TargetLedNum, setVal.TargetLedNum);
        PlayerPrefs.SetInt(SET_WallNum_Width, setVal.WallNum_Width);
        PlayerPrefs.SetInt(SET_WallNum_Height, setVal.WallNum_Height);
        for (int i = 0; i < ChannelLength.Length; i++)
        {
            PlayerPrefs.SetInt(SET_ChannelLength + i, ChannelLength[i]);
        }
        PlayerPrefs.Save();
        Update_StartPos();
    }
    public static void Update_StartPos()
    {
        int startPos = 0;
        for (int i = 0; i < StartPos.Length; i++)
        {
            StartPos[i] = startPos;
            startPos += ChannelLength[i];
        }
    }

    public static void Default()
    {
        setVal.GameChoose = (int)en_GameId.LeiSheWu;
        setVal.GameMode = (int)en_GameMode.Normal;
        setVal.BraceletMode = 0;
        setVal.InOutMode = (int)en_InOutMode.TwoInOneOut;
        setVal.LedProtocol = (int)en_LedProtocol.Zhu;
        setVal.GunMode = (int)en_GunMode.Gun;//默认枪
        setVal.PlayerMode = (int)en_PlayerMode.Free;
        setVal.OutMode = (int)en_OutMode.OutNone;			// 奖励模式: 退彩票
        setVal.StartCoins = 0;			// 游戏比例: 几分玩一次
        setVal.TicketBl = 10;			// 彩票比率
        setVal.GiftBl = 1;            //
        setVal.GameTime = 120;          // 游戏时间(自动出兵时间)
        setVal.LevelWaitTime = 20;
        setVal.TimeEnable = 1;
        setVal.TouchStart = 1;
        setVal.CanErrorLedNum = 0;
        setVal.OutDcTime = 20;
        setVal.MonsterNum_1 = 40;
        setVal.MonsterNum_2 = 50;
        setVal.MonsterNum_3 = 40;
        setVal.ScoreTtl = 0;
        setVal.DeskMusic = 0;   //
        setVal.KeyDelay = 10;   //
        setVal.PlayerOrder = 0;   //
        setVal.TicketsOneCoin = 20;				// 
        setVal.EditerOneCoin = 40;				// 
        setVal.MinTickets = 0;
        // 
        setVal.MainSoundVolume = 8;		// 主机音量
        setVal.Index_AnZhuang = 0;		// 
        setVal.Num_SafePlace = 1200;		// 

        //      setVal.MinOut = 1;				// 最小退币
        //setVal.MaxOut = 500;			// 最大退币
        //setVal.OverflowCent = 50000	;	// 爆机分数
        //setVal.Odds = 1;				// 难度
        //setVal.Wave = 0;				// 波动

        SaveAll();
    }

    public static void DefaultLanguage()
    {
        setVal.Language = (int)en_Language.English;
        SaveLanguage();
    }
    public static void DefaultPassword()
    {
        setVal.Password = DEF_PASSWORD;
        SavePassword();
    }
    public static void DefaultGameSelect()
    {
        for (int i = 0; i < GameSelect.Length; i++)//i= 5
        {
            GameSelect[i] = 1;
        }
        SaveGameSelect();
    }

    public static void DefaultSize()
    {
        setVal.Width = 16;
        setVal.Height = 5;
        setVal.PPDWidth = 12;
        setVal.PPDHeight = 5;
        setVal.WallLedNum = 0;
        setVal.TargetLedNum = 0;
        for (int i = 0; i < ChannelLength.Length; i++)
        {
            ChannelLength[i] = 0;
        }
        ChannelLength[0] = 80;
        ChannelLength[2] = 80;
        ChannelLength[5] = 60;
        ChannelLength[4] = 60;

        SaveSize();
    }

    //
    static int GetArryIndex(int[] arry, int value)
    {
        for (int i = 0; i < arry.Length; i++)
        {
            if (arry[i] == value)
            {
                return i;
            }
        }
        return -1;
    }
    public static int LoadOne(string addr, int[] tabValue, int defValue)
    {
        int value;
        if (PlayerPrefs.HasKey(addr) == false)
        {
            value = defValue;
        }
        else
        {
            value = PlayerPrefs.GetInt(addr);
            if (GetArryIndex(tabValue, value) < 0)
            {
                value = defValue;
            }
        }
        return value;
    }
    public static int LoadOne(string addr, int min, int max, int defValue)
    {
        int value;
        if (PlayerPrefs.HasKey(addr) == false)
        {
            value = defValue;
        }
        else
        {
            value = PlayerPrefs.GetInt(addr);
            if (value < min || value > max)
            {
                value = defValue;
            }
        }
        return value;
    }
    public static void DefaultMenuPassword()
    {
        setVal.MenuPassword = DEF_MENUPASSWORD;
        SaveMenuPassword();
    }
    public static void DefaultAccPassword()
    {
        setVal.AccPassword = DEF_ACCPASSWORD;
        SaveAccPassword();
    }

    //------------------------------------------------------------------------------------

    public static void SaveMenuPassword()
    {
        PlayerPrefs.SetInt(SET_MenuPassword, setVal.MenuPassword);
        PlayerPrefs.Save();
    }


    public const string GAMENAME_DEFAULT = "D-001";
    public static void DefaultGameName()
    {
        //gameName = GAMENAME_DEFAULT;
        for (int i = 0; i < gameName.Length; i++)
        {
            gameName[i] = "D-" + (i + 1).ToString("D3");
        }
        SaveGameName();
    }
    public static void SaveGameName()
    {
        for (int i = 0; i < gameName.Length; i++)
        {
            PlayerPrefs.SetString("gameName" + i, gameName[i]);
        }
        PlayerPrefs.Save();
    }
    public static void LoadGameSetting()
    {
        for (int i = 0; i < gameName.Length; i++)
        {
            gameName[i] = PlayerPrefs.GetString("gameName" + i);
            if (gameName[i] == "")
            {
                //gameName = GAMENAME_DEFAULT;
                gameName[i] = "D-" + (i + 1).ToString("D3");
            }
#if UNITY_EDITOR
            //            Debug.LogError ("Game: " + gameName[i]);
#endif
            //if (GameSetting.ExistGameSettingFile (gameName) == false) {
            //    gameName = GAMENAME_DEFAULT;
            //}
            gameSetting[i] = GameSetting.LoadGameSetting(gameName[i]);
        }
    }

    /*
    public static void SaveGameSetting (string fileName, GameSetting setting) {
        string directory = GetGameNameDirectory ();
        string path = directory + fileName;
        //        
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }
        ////二进制文件保存
        //BinaryFormatter bf = new BinaryFormatter ();
        //FileStream fs = File.Create (path);
        //bf.Serialize (fs, setting); 
        
        ////XML文件保存
        //FileStream fs = new FileStream (path, FileMode.OpenOrCreate);
        //XmlSerializer xml = new XmlSerializer (typeof (GameSetting));
        //xml.Serialize (fs, setting);
        

        //Jsion保存
#if UNITY_EDITOR
        Debug.Log ("SaveGame: " + path);
#endif
        string json = JsonUtility.ToJson (setting);
        StreamWriter fs = new StreamWriter (path);
        fs.Write (json);
        //
        fs.Close ();
    }

    public static GameSetting LoadGameSetting (string fileName) {
        GameSetting setting = null;
        string directory = GetGameNameDirectory ();        
        string path = directory + fileName;
#if UNITY_EDITOR
        Debug.Log ("LoadGame: " + path);
#endif
        if (File.Exists (path)) {
            //Debug.Log ("LoadedGame: ");
            try {
                ////二进制文件保存
                //FileStream fs = File.Open (path, FileMode.Open);
                //BinaryFormatter bf = new BinaryFormatter ();
                //setting = bf.Deserialize (fs) as GameSetting;
                //fs.Close ();
                
                ////XML文件保存
                //FileStream fs = File.Open (path, FileMode.Open);
                //XmlSerializer bf = new XmlSerializer (typeof (GameSetting));
                //setting = bf.Deserialize (fs) as GameSetting;
                //fs.Close ();
                
                //Jsion保存
                string json = File.ReadAllText (path);
                setting = JsonUtility.FromJson<GameSetting> (json);
            } catch (Exception e) {
                setting = null;
                Debug.LogError ("GameSetting Load Error: " + e);
            }
        }
        bool saveFlag = false;
        if (setting == null) {
            setting = new GameSetting ();
            setting.Default ();
            saveFlag = true;
        }
        for (int i = 0; i < setting.gameLevelSetting.Length; i++) {
            if (setting.gameLevelSetting[i] == null) {
                setting.gameLevelSetting[i] = new GameLevelSetting ();
                saveFlag = true;
            }
            for (int j = 0; j < setting.gameLevelSetting[i].animInfo.Length; j++) {
                if (setting.gameLevelSetting[i].animInfo[j] == null) {
                    setting.gameLevelSetting[i].animInfo[j] = new AnimOne ();
                    saveFlag = true;
                }
            }
        }
        if (setting.maxLevel > setting.gameLevelSetting.Length) {
            setting.maxLevel = setting.gameLevelSetting.Length;
        }
        if (saveFlag) {
            SaveGameSetting (fileName, setting);
        }
        return setting;
    }
    public static void DeleteGameSetting (string filename) {
        string directory = GetGameNameDirectory ();
        File.Delete (directory + filename);
    }
    public static void CreateGameSetting (string filename) {        
        GameSetting gameSetting = new GameSetting ();
        gameSetting.Default ();
        SaveGameSetting (filename, gameSetting);
    }
    public static bool ExistGameSettingFile (string filename) {
        string directory = GetGameNameDirectory ();
        return File.Exists (directory + filename);
    }
    public static string GetGameNameDirectory () {
        return Application.persistentDataPath + "/GameSetting/";
    }
    public static string[] GetGameNamePathAll () {
        string directory = GetGameNameDirectory ();
        return Directory.GetFiles (directory);
    }
    */
}
