using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Core.Led] 统一的游戏点结构体
/// 包含所有游戏点的通用字段和特定字段，用于统一 GameLedControl 和 GameLeiSheBase 的数据结构
/// </summary>
public struct GamePoint
{
    /// <summary>
    /// 基础点信息（状态和颜色）
    /// </summary>
    public BaseGamePoint basePoint;

    // ========== 通用LED控制字段（GameLedControl使用） ==========
    /// <summary>
    /// 死亡动画时间计数（通用LED控制使用）
    /// </summary>
    public int time;

    /// <summary>
    /// 错误闪烁时间计数（通用LED控制使用）
    /// </summary>
    public int errorTime;

    /// <summary>
    /// 目标时间（通用LED控制使用）
    /// </summary>
    public float tarageTime;

    // ========== 镭射模式字段（GameLeiSheBase使用） ==========
    /// <summary>
    /// 绑定计数（闪烁次数，镭射模式使用）
    /// </summary>
    public int bindCnt;

    /// <summary>
    /// 绑定时间（闪烁间隔，镭射模式使用）
    /// </summary>
    public float bindTime;

    /// <summary>
    /// 错误标志（镭射模式使用）
    /// </summary>
    public bool error;

    /// <summary>
    /// 点的状态（访问基础点的状态）
    /// </summary>
    public enPointSta statue
    {
        get => basePoint.statue;
        set => basePoint.statue = value;
    }

    /// <summary>
    /// 点的颜色（大写C，通用LED控制使用，保持向后兼容）
    /// </summary>
    public uint Color
    {
        get => basePoint.color;
        set => basePoint.color = value;
    }

    /// <summary>
    /// 点的颜色（小写c，镭射模式使用，保持向后兼容）
    /// </summary>
    public uint color
    {
        get => basePoint.color;
        set => basePoint.color = value;
    }

    /// <summary>
    /// 默认构造函数（通用LED控制）
    /// </summary>
    public GamePoint(enPointSta statue, uint color, int time = 0, int errorTime = 0, float tarageTime = 0)
    {
        basePoint = new BaseGamePoint(statue, color);
        this.time = time;
        this.errorTime = errorTime;
        this.tarageTime = tarageTime;
        // 初始化镭射模式字段为默认值
        this.bindCnt = 0;
        this.bindTime = 0;
        this.error = false;
    }

    /// <summary>
    /// 重置为默认值
    /// </summary>
    public void Reset()
    {
        basePoint.Reset();
        time = 0;
        errorTime = 0;
        tarageTime = 0;
        bindCnt = 0;
        bindTime = 0;
        error = false;
    }
}

/// <summary>[Core.Led] LED 点阵控制，管理 gamePoint 状态与 Framebuffer 同步。</summary>
public class GameLedControl
{
    public static GamePoint[] gamePoint = new GamePoint[Main.MAX_LED];
    public static PlayerControl[] playerControl = new PlayerControl[Main.MAX_PLAYER];
    static PresetPic picSetting;
    static bool isRunning = false;
    static float runTime;
    static int bkWidth;
    static int playerNum = 0;
    public static en_PlayerMode playerMode;

    public static void Init()
    {
        for (int i = 0; i < playerControl.Length; i++)
        {
            playerControl[i] = new PlayerControl();
        }
    }

    public static void Update_PointStatue(int id, enPointSta sta)
    {
        gamePoint[id].statue = sta;
    }

    public static void Stop()
    {
        isRunning = false;
    }

    public static void GameStart(en_PlayerMode mode)
    {
        playerMode = mode;
        if (playerMode == en_PlayerMode.Free)
        {
            playerNum = 1;
        }
        else
        {
            playerNum = 2;
        }
        for (int i = 0; i < playerControl.Length; i++)
        {
            playerControl[i].StopAll();
        }
    }
    public static void ReadyStart(GameLevelSetting levelSetting, bool isOverThree_tity)
    {
        for (int i = 0; i < playerControl.Length && i < playerNum; i++)
        {
            playerControl[i].Init(i, playerMode, 0);
        }
        picSetting = null;

        if (!isOverThree_tity)
        {
            picSetting = null;


            bkWidth = 0;



        }
        else
        {

            picSetting = levelSetting.picSetting;
            bkWidth = 1;
            DrawBianKuang(playerMode, bkWidth);
            for (int i = 0; i < playerControl.Length && i < playerNum; i++)
            {
                playerControl[i].Init(i, playerMode, 0);
            }
        }
        isRunning = true;

        //   
    }
    public static void PlayStart(GameLevelSetting levelSetting)
    {
        //   
        //for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        //{

        //    GameLedControl.gamePoint[i].Color = 0;


        //}


        switch (Set.setVal.GameChoose)
        {
            case 0:
               Game_Map00.instance.InitMap(Game00_Main.instance.gameLevel);
#if LiuGuang
                Map_WallLED.instance.InitMap(Game00_Main.instance.gameLevel);
#endif

                break;
            case 1:
             //   Game_Map01.instance.InitMap(Game01_Main.instance.gameLevel);

                break;
            case 2:
                //   
                if (Game02_Main.instance.BigGameLevel!=2)
                {
                    DrawBianKuang(playerMode, bkWidth);
                    SetRestPoint(levelSetting.picSetting.dataBuff);
                    for (int i = 0; i < playerControl.Length && i < playerNum; i++)
                    {
                        playerControl[i].RunStart(levelSetting.animSetting);
                    }
                }
                else
                {
                    Debug.LogError(Game02_Main.instance.gameLevel);
                    Game_Map02.instance.InitMap(Game02_Main.instance.gameLevel-20);
                }
               
                break;
        }
        if (Set.setVal.GameChoose==0)
        {
            if (Game00_Main.instance.gameLevel < 30)
            {
                for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
                {
                    if (GameLedControl.gamePoint[i].statue != enPointSta.Target && GameLedControl.gamePoint[i].statue != enPointSta.Rest)
                    {
                        GameLedControl.gamePoint[i].statue = enPointSta.None;
                    }

                }

            

            }
            else
            {
                DrawBianKuang(playerMode, bkWidth);
                SetRestPoint(levelSetting.picSetting.dataBuff);
                for (int i = 0; i < playerControl.Length && i < playerNum; i++)
                {
                    playerControl[i].RunStart(levelSetting.animSetting);
                }
            }
        }

    

    }

    public static void RunCheck()
    {
        if (isRunning == false)
            return;
        if (runTime < 0.02f)
        {
            runTime += Time.deltaTime;
            return;
        }
        runTime = 0;
        // 1步:
        Framebuffer.FullScreen(0, enPointSta.None);
        //FullScreen (0, enPointSta.None);
        //   DrawBianKuang (playerMode, bkWidth);
        DrawPresetPic();
        // 玩家花样
        switch (Set.setVal.GameChoose)
        {
            case 0:
                if (Game00_Main.instance.gameLevel >= 30)
                {
                    for (int i = 0; i < playerControl.Length && i < playerNum; i++)
                    {
                        playerControl[i].RunCheck();
                    }
                }
                break;
            case 1:
                break;
            case 2:
                playerControl[0].RunCheck();
                break;

        }
         CheckTaragePoint();
        ShowErrorAndDiedingPoint();


    }


    public static void RunCheck(bool isOver_Page1)
    {
        if (isRunning == false)
            return;
        if (runTime < 0.02f)
        {
            runTime += Time.deltaTime;
            return;
        }
        runTime = 0;
        // 1步:
        Framebuffer.FullScreen(0, enPointSta.None);
        //FullScreen (0, enPointSta.None);
        //   DrawBianKuang (playerMode, bkWidth);
        if (!isOver_Page1)
        {
            DrawPresetPic();

        }
        // 玩家花样
        switch (Set.setVal.GameChoose)
        {
            case 0:
                if (Game00_Main.instance.gameLevel >= 30)
                {
                    for (int i = 0; i < playerControl.Length && i < playerNum; i++)
                    {
                        playerControl[i].RunCheck();
                    }
                }
                break;
            case 1:
                break;
            case 2:
                playerControl[0].RunCheck();
                break;

        }
        CheckTaragePoint();
        ShowErrorAndDiedingPoint();


    }

    public static void FullScreen(uint color, enPointSta sta)
    {
        int len = Mathf.Min(gamePoint.Length, Set.setVal.Width * Set.setVal.Height);
        for (int i = 0; i < len; i++)
        {
            gamePoint[i].statue = sta;
        }
        Framebuffer.FullScreen(color, sta);
    }

    public static void DrawBianKuang(en_PlayerMode playerMode, int bkWidth)
    {
        int i;
        // 画边框         
        //for (i = 0; i < bkWidth; i++) {
        //    DrawPic.DrawRectangleKuang (i, i, Set.setVal.Width - i * 2, Set.setVal.Height - i * 2, 0x00fc00, enPointSta.Rest);
        //}
        // 画中线
        if (playerMode == en_PlayerMode.Free)
            return;
        int mw;
        if (Set.setVal.Width >= Set.setVal.Height)
        {
            if ((Set.setVal.Width % 2) == 0)
            {
                mw = 2;
            }
            else
            {
                mw = 1;
            }
            for (i = 0; i < mw; i++)
            {
                DrawPic.DrawCol((Set.setVal.Width - 1) / 2 + i, 0, Set.setVal.Height, 0x00fc00, enPointSta.Rest);
            }
        }
        else
        {
            if ((Set.setVal.Height % 2) == 0)
            {
                mw = 2;
            }
            else
            {
                mw = 1;
            }
            for (i = 0; i < mw; i++)
            {
                DrawPic.DrawRol(0, (Set.setVal.Height - 1) / 2 + i, Set.setVal.Width, 0x00fc00, enPointSta.Rest);
            }
        }
    }

    // ÊÕµ½ÉèÖÃ×¼±¸µã
    static void SetRestPoint(byte[] picDataBuf)
    {
        int i, j;
        int picId;
        int pointId;
        int pointId2;

        if (Set.setVal.Height < 40 && Set.setVal.Width < 40)
        {
            for (int y = 0; y < Set.setVal.Height && y < PresetPic.PIC_HEIGHT; y++)
            {

                for (int x = 0; x < Set.setVal.Width && x < PresetPic.PIC_WIDTH; x++)
                {
                    picId = y * PresetPic.PIC_WIDTH + x;
                    pointId = Framebuffer.tab_Mapping[y * Set.setVal.Width + x];
                    gamePoint[pointId].statue = (enPointSta)picDataBuf[picId];
                }
            }
        }
        if (Set.setVal.Height >= 40 || Set.setVal.Width >= 40)
        {
            for (int y = 40; y < Set.setVal.Height; y++)
            {

                for (int x = 0; x < Set.setVal.Width; x++)
                {
                    picId = y * PresetPic.PIC_WIDTH + x;
                    pointId = Framebuffer.tab_Mapping[y * Set.setVal.Width + x];
                    gamePoint[pointId].statue = (enPointSta)picDataBuf[picId - 40 * 40];
                }
            }
            for (int x = 40; x < Set.setVal.Width; x++)
            {

                for (int y = 0; y < Set.setVal.Height; y++)
                {
                    picId = y * PresetPic.PIC_WIDTH + x;
                    pointId = Framebuffer.tab_Mapping[y * Set.setVal.Width + x];
                    gamePoint[pointId].statue = (enPointSta)picDataBuf[picId - (x - 40 + y * Set.setVal.Width)];
                }
            }
        }

        // 玩家2对称图案：
        if (playerMode != en_PlayerMode.Free)
        {
            for (i = 0; i < playerControl[0].width; i++)
            {
                for (j = 0; j < playerControl[0].height; j++)
                {
                    pointId = (playerControl[0].startx + i) + (playerControl[0].starty + j) * Set.setVal.Width;
                    pointId = Framebuffer.tab_Mapping[pointId];
                    //pointId2 = (g_Player[1].x + i) + (g_Player[1].y + j) * g_Set.width;
                    if (Set.setVal.Width >= Set.setVal.Height)
                    {
                        pointId2 = (playerControl[1].startx + playerControl[0].width - 1 - i) + (playerControl[1].starty + j) * Set.setVal.Width;
                    }
                    else
                    {
                        pointId2 = (playerControl[1].startx + i) + (playerControl[1].starty + playerControl[1].height - 1 - j) * Set.setVal.Width;
                    }
                    pointId2 = Framebuffer.tab_Mapping[pointId2];
                    gamePoint[pointId2].statue = gamePoint[pointId].statue;
                }
            }
        }
    }
    // 画图案
    public static void DrawPresetPic()
    {
        //if (picSetting == null)
        //    return;
        int i;
        int len;
        len = Set.setVal.Width * Set.setVal.Height;
        if (len > Main.MAX_LED)
        {
            len = Main.MAX_LED;
        }

        for (i = 0; i < len; i++)
        {
            if (gamePoint[i].statue == enPointSta.Target)
            {
                DrawPic.DrawPointId(i, 0x0000fc, enPointSta.Target);
            }
            else if (gamePoint[i].statue == enPointSta.Rest)
            {
                if (gamePoint[i].Color != 0)
                {
                    DrawPic.DrawPointId(i, gamePoint[i].Color, enPointSta.Rest);
                }
                else
                {
                    DrawPic.DrawPointId(i, 0x00fc00, enPointSta.Rest);

                }
            }
            else if (gamePoint[i].statue == enPointSta.Die)
            {
                DrawPic.DrawPointId(i, 0xfc0000, enPointSta.Die);
            }
        }
    }
    static void CheckTaragePoint()
    {

        int len;
        len = Set.setVal.Width * Set.setVal.Height;
        if (len > Main.MAX_LED)
        {
            len = Main.MAX_LED;
        }
        for (int i = 0; i < len; i++)
        {
            if (gamePoint[i].tarageTime > 0)
            {
                gamePoint[i].tarageTime -= Time.deltaTime;
            }

        }
    }
    // 画闪烁点(报错灯和正在死亡的灯)
    static void ShowErrorAndDiedingPoint()
    {
        int i;
        int len;
        len = Set.setVal.Width * Set.setVal.Height;
        if (len > Main.MAX_LED)
        {
            len = Main.MAX_LED;
        }

        for (i = 0; i < len; i++)
        {
            // ÕýÔÚËÀÍöµÄµÆ
            if (gamePoint[i].statue == enPointSta.Dieing)
            {
                if (gamePoint[i].time > 0)
                {
                    gamePoint[i].time--;
                    if ((gamePoint[i].time % 10) < 5)
                    {
                        DrawPic.DrawPointId(i, 0xfc00fc, enPointSta.None);   // ×ÏÉ«
                    }
                }
                else
                {
                    gamePoint[i].statue = enPointSta.Die;
                    DrawPic.DrawPointId(i, 0xfc0000, enPointSta.Die);      // ºìÉ«
                }
                continue;
            }
            // ±¨´íµÄµÆ
            if (gamePoint[i].errorTime <= 0)
                continue;
            gamePoint[i].errorTime--;
            if ((gamePoint[i].errorTime % 10) < 5)
            {
                DrawPic.DrawPointId(i, 0xDBFF00, enPointSta.None); // °×É«
            }
        }
    }
}
