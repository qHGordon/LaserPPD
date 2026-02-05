using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enPicType
{
    Col,        // ??(?��??)
    Rol,        // ??(?��??)
    Rectangle,  // ????
    Prismatic,	// ????
}
public enum enAnimMode
{
    LeftToRight,   // ����????
    RightToLeft,   // ????����
    LRRL,          // ����??????
    UpToDown,      // ??????
    DownToUp,      // ??????
    UDDU,          // ????????
    SmallToBig,    // ?????��
    BigToSmall,    // ?��????
    SBBS,           // ?��??????
    TurnLeft,      // ������???
    TurnRight,		// ??��???
    TurnLR,
    Anim_FlyPlane
}

// ????
public enum enCOLOR
{
    NONE = 0,
    R,
    G,
    B,
    RG,
    RB,
    GB,
    RGB,
}

public enum enPointSta
{
    None = 0,      // ??(????)
    Target,        // ??����??(????)
    Die,           // ??????(?��??)
    Rest,          // ??????(????)
    MoveRest,      // ??��???????
    MoveDie,       // ??��???????
    Dieing,		// ??????(????��??��??????)
}

[Serializable]
public class AnimOne
{
    public enPicType picType;
    public enAnimMode animMode;
    public enCOLOR color;
    public enPointSta pointSta;
    public int loop;
    public int x;
    public int y;
    public int speed;
    public int width;
    public int height;
    public int delayTime;
    public int stepTime;


    // 
    public static readonly int[] tab_picType = { 0, 1, 2, 3 };
    public static readonly int[] tab_animMode = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    public static readonly int[] tab_color = { 0, 1, 2, 3, 4, 5, 6, 7 };
    public static readonly int[] tab_pointSta = { 0, 1, 2, 3, 4 };
    public static readonly int[] tab_loop = { 0, 1 };
    public static readonly int[] tab_speed = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public static readonly int[] tab_delayTime = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public static readonly int[] tab_delay = { 
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 
        11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 
        21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 
        31, 32, 33, 34, 35, 36, 37, 38, 39, 40 
    };
    public static readonly int[] tab_Pos = { 
        -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
        11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
        21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
        31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
        41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
    };
    public static readonly int[] SET_c_Size = {
        1,2,3,4,5,6,7,8,9,10,
        11,12,13,14,15,16,17,18,19,20,
        21,22,23,24,25,26,27,28,29,30,
        31,32,33,34,35,36,37,38,39,40,
        41,42,43,44,45,46,47,48,49,50,
        51,52,53,54,55,56,57,58,59,60,
        61,62,63,64,65,66,67,68,69,70,
        71,72,73,74,75,76,77,78,79,80,
        81,82,83,84,85,86,87,88,89,90,
        91,92,93,94,95,96,97,98,99,100
    };


    public void Default () {
        picType = enPicType.Rol;
        animMode = enAnimMode.UDDU;
        color = enCOLOR.R;
        pointSta = enPointSta.Die;
        loop = 1;
        x = 0;
        y = 0;
        width = 1;
        height = 1;
        speed = 7;
        delayTime = 0;
    }

    public void CopyTo (AnimOne targetSetting) {
        targetSetting.picType = picType;
        targetSetting.animMode = animMode;
        targetSetting.color = color;
        targetSetting.pointSta = pointSta;
        targetSetting.loop = loop;
        targetSetting.x = x;
        targetSetting.y = y;
        targetSetting.width = width;
        targetSetting.height = height;
        targetSetting.speed = speed;
        targetSetting.delayTime = delayTime;
    }
}
