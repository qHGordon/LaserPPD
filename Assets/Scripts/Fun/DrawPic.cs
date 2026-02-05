using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPic
{
    readonly static byte[][] tab_Number_3x5 = {
        new byte[] {0x07,0x05,0x05,0x05,0x07},		// "0"
		new byte[] {0x02,0x06,0x02,0x02,0x07},		// "1"
		new byte[] {0x07,0x01,0x07,0x04,0x07},		// "2"
		new byte[] {0x07,0x01,0x07,0x01,0x07},		// "3"
		new byte[] {0x05,0x05,0x07,0x01,0x01},		// "4"
		new byte[] {0x07,0x04,0x07,0x01,0x07},		// "5"
		new byte[] {0x07,0x04,0x07,0x05,0x07},		// "6"
		new byte[] {0x07,0x01,0x01,0x01,0x01},		// "7"	
		new byte[] {0x07,0x05,0x07,0x05,0x07},		// "8"
		new byte[] {0x07,0x05,0x07,0x01,0x07},      // "9"
	};

    readonly static byte[][] tab_Number_5x7 = {
        new byte[] {0x70,0x88,0x98,0xA8,0xC8,0x88,0x70},		// "0"
		new byte[] {0x20,0x60,0x20,0x20,0x20,0x20,0x70},		// "1"
		new byte[] {0x70,0x88,0x08,0x30,0x40,0x80,0xF8},		// "2"
		new byte[] {0xF8,0x08,0x10,0x30,0x08,0x88,0x70},		// "3"
		new byte[] {0x10,0x30,0x50,0x90,0xF8,0x10,0x10},		// "4"
		new byte[] {0xF8,0x80,0xF0,0x08,0x08,0x88,0x70},		// "5"
		new byte[] {0x38,0x40,0x80,0xF0,0x88,0x88,0x70},		// "6"
		new byte[] {0xF8,0x08,0x10,0x20,0x40,0x40,0x40},		// "7"	
		new byte[] {0x70,0x88,0x88,0x70,0x88,0x88,0x70},		// "8"
		new byte[] {0x70,0x88,0x88,0x78,0x08,0x10,0xE0},		// "9"
	};

    readonly static byte[][] tab_Number_7x8 = {
        new byte[] {0x38,0x44,0x44,0x44,0x44,0x44,0x44,0x38}, // -0-
		new byte[] {0x30,0x10,0x10,0x10,0x10,0x10,0x10,0x7C}, // -1-
		new byte[] {0x38,0x44,0x04,0x08,0x10,0x20,0x44,0x7C}, // -2-
		new byte[] {0x38,0x44,0x04,0x18,0x04,0x04,0x44,0x38}, // -3-
		new byte[] {0x0C,0x14,0x14,0x24,0x44,0x7C,0x04,0x0C}, // -4-
		new byte[] {0x3C,0x20,0x20,0x38,0x04,0x04,0x44,0x38}, // -5-
		new byte[] {0x1C,0x20,0x40,0x78,0x44,0x44,0x44,0x38}, // -6-
		new byte[] {0x7C,0x44,0x04,0x08,0x08,0x08,0x10,0x10}, // -7-
		new byte[] {0x38,0x44,0x44,0x38,0x44,0x44,0x44,0x38}, // -8-
		new byte[] {0x38,0x44,0x44,0x44,0x3C,0x04,0x08,0x70}, // -9-
	};

    readonly static byte[][] tab_Number_8x10 = {
        new byte[] {0x7C,0xC6,0xC6,0xCE,0xD6,0xD6,0xE6,0xC6,0xC6,0x7C}, // -0-
		new byte[] {0x18,0x38,0x78,0x18,0x18,0x18,0x18,0x18,0x18,0x7E}, // -1-
		new byte[] {0x7C,0xC6,0x06,0x0C,0x18,0x30,0x60,0xC0,0xC6,0xFE}, // -2-
		new byte[] {0x7C,0xC6,0x06,0x06,0x3C,0x06,0x06,0x06,0xC6,0x7C}, // -3-
		new byte[] {0x0C,0x1C,0x3C,0x6C,0xCC,0xFE,0x0C,0x0C,0x0C,0x1E}, // -4-
		new byte[] {0xFE,0xC0,0xC0,0xC0,0xFC,0x0E,0x06,0x06,0xC6,0x7C}, // -5-
		new byte[] {0x38,0x60,0xC0,0xC0,0xFC,0xC6,0xC6,0xC6,0xC6,0x7C}, // -6-
		new byte[] {0xFE,0xC6,0x06,0x06,0x0C,0x18,0x30,0x30,0x30,0x30}, // -7-
		new byte[] {0x7C,0xC6,0xC6,0xC6,0x7C,0xC6,0xC6,0xC6,0xC6,0x7C}, // -8-
		new byte[] {0x7C,0xC6,0xC6,0xC6,0x7E,0x06,0x06,0x06,0x0C,0x78}, // -9-
	};


    //
    static bool InAear(int x, int y, int limitL, int limitR, int limitU, int limitD)
    {
        if (x < limitL)
            return false;
        if (x > limitR)
            return false;
        if (y < limitD)
            return false;
        if (y > limitU)
            return false;
        return true;
    }

    public static void DrawPoint(int x, int y, uint color, enPointSta statue)
    {
        if (x < 0 || y < 0)
        {
            return;
        }
        if (x > Set.setVal.Width - 1 || y > Set.setVal.Height)
        {
            return;
        }
        int id = y * Set.setVal.Width + x;

        if (id >= Set.setVal.Width * (Set.setVal.Height + Set.setVal.WallNum_Height))
        {
            return;
        }
        //if (Framebuffer.ledEnable[id] == 0)
        //    return;
        id = Framebuffer.tab_Mapping[id];
        if (GameLedControl.gamePoint[id].errorTime <= 0)
        {
            DrawPointId(id, color, statue);
        }
    }
    public static void DrawPointId(int id, uint color, enPointSta statue)
    {
        if (id >= Framebuffer.led.Length)
            return;
        if (Framebuffer.led[id].statue == enPointSta.Rest)
        {
            //     Debug.LogError(Framebuffer.led.Length + "    " + id);
            return;
        }
        Framebuffer.Update_PointColor(id, color, statue);
    }
    public static void DrawPointIdByCh(int ch, int id, uint color, enPointSta statue)//亮指定颜色指定通道的序号id的灯
    {
        if (id >= Framebuffer.led.Length)
            return;
        if (Framebuffer.led[id].statue == enPointSta.Rest)
        {
            //     Debug.LogError(Framebuffer.led.Length + "    " + id);
            return;
        }
        Framebuffer.Update_PointColor(ch, id, color, statue);
    }
    public static void DrawCol(int x, int y, int len, uint color, enPointSta statue)
    {
        for (int i = 0; i < len; i++)
        {
            DrawPoint(x, y + i, color, statue);
        }
    }
    public static void DrawCirculation(int x, int y, int ridio, uint color, enPointSta statue)//画从小到大的圈
    {
        int OX = Set.setVal.Width / 2 - 1;
        int OY = Set.setVal.Height / 2 - 1;
        int minX = OX - ridio;
        int maxX = OX + ridio;
        int minY = OY - ridio;
        int maxY = OY + ridio;
        for (int i = minY; i < maxY; i++)
        {
            if (minX >= 0)
            {
                DrawPoint(minX, i, color, statue);
            }
            if (maxX <= Set.setVal.Width - 1)
            {
                DrawPoint(maxX, i, color, statue);
            }
        }
        for (int k = minX; k < maxY; k++)
        {
            if (minY >= 0)
            {
                DrawPoint(k, minY, color, statue);
            }
            if (maxY <= Set.setVal.Height)
            {
                DrawPoint(k, maxY, color, statue);
            }
        }
    }

    public static void DrawRol(int x, int y, int len, uint color, enPointSta statue)
    {
        for (int i = 0; i < len; i++)
        {
            DrawPoint(x + i, y, color, statue);
        }
    }
    public static void DrawRectangleKuang(int x, int y, int width, int height, uint color, enPointSta statue)
    {
        DrawRol(x, y, width, color, statue);
        DrawRol(x, y + height - 1, width, color, statue);
        DrawCol(x, y + 1, height - 2, color, statue);
        DrawCol(x + width - 1, y + 1, height - 2, color, statue);
    }
    public static void DrawRectangle(int x, int y, int width, int height, uint color, enPointSta statue)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                DrawPoint(x + i, y + j, color, statue);
            }
        }
    }

    // 画棱形
    public static void DrawPrismatic_ByCenter(int x, int y, int radius, uint color, enPointSta sta, int limitL, int limitR, int limitU, int limitD)
    {
        DrawSlope(x + radius - 1, y, radius, color, sta, limitL, limitR, limitU, limitD);
        DrawBackSlash(x - radius + 1, y, radius, color, sta, limitL, limitR, limitU, limitD);
        DrawSlope(x + radius - 1, y, radius, color, sta, limitL, limitR, limitU, limitD);
        DrawBackSlash(x - radius + 1, y, radius, color, sta, limitL, limitR, limitU, limitD);
    }
    // 画斜线:从左到右，从下到上
    public static void DrawSlope(int x, int y, int len, uint color, enPointSta sta, int limitL, int limitR, int limitU, int limitD)
    {
        int addx, addy;
        addx = 1;
        addy = 1;

        for (int i = 0; i < len; i++)
        {
            if (InAear(x, y, limitL, limitR, limitU, limitD))
            {
                DrawPoint(x, y, color, sta);
            }
            x += addx;
            y += addy;
        }
    }
    // 画反斜线:从左到右，从上到下
    public static void DrawBackSlash(int x, int y, int len, uint color, enPointSta sta, int limitL, int limitR, int limitU, int limitD)
    {
        int addx, addy;
        addx = 1;
        addy = -1;

        for (int i = 0; i < len; i++)
        {
            if (InAear(x, y, limitL, limitR, limitU, limitD))
            {
                DrawPoint(x, y, color, sta);
            }
            x += addx;
            y += addy;
        }
    }

    // 画数字
    public static void DrawNumber(int value, int x, int y, int size, int dir, uint color)
    {
#if UNITY_EDITOR
        //        Debug.Log ("DrawNumber: " + value + ", x: " + x + ", y: " + y);
#endif
        int i, j;
        int id;
        int width;
        int height;
        byte bit = 0;
        byte[] buf;
        byte dat;
        //
        if (value > 10)
            return;
        switch (size)
        {
            case 0:
                buf = tab_Number_3x5[value];
                width = 3;
                height = 5;
                break;
            case 1:
                buf = tab_Number_5x7[value];
                width = 5;
                height = 7;
                break;
            case 2:
                buf = tab_Number_7x8[value];
                width = 7;
                height = 8;
                break;
            case 3:
                buf = tab_Number_8x10[value];
                width = 8;
                height = 10;
                break;
            default:
                return;
        }

        id = 0;
        for (i = 0; i < height; i++)
        {
            if (bit > 0)
                id++;
            bit = 0;
            //dat = buf[height - 1 - id];
            dat = buf[id];
            for (j = 0; j < width; j++)
            {
                //if(dat & (0x80 >> bit)){
                if ((dat & (1 << bit)) != 0)
                {
                    switch (dir)
                    {
                        case 0:
                            //    DrawPoint (x + j, y + i, color, 0);
                            DrawPoint(x + width + 1 - j, y + height - 1 - i, color, 0);
                            break;
                        case 1:
                            DrawPoint(x + i, y + height - j, color, 0);
                            break;
                        case 2:
                            //  DrawPoint (x + width + 1 - j, y + height - 1 - i, color, 0);
                            DrawPoint(x + j, y + i, color, 0);
                            break;
                        case 3:
                            DrawPoint(x + width - i, y + j, color, 0);
                            break;
                    }
                }
                if (++bit >= 8)
                {
                    bit = 0;
                    id++;
                }
            }
        }
    }
}
