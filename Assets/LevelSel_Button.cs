using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSel_Button : MonoBehaviour {
    public int Id = 0;
    Game97_LevelSel gamemain;
    Button button;
    public Sprite[] beSelect;
    public void Init(int _ID, Game97_LevelSel _gamemain) {
        gamemain = _gamemain;
        Id = _ID;
        button = GetComponent<Button>();
        button.onClick.AddListener(GiveID);
    }
    // Use this for initialization
    void GiveID() {
        gamemain.Map_Index = Id;
        Main.MapID = Id;

        //for (int i = 0; i < gamemain.buttons.Length; i++) {
        //    // gamemain.buttons[i].GetComponent<Image>().sprite = gamemain.buttons[i].GetComponent< LevelSel_Button > (). beSelect[0];
        //}
        //  GetComponent<Image>().sprite = beSelect[1];
        //  GetComponent<Image>().SetNativeSize();
  
        Game97_Main.instance.EnterGame(0);
    }

    // Update is called once per frame
    void Update() {

    }
}
