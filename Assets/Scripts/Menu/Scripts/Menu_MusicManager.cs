using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum en_MusicType
{
	MusicIdle = 0,
	MusicGame,
}
public class Menu_MusicManager : MonoBehaviour {
	public Text text_Title;
	public GameObject fileNameSelectOne_Prefab;
	public Transform fileNameSelectOne_Layer;
	public Toggle toggle_AllSelect;
	public Button button_Delete;
	public Button button_Download;
	public Button button_Export;
	public Button button_Back;

	List<Menu_SelectOne> list_FileName = new List<Menu_SelectOne> ();
	en_MusicType musicType;
	//int selectId;
	float fileNameLayerLimitUp;
	string musicDirectory = MusicManager.MusicDirectory_Idle;
	bool filesChanged;
	

	enum en_RunState
	{
		Idle = 0,
		ChangeSelectId,
		Delete,
		Download,
		Export,
		Back,
	}
	en_RunState statue;

	// 传入的变量
	Menu menu;
	Menu_PasswordManager passwordManager;
	Menu_Tips menuTips;
	Menu_CreateTips createTips;
	public Menu_FileDownLoad gameDownLoad;

	public void Awake0 (Menu mmenu) {
		menu = mmenu;
		menuTips = menu.menuTips;
		createTips = menu.createTips;
		passwordManager = menu.passwordManager;
		gameDownLoad = menu.menu_GameDownLoad;
		//
		toggle_AllSelect.onValueChanged.AddListener (OnChange_AllSelect);
		button_Delete.onClick.AddListener (OnClick_Delete);
		button_Download.onClick.AddListener (OnClick_Download);
		button_Export.onClick.AddListener (OnClick_Export);
		button_Back.onClick.AddListener (OnClick_Back);
	}

	public void GameStart (en_MusicType type) {
		musicType = type;
		switch (musicType) {
		case en_MusicType.MusicIdle:
			musicDirectory = MusicManager.MusicDirectory_Idle;
			break;
		case en_MusicType.MusicGame:
			musicDirectory = MusicManager.MusicDirectory_Game;
			break;
		}
		//
		Update_Languae ();
		Update_FileNameList ();
	}


	// Update is called once per frame
	void Update () {
		if (fileNameSelectOne_Layer.transform.localPosition.y < 0) {
			fileNameSelectOne_Layer.transform.localPosition = new Vector3 (0, 0);
		} else if (fileNameSelectOne_Layer.transform.localPosition.y > fileNameLayerLimitUp) {
			fileNameSelectOne_Layer.transform.localPosition = new Vector3 (0, fileNameLayerLimitUp);
		}		
		if (menuTips.gameObject.activeSelf)
			return;
		if (gameDownLoad != null) {
			if (gameDownLoad.gameObject.activeSelf) {
				return;
			}
		}
		if (filesChanged) {
			Update_FileNameList ();
		}
		//
		if (statue != en_RunState.Idle) {
			ChangeStatue (en_RunState.Idle);
		}
	}
	void ChangeStatue (en_RunState sta) {
		statue = sta;
		switch (statue) {
		case en_RunState.Idle:
			break;
		case en_RunState.Delete:
			break;
		case en_RunState.Download:
			break;
		case en_RunState.Export:
			break;
		case en_RunState.Back:
			break;
		}
	}

	void Update_Languae () {
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			switch (musicType) {
			case en_MusicType.MusicIdle:
				text_Title.text = "待 机 音 乐";
				break;
			case en_MusicType.MusicGame:
				text_Title.text = "游 戏 音 乐";
				break;
			}
			button_Delete.GetComponentInChildren<Text> ().text = "删 除";
			button_Download.GetComponentInChildren<Text> ().text = "下 载";
			button_Export.GetComponentInChildren<Text> ().text = "导 出";
			button_Back.GetComponentInChildren<Text> ().text = "返 回";
		} else {
			//
			switch (musicType) {
			case en_MusicType.MusicIdle:
				text_Title.text = "Idle Music";
				break;
			case en_MusicType.MusicGame:
				text_Title.text = "Game Music";
				break;
			}
			button_Delete.GetComponentInChildren<Text> ().text = "Delete";
			button_Download.GetComponentInChildren<Text> ().text = "Download";
			button_Export.GetComponentInChildren<Text> ().text = "Export";
			button_Back.GetComponentInChildren<Text> ().text = "Return";
		}

	}


	void Update_FileNameList () {
		string[] files;
		filesChanged = false;

		string directory = Application.persistentDataPath + "/" + musicDirectory;
		if (Directory.Exists (directory)) {
			files = Directory.GetFiles (directory);
		} else {
			files = new string[0];
		}
		//
		for (; list_FileName.Count > 0;) {
			if (list_FileName[0] != null) {
				Destroy (list_FileName[0].gameObject);
			}
			list_FileName.RemoveAt (0);
		}
		list_FileName.Clear ();
		//
		for (int i = 0; i < files.Length; i++) {
			Menu_SelectOne selectOne = Instantiate (fileNameSelectOne_Prefab, fileNameSelectOne_Layer).GetComponent<Menu_SelectOne> ();
			selectOne.Init (i, OnClick_SelectFileName);
			selectOne.SetName (Path.GetFileName (files[i]));
			list_FileName.Add (selectOne);
		}
		fileNameLayerLimitUp = list_FileName.Count * 70 - 700 + 50;
		if (fileNameLayerLimitUp < 0) {
			fileNameLayerLimitUp = 0;
		}
		//selectId = -1;
		Update_SelectId ();
	}

	void Update_SelectId () {
		//for (int i = 0; i < list_FileName.Count; i++) {
		//	if (i == selectId) {
		//		list_FileName[i].SetSelect (true);
		//	} else {
		//		list_FileName[i].SetSelect (false);
		//	}
		//}
	}

	int GetPointStaIndex (enPointSta[] arry, enPointSta value) {
		for (int i = 0; i < arry.Length; i++) {
			if (value == arry[i]) {
				return i;
			}
		}
		return 0;
	}
	
	void OnClick_SelectFileName (int id, bool isOn) {
		//if (selectId == id)
		//	return;		
		//selectId = id;
		Update_SelectId ();
	}
	public void OnChange_AllSelect (bool isOn) {
		for (int i = 0; i < list_FileName.Count; i++) {
			list_FileName[i].SetIsOn (isOn);
		}
	}

	void DeleteFiles (bool isOk) {
		if (isOk == false)
			return;
		string filePath;
		for (int i = 0; i < list_FileName.Count; i++) {
			if (list_FileName[i].IsOn () == false)
				continue;
			filePath = Application.persistentDataPath + "/" + musicDirectory + list_FileName[i].text.text;
			File.Delete (filePath);
		}
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init ("删除成功", 3, true);
		} else {
			menuTips.Init ("Deleted successfully", 3, true);
		}
		//
		filesChanged = true;
	}
	public void OnClick_Delete () {
		if (statue != en_RunState.Idle)
			return;
		int fileCount = 0;
		for (int i = 0; i < list_FileName.Count; i++) {			
			if (list_FileName[i].IsOn ()) {
				fileCount++;
			}
		}
		if (fileCount == 0) {
			if (Set.setVal.Language == (int)en_Language.Chinese) {
				menuTips.Init ("请选择要删除的文件", 3, true);
			} else {
				menuTips.Init ("Please select the file want to delete", 3, true);
			}
			return;
		}
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init ("是否删除选择的文件?", DeleteFiles);
		} else {
			menuTips.Init ("Do you want to delete selected files ?", DeleteFiles);
		}
	}

	en_FileType GetFileType () {
		if (musicType == en_MusicType.MusicIdle) {
			return en_FileType.MusicIdle;
		} else {
			return en_FileType.MusicGame;
		}
	}
	public void OnClick_Download () {
		gameDownLoad.GameStart (en_LoadType.Download, GetFileType ());
		filesChanged = true;
	}
	public void OnClick_Export () {
		gameDownLoad.GameStart (en_LoadType.Export, GetFileType ());
	}


	public void OnClick_Back () {
		gameObject.SetActive (false);
		//menu.ChangeStatue (en_MenuStatue.MenuSta_MusicMenu);
	}
}
