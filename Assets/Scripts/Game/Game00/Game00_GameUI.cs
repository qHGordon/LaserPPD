using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game00_GameUI : MonoBehaviour
{
    public Game00_PlayerUI[] playerUI;

    public GameObject time_Obj_All;
    public GameObject time_Obj_JieDuan;
    public Text text_ReaminTime;    
    public Text text_ReaminTime_JieDuan;
    //public Game_Out gameOut;
    //public Game00_CoinIn coinIn;

    // Use this for initialization
    public void Awake0 () {
        for (int i = 0; i < playerUI.Length; i++) {
            playerUI[i].Awake0 (i);
        }



    }

    public void GameStart()
    {
        //coinIn.Init (0);
    }

    
    void Update () {
    }


    public void Update_RemainTime (int value) {
        if (Set.setVal.TimeMode==1)
        {
            if (time_Obj_All.gameObject.activeSelf)
            {
                time_Obj_All.gameObject.SetActive(false);
              
            }
            return;
        }
        else
        {
            if (!time_Obj_All.gameObject.activeSelf)
            {
                time_Obj_All.gameObject.SetActive(true);

            }

        }
        text_ReaminTime.text = (value / 60).ToString ("D2") + ":" + (value % 60).ToString ("D2");

    }
    public void Update_RemainTime_JieDuan(int value) {
        if (Set.setVal.TimeMode == 0)
        {
            text_ReaminTime_JieDuan. transform.parent. localPosition  =new Vector2(438,-358);
        }
        else
        {
            text_ReaminTime_JieDuan.transform.parent. localPosition = new Vector2(0, -358);

        }
        text_ReaminTime_JieDuan.text = (value / 60).ToString ("D2") + ":" + (value % 60).ToString ("D2");
    }
}
