using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;


public class Game97_GameSelOne : MonoBehaviour
{
    public Image image_Pic;
    public Image image_GameName;
    public Image image_Pass;
    public Image isSelectImage;
    public Image select;
    public int scenceIndex;
    public int arrayIndex;
    private void Awake()
    {

    }
    public bool IsSelect
    {
        set
        {
            isSelectImage.gameObject.SetActive(value);
        }
    }
    private void OnEnable()
    {
        IsSelect = false;
        if (Main.COMPANY_NUM != 9&& Main.COMPANY_NUM != 8 && Main.COMPANY_NUM != 10)
        {
            select.gameObject.SetActive(false);
        }
        else
        {
            select.gameObject.SetActive(true);
            image_GameName.transform.localPosition = new Vector3(0, 103, 0);
            image_GameName.transform.localPosition = new Vector3(-2, -79, 0);
            transform.localScale = new Vector3(1, 0.8f, 1);
        }
    }
    public void Update_Statue(bool ispassed)
    {
        if (ispassed)
        {
            image_Pic.color = new Color(0.4f, 0.4f, 0.4f, 1);
            image_Pass.gameObject.SetActive(true);
        }
        else
        {
            image_Pic.color = new Color(1f, 1f, 1f, 1);
            image_Pass.gameObject.SetActive(false);
        }
    }
    public void Update_GamePic(Sprite sprite)
    {
        image_Pic.sprite = sprite;
    }

    public void Update_GameName(Sprite sprite)
    {
        image_GameName.sprite = sprite;
        image_GameName.SetNativeSize();
    }
}
