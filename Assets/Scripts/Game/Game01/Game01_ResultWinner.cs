using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game01_ResultWinner : MonoBehaviour {
	public Game01_ResultOne[] resultOne;
	public Image image_Winner;
	//old:
	public Text text_Level;
	public Image[] image_Result;
	public Image[] image_WinnerResult;
	public Text[] text_Score;
	public Image[] image_Wins;
	public Text[] text_Wins;

	Sprite[] sprite_Result;
	Sprite[] sprite_Winner;

	int language = -1;

	readonly string[] tab_Langage = { "CN", "EN" };
	void CheckLanguage () {
		if (language != Set.setVal.Language) {
			language = Set.setVal.Language;

			sprite_Result = Resources.LoadAll<Sprite> ("UI/Result_" + tab_Langage[language]);
			sprite_Winner = Resources.LoadAll<Sprite> ("UI/Winner_CN");
		}
	}

	int IndexOfArry (int value, int[] arry, int len) {
		for (int i = 0; i < arry.Length && i < len; i++) {
			if (value == arry[i]) {
				return i;
			}
		}
		return -1;
	}
	int GetFirstId (int[] exsitedId, int exsitLen) {
		int firstId = -1;
		for (int i = 0; i < Main.playerNum && i < Main.MAX_PLAYER; i++) {
			if (IndexOfArry (i, exsitedId, exsitLen) >= 0)
				continue;
			if (firstId < 0) {
				firstId = i;
				continue;
			}
			if (FjData.g_Fj[i].Level < FjData.g_Fj[firstId].Level)
				continue;
			if (FjData.g_Fj[i].Level == FjData.g_Fj[firstId].Level && FjData.g_Fj[i].Result < FjData.g_Fj[firstId].Result)
				continue;
			if (FjData.g_Fj[i].Scores < FjData.g_Fj[firstId].Scores)
				continue;
			firstId = i;
		}
		return firstId;
	}

	public void Update_Value () {
		int[] rankId = new int[Main.playerNum];
		int len = 0;
		int firstId;
		for (int i = 0; i < rankId.Length && len < rankId.Length; i++) {
			firstId = GetFirstId (rankId, len);
			if (firstId < 0)
				break;
#if UNITY_EDITOR
			Debug.Log ("R_" + len + ": P_" + firstId);
#endif
			rankId[len] = firstId;
			len++;
		}
		// show:
		int pid;
		for (int i = 0; i < resultOne.Length; i++) {
			if (i < Main.playerNum && i < len) {
				resultOne[i].gameObject.SetActive (true);
				pid = rankId[i];
				resultOne[i].ShowValue (i, pid, FjData.g_Fj[pid].Level, FjData.g_Fj[pid].Result, FjData.g_Fj[pid].Scores);
			} else {
				resultOne[i].gameObject.SetActive (false);
			}
		}
		//image_Winner.transform.localPosition = resultOne[0].transform.localPosition + new Vector3 (-770, 0);
	}
	public void Update_Value0 (int level, int[] result, int[] wins) {
		CheckLanguage ();
		//
		text_Level.text = (level + 1).ToString ();
		//
		int winnerId = 0;
		if (result[0] == result[1]) {
			if (FjData.g_Fj[0].Scores > FjData.g_Fj[1].Scores) {
				winnerId = 0;
			} else if (FjData.g_Fj[0].Scores < FjData.g_Fj[1].Scores) {
				winnerId = 1;
			} else {
				// 平局: 
				winnerId = 2;
			}
		} else if (result[0] == 0) {
			winnerId = 1;
		} else {
			winnerId = 0;
		}
		
		for (int i = 0; i < Main.MAX_PLAYER; i++) {
			image_Result[i].sprite = sprite_Result[result[i]];
			image_Result[i].SetNativeSize ();
			//
			if (winnerId == 2) {
				image_WinnerResult[i].sprite = sprite_Winner[1];
			} else if (i == winnerId) {
				image_WinnerResult[i].sprite = sprite_Winner[1];
			} else {
				image_WinnerResult[i].sprite = sprite_Winner[0];
			}
			image_WinnerResult[i].SetNativeSize ();
			//
			text_Score[i].text = FjData.g_Fj[i].Scores.ToString ();
			//
			if (Set.setVal.OutMode == (int)en_OutMode.OutNone) {
				image_Wins[i].gameObject.SetActive (false);
			} else {
				image_Wins[i].gameObject.SetActive (true);
				text_Wins[i].text = wins[i].ToString ();
			}
		}
	}
}
