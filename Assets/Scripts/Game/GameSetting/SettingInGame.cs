using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class SettingInGame : MonoBehaviour {
	public Text text_Title;
	public GameObject gameSetOne_Prefab;
	public Transform gameSet_Layer;
	public Button button_Save;
	public Button button_Defult;
	public Button button_Back;
	public Button button_Next;
	public Button button_Show;


	public List<GameSetOne> list_GameSetOne = new List<GameSetOne> ();

#if UNITY_EDITOR
	void Awake () {
		//Init (3);
		//GameStart ();
	}
#endif

	public void Init (int num) {
		button_Save.onClick.AddListener (OnClick_Save);
		button_Defult.onClick.AddListener (OnClick_Default);
		button_Back.onClick.AddListener (OnClick_Back);
		button_Next.onClick.AddListener (OnClick_Next);
		button_Show.onClick.AddListener (OnClick_Show);
		//
	//	SetInit (num);
	}
	public void GameStart () {
        UpdateLanguage ();
        UpdateSetValue ();
    }
	
	// Update is called once per frame
	void Update () {

	}
	//
	public void CreateSetOnes (int count) {
        for (int i = 0; i < list_GameSetOne.Count; i++) {
			if (list_GameSetOne[i] != null) {
				Destroy (list_GameSetOne[i].gameObject);
			}
        }
		list_GameSetOne.Clear ();

		for (int i = 0; i < count; i++) {
			GameSetOne gameSetOne = Instantiate (gameSetOne_Prefab, gameSet_Layer).GetComponent<GameSetOne> ();
			gameSetOne.transform.localPosition = new Vector3 ((i % 2) * 600 - 300, 0 - (i / 2) * 80);
			list_GameSetOne.Add (gameSetOne);
		}
	}
	public virtual void SetInit ( int num) {
	}
	public virtual void UpdateLanguage () {
        if (Set.setVal.Language == 0)
        {
            button_Save.GetComponentInChildren<Text> ().text = "更新";
			button_Defult.GetComponentInChildren<Text> ().text = "默认值";
			button_Back.GetComponentInChildren<Text> ().text = "取消";
			button_Next.GetComponentInChildren<Text> ().text = "下一阶段";
			button_Show.GetComponentInChildren<Text> ().text = "打开对照";
		} else {
			button_Save.GetComponentInChildren<Text> ().text = "Update";
			button_Defult.GetComponentInChildren<Text> ().text = "Default";
			button_Back.GetComponentInChildren<Text> ().text = "Cannel";
			button_Next.GetComponentInChildren<Text> ().text = "Next Level";
			button_Show.GetComponentInChildren<Text>().text = "Show Map";

		}
	}
	public virtual void UpdateSetValue () {		
	}

	public virtual void SaveOk () {
	}
	public virtual void DefaultOk () {
	}

	//
	public void OnClick_Save () {
		SaveOk ();
	}
	public void OnClick_Default () {
		DefaultOk ();
	}
	public void OnClick_Back () {
		gameObject.SetActive (false);
	}
	public void OnClick_Next()
	{
        if (SettingInGameRegistry.CurrentTarget != null)
        {
            SettingInGameRegistry.CurrentTarget.OnSettingNextLevel();
            return;
        }
        if (Set.setVal.GameChoose == 0)
        {

            if (Main.MapIndex == 7)
            {
                Game14_Main.instance.OnClickNextLevel();
            }
            else
            {
                Game_Map00.instance.TarageNum_now = 0;
                if (Main.MapID==25)
                {

                    Game_Map00.instance.Init_NewMap_01_next();
                }
                else if (Main.MapID == 1)
                {
                    Game_Map00.instance.Init_NewMap_02_Next();


                }
                else
                {
                    Game_Map00.instance.isClearAll = true;
                }
            }
           
        }
        else if (Set.setVal.GameChoose == 2)
        {
            Game_Map02.instance.TarageNum_now = 0;
            Game_Map02.instance.isClearAll = true;
        }
        }
	public void OnClick_Show()
	{
        if (SettingInGameRegistry.CurrentTarget != null)
        {
            SettingInGameRegistry.CurrentTarget.TogglePresetPicShow();
            return;
        }
        if (Set.setVal.GameChoose==0)
        {
            if (!Game00_Main.instance.presetPic_Layer.transform.parent.gameObject.activeSelf)
            {

                Game00_Main.instance.presetPic_Layer.transform.parent.gameObject.SetActive(true);
                Game00_Main.instance.presetPic_Layer.transform.localRotation = new Quaternion(0, 0, 0, 1);


            }
            else
            {

                Game00_Main.instance.presetPic_Layer.transform.parent.gameObject.SetActive(false);
                Game00_Main.instance.presetPic_Layer.transform.localRotation = new Quaternion(0, 0, 0, 1);

            }
        }
        else if (Set.setVal.GameChoose == 2)
        {
            if (!Game02_Main.instance.presetPic_Layer.transform.parent.gameObject.activeSelf)
            {

                Game02_Main.instance.presetPic_Layer.transform.parent.gameObject.SetActive(true);
                Game02_Main.instance.presetPic_Layer.transform.localRotation = new Quaternion(0, 0, 0, 1);


            }
            else
            {

                Game02_Main.instance.presetPic_Layer.transform.parent.gameObject.SetActive(false);
                Game02_Main.instance.presetPic_Layer.transform.localRotation = new Quaternion(0, 0, 0, 1);

            }
        }
		
	}
}
