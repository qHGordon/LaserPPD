using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;
 
/// <summary>
/// 测试窗口
/// </summary> 
/// <summary>
/// 提示窗口
/// </summary> 
/// <summary>
/// Game23 玩家玩家状态窗口
/// </summary>
public class Game23_PlayerStaWin : BaseWin
{
	public Game23_PlayerStaWin(Transform root, Image lost_score_img, Sprite blood_sprite, Sprite null_blood_sprite, List<Image> hp_img_list, List<Sprite> lost_sprite_list) : base(root, () => { }, () => { })
	{
		LostScoreImg = lost_score_img;
		for (int i = 0; i < HpImgList.Count; i++) HpImgList[i].raycastTarget = false;
		BloodSprite = blood_sprite;
		NullBloodSprite = null_blood_sprite;
		HpImgList = hp_img_list;
		LostScoreSpriteList = lost_sprite_list;
	}

	public override void Update()
	{
		base.Update();
	}

	public void Updata(int hp)
	{
		LostScoreImg.sprite = LostScoreSpriteList[Mathf.Max(0, HpImgList.Count - hp)];
		for (int i = 0; i < HpImgList.Count; i++) HpImgList[i].sprite = i < hp ? BloodSprite : NullBloodSprite;
	}

	protected Image LostScoreImg;

	protected Sprite BloodSprite, NullBloodSprite;

	protected List<Image> HpImgList = new List<Image>();

	protected List<Sprite> LostScoreSpriteList = new List<Sprite>();
}


public class Game23_UIManger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
