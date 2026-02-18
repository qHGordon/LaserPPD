using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>[Game07] 游戏地图控制器，实现 ILedMapCallback 供 LedAnim_Struts_07 解耦调用。</summary>
public class Game_Map07 : MonoBehaviour, ILedMapCallback
{


    public enum en_YouYu_sta
    {
        Idle,
        Playing,
        Checking,
        Check,
    }
    public readonly uint[] tab_PointColor = { 0x00a0f0, 0xff00ff, 0xffff00, 0x1145cd, 0xF44500 };// , 0xace0C, 0x5140B0,0xaC00CC, 0x1203BC,
    public readonly uint[] tab_PointColor_Swap = { 0x00f0a0, 0xffff00, 0xff00ff, 0x11cd45, 0xF40045 };
    readonly uint[] tab_ZadanColor = { 0xFFFF00, 0xFF0000, 0x0000FF };

    int picId = 0;
    int pointId = 0;
    [HideInInspector]
    public int snakeLength;
    [HideInInspector]
    public int tarageNum = 20;


    public int TarageNum_now = 0;

    public float SafeTime = 10f;

    int a = 0;//借用的随机数
    int[] area = new int[4];//借用的随机数,第八关
    int x, y;
    enPointSta[] map = new enPointSta[Set.setVal.Width * Set.setVal.Height];

    int step = 1;
    int Cnt;
    public float runTime = 0;
    float MaxrunTime = 0;
    float TankMoveTime = 0;
    float angly = 0;
    bool haveClear = false;//是否已经过场完成
    public bool isCleaning = false;//是否过场
    public bool isClearAll = false;//是否清理了所有的点
    public int remainPoint = 1;

    public int MaxJieDuan = 4;

    public static Game_Map07 instance;
    public int OldPoint = -1;
    public LedAnim_Struts_07[] ledSturts_Group;
    int taragecolor = 0;

    //镭射
    public static int rxKeyStartId;
    static int targetKeyId;
    static int wallKeyId;
    GameLeiSheLedControl ledControl = new GameLeiSheLedControl();
    float[] protectTime = new float[1000];
    int moveCnt;
    int runCnt;
    bool isLeiShe;//是否是镭射阶段
    public void ChangeLed_Sta(int ZX, int ZY, enPointSta sta)
    {
        picId = ZX + Set.setVal.Width * ZY;
        pointId = Framebuffer.tab_Mapping[picId];
        GameLedControl.gamePoint[pointId].statue = sta;
    }
    public void ChangeLed_Sta_Col(int ZX, int ZY, uint col, enPointSta sta)
    {
        picId = ZX + Set.setVal.Width * ZY;
        pointId = Framebuffer.tab_Mapping[picId];
        GameLedControl.gamePoint[pointId].statue = sta;
        GameLedControl.gamePoint[pointId].Color = col;
    }
    // Use this for initialization
    private void Awake()
    {
        instance = this;
        isClearAll = false;
    }
    void Start()
    {
        isClearAll = false;
    }
    public void Clear()
    {
        for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
        {
            GameLedControl.gamePoint[i].statue = enPointSta.None;
            DrawPic.DrawPointId(i, 0, enPointSta.None);
        }
        for (int i = 0; i < targetPos.Length; i++)
        {

            targetPos[i] = Vector2.one * -1;

        }
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            GameLedControl.gamePoint[i].Color = 0;
        }
    }
    void GetRandom_Tarage(int ch)
    {
        int cnt = 300;
        if (tarageNum > 0)
        {
            while (tarageNum > 0 && cnt > 0)
            {
                cnt--;
                randomX = Random.Range(0, Set.setVal.PPDWidth);
                randomY = Random.Range(0, Set.setVal.PPDHeight);
                a = randomX + randomY * Set.setVal.PPDWidth;
                a = Framebuffer.tab_Mapping[a];
                for (int i = 0; i < ch; i++)
                {
                    a += Set.ChannelLength[i];
                }
                if (GameLedControl.gamePoint[a].statue == enPointSta.None)
                {
                    GameLedControl.gamePoint[a].statue = enPointSta.Target;
                    tarageNum--;
                }
            }
        }
    }

    bool goDown = false;
    uint Colors = 0x00ff00;

    void TarageColorDown()
    {
        if (goDown)
        {
            Colors -= 0x000500;
            if (Colors <= 0)
            {
                Colors = 0;
                goDown = false;
            }
        }
        else
        {
            Colors += 0x000500;
            if (Colors >= 0x00ff00)
            {
                Colors = 0x00ff00;
                goDown = true;
            }
        }
    }


    public uint[] Led_Color;
    public void GetOtherLed(int ch)
    {
        for (int i = 0; i < Set.setVal.PPDWidth; i++)
        {
            for (int k = 0; k < Set.setVal.PPDHeight; k++)
            {
                picId = k * Set.setVal.PPDWidth + i;
                pointId = Framebuffer.tab_Mapping[picId];
                for (int m = 0; m < ch; m++)
                {
                    pointId += Set.ChannelLength[m];
                }
                if (GameLedControl.gamePoint[pointId].statue != enPointSta.Target)
                {
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                    int a = Random.Range(0, tab_PointColor.Length);
                    Led_Color[pointId] = tab_PointColor[a];
                }
            }
        }
    }
    void UpdateLed(int ch)
    {
        TarageColorDown();
        for (int i = 0; i < Set.setVal.PPDWidth; i++)
        {
            for (int k = 0; k < Set.setVal.PPDHeight; k++)
            {
                picId = k * Set.setVal.PPDWidth + i;
                pointId = Framebuffer.tab_Mapping[picId];
                for (int m = 0; m < ch; m++)
                {
                    pointId += Set.ChannelLength[ch];
                }
                if (GameLedControl.gamePoint[pointId].statue != enPointSta.Target)
                {
                    DrawPic.DrawPointId(pointId, Led_Color[pointId], GameLedControl.gamePoint[pointId].statue);
                }
                else
                {
                    DrawPic.DrawPointId(pointId, Colors, enPointSta.Target);

                }
            }
        }
    }
    float ShakeTime = 0;
    void UpdateLed_All()
    {

        ShakeTime += Time.deltaTime;
        if (ShakeTime > 0.3f)
        {
            ShakeTime = 0;
            goDown = !goDown;
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = k * Set.setVal.Width + i;
                pointId = Framebuffer.tab_Mapping[picId];
                if (OldPoint == pointId)
                {
                    if (goDown)
                    {
                        DrawPic.DrawPointId(pointId, Led_Color[pointId], enPointSta.Target);

                    }
                    else
                    {
                        DrawPic.DrawPointId(pointId, 0xff0000, enPointSta.Target);

                    }
                }
                else
                {

                    DrawPic.DrawPointId(pointId, Led_Color[pointId], GameLedControl.gamePoint[pointId].statue);

                }

            }
        }
    }


    void InitMap_00()
    {
        tarageNum = 20;
        if (Set.setVal.Width * Set.setVal.Height > 500)
        {
            tarageNum = 30;

        }

        if (Set.setVal.Width * Set.setVal.Height > 1000)
        {
            tarageNum = 50;

        }
        runCnt = 0;
        MaxJieDuan = 2;
        if (Game07_Main.instance.Index_JieDuan == 0)
        {
            GetRandom_Tarage(5);
            GetOtherLed(5);
        }
        else if (Game07_Main.instance.Index_JieDuan == 1 || Game07_Main.instance.Index_JieDuan == 3)
        {
            TarageNum_now = 10;//防止直接过关
            for (int i = 0; i < protectTime.Length; i++)
            {
                protectTime[i] = 2f - 0.02f * Set.setVal.LeiShe_LMD;
            }
            GameLeiSheBase.Update_ColorFull(0, enPointSta.None);
            moveCnt = Set.setVal.Width - 1;
            MusicManager.instance.Play_OpenLed();
            isLeiShe = true;
            ledControl.LedInit(Main.gameSetting.gameLevelSetting[Game07_Main.instance.gameLevel]);
        }
        else if (Game07_Main.instance.Index_JieDuan == 2)
        {
            GetRandom_Tarage(3);
            GetOtherLed(3);
        }
    }
    void InitMap_01()
    {
        MaxJieDuan = 2;


        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init(i, this);
            if (Game07_Main.instance.Index_JieDuan == 0)
            {
                ledSturts_Group[i].GetTarage(0);
            }
            else//todo
            {
                ledSturts_Group[i].GetTarage(3);
            }
        }
        //  GetOtherLed();
    }



    public int ShackCnt = 0;
    int Map02_tarage = 0;
    float ShackTime = 0;

    void InitMap_02()
    {
        MaxJieDuan = 2;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < 6; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init(i, this);
            //ledSturts_Group[i].GetPicType();
        }
        ledSturts_Group[0].GetPicType();

        Map02_tarage = Random.Range(0, 6);
        ShackCnt = 6;

    }
    void Run_Map02()
    {


        runTime -= Time.deltaTime;

        if (runTime <= 0)// Input.GetKeyDown(KeyCode.Q)
        {


            runTime = 2f;//和可设置的速度挂钩
            ClearMap();
            ShackCnt--;

            if (ShackCnt >= 0)
            {
                Clear();
                ledSturts_Group[ShackCnt].GetPicType();
            }
            else if (ShackCnt == -1)
            {
                ledSturts_Group[Map02_tarage].GetPicType();

            }
            else if (ShackCnt == -2)
            {
                //runTime = 0.3f;//和可设置的速度挂钩
                Clear();
                ledSturts_Group[Map02_tarage].GetTruePicType();
                if (Game07_Main.instance.Index_JieDuan == 0)
                {
                    GetOtherLed(0);
                }
                else//todo
                {
                    GetOtherLed(3);
                }
            }


        }
        if (ShackCnt < -2)
        {
            UpdateLed_All();
            // 
        }

        else

        {
            // 
            for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
            {
                if (GameLedControl.gamePoint[i].statue == enPointSta.Target)
                {
                    DrawPic.DrawPointId(i, Led_Color[i], enPointSta.Target);
                }
            }
        }

    }

    void InitMap_03()
    {
        MaxJieDuan = 2;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init(i, this);
            ledSturts_Group[i].GetTatagePos();
        }



        ShackCnt = 8;

    }
    void Run_Map03()
    {
        runTime -= Time.deltaTime;

        if (runTime <= 0)// Input.GetKeyDown(KeyCode.Q)
        {


            runTime = 1.5f;//和可设置的速度挂钩

            ShackCnt--;
            if (ShackCnt == 0)
            {
                if (Game07_Main.instance.Index_JieDuan == 0)
                {
                    GetOtherLed(0);
                }
                else//todo
                {
                    GetOtherLed(3);
                }
            }
            goDown = !goDown;

        }
        if (ShackCnt > 0)
        {
            for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
            {

                if (goDown)
                {
                    DrawPic.DrawPointId(i, Led_Color[i], enPointSta.Target);

                }
                else
                {
                    DrawPic.DrawPointId(i, 0, enPointSta.Target);

                }
            }



        }
        else
        {
            UpdateLed_All();

        }


    }
    void Run_Map07()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isStart = true;
            DianzuCnt_Gameing++;
        }
        if (DianzuCnt_Gameing < Pos_DianZu.Length)
        {
            if (LedKey.KeyStatus(Pos_DianZu[DianzuCnt_Gameing]))
            {
                isStart = true; DianzuCnt_Gameing++;
            }
        }

        for (int i = 0; i < targetPos.Length; i++)
        {
            DrawPic.DrawPoint((int)targetPos[i].x, (int)targetPos[i].y, 255, enPointSta.None);
        }
        for (int i = 0; i < Pos_DianZu.Length; i++)
        {
            if (Pos_DianZu[i] == 0)
            {
                continue;
            }
            if (i == 0)
            {
                DrawPic.DrawPointId(Pos_DianZu[i], 0xff00ff, enPointSta.None);

            }
            else
            {
                DrawPic.DrawPointId(Pos_DianZu[i], 0xff0000, enPointSta.None);

            }

        }
        if (isStart == false)
        {
            return;
        }
        runTime -= Time.deltaTime;

        if (Cnt_DianLiu > 0)
        {
            DrawPic.DrawPoint((int)targetPos[Cnt_DianLiu].x, (int)targetPos[Cnt_DianLiu].y, 0xff0000, enPointSta.None);
        }


        if (runTime < 0 && FjData.g_Fj[0].Life > 0)
        {
            runTime = 0.3f;

            if (Cnt_DianLiu > 0)
            {
                MusicManager.instance.Play_Correct();


                //  for (int i = 0; i < Cnt_DianZu; i++)
                {
                    //picId = (int)targetPos[i].x + Set.setVal.Width * (int)targetPos[i].y;
                    //pointId = Framebuffer.tab_Mapping[picId];
                    //Debug.LogError(pointId+"     "+Pos_DianZu[DianzuCnt_Gameing]);

                    picId = (int)targetPos[Cnt_DianLiu - 1].x + Set.setVal.Width * (int)targetPos[Cnt_DianLiu - 1].y;
                    pointId = Framebuffer.tab_Mapping[picId];
                    if (DianzuCnt_Gameing < Pos_DianZu.Length)
                    {
                        if (pointId == Pos_DianZu[DianzuCnt_Gameing])
                        {
                            if (!LedKey.KeyStatus(Pos_DianZu[DianzuCnt_Gameing]))
                            {
                                FjData.g_Fj[0].Life = 0;
                                return;
                            }
                            DianzuCnt_Gameing++;
                        }
                    }


                }

                Cnt_DianLiu--;
                if (Cnt_DianLiu == 0 && isStart)
                {
                    //    Debug.LogError("过关了!");
                    isClearAll = true;
                }
            }
        }



    }

    void Run_Map09()
    {
        UpdateLed_All();
        //过关后,需要闪烁几下
        if (IsOver_new09)
        {
            runTime += Time.deltaTime;

            for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
            {
                if (Led_Color[i] == 0x00ff00)
                {
                    if (ShackCnt % 2 == 0)
                    {
                        DrawPic.DrawPointId(i, 0, enPointSta.None);
                    }
                    else
                    {
                        DrawPic.DrawPointId(i, 0x00ff00, enPointSta.None);

                    }
                }
            }
            if (runTime > 0.6f)
            {
                runTime = 0;
                ShackCnt--;
                if (ShackCnt <= 0)
                {
                    isClearAll = true;
                }
            }
        }
    }
    void Run_Map08()
    {


        runTime -= Time.deltaTime;

        if (runTime <= 0)// Input.GetKeyDown(KeyCode.Q)
        {


            runTime = 2f;//和可设置的速度挂钩
            ClearMap();
            ShackCnt--;

            if (ShackCnt >= 0)
            {
                Clear();
                ledSturts_Group[ShackCnt].GetPicType();
            }
            else if (ShackCnt == -1)
            {
                ledSturts_Group[Map02_tarage].GetPicType();

            }
            else if (ShackCnt == -2)
            {
                Clear();
                ledSturts_Group[Map02_tarage].GetTruePicType();
                if (Game07_Main.instance.Index_JieDuan == 0)
                {
                    GetOtherLed(0);
                }
                else//todo
                {
                    GetOtherLed(3);
                }
            }


        }
        if (ShackCnt < -2)
        {
            UpdateLed_All();
            // 
        }

        else

        {
            // 
            for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
            {
                if (GameLedControl.gamePoint[i].statue == enPointSta.Target)
                {
                    DrawPic.DrawPointId(i, Led_Color[i], enPointSta.Target);
                }
            }
        }

    }
    void GetRandomTarage_different()
    {
        for (int i = 0; i < Set.setVal.Width / 3; i++)
        {
            for (int l = 0; l < Set.setVal.Height; l++)
            {
                picId = l * Set.setVal.Width + i;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
                int a = Random.Range(0, tab_PointColor.Length);
                Led_Color[pointId] = tab_PointColor[a];
                GameLedControl.gamePoint[pointId].Color = tab_PointColor[a];
                {
                    picId = l * Set.setVal.Width + i + Set.setVal.Width / 3 * 2;
                    pointId = Framebuffer.tab_Mapping[picId];
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                    Led_Color[pointId] = tab_PointColor[a];
                }

            }
        }
        while (tarageNum > 0)
        {

            randomX = Random.Range(Set.setVal.Width / 3 * 2, Set.setVal.Width);
            randomY = Random.Range(0, Set.setVal.Height);
            a = randomX + randomY * Set.setVal.Width;
            a = Framebuffer.tab_Mapping[a];
            if (GameLedControl.gamePoint[a].statue == enPointSta.Die)
            {
                int m = Random.Range(0, tab_PointColor.Length);
                while (Led_Color[a] == tab_PointColor[m])
                {
                    m = Random.Range(0, tab_PointColor.Length);
                }
                if (Led_Color[a] != tab_PointColor[m])
                {
                    Led_Color[a] = tab_PointColor[m];
                    GameLedControl.gamePoint[a].statue = enPointSta.Target;
                    //Debug.LogError("目标点" + tarageNum);
                    tarageNum--;
                }


            }
        }
    }
    void GetRandom_Tarage_Same()
    {
        int cnt = 300;
        taragecolor = Random.Range(0, tab_PointColor.Length);
        for (int i = 0; i < Set.setVal.Height; i++)
        {
            picId = i * Set.setVal.Width + 0;
            pointId = Framebuffer.tab_Mapping[picId];
            Led_Color[pointId] = tab_PointColor[taragecolor];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            GameLedControl.gamePoint[pointId].Color = tab_PointColor[taragecolor];
        }

        if (tarageNum > 0)
        {
            while (tarageNum > 0 && cnt > 0)//
            {
                cnt--;
                randomX = Random.Range(0, Set.setVal.Width);
                randomY = Random.Range(1, Set.setVal.Height);
                a = randomX + randomY * Set.setVal.Width;
                a = Framebuffer.tab_Mapping[a];
                if (GameLedControl.gamePoint[a].statue == enPointSta.None)
                {
                    Led_Color[a] = tab_PointColor[taragecolor];
                    GameLedControl.gamePoint[a].statue = enPointSta.Target;
                    tarageNum--;
                }

            }
        }

    }
    void GetRandomDie_Same()
    {
        for (int m = 0; m < Set.setVal.Height; m++)
        {
            for (int n = 1; n < Set.setVal.Width; n++)
            {
                picId = m * Set.setVal.Width + n;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                    continue;
                int k = Random.Range(0, tab_PointColor.Length);
                while (k == taragecolor)
                {
                    k = Random.Range(0, tab_PointColor.Length);
                }
                Led_Color[pointId] = tab_PointColor[k];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;

            }
        }
    }


    void InitMap_04()
    {
        MaxJieDuan = 2;
        tarageNum = 8;
        if (Set.setVal.Width * Set.setVal.Height > 500)
        {
            tarageNum = 30;

        }

        if (Set.setVal.Width * Set.setVal.Height > 1000)
        {
            tarageNum = 50;

        }

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        GetRandomTarage_different();
        //  GetOtherLed();
    }

    void InitMap_05()
    {
        MaxJieDuan = 1;
        tarageNum = 10;

        if (Set.setVal.Width * Set.setVal.Height > 500)
        {
            tarageNum = 20;

        }

        if (Set.setVal.Width * Set.setVal.Height > 600)
        {
            tarageNum = 30;

        }
        if (Set.setVal.Width * Set.setVal.Height > 800)
        {
            tarageNum = 40;

        }

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        GetRandom_Tarage_Same();
        GetRandomDie_Same();
    }
    void GetRandom_Tarage_zadan()
    {
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int l = 0; l < Set.setVal.Height; l++)
            {
                picId = l * Set.setVal.Width + i;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Target;

            }
        }
        int cnt = 300;
        if (tarageNum > 0)
        {
            while (tarageNum > 0 && cnt > 0)//
            {
                cnt--;

                randomX = Random.Range(0, Set.setVal.Width - 1);
                randomY = Random.Range(1, Set.setVal.Height);
                a = randomX + randomY * Set.setVal.Width;
                a = Framebuffer.tab_Mapping[a];
                if (GameLedControl.gamePoint[a].statue == enPointSta.Target)
                {
                    GameLedControl.gamePoint[a].statue = enPointSta.Rest;
                    tarageNum--;

                }

            }
        }

    }
    bool isStart = false;
    public int[] Pos_DianZu;
    public int Cnt_DianZu = 0;
    public int Cnt_DianLiu = 0;
    int DianzuCnt_Gameing = 0;
    public bool startBooming = false;
    void InitMap_07()
    {
        startBooming = false;
        MaxJieDuan = 2;
        DianzuCnt_Gameing = 0;
        zadashu = 6;
        zadanzuobiao = 0;
        Cnt_DianZu = 0;
        isStart = false;
        isClearAll = false;
        remainPoint = tarageNum;

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        ledSturts_Group[0].enabled = true;
        for (int i = 0; i < Led_Color.Length; i++)
        {
            Led_Color[i] = 0;
        }
        switch (Game07_Main.instance.Index_JieDuan)
        {
            default:
            case 0:
                Pos_DianZu = new int[3];
                if (Set.setVal.Width * Set.setVal.Height > 600)
                {
                    Pos_DianZu = new int[4];

                }



                ledSturts_Group[0].Init_Game06_V();
                //    ledSturts_Group[0].Init_Game06_WenHao();

                break;
            case 1:
                Pos_DianZu = new int[4];

                if (Set.setVal.Width * Set.setVal.Height > 400)
                {
                    Pos_DianZu = new int[5];

                }
                if (Set.setVal.Width * Set.setVal.Height > 600)
                {
                    Pos_DianZu = new int[6];

                }
                if (Set.setVal.Width * Set.setVal.Height > 800)
                {
                    Pos_DianZu = new int[7];

                }
                ledSturts_Group[0].Init_Game06_WenHao();

                break;
            case 2:
                Pos_DianZu = new int[5];


                if (Set.setVal.Width * Set.setVal.Height > 400)
                {
                    Pos_DianZu = new int[6];

                }
                if (Set.setVal.Width * Set.setVal.Height > 600)
                {
                    Pos_DianZu = new int[7];

                }
                ledSturts_Group[0].Init_Game06_L();

                break;
        }

        isStart = false;
    }

    void InitMap_08()
    {
        MaxJieDuan = 1;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < 6; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init(i, this);
            //ledSturts_Group[i].GetPicType();
        }
        ledSturts_Group[0].GetPicType();

        Map02_tarage = Random.Range(0, 6);
        ShackCnt = 6;
    }
    public bool isPass = false;
    void InitMap_09()
    {
        MaxJieDuan = 1;
        tarageNum = 8;
        zadashu = 6;
        startBooming = false;
        remainPoint = tarageNum;
        if (Set.setVal.Width * Set.setVal.Height > 500)
        {
            tarageNum = 18;
            zadashu = 14;
        }

        if (Set.setVal.Width * Set.setVal.Height > 1000)
        {
            tarageNum = 30;
            zadashu = 25;
        }

        isDetonate = false;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        zadanx = new int[zadashu];
        zadany = new int[zadashu];
        zadanzuobiao = 0;
        GetRandom_Tarage_zadan();
    }

    //第十关变量
    [HideInInspector]
    public bool isDetonate = false;
    [HideInInspector]
    public int zadashu;
    public int zadanzuobiao = 0;
    public int[] zadanx;
    public int[] zadany;

    int stratPos = 0;
    public int EndPos = 0;
    public bool IsOver_new09 = false;
    void InitMap_new09()
    {
        MaxJieDuan = 1;
        IsOver_new09 = false;
        isPass = false;
        ShackCnt = 7;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        x = Random.Range(0, Set.setVal.Width);
        y = Random.Range(0, Set.setVal.Height);
        picId = x + Set.setVal.Width * y;
        stratPos = Framebuffer.tab_Mapping[picId];



        int x1 = Random.Range(2, Set.setVal.Width - 2);
        int y1 = Random.Range(2, Set.setVal.Height - 2);
        picId = x1 + Set.setVal.Width * y1;
        EndPos = Framebuffer.tab_Mapping[picId];
        while (stratPos == EndPos)
        {
            x1 = Random.Range(2, Set.setVal.Width - 2);
            y1 = Random.Range(2, Set.setVal.Height - 2);
            picId = x1 + Set.setVal.Width * y1;
        }

        GameLedControl.gamePoint[stratPos].statue = enPointSta.Rest;
        GameLedControl.gamePoint[EndPos].statue = enPointSta.None;
        Led_Color[stratPos] = 0x00F5FF;
        Led_Color[EndPos] = 0xFF002E;
        GameLedControl.gamePoint[stratPos].Color = 0x00F5FF;

        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init(i, this);
            ledSturts_Group[i].GetTarage_new09();
        }
        if (x + 1 < Set.setVal.Width - 1)
        {
            picId = x + 1 + Set.setVal.Width * y;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Target;

        }
        if (x - 1 > 0)
        {
            picId = x - 1 + Set.setVal.Width * y;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
        }
        if (y + 1 < Set.setVal.Height - 1)
        {
            picId = x + Set.setVal.Width * (y + 1);
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
        }
        if (y - 1 > 0)
        {
            picId = x + Set.setVal.Width * (y - 1);
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
        }
        //for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
        //{
        //    if (GameLedControl.gamePoint[i].statue == enPointSta.None)
        //    {
        //        GameLedControl.gamePoint[i].statue = enPointSta.Target;
        //        Led_Color[i] = 0;


        //    }
        //}
    }
    void GetRandomBule()
    {
        int cnt = 300;
        if (tarageNum > 0)
        {
            while (tarageNum > 0 && cnt > 0)//
            {
                cnt--;

                randomX = Random.Range(0, Set.setVal.Width);
                randomY = Random.Range(1, Set.setVal.Height);
                a = randomX + randomY * Set.setVal.Width;
                a = Framebuffer.tab_Mapping[a];
                if (GameLedControl.gamePoint[a].statue == enPointSta.None)
                {
                    GameLedControl.gamePoint[a].statue = enPointSta.Rest;
                    tarageNum--;

                }



            }
        }

    }
    void ClearMap()
    {
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            if (GameLedControl.gamePoint[i].statue != enPointSta.Target && GameLedControl.gamePoint[i].statue != enPointSta.Rest)
            {
                GameLedControl.gamePoint[i].statue = enPointSta.None;
            }

        }
    }
    void ClearMap_Tarage()
    {
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            if (GameLedControl.gamePoint[i].statue == enPointSta.Target)
            {
                GameLedControl.gamePoint[i].statue = enPointSta.None;
            }

        }
    }
    void Check_TarageNum()
    {
        TarageNum_now = 0;
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            if (GameLedControl.gamePoint[i].statue == enPointSta.Target)
            {
                TarageNum_now++;


            }

        }





        int cnt = 100;
        if (tarageNum > 0)
        {
            while (tarageNum > 0 && cnt > 0)
            {
                cnt--;

                randomX = Random.Range(1, Set.setVal.Width);
                randomY = Random.Range(1, Set.setVal.Height);
                a = randomX + randomY * Set.setVal.Width;
                a = Framebuffer.tab_Mapping[a];
                if (GameLedControl.gamePoint[a].statue == enPointSta.None)
                {


                    GameLedControl.gamePoint[a].statue = enPointSta.Target;


                    tarageNum--;

                }



            }
        }

    }
    void Get_NowTarage(int ch)
    {
        MaxrunTime = 0.5f - 0.03f * Game07_Main.instance.Index_JieDuan;
        if (Game07_Main.instance.Index_JieDuan >= 0)
        {
            if (MaxrunTime <= 0.05f)
            {
                MaxrunTime = 0.05f;
            }
        }
        //   tarageNum = Game06_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        TarageNum_now = 0;
        for (int i = 0; i < Set.setVal.PPDWidth; i++)
        {
            for (int k = 0; k < Set.setVal.PPDHeight; k++)
            {
                picId = i + k * Set.setVal.PPDWidth;
                pointId = Framebuffer.tab_Mapping[picId];
                for (int m = 0; m < ch; m++)
                {
                    pointId += Set.ChannelLength[ch];
                }
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {
                    TarageNum_now++;
                }

            }
        }
#if UNITY_EDITOR
        //        Debug.LogError(" 目标点 " + TarageNum_now);
#endif


    }
    int randomX, randomY = 0;
    int yy_Num = 0;


    void DrawSafePlace(int num)
    {
        int pointId;
        if (num == 1)
        {
            for (int n = 0; n < Set.setVal.Width; n++)
            {
                pointId = Framebuffer.tab_Mapping[n];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;

                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * Set.setVal.Height - 1 - n];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;

            }

            for (int n = 0; n < Set.setVal.Height; n++)
            {
                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * n];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;


                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * (n + 1) - 1];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;

            }
        }


        if (num == 2)
        {
            for (int n = 0; n < Set.setVal.Width * 2; n++)
            {
                pointId = Framebuffer.tab_Mapping[n];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;


                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * Set.setVal.Height - 1 - n];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;

            }
            for (int n = 0; n < Set.setVal.Height; n++)
            {
                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * n];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;

                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * n + 1];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;


                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * (n + 1) - 1];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
                pointId = Framebuffer.tab_Mapping[Set.setVal.Width * (n + 1) - 2];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;

            }
        }
    }
    float waitTime = 2;
    public AudioSource audioSource_Others;
    public AudioClip[] Rules;

    public void PlayRule()
    {
        if(Set.setVal.Language != 1)//英文情况下不播放
        {
            audioSource_Others.Stop();
            audioSource_Others.clip = Rules[Main.MapIndex];
            audioSource_Others.Play();
        }
    }
    public void InitMap(int ID)
    {
        //镭射
        rxKeyStartId = LedKey.GetLeiSheKeyStartId();
        targetKeyId = LedKey.GetLeiSheTargetKeyId();
        wallKeyId = LedKey.GetLeiSheWallLedKeyId();
        isLeiShe = false;
        //拍拍灯
        Led_Color = new uint[Main.MAX_LED];
        waitTime = 2f;
        ShackCnt = 6;
        OldPoint = -1;
        Clear();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        switch (ID)
        {
            default:
                break;
            case 0:
                InitMap_00();
                break;
            case 1:
                InitMap_01();
                break;
            case 2:
                InitMap_02();
                break;
            case 3:
                InitMap_03();
                break;
            case 4:
                InitMap_04();
                break;
            case 5:
                InitMap_05();
                break;
            case 6:
                InitMap_09();
                break;
            case 7:
                InitMap_07();
                break;
            case 8:
                InitMap_08();
                break;
            case 9:
                InitMap_new09();
                break;
        }

    }

    public void Run_ChangeMap_Red(int _x, int _y, int r)//转场,绿色圆环或者红色
    {
        int picid = 0;
        float dis;
        for (int i = _x - r; i <= _x + r; i++)
        {
            for (int j = _y - r; j <= _y + r; j++)
            {

                dis = Mathf.Abs(Vector2.Distance(new Vector2(i, j), new Vector2(_x, _y)));


                if (dis <= r) //dis >= r - w &&
                {

                    //  Vector2 pos = new Vector2(_x + j * widthOne, _y + i * widthOne);

                    if (i >= 0 && j >= 0)
                    {
                        if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                        {
                            Vector2 pos = new Vector2(i, j);
                            picid = (int)pos.x + Set.setVal.Width * (int)pos.y;
                            if (picid < Set.setVal.Width * Set.setVal.Height)
                            {
                                //  DrawPic.DrawPoint((int)pos.x, (int)pos.y, 0xff0000, enPointSta.Die);
                                //   GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].statue = enPointSta.Die;
                                if (r > 2)
                                {
                                    if (dis > r * (r - (Game07_Main.instance.Index_JieDuan + 1)) / r)
                                    {
                                        DrawPic.DrawRol((int)pos.x, (int)pos.y, 1, 0xff0000, enPointSta.Die);

                                        //  DrawPic.DrawPoint((int)pos.x, (int)pos.y, 0, enPointSta.None);
                                        //   GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].statue = enPointSta.None;

                                    }
                                }


                            }

                        }

                    }

                }
            }
        }

    }
    public void Run_ChangeMap_Green(int _x, int _y, int r, int w)//转场,绿色圆环或者红色
    {

        int picid = 0;

        float dis;
        for (int i = _x - r; i <= _x + r; i++)
        {
            for (int j = _y - r; j <= _y + r; j++)
            {

                dis = Mathf.Abs(Vector2.Distance(new Vector2(i, j), new Vector2(_x, _y)));

                if (dis <= r) //dis >= r - w &&
                {

                    //  Vector2 pos = new Vector2(_x + j * widthOne, _y + i * widthOne);

                    if (i >= 0 && j >= 0)
                    {
                        if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                        {

                            picid = i + Set.setVal.Width * j;
                            if (picid < Set.setVal.Width * Set.setVal.Height)
                            {
                                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].statue = enPointSta.Rest;
                                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].Color = 0;
                            }


                        }

                    }

                }
            }
        }
    }
    public void Run_ChangeMap_Clor(int _x, int _y, int r, int Clor)//红蓝，黄，紫
    {

        int picid = 0;

        float dis;
        for (int i = _x - r; i <= _x + r; i++)
        {
            for (int j = _y - r; j <= _y + r; j++)
            {

                dis = Mathf.Abs(Vector2.Distance(new Vector2(i, j), new Vector2(_x, _y)));

                if (dis <= r) //dis >= r - w &&
                {

                    //  Vector2 pos = new Vector2(_x + j * widthOne, _y + i * widthOne);

                    if (i >= 0 && j >= 0)
                    {
                        if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                        {

                            picid = i + Set.setVal.Width * j;
                            pointId = Framebuffer.tab_Mapping[picid];
                            if (picid < Set.setVal.Width * Set.setVal.Height)
                            {
                                switch (Clor)
                                {
                                    case 0:
                                        DrawPic.DrawRol(i, j, 1, 0x00FF00, enPointSta.Rest);//绿
                                        GameLedControl.gamePoint[pointId].Color = 0x00FF00;
                                        break;
                                    case 1:
                                        DrawPic.DrawRol(i, j, 1, 0x0000FF, enPointSta.Rest);//蓝
                                        GameLedControl.gamePoint[pointId].Color = 0x0000FF;

                                        break;
                                    case 2:
                                        DrawPic.DrawRol(i, j, 1, 0xFffA00, enPointSta.Rest);//黄
                                        GameLedControl.gamePoint[pointId].Color = 0xFffA00;

                                        break;
                                    case 3:
                                        DrawPic.DrawRol(i, j, 1, 0xFF00ff, enPointSta.Rest);//紫
                                        GameLedControl.gamePoint[pointId].Color = 0xFF00ff;

                                        break;
                                    case 4:
                                        DrawPic.DrawRol(i, j, 1, 0xFF6300, enPointSta.Rest);//橙色
                                        GameLedControl.gamePoint[pointId].Color = 0xFF6300;

                                        break;
                                    case 5:
                                        DrawPic.DrawRol(i, j, 1, 0xFF0000, enPointSta.Rest);//
                                        GameLedControl.gamePoint[pointId].Color = 0xFF0000;

                                        break;

                                }
                            }


                        }

                    }

                }
            }
        }
    }
    public void Run_ChangeMap_None(int _x, int _y, int r, int w)//转场,绿色圆环或者红色
    {

        int picid = 0;

        float dis;
        for (int i = _x - r; i <= _x + r; i++)
        {
            for (int j = _y - r; j <= _y + r; j++)
            {
                dis = Mathf.Abs(Vector2.Distance(new Vector2(i, j), new Vector2(_x, _y)));

                if (dis <= r) //dis >= r - w &&
                {

                    //  Vector2 pos = new Vector2(_x + j * widthOne, _y + i * widthOne);

                    if (i >= 0 && j >= 0)
                    {
                        if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                        {
                            Vector2 pos = new Vector2(i, j);
                            picid = (int)pos.x + Set.setVal.Width * (int)pos.y;
                            if (picid < Set.setVal.Width * Set.setVal.Height) { GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].statue = enPointSta.None; }

                        }

                    }

                }
            }
        }
    }
    public bool Run_MapClear()
    {

        runTime -= Time.deltaTime;
#if UNITY_EDITOR
        //        Debug.LogError("正在清理");
#endif
        if (runTime <= 0)
        {
            runTime = 0.02f;
            if (Set.setVal.Width > Set.setVal.Height)
            {
                if (haveClear)
                {
                    x++;
                    Run_ChangeMap_None(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2 - 1, x, 2);
                    if (x >= Set.setVal.Width)
                    {
                        haveClear = false;
                        isCleaning = false;
                        x = 0;

                        return true;
                    }
                }
                else
                {
                    x++;
                    Run_ChangeMap_Green(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2 - 1, x, 2);
                    if (x >= Set.setVal.Width)
                    {
                        haveClear = true;
                        x = 0;
                    }
                }
            }
            else
            {
                if (haveClear)
                {
                    x++;
                    Run_ChangeMap_None(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2 - 1, x, 2);
                    if (x >= Set.setVal.Height)
                    {
                        haveClear = false;
                        isCleaning = false;
                        x = 0;

                        return true;
                    }
                }
                else
                {
                    x++;
                    Run_ChangeMap_Green(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2 - 1, x, 2);
                    if (x >= Set.setVal.Height)
                    {
                        haveClear = true;
                        x = 0;
                    }
                }
            }



        }
        ClearMap();
        return false;

    }
    void CheckRxKey()
    {

        int len = Set.setVal.Width * Set.setVal.Height;


        for (int i = 0; i < len && i < GameLeiSheBase.gamePoint.Length; i++)
        {

            if (GameLeiSheBase.gamePoint[i].bindCnt > 0)
                continue;
            if (Framebuffer.led[i].colorOld == 0)
            { continue; }
            int JieShou_ID = i;
            if (Framebuffer.isNewLeiShe)
            { JieShou_ID = GameLeiSheBase.tab_Point[i]; }

            if (GameLeiSheLedControl.PrePic_Copy[i] == 0)
            {
                continue;
            }
            else
            {

                //Debug.LogError("要扣血 " + i + " " + JieShou_ID);//


            }



            if (Framebuffer.led[i].waitTime > 0)
                continue;

            if (Framebuffer.isNewLeiShe)
            {
                if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID))
                {
                    continue;
                }
            }
            else
            {
                if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID))
                {
                    continue;
                }
            }
            if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID) == false && runCnt == 1)
            {
                protectTime[JieShou_ID] -= Time.deltaTime;
                if (protectTime[JieShou_ID] <= 0)
                {
                    //                    Debug.LogError("扣血灯 " + i + " " + JieShou_ID);//
                    if (FjData.g_Fj[0].Life > 0)
                    {
                        //FjData.g_Fj[0].Life--;
                        protectTime[JieShou_ID] = 2f - 0.02f * Set.setVal.LeiShe_LMD;

                        GameLeiSheBase.gamePoint[i].bindCnt = 10;
                        MusicManager.instance.Play_Fails();
                    }
                }

            }
            else
            {
                protectTime[JieShou_ID] = 2f - 0.02f * Set.setVal.LeiShe_LMD;
            }
        }
    }
    static float targetLedTime = 0;
    static uint targetLedSta = 0;

    public static void TargetLedOut(uint color)
    {
        Framebuffer.Update_TargetLedColor(1 + Set.setVal.WallLedNum, color);
    }
    public void TargetLed_Run()
    {
        targetLedTime += Time.deltaTime;
        if (targetLedTime >= 0.5f)
        {
            targetLedTime = 0;
            if (targetLedSta == 0)
            {
                targetLedSta = 0xa0a0a0;
            }
            else
            {
                targetLedSta = 0;
            }
            //GameLeiSheBase.Update_PointColor (2, Set.setVal.TargetLedNum, targetLedSta, enPointSta.None);            
            TargetLedOut(targetLedSta);
        }
    }
    public void RunLedAnim()
    {
        ledControl.Run();
    }
    void Run_Map00()
    {
        if (Game07_Main.instance.Index_JieDuan == 0)
        {
            UpdateLed(5);
        }
        else if (Game07_Main.instance.Index_JieDuan == 1 || Game07_Main.instance.Index_JieDuan == 3)
        {
            if(runCnt == 0)
            {
                CheckRxKey();
                runTime += Time.deltaTime;
                if (runTime >= 0.03f)
                {
                    runTime = 0;
                    if (moveCnt >= 0)
                    {
                        for (int i = 0; i < Set.setVal.Height; i++)
                        {
                            if (ledControl.GetPicPointValue(moveCnt, i) == 0)
                            {
                                GameLeiSheBase.Update_PointColor(moveCnt, i, 0, enPointSta.None);
                            }
                            else
                            {
                                GameLeiSheBase.Update_PointColor(moveCnt, i, GameLeiSheLedControl.LEISHE_COLOR, enPointSta.Die);
                            }
                        }
                        moveCnt--;
                    }
                    else
                    {
                        runCnt = 1;
                        //result = 0; // 未出结果            
                        ledControl.RunStart();
                        Framebuffer.ClearDelayBuf();
                        LedKey.Clear();
                    }
                }
            }
            else if (runCnt == 1)
            {
                TargetLed_Run();

#if UNITY_EDITOR //&& false
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    MusicManager.instance.Play_Correct();
                    runCnt = 2;
                }

#endif
                RunLedAnim();
                if (LedKey.KeyPressed(targetKeyId) || Input.GetKeyDown(KeyCode.Q))
                {
                    MusicManager.instance.Play_Correct();
                    runCnt = 2;
                    FjData.g_Fj[0].LevelTime = (int)Game07_Main.instance.remainTime;
                }
                //CheckRxKey();
            }
            else if (runCnt == 2)
            {
                runTime += Time.deltaTime;
                if (runCnt < Set.setVal.Width)
                {
                    if (runTime >= 0.03f)
                    {
                        runTime = 0;
                        for (int i = 0; i < Set.setVal.Height; i++)
                        {
                            GameLeiSheBase.Update_PointColor(runCnt, i, 0, enPointSta.None);
                        }
                        runCnt++;
                    }
                }
                else if (runTime > 0.5f)
                {
                    runCnt = 3;
                    GameLeiSheBase.Update_ColorFull(0, enPointSta.None);
                    //
                    MusicManager.instance.PlayOne(Game07_Main.instance.audioClip_Pass, 0);
                    MusicManager.instance.Play_Talk(1, 1.2f); // "恭喜过关"
                    FjData.g_Fj[0].LevelTime = (int)Game07_Main.instance.remainTime;
                    TarageNum_now = 0;
                    isClearAll = true;
                }
            }
        }
        else if (Game07_Main.instance.Index_JieDuan == 2)
        {
            UpdateLed(3);
        }
    }
    void Run_Map01()
    {
        UpdateLed_All();
        runTime -= Time.deltaTime;
        if (runTime <= 0)
        {

            runTime = MaxrunTime;//和可设置的速度挂钩


        }
    }


    void Run_Map04()
    {
        runTime -= Time.deltaTime;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {

            if (ledSturts_Group[0].enabled)
            {
                DrawPic.DrawRol(ledSturts_Group[0].x, ledSturts_Group[0].y, Set.setVal.Width, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(ledSturts_Group[1].x, ledSturts_Group[1].y, Set.setVal.Height, 0xff0000, enPointSta.Die);
                if (runTime <= 0)
                {
                    if (ledSturts_Group[0].dir == 0)
                    {
                        if (ledSturts_Group[0].y < Set.setVal.Height)
                        {
                            ledSturts_Group[0].y++;
                        }
                        else
                        {
                            ledSturts_Group[0].dir = 1;
                        }

                    }
                    else
                    {
                        if (ledSturts_Group[0].y > 0)
                        {
                            ledSturts_Group[i].y--;
                        }
                        else
                        {

                            ledSturts_Group[0].dir = 0;
                        }


                    }
                    if (ledSturts_Group[1].dir == 0)
                    {

                        if (ledSturts_Group[1].x < Set.setVal.Width)
                        {

                            ledSturts_Group[1].x++;
                        }
                        else
                        {

                            ledSturts_Group[1].dir = 1;
                        }

                    }
                    else
                    {
                        if (ledSturts_Group[1].x > 0)
                        {

                            ledSturts_Group[1].x--;
                        }
                        else
                        {

                            ledSturts_Group[1].dir = 0;
                        }
                    }


                    runTime = MaxrunTime;//和可设置的速度挂钩



                }
            }
        }



    }

    void Run_Map05()
    {
        runTime -= Time.deltaTime;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {



            if (runTime <= 0)
            {
                if (ledSturts_Group[0].dir == 0)
                {
                    if (ledSturts_Group[0].x < Set.setVal.Width / 2 + 1)
                    {
                        ledSturts_Group[0].x++;
                    }
                    else
                    {
                        ledSturts_Group[0].dir = 1;
                    }

                }
                else
                {
                    if (ledSturts_Group[0].x > 0)
                    {
                        ledSturts_Group[i].x--;
                    }
                    else
                    {

                        ledSturts_Group[0].dir = 0;
                    }


                }


                runTime = MaxrunTime;//和可设置的速度挂钩



            }


            for (int k = 0; k < ledSturts_Group[0].x; k++)
            {

                DrawPic.DrawCol(k, ledSturts_Group[0].y, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width - k, ledSturts_Group[0].y, Set.setVal.Height, 0xff0000, enPointSta.Die);

            }

        }

        ClearMap();
    }

    void Update()
    {


        if (Game07_Main.instance == null)
        {

            return;
        }

        if (Game07_Main.instance.statue != en_Game00_Sta.Play)
        {


            return;
        }

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            Framebuffer.Update_ColorFull(0, enPointSta.None);


            return;
        }



        if (isCleaning)
        {

            return;
        }
        if(Game07_Main.instance.Index_JieDuan == 0)
        {
            Get_NowTarage(1);
        }
        else if (Game07_Main.instance.Index_JieDuan == 2)
        {
            Get_NowTarage(3);
        }
        switch (Main.MapIndex)
        {
            default:
                break;
            case 0:
                Run_Map00();
                break;
            case 1:
                Run_Map01();
                break;
            case 2:
                Run_Map02();
                break;
            case 3:
                Run_Map03();
                break;
            case 4:
                UpdateLed_different();
                break;
            case 5:
                UpdateLed_different();
                break;
            case 6:
                UpdateLed_zadan();
                Update_Zadan();
                break;
            case 7:

                Run_Map07();
                break;

            case 8:
                Run_Map08();
                break;
            case 9:
                Run_Map09();

                break;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            TarageNum_now = 0;
            isClearAll = true;
        }
        if (Game07_Main.instance.isClearTarage && Game07_Main.instance.statue == en_Game00_Sta.Play)
        {
            if (TarageNum_now <= 0 /*&& Main.MapIndex != 2*/ && Main.MapIndex != 7)
            {
#if UNITY_EDITOR
                Debug.LogError("没有目标点了");
#endif
                isClearAll = true;
                return;
            }
        }
#if UNITY_EDITOR

        if (!Game07_Main.instance.isClearTarage)
        {
            Debug.Log("不要求全清理");

        }
        else
        {
            Debug.Log("要求全清理");
        }
#endif

    }


    int AiXinHigh, AiXinHighCnt, TarPosCnt = 0;
    public Vector2[] targetPos = new Vector2[500];
    public bool[] bool_HaveFire = new bool[500];

    void GetAiXin_5_10(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        int midX = Set.setVal.Width / 2;
        if (midX < 3)
        {
            midX = 3;
        }
        for (int x = 0; x < midX; x++)
        {
            if (x >= 1)
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }
            else
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }

            AiXinHighCnt++;

            int high = 4;


            if (x >= 2)

            {
                high = 3;
            }
            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)


            {
                int xx = (x + Set.setVal.Width / 2);

                picId = xx + y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }
    void GetAiXin_10_15(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        for (int x = 0; x < Set.setVal.Width / 2; x++)
        {
            if (x >= 0)
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }
            else
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }

            AiXinHighCnt++;

            int high = 5;
            switch (x)
            {
                default:
                    high = 0;
                    break;
                case 0:
                    high = 5;
                    break;
                case 1:
                    high = 5;
                    break;
                case 2:
                    high = 5;
                    break;
                case 3:
                    high = 4;
                    break;
                case 4:
                    high = 3;
                    break;
                    //case 5:
                    //    high = 1;
                    //    break;
                    //case 6:
                    //    high = 1;
                    //    break;
            }



            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)
            {
                int xx = (x + Set.setVal.Width / 2);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];


                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }

    void GetAiXin_Over15(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        for (int x = 0; x < Set.setVal.Width / 2; x++)
        {


            if (AiXinHighCnt >= 1)
            {
                AiXinHighCnt = 0;
                AiXinHigh++;
            }


            AiXinHighCnt++;

            int high = 5;
            if (x == 2 || x == 0)
            {
                high = 4;
            }
            else if (x >= 6)

            {
                high = 0;
            }
            else if (x >= 5)

            {
                high = 1;
            }
            else if (x >= 4)

            {
                high = 3;
            }
            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)

            {
                int xx = (x + Set.setVal.Width / 2);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }
    void GetAiXin_Over20(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        for (int x = 0; x < Set.setVal.Width / 2; x++)
        {


            if (AiXinHighCnt >= 1)
            {
                AiXinHighCnt = 0;
                AiXinHigh++;
            }


            AiXinHighCnt++;

            int high = 7;
            if (x == 2)
            {
                high = 10;
            }
            else if (x == 0)
            {
                high = 11;
            }
            else if (x > 6)

            {
                high = 0;
            }

            else if (x == 1)

            {
                high = 11;
            }
            else if (x == 3)

            {
                high = 9;
            }
            else if (x == 6)

            {
                high = 4;
            }

            else if (x >= 5)

            {
                high = 6;
            }
            else if (x >= 4)

            {
                high = 8;
            }

            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)

            {
                int xx = (x + Set.setVal.Width / 2);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }
    void GetJianTou_5_10(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        int midX = Set.setVal.Width / 2;
        if (midX < 3)
        {
            midX = 3;
        }
        for (int x = 0; x < midX; x++)
        {
            if (x >= 1)
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }
            else
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }

            AiXinHighCnt++;

            int high = 4;

            switch (x)
            {
                case 0:
                    high = 1;
                    break;
                case 1:
                    high = 1;
                    break;
                case 2:
                    high = 1;
                    break;
                case 3:
                    high = 1;
                    break;
            }

            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)

            {
                int xx = (x + Set.setVal.Width / 2);

                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }
    void GetJianTou_10_15(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        for (int x = 0; x < Set.setVal.Width / 2; x++)
        {
            if (x >= 0)
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }
            else
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }

            AiXinHighCnt++;

            int high = 5;
            switch (x)
            {
                default:
                    high = 0;
                    break;
                case 0:
                    high = 1;
                    break;
                case 1:
                    high = 1;
                    break;
                case 2:
                    high = 1;
                    break;
                case 3:
                    high = 1;
                    break;
                case 4:
                    high = 1;
                    break;
                    //case 5:
                    //    high = 1;
                    //    break;
                    //case 6:
                    //    high = 1;
                    //    break;
            }



            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)

            {
                int xx = (x + Set.setVal.Width / 2);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }

    void GetJianTou_Over15(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        for (int x = 0; x < Set.setVal.Width / 2; x++)
        {


            if (AiXinHighCnt >= 1)
            {
                AiXinHighCnt = 0;
                AiXinHigh++;
            }


            AiXinHighCnt++;

            int high = 5;
            switch (x)
            {
                default:
                    high = 0;
                    break;
                case 0:
                    high = 1;
                    break;
                case 1:
                    high = 1;
                    break;
                case 2:
                    high = 1;
                    break;
                case 3:
                    high = 1;
                    break;
                case 4:
                    high = 1;
                    break;
                case 5:
                    high = 1;
                    break;
                    //case 6:
                    //    high = 1;
                    //    break;
            }

            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)

            {
                int xx = (x + Set.setVal.Width / 2);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }
    void GetJianTou_Over20(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        for (int x = 0; x < Set.setVal.Width / 2; x++)
        {


            if (AiXinHighCnt >= 1)
            {
                AiXinHighCnt = 0;
                AiXinHigh++;
            }


            AiXinHighCnt++;

            int high = 7;
            switch (x)
            {
                default:
                    high = 0;
                    break;
                case 0:
                    high = 1;
                    break;
                case 1:
                    high = 1;
                    break;
                case 2:
                    high = 1;
                    break;
                case 3:
                    high = 1;
                    break;
                case 4:
                    high = 1;
                    break;
                case 5:
                    high = 1;
                    break;
                case 6:
                    high = 1;
                    break;
                case 7:
                    high = 1;
                    break;
                case 8:
                    high = 1;
                    break;
            }

            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)

            {
                int xx = (x + Set.setVal.Width / 2);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }
    void GetDiamond_5_10(int h)
    {
        AiXinHigh = h;
        AiXinHighCnt = 0;
        int midX = Set.setVal.Width / 2;
        if (midX < 3)
        {
            midX = 3;
        }
        for (int x = 0; x < midX; x++)
        {
            if (x >= 1)
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }
            else
            {
                if (AiXinHighCnt >= 1)
                {
                    AiXinHighCnt = 0;
                    AiXinHigh++;
                }
            }

            AiXinHighCnt++;

            int high = 4;

            switch (x)
            {
                case 0:
                    high = 5;
                    break;
                case 1:
                    high = 4;
                    break;
                case 2:
                    high = 3;
                    break;
                case 3:
                    high = 1;
                    break;
            }

            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height); y++)

            {
                int xx = (x + Set.setVal.Width / 2);

                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }
                xx = (Set.setVal.Width / 2 - x);
                picId = xx + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[picId];

                {

                    {

                        targetPos[TarPosCnt] = new Vector2(xx, y);
                        bool_HaveFire[TarPosCnt] = false;
                        TarPosCnt++;

                    }
                }

            }

        }
    }

    public void Update_Zadan()
    {
        int ppId = Framebuffer.tab_Mapping[Set.setVal.Width - 1 + (Set.setVal.Height / 2) * Set.setVal.Width];
        DrawPic.DrawPointId(ppId, 0xff0000, enPointSta.Target);
        if (LedKey.KeyStatus(ppId) || Input.GetKeyDown(KeyCode.Q)) //isDetonate
        {
            zadashu = 0;
            startBooming = true;
        }
        if (startBooming)
        {
            for (int i = 0; i < zadanx.Length; i++)
            {
                for (int k = 0; k < Set.setVal.Width; k++)
                {
                    //    Debug.LogError(zadany[i]);
                    picId = zadany[i] * Set.setVal.Width + k;
                    pointId = Framebuffer.tab_Mapping[picId];
                    if (GameLedControl.gamePoint[pointId].statue == enPointSta.Rest)
                    {
                        FjData.g_Fj[0].Scores += 1;
                        remainPoint--;
                    }
                    DrawPic.DrawPointId(pointId, tab_ZadanColor[0], enPointSta.None);
                    GameLedControl.Update_PointStatue(pointId, enPointSta.None);
                }
                for (int j = 0; j < Set.setVal.Height; j++)
                {
                    picId = j * Set.setVal.Width + zadanx[i];
                    pointId = Framebuffer.tab_Mapping[picId];
                    if (GameLedControl.gamePoint[pointId].statue == enPointSta.Rest)
                    {
                        FjData.g_Fj[0].Scores += 1;
                        remainPoint--;
                    }
                    DrawPic.DrawPointId(pointId, tab_ZadanColor[0], enPointSta.None);
                    GameLedControl.Update_PointStatue(pointId, enPointSta.None);
                }
            }
        }
    }
    void UpdateLed_zadan()
    {
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = k * Set.setVal.Width + i;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {
                    DrawPic.DrawPointId(pointId, 0, enPointSta.Target);
                }
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
                {
                    DrawPic.DrawPointId(pointId, tab_ZadanColor[0], enPointSta.None);

                }
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Rest)
                {
                    DrawPic.DrawPointId(pointId, tab_ZadanColor[2], enPointSta.Rest);

                }
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Die)
                {
                    DrawPic.DrawPointId(pointId, tab_ZadanColor[0], enPointSta.Die);

                }
            }
        }
    }
    void UpdateLed_different()
    {
        TarageColorDown();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = k * Set.setVal.Width + i;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {
                    DrawPic.DrawPointId(pointId, Led_Color[pointId], enPointSta.Target);
                }
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Die)
                {
                    DrawPic.DrawPointId(pointId, Led_Color[pointId], enPointSta.Die);

                }
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Rest)
                {
                    DrawPic.DrawPointId(pointId, Led_Color[pointId], enPointSta.Rest);

                }
            }
        }
    }
}

