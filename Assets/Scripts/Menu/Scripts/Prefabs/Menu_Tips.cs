using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public delegate void SelectCall(bool result);

public enum en_MenuTipsType
{
    Tips = 0,
    Select,
}

public class Menu_Tips : MonoBehaviour
{
    const int MAX_SEL = 2;

    public Text text_Tips;
    public Text text_Time;
    public Menu_Button[] button;
    public Menu_Button button_Ok;
    //
    SelectCall selectCall;

    public en_MenuTipsType tipsType;
    int remainTime;
    float runTime;
    int postIndex;

    public static Menu_Tips instance;
    void Awake()
    {
        instance = this;
        for (int i = 0; i < button.Length; i++)
        {
            button[i].Init(i, OnClick_Button);
        }
        button_Ok.Init(0, OnClick_Ok);
    }
    void OnDisable()
    {
        selectCall = null;
    }
    public void UpdataLanguage()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            //中文                        
            button[0].GetComponentInChildren<Text>().text = "是";
            button[1].GetComponentInChildren<Text>().text = "否";

            button_Ok.SetName("确定");
        }
        else
        {
            //英文            
            button[0].GetComponentInChildren<Text>().text = "Yes";
            button[1].GetComponentInChildren<Text>().text = "No";
            //
            button_Ok.SetName("OK");
        }
    }

    void GameStart(string tips)
    {
        gameObject.SetActive(true);
        UpdataLanguage();
        postIndex = 0;

        text_Tips.text = tips;
    }

    // Use this for initialization
    // Tips:
    public void Init(string tips, int time, bool canClose)
    {
        tipsType = en_MenuTipsType.Tips;
        selectCall = null;
        runTime = time + 0.9f;
        remainTime = time;
        for (int i = 0; i < button.Length; i++)
        {
            button[i].gameObject.SetActive(false);
        }
        if (time > 0)
        {
            text_Time.gameObject.SetActive(true);
            Update_Time(remainTime);
        }
        else
        {
            text_Time.gameObject.SetActive(false);
        }
        button_Ok.gameObject.SetActive(canClose);
        GameStart(tips);
    }
    // Select:
    public void Init(string tips, SelectCall call)
    {
        tipsType = en_MenuTipsType.Select;
        selectCall = call;

        text_Time.gameObject.SetActive(false);
        for (int i = 0; i < button.Length; i++)
        {
            button[i].gameObject.SetActive(true);
        }
        Update_SelectPos();

        button_Ok.gameObject.SetActive(false);
        GameStart(tips);
    }

    void Exit()
    {
        gameObject.SetActive(false);
        Key.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (tipsType == en_MenuTipsType.Select)
        {
            if (Key.MENU_LeftPressed() || Key.KEYFJ_Menu_LeftPressed())
            {
                postIndex = (postIndex + MAX_SEL - 1) % MAX_SEL;
                Update_SelectPos();
            }
            if (Key.MENU_RightPressed() || Key.KEYFJ_Menu_RightPressed())
            {
                postIndex = (postIndex + MAX_SEL - 1) % MAX_SEL;
                Update_SelectPos();
            }
            if (Key.MENU_OkPressed() || Key.KEYFJ_Menu_OkPressed())
            {
                OnClick_Button(postIndex);
                return;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                runTime = 0;
            }
            if (runTime > 0)
            {
                runTime -= Time.deltaTime;
                if (remainTime != (int)runTime)
                {
                    remainTime = (int)runTime;
                    Update_Time(remainTime);
                }
            }
            else
            {
                Exit();
            }
        }
    }

    void Update_SelectPos()
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (i == postIndex)
            {
                button[i].SetSelect(true);
            }
            else
            {
                button[i].SetSelect(false);
            }
        }
    }

    void Update_Time(int value)
    {
        text_Time.text = "(" + value + ")";
    }

    public void OnClick_Button(int id)
    {
        SelectCall tempCall = selectCall;   // Exit时会清空； 下次弹窗不消失，先Exit()
        Exit();

        bool result = false;
        if (id == 0)
        {
            result = true;
        }
        if (tempCall != null)
        {
            tempCall(result);
        }
    }

    public void OnClick_Ok(int id)
    {
        Exit();
    }
}
