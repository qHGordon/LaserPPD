using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_WallLED : MonoBehaviour
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

    public int MaxJieDuan = 4;
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
    public static Map_WallLED instance;
    Vector2 riverPos_Now = Vector2.zero;

    public LedAnim_Struts_Wall[] ledSturts_Group = new LedAnim_Struts_Wall[4];
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
public    void Clear()
    {
        for (int i = Set.setVal.Width * Set.setVal.Height; i < Set.setVal.Width * (Set.setVal.Height + Set.setVal.WallNum_Height); i++)
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
    void InitMap_00()
    {


        Clear();
        GetAiXin_5_10(5);

    }
    void InitMap_01(int jieduan)
    {
        MaxJieDuan = 3;
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
        MaxJieDuan = 5;
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
        MaxJieDuan = 7;
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
        }


        if (Game00_Main.instance.Index_JieDuan > 2)
        {
            ledSturts_Group[1].enabled = true;
            if (Set.setVal.Width > 30 || Set.setVal.Height > 30)
            {
                ledSturts_Group[2].enabled = true;

            }
        }
        ledSturts_Group[0].enabled = true;
        DrawSafePlace(2);
        GetRandom_Tarage();
    }
    void InitMap_04()
    {
        MaxJieDuan = 5;
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
        MaxJieDuan = 5;
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
        for (int i = 0; i < YouYu_Safe.Length; i++)
        {
            if (YouYu_Safe[i].x >= 0)
            {
                picId = (int)YouYu_Safe[i].x + (int)YouYu_Safe[i].y * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
            }
        }
    }
    void InitMap_06_YouYu(int jieduan)
    {

        MaxJieDuan = 6;
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
        MaxJieDuan = 3;

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
        return;
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
        MaxJieDuan = 4;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);


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

        Clear(); MaxJieDuan = 4;

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


        GetRandom_Tarage();


    }
    void InitMap_14()
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

        if (SettingInGame_01.instance.GetReMainNum(jieduan) > 0)
        {
            remainPoint = SettingInGame_01.instance.GetReMainNum(jieduan);

        }

        tarageNum = SettingInGame_01.instance.GetTarageNum();
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
    void InitMap_18(int jieduan)
    {
        Clear();
        MaxJieDuan = 3;
        tarageNum = 20;
        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        }
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        if (jieduan > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                ledSturts_Group[i].enabled = true;

                ledSturts_Group[i].Init(false, this);
                ledSturts_Group[i].dir = i % 2;
            }

        }
        ledSturts_Group[1].x = Set.setVal.Width / 2;
        ledSturts_Group[1].y = Set.setVal.Height / 2;
        if (jieduan == 2)
        {
            ledSturts_Group[0].x = Set.setVal.Width / 2;
            ledSturts_Group[0].dir = 0;
        }
        GetRandom_Tarage();

    }
    void InitMap_19(int jieduan)
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
        if (jieduan > 0)
        {
            for (int i = 0; i < 1; i++)
            {
                ledSturts_Group[i].enabled = true;

                ledSturts_Group[i].Init(false, this);

            }

        }
        ledSturts_Group[0].Init_Game19_M();

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
        switch (jieduan)
        {
            default:
                for (int i = 0; i < 1; i++)
                {
                    ledSturts_Group[i].enabled = true;



                    ledSturts_Group[i].Init(false, this);

                }
                startPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 2);
                break;
            case 1:
                for (int i = 0; i < 1; i++)
                {
                    ledSturts_Group[i].enabled = true;



                    ledSturts_Group[i].Init(false, this);

                }
                startPos = new Vector2(Set.setVal.Width / 3, Set.setVal.Height / 4);

                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    ledSturts_Group[i].enabled = true;



                    ledSturts_Group[i].Init(false, this);

                }
                startPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 4);
                startPos1 = new Vector2(Set.setVal.Width * 3 / 4, Set.setVal.Height / 4);

                break;
            case 3:
                for (int i = 0; i < 2; i++)
                {
                    ledSturts_Group[i].enabled = true;



                    ledSturts_Group[i].Init(false, this);

                }
                startPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 2);
                startPos1 = new Vector2(Set.setVal.Width * 3 / 4, Set.setVal.Height / 4);
                break;
            case 4:
                for (int i = 0; i < 2; i++)
                {
                    ledSturts_Group[i].enabled = true;



                    ledSturts_Group[i].Init(false, this);

                }
                startPos = new Vector2(Set.setVal.Width / 4, Set.setVal.Height / 4);
                startPos1 = new Vector2(Set.setVal.Width * 3 / 4, Set.setVal.Height * 3 / 4);
                break;
        }

        DrawSafePlace(1);
        GetRandom_Tarage();

    }

    void InitMap_22(int jieduan)
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
    void InitMap_23(int jieduan)
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
        MaxJieDuan = 4;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        switch (jieduan)
        {
            case 0:
                for (int i = 0; i < 2; i++)
                {
                    ledSturts_Group[i].enabled = true;
                }
                ledSturts_Group[0].x = 0;
                ledSturts_Group[1].x = Set.setVal.Width / 2;
                break;
            case 1:
                for (int i = 0; i < 4; i++)
                {
                    ledSturts_Group[i].enabled = true;
                    ledSturts_Group[i].dir = i;
                }
                ledSturts_Group[0].x = 1;
                ledSturts_Group[0].y = 1;
                ledSturts_Group[1].x = Set.setVal.Width - 3;
                ledSturts_Group[1].y = 1;

                ledSturts_Group[2].x = Set.setVal.Width - 3;
                ledSturts_Group[2].y = Set.setVal.Height - 3;

                ledSturts_Group[3].x = 1;
                ledSturts_Group[3].y = Set.setVal.Height - 3;
                break;
            case 2:
                for (int i = 0; i < 4; i++)
                {
                    ledSturts_Group[i].enabled = true;
                    ledSturts_Group[i].dir = i;
                }
                ledSturts_Group[0].x = 0;
                ledSturts_Group[0].y = 0;
                ledSturts_Group[1].x = Set.setVal.Width - 1;
                ledSturts_Group[1].y = 0;

                ledSturts_Group[2].x = Set.setVal.Width - 1;
                ledSturts_Group[2].y = Set.setVal.Height - 1;

                ledSturts_Group[3].x = 0;
                ledSturts_Group[3].y = Set.setVal.Height - 1;
                break;
        }



        ledSturts_Group[0].enabled = true;


        if (jieduan == 2)
        {
            for (int i = 1; i < Set.setVal.Width - 1; i++)
            {
                for (int k = 1; k < Set.setVal.Height - 1; k++)
                {
                    ChangeLed_Sta(i, k, enPointSta.Die);
                }
            }
        }
        for (int i = 0; i < Set.setVal.Height; i++)
        {
            ChangeLed_Sta(Set.setVal.Width / 2, i, enPointSta.Rest);
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            ChangeLed_Sta(i, Set.setVal.Height / 2, enPointSta.Rest);
        }
        switch (jieduan)
        {
            default:
                GetRandom_Tarage();
                break;
            case 1:
                for (int i = 1; i < Set.setVal.Width - 2; i++)
                {
                    ChangeLed_Sta(i, 1, enPointSta.Target);
                    ChangeLed_Sta(i, Set.setVal.Height - 2, enPointSta.Target);
                }
                for (int i = 1; i < Set.setVal.Height - 2; i++)
                {
                    ChangeLed_Sta(1, i, enPointSta.Target);
                    ChangeLed_Sta(Set.setVal.Width - 2, i, enPointSta.Target);
                }
                break;
            case 2:
                for (int i = 0; i < Set.setVal.Height; i++)
                {
                    ChangeLed_Sta(Set.setVal.Width * 3 / 4, i, enPointSta.Target);
                    ChangeLed_Sta(Set.setVal.Width / 4, i, enPointSta.Target);
                }
                for (int i = 0; i < 3; i++)
                {
                    ChangeLed_Sta(Set.setVal.Width / 4 - 1 + i, 0, enPointSta.Target);
                    ChangeLed_Sta(Set.setVal.Width / 4 - 1 + i, Set.setVal.Height - 1, enPointSta.Target);
                    ChangeLed_Sta(Set.setVal.Width * 3 / 4 - 1 + i, 0, enPointSta.Target);
                    ChangeLed_Sta(Set.setVal.Width * 3 / 4 - 1 + i, Set.setVal.Height - 1, enPointSta.Target);

                }

                break;
        }



    }
    void InitMap_28(int jieduan)
    {

        Clear();
        isClearing_Game25 = false;
        isClearAll = false;
        tarageCol = Random.Range(0, 4);
        isOver_25 = false;
        x = 0;
        y = 0;
        MaxJieDuan = 4;
        tarageNum = Game00_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }

        for (int i = 0; i < 2; i++)
        {
            ledSturts_Group[i].enabled = true;
        }


        ledSturts_Group[0].enabled = true;
        for (int i = 0; i < Set.setVal.Height; i++)
        {
            ChangeLed_Sta(0, i, enPointSta.Rest);
            ChangeLed_Sta(1, i, enPointSta.Rest);
            ChangeLed_Sta(Set.setVal.Width - 1, i, enPointSta.Rest);
            ChangeLed_Sta(Set.setVal.Width - 2, i, enPointSta.Rest);
        }
        if (jieduan == 0)
        {
            ledSturts_Group[0].x = 2;
            ledSturts_Group[0].y = Set.setVal.Height - 1;
        }




        switch (jieduan)//蓝
        {
            case 0:
            case 1:
            case 2:
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
                            
                            
                        }
                    }
                }

                break;
        }
        if (jieduan == 2)
        {
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


            ledSturts_Group[0].y = Set.setVal.Height - 1;
            ledSturts_Group[1].y = Set.setVal.Height / 2;
            ledSturts_Group[1].dir = 1;

        }
        if (jieduan == 3)
        {
            for (int i = 2; i < Set.setVal.Width - 2; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    picId = i + (Set.setVal.Height / 4 - 1 + k) * Set.setVal.Width;
                    pointId = Framebuffer.tab_Mapping[picId];

                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                    picId = i + (Set.setVal.Height * 3 / 4 - 1 + k) * Set.setVal.Width;
                    pointId = Framebuffer.tab_Mapping[picId];

                    GameLedControl.gamePoint[pointId].statue = enPointSta.Die;

                }
            }
            for (int i = 0; i < Set.setVal.Height; i++)
            {

                picId = Set.setVal.Width / 4 + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
                picId = Set.setVal.Width * 3 / 4 + i * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                GameLedControl.gamePoint[pointId].statue = enPointSta.Die;
            }
        }

    }

    void InitMap_29(int jieduan)
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
    float MaxWaitTime = 5f;
    void InitMap_25(int jieduan)
    {
        haveDesBlood = false;
        tarageCol = Random.Range(0, 5);
        sound_Youyu.Stop();
        sound_Youyu.clip = Spr_Tatage_Color[tarageCol];
        sound_Youyu.Play();
        Clear();
        MaxJieDuan = 4;
        Game00_Main.instance.isClearTarage = false;
        runTime = MaxWaitTime;

        if (Set.setVal.Height > 25 || Set.setVal.Width > 25)
        {
            switch (jieduan)
            {
                case 0:
                    tarageNum = 5;
                    break;
                case 1:
                    tarageNum = 4;
                    break;
                case 2:
                    tarageNum = 4;
                    break;
                case 3:
                    tarageNum = 3;
                    break;
                case 4:
                    tarageNum = 3;
                    break;
            }
        }
        else
        {
            switch (jieduan)
            {
                case 0:
                    tarageNum = 4;
                    break;
                case 1:
                    tarageNum = 3;
                    break;
                case 2:
                    tarageNum = 3;
                    break;
                case 3:
                    tarageNum = 2;
                    break;
                case 4:
                    tarageNum = 2;
                    break;
            }
        }


        while (tarageNum > 0)
        {
            switch (tarageNum)
            {
                default:
                    break;
            }
            randomX = Random.Range(0, Set.setVal.Width / 2);
            randomY = Random.Range(0, Set.setVal.Height / 2);




            ChangeLed_Sta_Col(randomX * 2, randomY * 2, col[tarageCol], enPointSta.Rest);
            ChangeLed_Sta_Col(randomX * 2 + 1, randomY * 2, col[tarageCol], enPointSta.Rest);
            ChangeLed_Sta_Col(randomX * 2 + 1, randomY * 2 + 1, col[tarageCol], enPointSta.Rest);
            ChangeLed_Sta_Col(randomX * 2, randomY * 2 + 1, col[tarageCol], enPointSta.Rest);
            //  DrawPic.DrawRol(randomX, randomY, 2, col, enPointSta.Rest);
            //   DrawPic.DrawRol(randomX, randomY + 1, 2, col, enPointSta.Rest);
            tarageNum--;
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
#if UNITY_EDITOR
        //        Debug.LogError(" 目标点 " + TarageNum_now);
#endif
        if (TarageNum_now <= 3 && Game00_Main.instance.gameLevel != 25 && Game00_Main.instance.gameLevel != 6)//
        {
            SafeTime -= Time.deltaTime;
            if (SafeTime <= 0)
            {
                isClearAll = true;
            }
        }

    }
    int randomX, randomY = 0;
    int yy_Num = 0;
    void GetRandom_Tarage()
    {
        //if (Set.setVal.Width > 25 || Set.setVal.Height > 25)
        //{
        //    tarageNum = 50;
        //}
        //else
        //{
        //    tarageNum = 30;
        //}

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
         
      
       
   
        Clear();
        GetBlueTargetPos();
        

        return;
  



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
            // Check_TarageNum();

        }
    }
    void Run_Map03()
    {
        runTime -= Time.deltaTime;
        if (ledSturts_Group[0].enabled == false)
        {
            ledSturts_Group[0].enabled = true;
        }
        ledSturts_Group[0].UpdatePic_Tank(ledSturts_Group[0].StartPos);

        if (runTime <= 0)
        {

            runTime = MaxrunTime;//和可设置的速度挂钩
                                 //  ClearMap();

            bool go = ledSturts_Group[0].Moving_Tank();


            bool go1 = false;
            if (!go)
            {
                go = ledSturts_Group[0].Tank_ChangeMoving();
            }
            if (Game00_Main.instance.Index_JieDuan > 1)
            {
                go1 = ledSturts_Group[1].Moving_Tank();
                if (!go1)
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
            ClearMap();

            runTime = MaxrunTime;
            if (ledSturts_Group[0].dir == 0)
            {
                if (x >= Set.setVal.Width - 1)
                {
                    x = Set.setVal.Width - 1;
                    ledSturts_Group[0].dir = 1;
                }
                else
                {
                    x++;
                }




            }
            else
            {
                if (x <= 0)
                {
                    x = 0;
                    ledSturts_Group[0].dir = 0;
                }
                else
                {
                    x--;
                }



            }


        }
        DrawPic.DrawCol(x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
    }
    void Run_Map11(int JieDuan)
    {
        runTime -= Time.deltaTime;
        //     ledSturts_Group[0].UpdatePic_FangKuang(new Vector2(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2), ledSturts_Group[0].x);

        if (runTime <= 0)
        {
            ClearMap();

            runTime = MaxrunTime;
            if (ledSturts_Group[0].dir == 0)
            {
                if (x >= Set.setVal.Width - 1)
                {
                    x = Set.setVal.Width - 1;
                    ledSturts_Group[0].dir = 1;
                }
                else
                {
                    x++;
                }




            }
            else
            {
                if (x <= 0)
                {
                    x = 0;
                    ledSturts_Group[0].dir = 0;
                }
                else
                {
                    x--;
                }



            }


        }
        DrawPic.DrawCol(x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

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
            if (ledSturts_Group[0].dir == 0)
            {
                if (x >= Set.setVal.Width - 1)
                {
                    x = Set.setVal.Width - 1;
                    ledSturts_Group[0].dir = 1;
                }
                else
                {
                    x++;
                }




            }
            else
            {
                if (x <= 0)
                {
                    x = 0;
                    ledSturts_Group[0].dir = 0;
                }
                else
                {
                    x--;
                }



            }

        }

        DrawPic.DrawCol(x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);
        //for (int i = 0; i < Set.setVal.Width; i++)
        //{
        //    for (int k = 0; k < 3; k++)
        //    {
        //        picId = i + k * Set.setVal.Width;
        //        pointId = Framebuffer.tab_Mapping[picId];
        //        GameLedControl.gamePoint[pointId].statue = enPointSta.Die;


        //        picId = i + (Set.setVal.Height - k) * Set.setVal.Height;
        //        pointId = Framebuffer.tab_Mapping[picId];
        //        GameLedControl.gamePoint[pointId].statue = enPointSta.Die;

        //    }
        //}
    }
    void Run_Map13(int JieDuan)
    {
        runTime -= Time.deltaTime;
        ClearMap();
        ledSturts_Group[0].UpdatePic_FangKuai_13();
        if (JieDuan >= 2)
        {
            ledSturts_Group[1].UpdatePic_FangKuai_13();

        }

        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            if (JieDuan < 5)
            {
                switch (JieDuan)
                {
                    default:

                        ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);
                        break;
                    case 1:

                        ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);
                        break;
                    case 2:

                        ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);
                        ledSturts_Group[1].Move_FangKuai_13(Game_Fangkuai_length);
                        break;
                    case 3:

                        ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);
                        ledSturts_Group[1].Move_FangKuai_13(Game_Fangkuai_length);
                        break;
                    case 4:

                        ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);
                        ledSturts_Group[1].Move_FangKuai_13(Game_Fangkuai_length);
                        break;

                }
            }
            else
            {

                ledSturts_Group[0].Move_FangKuai_13(Game_Fangkuai_length);
                ledSturts_Group[1].Move_FangKuai_13(Game_Fangkuai_length);
            }
        }

    }
    void Run_Map14(int JieDuan)
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
        ClearMap();

        if (JieDuan == 2)
        {
            for (int i = 0; i < ledSturts_Group[0].x; i++)
            {
                DrawPic.DrawCol(i, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

            }
            //for (int i = ledSturts_Group[0].x; i < Set.setVal.Width / 2; i++)
            //{
            //    DrawPic.DrawCol(i, 0, Set.setVal.Height, 0xff0000, enPointSta.None);

            //}
            for (int i = Set.setVal.Width / 2; i < Set.setVal.Width; i++)
            {

                DrawPic.DrawCol(i, 0, ledSturts_Group[1].y, 0xff0000, enPointSta.Die);



            }
            //for (int i = Set.setVal.Width / 2; i < ledSturts_Group[1].x; i++)
            //{
            //    DrawPic.DrawCol(i, 0, Set.setVal.Height, 0xff0000, enPointSta.None);

            //}
        }
        else
        {
            for (int i = 0; i < JieDuan + 1; i++)
            {
                ledSturts_Group[i].UpdatePic_FangKuai_18(JieDuan);

            }
        }

        if (runTime <= 0)
        {
            runTime = MaxrunTime;

            if (JieDuan == 2)
            {
                switch (ledSturts_Group[1].dir)
                {
                    case 0:

                        if (ledSturts_Group[1].y < Set.setVal.Height)
                        {

                            ledSturts_Group[1].y++;
                        }
                        else
                        {
                            ledSturts_Group[1].dir = 1;
                        }
                        break;
                    case 1:

                        if (ledSturts_Group[1].y > 0)
                        {

                            ledSturts_Group[1].y--;
                        }
                        else
                        {
                            ledSturts_Group[1].dir = 0;
                        }
                        break;

                }
                switch (ledSturts_Group[0].dir)
                {
                    case 0:

                        if (ledSturts_Group[0].x < Set.setVal.Width / 2)
                        {

                            ledSturts_Group[0].x++;
                        }
                        else
                        {
                            ledSturts_Group[0].dir = 1;
                        }
                        break;
                    case 1:

                        if (ledSturts_Group[0].x > 0)
                        {

                            ledSturts_Group[0].x--;
                        }
                        else
                        {
                            ledSturts_Group[0].dir = 0;
                        }
                        break;

                }
            }
            else
            {
                switch (JieDuan)
                {
                    case 0:
                        ledSturts_Group[0].MoveFangKuai_18(JieDuan);

                        break;
                    case 1:
                        ledSturts_Group[0].MoveFangKuai_18(JieDuan);
                        ledSturts_Group[1].MoveFangKuai_18(JieDuan);

                        break;
                    case 2:
                    case 3:
                        ledSturts_Group[0].MoveFangKuai_18(JieDuan);
                        switch (ledSturts_Group[1].dir)
                        {
                            case 0:

                                if (ledSturts_Group[1].x < Set.setVal.Width)
                                {

                                    ledSturts_Group[1].x++;
                                }
                                else
                                {
                                    ledSturts_Group[1].dir = 1;
                                }
                                break;
                            case 1:

                                if (ledSturts_Group[1].x > Set.setVal.Width / 2)
                                {

                                    ledSturts_Group[1].x--;
                                }
                                else
                                {
                                    ledSturts_Group[1].dir = 0;
                                }
                                break;

                        }
                        break;

                    case 4:
                        ledSturts_Group[0].MoveFangKuai_18(JieDuan);
                        ledSturts_Group[1].MoveFangKuai_18(JieDuan);

                        break;

                }
            }


        }

        for (int k = 0; k < Set.setVal.Height; k++)
        {
            picId = 0 + Set.setVal.Width / 2 + Set.setVal.Width * k;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
        }



        for (int k = 0; k < Set.setVal.Width; k++)
        {
            picId = k + (Set.setVal.Height / 2) * Set.setVal.Width;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
        }



    }

    void Run_Map19(int JieDuan)
    {
        runTime -= Time.deltaTime;

        ledSturts_Group[0].Update_Game19_M(JieDuan);
        if (runTime <= 0)
        {
            runTime = MaxrunTime;
            ledSturts_Group[0].Moving_Game19_M();

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
        if (JieDuan > 1)
        {
            Run_MapYuan(startPos1, r1);
        }
        Run_MapYuan(startPos, r);
        if (runTime < 0)
        {
            runTime = runTime = MaxrunTime;
            r++;
            if (JieDuan > 1)
            {
                r1++;
            }
            if (Set.setVal.Width > Set.setVal.Height)
            {

                if (r > Set.setVal.Width - 1)
                {
                    r = 0;

                }
                if (r1 > Set.setVal.Width - 1)
                {
                    r1 = 0;

                }
            }
            else
            {
                if (r > Set.setVal.Height - 1)
                {
                    r = 0;

                }
                if (r1 > Set.setVal.Height - 1)
                {
                    r1 = 0;

                }
            }
        }


    }
    void Run_Map22(int JieDuan)
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
    void Run_Map23(int JieDuan)
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


                        if (Set.setVal.Width - ledSturts_Group[0].x >= Set.setVal.Width / 2)
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
    public bool isClearing_Game25 = false;
    int tarageCol;

    bool isOver_25 = true;
    void CheckStayPos()
    {
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
                    Debug.LogError(GameLedControl.gamePoint[pointId].Color + "     " + col[tarageCol]);
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

    void Run_Map25(int JieDuan)
    {
        //  Debug.LogError(startPos_25.x + "     " + startPos_25.y);


        //if (Run_MapClear_25(new Vector2(startPos_25.x, startPos_25.y), tarageCol))
        //{

        //}
        //return;
        //  Debug.LogError((int)runTime);
        if (Input.GetKey(KeyCode.N))
        {
            startPos_25 = new Vector2(0, 1);
            isClearing_Game25 = true;
            isOver_25 = false;

        }
        if (Input.GetKey(KeyCode.M))
        {
            isOver_25 = false;
            startPos_25 = new Vector2(0, 1);
            isClearing_Game25 = true;


        }
        //  Debug.LogError(col[tarageCol] + "    " + GameLedControl.gamePoint[0].Color);

        CheckStayPos();

        if (!isClearing_Game25)
        {

            runTime -= Time.deltaTime;



            if (runTime < 0)
            {
                runTime = 5f;
                //

                isClearing_Game25 = true;



            }

        }
        else
        {
            // startPos_25 = new Vector2(Set.setVal.Width / 2, Set.setVal.Height);

            if (isOver_25)
            {
                if (FjData.g_Fj[0].Life > 0 && !haveDesBlood)
                {
                    FjData.g_Fj[0].Life--;
                    haveDesBlood = true;
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
                if (Run_MapClear_25(new Vector2(startPos_25.x, startPos_25.y), 5))
                {
                    InitMap_25(Game00_Main.instance.Index_JieDuan);
                }
            }
            else
            {
                if (Run_MapClear_25(new Vector2(startPos_25.x, startPos_25.y), tarageCol))


                {
                    Game00_Main.instance.NextJieDuan();

                }
            }

        }

    }
    void Run_Map27(int JieDuan)
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

    void Run_Map28(int JieDuan)
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
    void Run_Map29(int JieDuan)
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
    //  Run_ChangeMap_Clor
    public bool Run_MapClear_25(Vector2 StartPos, int Clor)
    {
        runTime -= Time.deltaTime;

        if (runTime <= 0)
        {
            runTime = 0.02f;
            if (Set.setVal.Width > Set.setVal.Height)
            {
                if (haveClear)
                {
                    x++;
                    Run_ChangeMap_None((int)StartPos.x, (int)StartPos.y, x, 2);
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
                    Run_ChangeMap_Clor((int)StartPos.x, (int)StartPos.y, x, Clor);

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
                    Run_ChangeMap_None((int)StartPos.x, (int)StartPos.y, x, 2);
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
                    Run_ChangeMap_Clor((int)StartPos.x, (int)StartPos.y, x, Clor);

                    if (x >= Set.setVal.Height)
                    {
                        haveClear = true;
                        x = 0;
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InitMap(0);
        }   if (Input.GetKeyDown(KeyCode.C))
        {
            Clear();
        }
        for (int i = 0; i < targetPos.Length; i++)
        {
            if (targetPos[i].x > Set.setVal.Width  || targetPos[i].y > Set.setVal.Height+Set.setVal.WallNum_Height)
            {
                continue;   
            }
            if (targetPos[i].x > 0&& targetPos[i].y>=Set.setVal.Height&& targetPos[i].x <Set.setVal.Width && targetPos[i].y < Set.setVal.Height+Set.setVal.WallNum_Height)
            {
                //                Debug.LogError((int)targetPos[i].x + "       " + (int)targetPos[i].y);
                //picId = (int)targetPos[i].x + Set.setVal.Width * (int)targetPos[i].y;
                //pointId = Framebuffer.tab_Mapping[picId];
                //GameLedControl.gamePoint[pointId].statue = enPointSta.Target;
                if ((int)targetPos[i].x<=0)
                {
                    continue;
                }
                if (randType==0)
                {
                    DrawPic.DrawPoint((int)targetPos[i].x, (int)targetPos[i].y, 0xff0000, enPointSta.Die);

                }
                else
                {
                    DrawPic.DrawPoint((int)targetPos[i].x, (int)targetPos[i].y, 255, enPointSta.Target);

                }

            }
        }
        return;
       

        if (isCleaning)
        {


            return;
        }

        Get_NowTarage();


        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    TarageNum_now = 0;
        //    isClearAll = true;
        //}
        if (Game00_Main.instance.isClearTarage && Game00_Main.instance.statue == en_Game00_Sta.Play)
        {
            if (TarageNum_now <= 0 && Game00_Main.instance.gameLevel != 6 && Game00_Main.instance.gameLevel != 20 && Game00_Main.instance.gameLevel < 30)
            {
#if UNITY_EDITOR
                Debug.LogError("没有目标点了");
#endif
                isClearAll = true;
                return;
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
        if (!Game00_Main.instance.isClearTarage && (Game00_Main.instance.gameLevel != 1 && Game00_Main.instance.gameLevel != 25))
        {

            Check_TarageNum();
        }
        //aTime += Time.deltaTime;
        //if (aTime > 1f)
        //{
        //    aaaa++;
        //    for (int i = 0; i < aaaa; i++)
        //    {
        //        GameLedControl.gamePoint[Framebuffer.tab_Mapping[i]].statue = enPointSta.Rest;
        //    }

        //}

  
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

                picId = xx +   y;
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



            for (int y = AiXinHigh + Set.setVal.Height; y < AiXinHigh + high + (Set.setVal.Height  ); y++)
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
    void GetDiamond_10_15(int h)
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
                    high = 7;
                    break;
                case 1:
                    high = 6;
                    break;
                case 2:
                    high = 5;
                    break;
                case 3:
                    high = 4;
                    break;
                case 4:
                    high = 2;
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

    void GetDiamond_Over15(int h)
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
                    high = 9;
                    break;
                case 1:
                    high = 8;
                    break;
                case 2:
                    high = 7;
                    break;
                case 3:
                    high = 6;
                    break;
                case 4:
                    high = 5;
                    break;
                case 5:
                    high = 3;
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
    void GetDiamond_Over20(int h)
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
                high = 12;
            }
            else if (x > 7)

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
            else if (x == 7)

            {
                high = 2;
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

    void GetBiaoQing(int _x, int _y, int r, int h)
    {
        //笑脸
        int picid = 0;

        float dis;
        for (int i = _x - r; i <= _x + r; i++)
        {
            for (int j = _y - r; j <= _y + r; j++)
            {

                dis = Mathf.Abs(Vector2.Distance(new Vector2(i, j), new Vector2(Set.setVal.Width / 2, Set.setVal.Height + Set.setVal.WallNum_Height / 2)));

                if (dis < r && dis >= (4 * r / 5)) //dis >= r - w &&
                {

                    //  Vector2 pos = new Vector2(_x + j * widthOne, _y + i * widthOne);

                    if (i >= 0 && j >= 0)
                    {
                        if (i <= Set.setVal.Width - 1 && j <= (Set.setVal.Height + Set.setVal.WallNum_Height) - 1)
                        {

                            picid = i + Set.setVal.Width * j;
                            if (picid < Set.setVal.Width * (Set.setVal.Height + Set.setVal.WallNum_Height))
                            {
                                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].statue = enPointSta.Target;
                                targetPos[TarPosCnt] = new Vector2(i, j);
                                bool_HaveFire[TarPosCnt] = false;
                                TarPosCnt++;
                            }


                        }

                    }

                }
            }
        }

        if (Set.setVal.Width >= 13)
        {
            AiXinHigh = _y - r + 4;
            AiXinHighCnt = 0;
        }
        else
        {
            AiXinHigh = _y - r + 2;
            AiXinHighCnt = 0;
        }

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
                    if (Set.setVal.Width >= 13)
                    {
                        high = 1;
                    }
                    else
                    {
                        high = 0;
                    }
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
        if (Set.setVal.Width >= 13)
        {
            AiXinHigh = _y + r - 5;
            AiXinHighCnt = 0;
        }
        else
        {
            AiXinHigh = _y + r - 4;
            AiXinHighCnt = 0;
        }

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
                //case 0:
                //    high = 1;
                //    break;
                //case 4:
                //    high = 1;
                //    break;
                case 1:
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
   public int randType = 0;

    public void GetBlueTargetPos()
    {
        

        TarPosCnt = 0;

        for (int i = 0; i < bool_HaveFire.Length; i++)
        {
            bool_HaveFire[i] = false;
        }
        for (int i = 0; i < targetPos.Length; i++)
        {
            targetPos[i] = Vector2.one * -1;
        }
        FjData.g_Fj[0].RemainPoint = 1000;
          randType =Random.Range(0,8);
       
        

        switch (randType)
        {
            case 0://爱心
                if (Set.setVal.Width < 10)
                {
                    GetAiXin_5_10(0);
                    
                    if (Set.setVal.WallNum_Height >= 30)
                    {
                        GetAiXin_5_10(6);
                        GetAiXin_5_10(12);
                        GetAiXin_5_10(18);
                        GetAiXin_5_10(24);
                        GetAiXin_5_10(30);
                    }
                    else if (Set.setVal.WallNum_Height > 24)
                    {
                        GetAiXin_5_10(6);
                        GetAiXin_5_10(12);
                        GetAiXin_5_10(18);
                        GetAiXin_5_10(24);
                    }
                    else if (Set.setVal.WallNum_Height > 18)
                    {
                        GetAiXin_5_10(6);
                        GetAiXin_5_10(12);
                        GetAiXin_5_10(18);
                    }
                    else if (Set.setVal.WallNum_Height > 12)
                    {

                        GetAiXin_5_10(6);
                        GetAiXin_5_10(12);
                    }
                    else if (Set.setVal.WallNum_Height > 6)
                    {
                        Debug.LogError(111);
                        GetAiXin_5_10(6);
                    }
                }
                else if (Set.setVal.Width <= 15)
                {
                    GetAiXin_10_15(0);
                    if (Set.setVal.WallNum_Height >= 40)
                    {
                        GetAiXin_10_15(8);
                        GetAiXin_10_15(16);
                        GetAiXin_10_15(24);
                        GetAiXin_10_15(32);
                        GetAiXin_10_15(40);
                    }
                    else if (Set.setVal.WallNum_Height > 32)
                    {
                        GetAiXin_10_15(8);
                        GetAiXin_10_15(16);
                        GetAiXin_10_15(24);
                        GetAiXin_10_15(32);
                    }
                    else if (Set.setVal.WallNum_Height > 24)
                    {
                        GetAiXin_10_15(8);
                        GetAiXin_10_15(16);
                        GetAiXin_10_15(24);
                    }
                    else if (Set.setVal.WallNum_Height > 16)
                    {
                        GetAiXin_10_15(8);
                        GetAiXin_10_15(16);
                    }
                    else
                    {
                        GetAiXin_10_15(8);
                    }
                }
                else if (Set.setVal.Width <= 20)
                {
                    GetAiXin_Over15(0);
                    if (Set.setVal.WallNum_Height >= 65)
                    {
                        GetAiXin_Over15(65);
                        GetAiXin_Over15(52);
                        GetAiXin_Over15(39);
                        GetAiXin_Over15(26);
                        GetAiXin_Over15(13);
                    }
                    else if (Set.setVal.WallNum_Height > 52)
                    {
                        GetAiXin_Over15(52);
                        GetAiXin_Over15(39);
                        GetAiXin_Over15(26);
                        GetAiXin_Over15(13);
                    }
                    else if (Set.setVal.WallNum_Height > 39)
                    {
                        GetAiXin_Over15(39);
                        GetAiXin_Over15(26);
                        GetAiXin_Over15(13);
                    }
                    else if (Set.setVal.WallNum_Height > 26)
                    {
                        GetAiXin_Over15(26);
                        GetAiXin_Over15(13);
                    }
                    else if (Set.setVal.WallNum_Height > 13)
                    {
                        GetAiXin_Over15(13);
                    }
                }
                else
                {
                    GetAiXin_Over20(0);
                    if (Set.setVal.Height >= 65)
                    {
                        GetAiXin_Over20(65);
                        GetAiXin_Over20(52);
                        GetAiXin_Over20(39);
                        GetAiXin_Over20(26);
                        GetAiXin_Over20(13);
                    }
                    else if (Set.setVal.WallNum_Height > 52)
                    {
                        GetAiXin_Over20(52);
                        GetAiXin_Over20(39);
                        GetAiXin_Over20(26);
                        GetAiXin_Over20(13);
                    }
                    else if (Set.setVal.WallNum_Height > 39)
                    {
                        GetAiXin_Over20(39);
                        GetAiXin_Over20(26);
                        GetAiXin_Over20(13);
                    }
                    else if (Set.setVal.WallNum_Height > 26)
                    {
                        GetAiXin_Over20(26);
                        GetAiXin_Over20(13);
                    }
                    else if (Set.setVal.WallNum_Height > 13)
                    {
                        GetAiXin_Over20(13);
                    }
                }
                break;
            case 1://[表情]
                int r = 2;
                if (Set.setVal.Width > Set.setVal.WallNum_Height)
                {
                    r = (Set.setVal.WallNum_Height - 1) / 2;
                }
                else
                {
                    r = (Set.setVal.Width - 1) / 2;

                }
                ChangeYuan_Blue(Set.setVal.Width / 2, Set.setVal.Height + Set.setVal.WallNum_Height / 2, r);
                //   ChangeYuan_None(Set.setVal.Width / 2, Set.setVal.Height / 2, 2);

                break;
            case 2://笑脸
                int b = 4;
                if (Set.setVal.Width > Set.setVal.WallNum_Height)
                {
                    b = (Set.setVal.WallNum_Height - 1) / 2;
                }
                else
                {
                    b = (Set.setVal.Width - 1) / 2;

                }
                GetBiaoQing(Set.setVal.Width / 2, Set.setVal.Height + Set.setVal.WallNum_Height / 2, b, 0);
                break;
            case 3://钻石
                if (Set.setVal.Width < 10)
                {
                    GetDiamond_5_10(0);
                    if (Set.setVal.Height >= 30)
                    {
                        GetDiamond_5_10(6);
                        GetDiamond_5_10(12);
                        GetDiamond_5_10(18);
                        GetDiamond_5_10(24);
                        GetDiamond_5_10(30);
                    }
                    else if (Set.setVal.Height > 24)
                    {
                        GetDiamond_5_10(6);
                        GetDiamond_5_10(12);
                        GetDiamond_5_10(18);
                        GetDiamond_5_10(24);
                    }
                    else if (Set.setVal.Height > 18)
                    {
                        GetDiamond_5_10(6);
                        GetDiamond_5_10(12);
                        GetDiamond_5_10(18);
                    }
                    else if (Set.setVal.Height > 12)
                    {
                        GetDiamond_5_10(6);
                        GetDiamond_5_10(12);
                    }
                    else if (Set.setVal.Height > 6)
                    {
                        GetDiamond_5_10(6);
                    }
                }
                else if (Set.setVal.Width <= 15)
                {
                    GetDiamond_10_15(0);
                    if (Set.setVal.Height >= 40)
                    {
                        GetDiamond_10_15(8);
                        GetDiamond_10_15(16);
                        GetDiamond_10_15(24);
                        GetDiamond_10_15(32);
                        GetDiamond_10_15(40);
                    }
                    else if (Set.setVal.Height > 32)
                    {
                        GetDiamond_10_15(8);
                        GetDiamond_10_15(16);
                        GetDiamond_10_15(24);
                        GetDiamond_10_15(32);
                    }
                    else if (Set.setVal.Height > 24)
                    {
                        GetDiamond_10_15(8);
                        GetDiamond_10_15(16);
                        GetDiamond_10_15(24);
                    }
                    else if (Set.setVal.Height > 16)
                    {
                        GetDiamond_10_15(8);
                        GetDiamond_10_15(16);
                    }
                    else if (Set.setVal.Height > 8)
                    {
                        GetDiamond_10_15(8);
                    }
                }
                else if (Set.setVal.Width <= 20)
                {
                    GetDiamond_Over15(0);
                    if (Set.setVal.Height >= 65)
                    {
                        GetDiamond_Over15(65);
                        GetDiamond_Over15(52);
                        GetDiamond_Over15(39);
                        GetDiamond_Over15(26);
                        GetDiamond_Over15(13);
                    }
                    else if (Set.setVal.Height > 52)
                    {
                        GetDiamond_Over15(52);
                        GetDiamond_Over15(39);
                        GetDiamond_Over15(26);
                        GetDiamond_Over15(13);
                    }
                    else if (Set.setVal.Height > 39)
                    {
                        GetDiamond_Over15(39);
                        GetDiamond_Over15(26);
                        GetDiamond_Over15(13);
                    }
                    else if (Set.setVal.Height > 26)
                    {
                        GetDiamond_Over15(26);
                        GetDiamond_Over15(13);
                    }
                    else if (Set.setVal.Height > 13)
                    {
                        GetDiamond_Over15(13);
                    }
                }
                else
                {
                    GetDiamond_Over20(0);
                    if (Set.setVal.Height >= 65)
                    {
                        GetDiamond_Over20(65);
                        GetDiamond_Over20(52);
                        GetDiamond_Over20(39);
                        GetDiamond_Over20(26);
                        GetDiamond_Over20(13);
                    }
                    else if (Set.setVal.Height > 52)
                    {
                        GetDiamond_Over20(52);
                        GetDiamond_Over20(39);
                        GetDiamond_Over20(26);
                        GetDiamond_Over20(13);
                    }
                    else if (Set.setVal.Height > 39)
                    {
                        GetDiamond_Over20(39);
                        GetDiamond_Over20(26);
                        GetDiamond_Over20(13);
                    }
                    else if (Set.setVal.Height > 26)
                    {
                        GetDiamond_Over20(26);
                        GetDiamond_Over20(13);
                    }
                    else if (Set.setVal.Height > 13)
                    {
                        GetDiamond_Over20(13);
                    }
                }
                break;
            case 4://箭头
                if (Set.setVal.Width < 10)
                {
                    GetJianTou_5_10(0);
                    if (Set.setVal.Height >= 27)
                    {
                        GetJianTou_5_10(9);
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);
                        GetJianTou_5_10(12);
                        GetJianTou_5_10(15);
                        GetJianTou_5_10(18);
                        GetJianTou_5_10(21);
                        GetJianTou_5_10(24);
                        GetJianTou_5_10(27);
                    }
                    else if (Set.setVal.Height >= 24)
                    {
                        GetJianTou_5_10(9);
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);
                        GetJianTou_5_10(12);
                        GetJianTou_5_10(15);
                        GetJianTou_5_10(18);
                        GetJianTou_5_10(21);
                        GetJianTou_5_10(24);
                    }
                    else if (Set.setVal.Height >= 21)
                    {
                        GetJianTou_5_10(9);
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);
                        GetJianTou_5_10(12);
                        GetJianTou_5_10(15);
                        GetJianTou_5_10(18);
                        GetJianTou_5_10(21);
                    }
                    else if (Set.setVal.Height > 18)
                    {
                        GetJianTou_5_10(9);
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);
                        GetJianTou_5_10(12);
                        GetJianTou_5_10(15);
                        GetJianTou_5_10(18);
                    }
                    else if (Set.setVal.Height > 15)
                    {
                        GetJianTou_5_10(9);
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);
                        GetJianTou_5_10(12);
                        GetJianTou_5_10(15);
                    }
                    else if (Set.setVal.Height > 12)
                    {
                        GetJianTou_5_10(12);
                        GetJianTou_5_10(9);
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);
                    }
                    else if (Set.setVal.Height > 9)
                    {
                        GetJianTou_5_10(9);
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);
                    }
                    else if (Set.setVal.Height > 6)
                    {
                        GetJianTou_5_10(6);
                        GetJianTou_5_10(3);

                    }
                    else if (Set.setVal.Height > 3)
                    {
                        GetJianTou_5_10(3);
                    }
                }
                else if (Set.setVal.Width <= 15)
                {
                    GetJianTou_10_15(0);
                    if (Set.setVal.Height >= 27)
                    {
                        GetJianTou_10_15(9);
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);
                        GetJianTou_10_15(12);
                        GetJianTou_10_15(15);
                        GetJianTou_10_15(18);
                        GetJianTou_10_15(21);
                        GetJianTou_10_15(24);
                        GetJianTou_10_15(27);
                    }
                    else if (Set.setVal.Height >= 24)
                    {
                        GetJianTou_10_15(9);
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);
                        GetJianTou_10_15(12);
                        GetJianTou_10_15(15);
                        GetJianTou_10_15(18);
                        GetJianTou_10_15(21);
                        GetJianTou_10_15(24);
                    }
                    else if (Set.setVal.Height >= 21)
                    {
                        GetJianTou_10_15(9);
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);
                        GetJianTou_10_15(12);
                        GetJianTou_10_15(15);
                        GetJianTou_10_15(18);
                        GetJianTou_10_15(21);
                    }
                    else if (Set.setVal.Height > 18)
                    {
                        GetJianTou_10_15(9);
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);
                        GetJianTou_10_15(12);
                        GetJianTou_10_15(15);
                        GetJianTou_10_15(18);
                    }
                    else if (Set.setVal.Height > 15)
                    {
                        GetJianTou_10_15(9);
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);
                        GetJianTou_10_15(12);
                        GetJianTou_10_15(15);
                    }
                    else if (Set.setVal.Height > 12)
                    {
                        GetJianTou_10_15(12);
                        GetJianTou_10_15(9);
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);
                    }
                    else if (Set.setVal.Height > 9)
                    {
                        GetJianTou_10_15(9);
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);
                    }
                    else if (Set.setVal.Height > 6)
                    {
                        GetJianTou_10_15(6);
                        GetJianTou_10_15(3);

                    }
                    else if (Set.setVal.Height > 3)
                    {
                        GetJianTou_10_15(3);
                    }
                }
                else if (Set.setVal.Width <= 20)
                {
                    GetJianTou_Over15(0);
                    if (Set.setVal.Height >= 27)
                    {
                        GetJianTou_Over15(9);
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);
                        GetJianTou_Over15(12);
                        GetJianTou_Over15(15);
                        GetJianTou_Over15(18);
                        GetJianTou_Over15(21);
                        GetJianTou_Over15(24);
                        GetJianTou_Over15(27);
                    }
                    else if (Set.setVal.Height >= 24)
                    {
                        GetJianTou_Over15(9);
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);
                        GetJianTou_Over15(12);
                        GetJianTou_Over15(15);
                        GetJianTou_Over15(18);
                        GetJianTou_Over15(21);
                        GetJianTou_Over15(24);
                    }
                    else if (Set.setVal.Height >= 21)
                    {
                        GetJianTou_Over15(9);
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);
                        GetJianTou_Over15(12);
                        GetJianTou_Over15(15);
                        GetJianTou_Over15(18);
                        GetJianTou_Over15(21);
                    }
                    else if (Set.setVal.Height > 18)
                    {
                        GetJianTou_Over15(9);
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);
                        GetJianTou_Over15(12);
                        GetJianTou_Over15(15);
                        GetJianTou_Over15(18);
                    }
                    else if (Set.setVal.Height > 15)
                    {
                        GetJianTou_Over15(9);
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);
                        GetJianTou_Over15(12);
                        GetJianTou_Over15(15);
                    }
                    else if (Set.setVal.Height > 12)
                    {
                        GetJianTou_Over15(12);
                        GetJianTou_Over15(9);
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);
                    }
                    else if (Set.setVal.Height > 9)
                    {
                        GetJianTou_Over15(9);
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);
                    }
                    else if (Set.setVal.Height > 6)
                    {
                        GetJianTou_Over15(6);
                        GetJianTou_Over15(3);

                    }
                    else if (Set.setVal.Height > 3)
                    {
                        GetJianTou_Over15(3);
                    }
                }
                else
                {
                    GetJianTou_Over20(0);
                    if (Set.setVal.Height >= 27)
                    {
                        GetJianTou_Over20(9);
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);
                        GetJianTou_Over20(12);
                        GetJianTou_Over20(15);
                        GetJianTou_Over20(18);
                        GetJianTou_Over20(21);
                        GetJianTou_Over20(24);
                        GetJianTou_Over20(27);
                    }
                    else if (Set.setVal.Height >= 24)
                    {
                        GetJianTou_Over20(9);
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);
                        GetJianTou_Over20(12);
                        GetJianTou_Over20(15);
                        GetJianTou_Over20(18);
                        GetJianTou_Over20(21);
                        GetJianTou_Over20(24);
                    }
                    else if (Set.setVal.Height >= 21)
                    {
                        GetJianTou_Over20(9);
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);
                        GetJianTou_Over20(12);
                        GetJianTou_Over20(15);
                        GetJianTou_Over20(18);
                        GetJianTou_Over20(21);
                    }
                    else if (Set.setVal.Height > 18)
                    {
                        GetJianTou_Over20(9);
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);
                        GetJianTou_Over20(12);
                        GetJianTou_Over20(15);
                        GetJianTou_Over20(18);
                    }
                    else if (Set.setVal.Height > 15)
                    {
                        GetJianTou_Over20(9);
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);
                        GetJianTou_Over20(12);
                        GetJianTou_Over20(15);
                    }
                    else if (Set.setVal.Height > 12)
                    {
                        GetJianTou_Over20(12);
                        GetJianTou_Over20(9);
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);
                    }
                    else if (Set.setVal.Height > 9)
                    {
                        GetJianTou_Over20(9);
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);
                    }
                    else if (Set.setVal.Height > 6)
                    {
                        GetJianTou_Over20(6);
                        GetJianTou_Over20(3);

                    }
                    else if (Set.setVal.Height > 3)
                    {
                        GetJianTou_Over20(3);
                    }
                }
                break;
            case 5:
                for (int x = 0; x < Set.setVal.Width; x++)
                {
                    for (int y = Set.setVal.Height; y < (Set.setVal.Height + Set.setVal.WallNum_Height); y++)
                    {
                        if (y % 2 == 0 && x % 2 == 0) continue;
                        picId = x + Set.setVal.Width * y;
                        pointId = Framebuffer.tab_Mapping[picId];

                        {

                            {
                                
                                targetPos[TarPosCnt] = new Vector2(x, y);
                                bool_HaveFire[TarPosCnt] = false;
                                TarPosCnt++;

                            }
                        }

                    }
                }
                break;
            case 6:
                for (int x = 0; x < Set.setVal.Width; x++)
                {
                    for (int y = Set.setVal.Height; y < (Set.setVal.Height + Set.setVal.WallNum_Height); y++)

                    {
                        if (x % 2 == 0) continue;
                        picId = x + Set.setVal.Width * y;
                        pointId = Framebuffer.tab_Mapping[picId];

                        {

                            {
                                
                                targetPos[TarPosCnt] = new Vector2(x, y);
                                bool_HaveFire[TarPosCnt] = false;
                                TarPosCnt++;

                            }
                        }

                    }
                }
                break;
            case 7:
                for (int x = 0; x < Set.setVal.Width; x++)
                {
                    for (int y = Set.setVal.Height; y < (Set.setVal.Height + Set.setVal.WallNum_Height); y++)

                    {
                        if (y % 2 == 0 || x % 2 == 0) continue;
                        picId = x + Set.setVal.Width * y;
                        pointId = Framebuffer.tab_Mapping[picId];

                        {

                            {
                                
                                targetPos[TarPosCnt] = new Vector2(x, y);
                                bool_HaveFire[TarPosCnt] = false;
                                TarPosCnt++;

                            }
                        }

                    }
                }
                break;

        }
    
    }

    public void ChangeYuan_Blue(int _x, int _y, int r)//转场,绿色圆环或者红色
    {

        int picid = 0;

        float dis;
        for (int i = _x - r; i <= _x + r; i++)
        {
            for (int j = _y - r; j <= _y + r; j++)
            {

                dis = Mathf.Abs(Vector2.Distance(new Vector2(i, j), new Vector2(Set.setVal.Width / 2, Set.setVal.Height + Set.setVal.WallNum_Height / 2)));

                if (dis <= r && dis >= (2 * r / 3)) //dis >= r - w &&
                {

                    //  Vector2 pos = new Vector2(_x + j * widthOne, _y + i * widthOne);

                    if (i >= 0 && j >= 0)
                    {
                        if (i <= Set.setVal.Width - 1 && j <= Set.setVal.Height + Set.setVal.WallNum_Height - 1)
                        {

                            picid = i + Set.setVal.Width * j;
                            if (picid < Set.setVal.Width * (Set.setVal.Height + Set.setVal.WallNum_Height))
                            {
                                GameLedControl.gamePoint[Framebuffer.tab_Mapping[picid]].statue = enPointSta.Target;
                                targetPos[TarPosCnt] = new Vector2(i, j);
                                bool_HaveFire[TarPosCnt] = false;
                                TarPosCnt++;
                            }


                        }

                    }

                }
            }
        }
    }


}

