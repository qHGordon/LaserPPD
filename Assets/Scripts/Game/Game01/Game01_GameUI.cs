using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Game01_GameUI : MonoBehaviour
{
    public Game01_PlayerUI playerUI;

    public GameObject tips_Obj;
    public GameObject showLevel_Obj;
    public Text text_ShowLevel;
    public GameObject showPlayer_Obj;
    public Text text_ShowPlayer;
    public GameObject level_Obj;
    public Text text_Level;
    public GameObject playerId_Obj;
    public Text text_PlayerId;
    public Image image_ReadyTime;
    public Sprite[] sprite_ReadyTime;
    public GameObject levelWaitTime_Obj;
    public Text text_LevelWaitTime;
    public Button button_LevelStart;
    public GameObject continue_Obj;
    public Button button_ContinueYes;
    public Button button_ContinueNo;
    public Text text_ContinueTime;
    public Game01_ResultWins resultWins;
    public Game01_ResultWinner resultWinner;
    public Button button_Exit;
    public GameObject exitTpis_Obj;
    public Button button_ExitYes;
    public Button button_ExitNo;
    public Game_PlayerNameInput playerNameInput;
    public GameRankList rankList;
    public Game00_CoinIn coinIn;
    public Game_GameTime gameTime;
    public GameObject pleaseCoin_Obj;

    float buttonExitShowTime;
    public GameObject time_Obj;
    public Text text_ReaminTime;
    public SettingInGame_02 SettingInGame_02;
    //public Game_Out gameOut;
    //public Game00_CoinIn coinIn;
    public bool levelStarted;
    public int continueResult;

    void Awake()
    {
        button_LevelStart.onClick.AddListener(OnClick_LevelStart);
        button_ContinueYes.onClick.AddListener(OnClick_ContinueYes);
        button_ContinueNo.onClick.AddListener(OnClick_ContinueNo);
        button_Exit.onClick.AddListener(OnClick_Exit);
        button_ExitYes.onClick.AddListener(OnClick_ExitYes);
        button_ExitNo.onClick.AddListener(OnClick_ExitNo);
        sprite_ReadyTime = Resources.LoadAll<Sprite>("UI/ReadyTime");
    }

    // Use this for initialization
    public void Awake0()
    {
        //for (int i = 0; i < playerUI.Length; i++) {
        //    playerUI[i].Awake0 (i);
        //}
        playerUI.Awake0(0);
    }

    public void GameStart()
    {

        exitTpis_Obj.SetActive(false);
        button_Exit.gameObject.SetActive(false);
        playerNameInput.gameObject.SetActive(false);
        rankList.gameObject.SetActive(false);
        resultWins.gameObject.SetActive(false);
        resultWinner.gameObject.SetActive(false);

        //coinIn.Init (0);
    }

    void Update()
    {
        if (Set.setVal.TimeMode == 1)
        {
            if (gameTime.gameObject.activeSelf)
            {
                gameTime.gameObject.SetActive(false);

            }
        }
        else
        {
            if (!gameTime.gameObject.activeSelf)
            {
                gameTime.gameObject.SetActive(false);

            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (button_Exit.gameObject.activeSelf == false)
            {
                button_Exit.gameObject.SetActive(true);
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


    public void Update_RemainTime(int value)
    {
        //text_ReaminTime.text = (value / 60).ToString("D2") + ":" + (value % 60).ToString("D2");
        text_ReaminTime.text = value.ToString("D3");
    }
    public void Update_ContinueTime(int value)
    {
        text_ContinueTime.text = value.ToString("D2");
    }
    public void Update_Level(int level)
    {
        text_Level.text = (level + 1).ToString();
    }
    public void Update_ShowLevel(int level)
    {
        text_ShowLevel.text = (level + 1).ToString();
    }
    public void Update_ShowPlayer(int id)
    {
        text_ShowPlayer.text = (id + 1).ToString();
    }
    public void Update_PlayerId(int id)
    {
        text_PlayerId.text = (id + 1).ToString();
    }
    public void Update_ReadyTime(int value)
    {
        image_ReadyTime.gameObject.SetActive(false);
        image_ReadyTime.gameObject.SetActive(true);
        image_ReadyTime.sprite = sprite_ReadyTime[value];
        image_ReadyTime.SetNativeSize();
    }
    public void Update_LevelWaitTime(int value)
    {
        text_LevelWaitTime.text = value.ToString("D2");
    }

    public void OnClick_LevelStart()
    {
        levelStarted = true;
    }
    public void OnClick_ContinueYes()
    {
        continueResult = 1;
    }
    public void OnClick_ContinueNo()
    {
        continueResult = 2;
    }
    public void OnClick_Exit()
    {
        exitTpis_Obj.SetActive(true);
    }
    public void OnClick_ExitYes()
    {
        exitTpis_Obj.SetActive(false);
        Main.instance.ChangeStatue(en_MainStatue.Game_97);
        if (Main.settingError != en_ErrorCode.None || Main.PlayTime < 0)
        {
            return;
        }
        Main.instance.game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
    }
    public void OnClick_ExitNo()
    {
        exitTpis_Obj.SetActive(false);
    }
}
