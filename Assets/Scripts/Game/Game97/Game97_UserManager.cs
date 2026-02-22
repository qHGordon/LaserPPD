using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game97_UserManager : MonoBehaviour
{
    public InputField inputField_CardId;
    public InputField inputField_UserName;
    public InputField inputField_PhoneNumber;
    public Text text_PlayCnt;
    public Text text_MaxScore;
    public Button button_Query;
    public Button button_Create;
    public Button button_Delete;
    public Button button_Change;
    public Button button_Back;
    public Button button_Tips;
    public Text text_Tips;

    UserOne userData;
    float showTipsTime;

    Game97_Main game97_Main;
    public void Awake0(Game97_Main game97)
    {
        game97_Main = game97;

        button_Query.onClick.AddListener(OnClick_Query);
        button_Create.onClick.AddListener(OnClick_Create);
        button_Delete.onClick.AddListener(OnClick_Delete);
        button_Change.onClick.AddListener(OnClick_Change);
        button_Back.onClick.AddListener(OnClick_Back);
        button_Tips.onClick.AddListener(OnClick_Tips);

    }
    // Use this for initialization
    public void GameStart()
    {
        button_Tips.gameObject.SetActive(false);
        userData = null;
        inputField_CardId.enabled = false;
        ClearUserInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (showTipsTime > 0)
        {
            showTipsTime -= Time.deltaTime;
            if (showTipsTime <= 0)
            {
                button_Tips.gameObject.SetActive(false);
            }
        }
        if (CardInputCheck.cardInputed > 0)
        {
            CardInputCheck.cardInputed = 0;
            if (inputField_CardId.isFocused == false)
            {
                inputField_CardId.text = CardInputCheck.inCardId.ToString("D10");
            }
            OnClick_Query();
        }
    }

    void ClearUserInfo()
    {
        inputField_CardId.text = "";
        inputField_UserName.text = "";
        inputField_PhoneNumber.text = "";
        text_PlayCnt.text = "";
        text_MaxScore.text = "";
    }
    void Update_UserData()
    {
        if (userData == null)
        {
            if(Set.setVal.Language == (int)en_Language.English)
            {
                inputField_UserName.text = "无信息";
            }
            else
            {
                inputField_UserName.text = "No message";
            }
            inputField_PhoneNumber.text = "";
            text_PlayCnt.text = "";
            text_MaxScore.text = "";
        }
        else
        {
            inputField_UserName.text = userData.userName;
            inputField_PhoneNumber.text = userData.phoneNumber;
            text_PlayCnt.text = userData.playCnt.ToString();
            text_MaxScore.text = userData.maxScore.ToString();
        }
    }

    public void OnClick_Query()
    {
        int cardId;
        if (int.TryParse(inputField_CardId.text, out cardId) == false)
        {
            return;
        }
        userData = UserManager.LoadUserData((uint)cardId);
        Update_UserData();
    }
    public void OnClick_Create()
    {
        int cardId;
        if (int.TryParse(inputField_CardId.text, out cardId) == false)
        {
            return;
        }
        userData = UserManager.LoadUserData((uint)cardId);
        if (userData != null)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                ShowTips("用户已存在，注册失败");
            }
            else
            {
                ShowTips("User already exists,\r\nregistration failed");
            }
            return;
        }
        // 新的
        userData = new UserOne();
        userData.userName = inputField_UserName.text;
        userData.phoneNumber = inputField_PhoneNumber.text;
        userData.playCnt = 0;
        userData.maxScore = 0;
        //
        UserManager.SaveData((uint)cardId, userData);
        Update_UserData();
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            ShowTips("注册成功");
        }
        else
        {
            ShowTips("Registration successful");
        }
    }
    public void OnClick_Delete()
    {
        int cardId;
        if (int.TryParse(inputField_CardId.text, out cardId) == false)
        {
            return;
        }
        userData = UserManager.LoadUserData((uint)cardId);
        if (userData == null)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                ShowTips("用户不存在，删除失败");
            }
            else
            {
                ShowTips("User does not exist, Deleted failed");
            }
            return;
        }
        UserManager.DeleteUser((uint)cardId);
        userData = null;
        Update_UserData();
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            ShowTips("删除成功");
        }
        else
        {
            ShowTips("Deleted successful");
        }
    }
    public void OnClick_Change()
    {
        int cardId;
        if (int.TryParse(inputField_CardId.text, out cardId) == false)
        {
            return;
        }
        userData = UserManager.LoadUserData((uint)cardId);
        if (userData == null)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                ShowTips("用户不存在，保存失败");
            }
            else
            {
                ShowTips("User does not exist, save failed");
            }
            return;
        }
        userData.userName = inputField_UserName.text;
        userData.phoneNumber = inputField_PhoneNumber.text;
        UserManager.SaveData((uint)cardId, userData);
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            ShowTips("保存成功");
        }
        else
        {
            ShowTips("Saved successful");
        }
    }
    public void OnClick_Back()
    {
        game97_Main.ChangeStatue(en_Game97_Sta.Idle);
    }

    public void OnClick_Tips()
    {
        button_Tips.gameObject.SetActive(false);
    }
    void ShowTips(string str)
    {
        button_Tips.gameObject.SetActive(true);
        text_Tips.text = str;
        showTipsTime = 4;
    }
}
