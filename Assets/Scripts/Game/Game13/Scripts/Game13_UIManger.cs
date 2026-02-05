using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseWin
{
	public Transform GetRoot
	{
		get
		{
			return Root;
		}
	}

	public BaseWin(Transform root, Action open_event, Action close_event)
	{
		Root = root;
		if (!Root.GetComponent<CanvasGroup>()) Root.gameObject.AddComponent<CanvasGroup>();
		CanvasGroup = Root.GetComponent<CanvasGroup>();
		if (open_event != null) OpenEvent += open_event;
		if (close_event != null) CloseEvent += close_event;
	}

	public virtual void Open()
	{
		CanvasGroup.interactable = true;
		CanvasGroup.blocksRaycasts = true;
		CanvasGroup.alpha = 1;
	}

	public virtual void Close()
	{
		CanvasGroup.interactable = false;
		CanvasGroup.blocksRaycasts = false;
		CanvasGroup.alpha = 0;
	}

	public virtual void Update()
	{

	}

	protected Transform Root;

	protected CanvasGroup CanvasGroup;

	protected Action OpenEvent = () => { }, CloseEvent = () => { };
}

/// <summary>
/// 测试窗口
/// </summary>
public class TestingWin : BaseWin
{
	public bool GetIsOpen
	{
		get
		{
			return IsOpen;
		}
	}

	public TestingWin(Transform root, Button graph_item_btn, Button open_btn, Dictionary<Vector2Int, Stack<ColorTable.ColorType>> color_dic, Action open_event, Action<Vector2Int> on_click_graph_item_btn, Func<int> get_width, Func<int> get_height) : base(root, open_event, () => { })
	{
		Text open_btn_text = open_btn.transform.GetChild(0).GetComponent<Text>();
		open_btn_text.text = "点击打开";
		open_btn.onClick.AddListener(() =>
		{
			if (IsOpen) Close();
			else Open();
			open_btn_text.text = !IsOpen ? "点击打开" : "点击关闭";
		});

		GraphItemBtn = graph_item_btn;
		GraphItemBtnRect = GraphItemBtn.GetComponent<RectTransform>();
		OnClickGraphItemBtn = on_click_graph_item_btn;
		GetWidth = get_width;
		GetHeight = get_height;
		ColorDic = color_dic;
		Close();
	}

	/// <summary>
	/// 打开测试时创造按钮同时更新和添加事件
	/// </summary>
	public override void Open()
	{
		if (IsOpen) return;
		IsOpen = true;
		Vector3 graph_btn_pos = GraphItemBtn.transform.localPosition;
		for (int x = 0; x < GetWidth(); x++)
		{
			for (int y = 0; y < GetHeight(); y++)
			{
				int xpos = x, ypos = y;
				Button btn = GameObject.Instantiate(GraphItemBtn, Root);
				btn.name = "Btn_Game13_GraphItemBtn" + " (" + x.ToString() + ',' + y.ToString() + ')';
				btn.transform.localPosition = graph_btn_pos + new Vector3(GraphItemBtnRect.rect.width * x, GraphItemBtnRect.rect.height * y, 0);
				btn.onClick.RemoveAllListeners();
				btn.onClick.AddListener(() => OnClickGraphItemBtn(new Vector2Int(xpos, ypos)));
				GraphItemBtnDic[new Vector2Int(x, y)] = btn;
			}
		}
		base.Open();

	}

	public override void Close()
	{
		if (!IsOpen) return;
		IsOpen = false;
		foreach (var item in GraphItemBtnDic)
		{
			GameObject.Destroy(item.Value.gameObject);
		}
		GraphItemBtnDic.Clear();
		base.Close();
	}

	public override void Update()
	{
		base.Update();
		for (int y = 0; y < GetHeight(); y++)
		{
			for (int x = 0; x < GetWidth(); x++)
			{
				Vector2Int vec = new Vector2Int(x, y);
				if (!GraphItemBtnDic.ContainsKey(vec)) continue;
				if (!ColorDic.ContainsKey(vec)) continue;
				if (ColorDic[vec].Count == 0) continue;
				Vector4 vec4 = ColorTable.GetRGBA(ColorDic[vec].Peek());
				GraphItemBtnDic[new Vector2Int(x, y)].image.color = new Color(vec4.x, vec4.y, vec4.z, vec4.w);
			}
		}
	}

	protected bool IsOpen = true;

	protected Button GraphItemBtn;

	protected RectTransform GraphItemBtnRect;

	protected Dictionary<Vector2Int, Button> GraphItemBtnDic = new Dictionary<Vector2Int, Button>();

	protected Dictionary<Vector2Int, Stack<ColorTable.ColorType>> ColorDic = new Dictionary<Vector2Int, Stack<ColorTable.ColorType>>();

	protected Action<Vector2Int> OnClickGraphItemBtn = (Vector2Int vec2) => { };

	protected Func<int> GetWidth = () => 0, GetHeight = () => 0;
}

/// <summary>
/// 提示窗口
/// </summary>
public class TipsWin : BaseWin
{
	public TipsWin(Transform root, Button confirm_btn, Button close_btn, Text tips_text) : base(root, () => { }, () => { })
	{
		ConfirmBtn = confirm_btn;
		CloseBtn = close_btn;
		TipsText = tips_text;
		Close();
	}

	public void Open(Func<string> get_tips, List<Action> confirm_event_list, List<Action> close_event_list)
	{
		base.Open();
		Tips = get_tips;
		TipsText.text = Tips();
		ConfirmBtn.onClick.RemoveAllListeners();
		CloseBtn.onClick.RemoveAllListeners();
		foreach (var m_event in confirm_event_list) ConfirmBtn.onClick.AddListener(() => m_event());
		foreach (var m_event in close_event_list) CloseBtn.onClick.AddListener(() => m_event());
	}

	public override void Close()
	{
		base.Close();
		Time.timeScale = 1;
	}

	public override void Update()
	{
		base.Update();
		TipsText.text = Tips();
	}

	protected Button ConfirmBtn, CloseBtn;

	protected Text TipsText;

	protected Func<string> Tips = () => "";
}

/// <summary>
/// Game13 玩家玩家状态窗口
/// </summary>
public class Game13_PlayerStaWin : BaseWin
{
	public Game13_PlayerStaWin(Transform root, Image lost_score_img, Sprite blood_sprite, Sprite null_blood_sprite, List<Image> hp_img_list, List<Sprite> lost_sprite_list) : base(root, () => { }, () => { })
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


public class Game13_UIManger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
