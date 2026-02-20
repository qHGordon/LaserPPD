using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

/// <summary>
/// 基础地砖，当前游戏的地砖类型由此派生
/// </summary>
public class Game13_Block
{

	public Vector2Int GetDirVec
	{
		get
		{
			return DirVec;
		}
	}

	public BlockType GetBlockTy
	{
		get
		{
			return BlockTy;
		}
	}

	public enum BlockType
	{
		Player1,
		Player2,
	}

	/// <summary>
	/// 构造函数
	/// </summary>
	/// <param name="dir_vec">地砖移动方向，游戏开始后便不可更改</param>
	public Game13_Block(Vector2Int dir_vec, BlockType block_ty, Func<Vector2Int> get_max_pos)
	{
		DirVec = dir_vec;
		BlockTy = block_ty;
		GetMaxPos = get_max_pos;
	}

	public virtual void Move(Action<Vector2Int, Vector2Int> block_event)
	{

	}

	public virtual void Update()
	{

	}

	protected Vector2Int DirVec;

	protected BlockType BlockTy;

	protected Func<Vector2Int> GetMaxPos = () => Vector2Int.zero;
}

/// <summary>
/// 派生地砖城堡，由多个地砖的位置组成
/// </summary>
public class Game13_Castle : Game13_Block
{

	public int GetHp
	{
		get
		{
			return HpList.Count;
		}
	}


	/// <summary>
	/// 城堡构造函数
	/// </summary>
	/// <param name="is_dir_x">是否在x轴移动</param>
	/// <param name="is_dir_y">是否在y轴移动</param>
	/// <param name="dir_vec">城堡目前移动方向</param>
	/// <param name="block_ty">城堡的类型</param>
	/// <param name="hp_list">城堡血量</param>
	/// <param name="get_max_pos">当前地图最大值</param>
	public Game13_Castle(bool is_dir_x, bool is_dir_y, Vector2Int dir_vec, BlockType block_ty, List<Vector2Int> hp_list, Func<Vector2Int> get_max_pos) : base(dir_vec, block_ty, get_max_pos)
	{
		IsDirX = is_dir_x;
		IsDirY = is_dir_y;
		HpList = hp_list;
		GetMaxPos = get_max_pos;
		UpdatePos();
	}

	/// <summary>
	/// 当受到攻击时使用此函数，判断是否收到攻击
	/// </summary>
	/// <param name="attack_vec">攻击物体</param>
	/// <param name="block_ty">攻击物体所属地砖类型</param>
	/// <param name="castle_led_event">如果成功受击触发的事件</param>
	/// <returns></returns>
	public virtual bool GetHurt(Vector2Int attack_vec, BlockType block_ty, Action<Vector2Int> castle_led_event)
	{
		if (HpList.Count == 0 || !HpList.Contains(attack_vec) || BlockTy == block_ty) return false;
		castle_led_event(attack_vec);
		HpList.Remove(attack_vec);
		return true;
	}

	/// <summary>
	/// 城堡根据当前方向进行移动，该函数执行时不会调用基类的Move
	/// </summary>
	/// <param name="block_event">更新当前地砖逻辑</param>
	public override void Move(Action<Vector2Int, Vector2Int> block_event)
	{
		UpdatePos();
		DirVec.x = !IsDirX ? 0 : LeftXPos == 0 ? 1 : RightXPos == GetMaxPos().x ? -1 : DirVec.x;
		DirVec.y = !IsDirY ? 0 : LeftYPos == 0 ? 1 : RightYPos == GetMaxPos().y ? -1 : DirVec.y;
		for (int i = 0; i < HpList.Count; i++)
		{
			int idx = i;
			block_event(HpList[idx], HpList[idx] += DirVec);
		}
		string s = "MaxX:" + GetMaxPos().x + "// " + "LeftXPos:" + LeftXPos.ToString() + "// " + "RightXPos:" + RightXPos.ToString() + "// " + "Dir:" + DirVec.ToString() + "// ";
		for (int i = 0; i < HpList.Count; i++)
		{
			string tmps = HpList[i].ToString();
			s = s + tmps + ' ';
		}
		s += '\n';

	}

	public override void Update()
	{
		UpdatePos();
		for (int i = 0; i < HpItemList.Count; i++)
		{
			if (!HpItemList[i].IsDestroy) continue;
			HpItemList[i].DestroyTime = Mathf.Max(0, HpItemList[i].DestroyTime - Time.deltaTime);

		}
	}

	// 是否该移动方向
	protected bool IsDirX = false, IsDirY = false;

	// 城堡位置列表中的位置的x, y最小值和最大值
	protected int LeftXPos = 0, LeftYPos = 0, RightXPos = 0, RightYPos = 0;

	// 城堡血量，即拥有的地砖数量，这里用位置进行表示
	protected List<Vector2Int> HpList = new List<Vector2Int>();

	protected List<HpItem> HpItemList = new List<HpItem>();


	/// <summary>
	/// 更新当前城堡左右边界状态
	/// </summary>
	protected void UpdatePos()
	{
		if (HpList.Count == 0) return;
		LeftXPos = RightXPos = HpList[0].x;
		LeftYPos = RightYPos = HpList[0].y;
		foreach (var vec in HpList)
		{
			LeftXPos = Mathf.Min(LeftXPos, vec.x);
			LeftYPos = Mathf.Min(LeftYPos, vec.y);
			RightXPos = Mathf.Max(RightXPos, vec.x);
			RightYPos = Mathf.Max(RightYPos, vec.y);
		}
	}

	protected class HpItem
	{
		public bool IsDestroy = false;

		public float DestroyTime = 0.5f;

		public Vector2Int Pos = Vector2Int.zero;

		public HpItem(Vector2Int pos)
		{
			Pos = pos;
		}
	}

}

/// <summary>
/// 派生地砖子弹，由一个地砖组成
/// </summary>
public class Game13_Bullet : Game13_Block
{

	public static Dictionary<Vector2Int, Game13_Bullet> BulletDic = new Dictionary<Vector2Int, Game13_Bullet>();

	public Vector2Int GetPos
	{
		get
		{
			return Pos;
		}
	}

	public bool AllowMove = false;

	public Game13_Bullet(Vector2Int dir_vec, Vector2Int pos, BlockType block_ty, Func<Vector2Int> get_max_pos) : base(dir_vec, block_ty, get_max_pos)
	{
		Pos = pos;
		BulletDic[Pos] = this;
	}

	public override void Move(Action<Vector2Int, Vector2Int> block_event)
	{
		if (!AllowMove || DirVec == Vector2Int.zero) return;
		DirVec.x = Pos.x <= 0 || Pos.x >= GetMaxPos().x ? 0 : DirVec.x;
		DirVec.y = Pos.y <= 0 || Pos.y >= GetMaxPos().y ? 0 : DirVec.y;
		block_event(Pos, Pos += DirVec);
	}

	protected Vector2Int Pos = Vector2Int.zero;
}


