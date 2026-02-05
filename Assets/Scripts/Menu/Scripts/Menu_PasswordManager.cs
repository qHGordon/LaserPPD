using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum en_PasswordType
{
    Menu = 0,   // 进设置密码
    Acc,        // 查账密码
}
public enum en_PasswordOptionType
{
    Check = 0,  // 验证
    Modify,     // 修改
}

enum en_PasswordOptionSta
{
    Check = 0,  // 验证
    NewPsw,     // 新密码
    AgainPsw,   // 
}

public class Menu_PasswordManager : MonoBehaviour
{
    public Text text_Title;
    public Text text_Tips;
    public Text text_WxTips;
    public AccValue accValue_Psw;
    public Menu_Button[] button;
    //
    const int BUTTON_ID_DEL = 10;
    const int BUTTON_ID_OK = 11;
    const int BUTTON_ID_BACK = 12;
    const int MAX_SEL = 13;
    //const int PASSWORD_LEN = 6;
    readonly int[] tab_Key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, BUTTON_ID_DEL, 0, BUTTON_ID_OK, BUTTON_ID_BACK };
    //
    public en_PasswordType passwordType;
    public en_PasswordOptionType optionType;
    en_PasswordOptionSta statue;
    public bool waitCheck;
    public bool checkResult;
    bool pressedButton;
    int postIndex;
    int numLen;
    int passwordLength;
    int targetPassword;
    int defaultPassword;
    int superPassword;
    int password;
    int newPsw;

    Menu menu;
    Menu_Tips menuTips;
    public void Init(Menu mm)
    {
        menu = mm;
        menuTips = menu.menuTips;

        for (int i = 0; i < button.Length; i++)
        {
            button[i].Init(i, OnClick_Button);
        }
    }

    int IndexOfArry(int[] arry, int value)
    {
        for (int i = 0; i < arry.Length; i++)
        {
            if (arry[i] == value)
            {
                return i;
            }
        }
        return -1;
    }
    public void UpdateLanguage()
    {
        string format = "D" + passwordLength;
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            //中文               
            string strPswType = "";
            if (passwordType == en_PasswordType.Acc)
            {
                strPswType = "账目";
            }
            else if (passwordType == en_PasswordType.Menu)
            {
                strPswType = "设置";
            }
            if (optionType == en_PasswordOptionType.Modify)
            {
                text_Title.text = "修改" + strPswType + "密码";
            }
            else
            {
                text_Title.text = strPswType + "密码验证";
            }
            text_WxTips.text = "温馨提示：出厂密码为 " + defaultPassword.ToString(format) + "，修改密码后请注意记住密码。";
            accValue_Psw.SetName("密码:");
            button[IndexOfArry(tab_Key, BUTTON_ID_DEL)].GetComponentInChildren<Text>().text = "删除";
            button[IndexOfArry(tab_Key, BUTTON_ID_OK)].GetComponentInChildren<Text>().text = "确认";
            button[IndexOfArry(tab_Key, BUTTON_ID_BACK)].GetComponentInChildren<Text>().text = "退出";
        }
        else
        {
            //英文
            string strPswType = "";
            if (passwordType == en_PasswordType.Acc)
            {
                strPswType = "Acount ";
            }
            else if (passwordType == en_PasswordType.Menu)
            {
                strPswType = "Setting ";
            }
            if (optionType == en_PasswordOptionType.Modify)
            {
                text_Title.text = "Modify " + strPswType + "Password";
            }
            else
            {
                text_Title.text = "Check " + strPswType + "Password";
            }
            text_WxTips.text = "Tips: the factory password is " + defaultPassword.ToString(format) + "，Please remember the password after changing the password.";
            accValue_Psw.SetName("Password:");
            button[IndexOfArry(tab_Key, BUTTON_ID_DEL)].GetComponentInChildren<Text>().text = "Del";
            button[IndexOfArry(tab_Key, BUTTON_ID_OK)].GetComponentInChildren<Text>().text = "Ok";
            button[IndexOfArry(tab_Key, BUTTON_ID_BACK)].GetComponentInChildren<Text>().text = "Back";
            //button[IndexOfArry(tab_Key, BUTTON_ID_BACK)].GetComponentInChildren<Text>().gameObject.SetActive(false);
        }
    }
    // Use this for initialization
    public void GameStart(en_PasswordType pswType, en_PasswordOptionType type)
    {
        //#if UNITY_EDITOR
        //      Set.setVal.Password = 888888;
        //#endif
        gameObject.SetActive(true);
        passwordType = pswType;
        optionType = type;

        checkResult = false;
        pressedButton = false;
        waitCheck = true;
        postIndex = BUTTON_ID_BACK;
        Update_SelectPos();
        ChangeStatue(en_PasswordOptionSta.Check);

        bool showPsw = false;
        if (passwordType == en_PasswordType.Acc)
        {
            targetPassword = Set.setVal.AccPassword;
            defaultPassword = Set.DEF_ACCPASSWORD;
            superPassword = Set.SUP_ACCPASSWORD;
            passwordLength = 6;
            showPsw = Set.setVal.AccPassword == defaultPassword;
        }
        else if (passwordType == en_PasswordType.Menu)
        {
            targetPassword = Set.setVal.MenuPassword;
            defaultPassword = Set.DEF_MENUPASSWORD;
            superPassword = Set.SUP_MENUPASSWORD;
            passwordLength = 3;
            showPsw = Set.setVal.MenuPassword == defaultPassword;
        }
        else
        {
            defaultPassword = 0;
            superPassword = 0;
            passwordLength = 0;
        }
        if (passwordLength > 0)
        {
            accValue_Psw.text_Data.transform.localPosition = new Vector3(-(passwordLength - 0) * 11, 0);
        }
        else
        {
            accValue_Psw.text_Data.transform.localPosition = new Vector3(0, 0);
        }
        if (showPsw && optionType == en_PasswordOptionType.Check)
        {
            password = defaultPassword;
            numLen = passwordLength;
            Update_Psw();
        }

        UpdateLanguage();
    }

    void Exit()
    {
        gameObject.SetActive(false);
        Key.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (menuTips.gameObject.activeSelf)
            return;

        if (Key.MENU_LeftPressed() || Key.KEYFJ_Menu_LeftPressed())
        {
            postIndex = (postIndex + MAX_SEL - 1) % MAX_SEL;
            Update_SelectPos();
        }
        if (Key.MENU_RightPressed() || Key.KEYFJ_Menu_RightPressed())
        {
            postIndex = (postIndex + 1) % MAX_SEL;
            Update_SelectPos();
        }
        if (Key.MENU_OkPressed() || Key.KEYFJ_Menu_OkPressed() || pressedButton)
        {
            pressedButton = false;
            int key = tab_Key[postIndex];
            //
            if (key == BUTTON_ID_DEL)
            {
                // DEL
                if (numLen > 0)
                {
                    numLen--;
                    password /= 10;
                    Update_Psw();
                }
            }
            else if (key == BUTTON_ID_OK)
            {
                // ok
                if (numLen >= passwordLength)
                {
                    switch (statue)
                    {
                        case en_PasswordOptionSta.Check:
                            if (password == superPassword)
                            {
                                // 超级密码：恢复初始密码：                            
                                if (passwordType == en_PasswordType.Acc)
                                {
                                    Set.DefaultAccPassword();
                                    targetPassword = Set.setVal.AccPassword;
                                }
                                else if (passwordType == en_PasswordType.Menu)
                                {
                                    Set.DefaultMenuPassword();
                                    targetPassword = Set.setVal.MenuPassword;
                                }
                                else
                                {
                                    return;
                                }
                                if (Set.setVal.Language == (int)en_Language.Chinese)
                                {
                                    menuTips.Init("已恢复为初始密码", 3, true);
                                }
                                else
                                {
                                    menuTips.Init("Restored to original password", 3, true);
                                }
                                ClearPsw();
                            }
                            else if (password == targetPassword)
                            {
                                // 密码正确
                                if (optionType == en_PasswordOptionType.Modify)
                                {
                                    ChangeStatue(en_PasswordOptionSta.NewPsw);
                                }
                                else
                                {
                                    checkResult = true;
                                    Main.PlayTime = Set.setVal.GameTime;
                                    Main.CanSend_Score = true;
                                    Exit();
                                }
                            }
                            else
                            {
                                // 密码错误
                                if (Set.setVal.Language == (int)en_Language.Chinese)
                                {
                                    menuTips.Init("密码错误", 3, true);
                                }
                                else
                                {
                                    menuTips.Init("Password error", 3, true);
                                }
                                ClearPsw();
                            }
                            break;
                        case en_PasswordOptionSta.NewPsw:
                            if (optionType == en_PasswordOptionType.Modify)
                            {
                                newPsw = password;
                                ChangeStatue(en_PasswordOptionSta.AgainPsw);
                            }
                            break;
                        case en_PasswordOptionSta.AgainPsw:
                            if (optionType == en_PasswordOptionType.Modify)
                            {
                                if (newPsw == password)
                                {
                                    if (passwordType == en_PasswordType.Acc)
                                    {
                                        Set.setVal.AccPassword = password;
                                        Set.SaveAccPassword();
                                    }
                                    else if (passwordType == en_PasswordType.Menu)
                                    {
                                        Set.setVal.MenuPassword = password;
                                        Set.SaveMenuPassword();
                                    }
                                    else
                                    {
                                        return;
                                    }
                                    Exit();
                                    if (Set.setVal.Language == (int)en_Language.Chinese)
                                    {
                                        menuTips.Init("修改密码成功", 3, true);
                                    }
                                    else
                                    {
                                        menuTips.Init("Password modified successfully", 3, true);
                                    }
                                }
                                else
                                {
                                    if (Set.setVal.Language == (int)en_Language.Chinese)
                                    {
                                        menuTips.Init("两次输入密码不一致，请重新输入", 3, true);
                                    }
                                    else
                                    {
                                        menuTips.Init("The two passwords are inconsistent. Please re-enter", 3, true);
                                    }
                                    ClearPsw();
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        menuTips.Init("请输入" + passwordLength + "位数密码", 3, true);
                    }
                    else
                    {
                        menuTips.Init("Please enter a " + passwordLength + "-digit password", 3, true);
                    }
                    ClearPsw();
                }
            }
            else if (key == BUTTON_ID_BACK)
            {
                Exit();
            }
            else if (key < 10)
            {
                // 数字:
                if (numLen < passwordLength)
                {
                    password *= 10;
                    password += key;
                    numLen++;
                    Update_Psw();
                }
            }
        }
    }

    void ChangeStatue(en_PasswordOptionSta sta)
    {
        statue = sta;
        switch (statue)
        {
            case en_PasswordOptionSta.Check:
                ClearPsw();
                if (optionType == en_PasswordOptionType.Modify)
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        text_Tips.text = "请输入旧密码：";
                    }
                    else
                    {
                        text_Tips.text = "Enter old password：";
                    }
                }
                else
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        text_Tips.text = "请输入密码：";
                    }
                    else
                    {
                        text_Tips.text = "Enter password：";
                    }
                }

                break;
            case en_PasswordOptionSta.NewPsw:
                ClearPsw();
                if (Set.setVal.Language == (int)en_Language.Chinese)
                {
                    text_Tips.text = "请输入新密码：";
                }
                else
                {
                    text_Tips.text = "Enter new password:";
                }
                break;
            case en_PasswordOptionSta.AgainPsw:
                ClearPsw();
                if (Set.setVal.Language == (int)en_Language.Chinese)
                {
                    text_Tips.text = "请再次输入新密码：";
                }
                else
                {
                    text_Tips.text = "Enter new password again：";
                }
                break;
        }
    }

    void Update_SelectPos()
    {
        for (int i = 0; i < button.Length; i++)
        {
            if (i == postIndex)
            {
                button[i].SetSelect(true);
            }
            else
            {
                button[i].SetSelect(false);
            }
        }
    }
    void ClearPsw()
    {
        password = 0;
        numLen = 0;
        Update_Psw();
    }
    void Update_Psw()
    {
        if (numLen <= 0)
        {
            accValue_Psw.text_Data.text = "";
        }
        else
        {
            string format = "D" + numLen;
            accValue_Psw.text_Data.text = password.ToString(format);
        }
    }

    public void OnClick_Button(int id)
    {
        postIndex = id;
        Update_SelectPos();
        pressedButton = true;
    }
}
