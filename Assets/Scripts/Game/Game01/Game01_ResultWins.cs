using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Game01_ResultWins : MonoBehaviour {
	public Text text_Level;
	public Text text_Score;
	public Image image_Wins;
	public Text text_Wins;

	public void Update_Result (int level, int score, int wins) {
		text_Level.text = level.ToString ();
		text_Score.text = score.ToString();
		if (Set.setVal.OutMode == (int)en_OutMode.OutNone) {
			image_Wins.gameObject.SetActive (false);
		} else {
			image_Wins.gameObject.SetActive (true);
			text_Wins.text = wins.ToString ();
		}
	}
}
