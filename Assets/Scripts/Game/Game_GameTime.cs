using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_GameTime : MonoBehaviour
{
    public Text text_Time;

    // Use this for initialization
    int gameTime;

    void OnEnable()
    {
        Update_Time();
    }
    // Update is called once per frame
    void Update()
    {
        if (gameTime != Main.PlayTime)
        {
            Update_Time();
        }
    }
    void Update_Time()
    {
        gameTime = (int)Main.PlayTime;      
        //text_Time.text = (gameTime / 60).ToString("D2") + ":" + (gameTime % 60).ToString("D2");
        text_Time.text = gameTime.ToString("D3");
    }
}
