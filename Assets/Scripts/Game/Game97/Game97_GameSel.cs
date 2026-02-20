using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

enum en_GameSelSta {
    Idle = 0,
    Selecting,
    EnterGame,
    End,
}
public class Game97_GameSel : MonoBehaviour {
    public Image Image_Title;
    public Text text_Time;

    public GameObject playerModeSelectObj;
    public GameObject playerModeSelectKuange;
    public Button[] button_PlayerMode;
    public GameObject diffcultySelectObj;
    public GameObject diffcultySelectKuange;
    public Button[] button_Diffculty;
    public Button button_Start;

    //
    Game97_Main game97_Main;

    en_GameSelSta statue;
    int playerMode;
    int diffcultSel;
    float runTime;

    public void Awake0(Game97_Main game) {
        game97_Main = game;

        //for (int i = 0; i < button_PlayerMode.Length; i++) {
        //    button_PlayerMode[i].onClick.AddListener (() => OnClick_PlayerModeSelect (i));
        //}
        button_PlayerMode[0].onClick.AddListener(() => OnClick_PlayerModeSelect(0));
        button_PlayerMode[1].onClick.AddListener(() => OnClick_PlayerModeSelect(1));
        //for (int i = 0; i < button_Diffculty.Length; i++) {
        //    button_Diffculty[i].onClick.AddListener (() => OnClick_DiffcultySelect (i));
        //}
        button_Diffculty[0].onClick.AddListener(() => OnClick_DiffcultySelect(0));
        button_Diffculty[1].onClick.AddListener(() => OnClick_DiffcultySelect(1));
        button_Diffculty[2].onClick.AddListener(() => OnClick_DiffcultySelect(2));
        button_Start.onClick.AddListener(OnClick_Start);
    }

    //
    readonly string[] strLanaguge = { "CN", "EN" };
    int lanauage = -1;
    void CheckLanguage() {
        if (lanauage != Set.setVal.Language) {
            lanauage = Set.setVal.Language;
            //

        }
    }

    public void GameStart() {
        //修改图标--语言
        CheckLanguage();
        //
        playerMode = 0;
        Update_PlayerMode();
        diffcultSel = 1;
        Update_DiffcultySelect();

        ChangeStatue(en_GameSelSta.Selecting);
    }

    void Update() {
        switch (statue) {
        case en_GameSelSta.Idle:

            break;


        case en_GameSelSta.Selecting:

            break;


        case en_GameSelSta.EnterGame:
            runTime += Time.deltaTime;
            if (runTime >= 0.5f) {

                game97_Main.EnterGame(0);
            }
            break;
        }
    }

    void ChangeStatue(en_GameSelSta sta) {
        statue = sta;
        //runTime = 0;
        Key.Clear();


        switch (statue) {
        case en_GameSelSta.Idle:
            break;

        case en_GameSelSta.Selecting:

            break;

        case en_GameSelSta.EnterGame:

            break;

        }
    }


    void Update_PlayerMode() {
        if (playerMode >= 0 && playerMode < button_PlayerMode.Length) {
            playerModeSelectKuange.SetActive(true);
            playerModeSelectKuange.transform.position = button_PlayerMode[playerMode].transform.position;
        } else {
            playerModeSelectKuange.SetActive(false);
        }

    }
    void Update_DiffcultySelect() {
        if (diffcultSel >= 0 && diffcultSel < button_Diffculty.Length) {
            diffcultySelectKuange.SetActive(true);
            diffcultySelectKuange.transform.position = button_Diffculty[diffcultSel].transform.position;
        } else {
            diffcultySelectKuange.SetActive(false);
        }
    }

    public void OnClick_PlayerModeSelect(int id) {
        //Debug.Log ("OnClick_PlayerModeSelect: " + id);
        if (Main.statue != en_MainStatue.Game_97)
            return;
        if (game97_Main.statue != en_Game97_Sta.GameSelect)
            return;
        if (statue == en_GameSelSta.Selecting) {
            playerMode = id;
            Update_PlayerMode();
        }
    }
    public void OnClick_DiffcultySelect(int id) {
        //Debug.Log ("OnClick_DiffcultySelect: " + id);
        if (Main.statue != en_MainStatue.Game_97)
            return;
        if (game97_Main.statue != en_Game97_Sta.GameSelect)
            return;
        if (statue == en_GameSelSta.Selecting) {
            diffcultSel = id;
            Update_DiffcultySelect();
        }
    }
    public void OnClick_Start() {
        if (Main.statue != en_MainStatue.Game_97)
            return;
        if (game97_Main.statue != en_Game97_Sta.GameSelect)
            return;
        if (statue == en_GameSelSta.Selecting) {
            ChangeStatue(en_GameSelSta.EnterGame);
        }
    }

}
