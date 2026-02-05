using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game97_UserRecordOne : MonoBehaviour
{
    public Text text_Id;
    public Text text_Level;
    public Text text_Score;

    public void Update_Value(int no, UserRecordOne recordOne)
    {
        text_Id.text = (no + 1).ToString();
        text_Level.text = (recordOne.level + 1).ToString();
        text_Score.text = recordOne.scores.ToString();
    }
}
