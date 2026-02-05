using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Menu_GameLevelSetOne : MonoBehaviour {
	public Image image_Pic;
	public Text text_Title;
	public Button button_Default;
	public Menu_SetOne[] setOne;

	const int SETID_gameTime = 0;
	const int SETID_life = 1;
	const int SETID_wallLedNum = 2;
	const int SETID_targetPoint = 3;
	const int SETID_targetDieType = 4;
	const int SETID_bkWidth = 5;
	const int SETID_presetPicName = 6;
    const int SETID_animInfoName = 7;
    
    OnClickCall onClickCall;
    GameLevelSetting gameLevelSetting;
	string[] picNames;
    string[] animNames;
    public int Id = 0;
	int playerMode;
	Color colorOld;
	bool firstShow;


	public void Awake0 (int no, OnClickCall call) {
        Id = no;
        onClickCall = call;
        

		colorOld = image_Pic.color;
		gameObject.GetComponent<Button>().onClick.AddListener(OnClick);
		button_Default.onClick.AddListener (OnClick_Default);

        for (int i = 0; i < setOne.Length; i++) {
			setOne[i].Init(i, OnClick_SelectId);
        }

		setOne[SETID_gameTime].SetValueInit (GameLevelSetting.tab_gameTime);
		setOne[SETID_life].SetValueInit (GameLevelSetting.tab_life);
		setOne[SETID_wallLedNum].SetValueInit (GameLevelSetting.tab_wallLedNum);
		setOne[SETID_targetPoint].SetValueInit (GameLevelSetting.tab_targetPoint);
		setOne[SETID_targetDieType].SetValueInit (GameLevelSetting.tab_targetDieType);
		setOne[SETID_bkWidth].SetValueInit (GameLevelSetting.tab_bkWidth);
		//setOne[SETID_animCount].SetValueInit (GameLevelSetting.tab_animCount);
	}
	// Use this for initialization
	public void GameStart (int playermode, GameLevelSetting settine, string[] picname, string[] animname) {
		playerMode = playermode;
		gameLevelSetting = settine;
        picNames = picname;
		animNames = animname;

		if (Set.setVal.Language == (int)en_Language.Chinese) {
			text_Title.text = "第 " + (Id + 1) + " 关";
		} else {
			text_Title.text = "Level " + (Id + 1);
		}
		
		Update_Languae ();
		Update_SetValueInit();
        Update_SetValue ();
		Update_SelectId(-1);
		firstShow = false;
	}

	void Update () {
		if (firstShow == false) {
			firstShow = true;
			//第一次更新设置，无对应选项时，会默认跳到第一个，重新刷新后，显示空
			Update_SetValue ();
		}
	}
	public void SetSelectSta (bool selected) {
		if (selected) {
			image_Pic.color = Color.yellow;
		} else {
			image_Pic.color = colorOld;
            Update_SelectId(-1);
        }
	}
	// 更新选中ID
	void Update_SelectId(int id) {
		for (int i = 0; i < setOne.Length; i++) {
			if (i == id) {
				setOne[i].SetSelect(true);
			} else {
                setOne[i].SetSelect(false);
            }
		}
	}

	void Update_Languae () {
		if (Set.setVal.Language == (int)en_Language.Chinese) {
			button_Default.GetComponentInChildren<Text> ().text = "默认值";
			setOne[SETID_gameTime].SetName ("游戏时间");
			setOne[SETID_life].SetName ("生命数");
			setOne[SETID_wallLedNum].SetName ("墙灯个数");
			setOne[SETID_targetPoint].SetName ("目标数");
			setOne[SETID_targetDieType].SetName ("消除方式");
			setOne[SETID_bkWidth].SetName ("边线宽度");
			//setOne[SETID_animCount].SetName ("花样个数");
			setOne[SETID_presetPicName].SetName ("图案");
			setOne[SETID_animInfoName].SetName ("花样");
			//
			setOne[SETID_targetDieType].SetValueName ((int)en_TargetDieType.ToNone, "变黑");
			setOne[SETID_targetDieType].SetValueName ((int)en_TargetDieType.ToDie, "变红");
		} else {
			//
			button_Default.GetComponentInChildren<Text> ().text = "Default";
			setOne[SETID_gameTime].SetName ("Game time");
			setOne[SETID_life].SetName ("Life");
			setOne[SETID_wallLedNum].SetName ("Wall light count");
			setOne[SETID_targetPoint].SetName ("Target Count");
			setOne[SETID_targetDieType].SetName ("Disappear mode");
			setOne[SETID_bkWidth].SetName ("Edge Width");
			//setOne[SETID_animCount].SetName ("花样个数");
			setOne[SETID_presetPicName].SetName ("Graphics");
			setOne[SETID_animInfoName].SetName ("Animation");
			//
			setOne[SETID_targetDieType].SetValueName ((int)en_TargetDieType.ToNone, "Turn black");
			setOne[SETID_targetDieType].SetValueName ((int)en_TargetDieType.ToDie, "Turn red");
		}
	}

  	void Update_SetValueInit() {
        setOne[SETID_presetPicName].SetValueInit(picNames);
        setOne[SETID_animInfoName].SetValueInit(animNames);
    }
    void Update_SetValue () {
		setOne[SETID_gameTime].UpdateValue (gameLevelSetting.gameTime);
		setOne[SETID_life].UpdateValue (gameLevelSetting.life);
		setOne[SETID_wallLedNum].UpdateValue (gameLevelSetting.wallLedNum);
		setOne[SETID_targetPoint].UpdateValue (gameLevelSetting.targetPoint);
		setOne[SETID_targetDieType].UpdateValue (gameLevelSetting.targetDieType);
		setOne[SETID_bkWidth].UpdateValue (gameLevelSetting.bkWidth);
        //setOne[SETID_animCount].UpdateValue (gameLevelSetting.animCount);
        //
        setOne[SETID_presetPicName].UpdateValue(gameLevelSetting.presetPicName);
        setOne[SETID_animInfoName].UpdateValue(gameLevelSetting.animInfoName);
    }

	public void GetSetting (GameLevelSetting gameLevelSettine) {
        gameLevelSettine.gameTime =  setOne[SETID_gameTime].GetValue ();
		gameLevelSettine.life = setOne[SETID_life].GetValue ();
		gameLevelSettine.wallLedNum = setOne[SETID_wallLedNum].GetValue ();
		gameLevelSettine.targetPoint = setOne[SETID_targetPoint].GetValue ();
		gameLevelSettine.targetDieType = setOne[SETID_targetDieType].GetValue ();
		gameLevelSettine.bkWidth = setOne[SETID_bkWidth].GetValue ();
		//gameLevelSettine.animCount = setOne[SETID_animCount].GetValue ();

		gameLevelSettine.presetPicName = setOne[SETID_presetPicName].GetValueName ();
        //gameLevelSettine.presetPicName = "D-" + (Id + 1).ToString("D3");

        gameLevelSettine.animInfoName = setOne[SETID_animInfoName].GetValueName ();
	}

	public bool Equals (GameLevelSetting gameLevelSettine) {
		if (gameLevelSettine == null)
			return false;
		if(gameLevelSettine.gameTime != setOne[SETID_gameTime].GetValue ())
			return false;
		if (gameLevelSettine.life != setOne[SETID_life].GetValue ())
			return false;
		if (gameLevelSettine.wallLedNum != setOne[SETID_wallLedNum].GetValue ())
			return false;
		if (gameLevelSettine.targetPoint != setOne[SETID_targetPoint].GetValue ())
			return false;
		if (gameLevelSettine.targetDieType != setOne[SETID_targetDieType].GetValue ())
			return false;
		if (gameLevelSettine.bkWidth != setOne[SETID_bkWidth].GetValue ())
			return false;
		//gameLevelSettine.animCount = setOne[SETID_animCount].GetValue ();

		if (gameLevelSettine.presetPicName != setOne[SETID_presetPicName].GetValueName ())
			return false;
		if (gameLevelSettine.animInfoName != setOne[SETID_animInfoName].GetValueName ())
			return false;

		return true;
	}

	public int GetCurrPicId () {
        return setOne[SETID_presetPicName].GetValueIndex();
    }
    public string GetCurrPicName() {
        return setOne[SETID_presetPicName].GetValueName();
    }
    public int GetCurrAnimId() {
		return setOne[SETID_animInfoName].GetValueIndex();
	}
    public string GetCurrAnimName() {
        return setOne[SETID_animInfoName].GetValueName();
    }
 //   void Default(bool isOk) {
	//	if (isOk) {
 //           if (Set.setVal.Language == (int)en_Language.Chinese) {
 //               Menu_Tips.instance.Init("恢复默认设置成功", 3, true);
 //           } else {
 //               Menu_Tips.instance.Init("Default success", 3, true);
 //           }
 //       }
	//}
	public void OnClick_Default () {
		//if (Set.setVal.Language == (int)en_Language.Chinese) {
		//          Menu_Tips.instance.Init("是否恢复默认设置", Default);
		//      } else {
		//          Menu_Tips.instance.Init("Default setting", Default);
		//      }
		gameLevelSetting.Default (Set.setVal.GameChoose, playerMode, Id);
		Update_SetValue ();

	}

	public void OnClick_SelectId(int id) {
        Update_SelectId(id);
		OnClick();
    }
	public void OnClick() {
		if (onClickCall != null) {
			onClickCall(Id);
        }
    }
}
