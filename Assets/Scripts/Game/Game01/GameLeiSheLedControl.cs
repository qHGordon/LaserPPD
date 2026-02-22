using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLeiSheLedControl
{

    PresetPic presetPic;
    LedAnim[] ledAnim = new LedAnim[10];
    int animCount;
    float ChangeTime = 0;
    float MaxChangeTime = 20;

    public GameLeiSheLedControl()
    {
        for (int i = 0; i < ledAnim.Length; i++)
        {
            ledAnim[i] = new LedAnim();
        }
    }

    /// <summary>
    /// LED灯ID与坐标关系  
    /// </summary>
    public static Dictionary<int, Vector2Int> ledCoordinate = new Dictionary<int, Vector2Int>();
    public static void LedCoordinate()
    {
        for (int x = 0; x < Set.setVal.Width; x++)
        {
            for (int y = 0; y < Set.setVal.Height; y++)
            {
                int id = Framebuffer.MappingId(x, y);
                if (!ledCoordinate.ContainsKey(id))
                {
                    ledCoordinate[id] = new Vector2Int(x, y);
                }
            }
        }
    }
    public void LedInit(GameLevelSetting gameLevelSetting)
    {

        MaxChangeTime = Random.Range(5, 20);
        ChangeTime = MaxChangeTime;
        Pic_Init(gameLevelSetting.picSetting);
        AnimsInit(gameLevelSetting.animSetting);
    }
    public void RunStart()
    {
        AnimsRunStart();
    }
    void GetNewPoint()
    {
        if (Main.playerMode != en_PlayerMode.Challenge)
        {
            return;
        }
        int tarageNum = Random.Range(0, Set.setVal.Width * Set.setVal.Height);
        int maxCnt = 300;
        while (PrePic_Copy[tarageNum] == 1 && maxCnt > 0)
        {
            maxCnt--;
            tarageNum = Random.Range(0, Set.setVal.Width * Set.setVal.Height);
        }
        PrePic_Copy[tarageNum] = 1;
    }
    void ChangeMap()
    {
        if (Main.playerMode == en_PlayerMode.Free)
        {
            return;
        }
        int tarageNum = Random.Range(0, Set.setVal.Width * Set.setVal.Height);
        int maxCnt = 300;
        while (PrePic_Copy[tarageNum] == 0 && maxCnt > 0)
        {
            maxCnt--;
            tarageNum = Random.Range(0, Set.setVal.Width * Set.setVal.Height);
        }
        PrePic_Copy[tarageNum] = 0;
        tarageNum = Random.Range(0, Set.setVal.Width * Set.setVal.Height);
        maxCnt = 300;
        while (PrePic_Copy[tarageNum] == 1 && maxCnt > 0)
        {
            maxCnt--;
            tarageNum = Random.Range(0, Set.setVal.Width * Set.setVal.Height);
        }
        PrePic_Copy[tarageNum] = 1;
        GetNewPoint();
    }
    public void Run()
    {
        GameLeiSheBase.Update_ColorFull(0, enPointSta.None);
        //ChangeTime -= Time.deltaTime;
        //if (ChangeTime <= 0)
        //{
        //    MaxChangeTime = Random.Range(5, 20);
        //    ChangeTime = MaxChangeTime;
        //    if (!Framebuffer.isNewLeiShe)
        //    {
        //        ChangeMap();

        //    }
        //}
        //Pic_Draw();
        //AnimsRun();
    }

    // PresetPic:-------------------------------------------------
    byte[] picBuf = new byte[Main.MAX_LED];
    void Pic_Init(PresetPic picSetting)
    {
        presetPic = picSetting;
        for (int i = 0; i < picBuf.Length; i++)
        {
            picBuf[i] = 0;
        }
        if (picSetting == null)
            return;
        int bufId;
        int picId;


        for (int i = 0; i < Set.setVal.Height; i++)
        {
            picId = i * PresetPic.PIC_WIDTH;
            bufId = i * Set.setVal.Width;
            for (int j = 0; j < Set.setVal.Width; j++)
            {
                picBuf[bufId + j] = picSetting.dataBuff[picId + j];
                if (picBuf[bufId + j] == 1)
                {
                    //                    Debug.LogError(i + "   " + j);
                }
            }
        }
        GetPic_Draw();
    }

#if UNITY_EDITOR && false
    public const uint LEISHE_COLOR = 0xfe00;
#else
    public const uint LEISHE_COLOR = 63;
#endif
    public static int[] PrePic_Copy;
    void GetPic_Draw()
    {
        if (presetPic == null)
            return;
        PrePic_Copy = new int[Set.setVal.Width * Set.setVal.Height];
        int id;
        int ledLen = Set.setVal.Width * Set.setVal.Height;


        for (int i = 0; i < ledLen; i++)
        {


            if (picBuf[i] != 0)
            {
                
                id = Framebuffer.tab_Mapping[i];
                if (id > PrePic_Copy.Length)
                {
                    Debug.Log("演示灯数越界");
                    continue;
                }
                PrePic_Copy[id] = 1;
                //            Debug.LogError("亮啊!!!!  " + id);

            }
        }

    }
    void Pic_Draw()
    {
        if (presetPic == null)
        {
            return;
        }
        int id;
        int ledLen = Set.setVal.Width * Set.setVal.Height;
        for (int i = 0; i < ledLen; i++)
        {
            //   id = Framebuffer.tab_Mapping[i];

            if (PrePic_Copy[i] != 0)//picBuf[id]
            {
                //Debug.LogError("要亮灯  " +i+"   "+ PrePic_Copy[i]);
                Framebuffer.Update_TransmitLedColor(i, LEISHE_COLOR, enPointSta.Target);
            }
            else
            {
                Framebuffer.Update_TransmitLedColor(i, 0, enPointSta.None);
            }

        }
    }
    public byte GetPicPointValue(int x, int y)
    {
        if (presetPic == null)
            return 0;
        //int id = x * Set.setVal.Height + y;
        int id = y * Set.setVal.Width + x;
        //id = Framebuffer.tab_Mapping[id];
        if (id < picBuf.Length)
        {
            //Debug.Log ("PicData_" + x + "_" + y + ": " + picBuf[id]);
            return picBuf[id];
        }
        return 0;
    }

    // LedAnim:----------------------------------------------------
    void Stop()
    {
        for (int i = 0; i < ledAnim.Length; i++)
        {
            ledAnim[i].statue = 0;
        }
    }
    void AnimsRunStart()
    {
        for (int i = 0; i < animCount && i < ledAnim.Length; i++)
        {
            ledAnim[i].RunStart();
        }
    }
    void AnimsInit(AnimSet animSetting)
    {
        if (animSetting == null)
        {
            for (int i = 0; i < ledAnim.Length; i++)
            {
                ledAnim[i].statue = 0;
            }
            animCount = 0;
            return;
        }
        animCount = animSetting.animCount;
        int startx = 0;
        int starty = 0;
        int width = Set.setVal.Width;
        int height = Set.setVal.Height;

        for (int i = 0; i < ledAnim.Length; i++)
        {
            ledAnim[i].statue = 0;
            if (i < animCount)
            {
                ledAnim[i].limitLeft = startx;
                ledAnim[i].limitRight = startx + width - 1;
                ledAnim[i].limitUp = starty + height - 1;
                ledAnim[i].limitDown = starty;
                //
                ledAnim[i].ledType = en_LedType.LeiShe;
                ledAnim[i].picType = animSetting.animOne[i].picType;
                ledAnim[i].animMode = animSetting.animOne[i].animMode;
                ledAnim[i].loop = animSetting.animOne[i].loop;
                ledAnim[i].value = 63;//                (byte)gameLevelSetting.animSetting.animOne[i].color;
                ledAnim[i].startx = animSetting.animOne[i].x;
                ledAnim[i].starty = animSetting.animOne[i].y;
                ledAnim[i].width = animSetting.animOne[i].width;
                ledAnim[i].height = animSetting.animOne[i].height;
                ledAnim[i].stepTime = animSetting.animOne[i].stepTime;
                ledAnim[i].delay = animSetting.animOne[i].delayTime;

                switch (ledAnim[i].picType)
                {
                    case enPicType.Col:
                        ledAnim[i].width = 1;
                        ledAnim[i].height = height;
                        break;
                    case enPicType.Rol:
                        ledAnim[i].width = width;
                        ledAnim[i].height = 1;
                        break;
                }

                // 校正开始/结束点
                //ledAnim[i].startx = startx;
                //ledAnim[i].starty = starty;
                //ledAnim[i].endx = startx + width - 1;
                //ledAnim[i].endy = starty + height - 1;
                switch (ledAnim[i].animMode)
                {
                    case enAnimMode.LeftToRight:
                    case enAnimMode.LRRL:
                        //ledAnim[i].startRunPos = x;
                        //ledAnim[i].endRunPos = x + width - 1;
                        ledAnim[i].startx = startx;
                        ledAnim[i].endx = startx + width - 1;
                        break;
                    case enAnimMode.RightToLeft:
                        //ledAnim[i].startRunPos = x + width - 1;
                        //ledAnim[i].endRunPos = x;
                        ledAnim[i].startx = startx + width - 1;
                        ledAnim[i].endx = startx;
                        break;

                    // ÉÏÏÂ
                    case enAnimMode.UpToDown:
                    case enAnimMode.UDDU:
                        //ledAnim[i].startRunPos = y;
                        //ledAnim[i].endRunPos = y + height - 1;
                        ledAnim[i].starty = starty;
                        ledAnim[i].endy = starty + height - 1;
                        break;
                    case enAnimMode.DownToUp:
                        //ledAnim[i].startRunPos = y + height - 1;
                        //ledAnim[i].endRunPos = y;
                        ledAnim[i].starty = starty + height - 1;
                        ledAnim[i].endy = starty;
                        break;

                    // ×ªÈ¦
                    case enAnimMode.TurnLeft:     // ×ó×ªÈ¦
                    case enAnimMode.TurnRight:        // ÓÒ×ªÈ¦
                    case enAnimMode.TurnLR:       // ×óÓÒÀ´»Ø×ª

                        break;
                }
            }
        }
    }
    void AnimsRun()
    {
        for (int i = 0; i < ledAnim.Length; i++)
        {
            if (ledAnim[i].statue == 0)
                continue;
            //Debug.Log ("_" + i);
            ledAnim[i].Run();
        }
    }

}
