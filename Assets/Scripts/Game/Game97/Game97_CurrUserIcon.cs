using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Game97_CurrUserIcon : MonoBehaviour
{
    public Text text_CardId;
    public Text text_UserName;

    float tipsTime = 0;
    public void Update_UserData(UserOne userData)
    {
        if (userData == null)
        {
            text_CardId.text = "";
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                text_UserName.text = "请刷手环";
            }
            else
            {
                text_UserName.text = "Input Bracelet";
            }
        }
        else
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                text_CardId.text = "卡号：" + userData.cardId.ToString("D10");
            }
            else
            {
                text_CardId.text = "CardNo: " + userData.cardId.ToString("D10");
            }
            text_UserName.text = userData.userName;
        }
        tipsTime = 0;
    }
    public void ShowTips(string str)
    {
        tipsTime = 2f;
        text_CardId.text = "";
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            text_CardId.text = "卡号：" + CardInputCheck.inCardId.ToString("D10");
        }
        else
        {
            text_CardId.text = "CardNo: " + CardInputCheck.inCardId.ToString("D10");
        }
        text_UserName.text = str;
    }

    void Update()
    {
        if (tipsTime > 0)
        {
            tipsTime -= Time.deltaTime;
            if (tipsTime <= 0)
            {
                Update_UserData(Main.currUser);
            }
        }
    }
}
