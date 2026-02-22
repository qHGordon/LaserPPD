using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Map04 : MonoBehaviour
{
    enum Num_En
    {
        A = 100,
        B,
        C, D, E, F, G, H, I, J, K, L, M, N, O, P,
        Q, R, S, T, U, V, W, X, Y, Z
    }
    public static Game_Map04 instance;
    int picId = 0;
    int pointId = 0;
    int jieduan = 0;
    readonly uint[] tab_PointColor = { 0xff0000, 0x00ff00, 0x0000ff, 0xD716AB, 0xfff700 };

    public int[] Tarage_Colors = new int[4];///5个篮筐
    public int[] Led_List = new int[5];//5个篮筐
    public uint[] Led_List_Color = new uint[5];//5个篮筐

    public int[] Eyes_NextIndex = { 0, 0, 0, 0 };//5个篮筐


    int MaxLedNum = 5;

    public int remainPoint = 0;
    public int remainPoint1 = 0;
    public int remainPoint2 = 0;


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

    public bool isMoving = false;
    public int[] MovePos = new int[300];
    public int MoveIndex = 0;
    public int MoveCnt = 0;
    public int MoveAllStep = 10;
    public int tarageNum = 3;

    public int RedLED_Index = 0;
    bool isDouble = true;


    float IdleTime = 2;
    float MaxIdleTime = 2;
    float waitTime = 2;



    //
    //
    bool[] isTarageList = new bool[20];


    //魔眼启动
    bool isStartChecking_DevEeys = false;
    bool isWaitChecking_DevEeys = false;

    //
    float RedStayTime = 4f;
    public float MaxRedStayTime = 4f;//魔眼检测中的    持续时间
                                     //
    float waitCheckTime = 0.4f;
    const float MaxwaitCheckTime = 0.4f;//魔眼检测的   准备时间

    int DevEyes_Index = 0;//下一个闪缩的是哪个魔眼
    float EyesWaitTime = 10;//魔眼当前还有多少检测时间
    float MaxEyesWaitTime = 10;//魔眼最大每轮的检测时间是多少



    int LedNum_Index = 0;//
    int GetNum_First = -1;
    int GetNum_Scend = -1;
    // Use this for initialization
    private void Awake()
    {
        instance = this;
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
        MaxJieDuan = 1;
        waitTime = 2;
        index = index1 = index2 = 0;
        picId = 0;
        pointId = 0;
        RedLED_Index = 0;
        MoveIndex = 0;
        LedNum_Index = 0;
        isDouble = true;
        jieduan = 0;
        MoveTime = MaxMoveTime;
        IdleTime = MaxIdleTime = 2;
        waitCheckTime = MaxwaitCheckTime;
        isMoving = false;
        isWaitChecking_DevEeys = false;
        isStartChecking_DevEeys = false;
        GetNum_First = -1;
        GetNum_Scend = -1;
        remainPoint = remainPoint1 = remainPoint2 = 10;

        Init_LedListColor();
        IdleTime = MaxIdleTime;
        Game04_Main.instance.remainTime = 120;
        ClearMap();
        for (int i = 0; i < isTarageList.Length; i++)
        {
            isTarageList[i] = false;
        }
        for (int i = 0; i < Led_List.Length; i++)
        {
            Led_List[i] = -1;
        }
        for (int i = 0; i < Tarage_Colors.Length; i++)
        {
            Tarage_Colors[i] = 0;
        }

        for (int i = 0; i < Game04_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(false);
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


        Invoke("InitMap_" + Id.ToString("D2"), 0);

    }
    void Start()
    {

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

        for (int i = 1; i < 5; i++)
        {
            bool isSame = true;
            while (isSame)
            {
                int index = Random.Range(0, Led_List.Length);
                if (Led_List[index] != i * 2)
                {
                    Led_List[index] = i * 2;
                    isSame = false;
                }


            }
        }

        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] < 0)
            {
                int aa = Random.Range(1, 10);
                while (aa % 2 == 0)
                {
                    aa = Random.Range(1, 10);
                }
                Led_List[i] = aa;

            }
        }
    }

    void InitMap_00()
    {
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;

        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "蓝色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 0, 1);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "5个";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 0, 0);
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "Blue";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "5 ";

        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 5;

        Game04_Main.instance.remainTime = 60;
        Led_List_Color[0] = 255;
        Led_List[0] = 1;
    }
    void InitMap_00_next()
    {
        waitTime = 2;
        MoveIndex++;
        if (MoveIndex > 4)
        {
            MoveIndex = 0;
        }
        IdleTime = MaxIdleTime = 2;

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        Led_List_Color[MoveIndex] = 255;
        Led_List[MoveIndex] = 1;
    }
    void InitMap_01_next()
    {
        waitTime = 2;

        IdleTime = MaxIdleTime = 2;

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        MoveIndex--;
        if (MoveIndex < 0)
        {
            MoveIndex = 4;
        }
        Led_List_Color[MoveIndex] = tab_PointColor[1];
        Led_List[MoveIndex] = 1;
    }
    void InitMap_01()
    {
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;

        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " 绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "8个";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 0);

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "8";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green";
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 8;

        Game04_Main.instance.remainTime = 60;
        MoveIndex = 4;
        Led_List_Color[MoveIndex] = tab_PointColor[1];
        Led_List[MoveIndex] = 1;
    }
    void InitMap_02()
    {
        MaxIdleTime = 2f;
        IdleTime = MaxIdleTime;
        LedNum_Index = 0;
        Game04_Main.instance.remainTime = 120;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " 黄色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(1, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "8个";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 1, 0);
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "8";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Yellow";
        }
        MaxJieDuan = 1;
        remainPoint = 8;
    }

    int index, index1, index2 = 0;

    void GetOneColor(int[] tarageList)
    {
        index = Random.Range(0, 0 + MaxLedNum);

        GameLedControl.gamePoint[index].statue = enPointSta.Target;

        DrawPic.DrawPointId(index, tab_PointColor[tarageList[jieduan]], enPointSta.Target);




        Led_List_Color[index] = tab_PointColor[tarageList[jieduan]];

        Led_List[index] = 1;


        for (int i = 0; i < 0 + MaxLedNum; i++)
        {
            if (i == index)
            {
                continue;
            }
            GameLedControl.gamePoint[i].statue = enPointSta.Die;
            GameLedControl.gamePoint[i].statue = enPointSta.Die;
            int color = Random.Range(0, Tarage_Colors.Length);
            while (color == tarageList[jieduan])
            {
                color = Random.Range(0, Tarage_Colors.Length);
            }
            Led_List_Color[i] = tab_PointColor[color];

            DrawPic.DrawPointId(i, tab_PointColor[color], enPointSta.Die);
        }
    }
    void GetTwoColor(int tarageIndex)
    {
        index = index1 = Random.Range(0, 0 + MaxLedNum);
        if (jieduan == 0)
        {
            while (index == index1)
            {
                index1 = Random.Range(0, 0 + MaxLedNum);
            }
            GameLedControl.gamePoint[index].statue = enPointSta.Target;
            GameLedControl.gamePoint[index1].statue = enPointSta.Target;
            DrawPic.DrawPointId(index, tab_PointColor[tarageIndex], enPointSta.Target);
            DrawPic.DrawPointId(index1, tab_PointColor[tarageIndex], enPointSta.Target);



            Led_List_Color[index] = tab_PointColor[tarageIndex];
            Led_List_Color[index1] = tab_PointColor[tarageIndex];

            Led_List[index] = 1;
            Led_List[index1] = 1;

            for (int i = 0; i < 0 + MaxLedNum; i++)
            {
                if (i == index || i == index1)
                {
                    continue;
                }
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                int color = Random.Range(0, Tarage_Colors.Length);
                while (color == tarageIndex)
                {
                    color = Random.Range(0, Tarage_Colors.Length);
                }
                Led_List_Color[i] = tab_PointColor[color];

                DrawPic.DrawPointId(i, tab_PointColor[color], enPointSta.Die);
            }
        }
        else
        {
            // Debug.LogError("index    "+ index+"      "+ index1);
            for (int i = 0; i < 0 + MaxLedNum; i++)
            {
                if (i != index || i != index1)
                {
                    GameLedControl.gamePoint[i].statue = enPointSta.Target;
                    GameLedControl.gamePoint[i].statue = enPointSta.Target;
                    DrawPic.DrawPointId(i, tab_PointColor[tarageIndex], enPointSta.Target);

                    Led_List[i] = 1;
                    Led_List_Color[i] = tab_PointColor[tarageIndex];

                }
                else
                {
                    int color = Random.Range(0, tab_PointColor.Length);
                    while (color == tarageIndex)
                    {
                        color = Random.Range(0, tab_PointColor.Length);
                    }
                    Led_List_Color[i] = tab_PointColor[color];
                    Led_List[i] = 0;


                    DrawPic.DrawPointId(i, tab_PointColor[color], enPointSta.Die);
                }


            }
        }

    }
    void GetTwoColor_Random(int tarageIndex, int tarageIndex1)
    {
        index = index1 = Random.Range(0, 0 + MaxLedNum);
        if (jieduan == 0)
        {
            while (index == index1)
            {
                index1 = Random.Range(0, 0 + MaxLedNum);
            }
            GameLedControl.gamePoint[index].statue = enPointSta.Target;
            GameLedControl.gamePoint[index1].statue = enPointSta.Target;
            DrawPic.DrawPointId(index, tab_PointColor[tarageIndex], enPointSta.Target);
            DrawPic.DrawPointId(index1, tab_PointColor[tarageIndex1], enPointSta.Target);



            Led_List_Color[index] = tab_PointColor[tarageIndex];
            Led_List_Color[index1] = tab_PointColor[tarageIndex1];

            Led_List[index] = 1;
            Led_List[index1] = 1;

            for (int i = 0; i < 0 + MaxLedNum; i++)
            {
                if (i == index || i == index1)
                {
                    continue;
                }
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                int color = Random.Range(0, Tarage_Colors.Length);
                while (color == tarageIndex || color == tarageIndex1)
                {
                    color = Random.Range(0, Tarage_Colors.Length);
                }
                Led_List_Color[i] = tab_PointColor[color];

                DrawPic.DrawPointId(i, tab_PointColor[color], enPointSta.Die);
            }
        }


    }
    void GetThirdColor(int tarageIndex, int tarageIndex1)
    {
        index = index1 = index2 = Random.Range(0, 0 + MaxLedNum);
        if (jieduan == 0)
        {

            while (index == index1)
            {
                index1 = Random.Range(0, 0 + MaxLedNum);
            }
            while (index2 == index1 || index == index2)
            {
                index2 = Random.Range(0, 0 + MaxLedNum);
            }
            GameLedControl.gamePoint[index].statue = enPointSta.Target;
            GameLedControl.gamePoint[index1].statue = enPointSta.Target;
            GameLedControl.gamePoint[index2].statue = enPointSta.Target;
            DrawPic.DrawPointId(index, tab_PointColor[tarageIndex], enPointSta.Target);
            DrawPic.DrawPointId(index1, tab_PointColor[tarageIndex], enPointSta.Target);
            DrawPic.DrawPointId(index2, tab_PointColor[tarageIndex], enPointSta.Target);



            Led_List_Color[index] = tab_PointColor[tarageIndex];
            Led_List_Color[index1] = tab_PointColor[tarageIndex];
            Led_List_Color[index2] = tab_PointColor[tarageIndex];

            Led_List[index] = 1;
            Led_List[index1] = 1;
            Led_List[index2] = 1;

            for (int i = 0; i < 0 + MaxLedNum; i++)
            {
                if (i == index || i == index1 || i == index2)
                {
                    continue;
                }
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                int color = Random.Range(0, tab_PointColor.Length);
                while (color == tarageIndex)
                {
                    color = Random.Range(0, tab_PointColor.Length);
                }
                Led_List_Color[i] = tab_PointColor[color];

                DrawPic.DrawPointId(i, tab_PointColor[color], enPointSta.Die);
            }
        }
        else
        {
            for (int i = 0; i < 0 + MaxLedNum; i++)
            {
                if (i != index || i != index1 | i != index2)
                {
                    GameLedControl.gamePoint[i].statue = enPointSta.Target;
                    GameLedControl.gamePoint[i].statue = enPointSta.Target;
                    DrawPic.DrawPointId(i, tab_PointColor[tarageIndex1], enPointSta.Target);

                    Led_List[i] = 1;
                    Led_List_Color[i] = tab_PointColor[tarageIndex1];

                }
                else
                {
                    int color = Random.Range(0, Tarage_Colors.Length);
                    while (color == tarageIndex1)
                    {
                        color = Random.Range(0, Tarage_Colors.Length);
                    }

                    Led_List[i] = 0;
                    Led_List_Color[i] = tab_PointColor[color];

                    DrawPic.DrawPointId(i, tab_PointColor[color], enPointSta.Die);
                }


            }
        }

    }
    int[] group = new int[5];
    int[] group1 = new int[5];
    int[] group2 = new int[5];
    void GetThirdColor_Random(int tarageIndex, int tarageIndex1, int tarageIndex2)
    {
        index = Random.Range(1, 4);
        if (Game04_Main.instance.gameLevel > 20)
        {
            index = 1;
        }
        index1 = Random.Range(1, 5 - index);
        index2 = 5 - index - index1;



        for (int i = 0; i < index; i++)
        {
            group[i] = Random.Range(0, 0 + MaxLedNum);
        }



        for (int i = 0; i < index1; i++)
        {
            group1[i] = Random.Range(0, 0 + MaxLedNum);
            while (group1[i] == group[i])
            {
                group1[i] = Random.Range(0, 0 + MaxLedNum);

            }
        }
        for (int i = 0; i < index; i++)
        {
            Led_List[group[i]] = 1;
            GameLedControl.gamePoint[group[i]].statue = enPointSta.Target;
            GameLedControl.gamePoint[group[i]].Color = tab_PointColor[tarageIndex];
            DrawPic.DrawPointId(group[i], tab_PointColor[tarageIndex], enPointSta.Target);
            Led_List_Color[group[i]] = tab_PointColor[tarageIndex];


        }
        Tarage_Colors[0] = tarageIndex;
        if (tarageIndex1 >= 0)
        {
            for (int i = 0; i < group1.Length; i++)
            {
                Led_List[group1[i]] = 1;

                GameLedControl.gamePoint[group1[i]].statue = enPointSta.Target;
                GameLedControl.gamePoint[group1[i]].Color = tab_PointColor[tarageIndex1];

                DrawPic.DrawPointId(group1[i], tab_PointColor[tarageIndex1], enPointSta.Target);
                Led_List_Color[group1[i]] = tab_PointColor[tarageIndex1];
            }
            Tarage_Colors[1] = tarageIndex1;

        }

        if (tarageIndex2 >= 0)
        {
            for (int i = 0; i < group2.Length; i++)
            {

                Led_List[group2[i]] = 1;

                GameLedControl.gamePoint[group2[i]].statue = enPointSta.Target;
                GameLedControl.gamePoint[group2[i]].Color = tab_PointColor[tarageIndex2];

                DrawPic.DrawPointId(group2[i], tab_PointColor[tarageIndex2], enPointSta.Target);
                Led_List_Color[group2[i]] = tab_PointColor[tarageIndex2];

            }
            Tarage_Colors[2] = tarageIndex2;

        }
        for (int i = 0; i < MaxLedNum; i++)
        {
            if (GameLedControl.gamePoint[i].statue != enPointSta.Target)
            {


                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                int color = Random.Range(0, tab_PointColor.Length);
                while (color == tarageIndex || color == tarageIndex1 || color == tarageIndex2)
                {
                    color = Random.Range(0, tab_PointColor.Length);
                }
                GameLedControl.gamePoint[i].Color = tab_PointColor[color];

                Led_List[i] = 0;
                Led_List_Color[i] = tab_PointColor[color];
                DrawPic.DrawPointId(i, tab_PointColor[color], enPointSta.Die);

            }

            //            Debug.LogError(GameLedControl.gamePoint[i].Color+"    "+i);
        }


    }


    void InitMap_03_next()
    {
        waitTime = 2;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;

        GetTwoColor(2);


    }
    void InitMap_03()
    {
        waitTime = 2;
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);

            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Tarage:  Blue ";

        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        Game04_Main.instance.remainTime = 120;
        GetTwoColor(2);


    }
    void InitMap_04()
    {
        waitTime = 2;

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Tarage:  Green ";


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        Game04_Main.instance.remainTime = 120;
        GetTwoColor(1);
    }
    void InitMap_04_next()
    {
        waitTime = 2;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;

        GetTwoColor(1);
    }
    void InitMap_05()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Tarage:  PurPle ";

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        Game04_Main.instance.remainTime = 120;
        GetThirdColor(3, 1);
    }
    void InitMap_05_next()
    {
        waitTime = 2;

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;

        GetThirdColor(2, 3);
    }



    void InitMap_06()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "消灭紫色";
        //Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(1, 0, 1);
        //Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        //Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "10个";
        //Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 0,1);

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);

            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Tarage:  PurPle ";
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        Game04_Main.instance.remainTime = 120;
        remainPoint = 10;
        GetTwoColor(3);

    }
    void InitMap_06_next()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "紫色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(1, 0, 1);

        if (Set.setVal.Language == 1)
        {

            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " purple";
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;

        GetTwoColor(3);

    }
    void InitMap_07()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "20个";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 0);
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Num:20";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green";
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        Game04_Main.instance.remainTime = 120;
        remainPoint = 20;
        GetTwoColor(1);
    }
    void InitMap_07_next()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);
        if (Set.setVal.Language == 1)
        {

            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green";
        }

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;

        GetTwoColor(1);
    }

    void InitMap_08()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "蓝色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(1, 0, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "紫色";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 0, 1);
        Game04_Main.instance.gameUIComm.txt_TarageList[4].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "共30个";
        Game04_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(0, 1, 1);

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Blue ";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " purple";
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "Total  30";

        }
        remainPoint = 30;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;

        Game04_Main.instance.remainTime = 40;
        GetTwoColor_Random(2, 3);
    }
    void InitMap_08_next()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "蓝色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(1, 0, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "紫色";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 0, 1);

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Blue";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Purple";
        }

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        MaxJieDuan = 1;

        GetTwoColor_Random(2, 3);
    }

    void InitMap_09()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "黄色";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 1, 0);
        Game04_Main.instance.gameUIComm.txt_TarageList[4].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "共20个";
        Game04_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(0, 1, 1);
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Yellow";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green And ";
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = " 20";
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        Game04_Main.instance.remainTime = 20;
        remainPoint = 30;
        GetTwoColor_Random(1, 4);
    }
    void InitMap_09_next()
    {
        waitTime = 1;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "黄色";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 1, 0);

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Yellow ";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green And";
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;

        GetTwoColor_Random(1, 4);
    }
    void InitMap_10()
    {
        waitTime = 2;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "蓝色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 0, 1);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "10个";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 0, 0);
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "10";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Blue";
        }
        IdleTime = MaxIdleTime = Random.Range(4, 8);
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 10;
        //    Game04_Main.instance.isClearTarage = false;
        GetThirdColor_Random(2, -1, -1);
    }
    void InitMap_10_next()
    {
        waitTime = 1;
        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "蓝色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 0, 1);
        if (Set.setVal.Language == 1)
        {

            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Blue";
        }

        IdleTime = MaxIdleTime = Random.Range(4, 8);
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;

        //   Game04_Main.instance.isClearTarage = false;
        GetThirdColor_Random(2, -1, -1);
    }
    void InitMap_11()
    {
        waitTime = 2;
        IdleTime = MaxIdleTime = Random.Range(4, 8);

        Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "15个";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 1);

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "15";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green";
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 15;

        Game04_Main.instance.remainTime = 60;
        GetThirdColor_Random(1, -1, -1);
    }
    void InitMap_11_next()
    {
        waitTime = 2;

        IdleTime = MaxIdleTime = Random.Range(4, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        GetThirdColor_Random(1, -1, -1);
    }
    void InitMap_12()
    {
        IdleTime = MaxIdleTime = Random.Range(4, 8);

        if (Set.setVal.Language == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
            }
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "10";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Purple";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "5";
            Game04_Main.instance.gameUIComm.txt_TarageList[2].text = "Yellow";
        }
        else
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "紫色";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(1, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "10个";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(1, 0, 1);


            Game04_Main.instance.gameUIComm.txt_TarageList[5].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "黄色";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].color = new Color(1, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[6].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "5个";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].color = new Color(1, 1, 0);


        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 3;
        remainPoint1 = 10;
        Game04_Main.instance.remainTime = 60;
        GetThirdColor_Random(4, 3, -1);
    }
    void InitMap_12_next()
    {
        IdleTime = MaxIdleTime = Random.Range(4, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;


        GetThirdColor_Random(4, 3, -1);
    }
    void InitMap_13()
    {
        IdleTime = MaxIdleTime = Random.Range(4, 8);

        if (Set.setVal.Language == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
            }
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "6";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "11";
            Game04_Main.instance.gameUIComm.txt_TarageList[2].text = "Blue";
        }
        else
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "6个";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 0);


            Game04_Main.instance.gameUIComm.txt_TarageList[5].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "蓝色";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].color = new Color(0, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[6].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "11个";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].color = new Color(1, 0, 0);

        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 6;
        remainPoint1 = 11;
        Game04_Main.instance.remainTime = 60;
        GetThirdColor_Random(1, 2, -1);
    }
    void InitMap_13_next()
    {
        IdleTime = MaxIdleTime = Random.Range(4, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;


        GetThirdColor_Random(1, 2, -1);
    }
    void InitMap_14()
    {
        MaxJieDuan = 1;
        IdleTime = MaxIdleTime = Random.Range(4, 8);
        if (Set.setVal.Language == 1)
        {
            for (int i = 0; i < 6; i++)
            {
                Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
            }
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "3";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green";

            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "5";
            Game04_Main.instance.gameUIComm.txt_TarageList[2].text = "Blue";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "4";
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "Purple";
        }
        else
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "3个";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[3].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "蓝色";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(0, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[4].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "5个";
            Game04_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(1, 0, 0);


            Game04_Main.instance.gameUIComm.txt_TarageList[5].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "紫色";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].color = new Color(1, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[6].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "4个";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].color = new Color(1, 0, 1);

        }



        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        remainPoint = 3;
        remainPoint1 = 5;
        remainPoint2 = 4;
        Game04_Main.instance.remainTime = 60;
        GetThirdColor_Random(1, 2, 3);
    }
    void InitMap_14_next()
    {
        IdleTime = MaxIdleTime = Random.Range(4, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }


        GetThirdColor_Random(1, 2, 3);
    }

    void InitMap_15()
    {
        MaxJieDuan = 1;
        waitTime = 1;
        IdleTime = MaxIdleTime = Random.Range(4, 8);

        if (Set.setVal.Language == 1)
        {
            for (int i = 0; i < 6; i++)
            {
                Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
            }
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "3";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = " Green";

            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "6";
            Game04_Main.instance.gameUIComm.txt_TarageList[2].text = "Yellow";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "8";
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "Purple";
        }
        else
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[0].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[1].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "3个";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[3].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "黄色";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(1, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[4].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "6个";
            Game04_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(1, 1, 0);


            Game04_Main.instance.gameUIComm.txt_TarageList[5].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "紫色";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].color = new Color(1, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[6].gameObject.SetActive(true);
            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "8个";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].color = new Color(1, 0, 1);
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        remainPoint = 3;
        remainPoint1 = 6;
        remainPoint2 = 8;
        Game04_Main.instance.remainTime = 60;
        GetThirdColor_Random(1, 4, 3);
    }
    void InitMap_15_next()
    {
        waitTime = 1;
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        GetThirdColor_Random(1, 4, 3);
    }
    void InitMap_16()
    {
        IdleTime = MaxIdleTime = Random.Range(3, 8);
        for (int i = 0; i < Game04_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);

        }

        if (Set.setVal.Language == 1)
        {
            for (int i = 0; i < 10; i++)
            {
                Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
            }
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "Yellow";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Green";
            Game04_Main.instance.gameUIComm.txt_TarageList[2].text = " Green";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "Purple";

            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "Purple";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "Green";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "Yellow";
            Game04_Main.instance.gameUIComm.txt_TarageList[7].text = "Yellow";
            Game04_Main.instance.gameUIComm.txt_TarageList[8].text = "Blue";
            Game04_Main.instance.gameUIComm.txt_TarageList[9].text = "Purple";

        }
        else
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "黄色";
            Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(1, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "绿色";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[2].text = "绿色";
            Game04_Main.instance.gameUIComm.txt_TarageList[2].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "紫色";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(1, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "紫色";
            Game04_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(1, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "绿色";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].color = new Color(0, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "黄色";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].color = new Color(1, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[7].text = "黄色";
            Game04_Main.instance.gameUIComm.txt_TarageList[7].color = new Color(1, 1, 0);

            Game04_Main.instance.gameUIComm.txt_TarageList[8].text = "蓝色";
            Game04_Main.instance.gameUIComm.txt_TarageList[8].color = new Color(0, 0, 1);

            Game04_Main.instance.gameUIComm.txt_TarageList[9].text = "紫色";
            Game04_Main.instance.gameUIComm.txt_TarageList[9].color = new Color(1, 0, 1);

        }


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        remainPoint = 6;
        remainPoint1 = 8;
        Game04_Main.instance.remainTime = 60;
        int[] tarlist = { 4, 1, 1, 3, 3, 1, 4, 4, 1, 3 };
        GetOneColor(tarlist);
    }
    void InitMap_16_next()
    {
        jieduan++;
        if (jieduan >= 10)
        {
            isClearAll = true;
            return;
        }
        IdleTime = MaxIdleTime = Random.Range(3, 8);
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        int[] tarlist = { 4, 1, 1, 3, 3, 1, 4, 4, 1, 3 };
        GetOneColor(tarlist);
    }
    void InitMap_17()
    {
        IdleTime = MaxIdleTime = Random.Range(3, 8);
        for (int i = 0; i < Game04_Main.instance.gameUIComm.txt_TarageList.Length; i++)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);

        }
        Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[0].color = new Color(0, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "绿色";
        Game04_Main.instance.gameUIComm.txt_TarageList[1].color = new Color(0, 1, 0);



        Game04_Main.instance.gameUIComm.txt_TarageList[2].text = "紫色";
        Game04_Main.instance.gameUIComm.txt_TarageList[2].color = new Color(1, 0, 1);

        Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "紫色";
        Game04_Main.instance.gameUIComm.txt_TarageList[3].color = new Color(1, 0, 1);

        Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "黄色";
        Game04_Main.instance.gameUIComm.txt_TarageList[4].color = new Color(1, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "蓝色";
        Game04_Main.instance.gameUIComm.txt_TarageList[5].color = new Color(0, 0, 1);


        Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "蓝色";
        Game04_Main.instance.gameUIComm.txt_TarageList[6].color = new Color(0, 0, 1);

        Game04_Main.instance.gameUIComm.txt_TarageList[7].text = "黄色";
        Game04_Main.instance.gameUIComm.txt_TarageList[7].color = new Color(1, 1, 0);

        Game04_Main.instance.gameUIComm.txt_TarageList[8].text = "紫色";
        Game04_Main.instance.gameUIComm.txt_TarageList[8].color = new Color(1, 0, 1);
        Game04_Main.instance.gameUIComm.txt_TarageList[9].text = "黄色";
        Game04_Main.instance.gameUIComm.txt_TarageList[9].color = new Color(1, 1, 0);

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[0].text = "Green";
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "Green";
            Game04_Main.instance.gameUIComm.txt_TarageList[2].text = " Purple";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "Purple";

            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "Yellow";
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "Blue";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = "Blue";
            Game04_Main.instance.gameUIComm.txt_TarageList[7].text = "Yellow";
            Game04_Main.instance.gameUIComm.txt_TarageList[8].text = "Purple";
            Game04_Main.instance.gameUIComm.txt_TarageList[9].text = "Yellow";

        }


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        remainPoint = 6;
        remainPoint1 = 8;
        Game04_Main.instance.remainTime = 60;
        int[] tarlist = { 1, 1, 3, 3, 4, 2, 2, 4, 3, 4 };
        GetOneColor(tarlist);
    }
    void InitMap_17_next()
    {
        jieduan++;
        if (jieduan >= 10)
        {
            isClearAll = true;
            return;
        }
        IdleTime = MaxIdleTime = Random.Range(3, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        int[] tarlist = { 1, 1, 3, 3, 4, 2, 2, 4, 3, 4 };
        GetOneColor(tarlist);
    }
    void InitMap_18()
    {
        IdleTime = MaxIdleTime = Random.Range(3, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        remainPoint = 6;
        remainPoint1 = 8;
        Game04_Main.instance.remainTime = 60;
        int[] tarlist = { 4, 3, 1, 1, 2, 1, 4, 3, 2, 1 };
        for (int i = 0; i < tarlist.Length; i++)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
            switch (tarlist[i])
            {
                case 0:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "红色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 0);
                    break;
                case 1:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "绿色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 1, 0);
                    break;
                case 2:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "蓝色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 2, 0);
                    break;
                case 3:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "紫色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 1);
                    break;
                case 4:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "黄色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 1, 0);
                    break;
            }
            if (Set.setVal.Language == 1)
            {
                switch (tarlist[i])
                {
                    case 0:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Red";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 0);
                        break;
                    case 1:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Green";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 1, 0);
                        break;
                    case 2:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Blue";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 2, 0);
                        break;
                    case 3:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Purple";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 1);
                        break;
                    case 4:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Yellow";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 1, 0);
                        break;
                }

            }
        }
        GetOneColor(tarlist);
    }
    void InitMap_18_next()
    {
        jieduan++;
        if (jieduan >= 10)
        {
            isClearAll = true;
            return;
        }
        IdleTime = MaxIdleTime = Random.Range(3, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        int[] tarlist = { 4, 3, 1, 1, 2, 1, 4, 3, 2, 1 };

        GetOneColor(tarlist);
    }

    void InitMap_19_next()
    {
        jieduan++;
        if (jieduan >= 10)
        {
            isClearAll = true;
            return;
        }
        IdleTime = MaxIdleTime = Random.Range(3, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        int[] tarlist = { 4, 1, 3, 3, 2, 3, 1, 3, 2, 3 };

        GetOneColor(tarlist);
    }
    void InitMap_19()
    {
        IdleTime = MaxIdleTime = Random.Range(3, 8);

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        remainPoint = 6;
        remainPoint1 = 8;
        Game04_Main.instance.remainTime = 60;
        int[] tarlist = { 4, 1, 3, 3, 2, 3, 1, 3, 2, 3 };
        for (int i = 0; i < tarlist.Length; i++)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[i].gameObject.SetActive(true);
            switch (tarlist[i])
            {
                case 0:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "红色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 0);
                    break;
                case 1:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "绿色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 1, 0);
                    break;
                case 2:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "蓝色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 2, 0);
                    break;
                case 3:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "紫色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 1);
                    break;
                case 4:
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "黄色";
                    Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 1, 0);
                    break;
            }
            if (Set.setVal.Language == 1)
            {
                switch (tarlist[i])
                {
                    case 0:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Red";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 0);
                        break;
                    case 1:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Green";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 1, 0);
                        break;
                    case 2:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Blue";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(0, 2, 0);
                        break;
                    case 3:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Purple";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 0, 1);
                        break;
                    case 4:
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].text = "Yellow";
                        Game04_Main.instance.gameUIComm.txt_TarageList[i].color = new Color(1, 1, 0);
                        break;
                }

            }
        }
        GetOneColor(tarlist);
    }
    float ShowTime = 0;
    float MaxShowTime = 1f;
    int ShowIndex = 0;
    int[] tarlist_Over20;
    void Show_Next()
    {
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 3;
        ShowIndex++;

        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;

        Game04_Main.instance.remainTime = 60;
        if (ShowIndex >= tarlist_Over20.Length)
        {
            isMoving = false;
            ShowIndex = 0;
            GetThirdColor_Random(tarlist_Over20[ShowIndex], -1, -1);
            return;
        }




    }
    void InitMap_20()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 3;
        isMoving = true;
        tarlist_Over20 = new int[] { 0, 1, 2 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_20_next()
    {
        waitTime = 2;

        IdleTime = MaxIdleTime = 3;

        if (ShowIndex >= tarlist_Over20.Length)
        {
            isMoving = false;
            ShowIndex = 0;
            isClearAll = true;
            return;
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }

        GetThirdColor_Random(tarlist_Over20[ShowIndex], -1, -1);

    }
    void InitMap_21()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 3;
        isMoving = true;
        tarlist_Over20 = new int[] { 0, 3, 1, 2 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_22()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 3;
        isMoving = true;
        tarlist_Over20 = new int[] { 4, 3, 2, 1 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_23()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 3;
        isMoving = true;
        tarlist_Over20 = new int[] { 2, 3, 2, 1 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_24()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;
        isMoving = true;
        tarlist_Over20 = new int[] { 1, 4, 4, 2, 1 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_25()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;
        isMoving = true;
        tarlist_Over20 = new int[] { 0, 1, 2, 0, 1, 2 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_26()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;
        isMoving = true;
        tarlist_Over20 = new int[] { 4, 1, 2, 4, 3, 2 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_27()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;
        isMoving = true;
        tarlist_Over20 = new int[] { 0, 1, 2, 3, 4, 3, 2, 1 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_28()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;
        isMoving = true;
        tarlist_Over20 = new int[] { 1, 1, 4, 0, 1, 4, 3, 1 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_29()
    {
        ShowIndex = 0;
        ShowTime = MaxShowTime;
        waitTime = 2;
        IdleTime = MaxIdleTime = 2;
        isMoving = true;
        tarlist_Over20 = new int[] { 1, 3, 1, 4, 0, 2, 3, 1 };


        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }
        MaxJieDuan = 1;
        remainPoint = 30;
        Game04_Main.instance.remainTime = 60;
    }
    void InitMap_Over20_next()
    {
        waitTime = 2;

        IdleTime = MaxIdleTime = 3;

        if (ShowIndex >= tarlist_Over20.Length)
        {
            isMoving = false;
            ShowIndex = 0;
            isClearAll = true;
            return;
        }
        for (int i = 0; i < Led_List_Color.Length; i++)
        {
            Led_List_Color[i] = 0;
            Led_List[i] = 0;
        }


        GetThirdColor_Random(tarlist_Over20[ShowIndex], -1, -1);

    }
    void GetLED_TarageList(int TTNum)//获得一定数量的,和目标灯颜色的灯
    {
        int a = 0;
        int[] List = new int[20];
        for (int i = 0; i < List.Length; i++)
        {
            List[i] = -1;
        }
        for (int i = 0; i < TTNum; i++)
        {
            a = Random.Range(0, MaxLedNum);


            bool isSame = false;

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
            switch (Game04_Main.instance.gameLevel)
            {
                case 20:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.C;
                            Led_List_Color[a] = 0x0000ff;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x0000ff;

                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.T;
                            Led_List_Color[a] = 0x0000ff;
                            break;
                    }

                    break;
                case 21:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.K;
                            Led_List_Color[a] = 0x0000ff;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0x0000ff;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x0000ff;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0x0000ff;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0x0000ff;
                            break;
                    }

                    break;
                case 22:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0xff0000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.K;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.Y;
                            Led_List_Color[a] = 0xff0000;
                            break;
                    }

                    break;
                case 23:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0xff0000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.D;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0xff0000;
                            break;

                    }

                    break;
                case 24:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0xff0000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.P;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0xff0000;
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
                            Led_List_Color[a] = 0xff0000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0xff0000;
                            break;
                    }

                    break;
                case 28:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.H;
                            Led_List_Color[a] = 0xff0000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.A;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 2:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 3:
                            Led_List[a] = (int)Num_En.I;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 4:
                            Led_List[a] = (int)Num_En.M;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 5:
                            Led_List[a] = (int)Num_En.E;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 6:
                            Led_List[a] = (int)Num_En.L;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 7:
                            Led_List[a] = (int)Num_En.O;
                            Led_List_Color[a] = 0xff0000;
                            break;
                        case 8:
                            Led_List[a] = (int)Num_En.N;
                            Led_List_Color[a] = 0xff0000;
                            break;
                    }

                    break;
                case 29:
                    switch (i)
                    {
                        case 0:
                            Led_List[a] = (int)Num_En.K;
                            Led_List_Color[a] = 0xff0000;
                            isTarageList[a] = true;
                            break;
                        case 1:
                            Led_List[a] = (int)Num_En.U;
                            Led_List_Color[a] = 0xff0000;
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



    void CheckLedKey()
    {


        if (Main.IsDemo) { return; }
        if (waitCheckTime > 0)
        {
            waitCheckTime -= Time.deltaTime;

            return;
        }

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
#endif


        for (int i = 0; i < MaxLedNum; i++)
        {
            pointId = i;



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

                    if (FjData.g_Fj[0].Life > 0)
                    {


                        FjData.g_Fj[0].Life--;
                        if (Game04_Main.instance.Score_LinShi > 0)
                        {
                            Game04_Main.instance.Score_LinShi--;
                        }
                        else
                        {
                            Game04_Main.instance.Score_LinShi = 0;
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
                waitCheckTime = 0.4f;
                Game04_Main.instance.Score_LinShi += 10;

                MusicManager.instance.Play_Correct();

                if (Game04_Main.instance.gameLevel < 20)
                {
                    GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                    DrawPic.DrawPointId(pointId, 0, enPointSta.None);
                    Led_List[i] = -1;
                    Led_List_Color[i] = 0;
                }

                pressTargetPoint = false;

                switch (Game04_Main.instance.gameLevel)
                {
                    default: break;
                    case 0:
                        remainPoint--;
                        break;
                    case 1:

                        remainPoint--;
                        break;
                    case 2:

                        remainPoint--;
                        break;
                    case 3:


                        break;
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                        remainPoint--;
                        break;

                    case 12:

                        //for (int k = 0; k < 3; k++)
                        //{
                        //    Debug.LogError(tab_PointColor[Tarage_Colors[k]]);
                        //}
                        if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[0]])
                        {
                            remainPoint--;

                        }

                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[1]])
                        {
                            remainPoint1--;

                        }
                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[2]])
                        {
                            remainPoint2--;

                        }
                        if (remainPoint1 <= 0)
                        {
                            remainPoint1 = 0;
                        }
                        if (remainPoint2 <= 0)
                        {
                            remainPoint2 = 0;
                        }
                        if (remainPoint <= 0)
                        {
                            remainPoint = 0;
                        }
                        break;
                    case 13:

                        if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[0]])
                        {
                            remainPoint--;

                        }

                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[1]])
                        {
                            remainPoint1--;

                        }
                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[2]])
                        {
                            remainPoint2--;

                        }
                        if (remainPoint1 <= 0)
                        {
                            remainPoint1 = 0;
                        }
                        if (remainPoint2 <= 0)
                        {
                            remainPoint2 = 0;
                        }
                        if (remainPoint <= 0)
                        {
                            remainPoint = 0;
                        }
                        break;
                    case 14:
                        if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[0]])
                        {
                            remainPoint--;

                        }

                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[1]])
                        {
                            remainPoint1--;

                        }
                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[2]])
                        {
                            remainPoint2--;

                        }
                        if (remainPoint1 <= 0)
                        {
                            remainPoint1 = 0;
                        }
                        if (remainPoint2 <= 0)
                        {
                            remainPoint2 = 0;
                        }
                        if (remainPoint <= 0)
                        {
                            remainPoint = 0;
                        }
                        break;
                    case 15:
                        if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[0]])
                        {
                            remainPoint--;

                        }

                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[1]])
                        {
                            remainPoint1--;

                        }
                        else if (GameLedControl.gamePoint[pointId].Color == tab_PointColor[Tarage_Colors[2]])
                        {
                            remainPoint2--;

                        }
                        if (remainPoint1 <= 0)
                        {
                            remainPoint1 = 0;
                        }
                        if (remainPoint2 <= 0)
                        {
                            remainPoint2 = 0;
                        }
                        if (remainPoint <= 0)
                        {
                            remainPoint = 0;
                        }
                        break;
                    case 16:

                        InitMap_16_next();


                        break;
                    case 17:


                        InitMap_17_next();


                        break;
                    case 18:


                        InitMap_18_next();



                        break;
                    case 19:
                        InitMap_19_next();
                        break;
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:

                        remainPoint--;

                        if (remainPoint <= 0)
                        {
                            remainPoint = 0;
                        }
                        ShowIndex++;
                        InitMap_Over20_next();
                        break;

                }


            }
        }
    }



    void Update_LEDNum_SmallToBig()
    {

        for (int i = 0; i < Led_List.Length; i++)
        {

            if (Led_List[i] != LedNum_Index || !isTarageList[i])
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
            }
            else
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Target);

            }
        }



    }
    void Update_LEDNum_Double()
    {

        for (int i = 0; i < Led_List.Length; i++)
        {

            if (Led_List[i] % 2 != 0)
            {
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
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
                Framebuffer.Update_PointColor(4 + i, 0, enPointSta.Die);
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


            if (Led_List[i] > 0)
            {
                GameLedControl.gamePoint[i].statue = enPointSta.Target;
                DrawPic.DrawPointId(i, Led_List_Color[i], enPointSta.Target);

            }
            else if (Led_List[i] < 0)
            {
                GameLedControl.gamePoint[i].statue = enPointSta.None;
                DrawPic.DrawPointId(i, Led_List_Color[i], enPointSta.None);
            }
            else
            {
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                DrawPic.DrawPointId(i, Led_List_Color[i], enPointSta.Die);
            }
        }
    }
    void Update_LED_DianZhen_Data()
    {
        for (int i = 0; i < Led_List.Length; i++)
        {
            if (Led_List[i] == 127)
            {
                Framebuffer.Update_PointColor_DianZhen(4 + 20 + i, 0, 127, enPointSta.Rest);

                continue;
            }
            Framebuffer.Update_PointColor_DianZhen(4 + 20 + i, Led_List_Color[i], (uint)Led_List[i], enPointSta.Rest);


        }
    }

    void RunMap_00()
    {

        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();

        }

        if (isPass_AllTarage() || IdleTime < 0)
        {
            InitMap_00_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }

    }
    void RunMap_01()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "   Num: " + remainPoint.ToString();

        }

        if (isPass_AllTarage() || IdleTime < 0)
        {
            InitMap_01_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }
    }
    void RunMap_02()
    {
        IdleTime -= Time.deltaTime;
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "   Num: " + remainPoint.ToString();

        }

        if (IdleTime <= 0)
        {
            IdleTime = MaxIdleTime;
            MoveIndex++;
            if (MoveIndex > 2)
            {
                MoveIndex = 0;

            }
        }
        for (int i = 0; i < MaxLedNum; i++)
        {
            if ((i == 2 - MoveIndex) || (i == 2 + MoveIndex))
            {
                GameLedControl.gamePoint[i].statue = enPointSta.Target;
                DrawPic.DrawPointId(i, 0xffff00, enPointSta.Target);
            }
            else
            {
                GameLedControl.gamePoint[i].statue = enPointSta.Die;
                DrawPic.DrawPointId(i, 0, enPointSta.Die);


            }
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }
    }
    //
    void RunMap_03()
    {
        Update_LedTarage();
        if (jieduan == 0 && isPass_AllTarage())
        {
            jieduan++;
            InitMap_03_next();
        }
        else
        {
            isClearAll = isPass_AllTarage();
            ClearMap();
        }

    }
    void RunMap_04()
    {
        Update_LedTarage();
        if (jieduan == 0 && isPass_AllTarage())
        {
            jieduan++;
            InitMap_04_next();
        }
        else
        {
            isClearAll = isPass_AllTarage();
            ClearMap();
        }
    }
    void RunMap_05()
    {
        Update_LedTarage();
        if (jieduan == 0 && isPass_AllTarage())
        {
            jieduan++;
            InitMap_05_next();
        }
        else
        {
            isClearAll = isPass_AllTarage();
        }
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
        IdleTime -= Time.deltaTime;

        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();

        }

        Update_LedTarage();
        if (isPass_AllTarage() || IdleTime < 0)
        {
            IdleTime = 3f;
            InitMap_06_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }
    }

    void RunMap_07()
    {

        IdleTime -= Time.deltaTime;
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();

        }

        Update_LedTarage();
        if (isPass_AllTarage() || IdleTime < 0)
        {
            IdleTime = 3f;
            InitMap_07_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }

    }

    void RunMap_08()
    {

        IdleTime -= Time.deltaTime;
        Game04_Main.instance.gameUIComm.txt_TarageList[4].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "  Num: " + remainPoint.ToString();

        }

        Update_LedTarage();
        if (isPass_AllTarage() || IdleTime < 0)
        {
            IdleTime = 2f;
            InitMap_08_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }

    }
    void RunMap_09()
    {
        IdleTime -= Time.deltaTime;
        Game04_Main.instance.gameUIComm.txt_TarageList[4].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[4].text = "  Num: " + remainPoint.ToString();

        }

        Update_LedTarage();
        if (isPass_AllTarage() || IdleTime < 0)
        {
            IdleTime = 3f;
            InitMap_09_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }

    }
    void RunMap_10()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();

        }

        if (isPass_AllTarage() || IdleTime < 0)
        {
            IdleTime = 1.5f;
            InitMap_10_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }
    }
    void RunMap_11()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();

        }


        if (isPass_AllTarage() || IdleTime < 0)
        {
            InitMap_11_next();
        }
        if (remainPoint <= 0)
        {
            isClearAll = true;
        }
    }
    void RunMap_12()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "  Num: " + remainPoint1.ToString();

        }
        else
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint1.ToString() + " 个";
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = remainPoint.ToString() + " 个";
        }


        if (isPass_AllTarage() || IdleTime < 0)
        {
            InitMap_12_next();
        }

        if (remainPoint <= 0 && remainPoint1 <= 0)
        {

            isClearAll = true;
        }
    }
    void RunMap_13()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        Game04_Main.instance.gameUIComm.txt_TarageList[6].text = remainPoint1.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "  Num: " + remainPoint1.ToString();

        }
        else
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
            Game04_Main.instance.gameUIComm.txt_TarageList[6].text = remainPoint1.ToString() + " 个";
        }

        if (isPass_AllTarage() || IdleTime < 0)
        {
            InitMap_13_next();
        }
        if (remainPoint <= 0 && remainPoint1 <= 0)
        {
            isClearAll = true;
        }
    }

    void RunMap_14()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        Game04_Main.instance.gameUIComm.txt_TarageList[3].text = remainPoint1.ToString() + " 个";
        Game04_Main.instance.gameUIComm.txt_TarageList[5].text = remainPoint2.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "  Num: " + remainPoint1.ToString();
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "  Num: " + remainPoint2.ToString();

        }


        if (isPass_AllTarage() || IdleTime < 0)
        {
            InitMap_14_next();
        }
        if (remainPoint <= 0 && remainPoint1 <= 0 && remainPoint2 <= 0)
        {
            isClearAll = true;
        }
    }
    void RunMap_15()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();
        Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";
        Game04_Main.instance.gameUIComm.txt_TarageList[3].text = remainPoint1.ToString() + " 个";
        Game04_Main.instance.gameUIComm.txt_TarageList[5].text = remainPoint2.ToString() + " 个";
        if (Set.setVal.Language == 1)
        {
            Game04_Main.instance.gameUIComm.txt_TarageList[1].text = "  Num: " + remainPoint.ToString();
            Game04_Main.instance.gameUIComm.txt_TarageList[3].text = "  Num: " + remainPoint1.ToString();
            Game04_Main.instance.gameUIComm.txt_TarageList[5].text = "  Num: " + remainPoint2.ToString();

        }

        if (isPass_AllTarage())// || IdleTime < 0
        {
            InitMap_15_next();
        }
        if (remainPoint <= 0 && remainPoint1 <= 0 && remainPoint2 <= 0)
        {
            isClearAll = true;
        }
    }
    void RunMap_16()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();
        if (jieduan >= 10)
        {
            isClearAll = true;
            return;
        }
    }
    void RunMap_17()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        if (jieduan >= 10)
        {
            isClearAll = true;
            return;
        }
    }
    void RunMap_18()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();


        if (jieduan >= 10)
        {

            isClearAll = true;
            return;
        }
    }
    void RunMap_19()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();
        if (jieduan >= 10)
        {
            isClearAll = true;
            return;
        }
    }

    void RunMap_20()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        //     Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";

        if (IdleTime < 0)
        {
            InitMap_Over20_next();
        }

    }
    void RunMap_21()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        //     Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";

        if (IdleTime < 0)
        {
            InitMap_Over20_next();
        }

    }
    void RunMap_22()
    {
        IdleTime -= Time.deltaTime;
        Update_LedTarage();

        //     Game04_Main.instance.gameUIComm.txt_TarageList[1].text = remainPoint.ToString() + " 个";

        if (IdleTime < 0)
        {
            InitMap_Over20_next();
        }

    }
    void RunMap_23()
    {

        Update_LedTarage();
        Update_LED_DianZhen_Data();

    }
    void RunMap_24()
    {

        Update_LedTarage();
        Update_LED_DianZhen_Data();

    }
    void RunMap_25()
    {

        Update_LedTarage();
        Update_LED_DianZhen_Data();

    }
    void RunMap_26()
    {

        Update_LedTarage();
        Update_LED_DianZhen_Data();

    }

    void RunMap_27()
    {

        Update_LedTarage();
        Update_LED_DianZhen_Data();

    }

    void RunMap_28()
    {

        Update_LedTarage();
        Update_LED_DianZhen_Data();

    }
    void RunMap_29()
    {

        Update_LedTarage();
        Update_LED_DianZhen_Data();
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



    bool isPass_AllTarage()
    {

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return false;
        }
        for (int i = 0; i < MaxLedNum; i++)
        {

            if (GameLedControl.gamePoint[i].statue == enPointSta.Target)
            {

                return false;

            }
        }

        return true;
    }


    // Update is called once per frame
    void Update()
    {

        if (Game04_Main.instance.statue != en_Game00_Sta.Play)
        {
            return;
        }
        if (isCleaning || isClearAll)
        {
            return;
        }
        for (int i = 0; i < 5; i++)
        {
            //            Debug.LogError(i + "          " + Led_List[i]);
        }
        if (isMoving)
        {
            if (ShowTime > 0)
            {
                ShowTime -= Time.deltaTime;
                for (int i = 0; i < 5; i++)
                {
                    DrawPic.DrawPointId(i, tab_PointColor[tarlist_Over20[ShowIndex]], enPointSta.Rest);
                }
            }
            else
            {
                Show_Next();
            }

        }

        CheckLedKey();
        switch (Game04_Main.instance.gameLevel)
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
                RunMap_06();


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
            case 21:
            case 22:
            case 23:
            case 24:
            case 25:
            case 26:
            case 27:
            case 28:
            case 29:
                RunMap_20();


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
}
