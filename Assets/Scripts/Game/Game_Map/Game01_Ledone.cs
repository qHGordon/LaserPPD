using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>[Game01] 镭射激光单点对象，负责运动、渲染与踩点碰撞检测。</summary>
public class Game01_Ledone : MonoBehaviour
{
    public static int rxKeyStartId;
    public int x;  // 列坐标 (column)
    public int y;  // 行坐标 (row)，y=0 为底部，y=Height-1 为顶部
    public int donw_Y = 0;
    int dir;
    public List<Vector2Int> pos_group;  // 每个元素 (x,y)：列 x, 行 y
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
        Game_Map01.instance.protectTime = 4; // 初始化时给玩家短暂无敌保护
        stayTime = 0;
        MaxstayTime = 0; donw_Y = 0;
        isShaking = true;
        pos_group = new List<Vector2Int>();
        x = _x;
        y = _y;
        runtime = 0;
        dir = _dir;
        pos_group.Add(new Vector2Int(x, y)); // 记录初始坐标点
        move_Type = _type;
    }
    public void Init(List<Vector2Int> _pos, int _dir, en_Move_Type _type)
    {
        Game_Map01.instance.protectTime = 4; // 初始化时给玩家短暂无敌保护
        donw_Y = 0;
        move_Type = _type;
        isShaking = true;
        pos_group = new List<Vector2Int>();
        for (int i = 0; i < _pos.Count; i++)
        {
            if (_pos[i].x < 0 || _pos[i].x >= Set.setVal.Width || _pos[i].y < 0 || _pos[i].y >= Set.setVal.Height)
            {
                continue; // 过滤掉越界的点
            }
            pos_group.Add(_pos[i]); // 收集合法坐标点
        }
        dir = _dir;
        if (_type == en_Move_Type.Down)
        {
            y = 0; // 向下模式从顶部开始
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
            stayTime -= Time.deltaTime; // 停留计时中，不进行移动
            return; // 保持静止直到停留结束
        }

        if (runtime > Game_Map01.instance.max_runtime)
        {
            Game_Map01.instance.protectTime = 0.4f; // 每次移动节拍给短暂保护
            runtime = 0; // 重置移动节拍计时
            switch ((en_Move_Type)move_Type)
            {
                case en_Move_Type.Up:
                    stayTime = 1f; // 到达拐点后短暂停留
                    if (dir == 0)
                    {
                        if (y < Set.setVal.Height)
                        {
                            y++; // 向上推进
                        }
                        else
                        {
                            dir = 1; // 触顶后反向
                        }
                    }
                    else
                    {
                        if (y > donw_Y)
                        {
                            y--; // 向下回退
                        }
                        else
                        {
                            dir = 0; // 触底后反向
                        }
                    }

                    break;

                case en_Move_Type.Down:
                    stayTime = 1f; // 到达拐点后短暂停留
                    if (dir == 0)
                    {
                        if (y < Set.setVal.Height)
                        {
                            y++; // 向下推进（坐标向上增加）
                        }
                        else
                        {
                            dir = 1; // 触顶后反向
                        }
                    }
                    else
                    {
                        if (y > donw_Y)
                        {
                            y--; // 向回退
                        }
                        else
                        {
                            dir = 0; // 触底后反向
                        }
                    }

                    break;
                case en_Move_Type.Shake:
                    isShaking = !isShaking; // 交替显示/隐藏实现闪烁效果

                    break;
                case en_Move_Type.OneWay:
                    switch (dir)
                    {
                        case 0:
                            for (int i = 0; i < pos_group.Count; i++)
                            {
                                int xx = pos_group[i].x + 1; // 向右平移
                                if (xx >= Set.setVal.Width)
                                {
                                    xx = 0; // 右边界回绕
                                }
                                pos_group[i] = new Vector2Int(xx, pos_group[i].y); // 更新坐标
                            }
                            break;
                        case 1:
                            for (int i = 0; i < pos_group.Count; i++)
                            {
                                int xx = pos_group[i].x - 1; // 向左平移
                                if (xx < 0)
                                {
                                    xx = Set.setVal.Width - 1; // 左边界回绕
                                }
                                pos_group[i] = new Vector2Int(xx, pos_group[i].y); // 更新坐标
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
                        continue; // 向下模式时忽略低于下限的点
                    }
                }
                if (move_Type == en_Move_Type.Up)
                {
                    if (pos_group[i].y > donw_Y)
                    {
                        continue; // 向上模式时忽略高于下限的点
                    }
                }
                Framebuffer.Update_TransmitLedColor(pos_group[i].x, pos_group[i].y, 255, enPointSta.Target); // 点亮目标点
                //  Debug.LogError(LedKey.GetKeyStatus(Framebuffer.MappingId(pos_group[i].x, pos_group[i].y)));
                if (LedKey.KeyStatus(startId + Framebuffer.MappingId(pos_group[i].x, pos_group[i].y)))
                {
                    continue; // 按键按下时不触发扣血
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

                            Game_Map01.instance.protectTime = 3f; // 扣血后进入保护时间
                            Debug.LogError("¿ÛÑª×ø±ê£º"+ pos_group[i].x+"  "+ pos_group[i].y); // 记录扣血坐标
                            GameLeiSheBase.gamePoint[i].bindCnt = 4; // 提示绑定点计数
                            MusicManager.instance.Play_Fails(); // 播放失败音效
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
            Destroy(gameObject); // 退出关卡时销毁自身
            return;
        }
        if (Game01_Main.instance.player.statue != en_Player01Sta.Play)
        { return; } // 非游戏中状态不更新
        if (Game01_Main.instance.player.currJieDuan != 1 && Game01_Main.instance.player.currJieDuan != 3)
            return; // 只在指定阶段执行移动
        Moving(); // 执行移动与碰撞检测



    }
}
