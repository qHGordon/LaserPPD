using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class GameButton : MonoBehaviour
{
    public Image image_Pic;
    public Sprite sprite_Normal;
    public Sprite sprite_Select;
    public Text text_Value;
    public Font font_Normal;
    public Font font_Select;
    OnClickCall onClickCall;
    public float clickTime = 0;
    float showSelectTime = 0;

    public int Id;

    public void Init(int id, OnClickCall call)
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        Id = id;
        onClickCall = call;
    }
    public void Init(int id, OnClickCall call, int value)
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
        Id = id;
        onClickCall = call;
        if (text_Value != null)
        {
            text_Value.text = value.ToString();
        }
    }

    public void Update()
    {
        if (showSelectTime > 0)
        {
            showSelectTime -= Time.deltaTime;
            if (showSelectTime <= 0)
            {
                image_Pic.sprite = sprite_Normal;
            }
        }
    }

    public void SetSelectFlag(bool selected)
    {
        if (image_Pic != null)
        {
            image_Pic.sprite = selected ? sprite_Select : sprite_Normal;
        }
        if (text_Value != null)
        {
            text_Value.font = selected ? font_Select : font_Normal;
        }
    }

    public void OnClick()
    {
        if (clickTime > 0 && image_Pic != null)
        {
            showSelectTime = clickTime;
            image_Pic.sprite = sprite_Select;
            image_Pic.SetNativeSize();
        }

        if (onClickCall != null)
        {
            onClickCall(Id);
        }
    }
}
