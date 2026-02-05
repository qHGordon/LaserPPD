using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Map03 : MonoBehaviour
{
    public static Game_Map03 instance;
    int picId = 0;
    int pointId = 0;
    readonly uint[] tab_PointColor = { 0x00ff00, 0x0000ff, 0xD800FF, 0x00FF7F, 0xFF8C00 };
    public uint[] Tarage_Colors = new uint[3];//
    public uint[] Other_Color0 = new uint[3];//完全随机的目标  
    public uint[] Other_Color1 = new uint[3];//一种颜色相同的干扰目标  
    public uint[] Other_Color2 = new uint[3];//二种颜色相同的干扰目标
    public int MaxLedNum = 0;
    public bool isinfinite = false;//无限模式
    public float[] tobeachtime = new float[300];
    public int remainPoint = 0;
    int[] Tarage_List = new int[300];

    int[] otherColor_List0 = new int[300];
    int[] otherColor_List1 = new int[300];
    int[] otherColor_List2 = new int[300];
    private float[] touchTime;//
    float speedtime = 0;//
    public bool ismove = false;//运动

    public bool ismoveindex = false;

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


    public float RedPassTime;
    public float MaxRedPassTime = 3;


    float RedStayTime = 2f;
    public float MaxRedStayTime = 2f;
    float ChangeTarage_Time;
    float MaxChangeTarage_Time = 10;
    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }
    void GetOther_Color0()
    {
        for (int i = 0; i < 3; i++)
        {
            Other_Color0[i] = tab_PointColor[Random.Range(0, tab_PointColor.Length)];

            while (Other_Color0[i] == Tarage_Colors[i])
            {
                Other_Color0[i] = tab_PointColor[Random.Range(0, tab_PointColor.Length)];

            }
        }

        for (int i = 0; i < MaxLedNum; i++)
        {
            //if (i != 14)
            otherColor_List0[i] = i;
            for (int k = 0; k < MaxLedNum; k++)
            {
                if (otherColor_List1[k] == i || otherColor_List2[k] == i || Tarage_List[k] == i)
                {
                    otherColor_List0[i] = -1;
                    break;
                }

            }


        }
        //for (int i = 0; i < MaxLedNum; i++)
        //{
        //    Debug.LogError(otherColor_List0[i] + "   list0" + "   " + i);

        //}
        //for (int i = 0; i < tarageNum; i++)
        //{
        //    bool isSame = true;

        //    while (isSame)
        //    {
        //        otherColor_List0[i] = Random.Range(0, MaxLedNum);
        //        isSame = false;
        //        for (int k = 0; k < tarageNum; k++)
        //        {
        //            if (Tarage_List[k] == otherColor_List0[i])
        //            {
        //                isSame = true;
        //            }
        //        }
        //    }

        //}
    }
    void GetOther_Color1()
    {
        int same1 = 0;
        same1 = Random.Range(0, 3);


        uint col = tab_PointColor[Random.Range(0, tab_PointColor.Length)];

        for (int i = 0; i < 3; i++)
        {
            Other_Color1[same1] = Tarage_Colors[same1];

            if (i == same1)
            {
                continue;
            }
            while (col == Tarage_Colors[0])
            {
                col = tab_PointColor[Random.Range(0, tab_PointColor.Length)];

            }
            //if (col == 14) break;
            Other_Color1[i] = col;
        }

        for (int i = 0; i < tarageNum; i++)
        {

            bool isSame = true;

            while (isSame)
            {
                int n = Random.Range(0, MaxLedNum);
                //while(n==14)
                //n = Random.Range(0, MaxLedNum);
                otherColor_List1[i] = n;
                //if (i == 15) break;
                isSame = false;
                for (int k = 0; k < tarageNum; k++)
                {
                    if (Tarage_List[k] == otherColor_List1[i])
                    {
                        isSame = true;
                    }
                }
            }

        }
        for (int i = 0; i < tarageNum; i++)
        {
            //Debug.LogError(otherColor_List1[i] + "   list1" + "   " + i);

        }
    }
    void GetOther_Color2()
    {



        int same1, same2;
        same1 = Random.Range(0, 3);
        same2 = same1;
        while (same1 == same2)
        {
            same2 = Random.Range(0, 3);
        }

        uint col = tab_PointColor[Random.Range(0, tab_PointColor.Length)];

        for (int i = 0; i < 3; i++)
        {
            Other_Color2[same1] = Tarage_Colors[same1];
            Other_Color2[same2] = Tarage_Colors[same2];
            if (i == same1 || same2 == i)
            {
                continue;
            }
            while (col == Tarage_Colors[i])
            {
                col = tab_PointColor[Random.Range(0, tab_PointColor.Length)];

            }
            Other_Color2[i] = col;



        }


        for (int i = 0; i < tarageNum; i++)
        {

            bool isSame = true;

            while (isSame)
            {
                int a = Random.Range(0, MaxLedNum);
                //while (a == 14) a = Random.Range(0, MaxLedNum);
                otherColor_List2[i] = a;
                isSame = false;
                for (int k = 0; k < tarageNum; k++)
                {
                    if (Tarage_List[k] == otherColor_List2[i] || otherColor_List1[k] == otherColor_List2[i])
                    {
                        isSame = true;
                    }
                }
            }

        }
    }
    void Update_NextTarage_Pos()
    {
        //Framebuffer.Update_ColorFull(0, enPointSta.Die);
        if (MoveIndex >= MoveAllStep)
        {

            isClearAll = true;
            return;
        }
        //Debug.LogError("MoveIndex   " + MoveIndex);
        //Debug.LogError("MoveAllStep   " + MoveAllStep);
        for (int i = 0; i < MoveAllStep; i++)
        {
            if (MovePos[i] < 0)
            {
                continue;
            }
            if (have_touch[MovePos[i]])
            {
                for (int k = 0; k < 3; k++)
                {
                    DrawPic.DrawPointId(MovePos[i] * 3 + k, Tarage_Colors[k], enPointSta.Rest);

                }
            }


        }


        for (int i = 0; i < MaxLedNum; i++)
        {
            for (int k = 0; k < MoveAllStep; k++)
            {
                if (i != MovePos[k])
                {
                    for (int n = 0; n < 3; n++)
                    {
                        DrawPic.DrawPointId(i * 3 + n, 0, enPointSta.Die);
                    }


                }
            }
        }
        if (ismoveindex)
        {

            for (int i = 0; i < MoveAllStep; i++)
            {

                if (MoveIndex == i)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        DrawPic.DrawPointId(MovePos[i] * 3 + k, 0, enPointSta.Target);

                    }

                }
            }

        }
        else
        {
            for (int i = 0; i < MoveAllStep; i++)
            {

                if (MovePos[i] >= 0 && have_touch[MovePos[i]] == false)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        DrawPic.DrawPointId(MovePos[i] * 3 + k, 0, enPointSta.Target);

                    }

                }
            }
        }






    }
    void Update_Move_Tarage_Pos()//第三个模式的
    {

        for (int i = 0; i < 3; i++)
        {
            //    Debug.LogError(MovePos[MoveIndex] + "    " + MoveIndex);
            DrawPic.DrawPointId(MovePos[MoveIndex] * 3 + i, Tarage_Colors[i], enPointSta.Target);
        }
        if (MoveTime > 0)
        {
            MoveTime -= Time.deltaTime;
        }

        if (MoveTime <= 0)
        {
            MoveTime = MaxMoveTime2;


            MoveIndex++;
            if (MoveIndex >= MoveAllStep)
            {
                MoveIndex = 0;
                isMoving = false;
            }

        }

    }
    void Update_MovePos()
    {
        if (MoveTime > 0)
        {
            MoveTime -= Time.deltaTime;
        }
        for (int i = 0; i <= MoveCnt; i++)
        {
            DrawPic.DrawPointId(MoveIndex * 3 + i, Tarage_Colors[i], enPointSta.Target);

        }
        if (MoveTime <= 0)
        {
            MoveTime = MaxMoveTime;

            MoveCnt++;
            if (MoveCnt >= 3)
            {
                MoveCnt = 0;
                MoveIndex++;
                if (MoveIndex >= MaxLedNum)
                {
                    MoveIndex = 0;
                }
            }
        }

    }
    void ChangeTarage_Map()
    {
        GetTarageColor();
        for (int i = 0; i < otherColor_List0.Length; i++)
        {
            otherColor_List0[i] = -1;
        }
        for (int i = 0; i < otherColor_List1.Length; i++)
        {
            otherColor_List1[i] = -2;
        }
        for (int i = 0; i < otherColor_List2.Length; i++)
        {
            otherColor_List2[i] = -3;
        }
        for (int i = 0; i < Tarage_List.Length; i++)
        {
            Tarage_List[i] = -4;
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
        MoveIndex = 0;
        for (int i = 0; i < beTouch.Length; i++)
        {
            beTouch[i] = false;
        }


        Invoke("InitMap_" + Game03_Main.instance.gameLevel.ToString("D2") + "_next", 0);
    }
    public bool[] have_touch = new bool[300];
    public void InitMap(int Id)
    {
        MaxJieDuan = 1;
        RedPassTime = 5;
        MaxRedPassTime = 3;
        picId = 0;
        pointId = 0;
        RedLED_Index = 0;
        isDouble = true;
        ismove = false;
        isinfinite = false;
        MoveTime = MaxMoveTime;
        ChangeTarage_Time = MaxChangeTarage_Time;
        isMoving = false;
        remainPoint = 10 + 5 * Game03_Main.instance.Index_JieDuan;
        Framebuffer.Update_ColorFull(0, enPointSta.None);
        for (int i = 0; i < tobeachtime.Length; i++)
        {
            tobeachtime[i] = 0;
        }
        GetTarageColor();
        for (int i = 0; i < have_touch.Length; i++)
        {
            have_touch[i] = false;
        }
        for (int i = 0; i < otherColor_List0.Length; i++)
        {
            otherColor_List0[i] = -1;
        }
        for (int i = 0; i < otherColor_List1.Length; i++)
        {
            otherColor_List1[i] = -2;
        }
        for (int i = 0; i < otherColor_List2.Length; i++)
        {
            otherColor_List2[i] = -3;
        }
        for (int i = 0; i < Tarage_List.Length; i++)
        {
            Tarage_List[i] = -4;
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
        MoveIndex = 0;
        for (int i = 0; i < beTouch.Length; i++)
        {
            beTouch[i] = false;
        }

        Invoke("InitMap_" + Id.ToString("D2"), 0);


    }
    void Start()
    {
        int m = 0;
        for (int i = 0; i < Set.ChannelLength.Length; i++)
        {
            m += Set.ChannelLength[i];
        }
        MaxLedNum = m / 3 - 1;
        Debug.LogError(MaxLedNum + "灯数");

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
                    DrawPic.DrawPointId(i * 3 + k, 0xff0000, enPointSta.Die);

                }

            }

        }
        else
        {
            for (int i = RedLED_Index; i < RedLED_Index + 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    DrawPic.DrawPointId(i * 3 + k, 0xff0000, enPointSta.Die);

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
    void InitMap_00()
    {
        //isinfinite = false;
        tarageNum = 5;
        Game03_Main.instance.remainTime = 120;
        GetLED_TarageList();

        GetOther_Color0();
    }
    void InitMap_01()
    {
        //isinfinite = false;
        Game03_Main.instance.remainTime = 120;
        tarageNum = 5;
        GetLED_TarageList();

        GetOther_Color0();
    }
    void InitMap_02()
    {
        //isinfinite = false;
        tarageNum = 6;
        Game03_Main.instance.remainTime = 120;
        GetLED_TarageList();

        GetOther_Color0();
    }


    void InitMap_03()
    {
        //isinfinite = false;
        tarageNum = 5;
        Game03_Main.instance.remainTime = 60;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color0();
        //  isMoving = true;
    }
    void InitMap_04()
    {
        tarageNum = 4;
        //isinfinite = false;
        remainPoint = 4;
        Game03_Main.instance.remainTime = 30;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color0();
    }
    void InitMap_05()
    {
        tarageNum = 6;
        isinfinite = true;
        remainPoint = 10;
        //speedtime = 3;
        //ismove = true;
        Game03_Main.instance.isClearTarage = false;
        Game03_Main.instance.remainTime = 30;
        GetLED_TarageList();
        //GetTouchtime(tarageNum);
        GetOther_Color1();
        GetOther_Color0();
    }


    //第三模式,跑灯
    void InitMap_06()
    {
        tarageNum = 5;
        isinfinite = true;
        remainPoint = 10;
        Game03_Main.instance.isClearTarage = false;
        Game03_Main.instance.remainTime = 60;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_07()
    {
        tarageNum = 5;
        isinfinite = true;
        remainPoint = 10;
        Game03_Main.instance.isClearTarage = false;
        Game03_Main.instance.remainTime = 60;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_08()
    {
        tarageNum = 6;
        isinfinite = true;
        remainPoint = 10;
        Game03_Main.instance.isClearTarage = false;
        Game03_Main.instance.remainTime = 30;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_09()
    {
        tarageNum = 6;
        isinfinite = true;
        remainPoint = 10;
        Game03_Main.instance.isClearTarage = false;
        Game03_Main.instance.remainTime = 30;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_10()
    {
        tarageNum = 5;
        remainPoint = 20;
        Game03_Main.instance.remainTime = 120;
        Game03_Main.instance.isClearTarage = false;
        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_10_next()
    {
        tarageNum = 5;
        Game03_Main.instance.isClearTarage = false;
        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_11()
    {
        tarageNum = 5;
        remainPoint = 20;
        Game03_Main.instance.remainTime = 60;
        Game03_Main.instance.isClearTarage = false;
        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_11_next()
    {
        tarageNum = 5;


        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_12()
    {
        tarageNum = 6;
        remainPoint = 20;
        Game03_Main.instance.remainTime = 60;
        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 3;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_12_next()
    {
        tarageNum = 6;


        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 3;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_13()
    {
        tarageNum = 6;
        remainPoint = 20;
        Game03_Main.instance.remainTime = 60;
        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_13_next()
    {
        tarageNum = 6;

        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_14()
    {
        tarageNum = 6;
        remainPoint = 25;
        Game03_Main.instance.remainTime = 60;
        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_14_next()
    {
        tarageNum = 6;

        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }

    void InitMap_15()
    {
        tarageNum = 5;
        remainPoint = 30;
        Game03_Main.instance.remainTime = 60;
        Game03_Main.instance.isClearTarage = false;
        speedtime = 5;
        ismove = true;
        isinfinite = true;
        GetTouchtime(tarageNum);
        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_15_next()
    {
        tarageNum = 5;
        speedtime = 4;
        ismove = true;
        isinfinite = true;
        GetTouchtime(tarageNum);
        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_16()
    {
        tarageNum = 6;
        remainPoint = 15;
        speedtime = 4;
        ismove = true;
        isinfinite = true;
        GetTouchtime(tarageNum);
        ////isinfinite = true;
        Game03_Main.instance.remainTime = 40;
        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_16_next()
    {
        tarageNum = 6;

        Game03_Main.instance.isClearTarage = false;
        isinfinite = true;
        speedtime = 4;
        ismove = true;
        GetTouchtime(tarageNum);
        //isinfinite = true;
        MaxRedPassTime = 2;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_17()
    {
        tarageNum = 6;
        remainPoint = 20;
        Game03_Main.instance.remainTime = 40;
        Game03_Main.instance.isClearTarage = false;
        speedtime = 3;
        ismove = true;
        isinfinite = true;
        GetTouchtime(tarageNum);
        //isinfinite = true;
        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_17_next()
    {
        tarageNum = 6;

        Game03_Main.instance.isClearTarage = false;
        speedtime = 3;
        ismove = true;
        isinfinite = true;
        GetTouchtime(tarageNum);
        //isinfinite = true;
        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_18()
    {
        tarageNum = 6;
        speedtime = 3;
        ismove = true;
        GetTouchtime(tarageNum);
        isinfinite = true;
        remainPoint = 20;
        Game03_Main.instance.remainTime = 30;
        Game03_Main.instance.isClearTarage = false;
        isinfinite = true;
        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_18_next()
    {
        isinfinite = true;
        tarageNum = 6;
        speedtime = 3;
        ismove = true;
        GetTouchtime(tarageNum);
        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_19()
    {
        isinfinite = true;
        tarageNum = 6;
        remainPoint = 30;
        speedtime = 2;
        ismove = true;
        GetTouchtime(tarageNum);
        Game03_Main.instance.remainTime = 30;
        Game03_Main.instance.isClearTarage = false;

        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_19_next()
    {
        tarageNum = 6;
        isinfinite = true;
        Game03_Main.instance.isClearTarage = false;
        speedtime = 2;
        ismove = true;
        GetTouchtime(tarageNum);
        MaxRedPassTime = 1;
        GetLED_TarageList();
        GetOther_Color1();
        GetOther_Color2();
        GetOther_Color0();
    }
    void InitMap_20()
    {
        MoveAllStep = 5;
        GetMovePos();
        ismoveindex = false;
        isMoving = true;
        Game03_Main.instance.remainTime = 120;
    }
    void InitMap_21()
    {
        MoveAllStep = 7;
        GetMovePos();
        ismoveindex = false;
        isMoving = true;
        Game03_Main.instance.remainTime = 60;
    }
    void InitMap_22()
    {
        MoveAllStep = 9;
        GetMovePos();
        ismoveindex = false;
        isMoving = true;
        Game03_Main.instance.remainTime = 60;
    }
    void InitMap_23()
    {
        MoveAllStep = 8;
        GetMovePos();
        ismoveindex = false;
        isMoving = true;
        Game03_Main.instance.remainTime = 30;
    }
    void InitMap_24()
    {
        MoveAllStep = 10;
        GetMovePos();
        ismoveindex = false;
        isMoving = true;
        Game03_Main.instance.remainTime = 10;
    }
    //
    void InitMap_25()
    {
        ismoveindex = true;
        MoveAllStep = 5;
        MaxMoveTime2 = 0.8f;
        GetMovePos();
        isMoving = true;
        Game03_Main.instance.remainTime = 30;
    }
    void InitMap_26()
    {
        ismoveindex = true;
        MoveAllStep = 7;
        MaxMoveTime2 = 0.6f;
        GetMovePos();
        isMoving = true;
        Game03_Main.instance.remainTime = 30;
    }
    void InitMap_27()
    {
        ismoveindex = true;
        MoveAllStep = 8;
        MaxMoveTime2 = 0.5f;
        GetMovePos();
        isMoving = true;
        Game03_Main.instance.remainTime = 30;
    }
    void InitMap_28()
    {
        ismoveindex = true;
        MoveAllStep = 10;
        MaxMoveTime2 = 0.5f;
        GetMovePos();
        isMoving = true;
        Game03_Main.instance.remainTime = 30;
    }
    void InitMap_29()
    {
        MoveAllStep = 10;
        ismoveindex = true;
        MaxMoveTime2 = 0.5f;
        GetMovePos();
        isMoving = true;
        Game03_Main.instance.remainTime = 10;
    }
    void InitMap_11(int a)
    {
        GetMovePos();

        GetLED_TarageList();
        isMoving = true;
    }

    void RunMap_00()
    {

        Update_TarageColor();
        Update_OhterColor0();

    }
    void RunMap_01()
    {
        Update_TarageColor();
        Update_OhterColor0();


    }
    void RunMap_02()
    {
        Update_TarageColor();
        Update_OhterColor0();
        //    Run_RedPass();

    }
    //
    void RunMap_03()
    {
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        //Run_RedPass();
    }
    void RunMap_04()
    {
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        //  Run_RedPass();
    }
    void RunMap_05()
    {
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        //   Run_RedPass();

    }

    void RunMap_06()
    {
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }

    void RunMap_07()
    {
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }

    void RunMap_08()
    {
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_09()
    {
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_10()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        //Run_RedPass();
    }
    void RunMap_11()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        //Run_RedPass();
    }
    void RunMap_12()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_13()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }

    void RunMap_14()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_15()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_16()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_17()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_18()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_19()
    {
        ChangeTarage_Time -= Time.deltaTime;
        if (ChangeTarage_Time < 0)
        {
            ChangeTarage_Time = MaxChangeTarage_Time;
            ChangeTarage_Map();
        }
        Update_TarageColor();
        Update_OhterColor0();
        Update_OhterColor1();
        Update_OhterColor2();
        Run_RedPass();
    }
    void RunMap_20()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_21()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_22()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_23()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_24()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_25()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_26()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_27()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_28()
    {
        Update_NextTarage_Pos();
    }
    void RunMap_29()
    {
        Update_NextTarage_Pos();
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
    void GetLED_TarageList()//获得一定数量的,和目标灯颜色的灯
    {
        int a = 0;
        for (int i = 0; i < tarageNum; i++)
        {
            a = Random.Range(0, MaxLedNum);
            //while (a == 14) a = Random.Range(0, MaxLedNum);
            if (i == 0)
            {

                Tarage_List[i] = a;
            }
            else
            {
                bool isSame = false;

                while (isSame)
                {
                    isSame = false;

                    //while (a == 14) a = Random.Range(0, MaxLedNum);
                    for (int k = 0; k < i; k++)
                    {
                        if (Tarage_List[k] == a)
                        {
                            isSame = true;
                        }
                    }
                }
                Tarage_List[i] = a;
            }
            //    Debug.LogError(Tarage_List[i] + "    " + i);
        }


    }
    void Update_OhterColor0()
    {
        for (int i = 0; i < otherColor_List0.Length; i++)
        {
            if (otherColor_List0[i] < 0)
            {
                continue;
            }
            int trueLed_ID = otherColor_List0[i] * 3;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color0[0], enPointSta.Die);

            trueLed_ID = otherColor_List0[i] * 3 + 1;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color0[1], enPointSta.Die);

            trueLed_ID = otherColor_List0[i] * 3 + 2;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color0[2], enPointSta.Die);


            //trueLed_ID = otherColor_List2[i] * 3;
            //Framebuffer.Update_PointColor(trueLed_ID, Other_Color2[0], enPointSta.None);

            //trueLed_ID = otherColor_List2[i] * 3 + 1;
            //Framebuffer.Update_PointColor(trueLed_ID, Other_Color2[1], enPointSta.None);

            //trueLed_ID = otherColor_List2[i] * 3 + 2;
            //Framebuffer.Update_PointColor(trueLed_ID, Other_Color2[2], enPointSta.None);
        }

    }
    void Update_OhterColor1()
    {
        for (int i = 0; i < otherColor_List1.Length; i++)
        {
            if (otherColor_List1[i] < 0)
            {
                continue;
            }
            int trueLed_ID = otherColor_List1[i] * 3;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color1[0], enPointSta.Die);

            trueLed_ID = otherColor_List1[i] * 3 + 1;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color1[1], enPointSta.Die);

            trueLed_ID = otherColor_List1[i] * 3 + 2;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color1[2], enPointSta.Die);

        }
    }
    void Update_OhterColor2()
    {
        for (int i = 0; i < otherColor_List2.Length; i++)
        {
            if (otherColor_List2[i] < 0)
            {
                continue;
            }
            int trueLed_ID = otherColor_List2[i] * 3;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color2[0], enPointSta.Die);

            trueLed_ID = otherColor_List2[i] * 3 + 1;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color2[1], enPointSta.Die);

            trueLed_ID = otherColor_List2[i] * 3 + 2;
            Framebuffer.Update_PointColor(trueLed_ID, Other_Color2[2], enPointSta.Die);

        }
    }
    int tarageNum_now = 1;
    void Get_NowTarage()
    {


        //   tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GettarageNum();
        tarageNum_now = 0;
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {

                    tarageNum_now++;


                }

            }
        }
#if UNITY_EDITOR
        //        Debug.LogError(" 目标点 " + tarageNum_now);
#endif


    }
    bool isPass()
    {
        if (!Game03_Main.instance.isClearTarage)
        {
            if (remainPoint <= 0)
            {
                isClearAll = true;
                return true;
            }

        }

        for (int i = 0; i < tarageNum; i++)
        {
            if (Tarage_List[i] < 0)
            {
                return false;

            }
            if (!beTouch[Tarage_List[i]])
            {
                return false;
            }

        }
        isClearAll = true;
        return true;
    }
    bool isEnough_Tarage()
    {

        if (remainPoint <= 0)
        {
            isClearAll = true;
            return true;
        }
        return false;
    }
    void Update_TarageColor()
    {
        for (int i = 0; i < 3; i++)
        {
            Framebuffer.Update_PointColor(MaxLedNum * 3 + i, Tarage_Colors[i], enPointSta.Rest);
            DrawPic.DrawPointId(MaxLedNum * 3 + i, Tarage_Colors[1], enPointSta.Rest);
        }

        for (int i = 0; i < tarageNum; i++)
        {
            if (Tarage_List[i] < 0)
            {
                continue;
            }
            if (!beTouch[Tarage_List[i]] && !ismove)
            {
                int trueLed_ID = Tarage_List[i] * 3;

                DrawPic.DrawPointId(trueLed_ID, Tarage_Colors[0], enPointSta.Target);

                trueLed_ID = Tarage_List[i] * 3 + 1;
                DrawPic.DrawPointId(trueLed_ID, Tarage_Colors[1], enPointSta.Target);

                trueLed_ID = Tarage_List[i] * 3 + 2;
                DrawPic.DrawPointId(trueLed_ID, Tarage_Colors[2], enPointSta.Target);



            }
            else if (!beTouch[Tarage_List[i]] && ismove)
            {
                int trueLed_ID = Tarage_List[i] * 3;

                DrawPic.DrawPointId(trueLed_ID, Tarage_Colors[0], enPointSta.Target);

                trueLed_ID = Tarage_List[i] * 3 + 1;
                DrawPic.DrawPointId(trueLed_ID, Tarage_Colors[1], enPointSta.Target);

                trueLed_ID = Tarage_List[i] * 3 + 2;
                DrawPic.DrawPointId(trueLed_ID, Tarage_Colors[2], enPointSta.Target);

                float changetime = Time.time - touchTime[i];
                if (changetime >= speedtime)
                {

                    int n = Tarage_List[i] + 1;
                    for (int j = 0; j < otherColor_List0.Length; j++)
                    {
                        if (otherColor_List0[j] == n)
                        {
                            touchTime[i] = Time.time;
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3, Other_Color0[0], enPointSta.Die);
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 1, Other_Color0[1], enPointSta.Die);
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 2, Other_Color0[2], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3, Other_Color0[0], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3 + 1, Other_Color0[0], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3 + 2, Other_Color0[0], enPointSta.Die);
                            Framebuffer.Update_PointColor(otherColor_List0[j] * 3, Tarage_Colors[0], enPointSta.Target);
                            Framebuffer.Update_PointColor(otherColor_List0[j] * 3 + 1, Tarage_Colors[1], enPointSta.Target);
                            Framebuffer.Update_PointColor(otherColor_List0[j] * 3 + 2, Tarage_Colors[2], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List0[j] * 3, Tarage_Colors[0], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List0[j] * 3 + 1, Tarage_Colors[0], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List0[j] * 3 + 2, Tarage_Colors[0], enPointSta.Target);
                            otherColor_List0[j] = Tarage_List[i];
                            Tarage_List[i] = n;

                            break;
                        }
                        if (otherColor_List1[j] == n)
                        {
                            touchTime[i] = Time.time;
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3, Other_Color1[0], enPointSta.Die);
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 1, Other_Color1[1], enPointSta.Die);
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 2, Other_Color1[2], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3, Other_Color1[0], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3 + 1, Other_Color1[0], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3 + 2, Other_Color1[0], enPointSta.Die);
                            Framebuffer.Update_PointColor(otherColor_List1[j] * 3, Tarage_Colors[0], enPointSta.Target);
                            Framebuffer.Update_PointColor(otherColor_List1[j] * 3 + 1, Tarage_Colors[1], enPointSta.Target);
                            Framebuffer.Update_PointColor(otherColor_List1[j] * 3 + 2, Tarage_Colors[2], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List1[j] * 3, Tarage_Colors[0], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List1[j] * 3 + 1, Tarage_Colors[0], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List1[j] * 3 + 2, Tarage_Colors[0], enPointSta.Target);
                            otherColor_List1[j] = Tarage_List[i];
                            Tarage_List[i] = n;

                            break;
                        }
                        if (otherColor_List2[j] == n)
                        {
                            touchTime[i] = Time.time;
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3, Other_Color2[0], enPointSta.Die);
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 1, Other_Color2[1], enPointSta.Die);
                            Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 2, Other_Color2[2], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3, Other_Color2[0], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3 + 1, Other_Color2[0], enPointSta.Die);
                            DrawPic.DrawPointId(Tarage_List[i] * 3 + 2, Other_Color2[0], enPointSta.Die);
                            Framebuffer.Update_PointColor(otherColor_List2[j] * 3, Tarage_Colors[0], enPointSta.Target);
                            Framebuffer.Update_PointColor(otherColor_List2[j] * 3 + 1, Tarage_Colors[1], enPointSta.Target);
                            Framebuffer.Update_PointColor(otherColor_List2[j] * 3 + 2, Tarage_Colors[2], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List2[j] * 3, Tarage_Colors[0], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List2[j] * 3 + 1, Tarage_Colors[0], enPointSta.Target);
                            DrawPic.DrawPointId(otherColor_List1[j] * 3 + 2, Tarage_Colors[0], enPointSta.Target);
                            otherColor_List2[j] = Tarage_List[i];
                            Tarage_List[i] = n;

                            break;
                        }
                    }
                }



            }
            else if (beTouch[Tarage_List[i]] && isinfinite)
            {
                if (ismove)
                    touchTime[i] = Time.time;
                beTouch[Tarage_List[i]] = false;
                Framebuffer.Update_PointColor(Tarage_List[i] * 3, Other_Color0[0], enPointSta.Die);
                Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 1, Other_Color0[1], enPointSta.Die);
                Framebuffer.Update_PointColor(Tarage_List[i] * 3 + 2, Other_Color0[2], enPointSta.Die);
                DrawPic.DrawPointId(Tarage_List[i] * 3, Other_Color0[0], enPointSta.Die);
                DrawPic.DrawPointId(Tarage_List[i] * 3 + 1, Other_Color0[0], enPointSta.Die);
                DrawPic.DrawPointId(Tarage_List[i] * 3 + 2, Other_Color0[0], enPointSta.Die);


                int n = Random.Range(0, otherColor_List0.Length);
                int cnt = 0;

                while (otherColor_List0[n] <= 0 && cnt < 200)
                {
                    cnt++;
                    n = Random.Range(0, otherColor_List0.Length);

                }
                Framebuffer.Update_PointColor(otherColor_List0[n] * 3, Tarage_Colors[0], enPointSta.Target);
                Framebuffer.Update_PointColor(otherColor_List0[n] * 3 + 1, Tarage_Colors[1], enPointSta.Target);
                Framebuffer.Update_PointColor(otherColor_List0[n] * 3 + 2, Tarage_Colors[2], enPointSta.Target);
                DrawPic.DrawPointId(otherColor_List0[n] * 3, Tarage_Colors[0], enPointSta.Target);
                DrawPic.DrawPointId(otherColor_List0[n] * 3 + 1, Tarage_Colors[0], enPointSta.Target);
                DrawPic.DrawPointId(otherColor_List0[n] * 3 + 2, Tarage_Colors[0], enPointSta.Target);
                int m = otherColor_List0[n];
                otherColor_List0[n] = Tarage_List[i];
                Tarage_List[i] = m;
            }


        }

    }
    void Update_TarageColor_None()
    {
        for (int i = 0; i < 3; i++)
        {
            Framebuffer.Update_PointColor(MaxLedNum * 3 + i, Tarage_Colors[i], enPointSta.Rest);
        }

        for (int i = 0; i < tarageNum; i++)
        {
            if (Tarage_List[i] < 0)
            {
                continue;
            }
            if (!beTouch[Tarage_List[i]])
            {
                int trueLed_ID = Tarage_List[i] * 3;
                //  Framebuffer.Update_PointColor(trueLed_ID, Tarage_Colors[0], enPointSta.Target);
                DrawPic.DrawPointId(trueLed_ID, 0, enPointSta.Target);

                trueLed_ID = Tarage_List[i] * 3 + 1;
                DrawPic.DrawPointId(trueLed_ID, 0, enPointSta.Target);

                trueLed_ID = Tarage_List[i] * 3 + 2;
                DrawPic.DrawPointId(trueLed_ID, 0, enPointSta.Target);
            }


        }


    }
    // Update is called once per frame
    void Update()
    {

        if (Game03_Main.instance.statue != en_Game00_Sta.Play)
        {
            return;
        }
        if (isCleaning)
        {
            return;
        }
        if (isMoving)
        {
            Update_Move_Tarage_Pos();

            return;
        }
        for (int i = 0; i < tobeachtime.Length; i++)
        {
            if (tobeachtime[i] > 0)
            {
                tobeachtime[i] -= Time.deltaTime;

            }
        }
        switch (Game03_Main.instance.gameLevel)
        {
            case 0:
                RunMap_00();
                isPass();
                break;
            case 1:
                RunMap_01();
                isPass();

                break;
            case 2:
                RunMap_02();
                isPass();
                break;

            //
            case 3:
                RunMap_03();
                isPass();

                //    isEnough_Tarage();
                break;
            case 4:
                RunMap_04();
                isPass();

                //  isEnough_Tarage();
                break;
            case 5:
                RunMap_05();
                isPass();

                //  isEnough_Tarage();
                break;
            //
            case 6:
                RunMap_06();
                isPass();

                break;
            case 7:
                RunMap_07();
                isPass();

                break;
            case 8:
                RunMap_08();
                isPass();

                break;
            case 9:
                RunMap_09();
                isPass();

                break;
            case 10:
                RunMap_10();
                isEnough_Tarage();

                break;
            case 11:
                RunMap_11();
                isEnough_Tarage();

                break;
            case 12:
                RunMap_12();
                isEnough_Tarage();

                break;
            case 13:
                RunMap_13();
                isEnough_Tarage();

                break;
            case 14:
                RunMap_14();
                isEnough_Tarage();

                break;
            case 15:
                RunMap_15();
                isEnough_Tarage();

                break;
            case 16:
                RunMap_16();
                isEnough_Tarage();

                break;
            case 17:
                RunMap_17();
                isEnough_Tarage();

                break;
            case 18:
                RunMap_18();
                isEnough_Tarage();

                break;
            case 19:
                RunMap_19();
                isEnough_Tarage();

                break;
            case 20:
                RunMap_20();
                isPass();

                break;
            case 21:
                RunMap_21();
                isPass();

                break;
            case 22:
                RunMap_22();
                isPass();

                break;
            case 23:
                RunMap_23();
                isPass();

                break;
            case 24:
                RunMap_24();
                isPass();

                break;
            case 25:
                RunMap_25();
                isPass();

                break;
            case 26:
                RunMap_26();
                isPass();

                break;
            case 27:
                RunMap_27();
                isPass();

                break;
            case 28:
                RunMap_28();
                isPass();

                break;
            case 29:
                RunMap_29();
                isPass();

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



    void GetTouchtime(int n)
    {
        touchTime = new float[n];
        for (int i = 0; i < touchTime.Length; i++)
        {
            touchTime[i] = Time.time;

        }
        Debug.Log("初始化时间");
    }
}
