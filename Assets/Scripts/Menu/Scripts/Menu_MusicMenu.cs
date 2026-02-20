using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Menu_MusicMenu : MonoBehaviour {
    // 要显示的内容(中英文切换)    
    public Text text_SysSet;
    public Menu_Button[] button;
    static int postIndex;
    bool pressedButton;

    const int BUTTON_ID_MUSICIDLE = 0;
    const int BUTTON_ID_MUSICGAME = 1;
    const int BUTTON_ID_BACK = 2;
    const int MAX_BUTTON = 3;

    // 传入的变量
    Menu menu;        
    //
    public void Awake0 (Menu mmenu) {
        //
        menu = mmenu;                
        //
        for (int i = 0; i < button.Length; i++) {
            button[i].Init (i, OnClick_Button);
            button[i].gameObject.SetActive (true);
        }
    }

    // Update is called once per frame
    void Update () {
        if (Menu.statue != en_MenuStatue.MenuSta_MusicMenu)
            return;
      

        if (Key.MENU_LeftPressed () || Key.KEYFJ_Menu_LeftPressed ()) {
            postIndex = (postIndex + MAX_BUTTON - 1) % MAX_BUTTON;
            UpdateCursor ();
        }
        if (Key.MENU_RightPressed () || Key.KEYFJ_Menu_RightPressed ()) {
            postIndex = (postIndex + 1) % MAX_BUTTON;
            UpdateCursor ();
        }
        if (Key.MENU_OkPressed () || Key.KEYFJ_Menu_OkPressed () || pressedButton) {
            pressedButton = false;
            switch (postIndex) {
            case BUTTON_ID_MUSICIDLE:
                OnButton_MusicIdle_Pressed ();
                break;
            case BUTTON_ID_MUSICGAME:
                OnButton_MusicGame_Pressed ();
                break;
            case BUTTON_ID_BACK:
                OnButton_Back_Pressed ();
                break;
            }
        }
    }

    // 更新光标坐标和大小
    void UpdateCursor () {
        for (int i = 0; i < MAX_BUTTON; i++) {
            if (i == postIndex) {
                button[i].SetSelect (true);
            } else {
                button[i].SetSelect (false);
            }
        }
    }

    // 1.进入初始化
    public void GameStart () {
        pressedButton = false;
        UpdateLanguage ();
        postIndex = 0;
        UpdateCursor ();
    }
    // 2.更新显示中英文
    public void UpdateLanguage () {
        if (Set.setVal.Language == (int)en_Language.Chinese) {
            //中文
            text_SysSet.text = "音 乐 管 理";
            button[BUTTON_ID_MUSICIDLE].GetComponentInChildren<Text> ().text = "待机音乐";
            button[BUTTON_ID_MUSICGAME].GetComponentInChildren<Text> ().text = "游戏音乐";
            button[BUTTON_ID_BACK].GetComponentInChildren<Text> ().text = "返 回";
        } else {
            //英文
            text_SysSet.text = "Music Manager";
            button[BUTTON_ID_MUSICIDLE].GetComponentInChildren<Text> ().text = "Idle music";
            button[BUTTON_ID_MUSICGAME].GetComponentInChildren<Text> ().text = "Game music";
            button[BUTTON_ID_BACK].GetComponentInChildren<Text> ().text = "Back";
        }
    }

    //按键 ------
    public void OnClick_Button (int id) {
        postIndex = id;
        UpdateCursor ();
        pressedButton = true;
    }
    public void OnButton_MusicIdle_Pressed () {
        //menu.ChangeStatue (en_MenuStatue.MenuSta_MusicManager);
        menu.menu_MusicManager.gameObject.SetActive (true);
        menu.menu_MusicManager.GameStart (en_MusicType.MusicIdle);
    }
    public void OnButton_MusicGame_Pressed () {
        //menu.ChangeStatue (en_MenuStatue.MenuSta_MusicManager);
        menu.menu_MusicManager.gameObject.SetActive (true);
        menu.menu_MusicManager.GameStart (en_MusicType.MusicGame);
    }
    public void OnButton_Back_Pressed () {
        menu.ChangeStatue (en_MenuStatue.MenuSta_SysSet);
    }
}
