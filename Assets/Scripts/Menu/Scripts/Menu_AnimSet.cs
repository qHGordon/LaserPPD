using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;


public class Menu_AnimSet : MonoBehaviour {
	public Text text_Title;
	public Text text_AnimName;	
    public Text text_DescriptTitle;    
	public Menu_SetOne setOne_AnimCount;
	public Menu_SetOne setOne_PlayerMode;
	public InputField intputField_Descript;
	//public GameObject selectFlag;
	public GameObject animSetName_Prefab;
	public GameObject animSetOne_Prefab;
	public Transform animSetName_Layer;
	public Transform animSetOne_Layer;

	public Button button_Descript;
	public Button button_Preview;
    public Button button_Delete;
    public Button button_Create;
    public Button button_Download;
    public Button button_Export;
    public Button button_Save;
    public Button button_Default;
	public Button button_Back;


    AnimSet animSetting;
	AnimSet showSetting;
	Menu_AnimSetOne[] animSetOne = new Menu_AnimSetOne[Main.MAX_ANIM];
	GameLevelSetting levelSettingPreview = new GameLevelSetting();
    List<Menu_Button> list_FileName = new List<Menu_Button>();


    int selectId;
	int nextSelectId;
	int animCount;
	float animSetLayerLimitUp;
	float animSetOneLayerLimitUp;

	public en_Game00_Sta statue;
	float runTime;
	int gameId = 20;
	public int setIndex;
	public int setCount;
	bool cmdRetSucess;
	int gameLevel;

	en_PlayerMode playerMode = en_PlayerMode.Free;

	public static Menu_AnimSet instance;

    public Menu_FileDownLoad gameDownLoad;
    Menu_Tips menuTips;
    Menu_CreateTips createTips;
    void Awake () {
		instance = this;

		levelSettingPreview.Default (Set.setVal.GameChoose, 0, 0);
		levelSettingPreview.picSetting = new PresetPic ();
		levelSettingPreview.animSetting = new AnimSet ();
		levelSettingPreview.animSetting.Default ();
	}

	void OnDisable () {
		ChangeStatue (en_Game00_Sta.None);
	}

	Menu menu;
	public void Awake0 (Menu mmenu) {
		menu = mmenu;
        menuTips = menu.menuTips;
        createTips = menu.createTips;
		gameDownLoad = menu.menu_GameDownLoad;

		button_Descript.onClick.AddListener (OnClick_Descript);
		button_Delete.onClick.AddListener(OnClick_Delete);
		button_Create.onClick.AddListener(OnClick_Create);
        button_Download.onClick.AddListener(OnClick_Download);
        button_Export.onClick.AddListener(OnClick_Export);
        button_Preview.onClick.AddListener (OnClick_Preview);
		button_Save.onClick.AddListener (OnClick_Save);
		button_Default.onClick.AddListener (OnClick_Default);
		button_Back.onClick.AddListener (OnClick_Back);
		//
		setOne_PlayerMode.SetValueInit (Set.SET_c_PlayerMode);
		//

		//for (int i = 0; i < gameLevelSetting.Length; i++) {
		//	gameLevelSetting[i] = new GameLevelSetting ();
		//}
		for (int i = 0; i < animSetOne.Length; i++) {
			animSetOne[i] = Instantiate (animSetOne_Prefab, animSetOne_Layer).GetComponent<Menu_AnimSetOne> ();
			animSetOne[i].Awake0(i);
        }
		//

        setOne_AnimCount.SetValueInit(AnimSet.tab_animCount);
    }

	// Use this for initialization	
	public void GameStart () {
		//
		//settingValue:
		setOne_PlayerMode.UpdateValue ((int)playerMode);
		//
		Update_FileNameList();

        
		// 保存当前值
		Update_Languae ();

		gameLevel = 0;
		selectId = -1;
		Update_SelectId ();

		ChangeStatue (en_Game00_Sta.None);
	}

	void Update_FileNameList() {
        string[] files;
        string drectory = AnimSet.GetDirectory();
        if (Directory.Exists(drectory)) {
            files = Directory.GetFiles(drectory);
        } else {
            files = new string[0];
        }
		//
		for (; list_FileName.Count > 0;) {
			if (list_FileName[0] != null) {
				Destroy(list_FileName[0].gameObject);
			}
			list_FileName.RemoveAt (0);
		}
        list_FileName.Clear();
        //
        for (int i = 0; i < files.Length; i++) {
            Menu_Button animName = Instantiate(animSetName_Prefab, animSetName_Layer).GetComponent<Menu_Button>();
			animName.Init(i, OnClick_AnimName);
			animName.SetName(Path.GetFileName(files[i]));
            list_FileName.Add(animName);
		}
		//		
        animSetLayerLimitUp = list_FileName.Count * 70 - 500 + 50;
        if (animSetLayerLimitUp < 0) {
			animSetLayerLimitUp = 0;
		}
		//
        selectId = -1;
        Update_SelectId();
    }
	

	// Update is called once per frame
	void Update () {
		if (animSetting != null) {
			if (animCount != setOne_AnimCount.GetValue()) {
				Update_AnimOneList (setOne_AnimCount.GetValue ());
			}
		}
		if (animSetName_Layer.transform.localPosition.y < 0) {
			animSetName_Layer.transform.localPosition = new Vector3 (0, 0);
		} else if (animSetName_Layer.transform.localPosition.y > animSetLayerLimitUp) {
			animSetName_Layer.transform.localPosition = new Vector3 (0, animSetLayerLimitUp);
		}
		if (animSetOne_Layer.transform.localPosition.y < 0) {
			animSetOne_Layer.transform.localPosition = new Vector3 (0, 0);
		} else if (animSetOne_Layer.transform.localPosition.y > animSetOneLayerLimitUp) {
			animSetOne_Layer.transform.localPosition = new Vector3 (0, animSetOneLayerLimitUp);
		}

		switch (statue) {
		case en_Game00_Sta.Idle:
            break;
        
        case en_Game00_Sta.Play:
            break;
        }


	}

	void ChangeStatue (en_Game00_Sta sta) {
		statue = sta;
		runTime = 0;
		setIndex = 0;
		setCount = 0;
		cmdRetSucess = false;

//		CmdIO_YDGZ.CMD0_SendCmd_GameStatue (gameId, gameLevel, (int)statue);

		switch (statue) {
		case en_Game00_Sta.None:
		case en_Game00_Sta.Idle:
			//CmdIO_YDGZ.CMD0_SendCmd_GameStatue (0, 0, 0);
			break;
		
		case en_Game00_Sta.Play:
			break;
		}
	}
	bool TimePassed (float passTime) {
		if (runTime > 0) {
			runTime -= Time.deltaTime;
		} else {
			runTime = passTime;
			return true;
		}
		return false;
	}

	void SendPresetPoint () {
		PresetPic.SendPresetPoint (gameId, levelSettingPreview.picSetting, ref setIndex);
	}

	//readonly enPointSta[] tab_PointSta = { enPointSta.Rest, enPointSta.Die, enPointSta.Target };
	//void SendPresetPoint () {
	//	if (setIndex >= tab_PointSta.Length)
	//		return;
	//	byte[] buf = new byte[128];
	//	byte pointType = (byte)tab_PointSta[setIndex];
	//	int bufLen = Mathf.Min (Set.setVal.Width * Set.setVal.Height, levelSettingPreview.picSetting.dataBuff.Length);
	//	int pointLen = 0;
	//	int bitn = 0;
	//	int len = 0;
	//	for (int i = 0; i < bufLen; i++) {
	//		if (bitn == 0) {
	//			buf[len] = 0;
	//		}
	//		if (levelSettingPreview.picSetting.dataBuff[i] == pointType) {
	//			buf[len] |= (byte)(1 << bitn);
	//			pointLen++;
	//		}
	//		if (++bitn >= 7) {
	//			bitn = 0;
	//			len++;
	//		}
	//	}
	//	if (bitn > 0) {
	//		len++;
	//	}
	//	if (pointLen == 0) {
	//		setIndex++;
	//		return;
	//	}
	//	//
	//	if (CmdIO_YDGZ.CMD0_SendCmd_PresetPoint (gameId, setIndex, pointType, buf, (byte)len) == false) {
	//		setIndex++;
	//	}
	//}

	//----------------------------------------------------------------------------------
	void Update_AnimOneList (int count) {
		animCount = count;
		if (animSetting == null) {
			animSetOneLayerLimitUp = 0;
			return;
		}	
		for (int i = 0; i < animSetOne.Length; i++) {
			if (i < animCount) {
				animSetOne[i].gameObject.SetActive (true);
				animSetOne[i].GameStart (animSetting.animOne[i]);
			} else {
				animSetOne[i].gameObject.SetActive (false);
			}
		}
		//
		animSetOneLayerLimitUp = animCount * 840 - 700 + 50;
		if (animSetOneLayerLimitUp < 0)
			animSetOneLayerLimitUp = 0;
	}
	void Update_CurrAnimSetting() {
		showSetting = animSetting;
		if (animSetting == null) {
			setOne_AnimCount.UpdateValue(-1);
			intputField_Descript.text = "";
			for (int i = 0; i < animSetOne.Length; i++) {
				animSetOne[i].gameObject.SetActive(false);
            }
			Update_AnimOneList (0);
		} else {
			setOne_AnimCount.UpdateValue(animSetting.animCount);
			intputField_Descript.text = animSetting.descript;
			Update_AnimOneList (animSetting.animCount);
		}
	}
	void Update_SelectId () {
		for (int i = 0; i < list_FileName.Count; i++) {
			if (i == selectId) {
                list_FileName[i].SetSelect(true);
			} else {
                list_FileName[i].SetSelect(false);
			}
		}
		if (selectId >= 0 && selectId < list_FileName.Count) {
			//
			animSetting = AnimSet.LoadSetting(list_FileName[selectId].text.text);
		} else {
			animSetting = null;
            //selectFlag.SetActive(false);
            for (int i = 0; i < animSetOne.Length; i++) {
				animSetOne[i].gameObject.SetActive(false);
			}
		}
		Update_CurrAnimSetting();
    }

	void Update_Languae () {
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			text_Title.text = "动 画 设 置";
			text_DescriptTitle.text = "备 注:";

			button_Descript.GetComponentInChildren<Text> ().text = "自动备注";
			button_Delete.GetComponentInChildren<Text> ().text = "删 除";
			button_Create.GetComponentInChildren<Text> ().text = "创 建";
			button_Download.GetComponentInChildren<Text> ().text = "下 载";
			button_Export.GetComponentInChildren<Text> ().text = "导 出";
			button_Preview.GetComponentInChildren<Text> ().text = "预 览";
			button_Save.GetComponentInChildren<Text> ().text = "保 存 设 置";
			button_Default.GetComponentInChildren<Text> ().text = "默 认 值";
			button_Back.GetComponentInChildren<Text> ().text = "返 回";

			//
			setOne_AnimCount.SetName ("动画个数");
			setOne_PlayerMode.SetName ("玩家模式");
			setOne_PlayerMode.SetValueName ((int)en_PlayerMode.PassLevel, "闯关模式");
			setOne_PlayerMode.SetValueName ((int)en_PlayerMode.Challenge, "对战模式");
		} else {
			// 英文
			text_Title.text = "Animation setting";
			text_DescriptTitle.text = "Note:";

			button_Descript.GetComponentInChildren<Text> ().text = "Auto note";
			button_Delete.GetComponentInChildren<Text> ().text = "Delete";
			button_Create.GetComponentInChildren<Text> ().text = "Create";
			button_Download.GetComponentInChildren<Text> ().text = "Download";
			button_Export.GetComponentInChildren<Text> ().text = "Export";
			button_Preview.GetComponentInChildren<Text> ().text = "Preview";
			button_Save.GetComponentInChildren<Text> ().text = "Save";
			button_Default.GetComponentInChildren<Text> ().text = "Default";
			button_Back.GetComponentInChildren<Text> ().text = "Return";

			//
			setOne_PlayerMode.SetName ("Play Mode");
			setOne_AnimCount.SetName ("Animation Count");
			setOne_PlayerMode.SetValueName ((int)en_PlayerMode.PassLevel, "Pass game");
			setOne_PlayerMode.SetValueName ((int)en_PlayerMode.Challenge, "Battle Game");
		}
	}




	bool IsChanged () {
		if (showSetting == null)
			return false;
		if (animSetting == null)
			return true;
		if (animSetting.descript != intputField_Descript.text)
			return true;
		if (animSetting.animCount != setOne_AnimCount.GetValue ())
			return true;
		for (int i = 0; i < animSetOne.Length && i < animSetting.animCount; i++) {
			if (animSetOne[i].Equals (animSetting.animOne[i]) == false) {
				return true;
			}
		}
		return false;
	}

	void SaveChangedSetting (bool isOk) {
		if (isOk) {
			SaveSet (isOk);
			return;
		}
		selectId = nextSelectId;
		Update_SelectId ();
	}
	public void OnClick_AnimName (int id) {
		if (selectId == id)
			return;

		if (selectId >= 0 && selectId < list_FileName.Count) {
			// 切换前，检查是否有修改未保存
			if (IsChanged ()) {
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


	public void OnClick_Descript () {
		if (selectId < 0 || selectId > list_FileName.Count)
			return;
		int count = setOne_AnimCount.GetValue ();
		string str = "";
        for (int i = 0; i < count && i < animSetOne.Length; i++) {
			str += animSetOne[i].GetAutoDescripts() + ";\r\n";
		}
		intputField_Descript.text = str;
	}


	void DeleteOne(bool isOk) {
        if (isOk == false)
            return;
        if (selectId >= 0 && selectId < list_FileName.Count) {
            string fileName = list_FileName[selectId].text.text;
			if (string.IsNullOrEmpty (fileName))
				return;
			string filePath = AnimSet.GetDirectory() + fileName;
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
    public void OnClick_Delete() {
   //     if (statue != en_RunState.Idle)
			//return;
        if (selectId < 0 || selectId >= list_FileName.Count)
            return;
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init ("是否删除", DeleteOne);
		} else {
			menuTips.Init ("Do you want to delete", DeleteOne);
		}
    }

    void CreateOne(string fileName) {
		//if (statue != en_RunState.Idle)
		//    return;
		if (string.IsNullOrEmpty (fileName)) {
			return;
		}
		fileName = "U-" + fileName;
        string filePath = AnimSet.GetDirectory() + fileName;
        if (File.Exists(filePath)) {
			if (Set.setVal.Language == (int)en_Language.Chinese) {
				menuTips.Init (fileName + " 已存在", 3, true);
			} else {
				menuTips.Init (fileName + " already exist", 3, true);
			}
            return;
        }
        AnimSet animSet = new AnimSet();
        animSet.Default();
        AnimSet.SaveSetting(fileName, animSet);
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init (fileName + " 创建成功", 3, true);
		} else {
			menuTips.Init (fileName + " created successfully", 3, true);
		}
        //
        Update_FileNameList();
    }
    public void OnClick_Create() {
        //if (statue != en_RunState.Idle)
        //    return;
        createTips.GameStart(CreateOne);
    }

	public void OnClick_Download() {
		gameDownLoad.GameStart(en_LoadType.Download, en_FileType.AnimSet);
	}
	public void OnClick_Export() {
        gameDownLoad.GameStart(en_LoadType.Export, en_FileType.AnimSet);
    }

    public void OnClick_Preview () {
		//Debug.Log ("预览");
		if (statue == en_Game00_Sta.None) {
			if (animSetting == null)
				return;
			
			if (selectId >= 0 && selectId < list_FileName.Count) {
				levelSettingPreview.animSetting.animCount = setOne_AnimCount.GetValue ();

				for (int i = 0; i < levelSettingPreview.animSetting.animOne.Length; i++) {
					animSetOne[i].GetSetting (levelSettingPreview.animSetting.animOne[i]);
                }
				playerMode = (en_PlayerMode)setOne_PlayerMode.GetValue ();
				ChangeStatue (en_Game00_Sta.Play);
			}
		} else if (statue == en_Game00_Sta.Play) {
			ChangeStatue (en_Game00_Sta.None);
		}
	}

	void SaveSet (bool isOk) {
		if (isOk == false)
			return;
		if (selectId < 0 || selectId >= list_FileName.Count)
			return;
		if (animSetting == null)
			return;
		string fileName = list_FileName[selectId].text.text;

		// 读取
		animSetting.descript = intputField_Descript.text;
		animSetting.animCount = setOne_AnimCount.GetValue ();
		
		for (int i = 0; i < animSetting.animOne.Length && i < animSetting.animCount; i++) {
			animSetOne[i].GetSetting (animSetting.animOne[i]);
		}
		//Debug.Log ("备注：" + animSetting.descript);
		AnimSet.SaveSetting (fileName, animSetting);

		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init ("保存设置成功", 3, true);
		} else {
			menuTips.Init ("Saved successfully", 3, true);
		}
	}
	public void OnClick_Save () {
		if (selectId < 0 || selectId >= list_FileName.Count)
			return;
		if (animSetting == null)
			return;
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			menuTips.Init ("是否保存设置", SaveSet);
		} else {
			menuTips.Init ("Save setting?", SaveSet);
		}
	}

	void DefaultSet (bool isOk) {
		if (isOk == false)
			return;
		if (selectId < 0 || selectId >= list_FileName.Count)
			return;
		if (animSetting == null)
			return;
		string fileName = list_FileName[selectId].text.text;

		if (fileName != "") {
			
			animSetting.Default ();
			AnimSet.SaveSetting (fileName, animSetting);
			Update_CurrAnimSetting ();

			//
			if (Set.setVal.Language == (int)en_Language.Chinese) {
				menuTips.Init ("恢复默认值成功", 3, true);
			} else {
				menuTips.Init ("Restore Default Success", 3, true);
			}
		}
	}
	public void OnClick_Default () {
		if (selectId < 0 || selectId >= list_FileName.Count)
			return;
		if (animSetting == null)
			return;
		string fileName = list_FileName[selectId].text.text;

		if (fileName != "") {
			//
			if (Set.setVal.Language == (int)en_Language.Chinese) {
				menuTips.Init ("是否恢复默认值", DefaultSet);
			} else {
				menuTips.Init ("Restore default setting?", DefaultSet);
			}
		}
	}
	public void OnClick_Back () {
		//gameObject.SetActive (false);
		menu.ChangeStatue (en_MenuStatue.MenuSta_SysSet);
		//menu.ChangeStatue (en_MenuStatue.MenuSta_GameSelect);
	}
}
