using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 倒计时窗口
/// </summary>
public class Game14_TimeWin : BaseWin
{
	public Game14_TimeWin(Transform root, Text time_text, Func<string> get_time_label) : base(root, () => { }, () => { })
	{
		TimeText = time_text;
		TimeLabel = get_time_label;
	}

	public override void Update()
	{
		base.Update();
		if (!TimeText) return;
		TimeText.text = TimeLabel();
	}

	protected Text TimeText;

	protected Func<string> TimeLabel = () => "";
}

public class Game14_UIManger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
