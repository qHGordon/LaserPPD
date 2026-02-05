using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game13 主类（继承自 GameCastleBase）
/// 只保留 Game13 特定的类型引用和实现
/// </summary>
public class Game13_Main : GameCastleBase
{
    #region UI引用（Game13特定）
    public Transform Game13_TestingWinRoot;
    public Button Game13_TestingWinOpenBtn, Game13_TestingWinGraphItemBtn;
    public Transform Game13_TipsWinRoot;
    public Button Game13_TipsWinConfirmBtn, Game13_TipsWinCancelBtn;
    public Text Game13_TipsWinText;
    #endregion

    // ========== 实现基类的抽象方法 ==========
    
    public override Transform GetTestingWinRoot() => Game13_TestingWinRoot;
    public override Button GetTestingWinOpenBtn() => Game13_TestingWinOpenBtn;
    public override Button GetTestingWinGraphItemBtn() => Game13_TestingWinGraphItemBtn;
    public override Transform GetTipsWinRoot() => Game13_TipsWinRoot;
    public override Button GetTipsWinConfirmBtn() => Game13_TipsWinConfirmBtn;
    public override Button GetTipsWinCancelBtn() => Game13_TipsWinCancelBtn;
    public override Text GetTipsWinText() => Game13_TipsWinText;

    protected override BasePlayerStaWin CreatePlayerStaWin1(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList)
    {
        return new Game13_PlayerStaWinAdapter(root, lostScoreImg, bloodSprite, nullBloodSprite, hpImgList, lostScoreSpriteList);
    }

    protected override BasePlayerStaWin CreatePlayerStaWin2(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList)
    {
        return new Game13_PlayerStaWinAdapter(root, lostScoreImg, bloodSprite, nullBloodSprite, hpImgList, lostScoreSpriteList);
    }

    protected override BaseCastle CreateCastle1(List<Vector2Int> posList)
    {
        Game13_Castle castle = new Game13_Castle(true, false, new Vector2Int(-1, 0), Game13_Block.BlockType.Player1, posList, () => new Vector2Int(XMax - 1, YMax - 1));
        return new Game13_CastleAdapter(castle);
    }

    protected override BaseCastle CreateCastle2(List<Vector2Int> posList)
    {
        Game13_Castle castle = new Game13_Castle(true, false, new Vector2Int(1, 0), Game13_Block.BlockType.Player2, posList, () => new Vector2Int(XMax - 1, YMax - 1));
        return new Game13_CastleAdapter(castle);
    }

    protected override BaseBullet CreateBullet(Vector2Int dirVec, Vector2Int pos, BlockType blockType)
    {
        Game13_Block.BlockType game13BlockType = blockType == BlockType.Player1 ? Game13_Block.BlockType.Player1 : Game13_Block.BlockType.Player2;
        Game13_Bullet bullet = new Game13_Bullet(dirVec, pos, game13BlockType, () => new Vector2Int(XMax - 1, YMax - 1));
        return new Game13_BulletAdapter(bullet);
    }

    protected override BaseAudioManager GetAudioManager()
    {
        return new Game13_AudioManagerAdapter(transform.GetComponent<Game13_AudioManger>());
    }

    protected override BlockType GetBlockTypePlayer1() => BlockType.Player1;
    protected override BlockType GetBlockTypePlayer2() => BlockType.Player2;

    protected override BlockType GetBulletBlockType(BaseBullet bullet)
    {
        if (bullet is Game13_BulletAdapter adapter)
        {
            Game13_Bullet originalBullet = adapter.GetOriginalBullet();
            return originalBullet.GetBlockTy == Game13_Block.BlockType.Player1 ? BlockType.Player1 : BlockType.Player2;
        }
        return bullet.GetBlockTy;
    }
}
