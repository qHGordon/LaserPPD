using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRankOne : MonoBehaviour
{
    public Image image_Rank;
    public Text text_PlayerName;
    public Text text_Score;
    public Text text_Level;
    public Text text_Rank;

    ChangeSprite changeSprite;

    public void SetPlayerNameFont(Font font, int fontSize)
    {
        text_PlayerName.font = font;
        text_PlayerName.fontSize = fontSize;
    }
    public void UpdateValue(Sprite rankPic, RankOne rankOne, float animDelay)
    {
        if (changeSprite == null)
        {
            changeSprite = GetComponent<ChangeSprite>();
        }
        if (changeSprite != null)
        {
            changeSprite.delayTime = animDelay;
            changeSprite.GameStart();
        }
        if (image_Rank != null)
        {
            image_Rank.sprite = rankPic;
            image_Rank.SetNativeSize();
        }
        text_PlayerName.text = rankOne.playerName;
        text_Score.text = rankOne.score.ToString();
        text_Level.text = (rankOne.level + 1).ToString();
    }
    public void UpdateRankText(int rank)
    {
        if(text_Rank != null)
        {
            text_Rank.text = rank.ToString("D2");
        }
    }
}
