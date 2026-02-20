using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Game01_ResultOne : MonoBehaviour {
	public Text text_RankId;
	public Text text_PlayerId;
	public Text text_Level;
	public Text text_Scores;
	public Image image_Result;

	public void ShowValue (int rank, int playerId, int level, int result, int scores) {
		text_RankId.text = (rank + 1).ToString ();
		text_PlayerId.text = (playerId + 1).ToString ();
		text_Level.text = (level + 1).ToString ();
		text_Scores.text = scores.ToString ();
		if (result == 0) {
			image_Result.gameObject.SetActive (false);
		} else {
			image_Result.gameObject.SetActive (true);
		}
	}
}
