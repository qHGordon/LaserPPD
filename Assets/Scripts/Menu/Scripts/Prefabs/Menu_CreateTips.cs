using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void CreateFileCall (string fileName);

public class Menu_CreateTips : MonoBehaviour {
	public Text text_Tips;
	public InputField inputField_FileName;
	public Button button_Yes;
	public Button button_No;

	CreateFileCall createFileCall;

	Menu_Tips menuTips;
	public void Init (Menu_Tips tips) {
		menuTips = tips;
		//
		button_Yes.onClick.AddListener (OnClick_Yes);
		button_No.onClick.AddListener (OnClick_No);
	}

    void OnDisable() {
        createFileCall = null;
    }

    // Use this for initialization
    public void GameStart (CreateFileCall call) {
        gameObject.SetActive(true);
        createFileCall = call;
		inputField_FileName.text = "";

		Update_Lauguage();
    }

	void Update_Lauguage() {
		if (Set.setVal.Language == (int)en_Language.Chinese) {
            button_Yes.GetComponentInChildren<Text>().text = "确定";
            button_No.GetComponentInChildren<Text>().text = "取消";
        } else {
            button_Yes.GetComponentInChildren<Text>().text = "OK";
            button_No.GetComponentInChildren<Text>().text = "Cancel";
        }
    }

	public void OnClick_Yes () {
		if (inputField_FileName.text == "")
			return;
		if (createFileCall != null) {
			createFileCall (inputField_FileName.text);
		}
		gameObject.SetActive (false);
	}
	public void OnClick_No () {
		gameObject.SetActive (false);
	}
}
