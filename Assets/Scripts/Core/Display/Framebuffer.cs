using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct LED
{
    public enPointSta statue;
    public uint color;
    public uint data;
    public uint colorOld;
    public byte keyDown;
    public byte keyOld;
    public float cdTime;
    public byte cdCnt;
    public float waitTime;
}

[Serializable]
public class LedEnableSet
{
    public byte[] ledEnableTab;
}

/// <summary>[Core.Display] LED 显示缓冲，管理逻辑坐标与硬件点位映射，下发颜色到 LED 硬件。</summary>
public class Framebuffer
{
    public static LED[] led = new LED[Main.MAX_LED];
    /// <summary>逻辑坐标 (x,y) 到硬件点位索引的映射，picid = x + Width * y</summary>
    public static int[] tab_Mapping = new int[Main.MAX_LED];
    public static int[] tab_PosMapping = new int[Main.MAX_LED];
    public static byte[,] ledMask = new byte[PresetPic.PIC_WIDTH, PresetPic.PIC_HEIGHT];
    public static bool isNewLeiShe = true;

    public static void defaultSetting()
    {
        for (int y = 0; y < PresetPic.PIC_HEIGHT; y++)
        {
            for (int x = 0; x < PresetPic.PIC_WIDTH; x++)
            {
                ledMask[x, y] = 0;
            }
        }
        SaveSetting(ledMask);
    }
    public static void SaveSetting(byte[,] setBuf)
    {
        string fileName = Application.persistentDataPath + "/LedMaskSet";

        //Jsion保存
#if UNITY_EDITOR
        Debug.Log("SaveLedMaskSet: " + fileName);
#endif

        LedEnableSet ledEnableSet = new LedEnableSet();
        ledEnableSet.ledEnableTab = new byte[PresetPic.PIC_WIDTH * PresetPic.PIC_HEIGHT];
        for (int y = 0; y < PresetPic.PIC_HEIGHT; y++)
        {
            for (int x = 0; x < PresetPic.PIC_WIDTH; x++)
            {
                ledMask[x, y] = setBuf[x, y];
                ledEnableSet.ledEnableTab[y * PresetPic.PIC_WIDTH + x] = setBuf[x, y];
            }
        }
        string json = JsonUtility.ToJson(ledEnableSet);
        StreamWriter fs = new StreamWriter(fileName);
        fs.Write(json);
        //
        fs.Close();
    }
    public static void LoadSetting()
    {
        string fileName = Application.persistentDataPath + "/LedMaskSet"; 
        LedEnableSet ledMaskSet = null;
        if (File.Exists(fileName))
        {
            try
            {
                //Jsion保存
                string json = File.ReadAllText(fileName);
                ledMaskSet = JsonUtility.FromJson<LedEnableSet>(json);
            }
            catch (Exception e)
            {
                ledMaskSet = null;
                Debug.LogError("AnimSetting Load Error: " + e);
            }
        }
        //
        if (ledMaskSet == null)
        {

            defaultSetting();
            return;
        }
        if (ledMaskSet.ledEnableTab == null)
        {
            defaultSetting();
            return;
        }
        if ((PresetPic.PIC_HEIGHT - 1) * (PresetPic.PIC_WIDTH - 1) > ledMaskSet.ledEnableTab.Length)
        {
            defaultSetting();
            return;
        }
        for (int y = 0; y < PresetPic.PIC_HEIGHT; y++)
        {
            for (int x = 0; x < PresetPic.PIC_WIDTH; x++)
            {


                ledMask[x, y] = ledMaskSet.ledEnableTab[y * PresetPic.PIC_WIDTH + x];
            }
        }
    }


    public static void Update_KeyStatue(int no, byte sta)
    {
        if (no >= led.Length)
            return;
        led[no].keyDown |= sta;
        //if (led[no].keyOld != 0 && sta == 0) {
        //          led[no].keyDown = 1;
        //      }
        //      led[no].keyOld = sta;
    }
    public static bool KeyStatus(int no)
    {
        if (no >= led.Length)
            return false;
        if (led[no].keyOld == 0)
            return true;
        return false;
    }

    public static bool KeyPressed(int no)
    {
        if (no >= led.Length)
            return false;
        if (led[no].keyDown != 0)
        {
            led[no].keyDown = 0;
            return true;
        }
        return false;
    }

    // 计算映射表：
    public static void MapTabInit()
    {
        LoadSetting();
        int i, j;
        int id = 0;
        int x = 0;
        int y = 0;
        int dir = 0;
        int len = 0;

        int mId;
        int ledId = 0;

        j = 0; 
        switch (Set.setVal.Index_AnZhuang)//
        {
            case 0://"左下往右");
                x = 0;
                y = 0;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (x + 1 < Set.setVal.Width)
                            {
                                x++;
                            }
                            else
                            {
                                y++;
                                dir = 1;
                            }
                        }
                        else if (x > 0)
                        {
                            x--;
                        }
                        else
                        {
                            y++;
                            dir = 0;
                        }
                    }
                    if (y >= Set.setVal.Height + Set.setVal.WallNum_Height)
                    {
                        break;
                    }
                }
                break;
            case 1://左下  上
                x = 0;
                y = 0;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (y + 1 < Set.setVal.Height + Set.setVal.WallNum_Height)
                            {
                                y++;
                            }
                            else
                            {
                                x++;
                                dir = 1;
                            }
                        }
                        else if (y > 0)
                        {
                            y--;
                        }
                        else
                        {
                            x++;
                            dir = 0;
                        }
                    }
                    if (x >= Set.setVal.Width)
                    {
                        break;
                    }
                }
                break;
            case 2:// 左上  右
                x = 0;
                y = Set.setVal.Height + Set.setVal.WallNum_Height - 1;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (x + 1 < Set.setVal.Width)
                            {
                                x++;
                            }
                            else
                            {
                                y--;
                                dir = 1;
                            }
                        }
                        else if (x > 0)
                        {
                            x--;
                        }
                        else
                        {
                            y--;
                            dir = 0;
                        }
                    }
                    if (y < 0)
                    {
                        break;
                    }
                }
                break;
            case 3:// 左上往下
                x = 0;
                y = Set.setVal.Height + Set.setVal.WallNum_Height - 1;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (y > 0)
                            {
                                y--;
                            }
                            else
                            {
                                x++;
                                dir = 1;
                            }
                        }
                        else if (y + 1 < Set.setVal.Height + Set.setVal.WallNum_Height)
                        {
                            y++;
                        }
                        else
                        {
                            x++;
                            dir = 0;
                        }
                    }
                    if (x >= Set.setVal.Width)
                    {
                        break;
                    }
                }
                break;

            case 4:// 右上往下
                x = Set.setVal.Width - 1;
                y = Set.setVal.Height + Set.setVal.WallNum_Height - 1;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (y > 0)
                            {
                                y--;
                            }
                            else
                            {
                                x--;
                                dir = 1;
                            }
                        }
                        else if (y + 1 < Set.setVal.Height + Set.setVal.WallNum_Height)
                        {
                            y++;
                        }
                        else
                        {
                            x--;
                            dir = 0;
                        }
                    }
                    if (x < 0)
                    {
                        break;
                    }
                }
                break;
            case 5:// 右上往左
                x = Set.setVal.Width - 1;
                y = Set.setVal.Height + Set.setVal.WallNum_Height - 1;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (x > 0)
                            {
                                x--;
                            }
                            else
                            {
                                y--;
                                dir = 1;
                            }

                        }
                        else if (x + 1 < Set.setVal.Width)
                        {
                            x++;
                        }
                        else
                        {
                            y--;
                            dir = 0;
                        }
                    }
                    if (y < 0)
                    {
                        break;
                    }
                }
                break;
            case 6:// 右下往上
                x = Set.setVal.Width - 1;
                y = 0;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (y + 1 < Set.setVal.Height + Set.setVal.WallNum_Height)
                            {
                                y++;
                            }
                            else
                            {
                                x--;
                                dir = 1;
                            }
                        }
                        else if (y > 0)
                        {
                            y--;
                        }
                        else
                        {
                            x--;
                            dir = 0;
                        }
                    }
                    if (x < 0)
                    {
                        break;
                    }
                }
                break;
            case 7:// 右下往左
                x = Set.setVal.Width - 1;
                y = 0;

                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height + Set.setVal.WallNum_Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }
                        if (dir == 0)
                        {
                            if (x > 0)
                            {
                                x--;
                            }
                            else
                            {
                                y++;
                                dir = 1;
                            }

                        }
                        else if (x + 1 < Set.setVal.Width)
                        {
                            x++;
                        }
                        else
                        {
                            y++;
                            dir = 0;
                        }
                    }
                    if (y >= Set.setVal.Height + Set.setVal.WallNum_Height)
                    {
                        break;
                    }
                }
                break;
            case 8:   // 左上往下,镭射



                x = 0;
                y = Set.setVal.Height - 1;

                int idd = 0;
                for (i = 0; i < Main.MAX_CH; i++)
                {
                    len = Set.ChannelLength[i];
                    for (j = 0; j < len && x >= 0 && x < Set.setVal.Width && y >= 0 && y < Set.setVal.Height;)
                    {
                        mId = y * Set.setVal.Width + x;
                        tab_Mapping[mId] = ledId;
                        tab_PosMapping[ledId] = mId;
                        // Debug.LogError("mId  " + mId + " ledId  " + ledId);
                        if (ledMask[x, y] == 0)
                        {
                            ledId++;
                            j++;
                        }

                        if ((Set.setVal.Height - 1 - y) % 2 == 0)
                        {
                            GameLeiSheBase.tab_Point[idd] = x + Set.setVal.Width * ((Set.setVal.Height - 1 - y));
                        }
                        else
                        {
                            GameLeiSheBase.tab_Point[idd] = Set.setVal.Width * ((Set.setVal.Height - y)) - x - 1;

                        }

                        idd++;

                        if (y > 0)
                        {
                            y--;
                        }
                        else
                        {
                            x++;

                            y = Set.setVal.Height - 1;

                        }
                    }
                    if (x >= Set.setVal.Width)
                    {
                        break;
                    }



                }

                break;
        }
#if UNITY_EDITOR  &&false
        for ( i = 0; i < Set.setVal.Width * Set.setVal.Height; i++) {
            Debug.LogError ("Map_" + i + ": " + tab_Mapping[i]);
        }
#endif
    }



    /// <summary>逻辑坐标 (列 x, 行 y) 转硬件点位索引，picid = x + Width * y</summary>
    public static int MappingId(int x, int y)
    {
        int id = y * Set.setVal.Width + x;
        if (id >= tab_Mapping.Length)
            return 0;
        return tab_Mapping[id];
    }

    public static void Clear()
    {
        int i;

        for (i = 0; i < led.Length; i++)
        {
            led[i].statue = 0;
            led[i].color = 0;
            led[i].colorOld = 0;
            led[i].cdTime = 0;
            led[i].cdCnt = 0;
            led[i].waitTime = 0.5f;
        } 
        CmdIO_YDGZ.CMD0_SendCmd_LedAll(0);
    }
    public static void FullScreen(uint color, enPointSta sta)
    {
        int len = Mathf.Min(led.Length, Set.setVal.Width * (Set.setVal.Height + Set.setVal.WallNum_Height));
        for (int i = 0; i < len; i++)
        {
            led[i].statue = sta;
            led[i].color = color;
        }
    }



    static uint[] outBufId = new uint[256];

    static int GetColorIdBuf(uint startid, uint[] outBuf, uint value, uint offset, int maxId, int maxLen)
    {
        int len = 0;
        for (uint i = offset; i < maxId && i < led.Length && len < maxLen; i++)
        {
            if (led[i].colorOld == led[i].color)
                continue;
            if (value == led[i].color)
            {
                led[i].colorOld = led[i].color;
                led[i].waitTime = 0.05f * Set.setVal.KeyDelay;
                outBuf[len] = i - startid;
                len++;
#if UNITY_EDITOR
                //Debug.Log ("WaitTime_" + i);
#endif
            }
        }
        return len;
    }

    public static void MapBuffer()
    {
        uint i;
        int len;
        int startId;
        int chLen;
        int ch;
        int totalLen = Set.setVal.Width * (Set.setVal.Height + Set.setVal.WallNum_Height);

        ch = 0;
        startId = 0;
        chLen = Set.ChannelLength[ch];
        for (i = 0; i < totalLen && i < led.Length; i++)
        {  // MAX_CH
            if (led[i].waitTime > 0)
            {
                led[i].waitTime -= Time.deltaTime;
#if UNITY_EDITOR
                //                Debug.LogError("WaitTime_" + i + " ??" + led[i].waitTime );
#endif
            }
            if (i >= chLen)
            {
                startId += Set.ChannelLength[ch];
                ch++;
                if (ch >= Main.MAX_CH)
                {
                    break;
                }
                chLen += Set.ChannelLength[ch];
            }

            if (led[i].colorOld != led[i].color)
            {

                len = GetColorIdBuf((uint)startId, outBufId, led[i].color, i, chLen, 120);
                if (len > 0)
                {
#if UNITY_EDITOR
                    string str = "";
                    for (int l = 0; l < len; l++)
                    {
                        str += outBufId[l] + ", ";
                    }
                    //                    Debug.LogError ("Led_" + ch + "_One: " + led[i].color.ToString ("X") + ": " + str);
#endif
                    if (Set.LedProtocol[ch] == (int)en_LedProtocol.DianZhen)
                    {

                        CmdIO_YDGZ.CMD0_SendCmd_LedOne_DianZhen(2, led[i].color, targetLedBuf, 2);

                    }
                    else
                    {
                        CmdIO_YDGZ.CMD0_SendCmd_LedOne(ch, led[i].color, outBufId, len);

                    }
                }
            }
        }
    }

    // 
    const int MAX_DELAY = 100;
    static byte[,] leiSheBuf = new byte[Main.MAX_LED, MAX_DELAY];
    static int leiSheBufIndex = 0;

    public static void ClearDelayBuf()
    {
        for (int i = 0; i < Main.MAX_LED; i++)
        {
            for (int j = 0; j < MAX_DELAY; j++)
            {
                leiSheBuf[i, j] = 0;
            }
        }
    }

    public static byte GetDelayValue(int no, int delay)
    {
        if (no >= led.Length)
            return 0;
        if (delay >= MAX_DELAY)
        {
            delay %= MAX_DELAY;
        }
        //int id = (leiSheBufIndex[no] + MAX_DELAY - delay) % MAX_DELAY;
        int id;
        if (leiSheBufIndex >= delay)
        {
            id = leiSheBufIndex - delay;
        }
        else
        {
            id = leiSheBufIndex + MAX_DELAY - delay;
        }
        return leiSheBuf[no, id];
    }


    public static void Update_PointColor_DianZhen(int id, uint color, uint data, enPointSta statue)
    {
        if (id >= led.Length)
            return;
        led[id].statue = statue;
        led[id].color = color;
        led[id].data = data;
    }
    public static void Update_PointColor(int id, uint color, enPointSta statue)
    {
        //  Debug.LogError("直接发送了颜色变化" + "ID"  +id);
        if (id >= led.Length)
            return;
        led[id].statue = statue;
        led[id].color = color;
    }
    public static void Update_PointColor(int x, int y, uint color, enPointSta statue)
    {

        if (x >= Set.setVal.Width || y >= (Set.setVal.Height + Set.setVal.WallNum_Height))
            return;
        if (x < 0 || y < 0)
            return;
        int id = y * Set.setVal.Width + x;
        //int id = x * Set.setVal.Height + y;
        if (id >= tab_Mapping.Length)
            return;
        id = tab_Mapping[id];
        Update_PointColor(id, color, statue);
      
    }
    public static void Update_ColorFull(uint color, enPointSta statue)
    {
        for (int i = 0; i < led.Length; i++)
        {
            led[i].statue = statue;
            led[i].color = color;
            led[i].colorOld = color;
        }
        uint CCol;
        CCol = ((uint)(ledCount[0] << 17) | (color & 0x7f));
        if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu && isNewLeiShe)
        {
            //            Debug.LogError(CCol);
            CmdIO_YDGZ.CMD0_SendCmd_LedAll(CCol);
        }
        else
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedAll(color);

        }
    }








    // 镭射灯用
    public const int MAX_LED_CH = 2;
    public const int RECEIVELED_CH = 2;
    // 发射灯
    public static void Update_TransmitLedClearAll()
    {
        int len = Mathf.Min(Main.MAX_LED_ONE * MAX_LED_CH, Set.setVal.Width * Set.setVal.Height);
        for (int i = 0; i < len; i++)
        {
            led[i].color = 0;
            //led[i].colorOld = 0;
            led[i].waitTime = 0.05f * Set.setVal.KeyDelay;
        }
#if UNITY_EDITOR
        // Debug.Log("AllWaitTime_2");
#endif
    }
    static byte[] ledCount = new byte[Main.MAX_LED];

    public static void Init_LEDNum()
    {
        for (int i = 0; i < ledCount.Length; i++)
        {
            ledCount[i] = (byte)Set.setVal.Height;
        }
#if UNITY_EDITOR
        // Debug.LogError((byte)Set.setVal.Height);
#endif
    }


    public static void Update_TransmitLedColor(int id, uint color, enPointSta sta)
    {


        int ch = 0;
        if (id >= Set.ChannelLength[0])
        {
            id -= Set.ChannelLength[0];
            ch++;

        }
        uint CCol;
        CCol = ((uint)(ledCount[id] << 17) | (color & 0x7f));


        //Update_PointColor(ch, id, color);
#if UNITY_EDITOR
        // Debug.LogError ("TransLed: " + ch + "-" + id + ": " + CCol.ToString ("X"));
#endif
        if (isNewLeiShe)
        {
            Update_PointColor(id, CCol, sta);
        }
        else
        {
            Update_PointColor(id, color, sta);
        }

    }
    public static void Update_TransmitLedColor(int x, int y, uint color, enPointSta sta)
    {
        if (x < 0 || y < 0)
            return;
        if (x >= Set.setVal.Width || y >= Set.setVal.Height)
            return;
        int id = y * Set.setVal.Width + x;
        if (id >= tab_Mapping.Length)
            return;
        id = tab_Mapping[id];
        Update_TransmitLedColor(id, color, sta);
    }
    public static void Update_ReceiveLedColor(int x, int y, uint color, enPointSta sta)
    {
        if (x < 0 || y < 0)
            return;
        if (x >= Set.setVal.Width || y >= Set.setVal.Height)
            return;
        if (ledMask[x, y] != 0)
            return;
        int id = y * Set.setVal.Width + x;
        if (id >= tab_Mapping.Length)
            return;
        id = tab_Mapping[id];
        //Debug.Log ("Update_PointColor: " + id + ": " + color);
        Update_PointColor(id, color, sta);
    }

    // 目标灯
    public const int TARGETLED_CH = 4;
    static uint[] targetLedBuf = new uint[Main.MAX_LED_ONE];
    public static void Update_TargetLedColorAll(uint color)
    {
        if (Set.ChannelLength[TARGETLED_CH] <= 0)
            return;
        for (int i = 0; i < Main.MAX_LED_ONE && i < Set.ChannelLength[TARGETLED_CH]; i++)
        {
            targetLedBuf[i] = (uint)i;
        }
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(TARGETLED_CH, color, targetLedBuf, Set.ChannelLength[TARGETLED_CH]);
    }
    public static void Update_TargetLedColorAll(int ch ,uint color)//亮指定颜色指定通道的所有灯
    {
        if (Set.ChannelLength[ch] <= 0)
            return;
        for (int i = 0; i < Main.MAX_LED_ONE && i < Set.ChannelLength[ch]; i++)
        {
            targetLedBuf[i] = (uint)i;
        }
        CmdIO_YDGZ.CMD0_SendCmd_LedOne(ch, color, targetLedBuf, Set.ChannelLength[ch]);
    }
    public static void Update_TargetLedColor(int id, uint color)
    {
        targetLedBuf[0] = (uint)id;

        uint CCol;
        CCol = ((uint)(ledCount[id] << 17) | (color & 0x7f));
        if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu && isNewLeiShe)
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne(TARGETLED_CH, CCol, targetLedBuf, 1);

        }
        else
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne(TARGETLED_CH, color, targetLedBuf, 1);

        }
    }
    public static void Update_TargetLedColor(int ch, int id, uint color)//亮指定颜色指定通道的序号id的灯
    {
        targetLedBuf[0] = (uint)id;

        uint CCol;
        CCol = ((uint)(ledCount[id] << 17) | (color & 0x7f));
        if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu && isNewLeiShe)
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne(ch, CCol, targetLedBuf, 1);

        }
        else
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne(ch, color, targetLedBuf, 1);
        }
    }
    public static void Update_TargetLedColor_DianZhen(int id, byte data, uint color)
    {
        targetLedBuf[0] = (uint)id;


        color |= (uint)data << 25;
        CmdIO_YDGZ.CMD0_SendCmd_LedOne_DianZhen(2, color, targetLedBuf, 1);

    }

    public static void Update_TargetLedColorAllByBuffer(uint color)
    {
        int startId = Set.setVal.Width * Set.setVal.Height;
        for (int i = 0; i < Main.MAX_LED_ONE && i < Set.ChannelLength[TARGETLED_CH]; i++)
        {
            led[i].color = color;
            //led[i].colorOld = 0;
            led[i].waitTime = 0.05f * Set.setVal.KeyDelay;
        }
#if UNITY_EDITOR
        Debug.Log("AllWaitTime_3");
#endif
    }
    public static void Update_TargetLedColorByBuffer(int id, uint color)
    {
        id += Set.setVal.Width * Set.setVal.Height;
        if (id >= 0 && id < led.Length)
        {
            led[id].color = color;
            led[id].waitTime = 0.05f * Set.setVal.KeyDelay;

#if UNITY_EDITOR
            Debug.Log("WaitTime_" + id + " 0.5");
#endif
        }
    }
}
