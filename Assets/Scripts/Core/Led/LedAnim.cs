using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct stColor
{
    public byte r;
    public byte g;
    public byte b;

    public stColor(byte R, byte G, byte B)
    {
        r = R;
        g = G;
        b = B;
    }
}

public enum en_LedType
{
    LeiShe = 0,
    TxLed,
    RxLed,
}

public class LedAnim
{

    readonly static stColor[] tab_stColor = {
        new stColor (0,0,0),		//	COLOR_NONE = 0,
	    new stColor (250,0,0),		//	COLOR_R,
	    new stColor (0,250,0),		//	COLOR_G,
	    new stColor (0,0,250),		//	COLOR_B,
	    new stColor (250,250,0),	//	COLOR_RG,
	    new stColor (250,0,250),	//	COLOR_RB,
	    new stColor (0,250,250),	//	COLOR_GB,
	    new stColor (250,250,250),	//	COLOR_RGB,
    };

    public readonly static uint[] tab_Color = {
        0x000000,		//	COLOR_NONE = 0,
	    0xfc0000,		//	COLOR_R,
	    0x00fc00,		//	COLOR_G,
	    0x0000fc,		//	COLOR_B,
	    0xfcfc00,	    //	COLOR_RG,
	    0xfc00fc,	    //	COLOR_RB,
	    0x00fcfc,	    //	COLOR_GB,
	    0xfcfcfc,	    //	COLOR_RGB,
    };

    //public AnimOne animSet;
    //{
    //public enPicType picType;
    //public enAnimMode animMode;
    //public enCOLOR color;
    //public enPointSta pointSta;
    //public int loop;
    //public int x;
    //public int y;
    //public int speed;
    //public int width;
    //public int height;
    //public int delayTime;
    //}

    public en_LedType ledType;
    public enPicType picType;
    public enAnimMode animMode;
    public int startx;
    public int starty;
    public int endx;
    public int endy;
    public int width;
    public int height;
    public int loop;
    public byte value;
    public float stepTime;
    public enCOLOR color;
    public enPointSta pointSta;


    //public int startx;
    //public int starty;
    //public int endRunPos;
    //public int endRunPos;

    public int limitLeft;
    public int limitRight;
    public int limitUp;
    public int limitDown;

    public int speed;
    public int statue;
    public int runTime;
    public int startRunPos;
    public int endRunPos;
    public int x;
    public int y;
    public int dir;
    public int delayTime;
    public int delay;
    public int radio;
    //
    int runSta;
    int getStartXY;
    int cnt;

    public GameObject[] flyButtle;

    public void Stop()
    {
        statue = 0;
    }
    public void RunStart()
    {
       
        statue = 1;
        runTime = 0;
        if (animMode == enAnimMode.BigToSmall || animMode == enAnimMode.SmallToBig)
        {
            x = Set.setVal.Width / 2 - 1;
            y = Set.setVal.Height / 2 - 1;
            if (x <= 0)
            {
                x = 0;
            }
            if (y <= 0)
            {
                y = 0;
            }
        }
        Show();
    }
    int trueSpeed = 1;
    public void Run()
    {
        if (statue == 0)
            return;
        runTime++;
        switch (Set.setVal.GameChoose)
        {
            default:
           
                trueSpeed = speed;
                break;
            case 0:
                trueSpeed = speed - Game00_Main.instance.Index_JieDuan;
                if (trueSpeed <= 1)
                {
                    trueSpeed = 1;
                }
                break;
            case 2:
                trueSpeed = speed - SettingInGame_02.instance.set_MoveSpeed[Game02_Main.instance.Index_JieDuan]*5;
                if (trueSpeed<=10)
                {
                    trueSpeed = 10;
                }
                break;
        }
     //   Debug.LogError("speed"+ SettingInGame_02.instance.set_MoveSpeed[Game02_Main.instance.Index_JieDuan] + "    "+ trueSpeed);
        if (runTime >= trueSpeed)
        {  //speed
            runTime = 0;
            if (delayTime > 0)
            {
                delayTime--;
            }
            else
            {
            
                switch (animMode)
                {
                    case enAnimMode.LeftToRight:
                        RunAnim_LeftToRight();
                        break;
                    case enAnimMode.RightToLeft:
                        RunAnim_RightToLeft();
                        break;
                    case enAnimMode.LRRL:
                        RunAnim_LRRL();
                        break;
                    case enAnimMode.UpToDown:
                        RunAnim_UpToDown();
                        break;
                    case enAnimMode.DownToUp:
                        RunAnim_DownToUp();
                        break;
                    case enAnimMode.UDDU:
                        RunAnim_UDDU();
                        break;
                    case enAnimMode.SmallToBig:
                        RunAnim_SmallToBig();
                        break;
                    case enAnimMode.BigToSmall:
                        RunAnim_BigToSmall();
                        break;
                    case enAnimMode.SBBS:
                        RunAnim_SBBS();
                        break;
                    case enAnimMode.TurnLeft:
                        RunAnim_TurnLeft();
                        break;
                    case enAnimMode.TurnRight:
                        RunAnim_TurnRight();
                        break;
                    case enAnimMode.TurnLR:
                        RunAnim_TurnLR();
                        break;
                    
                    break;
                }
               // RunAnim_FlyPlane();
            }
        }
        Show();
    }
    void ShowFlyButtle()
    {
        flyButtle = GameObject.FindGameObjectsWithTag("buttle");
        if (flyButtle.Length>0)
        {
            for (int i = 0; i < flyButtle.Length; i++)
            {
                int x, y;
                for (int k = 0; k < flyButtle[i].gameObject.GetComponent<LedAnim_Buttle>().pos_Attack.Length; k++)
                {
                    x = (int)flyButtle[i].gameObject.GetComponent<LedAnim_Buttle>().pos_Attack[k].x;
                    y = (int)flyButtle[i].gameObject.GetComponent<LedAnim_Buttle>().pos_Attack[k].y;
                    if (x>=0&&y>=0)
                    {
                        DrawPic.DrawRol(x, y, 1, tab_Color[(int)color], enPointSta.Die);

                    }

                }


            }
        }
     
    }
    void ShowFlyPlane()
    {
        //for (int i = 0; i < flyPlane.Length; i++)
        //{
        //    for (int k = 0; k < flyPlane[i].pos_Now.Length; k++)
        //    {

        //      //  DrawPic.DrawRol((int)flyPlane[i].pos_Now[k].x, (int)flyPlane[i].pos_Now[k].y, 1, tab_Color[(int)color], enPointSta.Die);
        //        DrawPic.DrawRol((int)flyPlane[0].pos_Now[k].x, (int)flyPlane[0].pos_Now[k].y, 1, tab_Color[(int)color], enPointSta.Die);

        //    }
        //}
    }
    public void Show()
    {
     //   ShowFlyPlane();
      //  ShowFlyButtle();
        //Debug.Log ("ShowAnim: " + ledType + ", " + picType + ", " + animMode + ", " + x + ", " + y + ", " + width + ", " + height + ", " + value);
        if (animMode == enAnimMode.BigToSmall || animMode == enAnimMode.SmallToBig)
        {
            DrawPic.DrawCirculation(x, y, radio, tab_Color[(int)color], pointSta);

        }
        else
        {

            switch (picType)
            {
                case enPicType.Col:
                    DrawPic.DrawCol(x, y, height, tab_Color[(int)color], pointSta);
                    break;
                case enPicType.Rol:
                    DrawPic.DrawRol(x, y, width, tab_Color[(int)color], pointSta);
                    break;
                case enPicType.Rectangle:
                    DrawPic.DrawRectangle(x, y, width, height, tab_Color[(int)color], pointSta);
                    break;
                case enPicType.Prismatic:
                    DrawPic.DrawPrismatic_ByCenter(x, y, width, tab_Color[(int)color], pointSta, limitLeft, limitRight, limitUp, limitDown);
                    break;
            }
        }

    }

    void RunAnim_LeftToRight()
    {
        if (x + width <= endRunPos)
        {
            x++;
        }
        else if (loop == 0)
        {
            statue = 0;
        }
        else
        {
            x = startRunPos;
        }
    }
    void RunAnim_RightToLeft()
    {
        if (x > endRunPos)
        {
            x--;
        }
        else if (loop == 0)
        {
            statue = 0;
        }
        else
        {
            x = startRunPos;
        }
    }
    void RunAnim_LRRL()
    {
        if (dir == 0)
        {
            if (x + width <= endRunPos)
            {
                x++;
            }
            if (x + width > endRunPos)
            {
                dir = 1;
            }
        }
        else
        {
            if (x > startRunPos)
            {
                x--;
            }
            if (x <= startRunPos)
            {
                if (loop == 0)
                {
                    statue = 0;
                }
                else
                {
                    dir = 0;
                }
            }
        }
    }
    void RunAnim_UpToDown()
    {
        if (y + height <= endRunPos)
        {
            y++;
        }
        else if (loop == 0)
        {
            statue = 0;
        }
        else
        {
            y = startRunPos;
        }
    }
    void RunAnim_DownToUp()
    {
        if (y > endRunPos)
        {
            y--;
        }
        else if (loop == 0)
        {
            statue = 0;
        }
        else
        {
            y = startRunPos;
        }
    }
    void RunAnim_UDDU()
    {
        if (dir == 0)
        {
            if (y + height <= endRunPos)
            {
                y++;
            }
            if (y + height > endRunPos)
            {
                dir = 1;
            }
        }
        else
        {
            if (y > startRunPos)
            {
                y--;
            }
            if (y <= startRunPos)
            {
                if (loop == 0)
                {
                    statue = 0;
                }
                else
                {
                    dir = 0;
                }
            }
        }
    }
    void RunAnim_SmallToBig()
    {
        if (radio <= endRunPos)
        {
            radio++;
        }
        else if (loop == 0)
        {
            statue = 0;
        }
        else
        {
            radio = 0;// startRunPos;
        }
    }
    void RunAnim_BigToSmall()
    {
        if (radio > endRunPos)
        {
            radio--;
        }
        else if (loop == 0)
        {
            statue = 0;
        }
        else
        {
            radio = startRunPos;
        }
    }
    void RunAnim_SBBS()
    {
        if (dir == 0)
        {
            if (radio <= endRunPos)
            {
                radio++;
            }
            if (radio > endRunPos)
            {
                dir = 1;
            }
        }
        else
        {
            if (radio > startRunPos)
            {
                radio--;
            }
            if (radio <= startRunPos)
            {
                if (loop == 0)
                {
                    statue = 0;
                }
                else
                {
                    dir = 0;
                }
            }
        }
    }


    int LEDANIM_Run_Turn()
    {
        if (runSta == 0)
        {
            // Ïò×ó
            if (x > limitLeft)
            {
                x--;
            }
            if (x > limitLeft)
            {
                return 0;
            }
        }
        else if (runSta == 1)
        {
            // ÏòÏÂ
            if (y > limitDown)
            {
                y--;
            }
            if (y > limitDown)
            {
                return 0;
            }
        }
        else if (runSta == 2)
        {
            // ÏòÓÒ
            if (x + width <= limitRight)
            {
                x++;
            }
            if (x + width <= limitRight)
            {
                return 0;
            }
        }
        else if (runSta == 3)
        {
            // ÏòÉÏ
            if (y + height <= limitUp)
            {
                y++;
            }
            if (y + height <= limitUp)
            {
                return 0;
            }
        }
        else
        {
            return 1;
        }
        if (getStartXY == 0)
        {
            getStartXY = 1;
            //x
            startRunPos = x;
            if (startRunPos < limitLeft)
                startRunPos = limitLeft;
            if (startRunPos > limitRight)
                startRunPos = limitRight;
            //
            startRunPos = y;
            if (startRunPos < limitDown)
                startRunPos = limitDown;
            if (startRunPos > limitUp)
                startRunPos = limitUp;
        }
        return 1;
    }
    void RunAnim_TurnLeft()
    {
        int runEnd = LEDANIM_Run_Turn();
        if (startRunPos == x && startRunPos == y)
        {
            if (loop == 0)
            {
                statue = 0;
                return;
            }
        }
        if (runEnd == 0)
            return;
        //
        if (++runSta >= 4)
        {
            runSta = 0;
        }
    }
    void RunAnim_TurnRight()
    {
        int runEnd = LEDANIM_Run_Turn();
        if (startRunPos == x && startRunPos == y)
        {
            if (loop == 0)
            {
                statue = 0;
                return;
            }
        }
        if (runEnd == 0)
            return;
        //
        if (runSta > 0)
        {
            runSta--;
        }
        else
        {
            runSta = 3;
        }
    }

    void RunAnim_FlyPlane()
    {
        //if (flyPlane[0].gameObject.activeSelf)
        //{
        //    for (int i = 0; i < flyPlane.Length; i++)
        //    {
        //        flyPlane[i].Moveing();
              
        //    }
          
        //}
               

        if (flyButtle.Length>0)
        {
            for (int i = 0; i < flyButtle.Length; i++)
            {
                flyButtle[i].gameObject.GetComponent<LedAnim_Buttle>().Move();

            }
        }
    }
        void RunAnim_TurnLR()
    {
        int runEnd = LEDANIM_Run_Turn();
        //
        if (startRunPos == x && startRunPos == y)
        {
            if (loop == 0)
            {
                statue = 0;
                return;
            }
            if (cnt != 0)
            {
                // ×óÓÒ/ÉÏÏÂ,·´·½Ïò		
                runSta = (runSta + 2) % 4;
                if (++dir >= 2)
                {
                    dir = 0;
                }
                return;
            }
            cnt = 1;
        }
        if (runEnd == 0)
            return;
        if (dir == 0)
        {
            if (++runSta >= 4)
            {
                runSta = 0;
            }
        }
        else
        {
            if (runSta > 0)
            {
                runSta--;
            }
            else
            {
                runSta = 3;
            }
        }
    }
}
