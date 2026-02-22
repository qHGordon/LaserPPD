using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_SetOne : MonoBehaviour
{
    public Text text_Name;
    public Dropdown dropdown;
    public Button button_Dec;
    public Button button_Add;
    public Image img_levelName;
    public Text txt_level_index;
    Sprite[] Spr_levelName;

    int[] tab_Value;
    public bool changed;

    public int Id;
    OnClickCall onClickCall;

    void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        if (button_Dec != null)
        {
            button_Dec.onClick.AddListener(ValueDec);
        }
        if (button_Add != null)
        {
            button_Add.onClick.AddListener(ValueAdd);
        }
        dropdown.onValueChanged.AddListener(OnValueChanged);
    }
    void OnEnable()
    {
        changed = false;
    }
    public void OnClick()
    {
        if (onClickCall != null)
        {
            onClickCall(Id);
        }
        else
        {
            SetSelect(true);
        }
    }

    public void SetValueInit(string[] tabValue)
    {

        tab_Value = new int[tabValue.Length];

        dropdown.ClearOptions();
        Dropdown.OptionData optionData;

        for (int i = 0; i < tabValue.Length; i++)
        {
            tab_Value[i] = i;
            optionData = new Dropdown.OptionData();
            optionData.text = tabValue[i];
            dropdown.options.Add(optionData);
        }
    }
    public void SetValueInit(int[] tabValue)
    {

        tab_Value = tabValue;

        dropdown.ClearOptions();
        Dropdown.OptionData optionData;

        for (int i = 0; i < tab_Value.Length; i++)
        {
            optionData = new Dropdown.OptionData();
            optionData.text = tab_Value[i].ToString();
            dropdown.options.Add(optionData);
        }
    }
    public void Init(int id, OnClickCall call)
    {
        Id = id;
        onClickCall = call;
    }
    //public void Init (int maxIndex) {
    //	int[] tabValue = new int[maxIndex];
    //	SetValueInit (tabValue);
    //}

    public void SetName(string str)
    {
        text_Name.text = str;
    }

    public void SetValueName(int value, string str)
    {
        if (tab_Value == null)
            return;
        int index = GetArryIndex(tab_Value, value);
        if (index < 0)
            return;
        dropdown.options[index].text = str;

    }
    //public void SetValueNameByIndex (int index, string str) {
    //	if (index >= 0 && index < dropdown.options.Count) {
    //		dropdown.options[index].text = str;
    //	}
    //}
    public void UpdateValue(int value)
    {
        if (tab_Value == null)
        {
            //Debug.LogError(1); 
            return; 
        }

        int index = GetArryIndex(tab_Value, value);
        if (index < 0)
        { 
            //Debug.LogError(1); 
            return; 
        }
        dropdown.value = index;
        dropdown.captionText.text = dropdown.options[index].text;
    }
    public void UpdateValue(string value)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == value)
            {
                dropdown.value = i;
                //dropdown.Show ();
                dropdown.captionText.text = value;

                return;
            }
        }
        //dropdown.value = 1000;
        dropdown.captionText.text = "";
        //Debug.Log ("SetValue: " + dropdown.value + ", " + value);
    }

    public void SetSelect(bool selectFlag)
    {
        button_Add.gameObject.SetActive(selectFlag);
        button_Dec.gameObject.SetActive(selectFlag);
    }

    public void ValueAdd()
    {
        if (tab_Value == null)
            return;
        dropdown.value++;
        if (dropdown.value >= tab_Value.Length)
        {
            dropdown.value = 0;
        }
    }

    public void ValueDec()
    {
        if (tab_Value == null)
            return;
        dropdown.value--;
        if (dropdown.value < 0)
        {
            dropdown.value = tab_Value.Length - 1;
        }
    }

    public int GetValue()
    {
        if (tab_Value == null)
            return 0;
        if (dropdown.value >= 0 && dropdown.value < tab_Value.Length)
        {
            return tab_Value[dropdown.value];
        }
        return 0;
    }
    public int GetValueIndex()
    {
        return dropdown.value;
    }
    public string GetValueName()
    {
        return dropdown.captionText.text;
    }
    public string GetValueName(int index)
    {
        if (index >= dropdown.options.Count)
        {
            return "";
        }
        return dropdown.options[index].text;
    }


    int GetArryIndex(int[] tabValue, int value)
    {
        for (int i = 0; i < tabValue.Length; i++)
        {
            if (tabValue[i] == value)
            {
                return i;
            }
        }
        return -1;
    }

    void OnValueChanged(int value)
    {
        changed = true;
    }
    public bool IsChanged()
    {
        if (changed)
        {
            changed = false;
            return true;
        }
        return false;
    }
}
