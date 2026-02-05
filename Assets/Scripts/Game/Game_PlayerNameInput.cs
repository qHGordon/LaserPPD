using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnClinkBack();

public class Game_PlayerNameInput : MonoBehaviour {
    public Button[] button_Char;
    public Button button_Delete;
    public Button button_OK;
    public Button button_Back;
    public Text[] text_NameOne;
    public Text text_Time;

    OnClinkBack onClinkBack;

    const string DEF_NAME = "AAA";
    float runTime;
    int runCnt;
    int maxNameLength;
    public string playerName;

    //public static Game_PlayerNameInput instance;

    void Awake() {
        //instance = this;
        for (int i = 0; i < button_Char.Length; i++) {
            GameButton selectOne = button_Char[i].GetComponent<GameButton> ();
            selectOne.Init((int)('A' + i), OnClick_Char);
        }
        button_Delete.onClick.AddListener(OnClick_Delete);
        button_OK.onClick.AddListener(OnClick_Ok);
        button_Back.onClick.AddListener(OnClick_Back);
    }

    void OnEnable() {
        GameStart();
    }
    public void GameStart(int nameLength, OnClinkBack backFun) {
        maxNameLength = Mathf.Clamp(nameLength, 1, 8);
        onClinkBack = backFun;
        if (onClinkBack == null) {
            button_Back.gameObject.SetActive(false);
        } else {
            button_Back.gameObject.SetActive(true);
        }
        
        runCnt = 30;
        runTime = runCnt;
        text_Time.text = runCnt.ToString("D2");
        playerName = "";
        Update_Name();

        // 根据个字重新排列坐标
        float advWidth = 150;
        float startx = 0;
        if(maxNameLength > 0) {
            startx = -(maxNameLength - 1) * advWidth * 0.5f;
        }
        for (int i = 0; i < text_NameOne.Length; i++) {
            if(i < maxNameLength) {
                text_NameOne[i].gameObject.SetActive(true);
                text_NameOne[i].transform.localPosition = new Vector3(startx + i * advWidth, text_NameOne[i].transform.localPosition.y);
            } else {
                text_NameOne[i].gameObject.SetActive(false);
            }
        }
    }
	// Use this for initialization
	public void GameStart () {
        GameStart(8, null);
    }
	
	// Update is called once per frame
	void Update () {
        if (runTime > 0) {
            runTime -= Time.deltaTime;
            if (runCnt != (int)runTime) {
                runCnt = (int)runTime;
                text_Time.text = runCnt.ToString("D2");
            }
        } else {
            runTime = 1;
            if (playerName == "") {
                playerName = DEF_NAME;
            }
            OnClick_Ok();
        }
    }

    void Update_Name() {
        for (int i = 0; i < text_NameOne.Length; i++) {
            if(i < playerName.Length) {
                text_NameOne[i].text = playerName[i].ToString();
            } else {
                text_NameOne[i].text = "";
            }
        }
    }
    public void OnClick_Char(int ch) {
        char c = (char)ch;
        //Debug.Log("Char: " + c.ToString());
        if (playerName.Length < text_NameOne.Length && playerName.Length < maxNameLength) {
            playerName += c;
        }
        Update_Name();
    }
    
    public void OnClick_Delete() {
        if (playerName.Length <= 0)
            return;
        playerName = playerName.Remove(playerName.Length - 1, 1);///
        Update_Name();
    }
    public void OnClick_Ok() {
        if (playerName == "") 
            return;
        gameObject.SetActive(false);
    }
    public void OnClick_Back() {
        if (onClinkBack != null) {
            onClinkBack();
        }
    }
    public string GetName() {
        if (playerName == "") {
            playerName = DEF_NAME;
        }
        return playerName;
    }
    
}
