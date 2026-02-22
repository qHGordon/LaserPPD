using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game01_GameUIComm : MonoBehaviour {
	public GameObject tips_Obj;
	public GameObject showLevel_Obj;	
	public Text text_ShowLevel;
	public GameObject showPlayer_Obj;
	public Text text_ShowPlayer;
	public GameObject level_Obj;
	public Text text_Level;
	public GameObject playerId_Obj;
	public Text text_PlayerId;
	public Image image_ReadyTime;
	public Sprite[] sprite_ReadyTime;


	public Button button_Exit;
	public GameObject exitTpis_Obj;
	public Button button_ExitYes;
	public Button button_ExitNo;
	public Game_PlayerNameInput playerNameInput;
	public GameRankList rankList;

	float buttonExitShowTime;

	void Awake () {
		button_Exit.onClick.AddListener (OnClick_Exit);
		button_ExitYes.onClick.AddListener (OnClick_ExitYes);
		button_ExitNo.onClick.AddListener (OnClick_ExitNo);
		sprite_ReadyTime = Resources.LoadAll<Sprite> ("UI/ReadyTime");
	}

	public void GameStart () {
		exitTpis_Obj.SetActive (false);
		button_Exit.gameObject.SetActive (false);
		playerNameInput.gameObject.SetActive (false);
		rankList.gameObject.SetActive (false);
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			if (button_Exit.gameObject.activeSelf == false) {
				button_Exit.gameObject.SetActive (true);
			}
			buttonExitShowTime = 0;
		}
		if (button_Exit.gameObject.activeSelf) {
			buttonExitShowTime += Time.deltaTime;
			if (buttonExitShowTime >= 5) {
				button_Exit.gameObject.SetActive (false);
			}
		}
	}

	public void Update_Level (int level) {
		text_Level.text = (level + 1).ToString();
	}
	public void Update_ShowLevel (int level) {
		text_ShowLevel.text = (level + 1).ToString();
	}
	public void Update_ShowPlayer (int id) {
		text_ShowPlayer.text = (id + 1).ToString ();
	}
	public void Update_PlayerId (int id) {
		text_PlayerId.text = (id + 1).ToString ();
	}
	public void Update_ReadyTime (int value) {
		image_ReadyTime.gameObject.SetActive (false);
		image_ReadyTime.gameObject.SetActive (true);
		image_ReadyTime.sprite = sprite_ReadyTime[value];
		image_ReadyTime.SetNativeSize ();
	}

	public void OnClick_Exit () {
		exitTpis_Obj.SetActive (true);
	}
	public void OnClick_ExitYes () {
		exitTpis_Obj.SetActive (false);
        Main.instance.game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
    }
	public void OnClick_ExitNo () {
		exitTpis_Obj.SetActive (false);
	}
}
