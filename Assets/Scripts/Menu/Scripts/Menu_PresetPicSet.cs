using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Menu_PresetPicSet :MonoBehaviour {
	public Text text_Title;
	public Text text_DescriptTitle;
	public InputField intputField_Descript;
	public Button[] button_PointType;
	public GameObject selectTypeFlag;
	public GameObject fileNameSelectOne_Prefab;
	public Transform fileNameSelectOne_Layer;
	public GameObject ledOne_Prefab;
	public Transform ledOne_Layer;
	public Button button_Delete;
	public Button button_Create;
	public Button button_Download;
	public Button button_Export;
	public Button button_Clear;
	public Button button_Ok;
	public Button button_Back;

	PresetPic persetPicSetting;
	List<Menu_Button> list_FileName = new List<Menu_Button> ();
	public List<Menu_LedOne> list_Led = new List<Menu_LedOne> ();
	int selectId;
	int nextSelectId;
	int currPointType;
	float fileNameLayerLimitUp;
    float ledLayerLimitUp;
    float ledLayerLimitX;
    Vector2 ledLayerPos;

    readonly enPointSta[] tab_PointType = { enPointSta.None, enPointSta.Rest, enPointSta.Target, enPointSta.Die };
	readonly Color[] tab_PointColor = { Color.black, Color.green, Color.blue, Color.red };

	enum en_RunState {
		Idle = 0,
		ChangeSelectId,
		Delete,
		Create,
		Download,
		Export,
		Clear,
		Save,
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
		button_PointType[0].onClick.AddListener (() => OnClick_SelectPointType (0));
		button_PointType[1].onClick.AddListener (() => OnClick_SelectPointType (1));
		button_PointType[2].onClick.AddListener (() => OnClick_SelectPointType (2));
		button_PointType[3].onClick.AddListener (() => OnClick_SelectPointType (3));
		button_Delete.onClick.AddListener (OnClick_Delete);
		button_Create.onClick.AddListener (OnClick_Create);
		button_Download.onClick.AddListener (OnClick_Download);
		button_Export.onClick.AddListener (OnClick_Export);
		button_Clear.onClick.AddListener (OnClick_Clear);
		button_Ok.onClick.AddListener (OnClick_Save);
		button_Back.onClick.AddListener (OnClick_Back);
	}

    const int LED_WIDHT = 50;
    public void GameStart () {

		int bufLen = Mathf.Min (Set.setVal.Width * Set.setVal.Height, Main.MAX_LED);

		for (int i = 0; i < list_Led.Count; i++) {
			if (list_Led[i] != null) {
				Destroy (list_Led[i].gameObject);
			}
		}
		list_Led.Clear ();
        ledLayerLimitX = Set.setVal.Width * LED_WIDHT - 1000 + LED_WIDHT;
        if (ledLayerLimitX < 0) {
            ledLayerLimitX = 0;
        }
        ledLayerLimitUp = Set.setVal.Height * LED_WIDHT - 700 + LED_WIDHT;
		if (ledLayerLimitUp < 0) {
			ledLayerLimitUp = 0;
		}
		//
		int x = 0;
		int y = 0;
		for (int i = 0; i < bufLen; i++) {
			Menu_LedOne ledOne = Instantiate (ledOne_Prefab, ledOne_Layer).GetComponent<Menu_LedOne> ();
			ledOne.Init (i, OnClickLed);
			ledOne.transform.localPosition = new Vector3 (50 + x * 50, -y * 50);
			if (++x >= Set.setVal.Width) {
				x = 0;
				y++;
			}
			list_Led.Add (ledOne);
		}
		//
		currPointType = 2;
		//
		Update_Languae ();
		Update_FileNameList ();
		Update_SetValue ();
		Update_CurrPointType ();
	}


	// Update is called once per frame
	void Update () {
		if (fileNameSelectOne_Layer.transform.localPosition.y < 0) {
			fileNameSelectOne_Layer.transform.localPosition = new Vector3 (0, 0);
		} else if (fileNameSelectOne_Layer.transform.localPosition.y > fileNameLayerLimitUp) {
			fileNameSelectOne_Layer.transform.localPosition = new Vector3 (0, fileNameLayerLimitUp);
		}
		//if (ledOne_Layer.transform.parent.localPosition.y < 0) {
		//	ledOne_Layer.transform.parent.localPosition = new Vector3 (0, 0);
		//} else if (ledOne_Layer.transform.parent.localPosition.y > ledLayerLimitUp) {
		//	ledOne_Layer.transform.parent.localPosition = new Vector3 (0, ledLayerLimitUp);
		//}
        ledLayerPos = ledOne_Layer.transform.parent.localPosition;
        ledLayerPos.x = Mathf.Clamp(ledLayerPos.x, -ledLayerLimitX, 0);
        ledLayerPos.y = Mathf.Clamp(ledLayerPos.y, 0, ledLayerLimitUp);
        ledOne_Layer.transform.parent.localPosition = ledLayerPos;

        if (menuTips.gameObject.activeSelf)
			return;
		//
		if (statue != en_RunState.Idle) {
			ChangeStatue (en_RunState.Idle);
		}

#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.T)) {
			Debug.Log ("转换图案：");
			PicTransMit ();
		}
#endif
	}
	void ChangeStatue (en_RunState sta) {
		statue = sta;
		switch (statue) {
		case en_RunState.Idle:
			break;
		case en_RunState.Delete:
			break;
		case en_RunState.Create:
			break;
		case en_RunState.Download:
			break;
		case en_RunState.Export:
			break;
		case en_RunState.Clear:
			break;
		case en_RunState.Save:
			break;
		case en_RunState.Back:
			break;
		}
	}

	void Update_Languae () {
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			text_Title.text = "图 案 设 置";
			text_DescriptTitle.text = "备 注";

			button_Delete.GetComponentInChildren<Text> ().text = "删 除";
			button_Create.GetComponentInChildren<Text> ().text = "创 建";
			button_Download.GetComponentInChildren<Text> ().text = "下 载";
			button_Export.GetComponentInChildren<Text> ().text = "导 出";
			button_Clear.GetComponentInChildren<Text> ().text = "清 空";
			button_Ok.GetComponentInChildren<Text> ().text = "保 存";
			button_Back.GetComponentInChildren<Text> ().text = "返 回";
		} else {
			//
			text_Title.text = "Graphic Setting";
			text_DescriptTitle.text = "Note";

			button_Delete.GetComponentInChildren<Text> ().text = "Delete";
			button_Create.GetComponentInChildren<Text> ().text = "Create";
			button_Download.GetComponentInChildren<Text> ().text = "Download";
			button_Export.GetComponentInChildren<Text> ().text = "Export";
			button_Clear.GetComponentInChildren<Text> ().text = "Clear";
			button_Ok.GetComponentInChildren<Text> ().text = "Save";
			button_Back.GetComponentInChildren<Text> ().text = "Return";

		}

	}

	void Update_FileNameList () {
		string[] files;
		string drectory = PresetPic.GetDirectory ();
		if (Directory.Exists (drectory)) {
			files = Directory.GetFiles (drectory);
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
			Menu_Button selectOne = Instantiate (fileNameSelectOne_Prefab, fileNameSelectOne_Layer).GetComponent<Menu_Button> ();
			selectOne.Init (i, OnClick_SelectFileName);
			selectOne.SetName (Path.GetFileName (files[i]));
			list_FileName.Add (selectOne);
		}
		fileNameLayerLimitUp = list_FileName.Count * 70 - 700 + 50;
		if (fileNameLayerLimitUp < 0) {
			fileNameLayerLimitUp = 0;
		}
		selectId = -1;
		Update_SelectId ();
	}

	void Update_SelectId () {
		for (int i = 0; i < list_FileName.Count; i++) {
			if (i == selectId) {
				list_FileName[i].SetSelect (true);
			} else {
				list_FileName[i].SetSelect (false);
			}
		}
		//
		if (selectId >= 0 && selectId < list_FileName.Count) {
			persetPicSetting = PresetPic.LoadSetting (list_FileName[selectId].text.text);
		} else {
			persetPicSetting = null;
		}
		Update_SetValue ();
	}

	int GetPointStaIndex (enPointSta[] arry, enPointSta value) {
		for (int i = 0; i < arry.Length; i++) {
			if (value == arry[i]) {
				return i;
			}
		}
		return 0;
	}
	void Update_SetValue () {
		int id;
		if (persetPicSetting == null) {
			// 描述
			intputField_Descript.text = "";
			// 图案
			ledOne_Layer.gameObject.SetActive (false);
			for (int i = 0; i < list_Led.Count; i++) {
				list_Led[i].SetColor (Color.black, (byte)enPointSta.None);
			}
		} else {
			// 描述
			intputField_Descript.text = persetPicSetting.descript;
			// 图案
			ledOne_Layer.gameObject.SetActive (true);
			//for (int i = 0; i < list_Led.Count && i < persetPicSetting.dataBuff.Length; i++) {
			//	id = GetPointStaIndex (tab_PointType, (enPointSta)persetPicSetting.dataBuff[i]);
			//	list_Led[i].SetColor (tab_PointColor[id], (byte)tab_PointType[id]);
			//}

			int pointType;
			int bufId;
			for (int i = 0; i < Set.setVal.Width && i < PresetPic.PIC_WIDTH; i++) {
				for (int j = 0; j < Set.setVal.Height && j < PresetPic.PIC_HEIGHT; j++) {
					id = j * Set.setVal.Width + i;
					bufId = j * PresetPic.PIC_WIDTH + i;
					//
					if (bufId >= persetPicSetting.dataBuff.Length)
						continue;
					if (id >= list_Led.Count)
						return;
					pointType = GetPointStaIndex (tab_PointType, (enPointSta)persetPicSetting.dataBuff[bufId]);
					list_Led[id].SetColor (tab_PointColor[pointType], (byte)tab_PointType[pointType]);
				}
			}
		}
	}

	bool IsChanged (PresetPic presetPic) {
		if (presetPic == null)
			return true;
		if (presetPic.descript != intputField_Descript.text)
			return true;
		int id;
		int bufId;
		for (int i = 0; i < Set.setVal.Width && i < PresetPic.PIC_WIDTH; i++) {
			for (int j = 0; j < Set.setVal.Height && j < PresetPic.PIC_HEIGHT; j++) {
				id = j * Set.setVal.Width + i;
				bufId = j * PresetPic.PIC_WIDTH + i;
				if (bufId >= presetPic.dataBuff.Length)
					continue;
				if (id >= list_Led.Count)
					return true;
				if (presetPic.dataBuff[bufId] != list_Led[id].pointType) {
					return true;
				}
			}
		}
		//     for (int i = 0; i < presetPic.dataBuff.Length && i < list_Led.Count; i++) {
		//if (presetPic.dataBuff[i] != list_Led[i].pointType) {
		//	return true;
		//}
		//     }
		return false;
	}


	void Update_CurrPointType () {
		if (currPointType < button_PointType.Length) {
			selectTypeFlag.transform.position = button_PointType[currPointType].transform.position;
			selectTypeFlag.SetActive (true);
		} else {
			selectTypeFlag.SetActive (false);
		}
	}

	//public void GetSetting (byte[] buff) {
	//       for (int i = 0; i < list_Led.Count && i < buff.Length; i++) {
	//		buff[i] = list_Led[i].pointType;
	//	}
	//}
	//public void GetSetting (byte[,] buff) {
	public void GetSetting (byte[] buff) {
		int id;
		int bufId;
		for (int i = 0; i < Set.setVal.Width && i < PresetPic.PIC_WIDTH; i++) {
			for (int j = 0; j < Set.setVal.Height && j < PresetPic.PIC_HEIGHT; j++) {
				id = j * Set.setVal.Width + i;
				bufId = j * PresetPic.PIC_WIDTH + i;
				if (bufId >= buff.Length)
					continue;
				if (id >= list_Led.Count)
					return;
				buff[bufId] = list_Led[id].pointType;
			}
		}
	}

	void SaveChangedSetting (bool isOk) {
		if (isOk) {
			OnClick_Save ();
			return;
		}
		selectId = nextSelectId;
		Update_SelectId ();
	}
	void OnClick_SelectFileName (int id) {
		if (selectId == id)
			return;
		if (persetPicSetting != null) {
			if (IsChanged (persetPicSetting)) {
				nextSelectId = id;
				if (Set.setVal.Language == (int)en_Language.Chinese) {
					menuTips.Init ("修改未保存，是否保存", SaveChangedSetting);
				} else {
					menuTips.Init ("The modification was not saved. Do you want to save it", SaveChangedSetting);
				}
				return;
			}
		}
		selectId = id;
		Update_SelectId ();
	}

	void OnClickLed (int id) {
#if UNITY_EDITOR
		//Debug.Log ("OnClickLed: " + id);
#endif
		if (id >= 0 && id < list_Led.Count) {
			int type = 0;
			if (currPointType >= 0 && currPointType < tab_PointType.Length) {
				type = currPointType;
			}
			if (list_Led[id].pointType != (byte)tab_PointType[type]) {
				list_Led[id].SetColor (tab_PointColor[type], (byte)tab_PointType[type]);
			} else {
				list_Led[id].SetColor (tab_PointColor[0], (byte)enPointSta.None);
			}
		}
	}

	public void OnClick_SelectPointType (int id) {
		currPointType = id;
		Update_CurrPointType ();
	}

    void DeleteOne(bool isOk) {
        if (isOk == false)
            return;
        if (selectId >= 0 && selectId < list_FileName.Count) {
            string fileName = list_FileName[selectId].text.text;
            string filePath = PresetPic.GetDirectory() + fileName;
            File.Delete(filePath);
			if (Set.setVal.Language == (int)en_Language.Chinese) {
				menuTips.Init (fileName + " 删除成功", 3, true);
			} else {
				menuTips.Init (fileName + " deleted successfully", 3, true);
			}
            //
            Update_FileNameList();
        }
    }
    public void OnClick_Delete () {
		if (statue != en_RunState.Idle)
			return;
		if (selectId < 0 || selectId >= list_FileName.Count)
			return;
		string fileName = list_FileName[selectId].text.text;
		if (string.IsNullOrEmpty (fileName)) 
			return;
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init ("是否删除 " + fileName, DeleteOne);
		} else {
			menuTips.Init ("Do you want to delete " + fileName, DeleteOne);
		}
	}

    void CreateFile(string fileName) {
        if (statue != en_RunState.Idle)
            return;
		if (string.IsNullOrEmpty (fileName)) {
			return;
		}
		fileName = "U-" + fileName;
        string filePath = PresetPic.GetDirectory() + fileName;
        if (File.Exists(filePath)) {
			if (Set.setVal.Language == (int)en_Language.Chinese) {
				menuTips.Init (fileName + " 已存在", 3, true);
			} else {
				menuTips.Init (fileName + " already exist", 3, true);
			}
            return;
        }
        PresetPic presetPic = new PresetPic();
        presetPic.Default();
        PresetPic.SaveSetting(fileName, presetPic);
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init (fileName + " 创建成功", 3, true);
		} else {
			menuTips.Init (fileName + " created successfully", 3, true);
		}
        //
        Update_FileNameList();
    }
    public void OnClick_Create () {
        if (statue != en_RunState.Idle)
            return;
		createTips.GameStart(CreateFile);
    }

    public void OnClick_Download() {
        gameDownLoad.GameStart(en_LoadType.Download, en_FileType.PresetPicSet);
    }
    public void OnClick_Export() {
        gameDownLoad.GameStart(en_LoadType.Export, en_FileType.PresetPicSet);
    }


    public void OnClick_Clear () {
		for (int i = 0; i < list_Led.Count; i++) {
			list_Led[i].SetColor (tab_PointColor[0], (byte)enPointSta.None);
		}
	}

    public void OnClick_Save() {
		if (selectId < 0 || selectId >= list_FileName.Count)
			return;
		string filePath = list_FileName[selectId].text.text;
        GetSetting(persetPicSetting.dataBuff);
		persetPicSetting.descript = intputField_Descript.text;
        PresetPic.SaveSetting(filePath, persetPicSetting);
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init ("保存成功", 3, true);
		} else {
			menuTips.Init ("Saved successfully", 3, true);
		}
    }

    public void OnClick_Back () {
		//gameObject.SetActive (false);
		menu.ChangeStatue (en_MenuStatue.MenuSta_SysSet);
	}

#if UNITY_EDITOR
	void TransMitBuf (byte[] outBuf, byte[] inBuf) {
		int oldWitdh = 40;
		int oldHeight = 40;
        if (Set.setVal.GameChoose==2)
        {
            oldWitdh = 100;
              oldHeight = 10;
        }
		int idIn;
		int idOut;

		for (int y = 0; y < PresetPic.PIC_HEIGHT; y++) {
			for (int x = 0; x < PresetPic.PIC_WIDTH; x++) {
				idOut = x + y * PresetPic.PIC_WIDTH;
				if (y >= oldHeight || x >= oldHeight) {
					outBuf[idOut] = 0;
					continue;
				}
				idIn = x + y * oldWitdh;
				outBuf[idOut] = inBuf[idIn];
            }
        }
	}
	void PicTransMit () {
		int gameId = Mathf.Clamp (Set.setVal.GameChoose, 0, PresetPic.tab_DefaultFiles.Length - 1);
		string[] defaultFile = PresetPic.tab_DefaultFiles[gameId];
		PresetPic presetPicNew = new PresetPic ();

		for (int i = 0; i < defaultFile.Length; i++) {
			PresetPic presetPic = PresetPic.LoadSetting (defaultFile[i]);
			//
			presetPicNew.descript = presetPic.descript;
			TransMitBuf (presetPicNew.dataBuff, presetPic.dataBuff);
			//			
			PresetPic.SaveSetting (defaultFile[i], presetPicNew);
		}
	}
#endif
}
