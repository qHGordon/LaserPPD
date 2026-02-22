using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game97_UserInfo : MonoBehaviour
{
    public Text text_CardId;
    public Text text_UserName;
    public Text text_PlayCnt;
    public Text text_MaxScore;
    public Game97_UserRecordOne[] recordOne;

    float runTime = 0;
    // Use this for initialization

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void ShowUserInfo()
    {
        text_CardId.text = Main.currUser.cardId.ToString("D10");
        text_UserName.text = Main.currUser.userName;
        text_PlayCnt.text = Main.currUser.playCnt.ToString();
        text_MaxScore.text = Main.currUser.maxScore.ToString();
        runTime = 0;

        List<UserRecordOne> list_Record = Main.currUser.list_RecordUse;
        for (int i = 0; i < recordOne.Length; i++)
        {
            if (i < list_Record.Count)
            {
                recordOne[i].gameObject.SetActive(true);
                recordOne[i].Update_Value(i, list_Record[i]);
            }
            else
            {
                recordOne[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        runTime += Time.deltaTime;
        if (runTime >= 20)
        {
            runTime = 0;
            gameObject.SetActive(false);
        }
    }
    public void OnClick()
    {
        gameObject.SetActive(false);
    }
}
