using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserPPD.Core
{


public class PlayerControl {
    LedAnim[] ledAnim = new LedAnim[AppConst.MAX_ANIM];
	AnimSet animSetting;
	//
	public int startx;
	public int starty;
	public int width;
	public int height;
	//
	enCOLOR pointColor;
	int playerId;
	int createPointDcTime;

	public PlayerControl () {
        for (int i = 0; i < ledAnim.Length; i++) {
            if (ledAnim[i] == null) {
                ledAnim[i] = new LedAnim ();
            }
        }
    }

	public void Init (int no, en_PlayerMode playerMode, int bkWidth) {
		playerId = no;
		//
		int mw; // 边框宽度
		int mLen;
		if (no >= AppConst.MAX_PLAYER)
			return;
		if (Set.setVal.Width >= Set.setVal.Height) {
			mLen = Set.setVal.Width;
		} else {
			mLen = Set.setVal.Height;
		}
		if ((mLen % 2) == 0) {
			mw = 2;
		} else {
			mw = 1;
		}

		//
		if (playerMode == en_PlayerMode.Free) {
			// 单人模式(全占)
			startx = bkWidth;
			starty = bkWidth;
			width = Set.setVal.Width - bkWidth * 2;
			height = Set.setVal.Height - bkWidth * 2;
		} else if (no == 0) {
			// 对战模式-玩家1
			startx = bkWidth;
			starty = bkWidth;
			//
			if (Set.setVal.Width >= Set.setVal.Height) {
				width = Set.setVal.Width / 2 - bkWidth - mw / 2;
				height = Set.setVal.Height - bkWidth * 2;
			} else {
				width = Set.setVal.Width - bkWidth * 2;
				height = Set.setVal.Height / 2 - bkWidth - mw / 2;
			}
		} else {
			// 对战模式-玩家2
			if (Set.setVal.Width >= Set.setVal.Height) {
				startx = Set.setVal.Width / 2 + 1;
				starty = bkWidth;
				width = Set.setVal.Width / 2 - bkWidth - mw / 2;
				height = Set.setVal.Height - bkWidth * 2;
			} else {
				startx = bkWidth;
				starty = Set.setVal.Height / 2 + 1;
				width = Set.setVal.Width - bkWidth * 2;
				height = Set.setVal.Height / 2 - bkWidth - mw / 2;
			}
		}
		//
		if (no == 0) {
			pointColor = enCOLOR.B;
		} else {
			pointColor = enCOLOR.GB;
		}
	}

    public void RunStart (AnimSet setting) {
		createPointDcTime = 0;
		StartAll (setting);
	}

    public void StopAll () {
        for (int i = 0; i < ledAnim.Length; i++) {
            if (ledAnim[i] == null) {
                ledAnim[i].Stop ();
            }
        }
    }

    public void StartAll (AnimSet animSet) {
		animSetting = animSet;
		if (animSet == null)
            return;
        for (int i = 0; i < ledAnim.Length; i++) {
            if (i < animSet.animCount) {
				//ledAnim[i].animSet = animSet.animOne[i];
				//
				ledAnim[i].limitLeft = startx;
				ledAnim[i].limitRight = startx + width - 1;
				ledAnim[i].limitUp = starty + height - 1;
				ledAnim[i].limitDown = starty;
				//
				ledAnim[i].picType = animSet.animOne[i].picType;
				ledAnim[i].animMode = animSet.animOne[i].animMode;
				ledAnim[i].loop = animSet.animOne[i].loop;
				ledAnim[i].color = animSet.animOne[i].color;
				ledAnim[i].pointSta = animSet.animOne[i].pointSta;
				ledAnim[i].width = animSet.animOne[i].width;
				ledAnim[i].height = animSet.animOne[i].height;
				ledAnim[i].speed = animSet.animOne[i].speed;
				ledAnim[i].delayTime = animSet.animOne[i].delayTime;

				//animinfo: ------------
				ledAnim[i].x = startx + animSet.animOne[i].x;
				ledAnim[i].y = starty + animSet.animOne[i].y;				

				// 图形校正
				//ledAnim[i].width = width;		// ¿í(/³¤¶È/°ë¾¶)
				//ledAnim[i].height = 1;		// ¸ß
				switch (ledAnim[i].picType) {
				case enPicType.Col:       // ÁÐ(ÊúÏß)
					ledAnim[i].width = 1;     // ¿í(/³¤¶È/°ë¾¶)
					ledAnim[i].height = height;      // ¸ß
					break;
				case  enPicType.Rol:       // ÅÅ(ºáÏß)
					ledAnim[i].width = width;        // ¿í(/³¤¶È/°ë¾¶)
					ledAnim[i].height = 1;        // ¸ß
					break;
				case enPicType.Rectangle: // ¾ØÐÎ
					//ledAnim[i].width = width /2;		// ¿í(/³¤¶È/°ë¾¶)
					//ledAnim[i].height = height / 2;		// ¸ß
					break;
				case enPicType.Prismatic: // ÀâÐÎ
					ledAnim[i].width = width / 2;        // ¿í(/³¤¶È/°ë¾¶)
					break;
				}
				//
				ledAnim[i].startRunPos = starty;
				ledAnim[i].endRunPos = starty + height;
				switch (ledAnim[i].animMode) {
				//
				case enAnimMode.LeftToRight:
				case enAnimMode.LRRL:
					ledAnim[i].startRunPos = startx;
					ledAnim[i].endRunPos = startx + width - 1;
					break;
				case enAnimMode.RightToLeft:
					ledAnim[i].startRunPos = startx + width - 1;
					ledAnim[i].endRunPos = startx;
					break;
				//
				case enAnimMode.UpToDown:
				case enAnimMode.UDDU:
					ledAnim[i].startRunPos = starty;
					ledAnim[i].endRunPos = starty + height - 1;
					break;
				case enAnimMode.DownToUp:
					ledAnim[i].startRunPos = starty + height - 1;
					ledAnim[i].endRunPos = starty;
					break;
				//
				case enAnimMode.TurnLeft:     // ×ó×ªÈ¦
				case enAnimMode.TurnRight:        // ÓÒ×ªÈ¦
				case enAnimMode.TurnLR:       // ×óÓÒÀ´»Ø×ª
					break;
                    case enAnimMode.SmallToBig:
                    case enAnimMode.BigToSmall:
                        break;
				}

				ledAnim[i].RunStart ();
            } else {
                ledAnim[i].Stop ();
            }
        }
    }

	public void RunCheck () {
		int remainTarget = GetRemainTargetPoint ();
		if (remainTarget == 0) {
			if (createPointDcTime > 0) {
				createPointDcTime--;
			} else {
				createPointDcTime = 30;
				GetRandomTargetPoint (playerId);
			}
		} else {
			createPointDcTime = 30;
		}
 

        if (animSetting == null)
        {

            return;
        }
			
        for (int i = 0; i < ledAnim.Length; i++) {
			if (i >= animSetting.animCount)
				break;
			ledAnim[i].Run ();
		}
	}

	public void ShowColorFull (uint color, enPointSta sta) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
				Framebuffer.Update_PointColor (startx + x, starty + y, color, sta);
            }
        }
	}

    public void ShowReadyTime(int value)
    {
        int dir;
        int fontSize;
        int fontWidth;
        int fontHeight;
        int x;
        int y;

        if (width < 8 || height < 8)
        {
            fontSize = 0;
            fontWidth = 4;
            fontHeight = 5;
        }
        else if (width < 10 || height < 10)
        {
            fontSize = 1;
            fontWidth = 4;
            fontHeight = 7;
        }
        else if (width < 12 || height < 12)
        {
            fontSize = 2;
            fontWidth = 8;
            fontHeight = 8;
        }
        else
        {
            fontSize = 3;
            fontWidth = 10;
            fontHeight = 10;
        }
        if (Set.setVal.Width >= Set.setVal.Height || GameLedControl.playerMode == en_PlayerMode.Free)
        {
            dir = 0;
        }
        else
        {
            dir = 1;
        }

        x = (width - fontWidth) / 2;
        y = (height - fontHeight) / 2;

        ShowColorFull(0, enPointSta.None);
        DrawPic.DrawNumber(value, startx + x, starty + y, fontSize, dir, 0xfc0000);
    }
    // 生成随机目标点
    void GetRandomTargetPoint (int no) {
		int x, y, id;
		int i;
		int maxOnce;
		int testCnt = 0;

		maxOnce = (width * height) / 4;
		//	if(maxOnce > MAX_POINT_ONCE)
		//		maxOnce = MAX_POINT_ONCE;

		for (i = 0; i < FjData.g_Fj[no].RemainPoint && i < maxOnce;) {
			x = Random.Range (0, width) + startx;
			y = Random.Range (0, height) + starty;
			
			id = Framebuffer.MappingId (x, y);
			if (++testCnt >= 200)
				return;
			if (GameLedControl.gamePoint[id].statue != enPointSta.None)
				continue;
			if (Framebuffer.led[id].statue == enPointSta.Rest)
				continue;
			GameLedControl.gamePoint[id].statue = enPointSta.Target;
			testCnt = 0;
			i++;
		}
	}

	// 计算剩余目标点个数
 	public int GetRemainTargetPoint () {
		int i, j, id;
		int num = 0;

		for (i = 0; i < width; i++) {
			for (j = 0; j < height; j++) {
				id = Framebuffer.MappingId (startx + i, starty + j);
				if (GameLedControl.gamePoint[id].statue == enPointSta.Target) {
					num++;
				}
			}
		}
		return num;
	}

	public bool IsPlayerGamePoint (int x, int y) {
		if (x < startx)
			return false;
		if (x >= startx + width)
			return false;
		if (y < starty)
			return false;
		if (y >= starty + height)
			return false;
		return true;
	}

}
}
