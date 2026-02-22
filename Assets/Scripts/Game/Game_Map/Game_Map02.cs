using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Map02 : MonoBehaviour
{





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

    int x, y;



    int Cnt;
    public float runTime = 0;
    float MaxrunTime = 0;
    float waitTime = 0;
    float angly = 0;
    bool haveClear = false;//是否已经过场完成
    public bool isCleaning = false;//是否过场
    public bool isClearAll = false;//是否清理了所有的点
    public int remainPoint = 1;

    public int MaxJieDuan = 4;

    public static Game_Map02 instance;


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

    void Clear()
    {
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            GameLedControl.gamePoint[i].statue = enPointSta.None;
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

        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {
                    GameLedControl.gamePoint[pointId].statue = enPointSta.None;
                    DrawPic.DrawRol(x, y, 1, 0, enPointSta.None);
                    Framebuffer.Update_PointColor(pointId, 0, enPointSta.None);
                }
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


        tarageNum = SettingInGame_02.instance.GetTarageNum() - TarageNum_now;


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


        MaxrunTime = 0.8f - 0.03f * Game02_Main.instance.Index_JieDuan;

        if (Game02_Main.instance.Index_JieDuan >= 0)
        {


            MaxrunTime = 0.8f - 0.05f * SettingInGame_02.instance.set_MoveSpeed[Game02_Main.instance.Index_JieDuan];
            if (MaxrunTime <= 0.05f)
            {
                MaxrunTime = 0.05f;
            }

        }
        //   tarageNum = Game02_Main.instance.player[0].playerUI.Ingame_Setting.GetTarageNum();
        TarageNum_now = 0;

        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                //if (Game02_Main.instance.BigGameLevel==3)
                //{
                //    if (Framebuffer.led[pointId].statue == enPointSta.Target)
                //    {
                //        // DrawPic.DrawRol(i, k, 1, 0x0000ff, enPointSta.Target);
                //        TarageNum_now++;
                //        //  Debug.LogError(" 目标点 " + i+"   "+k);

                //    }
                //}
                //else
                {
                    if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                    {
                        // DrawPic.DrawRol(i, k, 1, 0x0000ff, enPointSta.Target);
                        TarageNum_now++;
                        //  Debug.LogError(" 目标点 " + i+"   "+k);

                    }
                }
               

            }
        }
#if UNITY_EDITOR
        //        Debug.LogError(" 目标点 " + TarageNum_now);
#endif
        //if (TarageNum_now <= 3 && Game02_Main.instance.gameLevel != 25)
        //{
        //    SafeTime -= Time.deltaTime;
        //    if (SafeTime <= 0)
        //    {
        //        isClearAll = true;
        //    }
        //}

    }
    int randomX, randomY = 0;
    int yy_Num = 0;
    void GetRandom_Tarage()
    {


        //if (Set.setVal.Width > 25 || Set.setVal.Height > 25)
        //{
        //    tarageNum = 40;
        //}
        //else
        //{
        //    tarageNum = 20;
        //}



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

                    DrawPic.DrawPointId(a, 0x0000ff, enPointSta.Target);
                    GameLedControl.gamePoint[a].statue = enPointSta.Target;


                    tarageNum--;

                }



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
    public Vector2[] tarage_Pos = new Vector2[500];
    public void InitMap()
    {

        waitTime = 3;
        isClearAll = false;
        Cnt = 0;
        for (int i = 0; i < tarage_Pos.Length; i++)
        {
            tarage_Pos[i] = Vector2.one * -1;
        }
     

        for ( x = 0; x < Set.setVal.Width; x++)
        {
            for ( y = 0; y < Set.setVal.Height; y++)
            {
                picId = x + y * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
       
                {
                    if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                    {

                        tarage_Pos[Cnt] = new Vector2(x, y);
                        Cnt++;
                    }
                }

            }
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
                                    if (dis > r * (r - (Game02_Main.instance.Index_JieDuan + 1)) / r)
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


    // Update is called once per frame
    int ccnt = 0;

    void Update()
    {

        if (Game02_Main.instance == null)
        {
            return;
        }
        if (Game02_Main.instance.statue != en_Game00_Sta.Play)
        {
            return;
        }



        if (isCleaning || isClearAll)//)
        {


            return;
        }





        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    TarageNum_now = 0;
        //    isClearAll = true;
        //}
        runTime -= Time.deltaTime;
        if (Game02_Main.instance.BigGameLevel == 1)
        {
            //     DrawTarage_Pos();
        }

        if (Game02_Main.instance.BigGameLevel == 2)
        {
            switch (Game02_Main.instance.gameLevel - 20)
            {
                case 0:
                    Run_Map07();
                    break;
                case 1:
                    Run_Map05();
                    break;
                case 2:

                    angly += Time.deltaTime * 20;
                    if (Set.setVal.Width > Set.setVal.Height)
                    {
                        Run_Map08(Set.setVal.Width / 2, Set.setVal.Height / 2, angly, Set.setVal.Width / 2 + 1, 1, Game02_Main.instance.Index_JieDuan);

                    }
                    else
                    {
                        Run_Map08(Set.setVal.Width / 2, Set.setVal.Height / 2, angly, Set.setVal.Height / 2 + 1, 1, Game02_Main.instance.Index_JieDuan);

                    }
                    break;
                case 3:

                    Run_Map17(Game02_Main.instance.Index_JieDuan);
                    break;
                case 4:

                    Run_Map10(Game02_Main.instance.Index_JieDuan);
                    break;
                case 5:

                    Run_Map14(Game02_Main.instance.Index_JieDuan);
                    break;
                case 6:
                    Run_Map18(Game02_Main.instance.Index_JieDuan);

                    break;
                case 7:

                    Run_Map16(Game02_Main.instance.Index_JieDuan);
                    break;
                case 8:
                    Run_Map19(Game02_Main.instance.Index_JieDuan);

                    break;
                case 9:
                    Run_Map27(Game02_Main.instance.Index_JieDuan);

                    break;

            }

        }
        if (runTime <= 0)
        {
            runTime = 2f;

            //int aa = Framebuffer.tab_Mapping[ccnt];
            //Debug.LogError(aa);
            //GameLedControl.gamePoint[ccnt].statue = enPointSta.Rest;

            //ccnt++;
            if (Game02_Main.instance.BigGameLevel == 1)
            {

                // ClearMap_Tarage();
                //  MoveTarage();
            }

        }


        isPass();

    }
    void MoveTarage()
    {
        for (int i = 0; i < tarage_Pos.Length; i++)
        {
            if (tarage_Pos[i].x >= 0)
            {
                int a = Random.Range(0, 100);
                if (a <= 5)
                {
                    a = Random.Range(0, 4);
                    switch (a)
                    {
                        case 0:
                            if (tarage_Pos[i].y + 1 < Set.setVal.Height)
                            {
                                tarage_Pos[i].y += 1;
                            }
                            break;

                        case 1:
                            if (tarage_Pos[i].y - 1 > 0)
                            {
                                tarage_Pos[i].y -= 1;
                            }
                            break;
                        case 2:
                            if (tarage_Pos[i].x - 1 > 0)
                            {
                                tarage_Pos[i].x -= 1;
                            }
                            break;
                        case 3:
                            if (tarage_Pos[i].x + 1 < Set.setVal.Width)
                            {
                                tarage_Pos[i].x += 1;
                            }
                            break;
                    }
                }
                //  DrawPic.DrawRol((int)tarage_Pos[i].x, (int)tarage_Pos[i].y, 1, 0x0000ff, enPointSta.Target);
                // ChangeLed_Sta(x, y, enPointSta.Target);
            }

        }
    }
    public void DrawTarage_Pos()
    {


        for (int i = 0; i < tarage_Pos.Length; i++)
        {
            if (tarage_Pos[i].x >= 0)
            {

                DrawPic.DrawRol((int)tarage_Pos[i].x, (int)tarage_Pos[i].y, 1, 0x0000ff, enPointSta.Target);
                ChangeLed_Sta(x, y, enPointSta.Target);
            }

        }
    }
    void InitMap_27(int jieduan)
    {

        Clear();
        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;

        isClearAll = false;

        x = 0;
        y = 0;


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
    void Run_Map19(int JieDuan)
    {
        Update_TaragePos();

        ledSturts_Group[0].Update_Game19_M(JieDuan);
        if (runTime <= 0)
        {
            runTime = MaxrunTime;

            ledSturts_Group[0].Moving_Game19_M();

        }

    }
    void Run_Map27(int JieDuan)
    {
        Update_TaragePos();

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
    void Run_Map14(int JieDuan)
    {
        Update_TaragePos();

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
    void Update_TaragePos()
    {
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picId = i + k * Set.setVal.Width;
                pointId = Framebuffer.tab_Mapping[picId];
                if (GameLedControl.gamePoint[pointId].statue == enPointSta.Target)
                {
                    DrawPic.DrawRol(i, k, 1, 0x0000ff, enPointSta.Target);
                }
            }
        }
    }
    void Run_Map07()
    {

        Update_TaragePos();

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
        Update_TaragePos();

        angle = angle * Mathf.PI / 180;

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
            case 2:
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
            case 20:

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

            DrawPic.DrawCol(Set.setVal.Width / 2, 0, Set.setVal.Height, 0x00ff00, enPointSta.Rest);
            Update_TaragePos();
            for (int k = 0; k < ledSturts_Group[0].x; k++)
            {

                DrawPic.DrawCol(k, ledSturts_Group[0].y, Set.setVal.Height, 0xff0000, enPointSta.Die);
                DrawPic.DrawCol(Set.setVal.Width - k, ledSturts_Group[0].y, Set.setVal.Height, 0xff0000, enPointSta.Die);

            }

        }

        ClearMap();
    }
    void Run_Map10(int JieDuan)
    {

        //     ledSturts_Group[0].UpdatePic_FangKuang(new Vector2(Set.setVal.Width / 2 - 1, Set.setVal.Height / 2), ledSturts_Group[0].x);
        Update_TaragePos();

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
    void Run_Map15(int JieDuan)
    {
        runTime -= Time.deltaTime;
        Update_TaragePos();

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


        ClearMap();
        Update_TaragePos();

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
                    break;
                case 20:

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

        ClearMap();
        Update_TaragePos();

        ledSturts_Group[0].UpdateSnake_17_RightKLong();
        ledSturts_Group[1].UpdateSnake_17_RightKLong();
        ledSturts_Group[2].UpdateSnake_17_RightKLong();
        if (JieDuan == 4)
        {
            for (int i = 0; i < Set.setVal.Width / 6; i++)
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
                    DrawPic.DrawPointId(pointId, 0xff0000, enPointSta.Die);

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

        ClearMap();
        Update_TaragePos();

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

    void isPass()
    {

        if (Game02_Main.instance.statue != en_Game00_Sta.Play)
        {
            return;
        }
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }
        if (true)//Game02_Main.instance.BigGameLevel == 0 || Game02_Main.instance.BigGameLevel == 2
        {
            Get_NowTarage();

            if (TarageNum_now <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError("没有目标点了");
#endif

                isClearAll = true;
                return;
            }
#if UNITY_EDITOR
            else {
                Debug.LogError("有目标点了"+ TarageNum_now);
            }
#endif


        }

   
    }
    void InitMap_05(int jieduan)
    {
        Clear();

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;

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

        for (int k = 0; k < Set.setVal.Height; k++)
        {
            picId = Set.setVal.Width * k + Set.setVal.Width / 2;
            pointId = Framebuffer.tab_Mapping[picId];
            GameLedControl.gamePoint[pointId].statue = enPointSta.Rest;
        }

        DrawPic.DrawCol(Set.setVal.Width / 2, 0, Set.setVal.Height, 0x00ff00, enPointSta.Rest);
        GetRandom_Tarage();
    }
    void InitMap_07(int jieduan)
    {
        Clear();
        MaxJieDuan = 3;

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;

        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);

        //DrawSafePlace(1);//玩家替换
        GetRandom_Tarage();
    }
    void InitMap_17(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;
        Game00_Main.instance.isClearTarage = false;
        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;

        if (Game00_Main.instance.player[0].playerUI.Ingame_Setting != null)
        {
            if (SettingInGame_01.instance.GetReMainNum(jieduan) > 0)
            {
                remainPoint = SettingInGame_01.instance.GetReMainNum(jieduan);

            }
        }

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
    void InitMap_08(int jieduan)
    {

        angly = 0;

        Clear();

        MaxJieDuan = 4;

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;
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

        //   
        GetRandom_Tarage();
    }
    void InitMap_10()
    {

        Clear();


        MaxJieDuan = 4;

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;

        }
        ledSturts_Group[0].enabled = true;

        ledSturts_Group[0].dir = 0;

        ledSturts_Group[0].Init(false, this);



        GetRandom_Tarage();
    }
    void InitMap_15(int jieduan)
    {
        Clear();
        MaxJieDuan = 4;

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;

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
    void InitMap_14()
    {
        Clear();

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;

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
    void InitMap_16(int jieduan)
    {
        Clear();

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;

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
    void InitMap_19(int jieduan)
    {
        Clear();

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;


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
    void InitMap_18(int jieduan)
    {
        Clear();

        tarageNum = Set.setVal.Width * Set.setVal.Height / 4;


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
    public void InitMap(int ID)
    {

        Debug.LogError(11);
        for (int i = 0; i < GameLedControl.gamePoint.Length; i++)
        {
            GameLedControl.gamePoint[i].Color = 0;
        }
        Clear();
        for (int i = 0; i < ledSturts_Group.Length; i++)
        {
            ledSturts_Group[i].enabled = false;
        }
        switch (ID)
        {
            case 0:

                InitMap_07(Game02_Main.instance.Index_JieDuan);
                break;
            case 1:

                InitMap_05(Game02_Main.instance.Index_JieDuan);

                break;
            case 2:

                InitMap_08(Game02_Main.instance.Index_JieDuan);

                break;
            case 3:

                InitMap_17(Game02_Main.instance.Index_JieDuan);

                break;
            case 4:
                InitMap_10();
                break;
            case 5:
                InitMap_14();
                break;
            case 6:
                InitMap_18(Game02_Main.instance.Index_JieDuan);
                break;
            case 7:
                InitMap_16(Game02_Main.instance.Index_JieDuan);
                break;
            case 8:
                InitMap_19(Game02_Main.instance.Index_JieDuan);
                break;
            case 9:
                InitMap_27(Game02_Main.instance.Index_JieDuan);
                break;
            case 10:
                //  InitMap_10();
                break;

        }



    }
}

