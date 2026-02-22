using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeCotroy : MonoBehaviour {
    public Image image_Value;
    public Text text_Value;

    float runTime;
    float leftTime;
    float rightTime;
	// Use this for initialization
	void Start () {
        runTime = 0;
        leftTime = 0;
        rightTime = 0;
        UpdateVolume();
    }
	
	// Update is called once per frame
	void Update () {
        runTime += Time.deltaTime;
        if(runTime >= 3 || Main.statue == en_MainStatue.Menu) {
            Destroy(gameObject);
            return;
        }
        if (Key.MENU_Statue_Left()) {
            if (leftTime == 0 || leftTime >= 0.5f) {
                if (Set.setVal.SysVolume > Set.SET_c_MinSysVolume) {
                    Set.setVal.SysVolume--;
                    Set.SaveSysVolume();
                    UpdateVolume();
                }
                if (leftTime >= 0.5f) {
                    leftTime = 0.49f;
                }
            }
            leftTime += Time.deltaTime;
            runTime = 0;
        } else {
            leftTime = 0;
        }
        if (Key.MENU_Statue_Right()) {
            if (rightTime == 0 || rightTime >= 0.5f) {
                if (Set.setVal.SysVolume < Set.SET_c_MaxSysVolume) {
                    Set.setVal.SysVolume++;
                    Set.SaveSysVolume();
                    UpdateVolume();
                }
                if (rightTime >= 0.5f) {
                    rightTime = 0.49f;
                }
            }
            rightTime += Time.deltaTime;
            runTime = 0;
        } else {
            rightTime = 0;
        }
    }

    void UpdateVolume() {
		
		float  volume = (float)Set.setVal.SysVolume / 100;
        image_Value.fillAmount = volume;
        text_Value.text = Set.setVal.SysVolume.ToString();
        AudioListener.volume = volume;

    }
}
