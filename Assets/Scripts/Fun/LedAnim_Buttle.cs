using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedAnim_Buttle : MonoBehaviour
{

    public Vector2[] pos_Attack = { new Vector2(-1, -1), new Vector2(-1, -1) };
    int step = 0;
    float runTime = 0;
    int way = 0;

    // Use this for initialization
    void Start()
    {
        runTime = 0.5f;

    }
    public void Init(Vector2 v2, int _way)
    {
        step = 0;
        way = _way;
        for (int i = 0; i < 2; i++)
        {
            switch (way)//上下左右
            {
                case 0:

                    pos_Attack[i] = v2 + new Vector2(0, i);

                    break;
                case 1:
                    pos_Attack[i] = v2 + new Vector2(0, -i);
                    break;
                case 2:
                    pos_Attack[i] = v2 + new Vector2(-i, 0);
                    break;
                case 3:
                    pos_Attack[i] = v2 + new Vector2(i, 0);
                    break;
            }

            if (pos_Attack[i].x <= 0 || pos_Attack[i].x >= Set.setVal.Width - 1 || pos_Attack[i].y <= 0 || pos_Attack[i].y >= Set.setVal.Height - 1)
            {
                Destroy(gameObject);
            }
        }

    }
    public void Move()
    {

        for (int i = 0; i < 2; i++)
        {

            switch (way)//上下左右
            {
                case 0:

                    pos_Attack[0] += new Vector2(0, i);
                    pos_Attack[1] += new Vector2(0, i);

                    break;
                case 1:
                    pos_Attack[0] += new Vector2(0, -i);
                    pos_Attack[1] += new Vector2(0, -i);
                    break;
                case 2:
                    pos_Attack[0] += new Vector2(-i, 0);
                    pos_Attack[1] += new Vector2(-i, 0);
                    break;
                case 3:
                    pos_Attack[0] += new Vector2(i, 0);
                    pos_Attack[1] += new Vector2(i, 0);
                    break;
            }
            if (pos_Attack[i].x <= 0 || pos_Attack[i].x >= Set.setVal.Width - 1 || pos_Attack[i].y <= 0 || pos_Attack[i].y >= Set.setVal.Height - 1)
            {
                Destroy(gameObject);
            }
        }



    }
    // Update is called once per frame
    void Update()
    {
        //GameLedControl.gamePoint[(int)runTime].statue = enPointSta.Rest;
        if (Game00_Main.instance.gameLevel != 3)
        {
            Destroy(gameObject);
        }
        if (runTime > 0)
        {
            runTime -= Time.deltaTime;



        }
        else
        {
            runTime = 0.1f;

            Move();
        }
        for (int i = 0; i < pos_Attack.Length; i++)
        {
            if (pos_Attack[i].x < 0 || pos_Attack[i].y < 0)
            {
                return;
            }
            //int id = Framebuffer.tab_Mapping[(int)pos_Attack[i].x + (int)pos_Attack[i].y * Set.setVal.Width];
            //GameLedControl.gamePoint[id].statue = enPointSta.Die;

            DrawPic.DrawRol((int)pos_Attack[i].x, (int)pos_Attack[i].y, 1, 0xff0000, enPointSta.Die);

        }
    }
}
