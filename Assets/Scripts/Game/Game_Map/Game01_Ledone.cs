using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Game01_Ledone : MonoBehaviour
{
    public static int rxKeyStartId;
    public int x;
    public int y;
    public int donw_Y = 0;
    int dir;
    public List<Vector2Int> pos_group;
    float runtime = 0;
    public float stayTime = 0;
    public float MaxstayTime = 0;
    public enum en_Move_Type
    {
        Stay,
        Shake,
        OneWay,
        Down,
        Up,
        Trun,
    }
    public en_Move_Type move_Type;
    public bool isShaking = true;
    public void Init(int _x, int _y, int _dir, en_Move_Type _type)
    {
        Game_Map01.instance.protectTime = 4     ;
        stayTime = 0;
        MaxstayTime = 0; donw_Y = 0;
        isShaking = true;
        pos_group = new List<Vector2Int>();
        x = _x;
        y = _y;
        runtime = 0;
        dir = _dir;
        pos_group.Add(new Vector2Int(x, y)); ;
        move_Type = _type;
        ;
    }
    public void Init(List<Vector2Int> _pos, int _dir, en_Move_Type _type)
    {
        Game_Map01.instance.protectTime = 4;
        donw_Y = 0;
        move_Type = _type;
        isShaking = true;
        pos_group = new List<Vector2Int>();
        for (int i = 0; i < _pos.Count; i++)
        {
            if (_pos[i].x < 0 || _pos[i].x >= Set.setVal.Width || _pos[i].y < 0 || _pos[i].y >= Set.setVal.Height)
            {
                continue;
            }
            pos_group.Add(_pos[i]);
        }
        dir = _dir;
        if (_type == en_Move_Type.Down)
        {
            y = 0;
        }
    }
    int startId = LedKey.GetLeiSheKeyStartId();
    // Start is called before the first frame update
    Vector2Int pos_Test = new Vector2Int(0, 0);
    void Moving()
    {
        runtime += Time.deltaTime;
        if (stayTime > 0)
        {
            stayTime -= Time.deltaTime;
            return;
        }

        if (runtime > Game_Map01.instance.max_runtime)
        {
            Game_Map01.instance.protectTime = 0.4f;
            runtime = 0;
            switch ((en_Move_Type)move_Type)
            {
                case en_Move_Type.Up:
                    stayTime = 1f;
                    if (dir == 0)
                    {
                        if (y < Set.setVal.Height)
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
                        if (y > donw_Y)
                        {
                            y--;
                        }
                        else
                        {
                            dir = 0;
                        }
                    }

                    break;

                case en_Move_Type.Down:
                    stayTime = 1f;
                    if (dir == 0)
                    {
                        if (y < Set.setVal.Height)
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
                        if (y > donw_Y)
                        {
                            y--;
                        }
                        else
                        {
                            dir = 0;
                        }
                    }

                    break;
                case en_Move_Type.Shake:
                    isShaking = !isShaking;

                    break;
                case en_Move_Type.OneWay:
                    switch (dir)
                    {
                        case 0:
                            for (int i = 0; i < pos_group.Count; i++)
                            {
                                int xx = pos_group[i].x + 1;
                                if (xx >= Set.setVal.Width)
                                {
                                    xx = 0;
                                }
                                pos_group[i] = new Vector2Int(xx, pos_group[i].y); ;
                            }
                            break;
                        case 1:
                            for (int i = 0; i < pos_group.Count; i++)
                            {
                                int xx = pos_group[i].x - 1;
                                if (xx < 0)
                                {
                                    xx = Set.setVal.Width - 1;
                                }
                                pos_group[i] = new Vector2Int(xx, pos_group[i].y); ;
                            }
                            break;
                    }
                    break;
                case en_Move_Type.Trun:

                    break;
                default:
                    break;
            }


        }

        if (isShaking)
        {

            for (int i = 0; i < pos_group.Count; i++)
            {
                if (move_Type == en_Move_Type.Down)
                {
                    if (pos_group[i].y < donw_Y)
                    {
                        continue;
                    }
                }
                if (move_Type == en_Move_Type.Up)
                {
                    if (pos_group[i].y > donw_Y)
                    {
                        continue;
                    }
                }
                Framebuffer.Update_TransmitLedColor(pos_group[i].x, pos_group[i].y, 255, enPointSta.Target);
                //  Debug.LogError(LedKey.GetKeyStatus(Framebuffer.MappingId(pos_group[i].x, pos_group[i].y)));
                if (LedKey.KeyStatus(startId + Framebuffer.MappingId(pos_group[i].x, pos_group[i].y)))
                {
                    continue;
                }
                if (LedKey.KeyStatus(startId + Framebuffer.MappingId(pos_group[i].x, pos_group[i].y)) == false && Game01_Main.instance.statue == en_Game01_Sta.Play)
                {
                    if (Game_Map01.instance.protectTime <= 0)
                    {

                        if (FjData.g_Fj[0].Life > 0)
                        {
#if UNITY_EDITOR
#else
FjData.g_Fj[0].Life--; 
#endif

                            Game_Map01.instance.protectTime = 3f;
                            Debug.LogError("¿ÛÑª×ø±ê£º"+ pos_group[i].x+"  "+ pos_group[i].y);
                            GameLeiSheBase.gamePoint[i].bindCnt = 4;
                            MusicManager.instance.Play_Fails();
                        }
                    }
                }
            }
        }


    }
    // Update is called once per frame
    void Update()
    {

        if (Main.statue >= en_MainStatue.Game_97)
        {
            Destroy(gameObject);
            return;
        }
        if (Game01_Main.instance.player.statue != en_Player01Sta.Play)
        { return; }
        if (Game01_Main.instance.player.currJieDuan != 1 && Game01_Main.instance.player.currJieDuan != 3)
            return;
        Moving();



    }
}
