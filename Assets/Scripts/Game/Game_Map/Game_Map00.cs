using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Map00 : MonoBehaviour
{


    public enum en_YouYu_sta
    {
        Idle,
        Playing,
        Checking,
        Check,
    }

    public en_YouYu_sta sta_YouYu;
    public AudioSource sound_Youyu;
    public AudioClip Spr_YouYu;
    public AudioClip[] Spr_Tatage_Color;
    int picId = 0;
    int pointId = 0;
    [HideInInspector]
    public int snakeLength;
    [HideInInspector]
    public int tarageNum = 20;


    public int TarageNum_now = 0;
    //第14关,跑操场的方块宽度
    public int Game_Fangkuai_length = 3;

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

    public int MaxJieDuan = 2;
    enum en_riverWay
    {
        None,
        Up = 1,
        Down = -1,
        Left = 2,
        Right = -2
    }
    en_riverWay riverWay = en_riverWay.Up;
    en_riverWay riverWay_Old = en_riverWay.None;
    public static Game_Map00 instance;
    Vector2 riverPos_Now = Vector2.zero;

    public LedAnim_Struts[] ledSturts_Group = new LedAnim_Struts[4];
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
    void Clear()
    {
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            GameLedControl.gamePoint[i].statue = enPointSta.None;
        }

    }
    Vector2[] map0_Red = new Vector2[300];
    int map0_RedCnt = 0;
    void InitMap_00()
    {
        MaxJieDuan = 1;
        map0_RedCnt = 0;
        for (int i = 0; i < map0_Red.Length; i++)
        {
            map0_Red[i] = Vector2.one * -1;
        }
        Clear();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                a = Random.Range(0, 100);
                if (a <= 20)
                {
                    picId = Set.setVal.Width * k + i;
                    pointId = Framebuffer.tab_Mapping[picId];
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                    map0_Red[map0_RedCnt] = new Vector2(i, k);
                    map0_RedCnt++;
                    // Framebuffer.Update_PointColor(pointId, 0xff, enPointSta.Die);
                }
                else if (a <= 60)
                {
                    picId = Set.setVal.Width * k + i;
                    pointId = Framebuffer.tab_Mapping[picId];
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Target;

                }
                else
                {
                    picId = Set.setVal.Width * k + i;
                    pointId = Framebuffer.tab_Mapping[picId];
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
                    //  Framebuffer.Update_PointColor(pointId, 0xff, enPointSta.Die);

                }
            }
        }
    }
    void InitMap_01(int jieduan)
    {
        MaxJieDuan = 1;
        Clear();
        Game00_Main.instance.isClearTarage = false;
        remainPoint = 10 + jieduan;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {

            if (SettingInGame_01.instance.GetReMainNum(jieduan) > 0)
            {
                remainPoint = SettingInGame_01.instance.GetReMainNum(jieduan);

            }
        }




        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();


        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                a = Set.setVal.Width * k + i;
                GameLedControl.gamePoint[a].statue = enPointSta.Die;
            }
        }

        x = y = 1;

        Cnt = 80;
        if (Set.setVal.Width > 25 || Set.setVal.Height > 25)
        {

            Cnt = 250;
        }
        Get_NewWay();
        while (Cnt > 0)
        {
            //  break;
            switch (riverWay)
            {
                case en_riverWay.Up:
                    if (y + step <= Set.setVal.Height - 2)
                    {
                        Cnt--;
                        while (step > 0)
                        {
                            step--;
                            picId = y * Set.setVal.Width + x;
                            pointId = Framebuffer.tab_Mapping[picId];
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            y++;
                            if (y >= Set.setVal.Height - 2)
                            {
                                step = 0;
                            }
                        }

                    }
                    else
                    {
                        Get_NewWay();
                    }


                    break;
                case en_riverWay.Down:
                    if (y - step >= 2)
                    {
                        Cnt--;
                        while (step > 0)
                        {
                            step--;
                            picId = y * Set.setVal.Width + x;
                            pointId = Framebuffer.tab_Mapping[picId];
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            y--;
                            if (y <= 2)
                            {
                                step = 0;
                            }
                        }

                    }
                    else
                    {
                        Get_NewWay();
                    }
                    break;
                case en_riverWay.Right:
                    if (x + step <= Set.setVal.Width - 2)
                    {
                        Cnt--;
                        while (step > 0)
                        {
                            step--;
                            picId = y * Set.setVal.Width + x;
                            pointId = Framebuffer.tab_Mapping[picId];
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            x++;
                            if (x >= Set.setVal.Width - 2)
                            {
                                step = 0;
                            }
                        }

                    }
                    else
                    {
                        Get_NewWay();
                    }


                    break;
                case en_riverWay.Left:
                    if (x - step >= 2)
                    {
                        Cnt--;
                        while (step > 0)
                        {
                            step--;
                            picId = y * Set.setVal.Width + x;
                            pointId = Framebuffer.tab_Mapping[picId];
                            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                            x--;
                            if (x <= 2)
                            {
                                step = 0;
                            }
                        }

                    }
                    else
                    {
                        Get_NewWay();
                    }

                    break;

            }

            Get_NewWay();
        }
        GetRandom_Tarage();
    }
    void InitMap_02()
    {
        MaxJieDuan = 1;
        Clear();
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;
        ledSturts_Group[1].enabled = true;

        ledSturts_Group[0].Init(false, this);
        ledSturts_Group[1].Init(false, this);
        ledSturts_Group[0].UpdatePic_FangKuai(new Vector2(5, 5), 4, 4);
        GetRandom_Tarage();

    }
    void InitMap_03()
    {
        MaxJieDuan = 4;

        Clear();
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }

        for (int i = 0; i < 3; i++)
        {
            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].UpdatePic_Tank(new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 2));
            ledSturts_Group[1].UpdatePic_Tank(new Vector2(0, 0));
        }



        {
            ledSturts_Group[0].enabled = true;
            ledSturts_Group[1].enabled = true;
            if (Set.setVal.Width > 30 || Set.setVal.Height > 30)
            {
                ledSturts_Group[2].enabled = true;

            }
        }
        //   DrawSafePlace(1);
        GetRandom_Tarage();
    }
    void InitMap_04()
    {
        MaxJieDuan = 1;
        Clear();
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;
        ledSturts_Group[1].enabled = true;

        ledSturts_Group[0].Init(false, this);
        ledSturts_Group[1].Init(false, this);
        DrawSafePlace(1);//玩家替换
        GetRandom_Tarage();
    }
    void InitMap_05(int jieduan)
    {
        Clear();
        MaxJieDuan = 2;
        //Game00_Main.instance.isClearTarage = false;
        //remainPoint = 10 + jieduan;
        //if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        //{
        //    if (Game00_Main.instance.player[0].playerUI.Ingame_Setting.set_TarageNum[jieduan] > 0)
        //    {
        //        remainPoint = Game00_Main.instance.player[0].playerUI.Ingame_Setting.set_TarageNum[jieduan];

        //    }
        //}
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;
        ledSturts_Group[1].enabled = true;

        ledSturts_Group[0].Init(false, this);
        ledSturts_Group[1].Init(false, this);
        ledSturts_Group[0].dir = 0;
        ledSturts_Group[1].dir = 1;
        DrawSafePlace(1);//玩家替换
        GetRandom_Tarage();
    }
    Vector2[] YouYu_Tarage = new Vector2[100];
    Vector2[] YouYu_Safe = new Vector2[100];
    public int YouYu_Leng = 0;
    public int YouYu_SafeLeng = 0;
    void GetYouYu_SafePlace()
    {
        int safeNum = 4;
        int X_Ran = Random.Range(2, Set.setVal.Width - 4);
        int Y_Ran = Random.Range(2, Set.setVal.Height - 4);
        switch (Game00_Main.instance.Index_JieDuan)
        {
            case 0:
                safeNum = 20;


                while (safeNum > 0)
                {

                    YouYu_Safe[YouYu_SafeLeng] = new Vector2(X_Ran, Y_Ran);
                    YouYu_SafeLeng++;
                    X_Ran = Random.Range(2, Set.setVal.Width - 4);
                    Y_Ran = Random.Range(2, Set.setVal.Height - 4);
                    safeNum--;
                }


                break;
            case 1:
                safeNum = 15;


                while (safeNum > 0)
                {

                    YouYu_Safe[YouYu_SafeLeng] = new Vector2(X_Ran, Y_Ran);
                    YouYu_SafeLeng++;
                    X_Ran = Random.Range(2, Set.setVal.Width - 4);
                    Y_Ran = Random.Range(2, Set.setVal.Height - 4);
                    safeNum--;
                }


                break;
            case 2:
                safeNum = 10;

                while (safeNum > 0)
                {

                    YouYu_Safe[YouYu_SafeLeng] = new Vector2(X_Ran, Y_Ran);
                    YouYu_SafeLeng++;
                    X_Ran = Random.Range(2, Set.setVal.Width - 4);
                    Y_Ran = Random.Range(2, Set.setVal.Height - 4);
                    safeNum--;
                }


                break;
            default:
            case 3:
                safeNum = 5;

                while (safeNum > 0)
                {

                    YouYu_Safe[YouYu_SafeLeng] = new Vector2(X_Ran, Y_Ran);
                    YouYu_SafeLeng++;
                    X_Ran = Random.Range(2, Set.setVal.Width - 4);
                    Y_Ran = Random.Range(2, Set.setVal.Height - 4);
                    safeNum--;
                }


                break;
        }
    }
    void InitMap_06_YouYu(int jieduan)
    {

        MaxJieDuan = 2;
        Clear();
        runTime = 20f;
        YouYu_SafeLeng = 0;

        for (int i = 0; i < YouYu_Tarage.Length; i++)
        {
            YouYu_Tarage[i] = Vector2.one * -1;

        }
        for (int i = 0; i < YouYu_Safe.Length; i++)
        {
            YouYu_Safe[i] = Vector2.one * -1;
        }
        tarageNum = 30;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        }
        YouYu_Leng = 0;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[0].enabled = false;

        }
        ledSturts_Group[0].enabled = true;


        ledSturts_Group[0].Init(false, this);
        GetYouYu_SafePlace();
        ChangeYouYuSta(en_YouYu_sta.Playing);


        GetRandom_Tarage();

    }

    void InitMap_06(int jieduan)
    {
        MaxJieDuan = 1;

        Clear();
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        //ledSturts_Group[0].enabled = true;
        //   ledSturts_Group[1].enabled = true;

        // ledSturts_Group[0].Init(false, this);
        //   ledSturts_Group[1].Init(false, this);
        int xx = 0;

        xx = Set.setVal.Width / 2;
        DrawPic.DrawCol(xx, y, Set.setVal.Height, 0xff0000, enPointSta.Die);





        ledSturts_Group[0].InitGameMap_06(xx);
        //s  ledSturts_Group[1].InitGameMap_06(xx + 1, Set.setVal.Width - 1);


    }
    void InitMap_07(int jieduan)
    {
        MaxJieDuan = 4;
        Clear();

        tarageNum = 30;
        if (SettingInGame_01.instance != null)
        {
            tarageNum = SettingInGame_01.instance.GetTarageNum();

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);

        DrawSafePlace(1);//玩家替换
        GetRandom_Tarage();
    }


    void InitMap_08(int jieduan)
    {

        angly = 0;
        MaxJieDuan = 3;

        Clear();
        for (int i = 0; i < area.Length; i++)
        {
            area[i] = Random.Range(3, 8);
        }


        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);
        DrawPic.DrawRol(Set.setVal.Width / 2, Set.setVal.Height / 2, 2, 0xff00ff, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width / 2, Set.setVal.Height / 2 + 1, 2, 0xff00ff, enPointSta.Rest);
        for (int i = Set.setVal.Width / 2 - 1; i < Set.setVal.Width / 2 + 1; i++)
        {
            for (int k = Set.setVal.Height / 2 - 1; k < Set.setVal.Height / 2 + 1; k++)
            {
                picId = i + Set.setVal.Width * k;
                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picId]].statue = enPointSta.Rest;

            }
        }
        if (jieduan == 2)
        {
            DrawSafePlace(1);
        }
        //   
        GetRandom_Tarage();
    }
    void InitMap_09()
    {

        Clear();
        MaxJieDuan = 3;
        snakeLength = 5;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);


        ledSturts_Group[0].InitSnake();
        GetRandom_Tarage();
    }
    void InitMap_10()
    {

        Clear();
        MaxJieDuan = 2;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        switch (Game00_Main.instance.Index_JieDuan)
        {
            case 0:
                ledSturts_Group[0].Init(false, this);
                ledSturts_Group[0].enabled = true;
                ledSturts_Group[0].dir = 0;



                ledSturts_Group[1].enabled = true;
                ledSturts_Group[1].dir = 0;
                ledSturts_Group[1].Init(false, this);
                ledSturts_Group[1].x = Set.setVal.Width / 2;

                break;
            case 1:
                ledSturts_Group[0].enabled = true;
                ledSturts_Group[0].dir = 0;
                ledSturts_Group[0].Init(false, this);


                ledSturts_Group[1].enabled = true;
                ledSturts_Group[1].dir = 0;
                ledSturts_Group[1].Init(false, this);
                ledSturts_Group[1].x = Set.setVal.Width / 3;


                ledSturts_Group[2].enabled = true;
                ledSturts_Group[2].dir = 0;
                ledSturts_Group[2].Init(false, this);
                ledSturts_Group[2].x = Set.setVal.Width * 2 / 3;

                break;

        }


        DrawSafePlace(1);//玩家替换

        GetRandom_Tarage();
    }
    void InitMap_11()
    {
        Clear();
        MaxJieDuan = 4;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);

        DrawSafePlace(1);//玩家替换
        for (int i = 0; i < Set.setVal.Height; i++)
        {
            picId = Set.setVal.Width / 2 + i * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest; picId = Set.setVal.Width / 2 - 1 + i * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
        }


        GetRandom_Tarage();
    }
    void InitMap_12()
    {
        Clear();
        MaxJieDuan = 1;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {

            ledSturts_Group[i].enabled = true;

            ledSturts_Group[i].dir = 0;

            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].x = Set.setVal.Width - Random.Range(0, 4);
            ledSturts_Group[i].y = Random.Range(3, Set.setVal.Height - 3);
            ledSturts_Group[i].attackMaxCD = Random.Range(2, 3);


        }



        for (int i = 3; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;



            }

            picId = i + ((Set.setVal.Height - 1) * Set.setVal.Width);
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;


            picId = i + ((Set.setVal.Height - 2) * Set.setVal.Width);
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
        }

        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;


            }

        }
        GetRandom_Tarage();
    }
    void InitMap_13()
    {

        Clear();
        MaxJieDuan = 2;

        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;



            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].dir = 0;
            ledSturts_Group[1].x = Set.setVal.Width - 3;
            ledSturts_Group[1].y = Set.setVal.Height - 3;
        }



        for (int i = Game_Fangkuai_length; i < Set.setVal.Width - Game_Fangkuai_length; i++)//x
        {
            for (int k = Game_Fangkuai_length; k < Set.setVal.Height - Game_Fangkuai_length; k++)//y
            {

                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;



            }
        }
        Cnt = 0;
        Tarage_Pos = new Vector2[Set.setVal.Width * Set.setVal.Height];
        if (Game00_Main.instance.Index_JieDuan == 1)
        {
            x = Game_Fangkuai_length + 1;
            y = Game_Fangkuai_length + 1;
            for (int i = Game_Fangkuai_length + 1; i < Set.setVal.Width - Game_Fangkuai_length - 1; i++)//x
            {
                for (int k = Game_Fangkuai_length + 1; k < Set.setVal.Height - Game_Fangkuai_length - 1; k++)//y
                {

                    picId = i + k * Set.setVal.Width;
                    pointId = Framebuffer.tab_Mapping[picId];
                    Tarage_Pos[Cnt].Set(i, k);
                    Cnt++;
                    GameLedControl.gamePoint[pointId].statue = enPointSta.None;



                }
            }
        }



        GetRandom_Tarage();


    }
    void InitMap_14()
    {
        Clear();
        DieTime_14 = 1;
        MaxJieDuan = 1;
        tarageNum = 20;
        Cnt = 0;
        StayTime = 3;
        isFirst_NewMap01 = true;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        Tarage_Pos = new Vector2[Set.setVal.Width * Set.setVal.Height];
        for (int i = 0; i < Set.setVal.Height; i++)
        {
            for (int k = 4; k < Set.setVal.Width; k++)
            {
                picId = k + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
            }

        }
        int xCnt = 2;
        bool xBool = false;

        for (int i = 0; i < Set.setVal.Height; i++)
        {
            if (xCnt == 0)
            {
                xBool = !xBool;
                xCnt = 4;
                ledSturts_Group[Cnt].Init();

                ledSturts_Group[Cnt].Init_NewMap_14(i);
                ledSturts_Group[Cnt].enabled = true;
                Cnt++;
            }
            xCnt--;
        }

        GetRandom_Tarage();
        Cnt = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
                {
                    Tarage_Pos[Cnt] = new Vector2(i, k);
                    Cnt++;
                }

            }

        }
    }
    void InitMap_14_Old()
    {
        Clear();
        MaxJieDuan = 4;
        tarageNum = 10;// SettingInGame_01.instance.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;

            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].dir = i;
        }
        int cntX = -3;
        int cntY = -3;


        for (int i = 0; i < Set.setVal.Width; i++)//x
        {



            cntX++;



            for (int k = 0; k < Set.setVal.Height; k++)//y
            {

                if (cntX < 0)
                {
                    continue;
                }
                else if (cntX == 1)
                {
                    if (k == Set.setVal.Height - 1)
                    {
                        cntX = -3;
                    }

                }
                cntY++;
                if (cntY < 0)
                {

                    continue;
                }
                else if (cntY == 1)
                {
                    cntY = -3;
                }
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;



            }
        }



        GetRandom_Tarage();

    }
    void InitMap_14_1()
    {
        Clear();
        MaxJieDuan = 4;
        tarageNum = SettingInGame_01.instance.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;



            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].dir = i;
        }
        int cntX = -3;
        int cntY = -3;


        for (int i = 0; i < Set.setVal.Width; i++)//x
        {



            cntX++;



            for (int k = 0; k < Set.setVal.Height; k++)//y
            {

                if (cntX < 0)
                {
                    continue;
                }
                else if (cntX == 0)
                {
                    if (k == Set.setVal.Height - 1)
                    {
                        cntX = -3;
                    }

                }
                cntY++;
                if (cntY < 0)
                {

                    continue;
                }
                else if (cntY == 1)
                {
                    cntY = -3;
                }
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;



            }
        }

        GetRandom_Tarage();
    }
    void InitMap_15(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);

        ledSturts_Group[1].enabled = true;


        ledSturts_Group[0].Init(false, this);
        ledSturts_Group[1].dir = 1;
        ledSturts_Group[1].x = Set.setVal.Width / 3;


        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < Set.setVal.Width; k++)
            {
                picId = i * Set.setVal.Width + k;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            }
        }
        if (jieduan == 4)
        {


            for (int k = 0; k < Set.setVal.Height; k++)
            {
                ChangeLed_Sta(Set.setVal.Width - 1, k, enPointSta.Die);
                ChangeLed_Sta(0, k, enPointSta.Die);
            }

            for (int i = 0; i < Set.setVal.Width; i++)
            {
                ChangeLed_Sta(i, Set.setVal.Height - 1, enPointSta.Die);
                ChangeLed_Sta(i, 3, enPointSta.Die);

            }
        }
        switch (jieduan)
        {
            default:
                break;
            case 1:
                ledSturts_Group[0].x = ledSturts_Group[0].y = 0;
                break;
            case 2:
                ledSturts_Group[0].y = Set.setVal.Height / 2;
                break;
            case 3:
                ledSturts_Group[0].y = Set.setVal.Height / 2;
                break;
            case 4:
                ledSturts_Group[1].enabled = true;
                ledSturts_Group[1].Init(false, this);
                ledSturts_Group[1].dir = 1;

                ledSturts_Group[0].y = Set.setVal.Height / 2;
                ledSturts_Group[0].x = 0;

                ledSturts_Group[1].y = ledSturts_Group[0].y + 2;
                ledSturts_Group[1].x = Set.setVal.Width - 1;






                //DrawPic.DrawRol(0, Set.setVal.Height - 1, Set.setVal.Width - 1, 0xff0000, enPointSta.Die);
                //DrawPic.DrawRol(0, 3, Set.setVal.Width - 1, 0xff0000, enPointSta.Die);

                break;
        }

        GetRandom_Tarage();

    }
    void InitMap_16(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }


        for (int i = 0; i < 4; i++)
        {
            ledSturts_Group[i].enabled = true;



            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].dir = i % 2;
        }
        ledSturts_Group[1].x = Set.setVal.Width - 1;
        ledSturts_Group[3].y = Set.setVal.Height - 1;
        DrawSafePlace(1);
        GetRandom_Tarage();

    }
    void InitMap_17(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;
        Game00_Main.instance.isClearTarage = false;
        remainPoint = 10 + jieduan;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            if (SettingInGame_01.instance.GetReMainNum(jieduan) > 0)
            {
                remainPoint = SettingInGame_01.instance.GetReMainNum(jieduan);

            }
        }
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }

        for (int i = 0; i < 4; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init(false, this);






        }
        switch (jieduan)
        {
            case 0:
                ledSturts_Group[0].Snake_length_17 = Set.setVal.Height - 2;
                for (int k = 0; k < ledSturts_Group[0].Snake_length_17; k++)
                {
                    ledSturts_Group[0].SnakePos_17_Long[k] = new Vector2(1, Set.setVal.Height - 2);



                }
                ledSturts_Group[0].dir = 0;
                break;
            case 1:
                ledSturts_Group[0].Snake_length_17 = Set.setVal.Height - 2;
                ledSturts_Group[1].Snake_length_17 = Set.setVal.Height - 2;
                for (int k = 0; k < ledSturts_Group[0].Snake_length_17; k++)
                {
                    ledSturts_Group[0].SnakePos_17_Long[k] = new Vector2(1, Set.setVal.Height - 2);
                    ledSturts_Group[1].SnakePos_17_Long[k] = new Vector2(Set.setVal.Width - 2, 1);
                }
                ledSturts_Group[0].dir = 0;
                ledSturts_Group[1].dir = 2;

                break;

            case 2:
                ledSturts_Group[0].Snake_length_17 = Set.setVal.Height - 2;
                ledSturts_Group[1].Snake_length_17 = Set.setVal.Height - 2;
                for (int k = 0; k < ledSturts_Group[0].Snake_length_17; k++)
                {
                    ledSturts_Group[0].SnakePos_17_Long[k] = new Vector2(1, Set.setVal.Height - 2);
                    ledSturts_Group[1].SnakePos_17_Long[k] = new Vector2(Set.setVal.Width / 4, Set.setVal.Height / 4);
                }
                ledSturts_Group[0].dir = 0;
                ledSturts_Group[1].dir = 2;

                break;
            case 3:
            case 4:
                ledSturts_Group[0].Snake_length_17 = Set.setVal.Height - 2;
                ledSturts_Group[1].Snake_length_17 = Set.setVal.Height / 2;
                ledSturts_Group[2].Snake_length_17 = Set.setVal.Height / 4;
                for (int k = 0; k < ledSturts_Group[0].Snake_length_17; k++)
                {
                    ledSturts_Group[0].SnakePos_17_Long[k] = new Vector2(1, Set.setVal.Height - 2);
                    ledSturts_Group[1].SnakePos_17_Long[k] = new Vector2(Set.setVal.Width / 4, Set.setVal.Height / 4);
                    ledSturts_Group[2].SnakePos_17_Long[k] = new Vector2(Set.setVal.Width / 3, Set.setVal.Height / 3);
                }
                ledSturts_Group[0].dir = 0;
                ledSturts_Group[1].dir = 2;
                ledSturts_Group[3].dir = 2;
                for (int i = 0; i < 3; i++)
                {
                    picId = Set.setVal.Height / 4 * Set.setVal.Width + Set.setVal.Width / 4 - 1 + i;
                    pointId = Framebuffer.tab_Mapping[picId];
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;


                    picId = (Set.setVal.Height + 1) / 4 * Set.setVal.Width + Set.setVal.Width / 4 - 1 + i;
                    pointId = Framebuffer.tab_Mapping[picId];

                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                    picId = (Set.setVal.Height + 1) / 3 * Set.setVal.Width + Set.setVal.Width / 2 - 1 + i;
                    pointId = Framebuffer.tab_Mapping[picId];
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                }


                break;
        }
        DrawSafePlace(1);
        GetRandom_Tarage();

    }
    public int New_18_Height = 0;
    public int New_18_Height1 = 0;
    public int New_18_Height2 = 0;
    public int New_18_Down = 0;

    float waitime = 0;

    void InitMap_18(int jieduan)
    {
        int minsize = 12;
        Clear();
        MaxJieDuan = 1;
        tarageNum = 20;

        tarageNum = SettingInGame_01.instance.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        Cnt = 0;
        Tarage_Pos = new Vector2[Set.setVal.Width * Set.setVal.Height];
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            Tarage_Pos[Cnt] = new Vector2(i, 0);
            Cnt++;
            Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height - 1);
            Cnt++;
        }
        for (int i = 0; i < Set.setVal.Height; i++)
        {
            Tarage_Pos[Cnt] = new Vector2(0, i);
            Cnt++;
            Tarage_Pos[Cnt] = new Vector2(Set.setVal.Width - 1, i);
            Cnt++;
        }
        //第一柱子
        New_18_Height = Set.setVal.Height * 7 / 10;
        for (int i = 0; i < New_18_Height; i++)
        {
            Tarage_Pos[Cnt] = new Vector2(Set.setVal.Width / 4, i);
            Cnt++;
            if (Set.setVal.Width > minsize)
            {
                Tarage_Pos[Cnt] = new Vector2(Set.setVal.Width / 4 + 1, i);
                Cnt++;
            }


        }
        ///第二柱子
        New_18_Height1 = Set.setVal.Height * 5 / 10;
        ///
        for (int i = Set.setVal.Height * 3 / 10; i < New_18_Height1; i++)
        {
            Tarage_Pos[Cnt] = new Vector2(Set.setVal.Width / 2, i);
            Cnt++;
            if (Set.setVal.Width > minsize)
            {
                Tarage_Pos[Cnt] = new Vector2(Set.setVal.Width / 2 + 1, i);
                Cnt++;
            }
        }
        //第三柱子
        New_18_Height2 = Set.setVal.Height * 7 / 10;



        for (int i = Set.setVal.Height * 3 / 10; i < New_18_Height2; i++)
        {
            Tarage_Pos[Cnt] = new Vector2(Set.setVal.Width * 3 / 4, i);
            Cnt++;
            if (Set.setVal.Width > minsize && Set.setVal.Width > 12)
            {
                Tarage_Pos[Cnt] = new Vector2(Set.setVal.Width * 3 / 4 + 1, i);
                Cnt++;
            }
        }

        New_18_Down = Set.setVal.Height * 7 / 10;

        if (Set.setVal.Width > minsize && Set.setVal.Width > 12)
        {
            //第一横柱
            for (int i = Set.setVal.Width / 4; i < Set.setVal.Width * 3 / 4 + 2; i++)
            {
                if (Set.setVal.Width > minsize)
                {
                    Tarage_Pos[Cnt] = new Vector2(i, New_18_Height2);
                    Cnt++;
                }
                Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height * 7 / 10);
                Cnt++;
            }

            for (int i = Set.setVal.Width / 2; i < Set.setVal.Width * 3 / 4 + 2; i++)
            {
                Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height * 2 / 10);
                Cnt++;
                Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height * 2 / 10 + 1);
                Cnt++;
            }
        }
        else
        {
            //第一横柱
            for (int i = Set.setVal.Width / 4; i < Set.setVal.Width * 3 / 4 + 1; i++)
            {

                Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height * 7 / 10);
                Cnt++;
            }

            for (int i = Set.setVal.Width / 2; i < Set.setVal.Width * 3 / 4 + 1; i++)
            {

                Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height * 2 / 10 + 1);
                Cnt++;
            }
        }
        waitime = 5;
        ledSturts_Group[0].x = ledSturts_Group[0].y = 0;
        ledSturts_Group[2].x = ledSturts_Group[2].y = 0;
        ledSturts_Group[1].x = Set.setVal.Width;
        ledSturts_Group[1].y = Set.setVal.Height;
        ledSturts_Group[3].y = Set.setVal.Height;


        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            picId = (int)Tarage_Pos[i].x + (int)Tarage_Pos[i].y * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
            Framebuffer.Update_PointColor(pointId, 0xff0000, enPointSta.Die);
        }
        GetRandom_Tarage();

    }
    void InitMap_19(int jieduan)
    {
        Clear();
        MaxJieDuan = 1;
        tarageNum = 20;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < 3; i++)
        {
            ledSturts_Group[i].enabled = true;

            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].x = Set.setVal.Width * i / 3;

        }


        ledSturts_Group[0].Init_Game19_M();
        for (int i = 0; i < Set.setVal.Width; i++)
        {

            picId = i + 0 * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            Framebuffer.Update_PointColor(pointId, 0xff0000, enPointSta.Rest);
            picId = i + (Set.setVal.Height - 1) * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            Framebuffer.Update_PointColor(pointId, 0xff0000, enPointSta.Rest);
        }
        GetRandom_Tarage();

    }
    void InitMap_20(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;
        tarageNum = 20;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }



        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;

            ledSturts_Group[i].Init(false, this);

            ledSturts_Group[i].InitKuai_X(i);

            ledSturts_Group[i].UpdateFangKuai_X();

        }






    }
    Vector2 startPos = new Vector2();
    Vector2 startPos1 = new Vector2();

    void InitMap_21(int jieduan)
    {
        Clear();
        r = 0; r1 = 0;
        MaxJieDuan = 1;
        tarageNum = 20;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        for (int i = 0; i < 1; i++)
        {
            ledSturts_Group[i].enabled = true;



            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].Init_FaBo();

        }
        startPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 2);

        DrawSafePlace(1);
        GetRandom_Tarage();

    }
    int width_22 = 0;
    void InitMap_22()
    {
        Clear();
        MaxJieDuan = 1;
        tarageNum = 20;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        if (Set.setVal.Width < 16)
        {
            width_22 = Set.setVal.Width / 2;
            for (int i = 0; i < width_22; i++)
            {
                ledSturts_Group[i].enabled = true;
                ledSturts_Group[i].Init();
                ledSturts_Group[i].ID = i;
                ledSturts_Group[i].waitTime = i * 2;
                ledSturts_Group[i].y = Set.setVal.Height - 2;
            }
        }
        else
        {
            width_22 = Set.setVal.Width / ledSturts_Group.Length;
            for (int i = 0; i < ledSturts_Group.Length; i++)
            {
                ledSturts_Group[i].enabled = true;
                ledSturts_Group[i].Init();
                ledSturts_Group[i].ID = i;
                ledSturts_Group[i].waitTime = i * 2;

                ledSturts_Group[i].y = Set.setVal.Height - 2;
            }
        }

        tarageNum = SettingInGame_01.instance.GetTarageNum();





        Cnt = 0;

        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < Set.setVal.Width; k++)
            {
                picId = k + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            }

        }
        x = y = 0;


        GetRandom_Tarage();
    }
    void InitMap_22_Old(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;

        tarageNum = 20;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < 3; i++)
        {
            ledSturts_Group[i].enabled = true;

            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].dir = i % 2;
        }


        switch (jieduan)
        {
            case 0:
                ledSturts_Group[0].x = Set.setVal.Width / 3;
                ledSturts_Group[0].y = Set.setVal.Height / 3;


                break;
            case 1:
            case 2:

                ledSturts_Group[0].x = Set.setVal.Width / 3;
                ledSturts_Group[0].y = Set.setVal.Height / 4;
                ledSturts_Group[1].x = Set.setVal.Width / 3;
                ledSturts_Group[1].y = Set.setVal.Height * 3 / 4;

                break;
            case 3:
            case 4:
            case 5:
                for (int i = 0; i < 3; i++)
                {
                    ledSturts_Group[i].x = Set.setVal.Width / 3;


                    ledSturts_Group[i].y = Set.setVal.Height * (i + 1) / 4;
                }


                break;
        }

        GetRandom_Tarage();


    }
    int[] Cnt_ShakeMap_23;

    bool[] GoDown_ShakeMap_23;
    uint[] Color_ShakeMap_23;

    void InitMap_23()
    {
        Clear();
        MaxJieDuan = 1;
        tarageNum = 20;
        Cnt = 0;
        int xCnt = 2;
        int yCnt = 2;
        bool xOK = false;
        bool yOK = false;
        Cnt_ShakeMap_23 = new int[Set.setVal.Width * Set.setVal.Height];

        Color_ShakeMap_23 = new uint[Set.setVal.Width * Set.setVal.Height];
        GoDown_ShakeMap_23 = new bool[Set.setVal.Width * Set.setVal.Height];

        for (int i = 0; i < Cnt_ShakeMap_23.Length; i++)
        {
            Cnt_ShakeMap_23[i] = 2;
        }
        for (int i = 0; i < Cnt_ShakeMap_23.Length; i++)
        {
            Color_ShakeMap_23[i] = 0xff0000;
        }
        for (int i = 0; i < Cnt_ShakeMap_23.Length; i++)
        {
            GoDown_ShakeMap_23[i] = true;
        }
        Tarage_Pos = new Vector2[Set.setVal.Width * Set.setVal.Height];
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            if (xCnt == 0)
            {
                xCnt = 2;
                xOK = !xOK;
                yCnt = 2;
            }
            xCnt--;
            yCnt = 2;
            yOK = false;
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                if (!xOK)
                {
                    break;
                }
                if (yCnt == 0)
                {
                    yCnt = 2;
                    yOK = !yOK;

                }
                yCnt--;


                if (yOK)
                {
                    Tarage_Pos[Cnt] = new Vector2(i, k);
                    Cnt++;

                }

            }
        }
        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            if ((int)Tarage_Pos[i].x == 0 && (int)Tarage_Pos[i].y == 0)
            {
                continue;
            }
            picId = (int)Tarage_Pos[i].x + Set.setVal.Width * (int)Tarage_Pos[i].y;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
        }
    }
    void InitMap_23_OLD(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;
        tarageNum = 20;
        Game00_Main.instance.isClearTarage = false;
        remainPoint = 10 + jieduan;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            if (SettingInGame_01.instance.GetReMainNum(jieduan) > 0)
            {
                remainPoint = SettingInGame_01.instance.GetReMainNum(jieduan);

            }
        }
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;

            ledSturts_Group[i].Init(false, this);
            ledSturts_Group[i].dir = i % 2;
        }


        for (int i = 0; i < 2; i++)
        {


            ledSturts_Group[i].y = Set.setVal.Height / 4 + Set.setVal.Height * (i) / 2;
        }
        DrawSafePlace(2);
        GetRandom_Tarage();


    }
    void InitMap_24(int jieduan)
    {
        Clear();
        MaxJieDuan = 3;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;

            ledSturts_Group[i].Init(false, this);
            for (int k = 0; k < ledSturts_Group[i].fangkuai_24_Pos.Length; k++)
            {
                ledSturts_Group[i].fangkuai_24_Pos[k] = Vector2.one * -1;
            }

        }



        GetRandom_Tarage();


    }
    void InitMap_27(int jieduan)
    {

        Clear();
        isClearing_Game25 = false;
        isClearAll = false;
        tarageCol = Random.Range(0, 4);
        isOver_25 = false;
        x = 0;
        y = 0;
        MaxJieDuan = 2;

        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < 5; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init();
        }
        ledSturts_Group[1].x = 4;
        ledSturts_Group[1].y = 0;
        ledSturts_Group[2].x = Set.setVal.Width - 1;
        ledSturts_Group[2].y = 0;

        ledSturts_Group[3].x = Set.setVal.Width - 1;
        ledSturts_Group[3].y = Set.setVal.Height - 1;

        ledSturts_Group[4].x = 4;
        ledSturts_Group[4].y = Set.setVal.Height - 1;
        Cnt = 0;
        y = 2;
        Tarage_Pos = new Vector2[Set.setVal.Width * Set.setVal.Height];

        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                Tarage_Pos[Cnt].Set(i, k);
                Cnt++;
            }

        }
        for (int i = 4; i < Set.setVal.Width - 1; i++)
        {
            Tarage_Pos[Cnt].Set(i, 1);
            Cnt++;
            Tarage_Pos[Cnt].Set(i, Set.setVal.Height - 2);
            Cnt++;
        }


        for (int k = 1; k < Set.setVal.Height - 2; k++)
        {
            Tarage_Pos[Cnt].Set(4, k);
            Cnt++;
            Tarage_Pos[Cnt].Set(4, k);
            Cnt++;
            Tarage_Pos[Cnt].Set(Set.setVal.Width - 2, k);
            Cnt++;
            Tarage_Pos[Cnt].Set(Set.setVal.Width - 2, k);
            Cnt++;

            Tarage_Pos[Cnt].Set((4 + Set.setVal.Width - 2) / 2 - 1, k);
            Cnt++;
            Tarage_Pos[Cnt].Set((4 + Set.setVal.Width - 2) / 2 + 1, k);
            Cnt++;
        }


        //DrawPic.DrawCol(Set.setVal.Width - 2, 1, Set.setVal.Height - 2, 0xff0000, enPointSta.Die);
        //DrawPic.DrawCol(Set.setVal.Width / 2 - 1, Set.setVal.Height / 5, Set.setVal.Height * 3 / 5, 0xff0000, enPointSta.Die);
        //DrawPic.DrawCol(Set.setVal.Width / 2 + 1, Set.setVal.Height / 5, Set.setVal.Height * 3 / 5, 0xff0000, enPointSta.Die);
        //DrawPic.DrawRol(4, 1, Set.setVal.Width - 5, 0x00ff00, enPointSta.Die);
        //DrawPic.DrawRol(4, Set.setVal.Height - 2, Set.setVal.Width - 5, 0xff0000, enPointSta.Die);

        for (int i = 0; i < Cnt; i++)
        {
            picId = (int)Tarage_Pos[i].x + (int)Tarage_Pos[i].y * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;

        }
        GetRandom_Tarage();
    }
    public Vector2 GetSafePlace(int i)
    {
        bool isOK = true;
        int xx = Random.Range(0, Set.setVal.Width / 3);
        int yy = Random.Range(0, Set.setVal.Height / 3);
        int cnt = 0;
        while (isOK)
        {
            xx = Random.Range(0, Set.setVal.Width / 3);
            yy = Random.Range(0, Set.setVal.Height / 3);
            isOK = isSafe_Pass(i, new Vector2(xx, yy));

            cnt++;
            if (cnt > 100)
            {
                break;
            }
        }



        Vector2 pos = new Vector2(xx, yy);
        return pos;
    }
    bool isSafe_Pass(int i, Vector2 pos)
    {
        for (int n = 0; n < ledSturts_Group.Length; n++)
        {
            if (i == n)
            {
                continue;
            }
            if (pos == new Vector2(ledSturts_Group[i].x, ledSturts_Group[i].y))
            {
                return true;
            }

        }

        return false;
    }
    public int MaxSaftNum = 5;
    void InitMap_28(int jieduan)
    {
        Clear();
        isClearing_Game25 = false;
        isClearAll = false;
        tarageCol = Random.Range(0, 4);
        isOver_25 = false;
        x = 0;
        y = 0;
        MaxJieDuan = 1;
        MaxSaftNum = 5;
        r = 1;
        r1 = 3;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < MaxSaftNum; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init();
            ledSturts_Group[i].ID = i;



            ledSturts_Group[i].Init_SafePlace(i);
            ledSturts_Group[i].x = 3 * (i);

            ledSturts_Group[i].y = i * 3;
            ledSturts_Group[i].SafeTime = (i + 1) * 4;
        }


    }

    void InitMap_29()
    {
        MaxJieDuan = 1;
        Clear();
        isClearing_Game25 = false;
        isClearAll = false;
        tarageCol = Random.Range(0, 4);
        isOver_25 = false;
        dir = 0;
        Cnt = 0;
        Tarage_Pos = new Vector2[Set.setVal.Width * 4];
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            // for (int k= 0; k < Set.setVal.Height; k++)
            {
                Tarage_Pos[Cnt] = new Vector2(i, 0);
                Cnt++;
                Tarage_Pos[Cnt] = new Vector2(i, 1);
                Cnt++;
                Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height - 1);
                Cnt++;
                Tarage_Pos[Cnt] = new Vector2(i, Set.setVal.Height - 2);
                Cnt++;
            }
        }
        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            picId = (int)Tarage_Pos[i].x + Set.setVal.Width * (int)Tarage_Pos[i].y;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
        }
        GetRandom_Tarage();
        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            picId = (int)Tarage_Pos[i].x + Set.setVal.Width * (int)Tarage_Pos[i].y;
            pointId = Framebuffer.tab_Mapping[picId];

            GameLedControl.gamePoint[pointId].statue = enPointSta.None;
        }
    }
    void InitMap_29_Old(int jieduan)
    {

        Clear();
        isClearing_Game25 = false;
        isClearAll = false;
        tarageCol = Random.Range(0, 4);
        isOver_25 = false;
        x = 0;
        y = 0;
        MaxJieDuan = 3;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;
        }




        ChangeLed_Sta(Set.setVal.Width / 4, 0, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 4 + 1, 0, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4, 0, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4 + 1, 0, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2 + 1, 0, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2, 0, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 4, 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 4 + 1, 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4, 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4 + 1, 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2 + 1, 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2, 1, enPointSta.Rest);


        ChangeLed_Sta(Set.setVal.Width / 4, Set.setVal.Height - 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 4 + 1, Set.setVal.Height - 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4, Set.setVal.Height - 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4 + 1, Set.setVal.Height - 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2 + 1, Set.setVal.Height - 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2, Set.setVal.Height - 1, enPointSta.Rest);

        ChangeLed_Sta(Set.setVal.Width / 4, Set.setVal.Height - 2, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 4 + 1, Set.setVal.Height - 2, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4, Set.setVal.Height - 2, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width * 3 / 4 + 1, Set.setVal.Height - 2, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2 + 1, Set.setVal.Height - 2, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width / 2, Set.setVal.Height - 2, enPointSta.Rest);

        ChangeLed_Sta(0, Set.setVal.Height / 2, enPointSta.Rest);
        ChangeLed_Sta(1, Set.setVal.Height / 2, enPointSta.Rest);
        ChangeLed_Sta(0, Set.setVal.Height / 2 - 1, enPointSta.Rest);
        ChangeLed_Sta(1, Set.setVal.Height / 2 - 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width - 1, Set.setVal.Height / 2, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width - 2, Set.setVal.Height / 2, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width - 1, Set.setVal.Height / 2 - 1, enPointSta.Rest);
        ChangeLed_Sta(Set.setVal.Width - 2, Set.setVal.Height / 2 - 1, enPointSta.Rest);

        switch (jieduan)//蓝
        {
            case 0:
                for (int i = 2; i < Set.setVal.Width - 2; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 2; k++)
                    {
                        picId = i + k * Set.setVal.Width;
                        pointId = Framebuffer.tab_Mapping[picId];

                        GameLedControl.gamePoint[pointId].statue = enPointSta.Die;

                    }
                }
                ledSturts_Group[1].x = Set.setVal.Width - 2;
                ledSturts_Group[1].y = 0;
                ledSturts_Group[1].dir = 1;

                GetRandom_Tarage();

                break;
            case 1:


                for (int i = 2; i < Set.setVal.Width - 3; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 3; k++)
                    {

                        if (i % 3 == 0 || k % 3 == 0)
                        {
                            continue;
                        }
                        ChangeLed_Sta(i, k, enPointSta.Target);



                    }

                }
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 0; k < Set.setVal.Height; k++)
                    {
                        picId = i + k * Set.setVal.Width;
                        pointId = Framebuffer.tab_Mapping[picId];
                        if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
                        {
                            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                        }
                    }
                }
                break;
            case 2:
                ledSturts_Group[1].x = Set.setVal.Width - 3;
                ledSturts_Group[0].y = 3;
                ledSturts_Group[1].y = Set.setVal.Height - 3;
                ledSturts_Group[1].dir = 1;

                GetRandom_Tarage();
                break;



            case 3:


                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 0; k < Set.setVal.Height; k++)
                    {
                        picId = i + k * Set.setVal.Width;
                        pointId = Framebuffer.tab_Mapping[picId];
                        if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
                        {
                            GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
                            GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
                        }
                    }
                }

                break;
        }

    }

    uint[] col = { 0x00FF00,//绿

                0x0000FF,//蓝
                 0xFffA00,////黄

                 0xFF00ff,////紫
                 0xFF6300,////橙色
    };

    float MaxWaitTime = 6f;
    void InitMap_25()
    {
        haveDesBlood = false;
        tarageCol = Random.Range(0, 5);
       
        Clear();
        Framebuffer.Update_ColorFull(0, enPointSta.None);
        MaxJieDuan = 4;

       // Game00_Main.instance.isClearTarage = false;
        runTime = MaxWaitTime;
        isClearing_Game25 = false;
        tarageNum = 4;

        while (tarageNum > 0)
        {
             
            randomX = Random.Range(0, Set.setVal.Width / 2);
            randomY = Random.Range(0, Set.setVal.Height / 2);




            ChangeLed_Sta_Col(randomX * 2, randomY * 2, col[tarageCol], enPointSta.Rest);
            ChangeLed_Sta_Col(randomX * 2 + 1, randomY * 2, col[tarageCol], enPointSta.Rest);
            ChangeLed_Sta_Col(randomX * 2 + 1, randomY * 2 + 1, col[tarageCol], enPointSta.Rest);
            ChangeLed_Sta_Col(randomX * 2, randomY * 2 + 1, col[tarageCol], enPointSta.Rest);

            tarageNum--;
            if (tarageNum==0)
            {
                startPos_25.Set(randomX, randomY);
            }
        }
        for (int i = 0; i < Set.setVal.Width; i += 2)
        {
            for (int k = 0; k < Set.setVal.Height; k += 2)
            {
                picId = i + Set.setVal.Width * k;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue != enPointSta.None)
                {
                    continue;
                }
                int coolor = Random.Range(0, 5);
                while (coolor == tarageCol)
                {
                    coolor = Random.Range(0, 5);
                }


                //  DrawPic.DrawCol(i, k, 2, col, enPointSta.Rest);
                ChangeLed_Sta_Col(i, k, col[coolor], enPointSta.Rest);
                ChangeLed_Sta_Col(i, k + 1, col[coolor], enPointSta.Rest);

                ChangeLed_Sta_Col(i + 1, k, col[coolor], enPointSta.Rest);
                ChangeLed_Sta_Col(i + 1, k + 1, col[coolor], enPointSta.Rest);
            }

        }
        if (Game00_Main.instance.Index_JieDuan < MaxJieDuan)
        {
            sound_Youyu.Stop();
            sound_Youyu.clip = Spr_Tatage_Color[tarageCol];
            sound_Youyu.Play();
        }

    }

    float StayTime = 0;
    float MaxStayTime = 4;
    public bool haveTwo = false;
    Vector2[] Tarage_Pos;
    //
    int DieCnt_Map02 = 0;
    float DieCnt_Time = 0;
    float FleshTime_Map02 = 0;
    void Init_NewMap_02()
    {
        FleshTime_Map02 = 10;
        Clear();
        isFirst_NewMap01 = true;
        isClearing_Game25 = false;
        isClearAll = false;
        DieCnt_Map02 = 0; DieCnt_Time = 0.5f;
        isOver_25 = false;
        MaxJieDuan = 1;
        tarageNum = 50;
        tarageNum = SettingInGame_01.instance.GetTarageNum();
        //    Game00_Main.instance.isClearTarage = false;

        Tarage_Pos = new Vector2[2 * Set.setVal.Width];
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < 6; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].InitNewGameMap_02(i);
        }
        for (int i = 0; i < Set.setVal.Height; i++)
        {

            {
                picId = 0 + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                picId = Set.setVal.Width - 1 + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
            }

        }
        GetRandom_Tarage();

    }
    //
  public  void Init_NewMap_02_Next()
    {
        FleshTime_Map02 = 10;
        Clear();
        isFirst_NewMap01 = false;
        isClearing_Game25 = false;
        isClearAll = false;
        DieCnt_Map02 = 0;
        DieCnt_Time = 0.5f;
        isOver_25 = false;
        MaxJieDuan = 1;
        tarageNum = 50;
        tarageNum = SettingInGame_01.instance.GetTarageNum();


        Tarage_Pos = new Vector2[2 * Set.setVal.Width];
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        if (!isFirst_NewMap01)
        {
            for (int i = 0; i < Set.setVal.Width; i++)
            {
                for (int k = 0; k < Set.setVal.Height / 2; k++)
                {
                    picId = i + k * Set.setVal.Width;
                    pointId = Framebuffer.tab_Mapping[picId];
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                }
            }
        }
        GetRandom_Tarage();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height / 2; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.None;
            }
        }
        for (int i = 0; i < 1; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].InitNewGameMap_02_next();
        }
    }
    //
    void Init_NewMap_03()
    {

        Clear();
        isFirst_NewMap01 = true;
        isClearing_Game25 = false;
        isClearAll = false;


        MaxJieDuan = 1;
        tarageNum = 50;
        tarageNum = SettingInGame_01.instance.GetTarageNum();
        //    Game00_Main.instance.isClearTarage = false;

        Tarage_Pos = new Vector2[2 * Set.setVal.Width];
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < 3; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init();
            ledSturts_Group[i].waitTime = i * 6f;
            ledSturts_Group[i].x = y = 2 * i;
        }
        for (int i = 0; i < 2; i++)
        {
            for (int k = Set.setVal.Height / 4; k < Set.setVal.Height * 3 / 4; k++)
            {
                picId = Set.setVal.Width / 2 + i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            }
        }
        GetRandom_Tarage();

    }
    void Init_NewMap_03_next()
    {

        Clear();
        isFirst_NewMap01 = false;
        isClearing_Game25 = false;
        isClearAll = false;


        MaxJieDuan = 1;
        tarageNum = 50;
        tarageNum = SettingInGame_01.instance.GetTarageNum();
        //    Game00_Main.instance.isClearTarage = false;

        Tarage_Pos = new Vector2[Set.setVal.Height * Set.setVal.Width];
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < 4; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init();
            ledSturts_Group[i].waitTime = i;
        }
        for (int i = 0; i < 2; i++)
        {
            for (int k = Set.setVal.Height / 4; k < Set.setVal.Height * 3 / 4; k++)
            {
                picId = Set.setVal.Width / 2 + i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            }
        }
        GetRandom_Tarage();
        Cnt = 0;
        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            Tarage_Pos[i] = Vector2.zero;
        }

        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue != enPointSta.Target && GameLedControl.gamePoint[pointId].statue != enPointSta.Rest)
                {
                    Tarage_Pos[Cnt] = new Vector2(i, k);
                    Cnt++;
                }
            }
        }
    }
    //

    bool isFirst_NewMap01 = false;

    void Init_NewMap_01()
    {
        Clear();
        isFirst_NewMap01 = true;
        isClearing_Game25 = false;
        isClearAll = false;

        isOver_25 = false;
        MaxJieDuan = 1;
        tarageNum = 20;// SettingInGame_01.instance.GetTarageNum();
        int he = Set.setVal.Height / 4 - 1;
        Tarage_Pos = new Vector2[2 * Set.setVal.Width];
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < he; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init();
            ledSturts_Group[i].InitNewGameMap_01(4 + i * 4);


        }
        for (int i = 0; i < Tarage_Pos.Length / 2; i++)
        {
            Tarage_Pos[i] = new Vector2(i, Set.setVal.Height - 2);
            Tarage_Pos[i + Set.setVal.Width] = new Vector2(i, Set.setVal.Height - 1);
        }
        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            if (Tarage_Pos[i].x >= 0)
            {
                picId = (int)Tarage_Pos[i].x + (int)Tarage_Pos[i].y * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                Framebuffer.Update_PointColor((int)Tarage_Pos[i].x, (int)Tarage_Pos[i].y, 255, enPointSta.Target);
                GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
            }
        }

    }
  public  void Init_NewMap_01_next()
    {
        Clear();
        isFirst_NewMap01 = false;
        isClearing_Game25 = false;
        isClearAll = false;

        isOver_25 = false;
        MaxJieDuan = 1;
        tarageNum = 20;// SettingInGame_01.instance.GetTarageNum();
        int he = Set.setVal.Height / 4 - 1;
        Tarage_Pos = new Vector2[2 * Set.setVal.Width];
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        for (int i = 0; i < he; i++)
        {
            ledSturts_Group[i].enabled = true;
            ledSturts_Group[i].Init();
            ledSturts_Group[i].InitNewGameMap_01(4 + i * 4);


        }
        for (int i = 0; i < Tarage_Pos.Length / 2; i++)
        {
            Tarage_Pos[i] = new Vector2(i, 0);
            Tarage_Pos[i + Set.setVal.Width] = new Vector2(i, 1);
        }
        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            if (Tarage_Pos[i].x >= 0)
            {
                picId = (int)Tarage_Pos[i].x + (int)Tarage_Pos[i].y * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                Framebuffer.Update_PointColor((int)Tarage_Pos[i].x, (int)Tarage_Pos[i].y, 255, enPointSta.Target);
                GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
            }


        }
    }



    void Init_NewMap_00()
    {
        Clear();
        isClearing_Game25 = false;
        isClearAll = false;
        tarageCol = Random.Range(0, 4);
        isOver_25 = false;
        MaxJieDuan = 1;
        tarageNum = 20;// SettingInGame_01.instance.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        StayTime = MaxStayTime;


        ledSturts_Group[0].enabled = true;
        ledSturts_Group[1].enabled = true;
        ledSturts_Group[0].Init();
        ledSturts_Group[1].Init();

        ledSturts_Group[0].InitNewGameMap_00();

        ledSturts_Group[1].InitNewGameMap_00();
        haveTwo = false;
        GetRandom_Tarage();
    }


    void Run_NewMap00()
    {
        if (TarageNum_now <= 0)
        {

            isClearAll = true;
        }
        if (isClearAll)
        {
            return;
        }
        if (haveTwo)
        {
            ledSturts_Group[1].Update_NewMap_Shaking();

        }
        ledSturts_Group[0].Update_NewMap_Shaking();

    }
    void Run_NewMap01()
    {
        Get_NowTarage();
        if (TarageNum_now == 0)
        {
            if (isFirst_NewMap01)
            {

                Init_NewMap_01_next();

            }
            else
            {

                isClearAll = true;
            }
        }
        if (isFirst_NewMap01)
        {

            for (int i = 0; i < 2; i++)
            {
                DrawPic.DrawRol(0, i, Set.setVal.Width, 0x00ff00, enPointSta.Rest);
            }
        }
        else
        {

            DrawPic.DrawRol(0, Set.setVal.Height - 1, Set.setVal.Width, 0x00ff00, enPointSta.Rest);
            DrawPic.DrawRol(0, Set.setVal.Height - 2, Set.setVal.Width, 0x00ff00, enPointSta.Rest);

        }
        if (runTime > 0)
        {
            runTime -= Time.deltaTime;
        }
        else
        {
            if (!isFirst_NewMap01)
            {
                for (int i = 0; i < ledSturts_Group.Length; i++)
                {
                    if (ledSturts_Group[i].enabled)
                    {
                        ledSturts_Group[i].Move_NewMap_01();

                    }
                }
            }
            runTime = MaxrunTime;

        }


        for (int i = 2; i < Set.setVal.Height - 2; i++)
        {
            DrawPic.DrawRol(0, i, Set.setVal.Width, 0xff0000, enPointSta.Die);
        }

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            if (ledSturts_Group[i].enabled)
            {
                ledSturts_Group[i].Update_NewMap01();
            }

        }

    }
    void Run_NewMap02()
    {
        if (TarageNum_now <= 1 && isFirst_NewMap01)
        {
            Init_NewMap_02_Next();
        }
        if (isFirst_NewMap01)
        {

            if (FleshTime_Map02 > 0)
            {
                FleshTime_Map02 -= Time.deltaTime;
            }
            else
            {
                FleshTime_Map02 = 10f;
            }
            if (DieCnt_Time > 0)
            {
                DieCnt_Time -= Time.deltaTime;

            }
            else
            {
                if (DieCnt_Map02 < Set.setVal.Height)
                {
                    DieCnt_Map02++;
                    DieCnt_Time = 0.05f;
                }
                else
                {
                    if (runTime > 0)
                    {
                        runTime -= Time.deltaTime;
                    }
                    else
                    {

                        for (int i = 0; i < ledSturts_Group.Length; i++)
                        {
                            if (ledSturts_Group[i].enabled)
                            {
                                ledSturts_Group[i].Move_NewMap_02();

                            }
                        }

                        runTime = MaxrunTime;

                    }



                    for (int i = 0; i < ledSturts_Group.Length; i++)// 
                    {
                        if (ledSturts_Group[i].enabled)
                        {
                            ledSturts_Group[i].Update_NewMap02();
                        }

                    }
                }
            }
            DrawPic.DrawCol(0, 0, DieCnt_Map02, 0xff0000, enPointSta.Die);
            DrawPic.DrawCol(Set.setVal.Width - 1, 0, DieCnt_Map02, 0xff0000, enPointSta.Die);

        }
        else
        {
            ledSturts_Group[0].Update_FirePlane();
        }


    }
    void Run_NewMap03()
    {
        runTime -= Time.deltaTime;
        if (isFirst_NewMap01 && TarageNum_now <= 1)
        {
            Init_NewMap_03_next();
        }
        if (isFirst_NewMap01)
        {
            for (int i = 0; i < 3; i++)
            {
                //if (ledSturts_Group[i].enabled == false || ledSturts_Group[i].waitTime > 0)
                //{
                //    continue;
                //}
                ledSturts_Group[i].UpdatePic_FangKuang_new03();

            }
            if (runTime < 0)
            {
                runTime = MaxrunTime;
                for (int i = 0; i < ledSturts_Group.Length; i++)
                {
                    ledSturts_Group[i].x++;
                    ledSturts_Group[i].y++;
                }
            }
        }
        else
        {
            for (int i = 0; i < Cnt; i++)
            {
                //if (Tarage_Pos[i].x!=0&& (int)Tarage_Pos[i].y!=0)
                {
                    DrawPic.DrawRol((int)Tarage_Pos[i].x, (int)Tarage_Pos[i].y, 1, 0xff0000, enPointSta.Die);

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


        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum() - TarageNum_now;


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
    void Get_NowTarage()
    {


        MaxrunTime = 0.5f - 0.03f * Game00_Main.instance.Index_JieDuan;

        if (Game00_Main.instance.Index_JieDuan >= 0)
        {
            if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
            {
                if (Game00_Main.instance.Index_JieDuan < Game00_Main.instance.player[0].playerUI.Ingame_Setting.set_MoveSpeed.Length)
                {
                    MaxrunTime = 0.35f - 0.05f * SettingInGame_01.instance.GetMoveSpeed(Game00_Main.instance.Index_JieDuan);
                    if (MaxrunTime <= 0.05f)
                    {
                        MaxrunTime = 0.05f;
                    }

                }

            }





        }
        //   tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        TarageNum_now = 0;
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {

                    TarageNum_now++;

                }

            }

        }

        if (TarageNum_now <= 3 && Game00_Main.instance.gameLevel != 15 && Game00_Main.instance.gameLevel != 28 && Game00_Main.instance.gameLevel != 25 && Game00_Main.instance.gameLevel != 6)
        {

            SafeTime -= Time.deltaTime;
            if (SafeTime <= 0)
            {

#if UNITY_EDITOR
                Debug.LogError("没有目标点了");
#endif
                isClearAll = true;
                return;
            }
        }

    }
    int randomX, randomY = 0;
    int yy_Num = 0;
    void GetRandom_Tarage()
    {
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = SettingInGame_01.instance.GetTarageNum();

        }
        YouYu_Leng = 0;
        yy_Num = 0;
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


                    GameLedControl.gamePoint[a].statue = enPointSta.Target;

                    tarageNum--;

                }



            }
        }

    }
    void GetRandom_Tarage_YouYu(int num)
    {
        TouchHit_YouYu = false;
        while (num > 0)
        {
            randomX = Random.Range(2, Set.setVal.Width);
            randomY = Random.Range(2, Set.setVal.Height);
            a = randomX + randomY * Set.setVal.Width;
            a = Framebuffer.tab_Mapping[a];
            if (GameLedControl.gamePoint[a].statue == enPointSta.None)
            {


                GameLedControl.gamePoint[a].statue = enPointSta.Target;
                //if (Game00_Main.instance.gameLevel == 6)
                //{
                //    YouYu_Tarage[YouYu_Leng] = new Vector2(randomX, randomY);
                if (yy_Num < 100)
                {
                    yy_Num++;

                }
                //}
                num--;

            }
        }
        TouchHit_YouYu = true;
    }
    void Get_NewWay()//第二关,河流移动
    {
        step = Random.Range(2, 5);

        a = Random.Range(0, 100);
        if (a <= 20)
        {
            riverWay = en_riverWay.Up;


        }
        else if (a <= 40)
        {
            riverWay = en_riverWay.Down;

        }
        else if (a <= 70)
        {
            riverWay = en_riverWay.Left;
        }
        else
        {
            riverWay = en_riverWay.Right;

        }
        while (Mathf.Abs((int)riverWay) == Mathf.Abs((int)riverWay_Old))
        {
            a = Random.Range(0, 100);
            if (a <= 25)
            {
                riverWay = en_riverWay.Up;
            }
            else if (a <= 50)
            {
                riverWay = en_riverWay.Down;

            }
            else if (a <= 75)
            {
                riverWay = en_riverWay.Left;
            }
            else
            {
                riverWay = en_riverWay.Right;
            }
        }
    }


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
    public void InitMap(int ID)
    {
        Debug.LogError("trueID" + ID);
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            GameLedControl.gamePoint[i].Color = 0;
        }
        remainPoint = 1000;
        Clear();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        switch (ID)
        {
            case 0:
                InitMap_00();
                MaxJieDuan = 1;
                break;
            case 1:
                Init_NewMap_02();
                MaxJieDuan = 1;

                //    Init_NewMap_02_Next();
                //  InitMap_01(Game00_Main.instance.Index_JieDuan);
                //DrawSafePlace(1);
                break;
            case 2:

                InitMap_02();
                MaxJieDuan = 1;

                break;
            case 3:

                InitMap_03();
                MaxJieDuan = 1;

                break;
            case 4:
                Init_NewMap_03();
                MaxJieDuan = 1;

                //  InitMap_04();
                break;
            case 5:
                InitMap_05(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 6:
                InitMap_06_YouYu(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 2;

                break;
            case 7:
                InitMap_07(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 8:
                InitMap_08(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 9:
                InitMap_09();
                MaxJieDuan = 1;

                break;
            case 10:
                InitMap_10();
                MaxJieDuan = 1;

                break;
            case 11:
                InitMap_11();
                MaxJieDuan = 1;

                break;
            case 12:
                InitMap_12();
                MaxJieDuan = 1;

                break;
            case 13:
                InitMap_13();
                MaxJieDuan = 2;

                break;
            case 14:

                InitMap_14();
                MaxJieDuan = 1;

                break;
            case 15:
                InitMap_25();
                MaxJieDuan = 4;

                break;
            case 16:
                InitMap_16(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 17:
                InitMap_17(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 18:
                InitMap_18(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 19:
                InitMap_19(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 20:
                Init_NewMap_00();
                MaxJieDuan = 1;

                //  InitMap_20(Game00_Main.instance.Index_JieDuan);
                break;
            case 21:
                InitMap_21(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 22:
                InitMap_22();
                MaxJieDuan = 1;

                break;
            case 23:
                InitMap_23();
                MaxJieDuan = 1;

                break;
            case 24:
                InitMap_24(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 25:
                Init_NewMap_01();
                MaxJieDuan = 1;

                //InitMap_25(Game00_Main.instance.Index_JieDuan);
                break;
            case 26:
                InitMap_06(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 27:
                InitMap_27(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;

            case 28:
                InitMap_28(Game00_Main.instance.Index_JieDuan);
                MaxJieDuan = 1;

                break;
            case 29:
                InitMap_29();
                MaxJieDuan = 1;

                break;
        }



    }
    void NextRiver_Pos()
    {

        if (TarageNum_now == 0)
        {


            FjData.g_Fj[0].Life = 0;
            Game00_Main.instance.remainTime = 0;
            Game00_Main.instance.result = 0;

        }
        // Debug.Log(riverPos_Now);
        if ((int)riverPos_Now.x + 1 < Set.setVal.Width)
        {
            pointId = Framebuffer.tab_Mapping[(int)riverPos_Now.x + 1 + (int)riverPos_Now.y * Set.setVal.Width];
            if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
            {

                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;

                riverPos_Now = new Vector2(riverPos_Now.x + 1, riverPos_Now.y);
                return;
            }
            if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
            {

                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                tarageNum--;
                riverPos_Now = new Vector2(riverPos_Now.x + 1, riverPos_Now.y);
                return;
            }

        }
        if ((int)riverPos_Now.x - 1 >= 0)
        {
            pointId = Framebuffer.tab_Mapping[(int)riverPos_Now.x - 1 + (int)riverPos_Now.y * Set.setVal.Width];
            if (GameLedControl.gamePoint[pointId].statue == enPointSta.None || GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
            {
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                riverPos_Now = new Vector2(riverPos_Now.x - 1, riverPos_Now.y);
                return;
            }


        }
        if ((int)riverPos_Now.y + 1 < Set.setVal.Height)
        {
            pointId = Framebuffer.tab_Mapping[(int)riverPos_Now.x + ((int)riverPos_Now.y + 1) * Set.setVal.Width];
            if (GameLedControl.gamePoint[pointId].statue == enPointSta.None || GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
            {
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                riverPos_Now = new Vector2(riverPos_Now.x, riverPos_Now.y + 1);
                return;

            }
        }
        if ((int)riverPos_Now.y - 1 >= 0)
        {


            pointId = (int)riverPos_Now.x + ((int)riverPos_Now.y - 1) * Set.setVal.Width;

            pointId = Framebuffer.tab_Mapping[pointId];

            if (GameLedControl.gamePoint[pointId].statue == enPointSta.None || GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
            {
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                riverPos_Now = new Vector2(riverPos_Now.x, riverPos_Now.y - 1);
                return;
            }

        }


        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                pointId = Framebuffer.tab_Mapping[i + k * Set.setVal.Width];

                if (GameLedControl.gamePoint[pointId].statue == enPointSta.None || GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {
                    if (GameLedControl.gamePoint[pointId].errorTime <= 0)
                    {
                        GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                    }

                    riverPos_Now = new Vector2(i, k);
                    return;
                }
            }
        }
        FjData.g_Fj[0].Life = 0;
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
                                    if (dis > r * (r - (Game00_Main.instance.Index_JieDuan + 1)) / r)
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
                            if (GameLedControl.gamePoint[pointId].Color==col[tarageCol])
                            {
                                continue;
                            }
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
                            pointId = Framebuffer.tab_Mapping[picid];
                            if (picid < Set.setVal.Width * Set.setVal.Height) {
                                if (GameLedControl.gamePoint[pointId].Color==col[tarageCol])
                                {
                                    continue;
                                }
                                DrawPic.DrawRol(i, j, 1, 0xFF0000, enPointSta.Die);//

                              //  GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].statue = enPointSta.None;
                            }

                        }

                    }

                }
            }
        }
    }
    void Run_Map01()
    {
        runTime -= Time.deltaTime;
        if (runTime <= 0)
        {

            runTime = MaxrunTime;//和可设置的速度挂钩

            NextRiver_Pos();


        }
    }
    void Run_Map02()
    {
        runTime -= Time.deltaTime;
        if (runTime <= 0)
        {

            runTime = MaxrunTime;//和可设置的速度挂钩
            ClearMap();

            bool go = ledSturts_Group[0].FangKuaiMoving();
            while (!go)
            {
                go = ledSturts_Group[0].FangKuai_ChangeMoving();
            }


        }
    }
    void Run_Map03()
    {

        runTime -= Time.deltaTime;
        ledSturts_Group[0].UpdatePic_Tank(ledSturts_Group[0].StartPos);
        if (ledSturts_Group[0].enabled == false)
        {
            ledSturts_Group[0].enabled = true;
            ledSturts_Group[0].StartPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 2);
        }

        if (Game00_Main.instance.Index_JieDuan > 1)
        {
            ledSturts_Group[1].UpdatePic_Tank(ledSturts_Group[1].StartPos);
            if (ledSturts_Group[1].enabled == false)
            {
                ledSturts_Group[1].enabled = true;
                ledSturts_Group[1].StartPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 2);
            }
        }
        if (runTime <= 0)
        {

            runTime = 0.5f - 0.05f * SettingInGame_01.instance.GetMoveSpeed(Game00_Main.instance.Index_JieDuan);//和可设置的速度挂钩
                                                                                                                //  ClearMap();

            bool go = ledSturts_Group[0].Moving_Tank();


            bool go1 = false;
            while (!go)
            {
                go = ledSturts_Group[0].Tank_ChangeMoving();
            }

            if (Game00_Main.instance.Index_JieDuan > 1)
            {
                go1 = ledSturts_Group[1].Moving_Tank();
                while (!go1)
                {
                    go1 = ledSturts_Group[1].Tank_ChangeMoving();

                }
            }




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
    bool TouchHit_YouYu = false;
    bool GetTT_YouYU = false;
    Vector2 Hit_YouYu_Pos;
    public void ChangeYouYuSta(en_YouYu_sta sta)
    {
        sta_YouYu = sta;
        switch (sta_YouYu)
        {
            case en_YouYu_sta.Idle:
                break;
            case en_YouYu_sta.Playing:
                runTime = 5;
                ClearMap();
                Game00_Main.instance.audioSource_BackG.Stop();
                Game00_Main.instance.audioSource_BackG.Stop();

                sound_Youyu.Stop();
                sound_Youyu.clip = Spr_YouYu;
                sound_Youyu.Play();
                GetRandom_Tarage_YouYu(YouYu_Leng);
                break;

            case en_YouYu_sta.Checking:
                YouYu_Leng = 0;
                for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
                {
                    if (GameLedControl.gamePoint[i].statue == enPointSta.Target)
                    {
                        YouYu_Leng++;
                    }
                }
                ClearMap_Tarage();

                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 0; k < Set.setVal.Height; k++)
                    {
                        picId = i + k * Set.setVal.Width;
                        pointId = Framebuffer.tab_Mapping[picId];
                        if (GameLedControl.gamePoint[pointId].statue != enPointSta.Rest)
                        {
                            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                        }
                    }

                }

                runTime = 3;
                break;

        }
    }
    void Run_Map06_Youyu()
    {


        runTime -= Time.deltaTime;
        for (int i = 0; i < YouYu_SafeLeng; i++)
        {
            DrawPic.DrawRol((int)YouYu_Safe[i].x, (int)YouYu_Safe[i].y, 1, 0x00ff00, enPointSta.Rest);


        }

        switch (sta_YouYu)
        {

            case en_YouYu_sta.Playing:
                Game00_Main.instance.audioSource_BackG.Stop();

                Get_NowTarage();
                if (TarageNum_now <= 0 && TouchHit_YouYu)
                {
                    isClearAll = true;
                    runTime = 0.2f;

                    return;
                }
                if (runTime <= 0)
                {
                    ChangeYouYuSta(en_YouYu_sta.Checking);
                }

                break;

            case en_YouYu_sta.Checking:


                if (runTime <= 0)
                {
                    ChangeYouYuSta(en_YouYu_sta.Playing);
                }
                break;

        }

        //   ClearMap();
    }
    void Run_Map06()
    {

        runTime -= Time.deltaTime;
        if (Set.setVal.Width % 2 == 0)
        {

            for (int i = 0; i < Set.setVal.Height; i++)
            {
                picId = Set.setVal.Width / 2 + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
            }
        }
        else
        {

            for (int i = 0; i < Set.setVal.Height; i++)
            {
                picId = Set.setVal.Width / 2 + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
            }

        }

        ledSturts_Group[0].UpdateTarage_06();
    }
    void Run_Map07()
    {

        runTime -= Time.deltaTime;
        ledSturts_Group[0].UpdatePic_FangKuang(new Vector2(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2), ledSturts_Group[0].x);

        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            ClearMap();
            ledSturts_Group[0].x++;
        }
    }
    void Run_Map08(int _x, int _y, float angle, int radis, int w, int jieduan)
    {


        angle = angle * Mathf.PI / 180;
        runTime -= Time.deltaTime;

        float raiox = Mathf.Cos(angle);
        float raioy = Mathf.Sin(angle);
        float lx, ly;
        float dis;
        switch (jieduan)
        {
            case 0:
                for (int r = 0; r <= radis; r++)
                {
                    raiox = Mathf.Cos(angle);
                    raioy = Mathf.Sin(angle);
                    lx = _x + r * raiox;
                    ly = _y + r * raioy;
                    for (int i = (int)lx; i <= lx + 1; i++)
                    {
                        for (int j = (int)ly; j <= ly + 1; j++)
                        {
                            dis = Vector2.Distance(new Vector2(i, j), new Vector2(lx, ly));
                            if (dis < w)
                            {

                                if (i >= 0 && j >= 0)
                                {
                                    if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                                    {

                                        picId = i + Set.setVal.Width * j;


                                        DrawPic.DrawRol(i, j, 1, 0xff0000, enPointSta.Die);




                                    }

                                }

                            }
                        }
                    }
                }


                break;
            case 1:
                for (int r = -radis; r <= radis; r++)
                {
                    raiox = Mathf.Cos(angle);
                    raioy = Mathf.Sin(angle);
                    lx = _x + r * raiox;
                    ly = _y + r * raioy;
                    for (int i = (int)lx - w; i <= lx + w; i++)
                    {
                        for (int j = (int)ly - w; j <= ly + w; j++)
                        {
                            dis = Vector2.Distance(new Vector2(i, j), new Vector2(lx, ly));
                            if (dis < w)
                            {

                                if (i >= 0 && j >= 0)
                                {
                                    if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                                    {

                                        picId = i + Set.setVal.Width * j;

                                        DrawPic.DrawRol(i, j, 1, 0xff0000, enPointSta.Die);


                                    }

                                }
                            }

                        }
                    }
                }

                break;
            case 2:

                for (int r = -radis; r <= radis; r++)
                {
                    raiox = Mathf.Cos(angle + 90);
                    raioy = Mathf.Sin(angle + 90);
                    lx = _x + r * raiox;
                    ly = _y + r * raioy;
                    for (int i = (int)lx - w; i <= lx + w; i++)
                    {
                        for (int j = (int)ly - w; j <= ly + w; j++)
                        {
                            dis = Vector2.Distance(new Vector2(i, j), new Vector2(lx, ly));
                            if (dis < w)
                            {

                                if (i >= 0 && j >= 0)
                                {
                                    if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                                    {

                                        picId = i + Set.setVal.Width * j;

                                        DrawPic.DrawRol(i, j, 1, 0xff0000, enPointSta.Die);


                                    }

                                }
                            }
                        }
                    }


                }


                for (int r = -radis; r <= radis; r++)
                {
                    raiox = Mathf.Cos(angle);
                    raioy = Mathf.Sin(angle);
                    lx = _x + r * raiox;
                    ly = _y + r * raioy;

                    for (int i = (int)lx - w; i <= lx + w; i++)
                    {
                        for (int j = (int)ly - w; j <= ly + w; j++)
                        {
                            dis = Vector2.Distance(new Vector2(i, j), new Vector2(lx, ly));
                            if (dis < w)
                            {

                                if (i >= 0 && j >= 0)
                                {
                                    if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height - 1)
                                    {

                                        picId = i + Set.setVal.Width * j;

                                        DrawPic.DrawRol(i, j, 1, 0xff0000, enPointSta.Die);


                                    }

                                }

                            }
                        }
                    }
                }




                break;





        }
        if (runTime <= 0)
        {
            ClearMap();
            runTime = MaxrunTime;


        }



    }
    void Run_Map09(int JieDuan)
    {
        runTime -= Time.deltaTime;
        //     ledSturts_Group[0].UpdatePic_FangKuang(new Vector2(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2), ledSturts_Group[0].x);
        ledSturts_Group[0].UpdateSnake();
        if (runTime <= 0)
        {
            ClearMap();

            runTime = MaxrunTime;
            ledSturts_Group[0].MoveSnakePos();
            snakeLength = 5 + JieDuan;


        }
    }
    void Run_Map10(int JieDuan)
    {

        runTime -= Time.deltaTime;
        //     ledSturts_Group[0].UpdatePic_FangKuang(new Vector2(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2), ledSturts_Group[0].x);

        if (runTime <= 0)
        {
            //    ClearMap();

            runTime = MaxrunTime;
            for (int i = 0; i < 3; i++)
            {
                if (ledSturts_Group[i].dir == 0)
                {
                    if (ledSturts_Group[i].x >= Set.setVal.Width - 1)
                    {
                        ledSturts_Group[i].x = Set.setVal.Width - 1;
                        ledSturts_Group[i].dir = 1;
                    }
                    else
                    {
                        ledSturts_Group[i].x++;
                    }




                }
                else
                {
                    if (ledSturts_Group[i].x <= 0)
                    {
                        ledSturts_Group[i].x = 0;
                        ledSturts_Group[i].dir = 0;
                    }
                    else
                    {
                        ledSturts_Group[i].x--;
                    }



                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            DrawPic.DrawCol(ledSturts_Group[i].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

        }
    }

    void Run_Map11(int JieDuan)
    {
        runTime -= Time.deltaTime;
        //     ledSturts_Group[0].UpdatePic_FangKuang(new Vector2(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2), ledSturts_Group[0].x);

        if (runTime <= 0)
        {
            Cnt--;

            if (Cnt <= 0)
            {
                int xx = Random.Range(0, 4);

                if (ledSturts_Group[0].dir == xx)
                {

                    while (xx == ledSturts_Group[0].dir)
                    {
                        xx = Random.Range(0, 4);

                    }

                    ledSturts_Group[0].dir = xx;
                    Cnt = 5;
                }
            }
            runTime = MaxrunTime;



        }
        switch (ledSturts_Group[0].dir)
        {
            case 0:
                for (int i = 1; i < Set.setVal.Width / 2; i++)
                {
                    DrawPic.DrawCol(i, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

                }

                break;
            case 1:
                for (int i = Set.setVal.Width / 2; i < Set.setVal.Width; i++)
                {
                    DrawPic.DrawCol(i, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

                }

                break;
            case 2:
                for (int i = 1; i < Set.setVal.Width; i++)
                {
                    DrawPic.DrawCol(i, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

                }
                break;
        }

    }
    void Run_Map12(int JieDuan)
    {

        runTime -= Time.deltaTime;
        //     ledSturts_Group[0].UpdatePic_FangKuang(new Vector2(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2), ledSturts_Group[0].x);
        for (int i = 3; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;



            }

            picId = i + ((Set.setVal.Height - 1) * Set.setVal.Width);
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;


            picId = i + ((Set.setVal.Height - 2) * Set.setVal.Width);
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
        }

        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;


            }
        }
        if (runTime <= 0)
        {
            //   ClearMap();

            runTime = MaxrunTime;



        }
        for (int i = 0; i < 4; i++)
        {
            ledSturts_Group[i].attackCD -= Time.deltaTime;
            if (ledSturts_Group[i].attackCD <= 0)
            {
                ledSturts_Group[i].attackMaxCD = 0.3f;
                ledSturts_Group[i].attackCD = ledSturts_Group[i].attackMaxCD;
                ledSturts_Group[i].Moving_new12();

            }
        }
        for (int i = 0; i < 4; i++)
        {
            DrawPic.DrawCol(ledSturts_Group[i].x, ledSturts_Group[i].y, 1, 0xff0000, enPointSta.Die);
        }
    }
    void Run_Map13(int JieDuan)
    {
        runTime -= Time.deltaTime;
        ClearMap();
        ledSturts_Group[0].UpdatePic_FangKuai_13();
        if (JieDuan >= 1)
        {
            ledSturts_Group[1].UpdatePic_FangKuai_13();
            DrawPic.DrawRol(x, y, Set.setVal.Width - Game_Fangkuai_length * 2 - 1, 0xff0000, enPointSta.Die);
        }
        for (int i = 0; i < Cnt; i++)
        {
            //    Framebuffer.Update_PointColor((int)Tarage_Pos[i].x, (int)Tarage_Pos[i].y, 0xff0000, enPointSta.Die);
        }
        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            if (JieDuan == 0)
            {
                ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);

            }
            else
            {
                if (dir == 0)
                {
                    if (y < Set.setVal.Height - Game_Fangkuai_length - 1)
                    {
                        y++;
                    }
                    else
                    {
                        dir = 1;
                    }
                }
                else
                {
                    if (y > Game_Fangkuai_length + 1)
                    {
                        y--;
                    }
                    else
                    {
                        dir = 0;
                    }
                }

                ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);
                ledSturts_Group[1].Move_FangKuai_13(Game_Fangkuai_length);
            }

        }

    }
    float DieTime_14 = 1;

    bool All_ReadyGo_14()
    {
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            if (ledSturts_Group[i].canGo_14)
            {
                return false;
            }
          
        }
        return true;
    }

    bool All_GoDown_14()
    {
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            if (!ledSturts_Group[i].GoDown&& ledSturts_Group[i].enabled)
            {
                return false;
            }
          
        }
        return true;
    }
    void Run_Map14()
    {
    
        runTime -= Time.deltaTime;
        for (int i = 0; i < Set.setVal.Height; i++)
        {
         //   DrawPic.DrawRol(4, i, Set.setVal.Width, 0xff0000, enPointSta.Die);

        }

        for (int i = 0; i < Tarage_Pos.Length; i++)
        {

            picId = (int)Tarage_Pos[i].x + (int)Tarage_Pos[i].y * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];

            {
         //       DrawPic.DrawRol((int)Tarage_Pos[i].x, (int)Tarage_Pos[i].y, 1, 0xff0000, enPointSta.Die);

            }

        }
        if (All_ReadyGo_14())
        {
            DieTime_14 -= Time.deltaTime;
            if (DieTime_14 < 0)
            {
                for (int i = 0; i < ledSturts_Group.Length; i++)
                {
                    ledSturts_Group[i].canGo_14 = true;
                }
                DieTime_14 = Random.Range(1, 3) ;
            }
            else
            {
               
            }

        }
        if (All_GoDown_14())
        {
 
            for (int i = 0; i < 4; i++)
            {
                DrawPic.DrawCol(i, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
            }

        }
        if (runTime < 0)
        {
            runTime = MaxrunTime;
          
            for (int i = 0; i < ledSturts_Group.Length; i++)
            {
                ledSturts_Group[i].Move_New14();
            }

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            if (ledSturts_Group[i].y != 0)
            {
                ledSturts_Group[i].Update_NewMap14();

            }
        }
    }
    void Run_Map14_Old(int JieDuan)
    {
        runTime -= Time.deltaTime;


        if (JieDuan < 3)
        {
            switch (JieDuan)
            {
                case 0:

                    ledSturts_Group[0].UpdatePic_DuanLie();
                    break;
                case 1:

                    ledSturts_Group[1].UpdatePic_DuanLie();
                    break;
                case 2:

                    ledSturts_Group[0].UpdatePic_DuanLie();
                    ledSturts_Group[1].UpdatePic_DuanLie();
                    break;


            }
        }
        else
        {

            ledSturts_Group[0].UpdatePic_DuanLie_Full();
            ledSturts_Group[1].UpdatePic_DuanLie_Full();
        }
        DrawSafePlace(1);

        if (runTime <= 0)
        {
            ClearMap();

            runTime = MaxrunTime;




            ledSturts_Group[0].y++;
            if (ledSturts_Group[0].y >= Set.setVal.Height)
            {
                ledSturts_Group[0].y = 0;

            }

            ledSturts_Group[1].x++;
            if (ledSturts_Group[1].x >= Set.setVal.Width)
            {
                ledSturts_Group[1].x = 0;

            }

        }

    }
    void Run_Map15(int JieDuan)
    {
        runTime -= Time.deltaTime;
        ClearMap();
        switch (JieDuan)
        {

            case 0:
                DrawPic.DrawCol(ledSturts_Group[0].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                break;
            case 1:

                DrawPic.DrawCol(ledSturts_Group[0].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[0].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                break;

            case 2:
                ledSturts_Group[0].UpdatePic_FangKuai_13();

                DrawPic.DrawCol(ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                break;
            case 3:
                DrawPic.DrawCol(ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

                for (int k = 0; k < Set.setVal.Height; k++)
                {
                    ChangeLed_Sta(Set.setVal.Width - 1, k, enPointSta.Die);
                    ChangeLed_Sta(0, k, enPointSta.Die);
                }

                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    ChangeLed_Sta(i, Set.setVal.Height - 1, enPointSta.Die);
                    ChangeLed_Sta(i, 3, enPointSta.Die);

                }

                ledSturts_Group[0].UpdatePic_FangKuai_13();
                break;
            case 4:
                //DrawPic.DrawCol(ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                //DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

                //for (int k = 0; k < Set.setVal.Height; k++)
                //{
                //    ChangeLed_Sta(Set.setVal.Width - 1, k, enPointSta.Die);
                //    ChangeLed_Sta(0, k, enPointSta.Die);
                //}

                //for (int i = 0; i < Set.setVal.Width; i++)
                //{
                //    ChangeLed_Sta(i, Set.setVal.Height - 1, enPointSta.Die);
                //    ChangeLed_Sta(i, 3, enPointSta.Die);

                //}

                //  ledSturts_Group[0].UpdatePic_FangKuai_15();
                //     ledSturts_Group[1].UpdatePic_FangKuai_15();
                break;
        }
        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            switch (JieDuan)
            {
                default:
                case 1:

                    if (ledSturts_Group[0].dir == 0)
                    {
                        if (ledSturts_Group[0].x < Set.setVal.Width - 1)
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
                            ledSturts_Group[0].x--;
                        }
                        else
                        {
                            ledSturts_Group[0].dir = 0;
                        }

                    }
                    break;

                case 2:
                case 3:

                    if (ledSturts_Group[1].dir == 0)
                    {
                        if (ledSturts_Group[1].x < Set.setVal.Width - 1)
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
                    if (ledSturts_Group[0].dir == 0)
                    {
                        if (ledSturts_Group[0].x + Game_Fangkuai_length < Set.setVal.Width)
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
                            ledSturts_Group[0].x--;
                        }
                        else
                        {
                            ledSturts_Group[0].dir = 0;
                        }

                    }
                    break;
                case 4:
                    if (ledSturts_Group[1].dir == 0)
                    {
                        if (ledSturts_Group[1].x < Set.setVal.Width - 1)
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
                    for (int i = 0; i < 2; i++)
                    {
                        if (ledSturts_Group[i].dir == 0)
                        {
                            if (ledSturts_Group[i].x + Game_Fangkuai_length < Set.setVal.Width)
                            {
                                ledSturts_Group[i].x++;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 1;
                            }


                        }
                        else
                        {
                            if (ledSturts_Group[i].x > 0)
                            {
                                ledSturts_Group[i].x--;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 0;
                            }

                        }
                    }

                    break;
            }


        }

    }
    void Run_Map16(int JieDuan)
    {
        runTime -= Time.deltaTime;
        ClearMap();

        DrawPic.DrawCol(ledSturts_Group[0].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
        DrawPic.DrawCol(ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(0, ledSturts_Group[2].y, Set.setVal.Width, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(0, ledSturts_Group[3].y, Set.setVal.Width, 0xff0000, enPointSta.Die);

        if (runTime <= 0)
        {

            runTime = MaxrunTime;

            switch (JieDuan)
            {
                default:

                    if (ledSturts_Group[0].dir == 0)
                    {
                        if (ledSturts_Group[0].x < Set.setVal.Width - 1)
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
                            ledSturts_Group[0].x--;
                        }
                        else
                        {
                            ledSturts_Group[0].dir = 0;
                        }
                    }

                    break;
                case 1:

                    for (int i = 0; i < 2; i++)
                    {
                        if (ledSturts_Group[i].dir == 0)
                        {
                            if (ledSturts_Group[i].x < Set.setVal.Width - 1)
                            {
                                ledSturts_Group[i].x++;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 1;
                            }

                        }
                        else
                        {
                            if (ledSturts_Group[i].x > 0)
                            {
                                ledSturts_Group[i].x--;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 0;
                            }
                        }
                    }
                    break;
                case 2:

                    for (int i = 0; i < 2; i++)
                    {
                        if (ledSturts_Group[i].dir == 0)
                        {
                            if (ledSturts_Group[i].x < Set.setVal.Width - 1)
                            {
                                ledSturts_Group[i].x++;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 1;
                            }

                        }
                        else
                        {
                            if (ledSturts_Group[i].x > 0)
                            {
                                ledSturts_Group[i].x--;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 0;
                            }
                        }
                    }

                    if (ledSturts_Group[2].dir == 0)
                    {
                        if (ledSturts_Group[2].y < Set.setVal.Height - 1)
                        {
                            ledSturts_Group[2].y++;
                        }
                        else
                        {
                            ledSturts_Group[2].dir = 1;
                        }

                    }
                    else
                    {
                        if (ledSturts_Group[2].y > 0)
                        {
                            ledSturts_Group[2].y--;
                        }
                        else
                        {
                            ledSturts_Group[2].dir = 0;
                        }
                    }
                    break;
                case 3:

                    for (int i = 0; i < 2; i++)
                    {
                        if (ledSturts_Group[i].dir == 0)
                        {
                            if (ledSturts_Group[i].x < Set.setVal.Width - 1)
                            {
                                ledSturts_Group[i].x++;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 1;
                            }

                        }
                        else
                        {
                            if (ledSturts_Group[i].x > 0)
                            {
                                ledSturts_Group[i].x--;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 0;
                            }
                        }
                    }
                    for (int i = 2; i < 4; i++)
                    {
                        if (ledSturts_Group[i].dir == 0)
                        {
                            if (ledSturts_Group[i].y < Set.setVal.Height - 1)
                            {
                                ledSturts_Group[i].y++;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 1;
                            }

                        }
                        else
                        {
                            if (ledSturts_Group[i].y > 0)
                            {
                                ledSturts_Group[i].y--;
                            }
                            else
                            {
                                ledSturts_Group[i].dir = 0;
                            }
                        }
                    }
                    break;


            }



        }

    }
    void Run_Map17(int JieDuan)
    {
        runTime -= Time.deltaTime;
        ClearMap();
        ledSturts_Group[0].UpdateSnake_17_RightKLong();
        ledSturts_Group[1].UpdateSnake_17_RightKLong();
        ledSturts_Group[2].UpdateSnake_17_RightKLong();
        if (JieDuan == 4)
        {
            for (int i = 0; i < 2; i++)
            {

                picId = -1 + i + Set.setVal.Width / 2 + Set.setVal.Height / 2 * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].errorTime <= 0)
                {
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                }
                picId = -1 + i + Set.setVal.Width / 2 + (Set.setVal.Height / 2 + 1) * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];

                if (GameLedControl.gamePoint[pointId].errorTime <= 0)
                {
                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                }
            }
        }

        if (runTime <= 0)
        {
            runTime = MaxrunTime;

            switch (JieDuan)
            {
                case 0:
                    ledSturts_Group[0].MoveSnakePos_17(0);

                    break;
                case 1:
                    ledSturts_Group[0].MoveSnakePos_17(0);
                    ledSturts_Group[1].MoveSnakePos_17(0);

                    break;
                case 2:
                case 3:
                    ledSturts_Group[0].MoveSnakePos_17(0);
                    ledSturts_Group[1].MoveSnakePos_17(1);
                    ledSturts_Group[2].MoveSnakePos_17(2);
                    break;

                case 4:
                    ledSturts_Group[0].MoveSnakePos_17(0);
                    ledSturts_Group[1].MoveSnakePos_17(1);
                    ledSturts_Group[2].MoveSnakePos_17(2);

                    break;

            }





        }

    }
    void Run_Map18(int JieDuan)
    {

        runTime -= Time.deltaTime;
        for (int i = 0; i < Tarage_Pos.Length; i++)
        {
            Framebuffer.Update_PointColor((int)Tarage_Pos[i].x, (int)Tarage_Pos[i].y, 0xff0000, enPointSta.Die);
        }


        if (runTime <= 0)
        {

            runTime = 0.2f;// MaxrunTime;
            if (waitime < 0)
            {

                if (ledSturts_Group[0].y < Set.setVal.Height)
                {
                    ledSturts_Group[0].y++;

                }
                else
                {
                    if (ledSturts_Group[0].x < Set.setVal.Width)
                    {
                        ledSturts_Group[0].x++;

                    }
                    else
                    {
                        if (ledSturts_Group[1].y > 0)
                        {
                            ledSturts_Group[1].y--;

                        }
                        else
                        {
                            if (ledSturts_Group[1].x > 0)
                            {
                                ledSturts_Group[1].x--;

                            }
                            else
                            {
                                if (ledSturts_Group[2].y < Set.setVal.Height)
                                {
                                    ledSturts_Group[2].y++;

                                }
                                else
                                {
                                    if (ledSturts_Group[2].x < Set.setVal.Width * 3 / 4)
                                    {
                                        ledSturts_Group[2].x++;

                                    }
                                    else
                                    {
                                        if (ledSturts_Group[3].y > 0)
                                        {
                                            ledSturts_Group[3].y--;

                                        }
                                        else
                                        {
                                            waitime = 5;
                                            ledSturts_Group[0].x = ledSturts_Group[0].y = 0;
                                            ledSturts_Group[2].x = ledSturts_Group[2].y = 0;
                                            ledSturts_Group[1].x = Set.setVal.Width;
                                            ledSturts_Group[1].y = Set.setVal.Height;
                                            ledSturts_Group[3].y = Set.setVal.Height;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

        }


        if (waitime > 0)
        {
            waitime -= Time.deltaTime;
        }
        else
        {

            if (TarageNum_now == 0)
            {
                isClearAll = true;
            }
            for (int i = 0; i < Set.setVal.Width / 4; i++)
            {
                DrawPic.DrawCol(i, 0, (int)ledSturts_Group[0].y, 0xff0000, enPointSta.Die);

            }
            for (int i = New_18_Height; i < Set.setVal.Height; i++)
            {
                //if (ledSturts_Group[0].x>Set.setVal.Width-1)
                //{
                //    ledSturts_Group[0].x = Set.setVal.Width - 1;
                //}
                DrawPic.DrawRol(0, i, (int)ledSturts_Group[0].x, 0xff0000, enPointSta.Die);

            }
            for (int i = Set.setVal.Width * 3 / 4 + 1; i < Set.setVal.Width; i++)
            {

                for (int k = Set.setVal.Height; k > ledSturts_Group[1].y; k--)
                {
                    DrawPic.DrawCol(i, k, 1, 0xff0000, enPointSta.Die);//Set.setVal.Height -k

                }
            }


            for (int k = Set.setVal.Height * 3 / 10; k > 0; k--)
            {
                for (int i = Set.setVal.Width; i > ledSturts_Group[1].x; i--)
                {

                    DrawPic.DrawRol(i, k, 1, 0xff0000, enPointSta.Die);//Set.setVal.Height -k

                }
            }
            //
            for (int i = Set.setVal.Width / 4; i < Set.setVal.Width / 2; i++)
            {
                DrawPic.DrawCol(i, 0, (int)ledSturts_Group[2].y, 0xff0000, enPointSta.Die);

            }
            for (int i = New_18_Height1; i < Set.setVal.Height; i++)
            {
                //if (ledSturts_Group[0].x>Set.setVal.Width-1)
                //{
                //    ledSturts_Group[0].x = Set.setVal.Width - 1;
                //}
                DrawPic.DrawRol(0, i, (int)ledSturts_Group[2].x, 0xff0000, enPointSta.Die);

            }

            //
            for (int i = Set.setVal.Width / 2; i < Set.setVal.Width * 3 / 4; i++)
            {

                for (int k = Set.setVal.Height; k > ledSturts_Group[3].y; k--)
                {
                    DrawPic.DrawCol(i, k, 1, 0xff0000, enPointSta.Die);//Set.setVal.Height -k

                }
            }

        }


    }

    void Run_Map19(int JieDuan)
    {
        runTime -= Time.deltaTime;
        for (int i = 0; i < 3; i++)
        {
            ledSturts_Group[i].Update_Game19_M(JieDuan);
        }
        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            for (int i = 0; i < 3; i++)
            {
                ledSturts_Group[i].Moving_Game19_M();
            }


        }

    }

    void Run_Map20(int JieDuan)
    {

        runTime -= Time.deltaTime;
        switch (JieDuan)
        {
            default:
                DrawPic.DrawRol(Set.setVal.Width / 2, Set.setVal.Height / 2, 2, 0xff0000, enPointSta.Die);
                DrawPic.DrawRol(Set.setVal.Width / 2, Set.setVal.Height / 2 + 1, 2, 0xff0000, enPointSta.Die);
                break;
            case 1:
                DrawPic.DrawCol(Set.setVal.Width / 2, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                break;
            case 2:
                DrawPic.DrawCol(Set.setVal.Width / 2, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width / 2 + 1, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                break;
            case 3:
                DrawPic.DrawCol(Set.setVal.Width * 3 / 4, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width / 4, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                break;
            case 4:
                DrawPic.DrawCol(Set.setVal.Width * 3 / 4, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width / 4, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width / 2, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                break;
        }

        DrawPic.DrawRol(0, 0, Set.setVal.Width, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(0, 1, Set.setVal.Width, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(0, Set.setVal.Height - 1, Set.setVal.Width, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(0, Set.setVal.Height - 2, Set.setVal.Width, 0xff0000, enPointSta.Die);


        DrawPic.DrawCol(0, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
        DrawPic.DrawCol(Set.setVal.Width - 1, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

        DrawPic.DrawRol(Set.setVal.Width / 4, 0, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width / 4, 1, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width / 4 + 1, 0, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width / 4 + 1, 1, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4, 0, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4, 1, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4 + 1, 0, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4 + 1, 1, 1, 0x00ff00, enPointSta.Rest);


        DrawPic.DrawRol(Set.setVal.Width / 4, Set.setVal.Height - 1, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width / 4, Set.setVal.Height - 2, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width / 4 + 1, Set.setVal.Height - 1, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width / 4 + 1, Set.setVal.Height - 2, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4, Set.setVal.Height - 1, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4, Set.setVal.Height - 2, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4 + 1, Set.setVal.Height - 1, 1, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(Set.setVal.Width * 3 / 4 + 1, Set.setVal.Height - 2, 1, 0x00ff00, enPointSta.Rest);


        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            ClearMap_Tarage();
            for (int i = 0; i < 2; i++)
            {
                ledSturts_Group[i].UpdateFangKuai_X(JieDuan);
                ledSturts_Group[i].MoveFangKuai_X();

            }

            if (TarageNum_now <= 0)
            {

                isClearAll = true;

#if UNITY_EDITOR
                Debug.LogError("没有目标点了" + isClearAll);
#endif
                return;
            }

        }


    }
    int r, r1 = 0;//第21关，圆形扩散
    void Run_Map21(int JieDuan)
    {

        //ClearMap();
        runTime -= Time.deltaTime;

        // Run_MapYuan(startPos1, r1);
        ledSturts_Group[0].Update_FaBo();
        //    Run_MapYuan(startPos, r);
        if (runTime < 0)
        {
            runTime = runTime = MaxrunTime;
            ledSturts_Group[0].Move_FaBo();
            //r++;
            //if (JieDuan > 1)
            //{
            //    r1++;
            //}
            //if (Set.setVal.Width > Set.setVal.Height)
            //{

            //    if (r > Set.setVal.Width - 1)
            //    {
            //        r = 0;

            //    }
            //    if (r1 > Set.setVal.Width - 1)
            //    {
            //        r1 = 0;

            //    }
            //}
            //else
            //{
            //    if (r > Set.setVal.Height - 1)
            //    {
            //        r = 0;

            //    }
            //    if (r1 > Set.setVal.Height - 1)
            //    {
            //        r1 = 0;

            //    }
            //}
        }


    }
    int dir = 0;
    void Run_Map22()
    {

        runTime -= Time.deltaTime;

        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            for (int i = 0; i < ledSturts_Group.Length; i++)
            {
                ledSturts_Group[i].Move_New22();

            }
            if (dir == 0)
            {
                if (y < Set.setVal.Height)
                {
                    y++;
                }
                else
                {
                    dir = 1;
                    Cnt++;
                }
            }
            else
            {
                if (y > 0)
                {
                    y--;
                }
                else
                {
                    dir = 0;
                }
            }


        }

        DrawPic.DrawRol(0, y, Set.setVal.Width, 0xff0000, enPointSta.Die);
        // DrawPic.DrawRol(0, ledSturts_Group[1].y, Set.setVal.Width, 0xff0000, enPointSta.Die);
        if (Set.setVal.Width < 16)
        {
            for (int i = 0; i < width_22; i++)
            {
                ledSturts_Group[i].UpdatePic_FangKuai_Long(width_22);

            }
        }
        else
        {
            for (int i = 0; i < ledSturts_Group.Length; i++)
            {
                ledSturts_Group[i].UpdatePic_FangKuai_Long(width_22);

            }
        }

        DrawPic.DrawRol(0, 0, Set.setVal.Width, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(0, 1, Set.setVal.Width, 0x00ff00, enPointSta.Rest);

    }
    void Run_Map22_Old(int JieDuan)
    {
        runTime -= Time.deltaTime;
        switch (JieDuan)
        {
            case 0:
                ledSturts_Group[0].UpdatePic_22();


                break;
            case 1:
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    ledSturts_Group[i].UpdatePic_22();
                }

                break;

            case 3:
            case 4:
            case 5:
                for (int i = 0; i < 3; i++)
                {
                    ledSturts_Group[i].UpdatePic_22();
                }

                break;
        }
        //ClearMap();
        if (runTime < 0)
        {
            runTime = MaxrunTime;
            switch (JieDuan)
            {
                case 0:
                    ledSturts_Group[0].Moving_22(JieDuan);


                    break;
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        ledSturts_Group[i].Moving_22(JieDuan);
                    }

                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    for (int i = 0; i < 3; i++)
                    {
                        ledSturts_Group[i].Moving_22(JieDuan);
                    }

                    break;
            }
        }


    }
    void Run_Map23()
    {
        if (runTime < 0)
        {
            runTime = MaxrunTime;
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 2; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.None)
                {
                    if (Cnt_ShakeMap_23[pointId] > 0)
                    {

                        if (GoDown_ShakeMap_23[pointId])
                        {
                            Color_ShakeMap_23[pointId] -= 0x0f0000;
                            if (Color_ShakeMap_23[pointId] <= 0x0f0000)
                            {
                                Color_ShakeMap_23[pointId] = 0x0f0000;
                                GoDown_ShakeMap_23[pointId] = false;
                            }
                        }
                        else
                        {
                            Color_ShakeMap_23[pointId] += 0x0f0000;
                            if (Color_ShakeMap_23[pointId] >= 0xff0000)
                            {
                                Color_ShakeMap_23[pointId] = 0xff0000;
                                GoDown_ShakeMap_23[pointId] = true;
                                Cnt_ShakeMap_23[pointId]--;
                            }
                        }

                    }
                    if (Cnt_ShakeMap_23[pointId] <= 0)
                    {

                        Framebuffer.Update_PointColor(pointId, 0xff0000, enPointSta.Die);
                    }
                    else
                    {
                        Framebuffer.Update_PointColor(pointId, Color_ShakeMap_23[pointId], enPointSta.Rest);


                    }
                    //  Framebuffer.Update_PointColor(i, k, 0xff0000, enPointSta.Die);
                }
            }
        }
        DrawPic.DrawRol(0, 1, Set.setVal.Width, 0x00ff00, enPointSta.Rest);
        DrawPic.DrawRol(0, 0, Set.setVal.Width, 0x00ff00, enPointSta.Rest);

    }
    void Run_Map23_Old(int JieDuan)
    {
        runTime -= Time.deltaTime;

        switch (JieDuan)
        {
            case 0:
                DrawPic.DrawRol(0, Set.setVal.Height / 3, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);
                DrawPic.DrawRol(Set.setVal.Width / 2, Set.setVal.Height * 2 / 3, Set.setVal.Width / 2, 0xff0000, enPointSta.Die);
                DrawPic.DrawRol(Set.setVal.Width / 2, Set.setVal.Height * 2 / 3, Set.setVal.Width / 2 - ledSturts_Group[0].x, 0x00, enPointSta.None);


                break;
            case 1:


                if (Set.setVal.Width > Set.setVal.Height)
                {
                    for (int k = Set.setVal.Width / 6; k < Set.setVal.Width / 3; k++)
                    {
                        DrawPic.DrawRol(0, k, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);

                        DrawPic.DrawRol(Set.setVal.Width / 2, k + Set.setVal.Width / 6, Set.setVal.Width / 2, 0xff0000, enPointSta.Die);
                        DrawPic.DrawRol(Set.setVal.Width / 2, k + Set.setVal.Width / 6, Set.setVal.Width / 2 - ledSturts_Group[0].x, 0, enPointSta.None);

                    }
                }
                else
                {
                    for (int k = Set.setVal.Height / 6; k < Set.setVal.Height / 3; k++)
                    {
                        DrawPic.DrawRol(0, k, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);
                        DrawPic.DrawRol(0, k + Set.setVal.Height / 6, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);

                        DrawPic.DrawRol(Set.setVal.Width / 2, k + Set.setVal.Height / 3, Set.setVal.Width / 2, 0xff0000, enPointSta.Die);
                        DrawPic.DrawRol(Set.setVal.Width / 2, k + Set.setVal.Height / 3, Set.setVal.Width / 2 - ledSturts_Group[0].x, 0, enPointSta.None);
                        DrawPic.DrawRol(Set.setVal.Width / 2, k + Set.setVal.Height / 3 + Set.setVal.Height / 6, Set.setVal.Width / 2, 0xff0000, enPointSta.Die);
                        DrawPic.DrawRol(Set.setVal.Width / 2, k + Set.setVal.Height / 3 + Set.setVal.Height / 6, Set.setVal.Width / 2 - ledSturts_Group[0].x, 0, enPointSta.None);

                    }
                }


                break;
            case 2:

                if (Set.setVal.Width > Set.setVal.Height)
                {
                    for (int k = Set.setVal.Width / 6; k < Set.setVal.Width / 3; k++)
                    {
                        DrawPic.DrawRol(0, k, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);
                        DrawPic.DrawCol(ledSturts_Group[0].x, k, k, 0xff0000, enPointSta.Die);
                        DrawPic.DrawCol(ledSturts_Group[0].x - 1, k, k, 0xff0000, enPointSta.Die);

                        DrawPic.DrawRol(Set.setVal.Width - ledSturts_Group[0].x, k + Set.setVal.Width / 6, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);

                        // DrawPic.DrawRol(Set.setVal.Width / 2, k + Set.setVal.Width / 6, Set.setVal.Width / 2 - ledSturts_Group[0].x, 0xff0000, enPointSta.None);


                        DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[0].x, 0, Set.setVal.Height - k, 0xff0000, enPointSta.Die);
                        if (ledSturts_Group[0].x < Set.setVal.Width)
                        {
                            DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[0].x + 1, 0, Set.setVal.Height - k, 0xff0000, enPointSta.Die);

                        }
                    }
                }
                else
                {
                    for (int k = Set.setVal.Height / 6; k < Set.setVal.Height / 3; k++)
                    {
                        DrawPic.DrawRol(0, Set.setVal.Width / 2, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);
                        DrawPic.DrawRol(0, Set.setVal.Width / 2 + 1, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);
                        DrawPic.DrawCol(ledSturts_Group[0].x, k, k, 0xff0000, enPointSta.Die);
                        DrawPic.DrawCol(ledSturts_Group[0].x - 1, k, k, 0xff0000, enPointSta.Die);


                        DrawPic.DrawRol(Set.setVal.Width - ledSturts_Group[0].x, Set.setVal.Height / 2, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);

                        DrawPic.DrawRol(Set.setVal.Width / 2, Set.setVal.Height / 2, Set.setVal.Width / 2 - ledSturts_Group[0].x, 0xff0000, enPointSta.None);


                        if (Set.setVal.Width - ledSturts_Group[0].x > Set.setVal.Width / 2)
                        {
                            DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[0].x, Set.setVal.Height / 2, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);

                            DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[0].x + 1, Set.setVal.Height / 2, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);

                        }
                    }
                }

                break;
            case 3:
                for (int k = Set.setVal.Width / 6; k < Set.setVal.Width / 3; k++)
                {
                    DrawPic.DrawRol(0, k, ledSturts_Group[0].x, 0xff0000, enPointSta.Die);
                    DrawPic.DrawCol(ledSturts_Group[0].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                    if (ledSturts_Group[0].x > 0)
                    {
                        DrawPic.DrawCol(ledSturts_Group[0].x - 1, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

                    }

                    DrawPic.DrawRol(Set.setVal.Width / 2, k, Set.setVal.Width / 2, 0xff0000, enPointSta.Die);
                    DrawPic.DrawRol(Set.setVal.Width / 2, k, Set.setVal.Width / 2 - ledSturts_Group[0].x, 0xff0000, enPointSta.None);

                    DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[0].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                    DrawPic.DrawCol(Set.setVal.Width - ledSturts_Group[0].x + 1, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);


                }
                break;

        }

        //ClearMap();
        if (runTime < 0)
        {
            runTime = MaxrunTime;
            if (ledSturts_Group[0].dir == 0)
            {
                if (ledSturts_Group[0].x < Set.setVal.Width / 2)
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
                    ledSturts_Group[0].x--;
                }
                else
                {

                    ledSturts_Group[0].dir = 0;

                }
            }
        }


    }
    void Run_Map24(int JieDuan)
    {
        runTime -= Time.deltaTime;
        ledSturts_Group[0].UpdatePic_FangKuai_24(JieDuan);
        if (runTime < 0)
        {
            runTime = MaxrunTime + 0.5f;
            for (int i = 0; i < ledSturts_Group[0].fangkuai_24_Pos.Length; i++)
            {
                if (ledSturts_Group[0].fangkuai_24_Pos[i].x < 0)
                {
                    switch (JieDuan)
                    {
                        case 0:
                            ledSturts_Group[0].fangkuai_24_Pos[i].x = (Random.Range(0, Set.setVal.Width - 4 / 4) * 4);
                            ledSturts_Group[0].fangkuai_24_Pos[i].y = Set.setVal.Height - 1;
                            break;
                        case 1:
                            ledSturts_Group[0].fangkuai_24_Pos[i].x = (Random.Range(0, Set.setVal.Width - 1));
                            ledSturts_Group[0].fangkuai_24_Pos[i].y = Set.setVal.Height - 1;
                            break;
                        case 2:
                            ledSturts_Group[0].fangkuai_24_Pos[i].x = (Random.Range(0, Set.setVal.Width - 1));
                            ledSturts_Group[0].fangkuai_24_Pos[i].y = Set.setVal.Height - 1;
                            break;
                    }
                    break;
                }
            }
            ledSturts_Group[0].MoveFangKuai_24(JieDuan);

        }
    }
    Vector2 startPos_25 = new Vector2();
    bool isClearing_Game25 = false;
    int tarageCol;

    bool isOver_25 = true;
    void CheckStayPos()
    {

        return;
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (LedKey.KeyStatus(pointId) == false)
                {
                    continue;

                }
                else
                {
                    //   Debug.LogError(GameLedControl.gamePoint[pointId].Color + "     " + col[tarageCol]);
                }

                if (GameLedControl.gamePoint[pointId].Color != col[tarageCol])
                {

                    //     GameLedControl.gamePoint[pointId].statue = enPointSta.Die;

                    //   errorCol = GameLedControl.gamePoint[pointId].Color;

                    startPos_25 = new Vector2(i, k);
                    isOver_25 = true;

                    //  Debug.LogError("错误的坐标是" + i + "    " + k);

                    return;
                }
            }

        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (LedKey.KeyStatus(pointId))
                {

                    if (GameLedControl.gamePoint[pointId].Color == col[tarageCol])
                    {
                        isOver_25 = false;

                        startPos_25 = new Vector2(i, k);
                        return;
                    }
                }

            }

        }
    }
    bool haveDesBlood = false;
    int checkTime = 0;
    float runTime25 = 1;
    void Run_Map25(int JieDuan)
    {


        CheckStayPos();
        runTime -= Time.deltaTime;
        if (!isClearing_Game25)
        {

            Debug.LogError(checkTime);

            if (checkTime != (int)runTime)
            {

                checkTime = (int)runTime;

                if (checkTime <= 3 && checkTime > 0 && Game00_Main.instance.Index_JieDuan < MaxJieDuan)
                {

                    Game00_Main.instance.PlayCountDown(checkTime);
                }
            }


            if (runTime < 0)
            {

             
                runTime = 5;
                isClearing_Game25 = true;
                for (int i = 0; i < Set.setVal.Width; i ++)
                {
                    for (int k = 0; k < Set.setVal.Height; k ++)
                    {
                        picId = i + Set.setVal.Width * k;
                        pointId = Framebuffer.tab_Mapping[picId];
                        if (GameLedControl.gamePoint[pointId].Color!= col[ tarageCol])
                        {
                            GameLedControl.gamePoint[pointId].Color = 0xff0000;
                            //  DrawPic.DrawCol(i, k, 2, col, enPointSta.Rest);
                            ChangeLed_Sta_Col(i, k, 0xff0000, enPointSta.Die);
                       

                        }
                       
                    }

                } 

            }

        }
        else
        {


            if (Run_MapClear_25(new Vector2(startPos_25.x, startPos_25.y), tarageCol))
            {

                Game00_Main.instance.Score_LinShi += 50;
                MusicManager.instance.Play_Correct();
                Game00_Main.instance.NextJieDuan();
            }        
            if (runTime<0)
        
            {


            }


        }

    }
    void Run_Map27()
    {

        for (int i = 0; i < Cnt; i++)
        {
            Framebuffer.Update_PointColor((int)Tarage_Pos[i].x, (int)Tarage_Pos[i].y, 0xff0000, enPointSta.Die);

        }
        for (int i = 0; i < 3; i++)
        {
            DrawPic.DrawCol(i, 0, Set.setVal.Height, 0x00ff00, enPointSta.Rest);
        }

        runTime -= Time.deltaTime;
        if (runTime < 0)
        {
            runTime = MaxrunTime;
            ledSturts_Group[0].Move_New27_Middle();
            ledSturts_Group[1].Move_New27_OutWay();
            ledSturts_Group[2].Move_New27_OutWay();
            ledSturts_Group[3].Move_New27_OutWay();
            ledSturts_Group[4].Move_New27_OutWay();
        }
        for (int i = 1; i < 5; i++)
        {
            Framebuffer.Update_PointColor((int)ledSturts_Group[i].x, (int)ledSturts_Group[i].y, 0xff0000, enPointSta.Die);

        }
        Framebuffer.Update_PointColor((int)(4 + Set.setVal.Width - 2) / 2, (int)ledSturts_Group[0].y, 0xff0000, enPointSta.Die);

    }
    void Run_Map27_Old(int JieDuan)
    {
        runTime -= Time.deltaTime;
        switch (JieDuan)
        {
            case 0:
                DrawPic.DrawCol(ledSturts_Group[0].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(ledSturts_Group[1].x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

                DrawPic.DrawCol(Set.setVal.Width / 2, 0, Set.setVal.Height, 0x00ff00, enPointSta.Rest);

                DrawPic.DrawRol(0, Set.setVal.Height / 2, Set.setVal.Width, 0x00ff00, enPointSta.Rest);

                break;
            case 1:
                for (int i = 0; i < 4; i++)
                {
                    ledSturts_Group[i].UpdatePic_FangKuai_27();
                }


                break;
            case 2:
                for (int i = 0; i < 4; i++)
                {
                    ledSturts_Group[i].UpdateFangKuai_27_Red();
                }

                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    ledSturts_Group[i].UpdateFangKuai_27_RedDoor(i);
                }

                break;
        }
        if (runTime < 0)
        {
            runTime = MaxrunTime;

            switch (JieDuan)
            {
                case 0:
                    for (int i = 0; i < 2; i++)
                    {
                        if (ledSturts_Group[i].dir == 0)
                        {
                            if (ledSturts_Group[i].x < Set.setVal.Width - 1)
                            {
                                ledSturts_Group[i].x++;

                            }
                            else
                            {
                                ledSturts_Group[i].dir = 1;
                            }
                        }
                        else
                        {
                            if (ledSturts_Group[i].x > 0)
                            {
                                ledSturts_Group[i].x--;

                            }
                            else
                            {
                                ledSturts_Group[i].dir = 0;
                            }
                        }
                    }


                    break;
                case 1:
                    for (int i = 0; i < 4; i++)
                    {
                        ledSturts_Group[i].MoveFangKuai_27();

                    }
                    break;
                case 2:
                    for (int i = 0; i < 4; i++)
                    {
                        ledSturts_Group[i].MoveFangKuai_27_Red();

                    }
                    break;
                case 3:
                    for (int i = 0; i < 4; i++)
                    {
                        ledSturts_Group[i].MoveFangKuai_27_RedDoor();

                    }
                    break;
            }



        }
    }
    void Run_Map28()
    {
        for (int i = 0; i < MaxSaftNum; i++)
        {
            ledSturts_Group[i].Update_SafePlace();
        }
        runTime -= Time.deltaTime;
        if (runTime < 0)
        {
            runTime = MaxrunTime / 2;
            if (r == 0)
            {
                r1++;

                if (r1 > Set.setVal.Width - 1)
                {
                    r1 = Set.setVal.Width - 1;
                    r = 1;
                }
            }
            else
            {
                r1--;
                if (r1 < 0)
                {
                    r1 = 0;
                    r = 0;
                }
            }

            if (dir == 0)
            {
                x++;

                if (x > Set.setVal.Width - 1)
                {
                    x = Set.setVal.Width - 1;
                    dir = 1;
                }
            }
            else
            {
                x--;
                if (x < 0)
                {
                    dir = 0;
                    x = 0;
                }
            }


        }
        DrawPic.DrawCol(x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
        DrawPic.DrawCol(r1, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
        if (Game00_Main.instance.remainTime < SettingInGame_01.instance.GetLevelTime() / 4)
        {
            if (MaxSaftNum != 4)
            {
                MaxSaftNum = 4;
                Game00_Main.instance.Index_JieDuan = 2;
            }

        }
        if (Game00_Main.instance.remainTime < SettingInGame_01.instance.GetLevelTime() / 2)
        {
            if (MaxSaftNum != 3)
            {
                MaxSaftNum = 3;
                Game00_Main.instance.Index_JieDuan = 3;
            }

        }
        if (Game00_Main.instance.remainTime < 1)
        {
            isClearAll = true;
            FjData.g_Fj[0].Scores += 100;
            return;
        }

    }
    void Run_Map28_Old(int JieDuan)
    {
        runTime -= Time.deltaTime;
        switch (JieDuan)
        {
            case 0:

                DrawPic.DrawRol(0, ledSturts_Group[0].y, Set.setVal.Width, 0xff0000, enPointSta.Die);
                DrawPic.DrawRol(0, ledSturts_Group[0].y + 1, Set.setVal.Width, 0xff0000, enPointSta.Die);

                break;
            case 1:

                ledSturts_Group[0].UpdateFangKuai_28();



                break;
            case 2:
                for (int i = 0; i < 4; i++)
                {
                    ledSturts_Group[i].UpdateFangKuai_28_Green(i);
                }

                break;
            case 3:


                break;
        }
        if (runTime < 0)
        {
            runTime = MaxrunTime;

            switch (JieDuan)
            {
                case 0:
                    for (int i = 0; i < 1; i++)
                    {
                        if (ledSturts_Group[i].dir == 0)
                        {
                            if (ledSturts_Group[i].y < Set.setVal.Height - 2)
                            {
                                ledSturts_Group[i].y++;

                            }
                            else
                            {
                                ledSturts_Group[i].dir = 1;
                            }
                        }
                        else
                        {
                            if (ledSturts_Group[i].y > 0)
                            {
                                ledSturts_Group[i].y--;

                            }
                            else
                            {
                                ledSturts_Group[i].dir = 0;
                            }
                        }
                    }


                    break;
                case 1:
                    for (int i = 0; i < 4; i++)
                    {
                        ledSturts_Group[i].MoveFangKuai_28();

                    }
                    break;
                case 2:
                    for (int i = 0; i < 4; i++)
                    {
                        ledSturts_Group[i].MoveFangKuai_28_Green();

                    }
                    break;
                case 3:


                    break;
            }



        }
    }
    void Run_Map29()
    {

        if (waitime > 0)
        {
            waitime -= Time.deltaTime;
        }
        else
        {
            if (dir == 0)
            {
                for (int i = 0; i < y; i++)
                {
                    DrawPic.DrawRol(0, i, Set.setVal.Width, 0xff0000, enPointSta.Die);
                }

            }
            else
            {
                for (int i = Set.setVal.Height; i > y; i--)
                {
                    DrawPic.DrawRol(0, i, Set.setVal.Width, 0xff0000, enPointSta.Die);
                }
            }

        }
        if (dir == 0)
        {
            if (y > Set.setVal.Height - 3)
            {
                DrawPic.DrawRol(0, Set.setVal.Height - 1, Set.setVal.Width, 0x00ff00, enPointSta.Rest);
                DrawPic.DrawRol(0, Set.setVal.Height - 2, Set.setVal.Width, 0x00ff00, enPointSta.Rest);

            }


        }
        else
        {
            if (y < 3)
            {
                DrawPic.DrawRol(0, 1, Set.setVal.Width, 0x00ff00, enPointSta.Rest);
                DrawPic.DrawRol(0, 0, Set.setVal.Width, 0x00ff00, enPointSta.Rest);

            }

        }
        runTime -= Time.deltaTime;

        if (runTime < 0)
        {
            runTime = MaxrunTime;
            if (dir == 0)
            {
                if (y < Set.setVal.Height - 2)
                {
                    y++;
                }
                else
                {
                    dir = 1;
                    waitime = 1f;
                }
            }
            else
            {
                if (y > 2)
                {
                    y--;
                }
                else
                {
                    waitime = 1f;
                    dir = 0;
                }
            }
        }
    }
    void Run_Map29_Old(int JieDuan)
    {
        runTime -= Time.deltaTime;
        switch (JieDuan)
        {
            case 0:
                ledSturts_Group[0].UpdateFangKuai_29();
                ledSturts_Group[1].UpdateFangKuai_29();

                break;
            case 1:

                break;
            case 2:
                ledSturts_Group[0].UpdatePic_Tank(ledSturts_Group[0].StartPos);

                //ledSturts_Group[0].UpdatedateYuan_29();
                //ledSturts_Group[1].UpdatedateYuan_29();
                break;
            case 3:


                break;
        }
        if (runTime < 0)
        {
            runTime = MaxrunTime;

            switch (JieDuan)
            {
                case 0:
                    for (int i = 0; i < 2; i++)
                    {
                        ledSturts_Group[i].MoveFangKuai_29();

                    }


                    break;
                case 1:

                    break;
                case 2:
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    ledSturts_Group[i].MoveYuan_29();

                    //}
                    bool go = ledSturts_Group[0].Moving_Tank();


                    bool go1 = false;
                    while (!go)
                    {
                        go = ledSturts_Group[0].Tank_ChangeMoving();
                    }
                    break;
                case 3:


                    break;
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
    

    public bool Run_MapClear_25(Vector2 StartPos, int Clor)
    {
        runTime25 -= Time.deltaTime;



      Run_ChangeMap_Clor((int)StartPos.x, (int)StartPos.y, x, Clor);
      
        if (x >2)
        {
            Run_ChangeMap_Clor((int)StartPos.x, (int)StartPos.y, x - 1, Clor);
            Run_ChangeMap_Clor((int)StartPos.x, (int)StartPos.y, x - 2, Clor);


        }
        if (runTime25 <= 0)
        {
            runTime25 =0.1f;
          
            { 
                
                    x++;

                if (Set.setVal.Width>=Set.setVal.Height)
                {
                    if (x >= Set.setVal.Width*2)
                    {
                        Framebuffer.FullScreen(0, enPointSta.None);
                        x = 0;
                        return true;

                    }

                }
                else
                {
                    if (x >= Set.setVal.Height * 2)
                    {
                        Framebuffer.FullScreen(0, enPointSta.None);
                        x = 0;
                        return true;

                    }
                }



            }
         



        }
        //  ClearMap();
        return false;

    }
    public void Run_MapYuan(Vector2 startPos, int r)
    {



        Run_ChangeMap_Red((int)startPos.x, (int)startPos.y, r);






    }
    void Middle_Safe()
    {
        for (int i = Set.setVal.Width / 4 - 1; i < Set.setVal.Width / 4 + 1; i++)
        {
            for (int k = Set.setVal.Height / 4 - 1; k < Set.setVal.Height / 4 + 1; k++)
            {
                picId = i + Set.setVal.Width * k;
                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picId]].statue = enPointSta.Rest;

            }
        }
        for (int i = Set.setVal.Width * 3 / 4 - 1; i < Set.setVal.Width * 3 / 4 + 1; i++)
        {
            for (int k = Set.setVal.Height * 3 / 4 - 1; k < Set.setVal.Height * 3 / 4 + 1; k++)
            {
                picId = i + Set.setVal.Width * k;
                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picId]].statue = enPointSta.Rest;

            }
        }
        for (int i = Set.setVal.Width / 4 - 1; i < Set.setVal.Width / 4 + 1; i++)
        {
            for (int k = Set.setVal.Height * 3 / 4 - 1; k < Set.setVal.Height * 3 / 4 + 1; k++)
            {
                picId = i + Set.setVal.Width * k;
                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picId]].statue = enPointSta.Rest;

            }
        }
        for (int i = Set.setVal.Width * 3 / 4 - 1; i < Set.setVal.Width * 3 / 4 + 1; i++)
        {
            for (int k = Set.setVal.Height / 4 - 1; k < Set.setVal.Height / 4 + 1; k++)
            {
                picId = i + Set.setVal.Width * k;
                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picId]].statue = enPointSta.Rest;

            }
        }
    }

    void EatTarage(int id)
    {
        if (GameLedControl.gamePoint[Framebuffer.tab_Mapping[id]].statue == enPointSta.Target)
        {
            tarageNum--;
            GameLedControl.gamePoint[Framebuffer.tab_Mapping[id]].statue = enPointSta.None;
        }

    }
    // Update is called once per frame

    int aaaa = 0;
    float aTime = 0;
    void Update()
    {

        if (Game00_Main.instance == null)
        {
            return;
        }
        if (Game00_Main.instance.statue != en_Game00_Sta.Play)
        {
            return;
        }


        if (isCleaning)
        {


            return;
        }

        Get_NowTarage();


        if (Game00_Main.instance.isClearTarage && Game00_Main.instance.statue == en_Game00_Sta.Play)
        {
            //Debug.LogError("剩下" + TarageNum_now);

            if (TarageNum_now <= 0)
            {
                if (Game00_Main.instance.gameLevel != 6 && Game00_Main.instance.gameLevel != 15 && Game00_Main.instance.gameLevel != 28 && Game00_Main.instance.gameLevel != 25 && Game00_Main.instance.gameLevel < 30)
                {

#if UNITY_EDITOR
                    Debug.LogError("没有目标点了");
#endif
                    isClearAll = true;
                    return;
                }
            }
        }
#if UNITY_EDITOR


        if (!Game00_Main.instance.isClearTarage)
        {
            Debug.Log("不要求全清理");

        }
        else
        {
            Debug.Log("要求全清理");
        }
#endif
        if (!Game00_Main.instance.isClearTarage)
        {

            Check_TarageNum();
        }

#if UNITY_EDITOR
        //        Debug.LogError(" Game00_Main.instance.gameLeve " + Game00_Main.instance.gameLevel);
#endif
        switch (Game00_Main.instance.gameLevel)
        {
            case 0:

                for (int i = 0; i < map0_Red.Length; i++)
                {
                    if (map0_Red[i].x >= 0)
                    {
                        picId = (int)map0_Red[i].x + Set.setVal.Width * (int)map0_Red[i].y;
                        pointId = Framebuffer.tab_Mapping[picId];
                        GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                        // DrawPic.DrawRol()
                    }


                }

                break;
            case 1:
                Run_NewMap02();
                //    Run_Map01();
                break;
            case 2:

                Run_Map02();
                break;
            case 3:

                Run_Map03();
                break;
            case 4:
                Run_NewMap03();
                // Run_Map04();
                break;
            case 5:

                Run_Map05();
                break;
            case 6:
                Run_Map06_Youyu();

                break;
            case 7:

                Run_Map07();
                break;
            case 8:
                angly += Time.deltaTime * 20;
                if (Set.setVal.Width > Set.setVal.Height)
                {
                    Run_Map08(Set.setVal.Width / 2, Set.setVal.Height / 2, angly, Set.setVal.Width / 2 + 1, 1, Game00_Main.instance.Index_JieDuan);

                }
                else
                {
                    Run_Map08(Set.setVal.Width / 2, Set.setVal.Height / 2, angly, Set.setVal.Height / 2 + 1, 1, Game00_Main.instance.Index_JieDuan);

                }
                break;
            case 9:
                //  Run_MapClear();
                Run_Map09(Game00_Main.instance.Index_JieDuan);
                break;
            case 10:

                Run_Map10(Game00_Main.instance.Index_JieDuan);
                break;
            case 11:

                Run_Map11(Game00_Main.instance.Index_JieDuan);
                break;
            case 12:

                Run_Map12(Game00_Main.instance.Index_JieDuan);
                break;
            case 13:

                Run_Map13(Game00_Main.instance.Index_JieDuan);
                break;
            case 14:
                Run_Map14();
                break;
            case 15:

                Run_Map25(Game00_Main.instance.Index_JieDuan);
                break;
            case 16:

                Run_Map16(Game00_Main.instance.Index_JieDuan);
                break;
            case 17:


                Run_Map17(Game00_Main.instance.Index_JieDuan);
                break;
            case 18:


                Run_Map18(Game00_Main.instance.Index_JieDuan);
                break;
            case 19:


                Run_Map19(Game00_Main.instance.Index_JieDuan);
                break;
            case 20:
                Run_NewMap00();

                // Run_Map20(Game00_Main.instance.Index_JieDuan);
                break;
            case 21:


                Run_Map21(Game00_Main.instance.Index_JieDuan);
                break;
            case 22:


                Run_Map22();
                break;
            case 23:


                Run_Map23();
                break;
            case 24:


                Run_Map24(Game00_Main.instance.Index_JieDuan);
                break;
            case 25:

                Run_NewMap01();
                //   Run_Map25(Game00_Main.instance.Index_JieDuan);
                break;
            case 26:


                Run_Map06();
                break;
            case 27:


                Run_Map27();

                break;
            case 28:


                Run_Map28();

                break;

            case 29:


                Run_Map29();

                break;
        }
    }
}

