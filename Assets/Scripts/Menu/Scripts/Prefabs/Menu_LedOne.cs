using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnClickCall (int id);

public class Menu_LedOne : MonoBehaviour {
	public Image image_Pic;
	public Text text_ID;
	public Button button;

	public int Id;
    public int x;
    public int y;
	public byte pointType;
	OnClickCall onClickCall;
	public void Init (int ch, int id) {
		text_ID.text = (ch + 1).ToString () + "-" + (id + 1).ToString ("D3");
	}
	public void Init (int id, OnClickCall call) {
		Id = id;
		text_ID.text = (id + 1).ToString ("D3");
		button.onClick.AddListener (OnClick);
		onClickCall = call;
	}
    public void SetPosXY(int posx, int posy) {
        x = posx;
        y = posy;
    }
	public void SetColor (Color color) {
		image_Pic.color = color;
	}
	public void SetColor (Color color, byte type) {
		image_Pic.color = color;
		pointType = type;
	}

	public void OnClick () {
		if (onClickCall != null) {
#if UNITY_EDITOR
			//Debug.Log ("OnClick: " + Id);
#endif
			onClickCall (Id);
		}
	}
}
