using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetValue : MonoBehaviour
{
    public Image image_BackG;
    public Image image_SelectFlag;
    public Text text_Name;
    public Image image_ValueBackG;
    public Text text_SetValue;
    public Sprite sprite_Normal;
    public Sprite sprite_Selected;
    //
    bool isSelect;
    float runTime;

    // Use this for initialization
    public void SetSelect(bool select)
    {
        isSelect = select;
        image_SelectFlag.gameObject.SetActive(select);
        if (isSelect)
        {
            image_BackG.sprite = sprite_Selected;
        }
        else
        {
            image_BackG.sprite = sprite_Normal;
        }
        runTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelect)
        {
            runTime += Time.deltaTime;
            if (runTime >= 0.5f)
            {
                runTime = 0;
                if (image_SelectFlag.gameObject.activeSelf)
                {
                    image_SelectFlag.gameObject.SetActive(false);
                }
                else
                {
                    image_SelectFlag.gameObject.SetActive(true);
                }
            }
        }
    }

    //初始化值 --------------------------------------------------------------------------------------
    int[] tab_Value;
    float[] tab_ValueF;
    string[] setVal_String = null;
    string[] str_DescriptOne;
    string str_DescriptCommon = "";
    public int setVal_Index;

    public void SetValueInit(int[] val)
    {
        setVal_Index = 0;
        if (setVal_String == null)
        {
            setVal_String = new string[val.Length];
            for (int i = 0; i < val.Length; i++)
            {
                setVal_String[i] = val[i].ToString();
            }
        }
        if (tab_Value == null)
        {
            tab_Value = new int[val.Length];
            for (int i = 0; i < val.Length; i++)
            {
                tab_Value[i] = val[i];
            }
        }
        if (str_DescriptOne == null)
        {
            str_DescriptOne = new string[val.Length];
            for (int i = 0; i < str_DescriptOne.Length; i++)
            {
                str_DescriptOne[i] = "";
            }
        }
    }

    public void SetValueInit(float[] val)
    {
        setVal_Index = 0;
        if (setVal_String == null)
        {
            setVal_String = new string[val.Length];
            for (int i = 0; i < val.Length; i++)
            {
                setVal_String[i] = val[i].ToString();
            }
        }
        if (tab_ValueF == null)
        {
            tab_ValueF = new float[val.Length];
            for (int i = 0; i < val.Length; i++)
            {
                tab_ValueF[i] = val[i];
            }
        }
    }

    //名字
    public void SetSelectName(string name)
    {
        text_Name.text = name;
    }
    //选项名字
    public void SetValueName(int value, string name)
    {
        int index = IndexOfArry(tab_Value, value);
        if (index >= 0)
        {
            setVal_String[index] = name;
        }
    }
    public void SetValueName(float value, string name)
    {
        int index = IndexOfArry(tab_ValueF, value);
        if (index >= 0)
        {
            setVal_String[index] = name;
        }
    }
    public string GetValueName(int value)
    {
        int index = IndexOfArry(tab_Value, value);
        if (index >= 0)
        {
            return setVal_String[index];
        }
        return "";
    }
    public string GetValueName(float value)
    {
        int index = IndexOfArry(tab_ValueF, value);
        if (index >= 0)
        {
            return setVal_String[index];
        }
        return "";
    }
    //更新选项值
    public void UpdateValue(int value)
    {
        int index = IndexOfArry(tab_Value, value);
        if (index >= 0)
        {
            setVal_Index = index;
            text_SetValue.text = setVal_String[index];
        }
    }
    public void UpdateValue(float value)
    {
        int index = IndexOfArry(tab_ValueF, value);
        if (index >= 0)
        {
            setVal_Index = index;
            text_SetValue.text = setVal_String[index];
        }
    }

    //++
    public void ValueAdd()
    {
        if (setVal_String != null)
        {
            if (++setVal_Index >= setVal_String.Length)
            {
                setVal_Index = 0;
            }
            text_SetValue.text = setVal_String[setVal_Index];
        }
    }
    //--
    public void ValueDec()
    {
        if (setVal_String != null)
        {
            if (--setVal_Index < 0)
            {
                setVal_Index = setVal_String.Length - 1;
            }
            text_SetValue.text = setVal_String[setVal_Index];
        }
    }
    //返回值
    public int GetValue()
    {
        return tab_Value[setVal_Index];
    }
    public float GetValueF()
    {
        return tab_ValueF[setVal_Index];
    }

    // 描述：
    public void SetDescript(string str)
    {
        str_DescriptCommon = str;
    }
    public void SetDescript(int value, string str)
    {
        int index = IndexOfArry(tab_Value, value);
        if (index >= 0)
        {
            str_DescriptOne[index] = str;
        }
    }
    public string GetDescript(int value)
    {
        int index = IndexOfArry(tab_Value, value);
        if (index < 0)
            return "";
        if (str_DescriptOne[index] != "")
        {
            return str_DescriptOne[index];
        }
        string str = str_DescriptCommon;
        if (str.Contains("%s"))
        {
            str = str.Replace("%s", setVal_String[index]);
        }
        return str;
    }

    //
    int IndexOfArry(int[] arry, int value)
    {
        for (int i = 0; i < arry.Length; i++)
        {
            if (value == arry[i])
            {
                return i;
            }
        }
        return -1;
    }
    int IndexOfArry(float[] arry, float value)
    {
        for (int i = 0; i < arry.Length; i++)
        {
            if (value == arry[i])
            {
                return i;
            }
        }
        return -1;
    }
}
