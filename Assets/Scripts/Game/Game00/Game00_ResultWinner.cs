using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Game00_ResultWinner : MonoBehaviour {
	public Image image_Result;
	public Text text_Level;
	public Text text_Score;

	Sprite[] sprite_Result;
	public void Update_Value (bool winner, int level, int score) {
		if (sprite_Result == null) {
			sprite_Result = Resources.LoadAll<Sprite> ("UI/Winner_CN");
		}
		if (winner) {
			image_Result.sprite = sprite_Result[1];
		} else {
			image_Result.sprite = sprite_Result[0];
		}
		image_Result.SetNativeSize ();
		//
		text_Level.text = (level + 1).ToString ();
		text_Score.text = score.ToString ();
	}
}
