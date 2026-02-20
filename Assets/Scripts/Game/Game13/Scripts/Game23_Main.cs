using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

/// <summary>
/// Game23 主类（继承自 GameCastleBase）
/// 只保留 Game23 特定的类型引用和实现
/// </summary>
public class Game23_Main : GameCastleBase
{
    #region UI引用（Game23特定）
    public Transform Game23_TestingWinRoot;
    public Button Game23_TestingWinOpenBtn, Game23_TestingWinGraphItemBtn;
    public Transform Game23_TipsWinRoot;
    public Button Game23_TipsWinConfirmBtn, Game23_TipsWinCancelBtn;
    public Text Game23_TipsWinText;
    #endregion

    // ========== 实现基类的抽象方法 ==========
    
    public override Transform GetTestingWinRoot() => Game23_TestingWinRoot;
    public override Button GetTestingWinOpenBtn() => Game23_TestingWinOpenBtn;
    public override Button GetTestingWinGraphItemBtn() => Game23_TestingWinGraphItemBtn;
    public override Transform GetTipsWinRoot() => Game23_TipsWinRoot;
    public override Button GetTipsWinConfirmBtn() => Game23_TipsWinConfirmBtn;
    public override Button GetTipsWinCancelBtn() => Game23_TipsWinCancelBtn;
    public override Text GetTipsWinText() => Game23_TipsWinText;

    protected override BasePlayerStaWin CreatePlayerStaWin1(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList)
    {
        return new Game23_PlayerStaWinAdapter(root, lostScoreImg, bloodSprite, nullBloodSprite, hpImgList, lostScoreSpriteList);
    }

    protected override BasePlayerStaWin CreatePlayerStaWin2(Transform root, Image lostScoreImg, Sprite bloodSprite, Sprite nullBloodSprite, List<Image> hpImgList, List<Sprite> lostScoreSpriteList)
    {
        return new Game23_PlayerStaWinAdapter(root, lostScoreImg, bloodSprite, nullBloodSprite, hpImgList, lostScoreSpriteList);
    }

    protected override BaseCastle CreateCastle1(List<Vector2Int> posList)
    {
        Game23_Castle castle = new Game23_Castle(true, false, new Vector2Int(-1, 0), Game23_Block.BlockType.Player1, posList, () => new Vector2Int(XMax - 1, YMax - 1));
        return new Game23_CastleAdapter(castle);
    }

    protected override BaseCastle CreateCastle2(List<Vector2Int> posList)
    {
        Game23_Castle castle = new Game23_Castle(true, false, new Vector2Int(1, 0), Game23_Block.BlockType.Player2, posList, () => new Vector2Int(XMax - 1, YMax - 1));
        return new Game23_CastleAdapter(castle);
    }

    protected override BaseBullet CreateBullet(Vector2Int dirVec, Vector2Int pos, BlockType blockType)
    {
        Game23_Block.BlockType game23BlockType = blockType == BlockType.Player1 ? Game23_Block.BlockType.Player1 : Game23_Block.BlockType.Player2;
        Game23_Bullet bullet = new Game23_Bullet(dirVec, pos, game23BlockType, () => new Vector2Int(XMax - 1, YMax - 1));
        return new Game23_BulletAdapter(bullet);
    }

    protected override BaseAudioManager GetAudioManager()
    {
        return new Game23_AudioManagerAdapter(transform.GetComponent<Game23_AudioManger>());
    }

    protected override BlockType GetBlockTypePlayer1() => BlockType.Player1;
    protected override BlockType GetBlockTypePlayer2() => BlockType.Player2;

    protected override BlockType GetBulletBlockType(BaseBullet bullet)
    {
        if (bullet is Game23_BulletAdapter adapter)
        {
            Game23_Bullet originalBullet = adapter.GetOriginalBullet();
            return originalBullet.GetBlockTy == Game23_Block.BlockType.Player1 ? BlockType.Player1 : BlockType.Player2;
        }
        return bullet.GetBlockTy;
    }
}
