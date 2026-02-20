using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public delegate void OnValueChangedCall(int id, bool value);

public class Menu_SelectOne : MonoBehaviour
{
	public Toggle toggle;
	public Text text;

	int Id;
	OnValueChangedCall onValueChangedCall;
	public void Init(int id, OnValueChangedCall call)
	{
		Id = id;
		onValueChangedCall = call;
		if (toggle != null)
		{
			toggle.onValueChanged.AddListener(OnValueChange);
		}
	}
	public void SetName(string str)
	{
		if (text != null)
		{
			text.text = str;
		}
	}
	public bool IsOn()
	{
		return toggle.isOn;
	}
	public void SetIsOn(bool isOne)
	{
		if (toggle != null)
		{
			toggle.isOn = isOne;
		}
	}

	void OnValueChange(bool isOn)
	{
		if (onValueChangedCall != null)
		{
			onValueChangedCall(Id, isOn);
		}
	}
}
