using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GamePoint2
{
    public enPointSta statue;
    public uint color;
	public int bindCnt;
	public float bindTime;
}


public class GameBase {
	public static GamePoint2[] gamePoint = new GamePoint2[Main.MAX_LED];

    public static void Update_ColorFull (uint color, enPointSta sta) {
        for (int i = 0; i < gamePoint.Length; i++) {
            gamePoint[i].statue = sta;
            gamePoint[i].color = color;
        }
        //Framebuffer.Update_ColorFull (color, sta);
        Framebuffer.FullScreen (color, sta);
    }
    public static void Update_PointColor (int id, uint color, enPointSta sta) {
        if (id >= gamePoint.Length)
            return;
        gamePoint[id].color = color;
        gamePoint[id].statue = sta;
        Framebuffer.Update_PointColor (id, color, sta);
    }
    public static void Update_PointColor (int x, int y, uint color, enPointSta sta) {
        int id = x * Set.setVal.Height + y;
        id = Framebuffer.tab_Mapping[id];
        Update_PointColor (id, color, sta);
    }


    public static void Clear () {
        for (int i = 0; i < gamePoint.Length; i++) {
            gamePoint[i].bindCnt = 0;
            gamePoint[i].bindTime = 0;
        }
    }
    public static void Check () {
        for (int i = 0; i < gamePoint.Length; i++) {
            // 闪灯
            if (gamePoint[i].bindCnt > 0) {
                if (gamePoint[i].bindTime > 0) {
                    gamePoint[i].bindTime -= Time.deltaTime;
                } else {
                    gamePoint[i].bindCnt--;
                    gamePoint[i].bindTime = 0.2f;
                }
                if (gamePoint[i].bindCnt == 0) {
                    Framebuffer.Update_PointColor (i, gamePoint[i].color, enPointSta.None);
                } else if ((gamePoint[i].bindCnt % 2) == 0) {
                    Framebuffer.Update_PointColor (i, 0, enPointSta.None);
                } else {
                    Framebuffer.Update_PointColor (i, 0x1001, enPointSta.None);
                }
            }
        }
    }
}
