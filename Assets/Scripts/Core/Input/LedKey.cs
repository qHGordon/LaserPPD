using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{


public class LedKey
{
    static byte[] keyDown = new byte[AppConst.MAX_LED];
    static byte[] keyOld = new byte[AppConst.MAX_LED];

    public static byte[] newSta = new byte[AppConst.MAX_LED];
    public static byte[] holdSta = new byte[AppConst.MAX_LED];
    public static float[] holdTime = new float[AppConst.MAX_LED];

    const byte KEYDOWN_VALUE = 1;


    public static void Check()
    {
        for (int i = 0; i < AppConst.MAX_LED; i++)
        {
            if (holdSta[i] != newSta[i])
            {
                holdSta[i] = newSta[i];
                holdTime[i] = 0;
            }
            else if (newSta[i] == KEYDOWN_VALUE)
            { // 接收到激光：马上跳变
                keyOld[i] = holdSta[i];
            }
            else if (holdTime[i] < 0.05f)
            {         // 断开激光：0.1秒持续时间
                holdTime[i] += Time.deltaTime;
            }
            else if (keyOld[i] != holdSta[i])
            {
                keyOld[i] = holdSta[i];
            }
        }
    }

    public static void Clear()
    {
        for (int i = 0; i < keyDown.Length; i++)
        {
            keyDown[i] = 0;
        }
    }
    //TODO 触发返回的序号 镭射的序号是80~159 80个镭射，拍拍灯是通道5 序号位160~221（220和221分别是开始和结束） ，通道6 为222到281
    public static void Update_KeyValue(int id, byte value)
    {
        if (id >= keyDown.Length)
            return;
        if (keyOld[id] != KEYDOWN_VALUE && value == KEYDOWN_VALUE)
        {
            keyDown[id] = 1;
#if UNITY_EDITOR
            Debug.Log("LedKey_Press: " + id);
#endif
        }

        if (Set.setVal.GameChoose == (int)en_GameId.LeiSheWu)
        {
            if(id < 32 && value != newSta[id])
            {
                Debug.Log("激光变 id: " + id + ":" + value);
            }
            newSta[id] = value;
        }
        else
        {
            keyOld[id] = value;
        }
    }

    public static bool KeyPressed(int id)
    {
        if (id >= keyDown.Length)
            return false;
        if (keyDown[id] != 0)
        {
            keyDown[id] = 0;
            return true;
        }
        return false;
    }

    public static bool KeyStatus(int id)
    {
        if (id >= keyOld.Length)
            return false;
        if (keyOld[id] == KEYDOWN_VALUE)
        {
            return true;
        }
        return false;
    }
    public static byte GetKeyStatus(int id)
    {
        if (KeyStatus(id))
        {
            return 1;
        }
        return 0;
    }

    public static int GetLeiSheKeyStartId()
    {
        return Set.ChannelLength[0] + Set.ChannelLength[1];
    }
    public static int GetLeiSheStartButtonId()
    {
        int keyId = 0;
        for (int i = 0; i < Set.ChannelLength.Length && i < Framebuffer.TARGETLED_CH + 1; i++)
        {
            keyId += Set.ChannelLength[i];
        }
        keyId -= 2;
        return keyId;
    }
    public static int GetLeiSheWallLedKeyId()
    {
        int keyId = GetLeiSheStartButtonId() + 1;
        return keyId;
    }
   
    public static int GetLeiSheTargetKeyId()
    {
        int keyId = GetLeiSheWallLedKeyId() + Set.setVal.WallLedNum;
        return keyId;
    }

    public static int GetTouZhi_TarageLed1()
    {
        int keyId = GetLeiSheStartButtonId() + 2;
        return keyId;
    }
    public static int GetTouZhi_TarageLed2()
    {
        int keyId = GetLeiSheStartButtonId() + 3;
        return keyId;
    }
    public static int GetTouZhi_TarageLed3()
    {
        int keyId = GetLeiSheStartButtonId() + 4;
        return keyId;
    }


    public static int GetPanYan_DiBan_KeyId()//获得攀岩灯的,地板的开始ID,在通道6
    {
        int keyId1 = 0;
        for (int i = 0; i < Set.ChannelLength.Length && i <5; i++)
        {
            keyId1 += Set.ChannelLength[i];
        }
        int keyId = keyId1 + Set.setVal.WallLedNum;
        return keyId;
    }


}
}
