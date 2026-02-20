using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserPPD.Core;

public class LedAnim_Struts_06 : MonoBehaviour
{

    public enum en_MoveWay
    {
        Up = 0,
        Down,
        Left,
        Right,
        LU,
        RU,
        LD,
        RD

        //可能还有打斜
    }


    public en_MoveWay moveWay;
    public int x;
    public int startx;//第六关用
    public int x1;//第六关用
    public int y;
    public int y1;//第六关用
    public int dir = 0;
    int picID = 0;
    int pointID = 0;
    public Vector2 StartPos;



    public Vector2[] SnakePos = new Vector2[300];
    public Vector2[] SnakePos_17_Long = new Vector2[40];
    public Vector2 Snake_TaragePos;
    public int Snake_length_17 = 0;
    int sanke_Length = 0;


    public int TopPos;
    public int DownPos;
    public int LeftPos;
    public int RightPos;
    Vector2 Tank_FirePos;
    float attackCD = 3;
    float attackMaxCD = 3;
    float dinstance = 3111;
    bool isLeft = false;
    bool haveFire = false;
    public GameObject buttle;

    public Vector2[] pos_Now = new Vector2[4];
    public Vector2[] pos_Now1 = new Vector2[25];//5x5先用着

    ILedMapCallback ledAnim;

    public bool isTank = false;



    public void UpdatePic_FangKuang(Vector2 _StartPos, int width)
    {
        bool pass0, pass1;
        pass0 = true;
        pass1 = true;
        int xxx = (int)_StartPos.x - width;
        int yyy = (int)_StartPos.y - width;
        if (Set.setVal.Width > Set.setVal.Height)
        {
            if (_StartPos.x - width >= 0 && _StartPos.x + width < Set.setVal.Width)
            {
                DrawPic.DrawRol((int)_StartPos.x - width, (int)_StartPos.y - width, 1 + 2 * width, 0xff0000, enPointSta.Die);
                DrawPic.DrawRol((int)_StartPos.x - width, (int)_StartPos.y + width, 1 + 2 * width, 0xff0000, enPointSta.Die);
                pass0 = false;
            }
        }
        else
        {
            if (_StartPos.x - width >= 0 && _StartPos.x + width < Set.setVal.Width)
            {
                DrawPic.DrawRol((int)_StartPos.x - width, (int)_StartPos.y - width, 1 + 2 * width, 0xff0000, enPointSta.Die);
                DrawPic.DrawRol((int)_StartPos.x - width, (int)_StartPos.y + width, 1 + 2 * width, 0xff0000, enPointSta.Die);
                pass0 = false;
            }
            else
            {
                if (_StartPos.y + width < Set.setVal.Height)
                {
                    DrawPic.DrawRol(0, (int)_StartPos.y - width, Set.setVal.Width, 0xff0000, enPointSta.Die);
                    DrawPic.DrawRol(0, (int)_StartPos.y + width, Set.setVal.Width, 0xff0000, enPointSta.Die);
                    pass0 = false;
                }

            }

        }

        if (Set.setVal.Width > Set.setVal.Height)
        {
            if (_StartPos.x - width >= 0 && _StartPos.x + width < Set.setVal.Width)
            {


                if (xxx < 0)
                {
                    xxx = 0;
                }
                if (yyy < 0)
                {
                    yyy = 0;
                }
                DrawPic.DrawCol(xxx, yyy, 1 + 2 * width, 0xff0000, enPointSta.Die);

                xxx = (int)_StartPos.x + width;
                yyy = (int)_StartPos.y - width;
                if (xxx >= Set.setVal.Width)
                {
                    xxx = Set.setVal.Width - 1;
                }
                if (yyy < 0)
                {
                    yyy = 0;
                }
                int bu = 1 + 2 * width;
                if (_StartPos.y + 1 + width >= Set.setVal.Height)
                {
                    bu = Set.setVal.Height - 1;
                }
                DrawPic.DrawCol((int)_StartPos.x + width, yyy, bu, 0xff0000, enPointSta.Die);
                pass1 = false;
            }
        }
        else
        {
            if (_StartPos.x - width >= 0 && _StartPos.x + width < Set.setVal.Width)
            {


                if (xxx < 0)
                {
                    xxx = 0;
                }
                if (yyy < 0)
                {
                    yyy = 0;
                }
                DrawPic.DrawCol(xxx, yyy, 1 + 2 * width, 0xff0000, enPointSta.Die);

                xxx = (int)_StartPos.x + width;
                yyy = (int)_StartPos.y - width;
                if (xxx >= Set.setVal.Width - 1)
                {
                    xxx = Set.setVal.Width - 1;
                }
                if (yyy < 0)
                {
                    yyy = 0;
                }
                int bu = 1 + 2 * width;
                if (_StartPos.y + 1 + width >= Set.setVal.Height)
                {
                    bu = Set.setVal.Height - 1;
                }
                DrawPic.DrawCol((int)_StartPos.x + width, yyy, bu, 0xff0000, enPointSta.Die);
                pass1 = false;
            }
        }

        if (pass0 && pass1)
        {
            x = 0; y = 0;
        }

    }
    public void Move_FangKuai_13(int length)
    {

        switch (dir)
        {
            case 0:
                if (x < Set.setVal.Width - length)
                {
                    x++;
                }
                else
                {
                    dir = 1;
                }
                break;

            case 1:
                if (y < Set.setVal.Height - length)
                {
                    y++;
                }
                else
                {
                    dir = 2;
                }
                break;
            case 2:
                if (x > 0)
                {
                    x--;
                }
                else
                {
                    dir = 3;
                }
                break;

            case 3:
                if (y > 0)
                {
                    y--;
                }
                else
                {
                    dir = 0;
                }
                break;

        }
    }
    int cntX = -3;
    int cntY = -3;

    public void BeTouched_X(int x_X, int y_Y)//id是物理的
    {
        // Debug.LogError("x_X  "+ x_X+ "y_Y  "+ y_Y);


        for (int i = 0; i < X_length; i++)
        {
            // Debug.LogError("x_  "+ (int)X_Pos[i].x + "y_  "+ (int)X_Pos[i].y);


            if (x_X + 1 == (int)X_Pos[i].x && y_Y == (int)X_Pos[i].y)//
            {
                picID = (int)X_Pos[i].x + (int)X_Pos[i].y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];

                X_Pos[i].y = -1;

                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                bool_X_Pos[i] = false;
                //break;
            }
        }

    }
    public void UpdateFangKuai_X(int jieduan)//第21关
    {



        for (int i = 0; i < X_length; i++)
        {
            if (bool_X_Pos[i])
            {
                ledAnim.ChangeLed_Sta((int)X_Pos[i].x, (int)X_Pos[i].y, enPointSta.Target);

            }
            if (X_Pos[i].y <= 1 || X_Pos[i].y >= Set.setVal.Height - 2)
            {
                bool_X_Pos[i] = false;
            }

        }
        for (int i = 0; i < X_length; i++)
        {
            if (bool_X_Pos[i])
            {

                return;
            }

        }





    }
    public void UpdatePic_DuanLie()
    {

        if (dir == 0)
        {
            for (int i = 0; i < Set.setVal.Width; i++)//x
                                                      //{



                //    cntX++;




                //    if (cntX >= 0 && cntX <= 1)
                //    {
                //        continue;
                //    }
                //    if (cntX > 1)
                //    {

                //        cntX = -2;


                //    }

                //    picID = i + y * Set.setVal.Width;
                //    pointID = Framebuffer.tab_Mapping[picID];
                //    DrawPic.DrawPointId(pointID, 0xff0000, enPointSta.Die);



                //}
                DrawPic.DrawRol(1, y, Set.setVal.Width, 0xff0000, enPointSta.Die);


        }
        else
        {
            for (int i = 0; i < Set.setVal.Height; i++)//y
            {



                cntX++;




                if (cntX >= 0 && cntX <= 1)
                {
                    continue;
                }
                if (cntX > 1)
                {

                    cntX = -2;


                }

                DrawPic.DrawCol(x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);


                // DrawPic.DrawRol(x, i, 1, 0xff0000, enPointSta.Die);


            }
        }

    }
    public void UpdatePic_DuanLie_Full()
    {
        if (dir == 0)
        {
            DrawPic.DrawRol(0, y, Set.setVal.Width, 0xff0000, enPointSta.Die);
            //for (int i = 0; i < Set.setVal.Width; i++)
            //{

            //    picID = i + y * Set.setVal.Width;
            //    pointID = Framebuffer.tab_Mapping[picID];
            //    //if (GameLedControl.gamePoint[pointID].statue != enPointSta.Rest)
            //    //{
            //    //    GameLedControl.gamePoint[pointID].statue = enPointSta.Die;

            //    //}
            //    DrawPic.DrawPointId(pointID, 0xff0000, enPointSta.Die);

            //}
        }
        else
        {
            for (int i = 0; i < Set.setVal.Height; i++)
            {

                picID = x + i * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];
                //if (GameLedControl.gamePoint[pointID].statue != enPointSta.Rest)
                //{
                //    GameLedControl.gamePoint[pointID].statue = enPointSta.Die;

                //}
                DrawPic.DrawPointId(pointID, 0xff0000, enPointSta.Die);

            }
            DrawPic.DrawCol(x, 0, Set.setVal.Height, 0xff0000, enPointSta.Die);

        }
    }

    int DDR = 0;

    public void UpdatePic_BoLang()
    {
        int y_now = 0;
        for (int i = 0; i < Set.setVal.Width; i++)
        {

            if (DDR == 0)
            {

                if (y_now < Set.setVal.Height)
                {
                    y_now++;

                }
                else
                {

                    DDR = 1;
                }
            }
            else
            {

                if (y_now > 0)
                {
                    y_now--;

                }
                else
                {
                    y_now = 0;
                    dir = 0;
                }
            }



            picID = i + Set.setVal.Width * y_now;
            pointID = Framebuffer.tab_Mapping[picID];
            GameLedControl.gamePoint[pointID].statue = enPointSta.Die;
            DrawPic.DrawPointId(pointID, 0xff0000, enPointSta.Die);

        }
    }

    public void UpdatePic_FangKuai_27()
    {
        DrawPic.DrawRol(x, y, 2, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(x, y + 1, 2, 0xff0000, enPointSta.Die);
    }
    public void UpdatePic_FangKuai_13()
    {


        for (int i = (int)x; i < (int)x + 3; i++)
        {
            for (int k = (int)y; k < (int)y + 3; k++)
            {
                DrawPic.DrawRol(i, k, 1, 0xff0000, enPointSta.Die);
                //picID = i + k * Set.setVal.Width;
                //pointID = Framebuffer.tab_Mapping[picID];
                //if (pointID >= 0 && pointID < Set.setVal.Width * Set.setVal.Height)
                //{
                //    GameLedControl.gamePoint[pointID].statue = enPointSta.Die;

                //}




            }
        }

        LeftPos = x;
        RightPos = x + 3;
        TopPos = y + 3;
        DownPos = y;

    }
    public void UpdatePic_FangKuai_15()//第四阶段,两个方块
    {


        for (int i = (int)x; i < (int)x + 2; i++)
        {
            for (int k = (int)y; k < (int)y + 2; k++)
            {
                picID = i + k * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];
                if (pointID >= 0 && pointID < Set.setVal.Width * Set.setVal.Height)
                {
                    DrawPic.DrawRol(i, k, 1, 0xff0000, enPointSta.Die);

                }




            }
        }

        LeftPos = x;
        RightPos = x + 3;
        TopPos = y + 3;
        DownPos = y;

    }
    int lenx = 0;
    int leny = 0;
    public void UpdatePic_FangKuai_18(int jieduan)
    {

        switch (jieduan)
        {
            case 0:
                lenx = Set.setVal.Width / 2;
                leny = Set.setVal.Height / 2;
                for (int i = x; i < x + lenx; i++)
                {
                    DrawPic.DrawCol(i, y, leny, 0xff0000, enPointSta.Die);

                }

                break;
            case 1:
                lenx = Set.setVal.Width / 2;
                leny = Set.setVal.Height / 2;
                for (int i = x; i < x + lenx; i++)
                {
                    DrawPic.DrawCol(i, y, leny, 0xff0000, enPointSta.Die);

                }
                break;
            case 2:
                lenx = Set.setVal.Width / 2;
                leny = Set.setVal.Height;
                int xx = x;
                //for (int i = xx; i < x+lenx; ++)
                //{
                //    DrawPic.DrawCol(i, 0, leny, 0xff0000, enPointSta.Die);

                //}
                break;
        }



        LeftPos = x;
        RightPos = x + lenx;
        TopPos = y + leny;
        DownPos = y;

    }
    public void MoveFangKuai_18(int JieDuan)
    {
        switch (JieDuan)
        {
            case 0:

                switch (dir)
                {
                    case 0:

                        if (TopPos < Set.setVal.Height)
                        {

                            y++;
                        }
                        else
                        {
                            dir = 3;
                        }
                        break;
                    case 1:
                        if (DownPos > 0)
                        {

                            y--;

                        }
                        else
                        {
                            dir = 2;
                        }
                        break;
                    case 2:
                        if (LeftPos > 0)
                        {

                            x--;

                        }
                        else
                        {
                            dir = 0;
                        }
                        break;
                    case 3:
                        if (RightPos < Set.setVal.Width)
                        {

                            x++;
                        }
                        else
                        {
                            dir = 1;
                        }
                        break;
                }
                break;
            case 1:
                switch (dir)
                {
                    case 0:

                        if (TopPos < Set.setVal.Height)
                        {

                            y++;
                        }
                        else
                        {
                            dir = 1;
                        }
                        break;
                    case 1:
                        if (DownPos > 0)
                        {

                            y--;

                        }
                        else
                        {
                            dir = 0;
                        }
                        break;

                }

                break;
            case 2:
                switch (dir)
                {
                    case 0:

                        if (x < Set.setVal.Width / 2)
                        {

                            x++;
                        }
                        else
                        {
                            dir = 1;
                        }
                        break;
                    case 1:

                        if (x > Set.setVal.Width / 2)
                        {

                            x--;
                        }
                        else
                        {
                            dir = 0;
                        }
                        break;

                }
                break;
            case 3:
                switch (dir)
                {
                    case 0:

                        if (SnakePos_17_Long[0].y < Set.setVal.Height / 2 + 1)
                        {

                            SnakePos_17_Long[0].y++;
                        }
                        else
                        {
                            dir = 3;
                        }
                        break;
                    case 1:
                        if (SnakePos_17_Long[0].y > Set.setVal.Height / 2 - 1)
                        {

                            SnakePos_17_Long[0].y--;

                        }
                        else
                        {
                            dir = 2;
                        }
                        break;
                    case 2:
                        if (SnakePos_17_Long[0].x > Set.setVal.Width / 2 - 1)
                        {

                            SnakePos_17_Long[0].x--;

                        }
                        else
                        {
                            dir = 0;
                        }
                        break;
                    case 3:
                        if (SnakePos_17_Long[0].x < Set.setVal.Width / 2 + 1)
                        {

                            SnakePos_17_Long[0].x++;
                        }
                        else
                        {
                            dir = 1;
                        }
                        break;
                }
                break;
        }

    }
    public void MoveFangKuai_27()
    {
        switch (dir)
        {
            case 0:
                if (x < Set.setVal.Width - 3)
                {
                    x++;
                }
                else
                {
                    dir = 1;
                }
                break;
            case 1:
                if (y < Set.setVal.Height - 3)
                {
                    y++;
                }
                else
                {
                    dir = 2;
                }
                break;
            case 2:
                if (x > 1)
                {
                    x--;
                }
                else
                {
                    dir = 3;
                }
                break;
            case 3:
                if (y > 1)
                {
                    y--;
                }
                else
                {
                    dir = 0;
                }
                break;
        }
    }

    public void MoveFangKuai_29()
    {
        switch (dir)
        {
            case 0:
                if (x < Set.setVal.Width - 2)
                {
                    x++;
                }
                else
                {
                    dir = 1;
                }
                break;
            case 1:
                if (y < Set.setVal.Height - 2)
                {
                    y++;
                }
                else
                {
                    dir = 2;
                }
                break;
            case 2:
                if (x > 0)
                {
                    x--;
                }
                else
                {
                    dir = 3;
                }
                break;
            case 3:
                if (y > 0)
                {
                    y--;
                }
                else
                {
                    dir = 0;
                }
                break;
        }
    }
    public void UpdatedateYuan_29()

    {


        DrawPic.DrawRol(x, y, Set.setVal.Width / 4, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(1 + x, y + 1, Set.setVal.Width / 4 - 1, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(x + 1, y - 1, Set.setVal.Width / 4 - 1, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(x + 2, y - 2, Set.setVal.Width / 4 - 2, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(x + 2, y + 2, Set.setVal.Width / 4 - 2, 0xff0000, enPointSta.Die);


    }
    public void MoveYuan_29()
    {
        if (dir == 0)
        {
            if (x < Set.setVal.Width * 3 / 4)
            {
                x++;
            }
            else
            {
                dir = 1;
            }

        }
        else
        {
            if (x > 0)
            {
                x--;
            }
            else
            {
                dir = 0;
            }

        }
    }
    int winth_28 = (Set.setVal.Width - 4) / 2;

    public void UpdateFangKuai_28()
    {
        winth_28 = (Set.setVal.Width - 4) / 2;

        for (int i = 0; i < winth_28; i++)
        {
            DrawPic.DrawCol(x + i, y, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);

        }
    }
    public void MoveFangKuai_28()
    {
        winth_28 = (Set.setVal.Width - 4) / 2;

        switch (dir)
        {
            case 0:
                if (x + winth_28 < Set.setVal.Width - 2)
                {
                    x++;
                }
                else
                {
                    dir = 1;
                }
                break;
            case 1:
                if (y < Set.setVal.Height / 2)
                {
                    y++;
                }
                else
                {
                    dir = 2;
                }
                break;
            case 2:
                if (x > 2)

                {
                    x--;
                }
                else
                {
                    dir = 3;
                }
                break;
            case 3:
                if (y > 0)
                {
                    y--;
                }
                else
                {
                    dir = 0;
                }
                break;
        }
    }
    public void UpdateFangKuai_28_Green(int id)
    {
        if (id == 0)
        {
            DrawPic.DrawCol(Set.setVal.Width / 4, y, Set.setVal.Height / 4, 0x00ff00, enPointSta.Rest);
            DrawPic.DrawCol(Set.setVal.Width / 4 + 1, y, Set.setVal.Height / 4, 0x00ff00, enPointSta.Rest);

        }
        else
        {
            DrawPic.DrawCol(Set.setVal.Width * 3 / 4, y, Set.setVal.Height / 4, 0x00ff00, enPointSta.Rest);
            DrawPic.DrawCol(Set.setVal.Width * 3 / 4 + 1, y, Set.setVal.Height / 4, 0x00ff00, enPointSta.Rest);

        }
    }
    public void MoveFangKuai_28_Green()
    {
        if (dir == 0)
        {

            if (y < Set.setVal.Height * 3 / 4)
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
    public void UpdateFangKuai_27_Red()
    {
        DrawPic.DrawRol(x, y, 1, 0xff0000, enPointSta.Die);
    }
    public void UpdateFangKuai_29()
    {
        DrawPic.DrawRol(x, y, 2, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(x, y + 1, 2, 0xff0000, enPointSta.Die);
    }
    public void MoveFangKuai_27_RedDoor()
    {
        if (dir == 0)
        {
            if (x < Set.setVal.Width / 4)
            {
                x++;
            }
            else
            {
                dir = 1;
            }

        }
        else
        {
            if (x > 0)
            {
                x--;
            }
            else
            {
                dir = 0;
            }

        }

    }
    public void UpdateFangKuai_27_RedDoor(int id)
    {
        switch (id)
        {
            case 0:
                for (int i = 0; i < x; i++)
                {
                    DrawPic.DrawCol(i, 0, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);
                    DrawPic.DrawCol(Set.setVal.Width / 2 - 1 - i, 0, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);

                }

                break;
            case 1:
                for (int i = 0; i < x; i++)
                {
                    DrawPic.DrawCol(Set.setVal.Width / 2 + i, 0, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);
                    DrawPic.DrawCol(Set.setVal.Width - 1 - i, 0, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);

                }
                break;
            case 2:
                for (int i = 0; i < x; i++)
                {
                    DrawPic.DrawCol(i, Set.setVal.Height / 2, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);
                    DrawPic.DrawCol(Set.setVal.Width / 2 - 1 - i, Set.setVal.Height / 2, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);

                }

                break;
            case 3:
                for (int i = 0; i < x; i++)
                {
                    DrawPic.DrawCol(Set.setVal.Width / 2 + i, Set.setVal.Height / 2, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);
                    DrawPic.DrawCol(Set.setVal.Width - 1 - i, Set.setVal.Height / 2, Set.setVal.Height / 2, 0xff0000, enPointSta.Die);

                }

                break;
        }
    }
    public void MoveFangKuai_27_Red()
    {
        switch (dir)
        {
            case 0:
                if (x < Set.setVal.Width - 1)
                {
                    x++;
                }
                else
                {
                    dir = 1;
                }
                break;
            case 1:
                if (y < Set.setVal.Height - 1)
                {
                    y++;
                }
                else
                {
                    dir = 2;
                }
                break;
            case 2:
                if (x > 0)
                {
                    x--;
                }
                else
                {
                    dir = 3;
                }
                break;
            case 3:
                if (y > 0)
                {
                    y--;
                }
                else
                {
                    dir = 0;
                }
                break;
        }
    }
    public void MoveFangKuai_24(int JieDuan)
    {
        switch (JieDuan)
        {
            case 0:

                for (int i = 0; i < fangkuai_24_Pos.Length; i++)
                {
                    if (fangkuai_24_Pos[i].x >= 0)
                    {
                        fangkuai_24_Pos[i].y--;

                    }
                    if (fangkuai_24_Pos[i].y < -2)
                    {
                        fangkuai_24_Pos[i] = Vector2.one * -1;
                    }
                }
                break;
            case 1:
                for (int i = 0; i < fangkuai_24_Pos.Length; i++)
                {
                    if (fangkuai_24_Pos[i].x >= 0)
                    {
                        fangkuai_24_Pos[i].y--;

                    }
                    if (fangkuai_24_Pos[i].y < -1)
                    {
                        fangkuai_24_Pos[i] = Vector2.one * -1;
                    }
                }

                break;
            case 2:
                for (int i = 0; i < fangkuai_24_Pos.Length; i++)
                {
                    if (fangkuai_24_Pos[i].x >= 0)
                    {
                        fangkuai_24_Pos[i].y--;

                    }
                    if (fangkuai_24_Pos[i].y < 0)
                    {
                        fangkuai_24_Pos[i] = Vector2.one * -1;
                    }
                }
                break;
            case 3:
                switch (dir)
                {
                    case 0:

                        if (SnakePos_17_Long[0].y < Set.setVal.Height / 2 + 1)
                        {

                            SnakePos_17_Long[0].y++;
                        }
                        else
                        {
                            dir = 3;
                        }
                        break;
                    case 1:
                        if (SnakePos_17_Long[0].y > Set.setVal.Height / 2 - 1)
                        {

                            SnakePos_17_Long[0].y--;

                        }
                        else
                        {
                            dir = 2;
                        }
                        break;
                    case 2:
                        if (SnakePos_17_Long[0].x > Set.setVal.Width / 2 - 1)
                        {

                            SnakePos_17_Long[0].x--;

                        }
                        else
                        {
                            dir = 0;
                        }
                        break;
                    case 3:
                        if (SnakePos_17_Long[0].x < Set.setVal.Width / 2 + 1)
                        {

                            SnakePos_17_Long[0].x++;
                        }
                        else
                        {
                            dir = 1;
                        }
                        break;
                }
                break;
        }

    }

    public void UpdatePic_FangKuai(Vector2 _StartPos, int width, int height)
    {
        StartPos = _StartPos;
        for (int i = 0; i < pos_Now1.Length; i++)
        {
            pos_Now1[i] = Vector2.one * -1;
        }
        for (int i = 0; i < width; i++)
        {

            for (int k = 0; k < height; k++)
            {


                picID = (int)(StartPos.x + i) + (int)((StartPos.y + k) * Set.setVal.Width);
                pointID = Framebuffer.tab_Mapping[picID];
                if (pointID >= 0 && pointID < Set.setVal.Width * Set.setVal.Height)
                {
                    GameLedControl.gamePoint[pointID].statue = enPointSta.Rest;

                }
            }
        }
        for (int i = 0; i < width; i++)
        {
            picID = (int)(StartPos.x + i) + (int)((StartPos.y) * Set.setVal.Width);
            pointID = Framebuffer.tab_Mapping[picID];
            if (pointID >= 0 && pointID < Set.setVal.Width * Set.setVal.Height)
            {
                GameLedControl.gamePoint[pointID].statue = enPointSta.Die;

            }
            picID = (int)(StartPos.x + i) + (int)((StartPos.y + height - 1) * Set.setVal.Width);
            pointID = Framebuffer.tab_Mapping[picID];
            if (pointID >= 0 && pointID < Set.setVal.Width * Set.setVal.Height)
            {
                GameLedControl.gamePoint[pointID].statue = enPointSta.Die;

            }

        }
        for (int i = 0; i < height; i++)
        {
            picID = (int)(StartPos.x) + (int)((StartPos.y + i) * Set.setVal.Width);
            pointID = Framebuffer.tab_Mapping[picID];
            if (pointID >= 0 && pointID < Set.setVal.Width * Set.setVal.Height)
            {
                GameLedControl.gamePoint[pointID].statue = enPointSta.Die;

            }
            picID = (int)(StartPos.x + width - 1) + (int)((StartPos.y + i) * Set.setVal.Width);
            pointID = Framebuffer.tab_Mapping[picID];
            if (pointID >= 0 && pointID < Set.setVal.Width * Set.setVal.Height)
            {
                GameLedControl.gamePoint[pointID].statue = enPointSta.Die;

            }

        }
        LeftPos = (int)StartPos.x;
        RightPos = (int)StartPos.x + width - 1;
        TopPos = (int)StartPos.y + height - 1;
        DownPos = (int)StartPos.y;

    }
    public Vector2[] fangkuai_24_Pos = new Vector2[5];
    public void UpdatePic_FangKuai_24(int jieDuan)
    {
        switch (jieDuan)
        {
            case 0:
                for (int i = 0; i < fangkuai_24_Pos.Length; i++)
                {
                    if (fangkuai_24_Pos[i].x > 0)
                    {
                        DrawPic.DrawRol((int)fangkuai_24_Pos[i].x, (int)fangkuai_24_Pos[i].y, 2, 0xff0000, enPointSta.Die);
                        if ((int)fangkuai_24_Pos[i].y + 1 < Set.setVal.Height)
                        {
                            DrawPic.DrawRol((int)fangkuai_24_Pos[i].x, (int)fangkuai_24_Pos[i].y + 1, 2, 0xff0000, enPointSta.Die);

                        }
                    }

                }
                break;
            case 1:

                for (int i = 0; i < fangkuai_24_Pos.Length; i++)
                {
                    if (fangkuai_24_Pos[i].x > 0)
                    {
                        DrawPic.DrawRol((int)fangkuai_24_Pos[i].x, (int)fangkuai_24_Pos[i].y, 1, 0xff0000, enPointSta.Die);
                        if ((int)fangkuai_24_Pos[i].y + 1 < Set.setVal.Height)
                        {
                            DrawPic.DrawRol((int)fangkuai_24_Pos[i].x, (int)fangkuai_24_Pos[i].y + 1, 1, 0xff0000, enPointSta.Die);

                        }
                    }

                }
                break;
            case 2:
                for (int i = 0; i < fangkuai_24_Pos.Length; i++)
                {
                    if (fangkuai_24_Pos[i].x > 0)
                    {
                        DrawPic.DrawRol((int)fangkuai_24_Pos[i].x, (int)fangkuai_24_Pos[i].y, 1, 0xff0000, enPointSta.Die);
                        if ((int)fangkuai_24_Pos[i].y + 1 < Set.setVal.Height)
                        {
                            DrawPic.DrawRol((int)fangkuai_24_Pos[i].x, (int)fangkuai_24_Pos[i].y, 1, 0xff0000, enPointSta.Die);

                        }
                    }

                }
                break;
        }
    }
    public void UpdatePic_Tank(Vector2 _StartPos)
    {
        isTank = true;
        StartPos = _StartPos;
        for (int i = 0; i < pos_Now1.Length; i++)
        {
            pos_Now1[i] = Vector2.one * -1;
        }

        switch (moveWay)
        {
            case en_MoveWay.Up:
                for (int i = 0; i < 3; i++)
                {


                    DrawPic.DrawRol((int)(StartPos.x - 1), (int)(StartPos.y + i), 3, 0xff0000, enPointSta.Die);

                }
                DrawPic.DrawRol((int)(StartPos.x), (int)(StartPos.y + 3), 1, 0xff0000, enPointSta.Die);



                LeftPos = (int)StartPos.x - 1;
                RightPos = (int)StartPos.x + 1;
                TopPos = (int)StartPos.y + 2;
                DownPos = (int)StartPos.y;
                Tank_FirePos = new Vector2((int)StartPos.x, TopPos);
                break;
            case en_MoveWay.Down:

                for (int i = 0; i < 3; i++)
                {


                    {

                        DrawPic.DrawRol((int)(StartPos.x - 1), (int)(StartPos.y - i), 3, 0xff0000, enPointSta.Die);

                    }
                }
                DrawPic.DrawRol((int)(StartPos.x), (int)(StartPos.y - 3), 1, 0xff0000, enPointSta.Die);
                LeftPos = (int)StartPos.x - 1;
                RightPos = (int)StartPos.x + 1;
                DownPos = (int)StartPos.y - 2;
                TopPos = (int)StartPos.y;
                Tank_FirePos = new Vector2((int)StartPos.x, DownPos);

                break;
            case en_MoveWay.Left:

                for (int i = 0; i < 3; i++)
                {


                    {

                        DrawPic.DrawCol((int)(StartPos.x - i), (int)(StartPos.y - 1), 3, 0xff0000, enPointSta.Die);

                    }
                }
                DrawPic.DrawRol((int)(StartPos.x - 3), (int)(StartPos.y), 1, 0xff0000, enPointSta.Die);
                LeftPos = (int)StartPos.x - 2;
                RightPos = (int)StartPos.x;
                TopPos = (int)StartPos.y + 1;
                DownPos = (int)StartPos.y - 1;

                Tank_FirePos = new Vector2(LeftPos, DownPos + 1);

                break;
            case en_MoveWay.Right:

                for (int i = 0; i < 3; i++)
                {


                    {

                        DrawPic.DrawCol((int)(StartPos.x + i), (int)(StartPos.y - 1), 3, 0xff0000, enPointSta.Die);

                    }
                }
                DrawPic.DrawRol((int)(StartPos.x + 3), (int)(StartPos.y), 1, 0xff0000, enPointSta.Die);
                LeftPos = (int)StartPos.x;
                RightPos = (int)StartPos.x + 2;
                TopPos = (int)StartPos.y + 1;
                DownPos = (int)StartPos.y - 1;
                Tank_FirePos = new Vector2(RightPos, DownPos + 1);

                break;


        }



    }
    public void UpdateSnake_17_RightKLong()
    {
        for (int i = 0; i < Snake_length_17; i++)
        {

            picID = (int)SnakePos_17_Long[i].x + Set.setVal.Width * (int)SnakePos_17_Long[i].y;
            pointID = Framebuffer.tab_Mapping[picID];
            GameLedControl.gamePoint[pointID].statue = enPointSta.Die;
        }
    }
    int maxTarage = 0;
    int game06_Tarage0 = 0;
    int game06_Tarage1 = 0;
    int player0_tarage = 10;
    int player0_tarage_Now = 10;
    int player1_tarage = 10;
    int player1_tarage_Now = 10;
    public Vector2[] Game06_List0 = new Vector2[30];
    public Vector2[] Game06_List1 = new Vector2[30];
    public void InitGameMap_06(int startX)
    {
        startx = 0;
        startx = startX;

        x = 0;
        x1 = 0;

        y = 0;
        y1 = 0;
        x0_06 = 0; x1_06 = 0; y0_06 = 0; y1_06 = 0;
        maxTarage = 0;
        game06_Tarage0 = 0;
        game06_Tarage1 = 0;
        player0_tarage = 10;
        player1_tarage = 10;
        player0_tarage_Now = 0;
        player1_tarage_Now = 0;
        if (Set.setVal.Width > 20 || Set.setVal.Height > 20)
        {
            player0_tarage = 15;
            player1_tarage = 15;
        }
        for (int i = 0; i < Game06_List0.Length; i++)
        {
            Game06_List0[i] = Vector2.one * -1;
            Game06_List1[i] = Vector2.one * -1;
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int y = 0; y < Set.setVal.Height; y++)
            {
                picID = i + Set.setVal.Width * y;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                maxTarage++;
            }
        }
        for (int i = startX; i < Set.setVal.Width; i++)
        {
            for (int y = 0; y < Set.setVal.Height; y++)
            {
                picID = i + Set.setVal.Width * y;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                maxTarage++;
            }
        }


        for (int i = 0; i < startX; i++)
        {
            for (int y = 0; y < Set.setVal.Height; y++)
            {
                picID = i + Set.setVal.Width * y;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
            }
        }
        cntX = 20;
        while (cntX > 0)
        {

            x = Random.Range(0, startX - 1);
            y = Random.Range(0, Set.setVal.Height);
            picID = x + Set.setVal.Width * y;
            pointID = Framebuffer.tab_Mapping[picID];
            if (GameLedControl.gamePoint[pointID].statue == enPointSta.None)
            {
                Game06_List0[player0_tarage_Now] = new Vector2(x, y);

                ledAnim.ChangeLed_Sta(x, y, enPointSta.Target);
                player0_tarage_Now++;
                cntX--;
            }
        }
        cntX = 20;
        while (cntX > 0)
        {
            x = Random.Range(startX, Set.setVal.Width);
            y = Random.Range(0, Set.setVal.Height);

            picID = x + Set.setVal.Width * y;
            pointID = Framebuffer.tab_Mapping[picID];
            if (GameLedControl.gamePoint[pointID].statue == enPointSta.None)
            {
                Game06_List1[player0_tarage_Now] = new Vector2(x, y);
                ledAnim.ChangeLed_Sta(x, y, enPointSta.Target);
                cntX--;
                player1_tarage_Now++;
            }
        }


    }
    int x0_06, x1_06, y0_06, y1_06 = 0;


    bool IsGamePass_06_0()
    {
        //Debug.LogError(x0_06 + "    " + y0_06+"   "+ startx +"   "+ Set.setVal.Height);
        if (x0_06 >= startx && y0_06 >= Set.setVal.Height - 1)
        {
            ledAnim.isClearAll = true;
            return true;
        }
        for (int i = 0; i < startx - 1; i++)
        {
            picID = i + Set.setVal.Width * (Set.setVal.Height - 1);
            pointID = Framebuffer.tab_Mapping[picID];
            if (GameLedControl.gamePoint[pointID].statue != enPointSta.Rest)
            {
                return false;
            }
        }
        ledAnim.isClearAll = true;
        return true;
    }
    bool IsGamePass_06_1()
    {
        if (x1_06 >= Set.setVal.Width - 1 && y1_06 >= Set.setVal.Height - 1)
        {
            ledAnim.isClearAll = true;
            return true;
        }
        for (int i = startx; i < Set.setVal.Width - 1; i++)
        {
            picID = i + Set.setVal.Width * (Set.setVal.Height - 1);
            pointID = Framebuffer.tab_Mapping[picID];
            if (GameLedControl.gamePoint[pointID].statue != enPointSta.Rest)
            {
                return false;
            }
        }

        ledAnim.isClearAll = true;
        return true;
    }

    public void CheckClick_06(int xx, int yy)
    {

        if (xx < startx)//判断左右两个玩家的
        {


            if (x0_06 >= startx)
            {
                x0_06 = 0;
                if (y0_06 < Set.setVal.Height)
                {
                    y0_06++;
                }
                else
                {
                    ledAnim.isClearAll = true;
                    return;
                }

            }


            //作弊
            //for (int i = 0; i < startx; i++)
            //{
            //    if (y0_06 > 0)
            //    {
            //        for (int k = 0; k < y0_06 - 1; k++)
            //        {
            //            picID = i + Set.setVal.Width * k;
            //            pointID = Framebuffer.tab_Mapping[picID];
            //            GameLedControl.gamePoint[pointID].statue = enPointSta.Rest;
            //        }
            //    }

            //}
            picID = x0_06 + Set.setVal.Width * y0_06;


            pointID = Framebuffer.tab_Mapping[picID];
            GameLedControl.gamePoint[pointID].statue = enPointSta.Rest;



            x0_06++;
            game06_Tarage0++;
            if (IsGamePass_06_0() || IsGamePass_06_1())
            {
                return;
            }



            int XX = Random.Range(0, startx);
            int YY = Random.Range(0, Set.setVal.Height);
            picID = XX + Set.setVal.Width * YY;
            pointID = Framebuffer.tab_Mapping[picID];

            int cnt = 300;

            while (cnt > 0)
            {
                cnt--;
                if (GameLedControl.gamePoint[pointID].statue == enPointSta.None)
                {
                    picID = XX + Set.setVal.Width * YY;
                    pointID = Framebuffer.tab_Mapping[picID];

                    ledAnim.ChangeLed_Sta(XX, YY, enPointSta.Target);

                    //    GameLedControl.gamePoint[pointID].statue = enPointSta.Target;
                    break;
                }


                XX = Random.Range(0, startx);
                YY = Random.Range(0, Set.setVal.Height);
                picID = XX + Set.setVal.Width * YY;
                pointID = Framebuffer.tab_Mapping[picID];

            }
            int change = Random.Range(0, 10);
            if (change == 2)
            {
                for (int i = 0; i < startx; i++)
                {
                    if (change <= 0)
                    {
                        break;
                    }
                    for (int k = 0; k < Set.setVal.Height; k++)
                    {
                        picID = i + Set.setVal.Width * k;
                        pointID = Framebuffer.tab_Mapping[picID];
                        if (GameLedControl.gamePoint[pointID].statue == enPointSta.None)
                        {

                            ledAnim.ChangeLed_Sta(i, k, enPointSta.Target);
                            change--;

                        }
                        if (change <= 0)
                        {
                            break;
                        }

                    }
                }
            }

            player0_tarage_Now++;


        }
        else
        {


            if (x1_06 >= Set.setVal.Width - startx - 1)
            {
                x1_06 = 0;
                if (y1_06 < Set.setVal.Height)
                {
                    y1_06++;
                }
            }
            //for (int i = startx + 1; i < Set.setVal.Width; i++)
            //{
            //    if (y1_06 > 0)
            //    {
            //        for (int k = 0; k < y1_06 - 1; k++)
            //        {
            //            picID = i + Set.setVal.Width * k;
            //            pointID = Framebuffer.tab_Mapping[picID];
            //            GameLedControl.gamePoint[pointID].statue = enPointSta.Rest;
            //        }
            //    }

            //}
            picID = startx + 1 + x1_06 + Set.setVal.Width * y1_06;
            pointID = Framebuffer.tab_Mapping[picID];
            GameLedControl.gamePoint[pointID].statue = enPointSta.Rest;
            //  DrawPic.DrawRol(x1_06, y1_06, 1, 0x00FF00, enPointSta.Rest);
            x1_06++;
            game06_Tarage1++;


            //int Index = 0;
            //for (int i = 0; i < Game06_List1.Length; i++)
            //{
            //    Vector2 m = new Vector2(xx, yy);
            //    if (m == Game06_List1[i])
            //    {
            //        Index = i;
            //        Game06_List1[i] = Vector2.one * -1;
            //        break;
            //    }
            //}
            if (IsGamePass_06_0() || IsGamePass_06_1())
            {
                return;
            }


            int XX1 = Random.Range(startx + 1, Set.setVal.Width);
            int YY1 = Random.Range(0, Set.setVal.Height);
            picID = XX1 + Set.setVal.Width * YY1;
            pointID = Framebuffer.tab_Mapping[picID];
            bool havePoint = false;
            cntX = 100;

            {
                int cnt = 300;

                while (cnt > 0)
                {
                    cnt--;
                    if (GameLedControl.gamePoint[pointID].statue == enPointSta.None)
                    {
                        picID = XX1 + Set.setVal.Width * YY1;
                        pointID = Framebuffer.tab_Mapping[picID];
                        GameLedControl.gamePoint[pointID].statue = enPointSta.Target;
                        havePoint = true;
                        break;
                    }

                    XX1 = Random.Range(startx + 1, Set.setVal.Width);

                    YY1 = Random.Range(0, Set.setVal.Height);
                    picID = XX1 + Set.setVal.Width * YY1;
                    pointID = Framebuffer.tab_Mapping[picID];

                }
            }
            int change = Random.Range(0, 10);
            if (change == 2)
            {
                for (int i = 0; i < startx; i++)
                {
                    if (change <= 0)
                    {
                        break;
                    }
                    for (int k = 0; k < Set.setVal.Height; k++)
                    {
                        picID = i + Set.setVal.Width * k;
                        pointID = Framebuffer.tab_Mapping[picID];
                        if (GameLedControl.gamePoint[pointID].statue == enPointSta.None)
                        {

                            ledAnim.ChangeLed_Sta(i, k, enPointSta.Target);
                            change--;

                        }
                        if (change <= 0)
                        {
                            break;
                        }

                    }
                }
            }

            player1_tarage_Now++;

        }

    }
    public void UpdateTarage_06()
    {
        return;
        //for (int i = 0; i < Game06_List1.Length; i++)
        //{
        //    if (Game06_List1[i].x >= 0)
        //    {

        //        ledAnim.ChangeLed_Sta((int)Game06_List1[i].x, (int)Game06_List1[i].y, enPointSta.Target);

        //    }
        //    if (Game06_List0[i].x >= 0)
        //    {
        //        Debug.LogError(Game06_List0[i]);
        //           ledAnim.ChangeLed_Sta((int)Game06_List0[i].x, (int)Game06_List0[i].y, enPointSta.Target);

        //    }
        //}

    }
    public void Init(bool _isLeft, ILedMapCallback _ledAnim)
    {
        isTank = false;
        ledAnim = _ledAnim;
        attackCD = attackMaxCD = 3;
        isLeft = _isLeft;
        x = y = x1 = y1 = 0;
        dir = 0;
        cntX = -3;
    }
    int ID = 0;
    public void Init(int i)
    {
        ID = i;
        isTank = false;
        attackCD = attackMaxCD = 3;

        x = y = x1 = y1 = 0;
        dir = 0;
        cntX = -3;
    }
    public void Init(int i, ILedMapCallback _ledAnim)
    {
        ledAnim = _ledAnim;
        Init(i);
    }
    public Vector2[] Pos_M;
    public void Init_Game06_V()
    {
        cntX = 0;
        x = Set.setVal.Width / 4;
        cntY = Set.setVal.Height * 2 / 3;
        snakeCnt = 0;

        for (int i = 0; i < Game_Map06.instance.targetPos.Length; i++)
        {
            Game_Map06.instance.targetPos[i].Set(-1, -1);
        }


        for (int i = Set.setVal.Width / 4; i < Set.setVal.Width / 2; i++)
        {
            if (cntY > Set.setVal.Height / 3)
            {
                Game_Map06.instance.targetPos[snakeCnt].Set(i, cntY);
                snakeCnt++;
                x++;
                cntX++;
                cntY--;
            }
        }
        for (int i = Set.setVal.Width / 2; i < Set.setVal.Width * 3 / 4 + 1; i++)
        {

            if (cntY < Set.setVal.Height * 2 / 3)
            {
                cntX++;
                cntY++;
                Game_Map06.instance.targetPos[snakeCnt].Set(i, cntY);
                snakeCnt++;
                x++;


            }
        }

        for (int i = 0; i < Game_Map06.instance.Pos_DianZu.Length; i++)
        {
            float pos = 0;

            if (i == 0)
            {


                picID = (int)Game_Map06.instance.targetPos[snakeCnt - 1].x + (int)Game_Map06.instance.targetPos[snakeCnt - 1].y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];

                Game_Map06.instance.Pos_DianZu[Game_Map06.instance.Cnt_DianZu] = pointID;


                Game_Map06.instance.Cnt_DianZu++;
            }
            else
            {
                pos = Random.Range(0, snakeCnt - 1);
                ///pos = (   ((float)snakeCnt  -1- i) / (float)(snakeCnt))* (float)snakeCnt;

                picID = (int)Game_Map06.instance.targetPos[(int)pos].x + (int)Game_Map06.instance.targetPos[(int)pos].y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];

                Game_Map06.instance.Pos_DianZu[Game_Map06.instance.Cnt_DianZu] = pointID;

                Game_Map06.instance.Cnt_DianZu++;

            }

        }


        Game_Map06.instance.Cnt_DianLiu = snakeCnt;



    }
    public void Init_Game06_WenHao()
    {
        cntX = 0;
        x = Set.setVal.Width / 4;
        cntY = Set.setVal.Height * 2 / 3;
        snakeCnt = 0;

        for (int i = 0; i < Game_Map06.instance.targetPos.Length; i++)
        {
            Game_Map06.instance.targetPos[i].Set(-1, -1);
        }


        for (int i = Set.setVal.Height * 3 / 4; i < Set.setVal.Height - 1; i++)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(Set.setVal.Width / 4, i);
            snakeCnt++;


        }
        for (int i = Set.setVal.Width / 4; i < Set.setVal.Width * 3 / 4; i++)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(i, Set.setVal.Height - 2);
            snakeCnt++;


        }
        for (int i = Set.setVal.Height - 2; i > Set.setVal.Height / 2; i--)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(Set.setVal.Width * 3 / 4, i);
            snakeCnt++;


        }
        for (int i = Set.setVal.Width * 3 / 4; i > Set.setVal.Width / 2; i--)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(i, Set.setVal.Height / 2);
            snakeCnt++;


        }
        for (int i = Set.setVal.Height / 2; i > 3; i--)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(Set.setVal.Width / 2, i);
            snakeCnt++;


        }

        Game_Map06.instance.targetPos[snakeCnt].Set(Set.setVal.Width / 2, 2);
        snakeCnt++;

        for (int i = 0; i < Game_Map06.instance.Pos_DianZu.Length; i++)
        {
            float pos = 0;
            if (i == 0)
            {


                picID = (int)Game_Map06.instance.targetPos[snakeCnt - 1].x + (int)Game_Map06.instance.targetPos[snakeCnt - 1].y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];

                Game_Map06.instance.Pos_DianZu[Game_Map06.instance.Cnt_DianZu] = pointID;


                Game_Map06.instance.Cnt_DianZu++;
            }
            else
            {

                pos = Random.Range(0, snakeCnt - 1);

                picID = (int)Game_Map06.instance.targetPos[(int)pos].x + (int)Game_Map06.instance.targetPos[(int)pos].y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];

                Game_Map06.instance.Pos_DianZu[Game_Map06.instance.Cnt_DianZu] = pointID;

                Game_Map06.instance.Cnt_DianZu++;

            }

        }


        Game_Map06.instance.Cnt_DianLiu = snakeCnt;



    }
    public void Init_Game06_L()
    {
        cntX = 0;
        x = Set.setVal.Width / 4;
        cntY = Set.setVal.Height * 2 / 3;
        snakeCnt = 0;

        for (int i = 0; i < Game_Map06.instance.targetPos.Length; i++)
        {
            Game_Map06.instance.targetPos[i].Set(-1, -1);
        }


        for (int i = Set.setVal.Height - 2; i > 1; i--)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(Set.setVal.Width / 4 - 1, i);
            snakeCnt++;


        }
        for (int i = Set.setVal.Width / 4; i < Set.setVal.Width * 3 / 4 + 1; i++)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(i, 2);
            snakeCnt++;

        }

        for (int i = Set.setVal.Width * 3 / 4; i > Set.setVal.Width / 4; i--)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(i, 3);
            snakeCnt++;

        }

        for (int i = 3; i < Set.setVal.Height - 1; i++)
        {

            Game_Map06.instance.targetPos[snakeCnt].Set(Set.setVal.Width / 4, i);
            snakeCnt++;


        }
        for (int i = 0; i < Game_Map06.instance.Pos_DianZu.Length; i++)
        {
            float pos = 0;
            if (i == 0)
            {


                picID = (int)Game_Map06.instance.targetPos[snakeCnt - 1].x + (int)Game_Map06.instance.targetPos[snakeCnt - 1].y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];

                Game_Map06.instance.Pos_DianZu[Game_Map06.instance.Cnt_DianZu] = pointID;


                Game_Map06.instance.Cnt_DianZu++;
            }
            else
            {
                pos = Random.Range(0, snakeCnt - 1);

                picID = (int)Game_Map06.instance.targetPos[(int)pos].x + (int)Game_Map06.instance.targetPos[(int)pos].y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];

                Game_Map06.instance.Pos_DianZu[Game_Map06.instance.Cnt_DianZu] = pointID;

                Game_Map06.instance.Cnt_DianZu++;

            }

        }


        Game_Map06.instance.Cnt_DianLiu = snakeCnt;



    }
    public void Init_Game19_M()
    {
        int dd = 0;
        int xx = 0;
        int yy = 0;
        Pos_M = new Vector2[(int)(Set.setVal.Width)];
        for (int i = 0; i < Pos_M.Length; i++)
        {
            if (Set.setVal.Width > Set.setVal.Height)
            {
                Pos_M[i] = new Vector2(xx, yy);
                if (xx < Set.setVal.Width)
                {
                    xx++;

                }
                else
                {
                    xx = 0;
                }

                if (dd == 0)
                {



                    if (yy < Set.setVal.Height)
                    {
                        yy++;
                    }
                    else
                    {
                        yy = Set.setVal.Height - 1;
                        dd = 1;
                    }
                }
                else
                {
                    if (yy > 0)
                    {
                        yy--;
                    }
                    else
                    {
                        yy = 0;
                        dd = 0;
                    }
                }
            }
            else
            {
                Pos_M[i] = new Vector2(xx, yy);
                if (xx < Set.setVal.Width)
                {
                    xx++;

                }
                else
                {
                    xx = 0;
                }

                if (dd == 0)
                {



                    if (yy < Set.setVal.Height)
                    {
                        yy++;
                    }
                    else
                    {
                        yy = Set.setVal.Height - 1;
                        dd = 1;
                    }
                }
                else
                {
                    if (yy > 0)
                    {
                        yy--;
                    }
                    else
                    {
                        yy = 0;
                        dd = 0;
                    }
                }
            }
        }

    }
    public void Update_Game19_M(int Jieduan)
    {
        switch (Jieduan)
        {
            case 0:
                for (int i = 0; i < Pos_M.Length; i++)
                {

                    DrawPic.DrawRol((int)Pos_M[i].x, (int)Pos_M[i].y, 1, 0xff0000, enPointSta.Die);

                }
                break;
            case 1:
                for (int i = 0; i < Pos_M.Length; i++)
                {

                    DrawPic.DrawRol((int)Pos_M[i].x, (int)Pos_M[i].y, 2, 0xff0000, enPointSta.Die);




                }
                break;
            case 2:
                for (int i = 0; i < Pos_M.Length; i++)
                {

                    DrawPic.DrawRol((int)Pos_M[i].x, (int)Pos_M[i].y, 1, 0xff0000, enPointSta.Die);
                    DrawPic.DrawRol((int)Pos_M[i].x + 1, (int)Pos_M[i].y + 1, 1, 0xff0000, enPointSta.Die);



                }
                break;
            case 3:
                for (int i = 0; i < Pos_M.Length; i++)
                {
                    if (Pos_M[i].y >= 0)
                    {
                        DrawPic.DrawRol((int)Pos_M[i].x, (int)Pos_M[i].y, 2, 0xff0000, enPointSta.Die);

                        DrawPic.DrawRol((int)Pos_M[i].x + 1, (int)Pos_M[i].y + 1, 1, 0xff0000, enPointSta.Die);


                    }


                }
                break;
        }

    }

    public void Moving_Game19_M()
    {

        for (int i = 0; i < Pos_M.Length; i++)
        {
            if (Set.setVal.Width > Set.setVal.Height)
            {
                if (Pos_M[i].y > 0)
                {
                    if (Pos_M[i].x < Set.setVal.Width)
                    {
                        Pos_M[i].x += 1;

                    }
                    else
                    {
                        Pos_M[i].x = 0;

                    }
                }
            }
            else
            {


                //if (Pos_M[i].x < Set.setVal.Width)
                //{
                //    Pos_M[i].x += 1;

                //}
                //else
                //{
                //    Pos_M[i].x = 0;

                //}
                if (Pos_M[i].y < Set.setVal.Height)
                {
                    Pos_M[i].y += 1;

                }
                else
                {
                    Pos_M[i].y = 0;

                }

            }



        }
    }
    public bool Moving_Tank()
    {
        switch (moveWay)
        {
            case en_MoveWay.Up:

                if (TopPos + 1 > Set.setVal.Height - 2)
                {
                    return false;
                }

                StartPos += new Vector2(0, 1);



                break;
            case en_MoveWay.Down:
                if (DownPos - 1 <= 0)
                {
                    return false;
                }
                StartPos -= new Vector2(0, 1);
                break;
            case en_MoveWay.Left:
                if (LeftPos - 1 <= 0)
                {
                    return false;
                }
                StartPos -= new Vector2(1, 0);
                break;
            case en_MoveWay.Right:
                if (RightPos + 1 >= Set.setVal.Width - 2)
                {
                    return false;
                }
                StartPos += new Vector2(1, 0);
                break;


        }
        return true;
    }
    public bool FangKuaiMoving()
    {
        switch (moveWay)
        {
            case en_MoveWay.Up:

                if (TopPos + 1 > Set.setVal.Height - 1)
                {
                    return false;
                }

                StartPos += new Vector2(0, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);



                break;
            case en_MoveWay.Down:
                if (DownPos - 1 < 0)
                {
                    return false;
                }
                StartPos -= new Vector2(0, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.Left:
                if (LeftPos - 1 < 0)
                {
                    return false;
                }
                StartPos -= new Vector2(1, 0);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.Right:
                if (RightPos + 1 >= Set.setVal.Width)
                {
                    return false;
                }
                StartPos += new Vector2(1, 0);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.LU:
                if (LeftPos - 1 <= 0 || TopPos + 1 > Set.setVal.Height - 1)
                {
                    return false;
                }
                StartPos += new Vector2(-1, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);

                break;
            case en_MoveWay.RU:
                if (RightPos + 1 >= Set.setVal.Width - 1 || TopPos + 1 > Set.setVal.Height - 1)
                {
                    return false;
                }
                StartPos += new Vector2(1, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.LD:
                if (LeftPos - 1 <= 0 || DownPos - 1 <= 0)
                {
                    return false;
                }
                StartPos += new Vector2(-1, -1);
                UpdatePic_FangKuai(StartPos, 4, 4);


                break;
            case en_MoveWay.RD:
                if (RightPos + 1 >= Set.setVal.Width - 2 || DownPos - 1 <= 0)
                {
                    return false;
                }
                StartPos += new Vector2(1, -1);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;

        }
        return true;
    }
    public bool FangKuai_ChangeMoving()
    {

        int a = Random.Range(0, 8);
        moveWay = (en_MoveWay)a;
        switch (moveWay)
        {
            case en_MoveWay.Up:

                if (TopPos + 1 > Set.setVal.Height - 1)
                {
                    return false;
                }

                StartPos += new Vector2(0, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);



                break;
            case en_MoveWay.Down:
                if (DownPos - 1 <= 0)
                {
                    return false;
                }
                StartPos -= new Vector2(0, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.Left:
                if (LeftPos - 1 <= 0)
                {
                    return false;
                }
                StartPos -= new Vector2(1, 0);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.Right:
                if (RightPos + 1 >= Set.setVal.Width - 2)
                {
                    return false;
                }
                StartPos += new Vector2(1, 0);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.LU:
                if (LeftPos - 1 <= 0 || TopPos + 1 > Set.setVal.Height - 2)
                {
                    return false;
                }
                StartPos += new Vector2(-1, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);

                break;
            case en_MoveWay.RU:
                if (RightPos + 1 >= Set.setVal.Width - 2 || TopPos + 1 > Set.setVal.Height - 2)
                {
                    return false;
                }
                StartPos += new Vector2(1, 1);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;
            case en_MoveWay.LD:
                if (LeftPos - 1 <= 0 || DownPos - 1 <= 0)
                {
                    return false;
                }
                StartPos += new Vector2(-1, -1);
                UpdatePic_FangKuai(StartPos, 4, 4);


                break;
            case en_MoveWay.RD:
                if (RightPos + 1 >= Set.setVal.Width - 2 || DownPos - 1 <= 0)
                {
                    return false;
                }
                StartPos += new Vector2(1, -1);
                UpdatePic_FangKuai(StartPos, 4, 4);
                break;

        }
        return true;
    }

    public bool Tank_ChangeMoving()
    {

        int a = Random.Range(0, 4);
        moveWay = (en_MoveWay)a;
        switch (moveWay)
        {
            case en_MoveWay.Up:

                if (TopPos + 1 > Set.setVal.Height - 2)
                {
                    return false;
                }

                StartPos += new Vector2(0, 1);
                UpdatePic_Tank(StartPos);

                break;
            case en_MoveWay.Down:
                if (DownPos - 1 <= 0)
                {
                    return false;
                }
                StartPos -= new Vector2(0, 1);
                UpdatePic_Tank(StartPos);
                break;
            case en_MoveWay.Left:
                if (LeftPos - 1 <= 0)
                {
                    return false;
                }
                StartPos -= new Vector2(1, 0);
                UpdatePic_Tank(StartPos);
                break;
            case en_MoveWay.Right:
                if (RightPos + 1 >= Set.setVal.Width - 2)
                {
                    return false;
                }
                StartPos += new Vector2(1, 0);
                UpdatePic_Tank(StartPos);
                break;

        }
        return true;
    }
    public void InitSnake()
    {
        for (int i = 0; i < SnakePos.Length; i++)
        {
            SnakePos[i] = Vector2.one * -1;
        }
        Snake_TaragePos = Vector2.one * -1;
        sanke_Length = 1;
        SnakePos[0] = new Vector2((int)Set.setVal.Width / 2, (int)Set.setVal.Height / 2);
        picID = (int)SnakePos[0].x + Set.setVal.Width * (int)SnakePos[0].y;
        pointID = Framebuffer.tab_Mapping[picID];
        if (GameLedControl.gamePoint[pointID].statue == enPointSta.Target)
        {
            ledAnim.tarageNum--;
        }
        GameLedControl.gamePoint[0].statue = enPointSta.Die;
        FindTarage();
    }
    public void UpdateSnake()
    {

        for (int i = 0; i < SnakePos.Length && i < sanke_Length; i++)
        {


            picID = (int)SnakePos[i].x + Set.setVal.Width * (int)SnakePos[i].y;
            if (picID > Set.setVal.Width * Set.setVal.Height)
            {
                return;
            }
            pointID = Framebuffer.tab_Mapping[picID];
            if (GameLedControl.gamePoint[pointID].statue == enPointSta.Target)
            {
                ledAnim.tarageNum--;
            }

            GameLedControl.gamePoint[pointID].statue = enPointSta.Die;
            DrawPic.DrawPointId(pointID, 0xff0000, enPointSta.Die);


        }
        // GameLedControl.gamePoint[pointID].statue = enPointSta.Die;


    }
    public Vector2[] X_Pos = new Vector2[300];
    public bool[] bool_X_Pos = new bool[300];
    public int X_length = 0;
    int Max_X = 1;//往四周发展,最多多少步数

    public void InitKuai_X(int index)//第21关
    {
        X_length = 0;
        for (int i = 0; i < bool_X_Pos.Length; i++)
        {
            bool_X_Pos[i] = true;
        }

        Vector2 startPos = Vector2.one;
        if (Set.setVal.Width > Set.setVal.Height)
        {
            switch (index)
            {
                case 0:
                    startPos = new Vector2(Set.setVal.Width / 4, Set.setVal.Height / 2);
                    break;
                case 1:
                    startPos = new Vector2(Set.setVal.Width * 3 / 4, Set.setVal.Height / 2);

                    break;
                case 2:
                    startPos = new Vector2(0, Set.setVal.Height / 2);

                    break;
                case 3:
                    startPos = new Vector2(Set.setVal.Width - 2, Set.setVal.Height / 2);

                    break;
            }
        }
        else
        {
            switch (index)
            {
                case 0:
                    startPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height / 4);
                    break;
                case 1:
                    startPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height * 3 / 4);

                    break;
                case 2:
                    startPos = new Vector2(Set.setVal.Height / 2, 0);

                    break;
                case 3:
                    startPos = new Vector2(Set.setVal.Width / 2, Set.setVal.Height - 2);

                    break;
            }
        }





        X_Pos[X_length] = new Vector2(startPos.x + 1, startPos.y + 1);

        while (
            X_Pos[X_length].x + 1 < Set.setVal.Width - 1
            &&
            X_Pos[X_length].y + 1 < Set.setVal.Height - 1
            )
        {
            X_length++;
            X_Pos[X_length] = X_Pos[X_length - 1] + Vector2.one;

            Max_X++;
        }
        /////////////////////

        X_length++;
        X_Pos[X_length] = new Vector2(startPos.x + 1, startPos.y);


        while (
            X_Pos[X_length].x + 1 < Set.setVal.Width - 1
            &&
            X_Pos[X_length].y - 1 > 0
            )
        {
            X_length++;
            X_Pos[X_length] = X_Pos[X_length - 1] + new Vector2(1, -1);

            Max_X++;
        }
        /////////////////

        X_length++;
        X_Pos[X_length] = startPos;

        while (
            X_Pos[X_length].x - 1 > 0
            &&
            X_Pos[X_length].y - 1 > 0
            )
        {
            X_length++;
            X_Pos[X_length] = X_Pos[X_length - 1] - Vector2.one;

            Max_X++;
        }
        ///////////////////
        X_length++;
        X_Pos[X_length] = new Vector2(startPos.x, startPos.y + 1);

        while (
            X_Pos[X_length].x - 1 > 0
            &&
            X_Pos[X_length].y + 1 < Set.setVal.Height - 1
            )
        {
            X_length++;
            X_Pos[X_length] = X_Pos[X_length - 1] + new Vector2(-1, 1);


            Max_X++;
        }


    }
    public void UpdateFangKuai_X()//第21关
    {

        for (int i = 0; i < X_length; i++)
        {
            picID = (int)X_Pos[i].x + (int)X_Pos[i].y * Set.setVal.Width;
            pointID = Framebuffer.tab_Mapping[picID];

            GameLedControl.gamePoint[pointID].statue = enPointSta.Target;
            //   DrawPic.DrawRol((int)X_Pos[i].x, (int)X_Pos[i].y, 1, 0x00FFE7, enPointSta.Target);

        }
    }
    public void MoveFangKuai_X()
    {

        for (int i = 0; i < X_length; i++)
        {
            if (X_Pos[i].x < Set.setVal.Width)
            {
                X_Pos[i].x++;

            }
            else
            {
                X_Pos[i].x = 0;
            }
        }
    }
    int snakeCnt = 0;
    int bugCnt = 0;
    public void FindTarage()
    {

        dinstance = 1000;
        snakeCnt--;
        if (Snake_TaragePos.x >= 0 && snakeCnt > 0)
        {
            return;
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                picID = i + Set.setVal.Width * k;
                pointID = Framebuffer.tab_Mapping[picID];

                if (GameLedControl.gamePoint[pointID].statue == enPointSta.Target)
                {

                    float dis = Vector2.Distance(SnakePos[0], new Vector2(i, k));

                    if (dis < dinstance)
                    {
                        dinstance = dis;
                        if (bugCnt > 3)
                        {
                            Snake_TaragePos = Vector2.one;
                            bugCnt = 0;
                        }
                        if (Snake_TaragePos != new Vector2(i, k))
                        {
                            Snake_TaragePos = new Vector2(i, k);
                        }
                        else
                        {
                            bugCnt++;
                        }

                        snakeCnt = 20;
                    }
                }

            }
        }

    }
    int tarageNum = 20;
    public enum PicType
    {
        ZuoKou,
        L,
        Z,
        Kou,
        ShuZ,
        Shi


    }
    public PicType picType;
    public void GetPicType()
    {
        picType = (PicType)ID;
        int col = Random.Range(0, Game_Map06.instance.tab_PointColor.Length);
        for (int i = Set.setVal.Width / 2 - 1; i < Set.setVal.Width / 2 + 2; i++)
        {
            for (int k = Set.setVal.Height / 2 - 1; k < Set.setVal.Height / 2 + 2; k++)
            {
                picID = k * Set.setVal.Width + i;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.Target;
                Game_Map06.instance.Led_Color[pointID] = Game_Map06.instance.tab_PointColor[col];
            }
        }

        switch (picType)
        {
            case PicType.ZuoKou:
                picID = Set.setVal.Height / 2 * Set.setVal.Width + Set.setVal.Width / 2;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (Set.setVal.Height / 2 + 1) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (Set.setVal.Height / 2 - 1) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;
            case PicType.L:
                picID = Set.setVal.Height / 2 * Set.setVal.Width + Set.setVal.Width / 2;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (Set.setVal.Height / 2 + 1) * Set.setVal.Width + Set.setVal.Width / 2;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (Set.setVal.Height / 2 + 1) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (Set.setVal.Height / 2 + 2) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;
            case PicType.Z:
                picID = Set.setVal.Height / 2 * Set.setVal.Width + Set.setVal.Width / 2 - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;



                picID = (Set.setVal.Height / 2) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                break;
            case PicType.Kou:
                picID = Set.setVal.Height / 2 * Set.setVal.Width + Set.setVal.Width / 2;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                break;
            case PicType.ShuZ:
                picID = (Set.setVal.Height / 2 + 1) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (Set.setVal.Height / 2) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (Set.setVal.Height / 2 - 1) * Set.setVal.Width + Set.setVal.Width / 2 - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (Set.setVal.Height / 2) * Set.setVal.Width + Set.setVal.Width / 2 - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;
            case PicType.Shi:
                picID = (Set.setVal.Height / 2 + 1) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (Set.setVal.Height / 2 + 1) * Set.setVal.Width + Set.setVal.Width / 2 - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (Set.setVal.Height / 2 - 1) * Set.setVal.Width + Set.setVal.Width / 2 + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (Set.setVal.Height / 2 - 1) * Set.setVal.Width + Set.setVal.Width / 2 - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;


        }
    }

    public void GetTruePicType()
    {
        x = Random.Range(2, Set.setVal.Width - 4);
        y = Random.Range(2, Set.setVal.Height - 4);
        picType = (PicType)ID;
        int col = Random.Range(0, Game_Map06.instance.tab_PointColor.Length);
        for (int i = x - 1; i < x + 2; i++)
        {
            for (int k = y - 1; k < y + 2; k++)
            {
                picID = k * Set.setVal.Width + i;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.Target;
                Game_Map06.instance.Led_Color[pointID] = Game_Map06.instance.tab_PointColor[col];
            }
        }

        switch (picType)
        {
            case PicType.ZuoKou:
                picID = y * Set.setVal.Width + x;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (y + 1) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (y - 1) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;
            case PicType.L:
                picID = y * Set.setVal.Width + x;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (y + 1) * Set.setVal.Width + x;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (y + 1) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (y + 2) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;
            case PicType.Z:
                picID = y * Set.setVal.Width + x - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;



                picID = (y) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                break;
            case PicType.Kou:
                picID = y * Set.setVal.Width + x;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                break;
            case PicType.ShuZ:
                picID = (y + 1) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (y) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (y - 1) * Set.setVal.Width + x - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (y) * Set.setVal.Width + x - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;
            case PicType.Shi:
                picID = (y + 1) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (y + 1) * Set.setVal.Width + x - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;

                picID = (y - 1) * Set.setVal.Width + x + 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                picID = (y - 1) * Set.setVal.Width + x - 1;
                pointID = Framebuffer.tab_Mapping[picID];
                GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                break;


        }
    }


    public void GetTatagePos()
    {
        tarageNum = 2;
        if (Set.setVal.Height * Set.setVal.Width > 500)
        {
            tarageNum = 3;

        }

        int cnt = 300;
        while (tarageNum > 0 && cnt > 0)
        {
            x = Random.Range(0, Set.setVal.Width * Set.setVal.Height);
            cnt--;
            if (GameLedControl.gamePoint[x].statue == enPointSta.None)
            {

                tarageNum--;

                GameLedControl.gamePoint[x].statue = enPointSta.Target;
                cnt = Random.Range(0, Game_Map06.instance.tab_PointColor.Length);
                Game_Map06.instance.Led_Color[x] = Game_Map06.instance.tab_PointColor[cnt];
            }
        }

    }
    int[] TarageNum_List;
    public void GetTarage()
    {

        tarageNum = 10;
        if (Set.setVal.Width * Set.setVal.Height > 500)
        {
            tarageNum = 20;

        }

        if (Set.setVal.Width * Set.setVal.Height > 800)
        {
            tarageNum = 30;

        }

        int[] TarageNum_List = new int[tarageNum];
        for (int i = 0; i < TarageNum_List.Length; i++)
        {
            TarageNum_List[i] = -1;
        }
        if (tarageNum > 0)
        {
            while (tarageNum > 0)//
            {


                x = Random.Range(0, Set.setVal.Width - 1);
                y = Random.Range(1, Set.setVal.Height - 1);
                picID = x + y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];
                if (GameLedControl.gamePoint[pointID].statue == enPointSta.None && Game_Map06.instance.Led_Color[pointID] == 0)
                {
                    if (ID == 0)
                    {
                        GameLedControl.gamePoint[pointID].statue = enPointSta.Target;
                        tarageNum--;
                        Game_Map06.instance.Led_Color[pointID] = 0x0000ff;
                    }
                    else
                    {
                        GameLedControl.gamePoint[pointID].statue = enPointSta.Target;
                        tarageNum--;
                        Game_Map06.instance.Led_Color[pointID] = 0x00ff00;
                    }


                }


            }
        }


    }
    public void GetTarage_new09()
    {

        tarageNum = 10;
        if (Set.setVal.Width * Set.setVal.Height > 500)
        {
            tarageNum = 20;

        }

        if (Set.setVal.Width * Set.setVal.Height > 800)
        {
            tarageNum = 30;

        }

        int[] TarageNum_List = new int[tarageNum];
        for (int i = 0; i < TarageNum_List.Length; i++)
        {
            TarageNum_List[i] = -1;
        }
        if (tarageNum > 0)
        {
            while (tarageNum > 0)//
            {


                x = Random.Range(0, Set.setVal.Width - 1);
                y = Random.Range(1, Set.setVal.Height - 1);
                picID = x + y * Set.setVal.Width;
                pointID = Framebuffer.tab_Mapping[picID];
                if (GameLedControl.gamePoint[pointID].statue == enPointSta.None && Game_Map06.instance.Led_Color[pointID] == 0)
                {
                    if (ID == 0)
                    {
                        GameLedControl.gamePoint[pointID].statue = enPointSta.None;
                        tarageNum--;
                        Game_Map06.instance.Led_Color[pointID] = 0x00ff00;
                    }
                    else
                    {
                        GameLedControl.gamePoint[pointID].statue = enPointSta.Die;
                        tarageNum--;
                        Game_Map06.instance.Led_Color[pointID] = 0xff0000;
                    }


                }


            }
        }


    }



    public void UpdatePic_22()
    {
        DrawPic.DrawRol(x, y, Set.setVal.Width / 2, 0xff0000, enPointSta.Die);
        DrawPic.DrawRol(x, y - 1, Set.setVal.Width / 2, 0xff0000, enPointSta.Die);
    }

    public void Moving_22(int index)
    {
        switch (index)
        {
            case 0:
            case 1:
                if (dir == 0)
                {

                    if (y + 1 < Set.setVal.Height)
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
                    if (y - 1 > 0)
                    {
                        y--;


                    }
                    else
                    {
                        dir = 0;
                    }
                }
                break;
            case 2:
            case 3:
                if (dir == 0)
                {

                    if (x + (Set.setVal.Width / 2) < Set.setVal.Width)
                    {
                        x++;
                    }
                    else
                    {
                        dir = 1;

                    }
                }
                else
                {
                    if (x > 0)
                    {
                        x--;


                    }
                    else
                    {
                        dir = 0;
                    }
                }
                break;
        }

    }
    public void Moveing()
    {
        haveFire = false;




        if (dir == 0)
        {
            TopPos = (int)pos_Now[0].y;


            if (TopPos < Set.setVal.Height - 3)
            {
                for (int i = 0; i < pos_Now.Length; i++)
                {
                    pos_Now[i] += new Vector2(0, 1);


                }


            }
            else
            {
                dir = 1;
                return;
            }

        }
        else
        {
            TopPos = (int)pos_Now[2].y;


            if (TopPos > 2)
            {
                for (int i = 0; i < pos_Now.Length; i++)
                {
                    pos_Now[i] -= new Vector2(0, 1);

                }
            }
            else
            {

                dir = 0;
                return;
            }

        }

    }
    // Use this for initialization
    void Attacking(Vector2 pos, int way)
    {
        GameObject b = Instantiate(buttle.gameObject, transform);
        LedAnim_Buttle blt = b.GetComponent<LedAnim_Buttle>();
        blt.Init(pos, way);

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Game06_Main.instance.statue != en_Game00_Sta.Play)
        {
            return;
        }
        if (isTank)
        {
            if (attackCD > 0)
            {
                attackCD -= Time.deltaTime;
            }
            else
            {
                //  attackCD = Random.Range(10, 20);
                attackCD = 3;
                Attacking(Tank_FirePos, (int)moveWay);


            }

        }
    }
}
