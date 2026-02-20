using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Game97_PlayerModeSel : MonoBehaviour {
	public Image Image_Title;
	public Text text_Time;

	public GameButton[] button_Select;
	public Button button_Start;
	public Button button_Back;


	Game97_Main game97_Main;

	en_GameSelSta statue;
	public static int selectId;	
	float runTime;

	public void Awake0 (Game97_Main game) {
		game97_Main = game;
        //
        for (int i = 0; i < button_Select.Length; i++) {
			button_Select[i].Init (i, OnClick_Select);
		}
		//
		button_Start.onClick.AddListener (OnClick_Start);
		button_Back.onClick.AddListener (OnClick_Back);
	}

	readonly string[] strLanaguge = { "CN", "EN" };
	int lanauage = -1;
	void CheckLanguage () {
		if (lanauage != Set.setVal.Language) {
			lanauage = Set.setVal.Language;
			//

		}
	}
	// Use this for initialization
	public void GameStart () {
		//修改图标--语言
		CheckLanguage ();
		//
		selectId = 0;
		Update_SelectId ();
		
		ChangeStatue (en_GameSelSta.Selecting);
	}

	// Update is called once per frame
	void Update () {
		switch (statue) {
		case en_GameSelSta.Idle:

			break;


		case en_GameSelSta.Selecting:

			break;


		case en_GameSelSta.EnterGame:
			runTime += Time.deltaTime;
			if (runTime >= 0.2f) {
				game97_Main.ChangeStatue (en_Game97_Sta.DiffcultySelect);
			}
			break;
		}
	}
	void ChangeStatue (en_GameSelSta sta) {
		statue = sta;
		//runTime = 0;
		Key.Clear ();


		switch (statue) {
		case en_GameSelSta.Idle:
			break;

		case en_GameSelSta.Selecting:

			break;

		case en_GameSelSta.EnterGame:

			break;

		}
	}
	void Update_SelectId () {
		for (int i = 0; i < button_Select.Length; i++) {
			if (i == selectId) {
				button_Select[i].SetSelectFlag (true);
			} else {
				button_Select[i].SetSelectFlag (false);
			}
		}
	}
	public void OnClick_Select (int id) {
		//Debug.Log ("OnClick_PlayerModeSelect: " + id);
		if (Main.statue != en_MainStatue.Game_97)
			return;
		if (game97_Main.statue != en_Game97_Sta.PlayerModeSelect)
			return;
		if (statue == en_GameSelSta.Selecting) {
			selectId = id;
			Update_SelectId ();
		}
	}

	public void OnClick_Start () {
		if (Main.statue != en_MainStatue.Game_97)
			return;
		if (game97_Main.statue != en_Game97_Sta.PlayerModeSelect)
			return;
		if (statue == en_GameSelSta.Selecting) {
			ChangeStatue (en_GameSelSta.EnterGame);
		}
	}
	public void OnClick_Back () {
		game97_Main.ChangeStatue (en_Game97_Sta.Idle);
	}
}
