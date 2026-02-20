using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class GameSetOne : MonoBehaviour {
	public Text text_Name;
	public Text text_Value;
	public Button button_Dec;
	public Button button_Add;

	int[] tab_Value;
	float [] tab_Value_float;
	string[] tab_ValueName;
	int valueIndex;	// index

	void Awake () {
		button_Dec.onClick.AddListener (OnClick_Dec);
		button_Add.onClick.AddListener (OnClick_Add);
	}

	int IndexOfArry (int[] arry, int value) {
		for (int i = 0; i < arry.Length; i++) {
			if (value == arry[i]) {
				return i;
			}
		}
		return -1;
	}
    int IndexOfArry (float[] arry, float value) {
		for (int i = 0; i < arry.Length; i++) {

			if (value == arry[i]) {
				return i;
			}
		}
		return -1;
	}

	public void SetInit (int[] tabValue) {
		tab_Value = tabValue;
		tab_ValueName = new string[tabValue.Length];
        for (int i = 0; i < tab_ValueName.Length; i++) {
			tab_ValueName[i] = tab_Value[i].ToString ();
		}
	}
    public void SetInit(float[] tabValue) {
        tab_Value_float = tabValue;
		tab_ValueName = new string[tabValue.Length];
        for (int i = 0; i < tab_ValueName.Length; i++) {
			tab_ValueName[i] = tab_Value_float[i].ToString ();
		}
	}
	public void SetName (string strName) {
		text_Name.text = strName;
	}
	public void SetValueName (int value, string valueName) {
        if (tab_Value == null)
            return;
        int index = IndexOfArry (tab_Value, value);
		if (index >= 0 && index < tab_ValueName.Length) {
			tab_ValueName[index] = valueName;
		}
	}

	public void UpdateValue (int value) {
        if (tab_Value == null)
        {
          
            text_Value.text = "";
            return;
        }
       
        int index = IndexOfArry (tab_Value, value);
      
        if (index >= 0 && index < tab_ValueName.Length) {
			text_Value.text = tab_ValueName[index];
			valueIndex = index;
		} else {

            text_Value.text = "";
		}
	}
    public void UpdateValue (float value) {
        if (tab_Value_float == null)
        {
          
            text_Value.text = "";
            return;
        }
       
        int index = IndexOfArry (tab_Value_float, value);

#if UNITY_EDITOR
//        Debug.LogError(value);


#endif
        if (index >= 0 && index < tab_ValueName.Length) {
			text_Value.text = tab_ValueName[index];
			valueIndex = index;
		} else {



            text_Value.text = "";
		}
	}

	public void OnClick_Dec () {
		if (tab_ValueName != null) {
			if (--valueIndex < 0) {
				valueIndex = tab_ValueName.Length - 1;
			}
			text_Value.text = tab_ValueName[valueIndex];
		}
	}

	public void OnClick_Add () {
		if (tab_ValueName != null) {
			if (++valueIndex >= tab_ValueName.Length) {
				valueIndex = 0;
			}
			text_Value.text = tab_ValueName[valueIndex];
		}
	}
	public int GetValue () {		
		return tab_Value[valueIndex];
	}
    public float GetValue_float () {		
		return tab_Value_float[valueIndex];
	}
}
