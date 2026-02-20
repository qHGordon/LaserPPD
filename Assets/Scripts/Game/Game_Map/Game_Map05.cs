using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;
using LaserPPD.Core;

public class Game_Map05 : MonoBehaviour
{
    enum Num_En
    {
        A = 100,
        B,
        C, D, E, F, G, H, I, J, K, L, M, N, O, P,
        Q, R, S, T, U, V, W, X, Y, Z
    }
    public static Game_Map05 instance;
    int picId = 0;
    int pointId = 0;
    readonly uint[] tab_PointColor = { 0x400600, 0x031601, 0x45000A, 0x500050 };


    uint[] Tarage_Colors = new uint[MaxLedNum];///四面墙20个灯
    int[] Led_List = new int[MaxLedNum];///四面墙20个灯
    uint[] Led_List_Color = new uint[MaxLedNum];///四面墙20个灯

    int[] Eyes_NextIndex = { 0, 0, 0, 0 };///四面墙20个灯


    const int MaxLedNum = 20;
    public int TarageNum = 0;
    public int remainPoint = 0;
    public Text[] texts = new Text[MaxLedNum];

    public int MaxJieDuan = 4;
    public bool[] beTouch = new bool[300];

    public bool isClearAll = false;
    public bool isCleaning = false;
    public bool haveClear = false;

    float runTime = 0;
    float MoveTime = 0;
    float MaxMoveTime = 0.3f;
    float MaxMoveTime2 = 1f;
    int x, y;
    //第6关
    int istarage = 0;
    float waittime = 0;
    bool iswait = false;

    public bool isMoving = false;
    public int[] MovePos = new int[300];
    public int MoveIndex = 0;
    public int MoveCnt = 0;
    public int MoveAllStep = 10;
    public int tarageNum = 3;

    public int RedLED_Index = 0;
    bool isDouble = true;

    float Checking_CountTime = 0;
    const float MaxChecking_CountTime = 5;

    float Checking_IdleTime = 0;
    float MaxChecking_IdleTime = 10;

    float waitTime = 2;
    public float RedPassTime;
    public float MaxRedPassTime = 3;

    float ChangeTarage_Time;
    float MaxChangeTarage_Time = 10;
    //
    //
    bool[] isTarageList = new bool[MaxLedNum];


    //魔眼启动
    bool isStartChecking_DevEeys = false;
    bool isWaitChecking_DevEeys = false;

    //
    float RedStayTime = 4f;
    public const float MaxRedStayTime = 3f;//魔眼检测中的    持续时间
                                           //
    float waitCheckTime = 4f;
    const float MaxwaitCheckTime = 4f;//魔眼检测的   准备时间

    int DevEyes_Index = 0;//下一个闪缩的是哪个魔眼
    float EyesWaitTime = 10;//魔眼当前还有多少检测时间
    float MaxEyesWaitTime = 10;//魔眼最大每轮的检测时间是多少



    int LedNum_Index = 0;//
    int GetNum_First = -1;
    int GetNum_Scend = -1;
    int GetNum_Firstpoint = -1;
    int GetNum_Scendpoint = -1;
    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }
    uint Colors = 0x400000;
    bool goDown = true;
    void DevEyes_IdleCountDown()
    {


        if (isStartChecking_DevEeys)
        {

            return;
        }

        if (isWaitChecking_DevEeys)
        {


            if (waitCheckTime > 0)
            {
                waitCheckTime -= Time.deltaTime;
                if (goDown)
                {
                    Colors -= 0x0f0000;
                    if (Colors <= 0x0f0000)
                    {
                        Colors = 0x0f0000;
                        goDown = false;
                    }
                }
                else
                {
                    Colors += 0x0f0000;
                    if (Colors >= 0x400000)
                    {
                        Colors = 0x400000;
                        goDown = true;
                    }
                }


                for (int i = 0; i < 4; i++)
                {

                    if (Eyes_NextIndex[i] == 1)
                    {
#if UNITY_EDITOR

                        Main.PlayTime = Set.setVal.GameTime;
                        Debug.LogError("第" + i + "个");
                        //     Framebuffer.Update_PointColor(0, 0x00ff00, enPointSta.Die);
#endif
                        Framebuffer.Update_PointColor(i, Colors, enPointSta.Die);
                    }


                }
            }
            else
            {
                StartChecking_DevEyes();
            }
            return;
        }
        if (Checking_IdleTime > 0)
        {
            Checking_IdleTime -= Time.deltaTime;

        }
        else
        {

            StartEyes_TimeCountDown();
        }
    }

    void DevEyes_WaitCheck()
    {


        if (isStartChecking_DevEeys)
        {

            return;
        }

        if (isWaitChecking_DevEeys)
        {


            if (waitCheckTime > 0)
            {
                waitCheckTime -= Time.deltaTime;
                if (goDown)
                {
                    Colors -= 0x0f0000;
                    if (Colors <= 0x0f0000)
                    {
                        Colors = 0x0f0000;
                        goDown = false;
                    }
                }
                else
                {
                    Colors += 0x0f0000;
                    if (Colors >= 0x400000)
                    {
                        Colors = 0x400000;
                        goDown = true;
                    }
                }


                for (int i = 0; i < 4; i++)
                {

                    if (Eyes_NextIndex[i] == 1)
                    {
#if UNITY_EDITOR

                        Debug.LogError(i);
                        //     Framebuffer.Update_PointColor(0, 0x00ff00, enPointSta.Die);
#endif
                        Framebuffer.Update_PointColor(i, Colors, enPointSta.Die);
                    }


                }
            }
            else
            {
                StartChecking_DevEyes();
            }
            return;
        }

    }
    public void StartEyes_TimeCountDown()//环节开始,播放声音
    {


        if (isStartChecking_DevEeys)
        {
            return;
        }

        isWaitChecking_DevEeys = true;
        for (int i = 0; i < Eyes_NextIndex.Length; i++)
        {
            Eyes_NextIndex[i] = 0;
        }
        Eyes_NextIndex[MoveIndex] = 1;
        MoveIndex++;
        if (MoveIndex >= 4)
        {
            MoveIndex = 0;
        }

        Game05_Main.instance.PlaySound_YouYu();

    }
    public void StartChecking_DevEyes()//启动开始
    {
        isWaitChecking_DevEeys = false;
        isStartChecking_DevEeys = true;
        Checking_IdleTime = MaxChecking_IdleTime;
        Checking_CountTime = MaxChecking_CountTime;
        waitCheckTime = MaxwaitCheckTime;
        RedStayTime = MaxRedStayTime;

    }

    public void Checking_DevEeys()
    {

        if (!isStartChecking_DevEeys)
        {


            return;
        }
        for (int i = 0; i < 4; i++)
        {

            if (Eyes_NextIndex[i] == 0)
            {

                continue;
            }
#if UNITY_EDITOR
            Debug.LogError(" 检测到" + i + "      " + LedKey.KeyStatus(i));
#endif
            Framebuffer.Update_PointColor(i, 0x400000, enPointSta.Die);
            if (Game05_Main.instance.gameLevel >= 20)
            {
                if (i + 1 <= 3)
                {
                    if (LedKey.KeyStatus(i + 1))
                    {


                        isStartChecking_DevEeys = false;

                        RedStayTime = MaxRedStayTime;
                        FjData.g_Fj[0].Life--;
                        if (FjData.g_Fj[0].Scores > 0)
                        {
                            FjData.g_Fj[0].Scores--;
                        }
                        else
                        {
                            FjData.g_Fj[0].Scores = 0;
                        }

                        MusicManager.instance.Play_Fails();
                    }
                }

            }

            if (LedKey.KeyStatus(i))
            {

                //     Debug.LogError(" 检测到" + i + "      " + LedKey.KeyStatus(i));

                isStartChecking_DevEeys = false;

                RedStayTime = MaxRedStayTime;
                FjData.g_Fj[0].Life--;
                if (FjData.g_Fj[0].Scores > 0)
                {
                    FjData.g_Fj[0].Scores--;
                }
                else
                {
                    FjData.g_Fj[0].Scores = 0;
                }

                MusicManager.instance.Play_Fails();
            }

        }

        if (RedStayTime > 0)
        {
            RedStayTime -= Time.deltaTime;


        }
        else
        {
            Checking_CountTime = 5;
            isStartChecking_DevEeys = false;
            isWaitChecking_DevEeys = false; ;


            RedStayTime = MaxRedStayTime;
            for (int i = 0; i < 8; i++)
            {
                DrawPic.DrawPointId(i, 0, enPointSta.Rest);
            }
        }



    }


    void Init_LedListColor()
    {
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
        }
    }
    public void InitMap(int Id)
    {
        Num_Small = 1;
        Num_Big = 9;
        waitTime = 2;
        MaxJieDuan = 1;
        RedPassTime = 5;
        MaxRedPassTime = 3;
        picId = 0;
        pointId = 0;
        RedLED_Index = 0;
        MoveIndex = 0;
        LedNum_Index = 0;
        isDouble = true;

        TarageNum = 4;
        MoveTime = MaxMoveTime;
        Checking_CountTime = MaxChecking_CountTime;
        Checking_IdleTime = MaxChecking_IdleTime;
        ChangeTarage_Time = MaxChangeTarage_Time;
        waitCheckTime = MaxwaitCheckTime;
        isMoving = false;
        isWaitChecking_DevEeys = false;
        isStartChecking_DevEeys = false;
        GetNum_First = -1;
        GetNum_Scend = -1;
        remainPoint = 10 + 5 * Game05_Main.instance.Index_JieDuan;
        for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
        {
            GameLedControl.gamePoint[i].statue = enPointSta.None;

        }
        GetTarageColor();
        for (int i = 0; i < Eyes_NextIndex.Length; i++)
        {
            Eyes_NextIndex[i] = 0;
        }
        Init_LedListColor();
        for (int i = 0; i < isTarageList.Length; i++)
        {
            isTarageList[i] = false;
        }
        for (int i = 0; i < Led_List.Length; i++)
        {
            Led_List[i] = -1;
        }

        for (int i = 0; i < Game05_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game05_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int j = 0; j < Set.setVal.Height; j++)
            {
                picId = i + Set.setVal.Width * j;
                pointId = Framebuffer.tab_Mapping[picId];
                //        GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                DrawPic.DrawPointId(pointId, 0, enPointSta.Die);

            }
        }
        for (int i = 0; i < MovePos.Length; i++)
        {
            MovePos[i] = -1;
        }

        for (int i = 0; i < beTouch.Length; i++)
        {
            beTouch[i] = false;
        }
        Debug.LogError("Id" + Id);
        if (Id == 6)
        {
            Id = 10;
            InitMap_10();
        }
        else
        {
            Invoke("InitMap_" + Id.ToString("D2"), 0);

        }

    }
    void Start()
    {

    }
    void Run_RedPass()
    {
        RedPassTime -= Time.deltaTime;
        if (RedPassTime > 0)
        {
            return;
        }
        RedStayTime -= Time.deltaTime;
        if (isDouble)
        {
            for (int i = RedLED_Index; i < RedLED_Index + 4; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    DrawPic.DrawPointId(i * 3 + k, 0x400000, enPointSta.Die);

                }

            }

        }
        else
        {
            for (int i = RedLED_Index; i < RedLED_Index + 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    DrawPic.DrawPointId(i * 3 + k, 0x400000, enPointSta.Die);

                }
            }

        }
        if (RedStayTime < 0)
        {
            if (isDouble)
            {
                RedLED_Index += 4;
            }
            else
            {
                RedLED_Index += 3;
            }
            isDouble = !isDouble;
            if (RedLED_Index >= 32)
            {
                RedLED_Index = 0;
                isDouble = true;
            }
            RedStayTime = MaxRedStayTime;
            RedPassTime = MaxRedPassTime;
        }
    }
    void LED_GetNum()
    {
        for (int i = 1; i < 10; i++)
        {
            bool isSame = true;
            while (isSame)
            {
                int index = Random.Range(0, Led_List.Length);
                if (Led_List[index] != i)
                {
                    Led_List[index] = i;
                    isSame = false;
                }


            }
        }

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] < 0)
            {
                Led_List[i] = Random.Range(1, 10);
            }
        }
    }
    void LED_GetNum_Double()
    {

        //for (int i = 1; i < 5; i++)
        //{
        //    bool isSame = true;
        //    while (isSame)
        //    {
        //        int index = Random.Range(0, Led_List.Length);
        //        if (Led_List[index] != i * 2)
        //        {
        //            Led_List[index] = i * 2;
        //            isSame = false;
        //        }


        //    }
        //}

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] < 0)
            {
                int aa = Random.Range(1, 11);
                while (aa % 2 == 0)
                {
                    aa = Random.Range(1, 11);
                }

                Led_List[i] = aa;
                int a = Random.Range(0, 100);
                if (a < 50)
                {
                    Led_List[i] = 2 * aa;

                }
            }
        }
    }

    void InitMap_00()
    {
        MaxChecking_IdleTime = 8;

        LedNum_Index = 1;
        Game05_Main.instance.remainTime = 150;


        Led_List[0] = 14;
        Led_List[1] = 10;
        Led_List[2] = 8;
        Led_List[3] = 6;
        Led_List[4] = 3;
        Led_List[5] = 11;
        Led_List[6] = 2;
        Led_List[7] = 4;
        Led_List[8] = 7;
        Led_List[9] = 15;
        Led_List[10] = 19;
        Led_List[11] = 16;
        Led_List[12] = 20;
        Led_List[13] = 9;
        Led_List[14] = 18;
        Led_List[15] = 1;
        Led_List[16] = 5;
        Led_List[17] = 17;


        Led_List[18] = 12;
        Led_List[19] = 13;
        //Led_List[20] = 17;
        //Led_List[21] = 4;
        //Led_List[22] = 6;
        //Led_List[23] = 7;

        //Led_List[24] = 29;

        //Led_List[25] = 10;
        //Led_List[26] = 11;
        //Led_List[27] = 27;
        //Led_List[28] = 18;
        //Led_List[29] = 7;

        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x002344;
        }

        Update_LED_DianZhen_Data();
    }
    void InitMap_01()
    {
        Game05_Main.instance.remainTime = 120;

        Game05_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[0].text = "G";
        Game05_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 0, 0x000025);

        Game05_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[1].text = "1";
        Game05_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 0x002500, 0);

        Game05_Main.instance.gameUIComm.txt_TarageList[2].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[2].text = "A";
        Game05_Main.instance.gameUIComm.txt_TarageList[2].color = new Color(0, 0x002500, 0);

        Game05_Main.instance.gameUIComm.txt_TarageList[3].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[3].text = "L";
        Game05_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(0, 0, 0x000025);

        Game05_Main.instance.gameUIComm.txt_TarageList[4].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[4].text = "3";
        Game05_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(0, 0, 0x000025);
        for (int i = 0; i < Led_List.Length; i++)
        {
            Led_List[i] = -1;
        }

        // G  1  A  L3
        Led_List[0] = (int)Num_En.A;
        Led_List[1] = (int)Num_En.C;
        Led_List[2] = (int)Num_En.H;
        Led_List[3] = (int)Num_En.G;
        Led_List[4] = 5;
        Led_List[5] = 1;
        Led_List[6] = (int)Num_En.L;
        Led_List[7] = 3;
        Led_List[8] = (int)Num_En.A;
        Led_List[9] = (int)Num_En.G;

        Led_List[10] = 2;
        Led_List[11] = (int)Num_En.K;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = (int)Num_En.J;
        Led_List[14] = 1;
        Led_List[15] = 8;
        Led_List[16] = (int)Num_En.A;
        Led_List[17] = (int)Num_En.G;
        Led_List[18] = (int)Num_En.I;
        Led_List[19] = (int)Num_En.B;
        //Led_List[20] = 23;
        //Led_List[21] = 21;
        //Led_List[22] = 22;
        //Led_List[23] = 10;
        //Led_List[24] = 11;



        //Led_List[25] = 4;
        //Led_List[26] = 29;
        //Led_List[27] = 28;
        //Led_List[28] = 5;
        //Led_List[29] = 27;

        // G  1  A  L 3



        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x000025;
        }
        for (int i = 0; i < 5; i++)
        {
            Led_List_Color[i] = 0x000025;
        }
        for (int i = 5; i < 10; i++)
        {
            Led_List_Color[i] = 0x002500;
        }
        Led_List_Color[6] = 0x000025;
        Led_List_Color[7] = 0x000025;

        for (int i = 10; i < 15; i++)
        {
            Led_List_Color[i] = 0x000025;
        }
        Led_List_Color[12] = 0x002500;


        Led_List_Color[16] = 0x002500;
        Led_List_Color[18] = 0x002500;

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] == (int)Num_En.G && Led_List_Color[i] == 0x000025)
            {
                isTarageList[i] = true;
            }

            if (Led_List[i] == (int)Num_En.A && Led_List_Color[i] == 0x002500)
            {
                isTarageList[i] = true;
            }
            if (Led_List[i] == 1 && Led_List_Color[i] == 0x002500)
            {
                isTarageList[i] = true;
            }
            if (Led_List[i] == (int)Num_En.L && Led_List_Color[i] == 0x000025)
            {
                isTarageList[i] = true;
            }

            if (Led_List[i] == 3 && Led_List_Color[i] == 0x000025)
            {
                isTarageList[i] = true;
            }
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_02()
    {
        Game05_Main.instance.remainTime = 120;
        MaxChecking_IdleTime = 7;

        for (int i = 0; i < Led_List.Length; i++)
        {
            Led_List[i] = -1;
        }
        Led_List[0] = (int)Num_En.A; ;
        Led_List[1] = (int)Num_En.C;
        Led_List[2] = (int)Num_En.H;
        Led_List[3] = (int)Num_En.G;
        Led_List[4] = 5;
        Led_List[5] = 1;
        Led_List[6] = (int)Num_En.L;
        Led_List[7] = 3;
        Led_List[8] = (int)Num_En.A;
        Led_List[9] = (int)Num_En.G;
        Led_List[10] = 2;
        Led_List[11] = (int)Num_En.K;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = (int)Num_En.J;
        Led_List[14] = 1;
        Led_List[15] = 8;
        Led_List[16] = (int)Num_En.A;
        Led_List[17] = (int)Num_En.I;
        Led_List[18] = 0;
       Led_List[19] = 9;

        //Led_List[20] = 2;
        //Led_List[21] = (int)Num_En.K;
        //Led_List[22] = (int)Num_En.L;
        //Led_List[23] = (int)Num_En.J;
        //Led_List[24] = 1;

        //Led_List[25] = (int)Num_En.P;
        //Led_List[26] = 29;
        //Led_List[27] = 28;
        //Led_List[28] = (int)Num_En.T;
        //Led_List[29] = 27;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x00ff00;
        }
        for (int i = 0; i < 5; i++)
        {
            Led_List_Color[i] = 0x000025;
        }
        Led_List_Color[1] = 0x00ff00;
        Led_List_Color[2] = 0x00ff00;

        for (int i = 5; i < 10; i++)
        {
            Led_List_Color[i] = 0x00ff00;
        }
        Led_List_Color[6] = 0x000025;
        for (int i = 10; i < 15; i++)
        {
            Led_List_Color[i] = 0x000025;
        }
        Led_List_Color[12] = 0x00ff00;
        for (int i = 15; i < 20; i++)
        {
            Led_List_Color[i] = 0x000025;
        }
        Led_List_Color[16] = 0x00ff00;
        Led_List_Color[18] = 0x00ff00;
       // Led_List_Color[20] = 0x00ff00;
        //     Led_List_Color[24] = 0x00ff00;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            if (Led_List_Color[i] == 0x000025)
            {
                isTarageList[i] = true;

            }

        }


        Update_LED_DianZhen_Data();
    }


    void InitMap_03()
    {
        MaxChecking_IdleTime = 10;
        LedNum_Index = MaxLedNum;
        Game05_Main.instance.remainTime = 120;

        Led_List[0] = 11;
        Led_List[1] = 9;
        Led_List[2] = 8;
        Led_List[3] = 3;
        Led_List[4] = 12;
        Led_List[5] = 15;
        Led_List[6] = 2;
        Led_List[7] = 10;
        Led_List[8] = 14;
        Led_List[9] = 1;

        Led_List[10] = 5;
        Led_List[11] = 13;
        Led_List[12] = 17;
        Led_List[13] = 4;
        Led_List[14] = 6;
        Led_List[15] = 18;
        Led_List[16] = 16;
        Led_List[17] = 19;
        Led_List[18] = 7;
        Led_List[19] = 20;
        //Led_List[20] = 23;
        //Led_List[21] = 20;
        //Led_List[22] = 22;
        //Led_List[23] = 1;
        //   Led_List[24] =26;

        //Led_List[25] = 30;
        //Led_List[26] = 29;
        //Led_List[27] = 28;
        //Led_List[28] = 10;
        //Led_List[29] = 27;
        for (int i = 0; i < Game05_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game05_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
        }


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x00ff00;
        }
        for (int i = 0; i < isTarageList.Length; i++)
        {
            isTarageList[i] = true;
        }
        for (int i = 0; i < Game05_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game05_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(false);
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_04()
    {
        MaxChecking_IdleTime = 7;

        LedNum_Index = 1;
        Game05_Main.instance.remainTime = 100;
        Led_List[0] = 7;
        Led_List[1] = 16;
        Led_List[2] = 2;
        Led_List[3] = 17;
        Led_List[4] = 3;
        Led_List[5] = 4;
        Led_List[6] = 8;
        Led_List[7] = 6;
        Led_List[8] = 14;
        Led_List[9] = 10;
        Led_List[10] = 19;
        Led_List[11] = 18;
        Led_List[12] = 20;
        Led_List[13] = 9;
        Led_List[14] = 11;
        Led_List[15] = 1;
        Led_List[16] = 5;
        Led_List[17] = 21;


        Led_List[18] = 12;
        Led_List[19] = 13;
        //Led_List[20] = 17;
        //Led_List[21] = 4;
        //Led_List[22] = 6;
        //Led_List[23] = 15;
        //Led_List[24] = 15;

        //Led_List[25] = 26;
        //Led_List[26] = 11;
        //Led_List[27] = 27;
        //Led_List[28] = 10;
        //Led_List[29] = 22;

        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x120044;
        }

        Update_LED_DianZhen_Data();
    }
    void InitMap_05()
    {

        LED_GetNum_Double();
        MaxChecking_IdleTime = 10;
        LedNum_Index = 0;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x00ff00;
        }
        Update_LED_DianZhen_Data();
    }




    void InitMap_07()
    {
        Game05_Main.instance.remainTime = 120;
        MaxChecking_IdleTime = 10;

        for (int i = 0; i < Led_List.Length; i++)
        {
            Led_List[i] = -1;
        }
        Led_List[0] = 2;
        Led_List[1] = 2;
        Led_List[2] = 2;
        Led_List[3] = 56;
        Led_List[4] = 7;
        Led_List[5] = 15;
        Led_List[6] = 3;
        Led_List[7] = 3;
        Led_List[8] = 1;
        Led_List[9] = 8;
        Led_List[10] = 55;
        Led_List[11] = 5;
        Led_List[12] = 6;
        Led_List[13] = 2;
        Led_List[14] = 10;
        Led_List[15] = 90;
        Led_List[16] = 7;
        Led_List[17] = 5;
        Led_List[18] = 30;
        Led_List[19] = 25;
        Led_List[18] = 15;
        Led_List[19] = 13;
        //Led_List[20] = 17;
        //Led_List[21] = 45;
        //Led_List[22] = 56;
        //Led_List[23] = 75;
        //        Led_List[24] = 16;


        //Led_List[25] = 11;
        //Led_List[26] = 16;
        //Led_List[27] = 17;
        //Led_List[28] = 87;
        //Led_List[29] = 67;

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x04ff00;
        }
        Update_LED_DianZhen_Data();
    }

    void InitMap_08()
    {
        Game05_Main.instance.remainTime = 120;


        for (int i = 0; i < Led_List.Length; i++)
        {
            Led_List[i] = -1;
        }
        Led_List[0] = 3;
        Led_List[1] = 102;
        Led_List[2] = 108;
        Led_List[3] = 110;
        Led_List[4] = 5;
        Led_List[5] = 1;
        Led_List[6] = 106;
        Led_List[7] = 3;
        Led_List[8] = 100;
        Led_List[9] = 107;
        Led_List[10] = 6;
        Led_List[11] = 121;
        Led_List[12] = 112;
        Led_List[13] = 102;
        Led_List[14] = 9;
        Led_List[15] = 4;
        Led_List[16] = 3;
        Led_List[17] = 103;
        Led_List[18] = 108;
        Led_List[19] = 118;

        Led_List[18] = 12;
        Led_List[19] = 13;
        //Led_List[20] = 17;
        //Led_List[21] = 4;
        //Led_List[22] = 6;
        //Led_List[23] = 7;
        //Led_List[24] = 16;


        //Led_List[25] = 11;
        //Led_List[26] = 16;
        //Led_List[27] = 20;
        //Led_List[28] = 27;
        //Led_List[29] = 67;

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x400000;
        }
        Led_List_Color[0] = 0x003200;
        Led_List_Color[1] = 0x003200;
        Led_List_Color[7] = 0x003200;
        Led_List_Color[8] = 0x003200;
        Led_List_Color[10] = 0x003200;
        Led_List_Color[11] = 0x003200;
        Led_List_Color[12] = 0x003200;
        Led_List_Color[15] = 0x003200;
        Led_List_Color[17] = 0x003200;
        Led_List_Color[19] = 0x003200;

        isTarageList[0] = true;
        isTarageList[1] = true;
        isTarageList[7] = true;
        isTarageList[8] = true;
        isTarageList[10] = true;
        isTarageList[11] = true;
        isTarageList[12] = true;
        isTarageList[15] = true;
        isTarageList[17] = true;
        isTarageList[19] = true;

        Update_LED_DianZhen_Data();
    }
    void InitMap_09()
    {
        Game05_Main.instance.remainTime = 120;


        for (int i = 0; i < Led_List.Length; i++)
        {
            Led_List[i] = -1;
        }
        Led_List[0] = 3;
        Led_List[1] = 6;
        Led_List[2] = 5;
        Led_List[3] = 4;
        Led_List[4] = 7;
        Led_List[5] = 9;
        Led_List[6] = 7;
        Led_List[7] = 7;
        Led_List[8] = 4;
        Led_List[9] = 2;
        Led_List[10] = 3;
        Led_List[11] = 6;
        Led_List[12] = 9;
        Led_List[13] = 1;
        Led_List[14] = 1;
        Led_List[15] = 6;
        Led_List[16] = 6;
        Led_List[17] = 2;
        Led_List[18] = 9;
        Led_List[19] = 4;

        //Led_List[20] = 17;
        //Led_List[21] = 24;
        //Led_List[22] = 16;
        //Led_List[23] = 47;
        //Led_List[24] = 16;


        //Led_List[25] = 12;
        //Led_List[26] = 16;
        //Led_List[27] = 87;
        //Led_List[28] = 27;
        //Led_List[29] = 67;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x04ff00;
        }
        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] % 3 == 0)
            {
                isTarageList[i] = true;

            }
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_10()
    {
        MaxChecking_IdleTime = 10;
        LedNum_Index = MaxLedNum;
        Game05_Main.instance.remainTime = 120;

        Led_List[0] = 11;
        Led_List[1] = 9;
        Led_List[2] = 8;
        Led_List[3] = 3;
        Led_List[4] = 12;
        Led_List[5] = 15;
        Led_List[6] = 2;
        Led_List[7] = 1;
        Led_List[8] = 14;
        Led_List[9] = 20;

        Led_List[10] = 5;
        Led_List[11] = 13;
        Led_List[12] = 17;
        Led_List[13] = 4;
        Led_List[14] = 6;
        Led_List[15] = 18;
        Led_List[16] = 16;
        Led_List[17] = 19;
        Led_List[18] = 7;
        Led_List[19] = 10;
        //Led_List[20] = 23;
        //Led_List[21] = 20;
        //Led_List[22] = 22;
        //Led_List[23] = 10;
        //Led_List[24] = 1;

        //Led_List[25] = 30;
        //Led_List[26] = 29;
        //Led_List[27] = 28;
        //Led_List[28] = 10;
        //Led_List[29] = 27;
        for (int i = 0; i < Game05_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game05_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
        }


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0x000012;
        }
        for (int i = 0; i < isTarageList.Length; i++)
        {
            isTarageList[i] = true;
        }
        for (int i = 0; i < Game05_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game05_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(false);
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_11()
    {

        Game05_Main.instance.remainTime = 120;
        Led_List[0] = 3;
        Led_List[1] = (int)Num_En.K;
        Led_List[2] = (int)Num_En.O;
        Led_List[3] = (int)Num_En.K;
        Led_List[4] = 5;
        Led_List[5] = 6;
        Led_List[6] = 6;
        Led_List[7] = 3;
        Led_List[8] = (int)Num_En.A;
        Led_List[9] = (int)Num_En.F;
        Led_List[10] = (int)Num_En.S;
        Led_List[11] = (int)Num_En.V;
        Led_List[12] = 8;
        Led_List[13] = (int)Num_En.P;
        Led_List[14] = 5;
        Led_List[15] = (int)Num_En.X;
        Led_List[16] = 7;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = (int)Num_En.I;
        Led_List[19] = (int)Num_En.L;
        //Led_List[20] = 9;
        //Led_List[21] = 8;
        //Led_List[22] = 22;
        //Led_List[23] = 15;
        //Led_List[24] = 73;
        //Led_List[25] = 11;
        //Led_List[26] = (int)Num_En.Z; ;
        //Led_List[27] = (int)Num_En.J;
        //Led_List[28] = 6;
        //Led_List[29] = (int)Num_En.A;
        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x004000;
        }
        Led_List_Color[0] = 0x400000;
        Led_List_Color[2] = 0x000044;
        Led_List_Color[4] = 0x000044;


        for (int i = 5; i < 10; i++)
        {
            Led_List_Color[i] = 0x008A00;
        }
        Led_List_Color[7] = 0x000044;
        Led_List_Color[8] = 0x000044;
        Led_List_Color[9] = 0x400000;


        for (int i = 10; i < 15; i++)
        {
            Led_List_Color[i] = 0x008A00;
        }

        Led_List_Color[11] = 0x400000;
        Led_List_Color[14] = 0x000044;


        Led_List_Color[16] = 0x000044;
        Led_List_Color[17] = 0x400000;
        Led_List_Color[18] = 0x400000;
        
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            if (Led_List_Color[i] == 0x400000 || Led_List_Color[i] == 0x000044)
            {
                isTarageList[i] = true;
            }

        }

        Update_LED_DianZhen_Data();
    }
    void InitMap_12()
    {
        MaxChecking_IdleTime = 6;
        Game05_Main.instance.remainTime = 120;
        Led_List[0] = 3;
        Led_List[1] = 4;
        Led_List[2] = 6;
        Led_List[3] = 7;
        Led_List[4] = 1;
        Led_List[5] = (int)Num_En.K;
        Led_List[6] = (int)Num_En.K;
        Led_List[7] = 6;
        Led_List[8] = 9;
        Led_List[9] = 2;
        Led_List[10] = 3;
        Led_List[11] = 5;
        Led_List[12] = 1;
        Led_List[13] = 9;
        Led_List[14] = 8;
        Led_List[15] = 1;
        Led_List[16] = 6;
        Led_List[17] = 5;
        Led_List[18] = 7;
        Led_List[19] = 3;
        //Led_List[20] = 9;
        //Led_List[21] = 8;
        //Led_List[22] = 3;
        //Led_List[23] = 2;
        //Led_List[24] = 1;

        //Led_List[25] = 26;
        //Led_List[26] = 11;
        //Led_List[27] = 27;
        //Led_List[28] = 16;
        //Led_List[29] = 7;


        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x000044;
            if (Led_List[i] % 3 == 0)
            {
                isTarageList[i] = true;
            }
        }

        Update_LED_DianZhen_Data();
    }
    void InitMap_13()
    {
        MaxChecking_IdleTime = 6;
        Game05_Main.instance.remainTime = 120;

        Game05_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[0].text = "S";
        Game05_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0x000025, 00, 0);

        Game05_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[1].text = "H";
        Game05_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 0, 0x000025);

        Game05_Main.instance.gameUIComm.txt_TarageList[2].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[2].text = "7";
        Game05_Main.instance.gameUIComm.txt_TarageList[2].color = new Color(0, 0, 0x000025);
        Game05_Main.instance.gameUIComm.txt_TarageList[3].gameObject.SetActive(true);

        Game05_Main.instance.gameUIComm.txt_TarageList[3].text = "I";
        Game05_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(0, 0x000025, 0);
        Game05_Main.instance.gameUIComm.txt_TarageList[4].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[4].text = "3";
        Game05_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(0, 0x000025, 0);
        Led_List[0] = (int)Num_En.S;
        Led_List[1] = (int)Num_En.H;
        Led_List[2] = 7;
        Led_List[3] = (int)Num_En.I;
        Led_List[4] = 3;
        Led_List[5] = 6;
        Led_List[6] = 3;
        Led_List[7] = 1;
        Led_List[8] = 5;
        Led_List[9] = (int)Num_En.H;
        Led_List[10] = (int)Num_En.S;
        Led_List[11] = (int)Num_En.H;
        Led_List[12] = 1;
        Led_List[13] = (int)Num_En.I;
        Led_List[14] = 3;
        Led_List[15] = 3;
        Led_List[16] = 7;
        Led_List[17] = (int)Num_En.I;
        Led_List[18] = 1;
        Led_List[19] = (int)Num_En.L;
        //Led_List[20] = 25;
        //Led_List[21] = 63;
        //Led_List[22] = 3;
        //Led_List[23] = 23;
        // Led_List[24] = 1;

        //Led_List[25] = 11;
        //Led_List[26] = (int)Num_En.N;
        //Led_List[27] = (int)Num_En.W;
        //Led_List[28] = 16;
        //Led_List[29] = 8;

        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x000034;
        }
        Led_List_Color[3] = 0x003400;
        Led_List_Color[6] = 0x003400;
        Led_List_Color[7] = 0x003400;

        Led_List_Color[10] = 0x340000;
        Led_List_Color[11] = 0x003400;
        Led_List_Color[14] = 0x340000;
        Led_List_Color[15] = 0x003400;
        Led_List_Color[17] = 0x003400;

        isTarageList[1] = true;
        isTarageList[2] = true;
        isTarageList[3] = true;
        isTarageList[6] = true;
        //isTarageList[7] = true;
        isTarageList[9] = true;
        isTarageList[10] = true;
        isTarageList[15] = true;
        isTarageList[16] = true;
        isTarageList[17] = true;
        Update_LED_DianZhen_Data();
    }
    void InitMap_14()
    {

        Game05_Main.instance.remainTime = 120;
        Led_List[0] = 3;
        Led_List[1] = 6;
        Led_List[2] = 3;
        Led_List[3] = 5;
        Led_List[4] = 4;
        Led_List[5] = 9;
        Led_List[6] = (int)Num_En.K;
        Led_List[7] = (int)Num_En.A;
        Led_List[8] = (int)Num_En.C;
        Led_List[9] = 3;
        Led_List[10] = (int)Num_En.V;
        Led_List[11] = (int)Num_En.X;
        Led_List[12] = 5;
        Led_List[13] = 1;
        Led_List[14] = (int)Num_En.F;
        Led_List[15] = (int)Num_En.B;
        Led_List[16] = (int)Num_En.N;
        Led_List[17] = (int)Num_En.M;
        Led_List[18] = (int)Num_En.H;
        Led_List[19] = (int)Num_En.T;

        //Led_List[20] = 16;
        //Led_List[21] = 21;
        //Led_List[22] = 35;
        //Led_List[23] = 40;
        //Led_List[24] = 21;

        //Led_List[25] = (int)Num_En.D;
        //Led_List[26] = (int)Num_En.N;
        //Led_List[27] = (int)Num_En.W;
        //Led_List[28] = 16;
        //Led_List[29] = 9;

        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x004000;
        }
        //for (int i = 21; i < MaxLedNum; i++)
        //{
        //    if (i % 2 == 0)
        //    {
        //        Led_List_Color[i] = 0x353500;

        //    }
        //    else
        //    {
        //        Led_List_Color[i] = 0x200020;

        //    }

        //}
        Led_List_Color[2] = 0x000044;
        Led_List_Color[4] = 0x400000;
        Led_List_Color[6] = 0x400000;
        Led_List_Color[7] = 0x400000;
        Led_List_Color[10] = 0x400000;
        Led_List_Color[12] = 0x000035;
        Led_List_Color[15] = 0x000035;
        Led_List_Color[17] = 0x400000;

        isTarageList[0] = true;
        isTarageList[1] = true;
        isTarageList[2] = true;
        isTarageList[3] = true;
        isTarageList[5] = true;
        isTarageList[8] = true;
        isTarageList[9] = true;
        isTarageList[11] = true;
        isTarageList[12] = true;
        isTarageList[13] = true;
        isTarageList[14] = true;
        isTarageList[15] = true;
        isTarageList[16] = true;
        isTarageList[18] = true;
        isTarageList[19] = true;
        Update_LED_DianZhen_Data();
    }

    void InitMap_15()
    {

        Game05_Main.instance.remainTime = 120;
        Game05_Main.instance.remainTime = 120;
        Game05_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[0].text = "5";
        Game05_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 0x000025, 0);
        Game05_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[1].text = "9";
        Game05_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 0, 0x000025);

        Game05_Main.instance.gameUIComm.txt_TarageList[2].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[2].text = "K";
        Game05_Main.instance.gameUIComm.txt_TarageList[2].color = new Color(0, 0, 0x000025);

        Game05_Main.instance.gameUIComm.txt_TarageList[3].gameObject.SetActive(true);
        Game05_Main.instance.gameUIComm.txt_TarageList[3].text = "G";
        Game05_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(0, 0x000025, 0);

        Led_List[0] = (int)Num_En.H;
        Led_List[1] = (int)Num_En.K;
        Led_List[2] = (int)Num_En.K;
        Led_List[3] = (int)Num_En.Y;
        Led_List[4] = 3;
        Led_List[5] = 5;
        Led_List[6] = 7;
        Led_List[7] = (int)Num_En.L;
        Led_List[8] = (int)Num_En.O;
        Led_List[9] = 9;
        Led_List[10] = (int)Num_En.K;
        Led_List[11] = 6;
        Led_List[12] = (int)Num_En.G;
        Led_List[13] = 9;
        Led_List[14] = (int)Num_En.P;
        Led_List[15] = 7;
        Led_List[16] = 25;
        Led_List[17] = (int)Num_En.G;
        Led_List[18] = 9;
        Led_List[19] = (int)Num_En.G;
        //Led_List[20] = 15;
        //Led_List[21] = 64;
        //Led_List[22] = 23;
        //Led_List[23] = 1;
        //Led_List[24] = 6;

        //Led_List[25] = (int)Num_En.T;
        //Led_List[26] = 55;
        //Led_List[27] = (int)Num_En.W;
        //Led_List[28] = 76;
        //Led_List[29] = 12;

        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x004400;
        }
        Led_List_Color[2] = 0x000044;

        Led_List_Color[9] = 0x000044;

        Led_List_Color[10] = 0x000044;
        Led_List_Color[13] = 0x000044;

        Led_List_Color[19] = 0x000044;


        isTarageList[5] = true;
        isTarageList[17] = true;

        isTarageList[9] = true;
        isTarageList[13] = true;

        isTarageList[2] = true;
        isTarageList[10] = true;

        isTarageList[12] = true;

        Update_LED_DianZhen_Data();
    }
    void InitMap_16()
    {

        Game05_Main.instance.remainTime = 120;
        Led_List[0] = 4;
        Led_List[1] = 5;
        Led_List[2] = (int)Num_En.K;
        Led_List[3] = (int)Num_En.L;
        Led_List[4] = (int)Num_En.T;
        Led_List[5] = 3;
        Led_List[6] = (int)Num_En.D;
        Led_List[7] = (int)Num_En.Y;
        Led_List[8] = 6;
        Led_List[9] = (int)Num_En.P;
        Led_List[10] = (int)Num_En.A;
        Led_List[11] = (int)Num_En.M;
        Led_List[12] = 8;
        Led_List[13] = (int)Num_En.Y;
        Led_List[14] = (int)Num_En.R;
        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 5;
        Led_List[17] = (int)Num_En.F;
        Led_List[18] = (int)Num_En.T;
        Led_List[19] = (int)Num_En.R;
        //Led_List[20] = 15;
        //Led_List[21] = 11;
        //Led_List[22] = 2;
        //Led_List[23] = 1;
        //Led_List[24] = 6;

        //Led_List[25] = 21;
        //Led_List[26] = 64;
        //Led_List[27] = 12;
        //Led_List[28] = 16;
        //Led_List[29] = 9;
        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x004400;
        }
        //for (int i = 21; i < MaxLedNum; i++)
        //{
        //    int a = Random.Range(0, 100);
        //    if (a < 30)
        //    {
        //        Led_List_Color[i] = 0x131440;

        //    }
        //    else
        //        Led_List_Color[i] = 0x001145;
        //    {

        //    }
        //}
        Led_List_Color[1] = 0x000044;

        Led_List_Color[3] = 0x004400;

        Led_List_Color[5] = 0x000044;
        Led_List_Color[8] = 0x000044;
        Led_List_Color[9] = 0x004400;
        Led_List_Color[19] = 0x000044;

        isTarageList[10] = true;
        Update_LED_DianZhen_Data();
    }
    void InitMap_17()
    {
        Game05_Main.instance.remainTime = 120;
        Led_List[0] = 4;
        Led_List[1] = 6;
        Led_List[2] = 8;
        Led_List[3] = 2;
        Led_List[4] = 9;
        Led_List[5] = 2;
        Led_List[6] = 2;
        Led_List[7] = 6;
        Led_List[8] = 4;
        Led_List[9] = 2;
        Led_List[10] = 8;
        Led_List[11] = 8;
        Led_List[12] = 8;
        Led_List[13] = 3;
        Led_List[14] = 1;
        Led_List[15] = 4;
        Led_List[16] = 9;
        Led_List[17] = 5;
        Led_List[18] = 4;
        Led_List[19] = 3;
        //Led_List[20] = 15;
        //Led_List[21] = 64;
        //Led_List[22] = 23;
        //Led_List[23] = 1;
        //Led_List[24] = 6;
        //Led_List[25] = 22;
        //Led_List[26] = 84;
        //Led_List[27] = 74;
        //Led_List[28] = 46;
        //Led_List[29] = 64;
        for (int i = 0; i < MaxLedNum; i++)
        {
            Led_List_Color[i] = 0x004400;
            if (Led_List[i] % 4 == 0)
            {
                isTarageList[i] = true;
            }
        }



        Update_LED_DianZhen_Data();
    }
    void InitMap_18()
    {
        MaxJieDuan = 1;

        Game05_Main.instance.remainTime = 60;
        if (Game05_Main.instance.Index_JieDuan == 0)
        {
            Game05_Main.instance.remainTime = 120;


            Game05_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
            Game05_Main.instance.gameUIComm.txt_TarageList[1].text = "G";
            Game05_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 0, 0x000025);
            Game05_Main.instance.gameUIComm.txt_TarageList[2].gameObject.SetActive(true);
            Game05_Main.instance.gameUIComm.txt_TarageList[2].text = "K";
            Game05_Main.instance.gameUIComm.txt_TarageList[2].color = new Color(0, 0x000025, 0);


            Led_List[0] = (int)Num_En.K;
            Led_List[1] = (int)Num_En.G;
            Led_List[2] = (int)Num_En.G;
            Led_List[3] = (int)Num_En.K;
            Led_List[4] = (int)Num_En.G;
            Led_List[5] = (int)Num_En.K;
            Led_List[6] = (int)Num_En.G;
            Led_List[7] = (int)Num_En.K;
            Led_List[8] = (int)Num_En.G;
            Led_List[9] = (int)Num_En.K;

            Led_List[10] = (int)Num_En.L;
            Led_List[11] = (int)Num_En.K;
            Led_List[12] = (int)Num_En.D;
            Led_List[13] = (int)Num_En.D;
            Led_List[14] = (int)Num_En.G;
            Led_List[15] = (int)Num_En.D;
            Led_List[16] = (int)Num_En.G;
            Led_List[17] = (int)Num_En.K;
            Led_List[18] = (int)Num_En.H;
            Led_List[19] = (int)Num_En.D;
            //Led_List[20] = (int)Num_En.D;
            //Led_List[21] = (int)Num_En.A;
            //Led_List[22] = (int)Num_En.B;
            //Led_List[23] = (int)Num_En.D;
            //Led_List[24] = (int)Num_En.M;

            //Led_List[25] = (int)Num_En.D;
            //Led_List[26] = (int)Num_En.A;
            //Led_List[27] = (int)Num_En.B;
            //Led_List[28] = (int)Num_En.D;
            //Led_List[29] = (int)Num_En.M;
            for (int i = 0; i < MaxLedNum; i++)
            {
                Led_List_Color[i] = 0x004400;
            }
            Led_List_Color[1] = 0x000035;
            Led_List_Color[2] = 0x000035;
            Led_List_Color[3] = 0x000035;
            Led_List_Color[4] = 0x000035;
            Led_List_Color[5] = 0x000035;
            Led_List_Color[7] = 0x000035;
            Led_List_Color[9] = 0x000035;
            Led_List_Color[16] = 0x000035;
            Led_List_Color[17] = 0x000035;
            Led_List_Color[18] = 0x000035;

            isTarageList[0] = true;
            isTarageList[1] = true;
            isTarageList[2] = true;
            isTarageList[4] = true;
            isTarageList[11] = true;
            isTarageList[16] = true;
        }
        else
        {
            Debug.LogError("????");
            Game05_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
            Game05_Main.instance.gameUIComm.txt_TarageList[1].text = "3";
            Game05_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 0x000025, 0);
            Game05_Main.instance.gameUIComm.txt_TarageList[2].gameObject.SetActive(true);
            Game05_Main.instance.gameUIComm.txt_TarageList[2].text = "L";
            Game05_Main.instance.gameUIComm.txt_TarageList[2].color = new Color(0, 0, 0x000025);

            Game05_Main.instance.gameUIComm.txt_TarageList[3].gameObject.SetActive(true);
            Game05_Main.instance.gameUIComm.txt_TarageList[3].text = "1";
            Game05_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(0, 0x002500, 0);


            Led_List[0] = 3;
            Led_List[1] = 1;
            Led_List[2] = 1;
            Led_List[3] = (int)Num_En.L;
            Led_List[4] = 3;
            Led_List[5] = 1;
            Led_List[6] = (int)Num_En.L;
            Led_List[7] = (int)Num_En.L;
            Led_List[8] = 1;
            Led_List[9] = (int)Num_En.L;
            Led_List[10] = 3;
            Led_List[11] = (int)Num_En.S;
            Led_List[12] = 3;
            Led_List[13] = (int)Num_En.S;
            Led_List[14] = (int)Num_En.L;
            Led_List[15] = (int)Num_En.L;
            Led_List[16] = 1;
            Led_List[17] = (int)Num_En.S;
            Led_List[18] = 3;
            Led_List[19] = 1;
            Led_List[20] = (int)Num_En.D;
            Led_List[21] = (int)Num_En.A;
            Led_List[22] = (int)Num_En.C;
            Led_List[23] = (int)Num_En.E;
            Led_List[24] = (int)Num_En.M;

            Led_List[25] = 4;
            Led_List[26] = (int)Num_En.A;
            Led_List[27] = 5;
            Led_List[28] = 1;
            Led_List[29] = (int)Num_En.M;
            for (int i = 0; i < MaxLedNum; i++)
            {
                Led_List_Color[i] = 0x004400;
            }
            Led_List_Color[0] = 0x000035;
            Led_List_Color[2] = 0x000035;
            Led_List_Color[4] = 0x000035;
            Led_List_Color[5] = 0x000035;
            Led_List_Color[6] = 0x000035;
            Led_List_Color[9] = 0x000035;
            Led_List_Color[10] = 0x000035;
            Led_List_Color[11] = 0x000035;
            Led_List_Color[15] = 0x000035;
            Led_List_Color[18] = 0x000035;
            Led_List_Color[19] = 0x000035;

            isTarageList[1] = true;
            isTarageList[6] = true;
            isTarageList[8] = true;
            isTarageList[9] = true;
            isTarageList[12] = true;
            isTarageList[15] = true;
            isTarageList[16] = true;
            isTarageList[17] = true;
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_19()
    {
        MaxJieDuan = 1;

        Game05_Main.instance.remainTime = 60;
        if (Game05_Main.instance.Index_JieDuan == 0)
        {
            Led_List[0] = (int)Num_En.K;
            Led_List[1] = (int)Num_En.L;
            Led_List[2] = (int)Num_En.F;
            Led_List[3] = (int)Num_En.D;
            Led_List[4] = (int)Num_En.C;
            Led_List[5] = 6;
            Led_List[6] = 8;
            Led_List[7] = 9;
            Led_List[8] = 7;
            Led_List[9] = (int)Num_En.L;

            Led_List[10] = (int)Num_En.S;
            Led_List[11] = 3;
            Led_List[12] = (int)Num_En.S;
            Led_List[13] = 3;
            Led_List[14] = (int)Num_En.S;

            Led_List[15] = (int)Num_En.B;
            Led_List[16] = 8;
            Led_List[17] = (int)Num_En.Z;
            Led_List[18] = 2;
            Led_List[19] = 2;
            //Led_List[20] = (int)Num_En.D;
            //Led_List[21] = (int)Num_En.A;
            //Led_List[22] = (int)Num_En.B;
            //Led_List[23] = (int)Num_En.D;
            //Led_List[24] = (int)Num_En.M;

            //Led_List[25] = 1;
            //Led_List[26] = (int)Num_En.D;
            //Led_List[27] = 3;
            //Led_List[28] = 5;
            //Led_List[29] = (int)Num_En.N;
            for (int i = 0; i < MaxLedNum; i++)
            {
                Led_List_Color[i] = 0x004400;
            }
            Led_List_Color[1] = 0x000035;
            Led_List_Color[2] = 0x000035;
            Led_List_Color[5] = 0x000035;
            Led_List_Color[7] = 0x000035;
            Led_List_Color[9] = 0x000035;
            Led_List_Color[11] = 0x000035;
            Led_List_Color[12] = 0x000035;
            Led_List_Color[13] = 0x000035;
            Led_List_Color[17] = 0x000035;
            Led_List_Color[19] = 0x000035;

            isTarageList[1] = true;
            isTarageList[2] = true;
            isTarageList[5] = true;
            isTarageList[7] = true;
            isTarageList[9] = true;
            isTarageList[11] = true;
            isTarageList[12] = true;
            isTarageList[13] = true;
            isTarageList[17] = true;
            isTarageList[19] = true;
        }
        else
        {
            Led_List[0] = (int)Num_En.G;
            Led_List[1] = (int)Num_En.L;
            Led_List[2] = (int)Num_En.J;
            Led_List[3] = (int)Num_En.D;
            Led_List[4] = (int)Num_En.C;
            Led_List[5] = 3;
            Led_List[6] = 3;
            Led_List[7] = 3;
            Led_List[8] = 3;
            Led_List[9] = 3;
            Led_List[10] = (int)Num_En.S;
            Led_List[11] = 9;
            Led_List[12] = (int)Num_En.G;
            Led_List[13] = (int)Num_En.D;
            Led_List[14] = (int)Num_En.A;
            Led_List[15] = (int)Num_En.Y;
            Led_List[16] = 3;
            Led_List[17] = 1;
            Led_List[18] = 2;
            Led_List[19] = 4;
            Led_List[20] = 32;
            Led_List[21] = 1;
            Led_List[22] = (int)Num_En.B;
            Led_List[23] = (int)Num_En.D;
            Led_List[24] = (int)Num_En.M;


            Led_List[25] = 66;
            Led_List[26] = (int)Num_En.O;
            Led_List[27] = 3;
            Led_List[28] = 5;
            Led_List[29] = 1;

            for (int i = 0; i < MaxLedNum; i++)
            {
                Led_List_Color[i] = 0x004400;
            }
            Led_List_Color[0] = 0x000035;
            Led_List_Color[4] = 0x000035;
            Led_List_Color[6] = 0x000035;
            Led_List_Color[8] = 0x000035;
            Led_List_Color[9] = 0x000035;
            Led_List_Color[10] = 0x000035;
            Led_List_Color[11] = 0x000035;
            Led_List_Color[12] = 0x000035;
            Led_List_Color[16] = 0x000035;
            Led_List_Color[17] = 0x000035;
            Led_List_Color[19] = 0x000035;

            isTarageList[0] = true;
            isTarageList[4] = true;
            isTarageList[6] = true;
            isTarageList[8] = true;
            isTarageList[9] = true;
            isTarageList[10] = true;
            isTarageList[11] = true;
            isTarageList[12] = true;
            isTarageList[16] = true;
            isTarageList[17] = true;
            isTarageList[19] = true;
            isTarageList[21] = true;
            isTarageList[29] = true;

        }
        Update_LED_DianZhen_Data();
    }
    void GetLED_TarageList(int TTNum)//获得一定数量的,和目标灯颜色的灯
    {
        int a = 0;
        int[] List = new int[MaxLedNum];
        for (int i = 0; i < List.Length; i++)
        {
            List[i] = -1;
        }
        for (int i = 0; i < TTNum; i++)
        {

            bool isSame = true;

            while (isSame)
            {
                isSame = false;
                a = Random.Range(0, MaxLedNum);
                for (int k = 0; k < i; k++)
                {
                    if (List[k] == a)
                    {
                        isSame = true;

                    }
                }
            }

            List[i] = a;
            switch (Game05_Main.instance.gameLevel)
            {
                case 20:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.C;
                            Led_List_Color[a] = 0x0000ff;
                            //Debug.LogError(Led_List[a] + "初始化到c为"+a);
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x0000ff;
                            // Debug.LogError(Led_List[a] + "初始化到a为" + a);
                            isTarageList[a] = true;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.T;
                            Led_List_Color[a] = 0x0000ff;
                            //Debug.LogError(Led_List[a] + "初始化到t为" + a);
                            isTarageList[a] = true;
                            break;
                    }
                    break;
                case 21:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.K;
                            Led_List_Color[a] = 0x0000ff;
                            //Debug.LogError(Led_List[a] + "初始化到k为" + a);
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0x0000ff;
                            //Debug.LogError(Led_List[a] + "初始化到o为" + a);
                            isTarageList[a] = true;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x0000ff;
                            // Debug.LogError(Led_List[a] + "初始化到a为" + a);
                            isTarageList[a] = true;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0x0000ff;
                            //Debug.LogError(Led_List[a] + "初始化到l为" + a);
                            isTarageList[a] = true;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x0000ff;
                            //Debug.LogError(Led_List[a] + "初始化到a为" + a);
                            isTarageList[a] = true;
                            break;
                    }

                    break;
                case 22:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.K;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.Y;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                    }

                    break;
                case 23:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.D;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;

                    }

                    break;
                case 24:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;

                    }
                    break;
                case 25:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0x00ff00;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.I;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 6:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 7:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 8:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                    }
                    break;
                case 26:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.W;
                            Led_List_Color[a] = 0x00ff00;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.T;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.R;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 6:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 7:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 8:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 9:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0x00ff00;
                            break;

                    }

                    break;
                case 27:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.B;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x400000;
                            break;
                    }

                    break;
                case 28:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.H;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.I;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 6:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 7:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 8:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0x400000;
                            break;
                    }

                    break;
                case 29:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.K;
                            Led_List_Color[a] = 0x400000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.U;
                            Led_List_Color[a] = 0x400000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0x0000Cc;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x0000Cc;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0x0000Cc;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 6:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x00ff00;
                            break;
                        case 7:
                            Led_List[a] = (int)Num_En.X;
                            Led_List_Color[a] = 0x00ff00;
                            break;

                    }

                    break;
            }
            for (int n = 0; n < Led_List.Length; n++)
            {
                if (Led_List[n] == 127 || Led_List[n] < 0)
                {
                    Led_List[n] = Random.Range(1, (int)Num_En.Z + 1);
                    int xx = Random.Range(0, tab_PointColor.Length);
                    Led_List_Color[a] = tab_PointColor[xx];

                }
            }
        }


    }
    void InitMap_20()
    {

        Game05_Main.instance.remainTime = 60;
        Led_List[0] = (int)Num_En.K;
        Led_List[1] = (int)Num_En.L;
        Led_List[2] = (int)Num_En.F;
        Led_List[3] = (int)Num_En.D;
        Led_List[4] = (int)Num_En.C;

        Led_List[5] = 6;
        Led_List[6] = 8;
        Led_List[7] = 9;
        Led_List[8] = 7;
        Led_List[9] = (int)Num_En.L;

        Led_List[10] = (int)Num_En.S;
        Led_List[11] = 3;
        Led_List[12] = (int)Num_En.S;
        Led_List[13] = 3;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 8;
        Led_List[17] = (int)Num_En.T;
        Led_List[18] = 2;
        Led_List[19] = 2;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.T;
        //Led_List[23] = (int)Num_En.D;
        //Led_List[24] = (int)Num_En.M;

        //Led_List[25] = (int)Num_En.O;
        //Led_List[26] = (int)Num_En.B;
        //Led_List[27] = 3;
        //Led_List[28] = (int)Num_En.Y;
        //Led_List[29] = (int)Num_En.N;
        for (int i = 0; i < Led_List.Length; i++)
        {

            Led_List_Color[i] = 0x000040;

        }

        Update_LED_DianZhen_Data();
    }
    void InitMap_21()
    {

        Game05_Main.instance.remainTime = 90;
        Game05_Main.instance.remainTime = 60;
        Led_List[0] = (int)Num_En.K;
        Led_List[1] = (int)Num_En.L;
        Led_List[2] = (int)Num_En.F;
        Led_List[3] = (int)Num_En.D;
        Led_List[4] = (int)Num_En.C;

        Led_List[5] = 6;
        Led_List[6] = 8;
        Led_List[7] = 9;
        Led_List[8] = 7;
        Led_List[9] = (int)Num_En.L;

        Led_List[10] = (int)Num_En.B;
        Led_List[11] = 3;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = 3;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 8;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = 2;
        Led_List[19] = (int)Num_En.T;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Z;
        //Led_List[24] = (int)Num_En.M;

        //Led_List[25] = (int)Num_En.E;
        //Led_List[26] = (int)Num_En.B;
        //Led_List[27] = 3;
        //Led_List[28] = (int)Num_En.Y;
        //Led_List[29] = (int)Num_En.N;

        Led_List[19] = (int)Num_En.K;
        Led_List[6] = (int)Num_En.O;
        Led_List[2] = (int)Num_En.A;
        Led_List[12] = (int)Num_En.L;
        Led_List[15] = (int)Num_En.A;



        for (int i = 0; i < Led_List.Length; i++)
        {

            Led_List_Color[i] = 0x400040;
            if (Led_List[i] == (int)Num_En.K || Led_List[i] == (int)Num_En.A || Led_List[i] == (int)Num_En.L || Led_List[i] == (int)Num_En.O)
            {
                Led_List_Color[i] = 0x000040;

            }
        }


        Update_LED_DianZhen_Data();
    }
    void InitMap_22()
    {

        Game05_Main.instance.remainTime = 60;
        Led_List[0] = (int)Num_En.K;
        Led_List[1] = (int)Num_En.L;
        Led_List[2] = (int)Num_En.F;
        Led_List[3] = (int)Num_En.N;
        Led_List[4] = (int)Num_En.E;

        Led_List[5] = 6;
        Led_List[6] = 8;
        Led_List[7] = 9;
        Led_List[8] = (int)Num_En.M;
        Led_List[9] = (int)Num_En.L;

        Led_List[10] = (int)Num_En.B;
        Led_List[11] = 3;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = 3;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 8;
        Led_List[17] = (int)Num_En.O;
        Led_List[18] = 2;
        Led_List[19] = (int)Num_En.Y;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Y;
        //Led_List[24] = (int)Num_En.M;


        //Led_List[25] = (int)Num_En.G;
        //Led_List[26] = (int)Num_En.B;
        //Led_List[27] = 3;
        //Led_List[28] = (int)Num_En.F;
        //Led_List[29] = (int)Num_En.U;



        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x004400;

            }

            if (Led_List[i] == (int)Num_En.M || Led_List[i] == (int)Num_En.N || Led_List[i] == (int)Num_En.K || Led_List[i] == (int)Num_En.O || Led_List[i] == (int)Num_En.E || Led_List[i] == (int)Num_En.Y)
            {
                Led_List_Color[i] = 0x320000;

            }
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_23()
    {

        Game05_Main.instance.remainTime = 60;
        Game05_Main.instance.remainTime = 60;
        Led_List[0] = (int)Num_En.K;
        Led_List[1] = (int)Num_En.L;
        Led_List[2] = (int)Num_En.F;
        Led_List[3] = (int)Num_En.N;
        Led_List[4] = (int)Num_En.C;

        Led_List[5] = 6;
        Led_List[6] = 8;
        Led_List[7] = 9;
        Led_List[8] = 7;
        Led_List[9] = (int)Num_En.L;

        Led_List[10] = (int)Num_En.B;
        Led_List[11] = 3;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = 3;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 8;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = 2;
        Led_List[19] = (int)Num_En.E;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Y;
        //Led_List[24] = (int)Num_En.M;
        //Led_List[25] = (int)Num_En.T;
        //Led_List[26] = (int)Num_En.T;
        //Led_List[27] = 36;
        //Led_List[28] = (int)Num_En.U;
        //Led_List[29] = (int)Num_En.U;


        Led_List[19] = (int)Num_En.P;
        Led_List[11] = (int)Num_En.A;
        Led_List[3] = (int)Num_En.N;
        Led_List[2] = (int)Num_En.D;
        Led_List[5] = (int)Num_En.A;


        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x004400;

            }
            if (Led_List[i] == (int)Num_En.P || Led_List[i] == (int)Num_En.A || Led_List[i] == (int)Num_En.N || Led_List[i] == (int)Num_En.D || Led_List[i] == (int)Num_En.A)
            {
                Led_List_Color[i] = 0x320000;

            }
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_24()
    {

        Game05_Main.instance.remainTime = 60;
        Game05_Main.instance.remainTime = 60;
        Led_List[0] = (int)Num_En.K;
        Led_List[1] = (int)Num_En.L;
        Led_List[2] = (int)Num_En.F;
        Led_List[3] = (int)Num_En.N;
        Led_List[4] = (int)Num_En.C;

        Led_List[5] = 6;
        Led_List[6] = 8;
        Led_List[7] = 9;
        Led_List[8] = 7;
        Led_List[9] = (int)Num_En.L;

        Led_List[10] = (int)Num_En.B;
        Led_List[11] = 3;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = 3;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 8;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = 2;
        Led_List[19] = (int)Num_En.E;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Y;
        //Led_List[24] = (int)Num_En.M;
        //Led_List[25] = (int)Num_En.B;
        //Led_List[26] = (int)Num_En.O;
        //Led_List[27] = 96;
        //Led_List[28] = (int)Num_En.F;
        //Led_List[29] = (int)Num_En.U;

        Led_List[11] = (int)Num_En.P;
        Led_List[17] = (int)Num_En.A;
        Led_List[3] = (int)Num_En.P;
        Led_List[9] = (int)Num_En.L;
        Led_List[0] = (int)Num_En.E;


        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x004400;

            }
            if (Led_List[i] == (int)Num_En.A || Led_List[i] == (int)Num_En.P || Led_List[i] == (int)Num_En.L || Led_List[i] == (int)Num_En.E)
            {
                Led_List_Color[i] = 0x320000;

            }
        }
        Update_LED_DianZhen_Data();
    }
    //
    void InitMap_25()
    {

        Game05_Main.instance.remainTime = 90;

        Led_List[0] = (int)Num_En.K;
        Led_List[1] = (int)Num_En.L;
        Led_List[2] = (int)Num_En.F;
        Led_List[3] = (int)Num_En.N;
        Led_List[4] = (int)Num_En.C;

        Led_List[5] = 6;
        Led_List[6] = 8;
        Led_List[7] = 9;
        Led_List[8] = 7;
        Led_List[9] = (int)Num_En.L;

        Led_List[10] = (int)Num_En.B;
        Led_List[11] = 3;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = 3;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 8;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = 2;
        Led_List[19] = (int)Num_En.E;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Y;
        ////Led_List[24] = (int)Num_En.M;

        //Led_List[25] = (int)Num_En.B;
        //Led_List[26] = (int)Num_En.O;
        //Led_List[27] = 96;
        //Led_List[28] = (int)Num_En.F;
        //Led_List[29] = (int)Num_En.U;


        Led_List[0] = (int)Num_En.P;
        Led_List[14] = (int)Num_En.A;
        Led_List[23] = (int)Num_En.P;
        Led_List[19] = (int)Num_En.L;
        Led_List[12] = (int)Num_En.E;

        Led_List[1] = (int)Num_En.P;
        Led_List[3] = (int)Num_En.I;
        Led_List[5] = (int)Num_En.N;
        Led_List[10] = (int)Num_En.E;


        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x001111;

            }
            if (Led_List[i] == (int)Num_En.P || Led_List[i] == (int)Num_En.I || Led_List[i] == (int)Num_En.N || Led_List[i] == (int)Num_En.E || Led_List[i] == (int)Num_En.A || Led_List[i] == (int)Num_En.P || Led_List[i] == (int)Num_En.L)
            {
                Led_List_Color[i] = 0x004000;

            }
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_26()
    {

        Game05_Main.instance.remainTime = 90;

        Led_List[0] = (int)Num_En.A;
        Led_List[1] = (int)Num_En.B;
        Led_List[2] = (int)Num_En.D;
        Led_List[3] = (int)Num_En.E;
        Led_List[4] = (int)Num_En.C;

        Led_List[5] = 6;
        Led_List[6] = 8;
        Led_List[7] = 9;
        Led_List[8] = 7;
        Led_List[9] = (int)Num_En.F;

        Led_List[10] = (int)Num_En.B;
        Led_List[11] = 3;
        Led_List[12] = (int)Num_En.L;
        Led_List[13] = 3;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 8;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = 2;
        Led_List[19] = (int)Num_En.E;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.J;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.K;
        //Led_List[24] = (int)Num_En.M;

        //Led_List[25] = (int)Num_En.Y;
        //Led_List[26] = (int)Num_En.C;
        //Led_List[27] = 78;
        //Led_List[28] = (int)Num_En.B;
        //Led_List[29] = (int)Num_En.U;


        Led_List[6] = (int)Num_En.W;
        Led_List[4] = (int)Num_En.A;
        Led_List[11] = (int)Num_En.T;
        Led_List[19] = (int)Num_En.E;
        Led_List[7] = (int)Num_En.R;

        Led_List[18] = (int)Num_En.M;
        Led_List[13] = (int)Num_En.E;
        Led_List[2] = (int)Num_En.L;
        Led_List[15] = (int)Num_En.O;
        Led_List[16] = (int)Num_En.N;


        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x001111;

            }
            if (Led_List[i] == (int)Num_En.W || Led_List[i] == (int)Num_En.A || Led_List[i] == (int)Num_En.T || Led_List[i] == (int)Num_En.E || Led_List[i] == (int)Num_En.R || Led_List[i] == (int)Num_En.M || Led_List[i] == (int)Num_En.E || Led_List[i] == (int)Num_En.L || Led_List[i] == (int)Num_En.O || Led_List[i] == (int)Num_En.N)
            {
                Led_List_Color[i] = 0x004000;

            }
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_27()
    {


        Game05_Main.instance.remainTime = 60;

        Led_List[0] = (int)Num_En.C;
        Led_List[1] = (int)Num_En.B;
        Led_List[2] = (int)Num_En.A;
        Led_List[3] = (int)Num_En.N;
        Led_List[4] = (int)Num_En.B;

        Led_List[5] = 36;
        Led_List[6] = 28;
        Led_List[7] = 19;
        Led_List[8] = 67;
        Led_List[9] = (int)Num_En.A;

        Led_List[10] = (int)Num_En.D;
        Led_List[11] = 33;
        Led_List[12] = (int)Num_En.C;
        Led_List[13] = 53;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 38;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = 22;
        Led_List[19] = (int)Num_En.E;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Y;
        //Led_List[24] = (int)Num_En.M;

        //Led_List[25] = (int)Num_En.D;
        //Led_List[26] = (int)Num_En.N;
        //Led_List[27] = (int)Num_En.J;
        //Led_List[28] = (int)Num_En.F;
        //Led_List[29] = (int)Num_En.U;


        Led_List[0] = (int)Num_En.B;
        Led_List[15] = (int)Num_En.A;
        Led_List[16] = (int)Num_En.N;
        Led_List[3] = (int)Num_En.A;
        Led_List[14] = (int)Num_En.N;

        Led_List[2] = (int)Num_En.A;

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x001111;

            }
            if (Led_List[i] == (int)Num_En.B || Led_List[i] == (int)Num_En.A || Led_List[i] == (int)Num_En.N)
            {
                Led_List_Color[i] = 0x400000;

            }
        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_28()
    {


        Game05_Main.instance.remainTime = 60;

        Led_List[0] = (int)Num_En.D;
        Led_List[1] = (int)Num_En.B;
        Led_List[2] = (int)Num_En.A;
        Led_List[3] = (int)Num_En.N;
        Led_List[4] = (int)Num_En.N;

        Led_List[5] = 36;
        Led_List[6] = 28;
        Led_List[7] = 64;
        Led_List[8] = 22;
        Led_List[9] = (int)Num_En.A;

        Led_List[10] = (int)Num_En.D;
        Led_List[11] = 33;
        Led_List[12] = (int)Num_En.C;
        Led_List[13] = 22;
        Led_List[14] = (int)Num_En.S;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 15;
        Led_List[17] = (int)Num_En.Z;
        Led_List[18] = 23;
        Led_List[19] = (int)Num_En.E;
        //Led_List[20] = (int)Num_En.D;
        //Led_List[21] = (int)Num_En.A;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Y;
        //Led_List[24] = (int)Num_En.M;

        //Led_List[25] = (int)Num_En.C;
        //Led_List[26] = (int)Num_En.F;
        //Led_List[27] = (int)Num_En.N;
        //Led_List[28] = (int)Num_En.Z;
        //Led_List[29] = (int)Num_En.Z;


        Led_List[1] = (int)Num_En.H;
        Led_List[5] = (int)Num_En.A;
        Led_List[8] = (int)Num_En.M;
        Led_List[3] = (int)Num_En.I;

        Led_List[17] = (int)Num_En.M;
        Led_List[16] = (int)Num_En.E;
        Led_List[7] = (int)Num_En.L;
        Led_List[12] = (int)Num_En.O;
        Led_List[10] = (int)Num_En.N;

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x001111;

            }
            if (Led_List[i] == (int)Num_En.H || Led_List[i] == (int)Num_En.A || Led_List[i] == (int)Num_En.I || Led_List[i] == (int)Num_En.E || Led_List[i] == (int)Num_En.O || Led_List[i] == (int)Num_En.L || Led_List[i] == (int)Num_En.N)
            {
                Led_List_Color[i] = 0x450000;

            }
            Led_List_Color[8] = 0x450000;
            Led_List_Color[17] = 0x450000;

        }
        Update_LED_DianZhen_Data();
    }
    void InitMap_29()
    {
        Game05_Main.instance.remainTime = 60;

        Led_List[0] = (int)Num_En.Z;
        Led_List[1] = (int)Num_En.J;
        Led_List[2] = (int)Num_En.J;
        Led_List[3] = (int)Num_En.I;
        Led_List[4] = (int)Num_En.Z;

        Led_List[5] = 36;
        Led_List[6] = 28;
        Led_List[7] = 20;
        Led_List[8] = 22;
        Led_List[9] = (int)Num_En.Z;

        Led_List[10] = (int)Num_En.X;
        Led_List[11] = 33;
        Led_List[12] = (int)Num_En.S;
        Led_List[13] = 22;
        Led_List[14] = (int)Num_En.Z;

        Led_List[15] = (int)Num_En.B;
        Led_List[16] = 15;
        Led_List[17] = (int)Num_En.A;
        Led_List[18] = 23;
        Led_List[19] = (int)Num_En.D;
        //Led_List[20] = (int)Num_En.Y;
        //Led_List[21] = (int)Num_En.J;
        //Led_List[22] = (int)Num_En.O;
        //Led_List[23] = (int)Num_En.Y;
        // Led_List[24] = (int)Num_En.M;

        //Led_List[25] = (int)Num_En.X;
        //   Led_List[26] = (int)Num_En.H;
        //   Led_List[27] = (int)Num_En.N;
        //    Led_List[28] = (int)Num_En.I;
        //   Led_List[29] = (int)Num_En.W;
        //     Led_List[30] = (int)Num_En.H;
        //   Led_List[31] = (int)Num_En.M;
        //
        Led_List[10] = (int)Num_En.G;
        Led_List[4] = (int)Num_En.O;
        Led_List[5] = (int)Num_En.O;
        Led_List[17] = (int)Num_En.D;
        Led_List[1] = (int)Num_En.L;
        Led_List[6] = (int)Num_En.U;
        Led_List[2] = (int)Num_En.C;
        Led_List[9] = (int)Num_En.K;


        for (int i = 0; i < Led_List.Length; i++)
        {
            if (i % 2 == 0)
            {
                Led_List_Color[i] = 0x400040;

            }
            else
            {
                Led_List_Color[i] = 0x001111;

            }
            if (Led_List[i] == (int)Num_En.G || Led_List[i] == (int)Num_En.O || Led_List[i] == (int)Num_En.D ||
                Led_List[i] == (int)Num_En.L || Led_List[i] == (int)Num_En.U || Led_List[i] == (int)Num_En.C || Led_List[i] == (int)Num_En.K)
            {
                Led_List_Color[i] = 0x400000;

            }
        }
        Update_LED_DianZhen_Data();
    }
    public float SafeTime = 1f;

    void CheckLedKey()
    {
        int id;
        int pointId = 0;


        if (Input.GetKey(KeyCode.I))
        {
            StartEyes_TimeCountDown();

        }

        if (Main.IsDemo) { return; }

        //int pointId = 0;
        int len = Mathf.Min(Set.setVal.Width * Set.setVal.Height, Main.MAX_LED);

        bool pressDiePoint = false;
        bool pressTargetPoint = false;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.A))
        {
            pressDiePoint = true;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            pressTargetPoint = true;
            //
        }
        for (x = 0; x < Set.setVal.Width; x++)
        {
            for (y = 0; y < Set.setVal.Height; y++)
            {
                id = x + Set.setVal.Width * y;
                pointId = Framebuffer.tab_Mapping[id];
                //switch (Framebuffer.led[pointId].statue)
                //{
                //    case enPointSta.None:
                //        Game05_Main.instance.list_PresetPic[id].color = Color.white;

                //        break;
                //    case enPointSta.Target:
                //        Game05_Main.instance.list_PresetPic[id].color = Color.blue;

                //        break;
                //    case enPointSta.Die:
                //        Game05_Main.instance.list_PresetPic[id].color = Color.red;

                //        break;
                //    case enPointSta.Rest:
                //        Game05_Main.instance.list_PresetPic[id].color = Color.green;

                //        break;
                //    case enPointSta.MoveRest:
                //        break;
                //    case enPointSta.MoveDie:
                //        break;
                //    case enPointSta.Dieing:
                //        break;
                //    default:
                //        break;
                //}
            }
        }
#endif

#if UNITY_EDITOR

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Framebuffer.led[4 + i].statue == enPointSta.Target)

            {
                Debug.LogError(i + "   " + Led_List[i] + Framebuffer.led[4 + i].statue + Led_List_Color[i]);
            }
        }
#endif
        for (int i = 0; i < Led_List.Length; i++)
        {

            pointId = 4 + i;

            if (pressDiePoint == false && pressTargetPoint == false)
            {
                if (LedKey.KeyStatus(pointId) == false)
                    continue;
            }
#if UNITY_EDITOR

            if (Framebuffer.led[pointId].statue == enPointSta.Die && GameLedControl.gamePoint[pointId].errorTime == 0 && pressDiePoint)
            {
#else
            // 踩到红色点
            if (Framebuffer.led[pointId].statue == enPointSta.Die && GameLedControl.gamePoint[pointId].errorTime == 0)
            {
#endif
                // 
                for (int j = 0; j < 1; j++)
                {

                    if (FjData.g_Fj[0].Life > 0 && SafeTime < 0)
                    {


                        FjData.g_Fj[0].Life--;
                        FjData.g_Fj[0].Life--;
                        if (Game05_Main.instance.Score_LinShi > 0)
                        {
                            Game05_Main.instance.Score_LinShi--;

                        }
                        else
                        {
                            Game05_Main.instance.Score_LinShi = 0;
                        }
                        MusicManager.instance.Play_Fails();

                    }
                    GameLedControl.gamePoint[pointId].errorTime = 10;
                    pressDiePoint = false;

                }
                continue;
            }
            // 踩到蓝色点
#if UNITY_EDITOR
            if (Framebuffer.led[pointId].statue == enPointSta.Target && pressTargetPoint)
            {

#else
            if (Framebuffer.led[pointId].statue == enPointSta.Target)
            {
#endif

                Game05_Main.instance.Score_LinShi += 3;
                MusicManager.instance.Play_Correct();

                SafeTime = 0.5f;

                pressTargetPoint = false;
                switch (Game05_Main.instance.gameLevel)
                {
                    case 0:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        Num_Small++;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 1:
                        LedNum_Index++;

                        if (pointId == 4 + 8 || pointId == 4 + 6)
                        {
                            StartEyes_TimeCountDown();
                        }
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 2:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 3:
                        LedNum_Index--;
                        Num_Big--;
                        Led_List[i] = 127;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 4:
                        LedNum_Index++;
                        Num_Small++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 5:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;

                    case 7:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 8:
                        LedNum_Index++;
                        if (Led_List[i] == 3)
                        {
                            StartChecking_DevEyes();
                        }
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 9:
                        LedNum_Index++;

                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 6:
                    case 10:

                        LedNum_Index--;
                        Num_Big--;
                        Led_List[i] = 127;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 11:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 12:
                        LedNum_Index++;
                        if (i == 2 && Led_List[i] == 6)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            Led_List[4] = 9;
                            isTarageList[4] = true;
                        }
                        else if (i == 8 && Led_List[i] == 9)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            Led_List[5] = 3;
                            isTarageList[5] = true;
                        }
                        else if (i == 16 && Led_List[i] == 6)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            Led_List[17] = 3;
                            isTarageList[17] = true;
                        }
                        else
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        }

                        break;
                    case 13:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 14:
                        LedNum_Index++;
                        if (i == 1 && Led_List[i] == 6)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            Led_List[4] = 4;
                            Led_List_Color[4] = 0x000040;
                            isTarageList[4] = true;
                        }


                        else
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        }

                        break;
                    case 15:
                        LedNum_Index++;
                        if (i == 2 && Led_List[i] == (int)Num_En.K)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            Led_List[0] = (int)Num_En.G;
                            Led_List_Color[0] = 0x004400;
                            isTarageList[0] = true;
                        }
                        else if (i == 10 && Led_List[i] == 5)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            Led_List_Color[18] = 0x000035;

                            isTarageList[18] = true;
                        }
                        else if (i == 16 && Led_List[i] == 5)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            Led_List_Color[19] = 0x004400;

                            isTarageList[19] = true;
                        }

                        else
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        }

                        break;
                    case 16:
                        LedNum_Index++;
                        if (i == 10 && Led_List[i] == (int)Num_En.A)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);


                            isTarageList[15] = true;
                        }
                        else if (i == 15 && Led_List[i] == (int)Num_En.B)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[6] = true;
                        }
                        else if (i == 6 && Led_List[i] == (int)Num_En.D)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[17] = true;
                        }
                        else if (i == 17 && Led_List[i] == (int)Num_En.F)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[2] = true;
                        }
                        else if (i == 2 && Led_List[i] == (int)Num_En.K)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[3] = true;
                        }
                        else if (i == 3 && Led_List[i] == (int)Num_En.L)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[11] = true;
                        }
                        else if (i == 11 && Led_List[i] == (int)Num_En.M)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[9] = true;
                        }
                        else if (i == 9 && Led_List[i] == (int)Num_En.P)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[14] = true;
                        }
                        else if (i == 14 && Led_List[i] == (int)Num_En.R)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[4] = true;
                            isTarageList[18] = true;
                        }
                        else if (i == 4 || i == 18)
                        {
                            Led_List[4] = 127;
                            Led_List[18] = 127;
                            isTarageList[4] = false;
                            isTarageList[18] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[13] = true;
                            isTarageList[7] = true;
                        }

                        else if (i == 7 || i == 13)
                        {
                            for (int k = 0; k < Led_List.Length; k++)
                            {
                                Led_List[k] = 127;
                                isTarageList[k] = false;
                            }

                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isClearAll = true;
                        }
                        else
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        }

                        break;
                    case 17:
                        LedNum_Index++;
                        if (i == 0 && Led_List[i] == 4)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);


                            isTarageList[3] = true;
                            Led_List[3] = 4;
                        }
                        else if (i == 8)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[0] = true;
                            isTarageList[1] = true;
                            isTarageList[4] = true;
                            Led_List[0] = 8;
                            Led_List[1] = 8;
                            Led_List[4] = 8;
                        }
                        else if (i == 10)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[6] = false;
                            isTarageList[7] = false;
                            Led_List[6] = 6;
                            Led_List[7] = 5;
                        }
                        else if (i == 15)
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                            isTarageList[18] = true;
                            Led_List[18] = 8;

                        }
                        else
                        {
                            Led_List[i] = 127;
                            isTarageList[i] = false;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        }


                        break;
                    case 18:
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 19:
                        if (i == 17 && Led_List[i] == (int)Num_En.S)
                        {
                            StartChecking_DevEyes();
                        }
                        if (i == 15 && Led_List[i] == (int)Num_En.S)
                        {
                            StartChecking_DevEyes();
                        }
                        if (i == 9 && Led_List[i] == 3)
                        {
                            StartChecking_DevEyes();
                        }
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        break;

                    case 20:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        //Debug.LogError(LedNum_Index + "  第21关");
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 21:

                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        //Debug.LogError(LedNum_Index + "  第22关");
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 22:


                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;

                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 23:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;

                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                        break;
                    case 24:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        break;
                    case 25:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        break;
                    case 26:
                        LedNum_Index++;
                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        break;
                    case 27:
                        LedNum_Index++;

                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        break;
                    case 28:
                        LedNum_Index++;

                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        break;
                    case 29:
                        LedNum_Index++;

                        Led_List[i] = 127;
                        isTarageList[i] = false;
                        GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                        Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);

                        break;
                }
                Update_LED_DianZhen_Data();
            }

        }

    }



    void Update_LEDNum_BigTOSmall()
    {

        for (int i = 0; i < Led_List.Length; i++)
        {

            if (Led_List[i] != LedNum_Index || !isTarageList[i])
            {

                if (Led_List[i] == 127)
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);

                }
                else
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);

                }
            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);

            }
        }
    }
    void Update_LEDNum_SmallToBig()
    {


        for (int i = 0; i < Led_List.Length; i++)
        {

            if (Led_List[i] != LedNum_Index)// || !isTarageList[i]
            {
                if (Led_List[i] == 127)
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);
                    GameLedControl.gamePoint[4 + i].statue = enPointSta.None;
                }
                else
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                    GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;

                }
            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;

            }
        }
    }
    int Num_Small = 1;
    int Num_Big = 9;

    void Update_LEDNum_Double()
    {

        for (int i = 0; i < Led_List.Length; i++)
        {

            if (Led_List[i] % 2 != 0)
            {

                if (Led_List[i] == 127)
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);

                }
                else
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);

                }

            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);

            }
        }
    }
    void Update_LEDNum_Third()
    {

        for (int i = 0; i < Led_List.Length; i++)
        {

            if (Led_List[i] % 3 != 0 || Led_List[i] >= 100)
            {

                if (Led_List[i] == 127)
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);

                }
                else
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);

                }

            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);

            }
        }
    }
    void Update_LedTarage()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {


            if (Led_List[i] == 127)
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Rest;

                continue;
            }
            if (!isTarageList[i])
            {
                Framebuffer.Update_PointColor(4 + i, 0x0004A0, enPointSta.Die);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;

            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0x0004A0, enPointSta.Target);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
            }

        }
    }
    void Update_LedTarages()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {


            if (Led_List[i] == 127)
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Rest;
                continue;
            }
            if (istarage == 0)
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
            }
            else if (Led_List[i] == istarage && isTarageList[i])
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
            }

        }
    }

    void Update_LedLetter()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {
            switch (Game05_Main.instance.gameLevel)
            {


                case 20:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.C)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.T)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                    }
                    break;
                case 21:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.K)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.O)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.L)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                    }
                    break;
                case 22:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.M)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.O)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.N)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.K)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.E)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 5:
                            if (Led_List[i] == (int)Num_En.Y)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                    }
                    break;
                case 23:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.P)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.N)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.D)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;

                    }
                    break;
                case 24:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.P)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.P)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.L)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.E)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;

                    }
                    break;
                case 25:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.P)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.I)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.N)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.E)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 5:
                            if (Led_List[i] == (int)Num_En.P)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 6:
                            if (Led_List[i] == (int)Num_En.P)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 7:
                            if (Led_List[i] == (int)Num_En.L)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 8:
                            if (Led_List[i] == (int)Num_En.E)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;


                    }
                    break;
                case 26:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.W)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.T)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.E)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.R)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 5:
                            if (Led_List[i] == (int)Num_En.M)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 6:
                            if (Led_List[i] == (int)Num_En.E)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 7:
                            if (Led_List[i] == (int)Num_En.L)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 8:
                            if (Led_List[i] == (int)Num_En.O)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 9:
                            if (Led_List[i] == (int)Num_En.N)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;

                    }
                    break;
                case 27:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.B)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.N)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.N)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 5:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                    }
                    break;
                case 28:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.H)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.A)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.M)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.I)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.M)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 5:
                            if (Led_List[i] == (int)Num_En.E)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 6:
                            if (Led_List[i] == (int)Num_En.L)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 7:
                            if (Led_List[i] == (int)Num_En.O)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 8:
                            if (Led_List[i] == (int)Num_En.N)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                    }
                    break;
                case 29:
                    switch (LedNum_Index)
                    {
                        case 0:
                            if (Led_List[i] == (int)Num_En.G)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 1:
                            if (Led_List[i] == (int)Num_En.O)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 2:
                            if (Led_List[i] == (int)Num_En.O)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 3:
                            if (Led_List[i] == (int)Num_En.D)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 4:
                            if (Led_List[i] == (int)Num_En.L)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;

                        case 5:
                            if (Led_List[i] == (int)Num_En.U)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 6:
                            if (Led_List[i] == (int)Num_En.C)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;
                        case 7:
                            if (Led_List[i] == (int)Num_En.K)
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Target;
                            }
                            else
                            {
                                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
                                GameLedControl.gamePoint[4 + i].statue = enPointSta.Die;
                            }
                            break;

                    }
                    break;


            }
            if (Led_List[i] == 127)
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);
                GameLedControl.gamePoint[4 + i].statue = enPointSta.None;
            }
        }
    }
    void Update_LED_DianZhen_Data()
    {

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] == 127)
            {
                Framebuffer.Update_TargetLedColor_DianZhen(i, 127, 0);

                continue;
            }
            Framebuffer.Update_TargetLedColor_DianZhen(i, (byte)Led_List[i], Led_List_Color[i]);

#if UNITY_EDITOR
            // 只有在Unity编辑器中才会执行的代码
            //    texts[i].text = Led_List[i].ToString();
            //   texts[i].color = UIntToColor(Led_List_Color[i]);
#endif
        }
    }
    void RunMap_00()
    {


        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LEDNum_SmallToBig();

        if (LedNum_Index > MaxLedNum)
        {
            isClearAll = true;
        }
    }
    void RunMap_01()
    {
        DevEyes_WaitCheck();
        Update_LedTarage();

        isPass_AllTarage();



    }
    void RunMap_02()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();



        isPass_AllTarage();

    }
    //
    void RunMap_03()
    {
        DevEyes_IdleCountDown();
        Update_LEDNum_BigTOSmall();


        if (LedNum_Index <= 0)
        {
            isClearAll = true;
        }
        //Run_RedPass();
    }
    void RunMap_04()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            isClearAll = true;
        }
        DevEyes_IdleCountDown();


        Update_LEDNum_SmallToBig();

        if (LedNum_Index > MaxLedNum)
        {
            isClearAll = true;
        }
    }
    void RunMap_05()
    {

        DevEyes_IdleCountDown();
        Update_LEDNum_Double();

        isPassDouble();
    }

    bool isPassFive()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] % 5 == 0)
            {

                return false;
            }

        }
        isClearAll = true;
        return true;
    }
    bool isPassDouble()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] % 2 == 0)
            {

                return false;
            }

        }
        isClearAll = true;
        return true;
    }
    bool isPassThird()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] % 3 == 0)
            {

                return false;
            }

        }
        isClearAll = true;
        return true;
    }
    bool isAll127()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] != 127)
            {

                return false;
            }

        }
        isClearAll = true;
        return true;
    }
    void RunMap_06()
    {

        DevEyes_IdleCountDown();
        Update_LedTarages();
        if (iswait) waittime += Time.deltaTime;
        if (waittime > 0.5f && iswait)
        {
            GetNum_First = GetNum_Scend = -1;
            GetNum_Firstpoint = GetNum_Scendpoint = -1;
            if (FjData.g_Fj[0].Life > 0)
            {

                FjData.g_Fj[0].Life--;
                if (FjData.g_Fj[0].Scores > 0)
                {
                    FjData.g_Fj[0].Scores--;
                }
                else
                {
                    FjData.g_Fj[0].Scores = 0;
                }
                MusicManager.instance.Play_Fails();
                istarage = 0;
                iswait = false;
            }
            else
            {
                FjData.g_Fj[0].Life = 0;
                istarage = 0;
                iswait = false;
            }

        }
        isAll127();
    }

    void RunMap_07()
    {
        DevEyes_IdleCountDown();
        for (int i = 0; i < Led_List.Length; i++)
        {

            if (Led_List[i] % 5 != 0)
            {

                if (Led_List[i] == 127)
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.None);

                }
                else
                {
                    Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);

                }

            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);

            }
        }

        isPassFive();
    }

    void RunMap_08()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();
        // DevEyes_WaitCheck();
        isPass_AllTarage();

    }
    void RunMap_09()
    {

        DevEyes_IdleCountDown();
        Update_LEDNum_Third();

        isPass_AllTarage();
    }
    void RunMap_10()
    {
        DevEyes_IdleCountDown();
        Update_LEDNum_BigTOSmall();


        if (LedNum_Index <= 0)
        {
            isClearAll = true;
        }
    }
    void RunMap_11()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();

        isPass_AllTarage();

        //Run_RedPass();
    }
    void RunMap_12()
    {
        DevEyes_IdleCountDown();
        //Update_LEDNum_Third();
        Update_LedTarage();
        isPass_AllTarage();

    }
    void RunMap_13()
    {
        DevEyes_IdleCountDown();
        //Update_LEDNum_Third();
        Update_LedTarage();
        isPass_AllTarage();


    }

    void RunMap_14()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();

        isPass_AllTarage();

    }
    void RunMap_15()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();

        isPass_AllTarage();
    }
    void RunMap_16()
    {
        DevEyes_IdleCountDown();
        // DevEyes_WaitCheck();
        Update_LedTarage();
        if (isTarageList[7] || isTarageList[13])
        {
            isPass_AllTarage();

        }



    }
    void RunMap_17()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();

        isPass_AllTarage();
    }
    void RunMap_18()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();

        isPass_AllTarage();
    }
    void RunMap_19()
    {
        DevEyes_IdleCountDown();
        Update_LedTarage();
        //  DevEyes_WaitCheck();
        isPass_AllTarage();
    }

    void RunMap_20()
    {

        //Update_LedTarage();
        DevEyes_WaitCheck();
        Update_LedLetter();
        //isPass_AllTarage();
        if (LedNum_Index > 2)
        {
            isClearAll = true;
        }
    }
    void RunMap_21()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        //isPass_AllTarage();
        if (LedNum_Index > 4)
        {
            isClearAll = true;
        }

    }
    void RunMap_22()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        //isPass_AllTarage();
        if (LedNum_Index > 5)
        {
            isClearAll = true;
        }
    }
    void RunMap_23()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        //isPass_AllTarage();
        if (LedNum_Index > 4)
        {
            isClearAll = true;
        }
    }
    void RunMap_24()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        //isPass_AllTarage();
        if (LedNum_Index > 4)
        {
            isClearAll = true;
        }
    }
    void RunMap_25()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        if (LedNum_Index > 8)
        {
            isClearAll = true;
        }
    }
    void RunMap_26()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        if (LedNum_Index > 9)
        {
            isClearAll = true;
        }
    }

    void RunMap_27()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        if (LedNum_Index > 5)
        {
            isClearAll = true;
        }
    }

    void RunMap_28()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        if (LedNum_Index > 8)
        {
            isClearAll = true;
        }
    }
    void RunMap_29()
    {
        DevEyes_IdleCountDown();
        //Update_LedTarage();
        Update_LedLetter();
        if (LedNum_Index > 7)
        {
            isClearAll = true;
        }
    }


    void GetMovePos()
    {
        for (int i = 0; i < MoveAllStep; i++)
        {
            MovePos[i] = Random.Range(0, MaxLedNum);
            for (int k = 0; k < i; k++)
            {
                while (MovePos[i] == MovePos[k])
                {
                    MovePos[i] = Random.Range(0, MaxLedNum);
                }
            }

        }
    }

    void GetTarageColor()
    {
        int a = Random.Range(0, tab_PointColor.Length);
        for (int i = 0; i < Tarage_Colors.Length; i++)
        {
            if (i == 0)
            {
                Tarage_Colors[i] = tab_PointColor[0];

            }
            else
            {
                Tarage_Colors[i] = tab_PointColor[a];

            }


        }

    }
    void GetTarageColorOne()
    {
        int a = Random.Range(0, tab_PointColor.Length);
        for (int i = 0; i < Tarage_Colors.Length; i++)
        {

            Tarage_Colors[i] = tab_PointColor[a];


        }

    }
    bool isPass_AllTarage()
    {


        for (int i = 0; i < isTarageList.Length; i++)
        {
            if (isTarageList[i])
            {
                Debug.LogError(i + "     " + (isTarageList[i]));

                return false;

            }
        }

        isClearAll = true;
        return true;
    }

    bool sss = false;
    // Update is called once per frame
    void Update()
    {

        if (Game05_Main.instance.statue != en_Game00_Sta.Play)
        {
            return;
        }
        waitTime -= Time.deltaTime;

        if (waitTime > 0)
        {
            return;
        }
        SafeTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.L))
        {

            sss = true;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            sss = false;
        }
        //for (int i = 0; i < 4; i++)
        //{



        //    if (sss)
        //    {

        //        DrawPic.DrawPointId(i, 0x400000, enPointSta.Die);
        //    }
        //    else
        //    {
        //      DrawPic.DrawPointId(i, 0x00ff00, enPointSta.Rest);
        //    }
        //}

        Checking_DevEeys();



        CheckLedKey();

        switch (Game05_Main.instance.gameLevel)
        {
            case 0:
                RunMap_00();

                break;
            case 1:
                RunMap_01();


                break;
            case 2:
                RunMap_02();

                break;

            //
            case 3:
                RunMap_03();


                //     
                break;
            case 4:
                RunMap_04();


                //   
                break;
            case 5:
                RunMap_05();


                //   
                break;
            //
            case 6:
                // RunMap_06();
                RunMap_10();


                break;
            case 7:
                RunMap_07();


                break;
            case 8:
                RunMap_08();


                break;
            case 9:
                RunMap_09();


                break;
            case 10:
                RunMap_10();


                break;
            case 11:
                RunMap_11();


                break;
            case 12:
                RunMap_12();


                break;
            case 13:
                RunMap_13();


                break;
            case 14:
                RunMap_14();


                break;
            case 15:
                RunMap_15();


                break;
            case 16:
                RunMap_16();


                break;
            case 17:
                RunMap_17();


                break;
            case 18:
                RunMap_18();


                break;
            case 19:
                RunMap_19();


                break;
            case 20:
                RunMap_20();


                break;
            case 21:
                RunMap_21();


                break;
            case 22:
                RunMap_22();


                break;
            case 23:
                RunMap_23();


                break;
            case 24:
                RunMap_24();


                break;
            case 25:
                RunMap_25();


                break;
            case 26:
                RunMap_26();


                break;
            case 27:
                RunMap_27();


                break;
            case 28:
                RunMap_28();


                break;
            case 29:
                RunMap_29();


                break;

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

    Color UIntToColor(uint rgba)
    {
        // 使用位操作提取各个颜色通道的值
        byte r = (byte)((rgba >> 24) & 0xFF); // 红色分量
        byte g = (byte)((rgba >> 16) & 0xFF); // 绿色分量
        byte b = (byte)((rgba >> 8) & 0xFF);  // 蓝色分量
        //byte a = (byte)(rgba & 0xFF);         // Alpha分量
        // 返回一个新的Color结构体实例
        return new Color32(r, g, b, 0x000025);
    }
}
