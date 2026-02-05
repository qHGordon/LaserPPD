using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ========== Game13 适配器 ==========

/// <summary>
/// Game13 玩家状态窗口适配器
/// </summary>
public class Game13_PlayerStaWinAdapter : BasePlayerStaWin
{
    private Game13_PlayerStaWin playerStaWin;

    public Game13_PlayerStaWinAdapter(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList)
        : base(root)
    {
        playerStaWin = new Game13_PlayerStaWin(root, lostScoreImg, bloodSprite, nullBloodSprite, hpImgList, lostScoreSpriteList);
    }

    public override void Updata(int hp)
    {
        playerStaWin.Updata(hp);
    }
}

/// <summary>
/// Game13 城堡适配器
/// </summary>
public class Game13_CastleAdapter : BaseCastle
{
    private Game13_Castle castle;

    public Game13_CastleAdapter(Game13_Castle castle)
    {
        this.castle = castle;
    }

    public override int GetHp => castle.GetHp;

    public override void Move(Action<Vector2Int, Vector2Int> blockEvent)
    {
        castle.Move(blockEvent);
    }

    public override bool GetHurt(Vector2Int attackVec, BlockType blockTy, Action<Vector2Int> castleLedEvent)
    {
        Game13_Block.BlockType game13BlockTy = blockTy == BlockType.Player1 ? Game13_Block.BlockType.Player1 : Game13_Block.BlockType.Player2;
        return castle.GetHurt(attackVec, game13BlockTy, castleLedEvent);
    }
}

/// <summary>
/// Game13 子弹适配器
/// </summary>
public class Game13_BulletAdapter : BaseBullet
{
    private Game13_Bullet bullet;

    public Game13_BulletAdapter(Game13_Bullet bullet)
    {
        this.bullet = bullet;
    }

    public Game13_Bullet GetOriginalBullet() => bullet;

    public override Vector2Int GetPos => bullet.GetPos;
    public override bool AllowMove { get => bullet.AllowMove; set => bullet.AllowMove = value; }
    public override BlockType GetBlockTy => bullet.GetBlockTy == Game13_Block.BlockType.Player1 ? BlockType.Player1 : BlockType.Player2;

    public override void Move(Action<Vector2Int, Vector2Int> blockEvent)
    {
        bullet.Move(blockEvent);
    }
}

/// <summary>
/// Game13 音频管理器适配器
/// </summary>
public class Game13_AudioManagerAdapter : BaseAudioManager
{
    private Game13_AudioManger audioManager;

    public Game13_AudioManagerAdapter(Game13_AudioManger audioManager)
    {
        this.audioManager = audioManager;
    }

    public override void PlaySound(string soundName)
    {
        if (audioManager.AudioDic.ContainsKey(soundName))
        {
            audioManager.AudioDic[soundName]();
        }
    }
}

// ========== Game23 适配器 ==========

/// <summary>
/// Game23 玩家状态窗口适配器
/// </summary>
public class Game23_PlayerStaWinAdapter : BasePlayerStaWin
{
    private Game23_PlayerStaWin playerStaWin;

    public Game23_PlayerStaWinAdapter(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList)
        : base(root)
    {
        playerStaWin = new Game23_PlayerStaWin(root, lostScoreImg, bloodSprite, nullBloodSprite, hpImgList, lostScoreSpriteList);
    }

    public override void Updata(int hp)
    {
        playerStaWin.Updata(hp);
    }
}

/// <summary>
/// Game23 城堡适配器
/// </summary>
public class Game23_CastleAdapter : BaseCastle
{
    private Game23_Castle castle;

    public Game23_CastleAdapter(Game23_Castle castle)
    {
        this.castle = castle;
    }

    public override int GetHp => castle.GetHp;

    public override void Move(Action<Vector2Int, Vector2Int> blockEvent)
    {
        castle.Move(blockEvent);
    }

    public override bool GetHurt(Vector2Int attackVec, BlockType blockTy, Action<Vector2Int> castleLedEvent)
    {
        Game23_Block.BlockType game23BlockTy = blockTy == BlockType.Player1 ? Game23_Block.BlockType.Player1 : Game23_Block.BlockType.Player2;
        return castle.GetHurt(attackVec, game23BlockTy, castleLedEvent);
    }
}

/// <summary>
/// Game23 子弹适配器
/// </summary>
public class Game23_BulletAdapter : BaseBullet
{
    private Game23_Bullet bullet;

    public Game23_BulletAdapter(Game23_Bullet bullet)
    {
        this.bullet = bullet;
    }

    public Game23_Bullet GetOriginalBullet() => bullet;

    public override Vector2Int GetPos => bullet.GetPos;
    public override bool AllowMove { get => bullet.AllowMove; set => bullet.AllowMove = value; }
    public override BlockType GetBlockTy => bullet.GetBlockTy == Game23_Block.BlockType.Player1 ? BlockType.Player1 : BlockType.Player2;

    public override void Move(Action<Vector2Int, Vector2Int> blockEvent)
    {
        bullet.Move(blockEvent);
    }
}

/// <summary>
/// Game23 音频管理器适配器
/// </summary>
public class Game23_AudioManagerAdapter : BaseAudioManager
{
    private Game23_AudioManger audioManager;

    public Game23_AudioManagerAdapter(Game23_AudioManger audioManager)
    {
        this.audioManager = audioManager;
    }

    public override void PlaySound(string soundName)
    {
        if (audioManager.AudioDic.ContainsKey(soundName))
        {
            audioManager.AudioDic[soundName]();
        }
    }
}
