using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_Area : MonoBehaviour
{

    public uint color = 0;
    public int start_Index = 0;
    int NowY = 1;
    int NowX = 1;
    // Use this for initialization
    void Start()
    {

    }
    public void Init(uint _color)
    {
        color = _color;
    }
    public void NextType()
    {
        start_Index++;
        int wid = 2 * Set.setVal.Width;
        if (wid < 50)
        {
            wid = 50;
        }
        if (start_Index > wid)
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
#if !LiuGuang
          Destroy(gameObject);

            return;
#endif
        if (Game00_Main.instance.statue == en_Game00_Sta.Play)
        {
            Destroy(gameObject);

            return;
        }
        if (!Game97_Idle.instance.isShow_NewIdleType || Game97_Idle.instance.gameObject.activeSelf == false || Main.statue != en_MainStatue.Game_97)
        {


            Destroy(gameObject);

            return;
        }
        if (color == 0)
        {
            return;
        }


        NowY = 1;
        NowX = 4 + start_Index;
        for (int k = 0; ; k++)
        {
            for (int i = NowX; i > NowX - 5; i--)
            {
                
                    Framebuffer.Update_PointColor(i, k, color, enPointSta.Rest);
                   // Debug.LogError(i + "   " + k);
                 
              
            }
            NowX--;
            NowY++;
            if (NowY > Set.setVal.Height + Set.setVal.WallNum_Height)//NowX < 0 ||
            {

                break;
            }
        }

    }
}
