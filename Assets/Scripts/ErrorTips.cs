using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public enum en_ErrorCode
{
	None = 0,
	IoAppError,
	IoDisConnect,	
	GameSetting,
	AnimSetting,
	PicSetting,
	LevelSetting,
}

public class ErrorTips : MonoBehaviour {
	public GameObject tips_Obj;
	public Image image_Tips;
	Sprite[] sprite_Tips;

	public en_ErrorCode errorCode;
	en_ErrorCode temp;

	int language = -1;
	string[] tab_Language = { "CN", "EN" };
	void ChangeLanguage () {
		if (language != Set.setVal.Language || sprite_Tips == null) {
			language = Set.setVal.Language;
			sprite_Tips = Resources.LoadAll<Sprite> ("UI/Error_" + tab_Language[language]);
		}
	}

	public static ErrorTips instance;
	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void OnEnable () {
		ChangeLanguage ();
		Update_ErrorTips (en_ErrorCode.None);
	}
	
	// Update is called once per frame
	void Update () {
		temp = GetErrorCode ();
		if (temp != errorCode) {
			Update_ErrorTips (temp);
		}
	}

	en_ErrorCode GetErrorCode () {
		if (Main.settingError != en_ErrorCode.None)
			return Main.settingError;
#if UNITY_EDITOR	 
    //return en_ErrorCode.None;
#endif
        if (CmdIO_YDGZ.connectStatue == false) {
			if (CmdIOUpdate.connectStatue) {
				if (CmdIOUpdate.errorCode != 0)
					return en_ErrorCode.IoAppError;
				return en_ErrorCode.None;
			}
			return en_ErrorCode.IoDisConnect;
		}
		return en_ErrorCode.None;
	}

	void Update_ErrorTips (en_ErrorCode errorcode) {
		errorCode = errorcode;
		switch (errorCode) {
        case en_ErrorCode.None:
			tips_Obj.SetActive (false);
			return;
		case en_ErrorCode.IoDisConnect:
			image_Tips.sprite = sprite_Tips[0];
			break;
		case en_ErrorCode.IoAppError:
			image_Tips.sprite = sprite_Tips[1];
			break;
		case en_ErrorCode.GameSetting:
			image_Tips.sprite = sprite_Tips[2];
			break;
		case en_ErrorCode.AnimSetting:
			image_Tips.sprite = sprite_Tips[3];
			break;
        }
		tips_Obj.SetActive (true);
		image_Tips.SetNativeSize ();
	}
}
