using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Game05_GameUIComm : MonoBehaviour
{
    public GameObject tips_Obj;
    public GameObject showLevel_Obj;
    public Text text_ShowLevel;
    public GameObject level_Obj;
    public Text text_Level;
    public GameObject wallLedNum_Obj;
    public Text text_WallLedNum;
    public Image image_ReadyTime;
    public Sprite[] sprite_ReadyTime;
    public GameObject continueGame_Obj;
    public Text text_ContinueTime;
    public Button button_ContinueYes;
    public Button button_ContinueNo;
    //
    public Button button_Exit;
    public GameObject exitTpis_Obj;
    public Button button_ExitYes;
    public Button button_ExitNo;
    public Game_PlayerNameInput playerNameInput;
    public GameRankList rankList;
    public Image image_JieDuan;

    public Sprite[] spr_JieDuan;
    public Text[] txt_TarageList;
    public static Game05_GameUIComm instance;
    float buttonExitShowTime;

    void Awake()
    {

        instance = this;

    }

    public void GameStart()
    {
        exitTpis_Obj.SetActive(false);
        button_Exit.gameObject.SetActive(false);
        playerNameInput.gameObject.SetActive(false);
        rankList.gameObject.SetActive(false);
        wallLedNum_Obj.SetActive(false);
        image_JieDuan.gameObject.SetActive(false);
        button_Exit.onClick.AddListener(OnClick_Exit);
        button_ExitYes.onClick.AddListener(OnClick_ExitYes);
        button_ExitNo.onClick.AddListener(OnClick_ExitNo);
        button_ContinueYes.onClick.AddListener(OnClick_ContinueYes);
        button_ContinueNo.onClick.AddListener(OnClick_ContinueNo);
        sprite_ReadyTime = Resources.LoadAll<Sprite>("UI/ReadyTime");
        for (int i = 0; i < txt_TarageList.Length; i++)
        {
            txt_TarageList[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (button_Exit.gameObject.activeSelf == false)
            {
                if (Main.IsDemo)
                {
                    Main.instance.ChangeStatue(en_MainStatue.Game_97);
                }
                else
                {
                    button_Exit.gameObject.SetActive(true);
                }
            }
            buttonExitShowTime = 0;
        }
        if (button_Exit.gameObject.activeSelf)
        {
            buttonExitShowTime += Time.deltaTime;
            if (buttonExitShowTime >= 5)
            {
                button_Exit.gameObject.SetActive(false);
            }
        }
    }

    public void Update_JieDuan(int level)
    {

        image_JieDuan.gameObject.SetActive(false);
          return;
        
        image_JieDuan.sprite = spr_JieDuan[level];
        image_JieDuan.gameObject.SetActive(true);
        image_JieDuan.SetNativeSize();
    }
    public void Update_Level(int level)
    {
        text_Level.text = (Main.MapIndex + 1).ToString();
    }
    public void Update_ShowLevel(int level)
    {
        text_ShowLevel.text = (level + 1).ToString();
    }
    public void Update_WallLedNum(int value)
    {
        text_WallLedNum.text = value.ToString();
    }
    public void Update_ReadyTime(int value)
    {
        image_ReadyTime.gameObject.SetActive(false);
        image_ReadyTime.gameObject.SetActive(true);
        image_ReadyTime.sprite = sprite_ReadyTime[value];
        image_ReadyTime.SetNativeSize();
    }

    public void Update_ContinueTime(int value)
    {
        text_ContinueTime.text = value.ToString("D2");
    }

    public void OnClick_ContinueYes()
    {
        Game05_Main.instance.continueResult = 1;
    }
    public void OnClick_ContinueNo()
    {
        Game05_Main.instance.continueResult = 2;
    }

    public void OnClick_Exit()
    {
        exitTpis_Obj.SetActive(true);
    }
    public void OnClick_ExitYes()
    {

        exitTpis_Obj.SetActive(false);
        Main.instance.ChangeStatue(en_MainStatue.Game_97);
        Main.instance.game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
    }
    public void OnClick_ExitNo()
    {

        exitTpis_Obj.SetActive(false);
    }
}
