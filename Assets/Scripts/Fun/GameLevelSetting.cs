using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum en_TargetNewType
{
    Random = 0,
    Pic,
}
public enum en_TargetDieType
{
    ToNone = 0,     // 变空点(黑点)
    ToDie,          // 变死亡点(红点)
}

[Serializable]
public class GameLevelSetting
{
    public int gameTime;        //游戏时间
    public int life;            //生命数[1],
    public int targetPoint;     //目标数[1],
    public int wallLedNum;      //墙灯个数
    //public int animCount;       //花样个数[1]
    public int bkWidth;         //边框宽度
    public int targetNewType;   //目标点生成方式      
    public int targetDieType;   //目标点死亡方式

    public string presetPicName;
    public string animInfoName;

    [NonSerialized]
    public PresetPic picSetting;
    //public byte[] presetPic = new byte[Main.MAX_LED];
    [NonSerialized]
    public AnimSet animSetting;
    //public AnimOne[] animInfo = new AnimOne[Main.MAX_ANIM];

    public static readonly int[] tab_gameTime = { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 180 };
    public static readonly int[] tab_life = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public static readonly int[] tab_wallLedNum = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public static readonly int[] tab_targetPoint = { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 110, 120 };
    public static readonly int[] tab_bkWidth = { 0, 1, 2 };
    public static readonly int[] tab_targetNewType = { 0, 1 };
    public static readonly int[] tab_targetDieType = { 0, 1 };

    public GameLevelSetting()
    {

    }

    public void CopyTo(GameLevelSetting targetSetting)
    {
        targetSetting.gameTime = gameTime;
        targetSetting.life = life;
        targetSetting.targetPoint = targetPoint;
        //targetSetting.animCount = animCount;
        targetSetting.bkWidth = bkWidth;
        targetSetting.targetNewType = targetNewType;
        targetSetting.targetDieType = targetDieType;

        //for (int i = 0; i < presetPic.Length; i++) {
        //    targetSetting.presetPic[i] = presetPic[i];
        //}
        //for (int i = 0; i < animInfo.Length; i++) {
        //    animInfo[i].CopyTo (targetSetting.animInfo[i]);
        //}
    }

    public void Default(int gameId, int playerMode, int level)
    {
        switch ((en_GameId)gameId)
        {
            case en_GameId.YueDongGeZi:
                gameTime = 120;        //游戏时间
                life = 10;            //生命数
                targetPoint = 40 + level * 5;     //目标数
                wallLedNum = 15 + level * 2;
                //animCount = 1 + level / 5;       //花样个数
                bkWidth = 1;
                targetNewType = (int)en_TargetNewType.Random;
                targetNewType = (int)en_TargetNewType.Pic;
                targetDieType = (int)en_TargetDieType.ToNone;

                //for (int i = 0; i < presetPic.Length; i++) {
                //    presetPic[i] = 0x0;
                //}
                //if (level < 2) {
                //    presetPicName = "";
                //} else {
                //    presetPicName = "D-" + level.ToString ("D3");
                //}
                //if (level < 10) {
                //    animInfoName = "D-" + (level + 1).ToString ("D3");
                //} else {
                //    animInfoName = "D-010";
                //}
                //if (level >= 7) {
                //    targetDieType = (int)en_TargetDieType.ToDie;
                //}
                animInfoName = "D-" + (level + 1).ToString("D3");
                presetPicName = "D-" + (level + 1).ToString("D3");
                break;

            case en_GameId.LeiSheWu:
                playerMode = Mathf.Clamp(playerMode, 0, 2);
                level = Mathf.Clamp(level, 0, 9);

                gameTime = 120;        //游戏时间
                life = 4;            //生命数
                targetPoint = 2 + level / 2;     //目标数

                //if (level < 9) {
                //    presetPicName = "D-" + (level + 1).ToString("D3");
                //} else {
                //    presetPicName = "D-009";
                //}
                //if (level < 3) {
                //    animInfoName = "";
                //} else if (level < 9) {
                //    animInfoName = "D-" + (level - 2).ToString("D3");
                //} else {
                //    animInfoName = "D-006";
                //}

                // presetPic
                presetPicName = "D-" + (playerMode * 10 + level + 1).ToString("D3");
                // animInfo
                if (playerMode == (int)en_PlayerMode.Challenge)
                {
                    animInfoName = "D-" + (level + 1).ToString("D3");
                }
                else
                {
                    animInfoName = "";
                }
                break;

            case en_GameId.PanYan:
                gameTime = 120;        //游戏时间
                life = 10;            //生命数
                targetPoint = 40 + level * 5;     //目标数
                wallLedNum = 15 + level * 2;
                //animCount = 1 + level / 5;       //花样个数
                bkWidth = 1;
                targetNewType = (int)en_TargetNewType.Random;
                targetNewType = (int)en_TargetNewType.Pic;
                targetDieType = (int)en_TargetDieType.ToNone;

                //for (int i = 0; i < presetPic.Length; i++) {
                //    presetPic[i] = 0x0;
                //}
                //if (level < 2) {
                //    presetPicName = "";
                //} else {
                //    presetPicName = "D-" + level.ToString ("D3");
                //}
                //if (level < 10) {
                //    animInfoName = "D-" + (level + 1).ToString ("D3");
                //} else {
                //    animInfoName = "D-010";
                //}
                //if (level >= 7) {
                //    targetDieType = (int)en_TargetDieType.ToDie;
                //}
                animInfoName = "D-" + (level + 1).ToString("D3");
                presetPicName = "D-" + (level + 1).ToString("D3");
                break;
        }



    }
}
