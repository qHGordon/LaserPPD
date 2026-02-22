using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRankList : MonoBehaviour
{
    public Text text_Name;
    public GameRankOne[] rankOne;
    public GameRankOne currRankOne;
    public Sprite[] sprite_Rank;
    public Font font_PlayerName;

    void Awake()
    {
        if (font_PlayerName != null)
        {
            for (int i = 0; i < rankOne.Length; i++)
            {
                rankOne[i].SetPlayerNameFont(font_PlayerName, 50);
            }
            currRankOne.SetPlayerNameFont(font_PlayerName, 50);
        }
    }

    public void UpdateValue(int rank, RankList rankList)
    {
        text_Name.text = Set.gameName[Set.setVal.GameChoose];
        currRankOne.gameObject.SetActive(false);
        //
        for (int i = 0; i < rankOne.Length; i++)
        {
            rankOne[i].gameObject.SetActive(false);
            if (i >= rankList.rankOne.Length)
                continue;
            if (rankList.rankOne[i].score <= 0)
                continue;
            //
            if (i == rank)
            {
                currRankOne.gameObject.SetActive(true);
                currRankOne.transform.position = rankOne[i].transform.position;
                currRankOne.UpdateValue(sprite_Rank[i], rankList.rankOne[i], i * 0.1f);
                currRankOne.UpdateRankText(i + 2);
            }
            else
            {
                rankOne[i].gameObject.SetActive(true);
                rankOne[i].UpdateValue(sprite_Rank[i], rankList.rankOne[i], i * 0.1f);
                rankOne[i].UpdateRankText(i + 1);
            }
        }
    }
}
