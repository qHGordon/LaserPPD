using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using LaserPPD.Core;


/// <summary>
/// Game01 游戏状态枚举，定义了游戏从开始到结束的完整生命周期中所有可能的状态。
/// 状态按照游戏流程的先后顺序排列，通过 ChangeStatue 方法进行状态切换。
/// </summary>
public enum en_Game01_Sta
{
    /// <summary>无状态（默认初始值）</summary>
    None = 0,
    /// <summary>空闲状态，游戏未开始</summary>
    Idle,
    /// <summary>显示操作提示界面</summary>
    Tips,
    /// <summary>等待玩家按下开始按钮</summary>
    WaitStart,
    /// <summary>展示当前关卡信息</summary>
    ShowLevel,
    /// <summary>展示当前玩家编号（多人模式使用）</summary>
    ShowPlayer,
    /// <summary>准备倒计时阶段</summary>
    Ready,
    /// <summary>游戏进行中（核心游戏循环）</summary>
    Play,
    /// <summary>续玩确认阶段（投币续玩）</summary>
    Continue,
    /// <summary>显示本局结果（通过/失败）</summary>
    ShowResult,
    /// <summary>显示结算分数</summary>
    ShowResultScore,
    /// <summary>等待进入下一关</summary>
    WaitNextLevel,
    /// <summary>单人模式结算奖励展示</summary>
    ShowWins,
    /// <summary>对战模式结算奖励展示（显示胜者）</summary>
    ShowWiner,
    /// <summary>玩家输入姓名（用于排行榜）</summary>
    InputName,
    /// <summary>显示排行榜</summary>
    RankList,
    /// <summary>游戏结束过渡状态</summary>
    End,
    /// <summary>退出游戏（执行清理和场景切换）</summary>
    Out,
    /// <summary>退出完成</summary>
    OutEnd,
}


/// <summary>
/// [Game01] Game01 游戏主控制器类。
/// 负责管理整个 Game01 的游戏生命周期，包括：
/// - 游戏状态机的驱动与切换（通过 en_Game01_Sta 枚举定义的各个状态）
/// - 关卡流程控制（关卡展示、准备倒计时、游戏进行、结算、续玩等）
/// - 玩家管理与分数结算
/// - UI 界面更新的调度
/// - 音效与背景音乐的播放控制
/// - 震屏特效与开始按钮 LED 灯效控制
/// </summary>
public class Game01_Main : MonoBehaviour
{
    /// <summary>当前阶段索引（用于标记游戏进行的阶段）</summary>
    public int index_JieDuan = 0;
    /// <summary>游戏 UI 管理器，负责所有界面元素的显示与更新</summary>
    public Game01_GameUI gameUI;
    /// <summary>震屏效果的根物体，通过改变其 localPosition 实现震动</summary>
    public GameObject shakeMain_Obj;
    /// <summary>玩家控制器，管理玩家的游戏逻辑与状态</summary>
    public Game01_Player player;
    /// <summary>生命值图标的预制体</summary>
    public GameObject lifeOne_Prefab;

    // ---- 音频资源 ----
    /// <summary>背景音乐播放器</summary>
    public AudioSource audioSource_BackG;
    /// <summary>其他音效播放器（倒计时、结果等短音效）</summary>
    public AudioSource audioSource_Others;
    /// <summary>准备倒计时音效</summary>
    public AudioClip audioClip_ReadyTime;
    /// <summary>开始游戏音效（"Go!"）</summary>
    public AudioClip audioClip_ReadyGo;
    /// <summary>时间不足警告音效（剩余 5 秒内每秒播放）</summary>
    public AudioClip audioClip_Timeout;
    /// <summary>时间耗尽音效</summary>
    public AudioClip audioClip_TimesUp;
    /// <summary>通关成功音效</summary>
    public AudioClip audioClip_Pass;
    /// <summary>关卡失败音效</summary>
    public AudioClip audioClip_Loss;

    /// <summary>主控制器引用</summary>
    Main main;
    /// <summary>当前游戏状态</summary>
    public en_Game01_Sta statue;
    /// <summary>通用运行计时器，用于各状态下的延时控制</summary>
    public float runTime = 0;
    /// <summary>通用运行计数器，用于倒计时等整数秒判断</summary>
    int runCnt;
    /// <summary>发送命令的计时器</summary>
    float sendTime;

    /// <summary>当前背景音乐索引，用于循环切换 BGM</summary>
    int bgmIndex = 0;
    /// <summary>当前游戏 ID 标识</summary>
    public int gameId = 56;
    /// <summary>当前设置项索引</summary>
    public int setIndex;
    /// <summary>设置项总数</summary>
    public int setCount;
    /// <summary>命令返回是否成功的标志</summary>
    bool cmdRetSucess;
    /// <summary>游戏是否已结束的标志</summary>
    bool isGameOver;

    /// <summary>当前参与的玩家数量</summary>
    int playerNum = Main.MAX_PLAYER;
    /// <summary>当前活跃的玩家 ID</summary>
    int playerId;
    /// <summary>当前游戏关卡编号</summary>
    public int gameLevel;
    /// <summary>最大可用关卡数</summary>
    int maxLevel;
    /// <summary>最大生命值</summary>
    public int maxLife;
    /// <summary>准备倒计时剩余秒数</summary>
    int readyTime;
    /// <summary>当前关卡的游戏时间（秒），运行中持续递减</summary>
    public float gameTime;

    /// <summary>剩余游戏总时间（从 Main.PlayTime 同步）</summary>
    float remainGameTime;
    /// <summary>当前投币数</summary>
    int coins;
    /// <summary>开始按钮的硬件 ID</summary>
    int startButtonId;
    /// <summary>排行榜单条记录数据</summary>
    RankOne rankOne;
    /// <summary>大关卡编号（gameLevel / 10）</summary>
    public int bigGameLevel;
    /// <summary>小关卡编号（gameLevel % 10）</summary>
    public int smallGameLevel;

    /// <summary>Game01_Main 的全局单例实例，方便其他模块访问</summary>
    public static Game01_Main instance;
    /// <summary>
    /// 自定义初始化方法，由主控制器 Main 调用以替代 Unity 原生 Awake。
    /// 初始化单例引用，并依次初始化玩家和游戏 UI。
    /// </summary>
    /// <param name="mainn">主控制器 Main 的引用</param>
    public void Awake0(Main mainn)
    {
        instance = this;
        main = mainn;
        player.Awake0(this);
        gameUI.Awake0();
    }

    /// <summary>左侧玩家 ID 映射表（用于双人模式的位置分配）</summary>
    readonly int[] tab_PlayerId_Left = { 0, 1 };
    /// <summary>右侧玩家 ID 映射表（用于双人模式的位置分配）</summary>
    readonly int[] tab_PlayerId_Right = { 1, 0 };

    /// <summary>
    /// 游戏开始入口方法。
    /// 初始化游戏参数、玩家数据、UI 界面和地图，并根据是否为演示模式切换到对应的初始状态。
    /// 非演示模式下还会记录玩家的游玩次数到本地存储。
    /// </summary>
    public void GameStart()
    {
        // 设置地图保护时间（秒），防止游戏刚开始时受到伤害
        Game_Map01.instance.protectTime = 5;
        // 获取硬件开始按钮的 ID
        startButtonId = LedKey.GetLeiSheStartButtonId();
        playerNum = 1;
        index_JieDuan = 0;

        // 将玩家 UI 引用传递给玩家控制器
        player.playerUI = gameUI.playerUI;

        // 激活游戏 UI 并执行 UI 初始化
        gameUI.gameObject.SetActive(true);
        gameUI.GameStart();
        // 初始化游戏地图
        Game_Map01.instance.Initmap();

        // 非演示模式下，记录本次游玩数据
        if (Main.IsDemo == false)
        {
            // 如果启用了手环模式且有当前用户，增加该用户的游玩次数并保存
            if (Set.setVal.BraceletMode != 0 && Main.currUser != null)
            {
                Main.currUser.playCnt++;
                UserManager.SaveData(Main.currUser.cardId, Main.currUser);
            }
            // 累加当日和总计的游玩次数，并持久化保存
            FjData.acc[0].PlayCnt++;
            FjData.totalAcc[0].PlayCnt++;
            FjData.SaveAcc_PlayCnt(0, false);
            FjData.SaveTotalAcc_PlayCnt(0);
        }

        // 初始化所有玩家的分数为 0，结果标记为 1（存活/通过状态）
        for (int i = 0; i < Main.MAX_PLAYER; i++)
        {
            FjData.g_Fj[i].Scores = 0;
            FjData.g_Fj[i].Result = 1;
        }
#if UNITY_EDITOR
        Debug.LogError("PlayerNum: " + Main.playerNum);
#endif
        // 启动玩家（ID=0），并计算最大关卡数
        player.GameStart(0);
        maxLevel = Mathf.Min(Main.gameSetting.maxLevel, Main.gameSetting.gameLevelSetting.Length);
        gameLevel = 0;
        gameLevel = Main.gameLevel;

        // 根据是否需要投币或时间限制，决定是否显示游戏时间 UI
        if (Set.setVal.StartCoins > 0 || Set.setVal.TimeEnable != 0)
        {
            gameUI.gameTime.gameObject.SetActive(true);
        }
        else
        {
            gameUI.gameTime.gameObject.SetActive(false);
        }

        // 演示模式直接进入游戏，正常模式先显示操作提示
        if (Main.IsDemo)
        {
            ChangeStatue(en_Game01_Sta.Play);
        }
        else
        {
            ChangeStatue(en_Game01_Sta.Tips);
        }
    }

    /// <summary>
    /// 续玩处理方法。当玩家确认续玩后调用，
    /// 重置游戏状态为 Play，隐藏续玩相关 UI，恢复游戏计时和背景音乐。
    /// </summary>
    void GameContinue()
    {
        player.Continue();
        statue = en_Game01_Sta.Play;

        // 隐藏续玩相关 UI，显示游戏时间
        gameUI.coinIn.gameObject.SetActive(false);
        gameUI.playerId_Obj.SetActive(false);
        gameUI.continue_Obj.SetActive(false);
        gameUI.time_Obj.SetActive(true);

        // 计算大小关卡编号（大关 = 十位，小关 = 个位）
        bigGameLevel = gameLevel / 10;
        smallGameLevel = gameLevel % 10;

        // 重置游戏时间为当前关卡配置的时间
        gameTime = Main.gameSetting.gameLevelSetting[gameLevel].gameTime;

        gameUI.Update_RemainTime((int)gameTime);
        // 恢复背景音乐并播放开始音效
        audioSource_BackG.Play();
        PlaySound(audioClip_ReadyGo);
    }

    /// <summary>上一帧的整数秒游戏时间，用于检测秒数变化以触发音效</summary>
    int rTime = 0;

    /// <summary>
    /// Unity 每帧更新方法，驱动游戏主状态机。
    /// 首先检查全局剩余时间，若时间耗尽则强制结束游戏；
    /// 然后根据当前游戏状态（statue）执行对应的逻辑分支。
    /// </summary>
    void Update()
    {
        // 同步全局剩余游戏时间
        remainGameTime = Main.PlayTime;

        // 非演示模式下，检测全局游戏时间是否已耗尽
        if (Main.IsDemo == false)
        {
            // 仅在需要投币或启用了时间限制的模式下检查
            if (Set.setVal.StartCoins > 0 || Set.setVal.TimeEnable != 0)
            {
                if (remainGameTime > 0)
                {
                    // 时间充足，继续游戏
                }
                else if (statue <= en_Game01_Sta.Play)
                {
                    // 全局时间耗尽且游戏仍在进行中，强制进入结果展示
                    ChangeStatue(en_Game01_Sta.ShowResult);
                }
            }
        }

        // ---- 游戏主状态机 ----
        switch (statue)
        {
            // 【提示状态】显示操作提示 2 秒后，切换到等待开始
            case en_Game01_Sta.Tips:
                runTime += Time.deltaTime;
                if (runTime >= 2f)
                {
                    runTime = 0;
                    ChangeStatue(en_Game01_Sta.WaitStart);
                }
                break;

            // 【等待开始】闪烁开始按钮 LED，等待玩家按下开始键
            case en_Game01_Sta.WaitStart:
                StartButtonLed_Run();
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // 调试用：空格键触发 LED 初始化（已注释）
                }
                // 检测硬件开始按钮、UI 开始按钮或键盘 O 键
                if (LedKey.KeyPressed(startButtonId) || gameUI.levelStarted || Input.GetKeyDown(KeyCode.O))
                {
                    ChangeStatue(en_Game01_Sta.ShowLevel);
                }
                break;

            // 【展示关卡】显示关卡信息 1.5 秒后，根据玩家数切换到下一状态
            case en_Game01_Sta.ShowLevel:
                runTime += Time.deltaTime;
                if (runTime >= 1.5f)
                {
                    runTime = 0;
#if UNITY_EDITOR && false
                ChangeStatue (en_Game01_Sta.Play);
#else
                    // 多人模式先展示玩家编号，单人模式直接进入准备倒计时
                    if (playerNum > 1)
                    {
                        ChangeStatue(en_Game01_Sta.ShowPlayer);
                    }
                    else
                    {
                        ChangeStatue(en_Game01_Sta.Ready);
                    }
#endif
                }
                break;

            // 【展示玩家】显示当前玩家编号 2 秒后，进入准备倒计时
            case en_Game01_Sta.ShowPlayer:
                runTime += Time.deltaTime;
                if (runTime >= 2f)
                {
                    runTime = 0;
                    ChangeStatue(en_Game01_Sta.Ready);
                }
                break;

            // 【准备倒计时】倒计时结束后进入游戏，每秒播放倒计时音效并驱动 LED 动画
            case en_Game01_Sta.Ready:
                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    // 检测是否跨过了整数秒边界
                    if (readyTime != (int)runTime)
                    {
                        readyTime = (int)runTime;
                        gameUI.Update_ReadyTime(readyTime);
                        if (readyTime > 0)
                        {
                            // 倒计时中，每秒播放提示音
                            PlaySound(audioClip_ReadyTime);
                        }
                        // 驱动玩家 LED 动画效果
                        player.RunLedAnim();
                        // 倒计时归零，正式开始游戏
                        if (readyTime == 0)
                        {
                            ChangeStatue(en_Game01_Sta.Play);
                        }
                    }
                }
                else
                {
                    // 无需倒计时，直接开始游戏
                    ChangeStatue(en_Game01_Sta.Play);
                }
                break;

            // 【游戏进行中】核心游戏循环：检测玩家状态、管理关卡倒计时、处理超时
            case en_Game01_Sta.Play:
                // 检查玩家是否已通关或失败（状态 >= Pass 表示已有结果）
                if (player.statue >= en_Player01Sta.Pass)
                {
                    Debug.LogError("00");
                    // 演示模式直接退出，正常模式进入结果展示
                    if (Main.IsDemo)
                    {
                        ChangeStatue(en_Game01_Sta.Out);
                    }
                    else
                    {
                        ChangeStatue(en_Game01_Sta.ShowResult);
                    }
                    break;
                }
                // 玩家状态超出 Play（如处于过渡动画中），暂不处理
                if (player.statue > en_Player01Sta.Play)
                {
                    break;
                }
                // 关卡计时逻辑（限时模式）
                if (/*Main.playerMode != en_PlayerMode.Free*/true)
                {
                    if (gameTime > 0)
                    {
                        // 每帧递减游戏剩余时间
                        gameTime -= Time.deltaTime;

                        // 更新 UI 上的剩余时间显示
                        gameUI.Update_RemainTime(rTime);

                        // 检测是否跨过整数秒边界，播放倒计时警告音效
                        if (rTime != (int)gameTime)
                        {
                            rTime = (int)gameTime;
                            // 最后 5 秒播放警告音效
                            if (rTime <= 5)
                            {
                                if (rTime > 0)
                                {
                                    // 剩余 1~5 秒：播放超时警告音
                                    PlaySound(audioClip_Timeout);
                                }
                                else
                                {
                                    // 时间归零：播放时间耗尽音效
                                    PlaySound(audioClip_TimesUp);
                                }
                            }
                        }
                    }
                    else
                    {
                        // 关卡时间耗尽，演示模式退出，正常模式进入结果展示
                        if (Main.IsDemo)
                        {
                            ChangeStatue(en_Game01_Sta.Out);
                        }
                        else
                        {
                            ChangeStatue(en_Game01_Sta.ShowResult);
                        }
                        break;
                    }
                }
                // 玩家正在游戏中（预留扩展位置）
                if (player.statue == en_Player01Sta.Play)
                {
                }
                break;

            // 【显示结果】展示通关/失败结果 3 秒后，进入分数结算
            case en_Game01_Sta.ShowResult:
                runTime += Time.deltaTime;
                if (runTime >= 3)
                {
                    ChangeStatue(en_Game01_Sta.ShowResultScore);
                }
                break;

            // 【续玩确认】倒计时等待玩家投币续玩，超时或拒绝则进入分数结算
            case en_Game01_Sta.Continue:
                // 检测投币数是否变化，更新投币提示 UI
                if (coins != FjData.g_Fj[0].Coins)
                {
                    Update_CoinsTips();
                }

                // 续玩倒计时递减
                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    if (runCnt != (int)runTime)
                    {
                        runCnt = (int)runTime;
                        gameUI.Update_ContinueTime(runCnt);
                    }
                }
                // 倒计时结束或玩家选择放弃（continueResult==2），进入分数结算
                if (runTime <= 0 || gameUI.continueResult == 2)
                {
                    ChangeStatue(en_Game01_Sta.ShowResultScore);
                    break;
                }
                // 玩家确认续玩（continueResult==1）
                if (gameUI.continueResult == 1)
                {
                    // 确认续玩：扣除所需投币
                    if (Main.IsDemo == false)
                    {
                        // 尝试扣除开始游戏所需的币数
                        if (Main.DecStartCoin(0))
                        {
                            // 手环模式下更新用户游玩次数
                            if (Set.setVal.BraceletMode != 0 && Main.currUser != null)
                            {
                                Main.currUser.playCnt++;
                                UserManager.SaveData(Main.currUser.cardId, Main.currUser);
                            }
                            // 累加游玩次数统计并保存
                            FjData.acc[0].PlayCnt++;
                            FjData.totalAcc[0].PlayCnt++;
                            FjData.SaveAcc_PlayCnt(0, false);
                            FjData.SaveTotalAcc_PlayCnt(0);
                            // 执行续玩逻辑，恢复游戏
                            GameContinue();
                            break;
                        }
                    }
                }
                // 重置续玩选择结果，等待下一次输入
                gameUI.continueResult = 0;
                break;

            // 【分数结算】等待加分动画完成后，判断游戏是否结束，决定后续流程
            case en_Game01_Sta.ShowResultScore:
                // 等待分数递增动画播放完毕
                if (player.AddScoreFinish() == false)
                    break;
                // 游戏结束时播放"游戏结束"语音（仅自由模式，且只播放一次）
                if (isGameOver && Main.playerMode == en_PlayerMode.Free && runCnt == 0)
                {
                    MusicManager.instance.Play_Talk(4, 0.5f); // "游戏结束"
                    runCnt = 1; // 标记已播放，防止重复
                }
                // 等待结算展示延时
                if (runTime > 0)
                {
                    runTime -= Time.deltaTime;
                    break;
                }

                if (isGameOver)
                {
                    // 闯关模式下，显示对战胜者界面
                    if (Main.playerMode == en_PlayerMode.PassLevel) {
                       ChangeStatue (en_Game01_Sta.ShowWiner);
                    }
                    // 找出所有玩家中的最高分，作为最终记录分数
                    int maxId = 0;
                    for (int i = 0; i < playerNum && i < Main.MAX_PLAYER; i++)
                    {
                        if (FjData.g_Fj[i].Scores > FjData.g_Fj[maxId].Scores)
                        {
                            maxId = i;
                        }
                    }
                    // 将最高分记录到玩家 0 的分数上（用于排行榜）
                    FjData.g_Fj[0].Scores = FjData.g_Fj[maxId].Scores;

                    // 手环模式下，更新用户的最高分和游戏记录
                    bool hasUser = false;
                    if (Set.setVal.BraceletMode != 0 && Main.currUser != null)
                    {
                        // 更新用户历史最高分
                        if (FjData.g_Fj[0].Scores > Main.currUser.maxScore)
                        {
                            Main.currUser.maxScore = FjData.g_Fj[0].Scores;
                        }
                        // 添加本次游玩记录和分数排名
                        Main.currUser.AddUseRecordOne(gameLevel, FjData.g_Fj[0].Scores);
                        Main.currUser.AddScoresRank(gameLevel, FjData.g_Fj[0].Scores);
                        UserManager.SaveData(Main.currUser.cardId, Main.currUser);
                        gameUI.playerNameInput.playerName = Main.currUser.userName;
                        hasUser = true;
                    }
                    // 检查分数是否能上排行榜
                    if (FjData.rankList.UpList(FjData.g_Fj[0].Scores))
                    {
                        // 有注册用户则直接显示排行榜，否则先输入姓名
                        if (hasUser)
                        {
                            ChangeStatue(en_Game01_Sta.RankList);
                        }
                        else
                        {
                            ChangeStatue(en_Game01_Sta.InputName);
                        }
                        break;
                    }
                    // 分数不够上榜，直接结束
                    ChangeStatue(en_Game01_Sta.End);
                }
                else
                {
                    // 游戏未结束，等待进入下一关
                    ChangeStatue(en_Game01_Sta.WaitNextLevel);
                }
                break;

            // 【等待下一关】闪烁开始按钮 LED，倒计时结束或玩家按键后进入下一关
            case en_Game01_Sta.WaitNextLevel:
                StartButtonLed_Run();
                // 玩家按下开始按钮可跳过等待
                if (LedKey.KeyPressed(startButtonId))
                {
                    Debug.Log("等待进入下一关");
                    runTime = 0; // 立即结束等待倒计时
                }
                // 等待倒计时递减，UI 同步更新
                if (runTime > 0 && gameUI.levelStarted == false)
                {
                    runTime -= Time.deltaTime;
                    if (readyTime != (int)runTime)
                    {
                        readyTime = (int)runTime;
                        gameUI.Update_LevelWaitTime(readyTime);
                    }
                }
                else
                {
                    // 倒计时结束或玩家确认，进入下一关
                    gameLevel++;
                    // 关卡超过 10 关则重置为 0，并切换到下一个游戏模式
                    if (gameLevel >= 10)
                    {
                        gameLevel = 0;
                        Debug.LogError(Main.playerMode);
                        Main.playerMode++;
                        // 超过最高模式则循环回第一个模式
                        if (Main.playerMode > en_PlayerMode.Challenge)
                        {
                            Main.playerMode = 0;
                        }
                        // 触发关卡选择界面的重置
                        Game97_LevelSel.instance.OnClick_Level(1);
                    }
                    ChangeStatue(en_Game01_Sta.ShowLevel);
                }
                break;

            // 【单人奖励展示】展示 4 秒后，检查是否上榜并进入对应流程
            case en_Game01_Sta.ShowWins:
                runTime += Time.deltaTime;
                if (runTime >= 4)
                {
                    // 检查分数是否能上排行榜
                    if (FjData.rankList.UpList(FjData.g_Fj[0].Scores))
                    {
                        ChangeStatue(en_Game01_Sta.InputName);
                        break;
                    }
                    ChangeStatue(en_Game01_Sta.End);
                }
                break;

            // 【对战胜者展示】展示 8 秒后退出游戏
            case en_Game01_Sta.ShowWiner:
                runTime += Time.deltaTime;
                if (runTime >= 8)
                {
                    ChangeStatue(en_Game01_Sta.Out);
                }
                break;

            // 【输入姓名】等待玩家输入姓名完成后，进入排行榜展示
            case en_Game01_Sta.InputName:
                // 姓名输入面板仍处于激活状态，等待玩家完成输入
                if (gameUI.playerNameInput.gameObject.activeSelf)
                    break;
                ChangeStatue(en_Game01_Sta.RankList);
                break;

            // 【排行榜展示】展示 8 秒后退出游戏
            case en_Game01_Sta.RankList:
                runTime += Time.deltaTime;
                if (runTime >= 8)
                {
                    ChangeStatue(en_Game01_Sta.Out);
                }
                break;

            // 【结束过渡】等待 1 秒后进入退出流程
            case en_Game01_Sta.End:
                runTime += Time.deltaTime;
                if (runTime >= 1)
                {
                    ChangeStatue(en_Game01_Sta.Out);
                }
                break;

            // 【退出游戏】等待 0.3 秒后执行场景切换，返回游戏选择界面
            case en_Game01_Sta.Out:
                runTime += Time.deltaTime;
                if (runTime >= 0.3f)
                {
                    // 标记为退出完成状态
                    ChangeStatue(en_Game01_Sta.OutEnd);
                    // 切换主状态到 Game_97（游戏大厅/选关界面）
                    Main.instance.ChangeStatue(en_MainStatue.Game_97);
                    // 如果存在设置错误或游戏时间已用完，直接返回不再切换子状态
                    if (Main.settingError != en_ErrorCode.None || Main.PlayTime < 0)
                    {
                        return;
                    }
                    // 进入游戏选择界面
                    Main.instance.game97_Main.ChangeStatue(en_Game97_Sta.GameSelect);
                }
                break;
        }
    }

    public void ChangeStatue(en_Game01_Sta sta)
    {
#if UNITY_EDITOR
        Debug.Log("GameSta: " + sta);
#endif
        statue = sta;
        runTime = 0;
        runCnt = 0;
        sendTime = 0;
        setIndex = 0;
        setCount = 0;
        cmdRetSucess = false;

        //CmdIO_YDGZ.CMD0_SendCmd_GameStatue (gameId, gameLevel, (int)statue);


        //gameUI.time_Obj.SetActive (false);
        gameUI.coinIn.gameObject.SetActive(false);
        gameUI.pleaseCoin_Obj.SetActive(false);
        gameUI.tips_Obj.SetActive(false);
        gameUI.button_LevelStart.gameObject.SetActive(false);
        gameUI.showLevel_Obj.SetActive(false);
        gameUI.showPlayer_Obj.SetActive(false);
        gameUI.image_ReadyTime.gameObject.SetActive(false);
        gameUI.continue_Obj.SetActive(false);
        gameUI.levelWaitTime_Obj.SetActive(false);
        gameUI.resultWinner.gameObject.SetActive(false);
        gameUI.resultWins.gameObject.SetActive(false);
        gameUI.levelStarted = false;

        StartButtonLedOut(0);//

        switch (statue)
        {
            case en_Game01_Sta.Idle:
                gameUI.time_Obj.SetActive(false);
                player.ChangeStatue(en_Player01Sta.Idle);
                break;

            case en_Game01_Sta.Tips:
                gameUI.tips_Obj.SetActive(true);
                break;

            case en_Game01_Sta.WaitStart:
                gameUI.tips_Obj.SetActive(true);
                gameUI.button_LevelStart.gameObject.SetActive(true);
                break;

            case en_Game01_Sta.ShowLevel:
                gameUI.time_Obj.SetActive(false);
                gameUI.level_Obj.SetActive(false);
                gameUI.playerId_Obj.SetActive(false);
                gameUI.showLevel_Obj.SetActive(true);
                gameUI.Update_ShowLevel(gameLevel);

                //for (int i = 0; i < playerNum && i < player.Length; i++) {
                //    player[i].GameStart (maxLife, playerNum);
                //}

                playerId = 0;
                if (Main.playerMode == en_PlayerMode.PassLevel && gameLevel > 0)
                {
                    for (int i = 0; i < playerNum; i++)
                    {
                        if (FjData.g_Fj[i].Result > 0)
                        {
                            playerId = i;
                            break;
                        }
                    }
                }
                player.GameStart(playerId);
                //
                audioSource_BackG.Stop();
                //PlaySound (audioClip_ShowLevel);
                MusicManager.instance.Play_ShowLevel();
                break;

            case en_Game01_Sta.ShowPlayer:
                gameUI.time_Obj.SetActive(false);
                gameUI.level_Obj.SetActive(false);
                gameUI.playerId_Obj.SetActive(false);
                gameUI.showPlayer_Obj.SetActive(true);
                gameUI.Update_ShowPlayer(playerId);
                //
                player.GameStart(playerId);
                break;

            case en_Game01_Sta.Ready:
                gameUI.level_Obj.SetActive(true);
                gameUI.Update_Level(gameLevel);
                if (playerNum > 1)
                {
                    gameUI.playerId_Obj.SetActive(true);
                    gameUI.Update_PlayerId(playerId);
                }
                else
                {
                    gameUI.playerId_Obj.SetActive(false);
                }
                runTime = 4;
                readyTime = 3;
                gameUI.Update_ReadyTime(readyTime);
                PlaySound(audioClip_ReadyTime);
                break;

            case en_Game01_Sta.Play:
                gameUI.level_Obj.SetActive(true);
                gameUI.Update_Level(gameLevel);
                if (playerNum > 1)
                {
                    gameUI.playerId_Obj.SetActive(true);
                    gameUI.Update_PlayerId(playerId);
                }
                else
                {
                    gameUI.playerId_Obj.SetActive(false);
                }
                if (/*Main.playerMode != en_PlayerMode.Free*/true)
                {
                    gameUI.time_Obj.SetActive(true);
                    //if (playerNum <= 1)
                    //{
                    //    // 不显示玩家号：
                    //    gameUI.time_Obj.transform.localPosition = new Vector3(0, 0);
                    //}
                    //else if (Main.gameSetting.gameLevelSetting[gameLevel].life <= 10)
                    //{
                    //    // 1排
                    //    gameUI.time_Obj.transform.localPosition = new Vector3(0, -70);
                    //}
                    //else
                    //{
                    //    // 2排
                    //    gameUI.time_Obj.transform.localPosition = new Vector3(0, -180);
                    //}
                }

                gameTime = 0;
                gameTime = Main.gameSetting.gameLevelSetting[gameLevel].gameTime;
                //  gameTime = Set.gameSetting[bigGameLevel].gameLevelSetting[smallGameLevel].gameTime;

                gameUI.Update_RemainTime((int)gameTime);

                player.PlayStart();
                MusicManager.instance.Play_Talk(0, 1.0f); // "挑战开始"
                                                          //
                if (++bgmIndex >= MusicManager.instance.audioClip_Leishe_BGM.Length)
                {
                    bgmIndex = 0;
                }
                audioSource_BackG.Stop();
                //audioSource_BackG.clip = MusicManager.instance.audioClip_BGM[bgmIndex];
                audioSource_BackG.clip = MusicManager.instance.GetAudioClip_LeiShe_Game();
                audioSource_BackG.PlayDelayed(2);
                //
                PlaySound(audioClip_ReadyGo);
                break;

            case en_Game01_Sta.ShowResult:
                gameUI.time_Obj.SetActive(false);
                gameUI.gameTime.gameObject.SetActive(false);
                player.ShowResult();
                //
                audioSource_BackG.Stop();

                if (Main.playerMode == (int)en_PlayerMode.Free)
                {
                    if (player.result == 0)
                    {
                        PlaySound(audioClip_Loss);
                    }
                    else
                    {
                        PlaySound(audioClip_Pass);
                    }
                }
                break;

            case en_Game01_Sta.Continue:
                gameUI.continue_Obj.SetActive(true);
                if (Set.setVal.StartCoins > 0)
                {
                    gameUI.coinIn.gameObject.SetActive(true);
                }
                Update_CoinsTips();
                //
                runCnt = 20;
                runTime = runCnt + 1;
                gameUI.Update_ContinueTime(runCnt);
                gameUI.continueResult = 0;
                break;

            case en_Game01_Sta.ShowResultScore:
                gameUI.level_Obj.SetActive(false);
                gameUI.playerId_Obj.SetActive(false);
                player.ShowResultScore();

                isGameOver = false;
                runTime = 1.5f;
                NextPlayerId();
                if (IsGameOver())
                {
                    isGameOver = true;
                    runTime = 3.0f;
                }
                break;

            case en_Game01_Sta.WaitNextLevel:
                gameUI.levelWaitTime_Obj.SetActive(true);
                gameUI.button_LevelStart.gameObject.SetActive(true);
                readyTime = Set.setVal.LevelWaitTime;
                runTime = readyTime + 0.9f;
                gameUI.Update_LevelWaitTime(readyTime);

                MusicManager.instance.Play_ShowLevel();
                audioSource_BackG.PlayDelayed(1.5f);
                MusicManager.instance.Play_Talk(3, 0.5f); // "准备进入下一关"
                break;

            case en_Game01_Sta.ShowWins:
                gameUI.resultWins.gameObject.SetActive(true);
                int win = Main.JieSuanScore(playerId);
                gameUI.resultWins.Update_Result(gameLevel, FjData.g_Fj[playerId].Scores, win);
                player.ChangeStatue(en_Player01Sta.GameOver);
                MusicManager.instance.Play_Talk(4, 2); // "游戏结束"
                break;

            case en_Game01_Sta.ShowWiner:
                gameUI.playerId_Obj.SetActive(false);
                gameUI.resultWinner.gameObject.SetActive(true);
                int[] wins = new int[Main.MAX_PLAYER];
                for (int i = 0; i < wins.Length; i++)
                {
                    wins[i] = Main.JieSuanScore(i);
                }
                gameUI.resultWinner.Update_Value();
                player.ChangeStatue(en_Player01Sta.GameOver);
                MusicManager.instance.Play_Talk(4, 1.5f); // "游戏结束"
                break;

            case en_Game01_Sta.InputName:
                gameUI.playerNameInput.gameObject.SetActive(true);
                gameUI.playerNameInput.GameStart(6, null);

                audioSource_BackG.clip = MusicManager.instance.audioClip_EndEff;
                audioSource_BackG.Play();
                break;

            case en_Game01_Sta.RankList:
                gameUI.rankList.gameObject.SetActive(true);
                rankOne.playerName = gameUI.playerNameInput.GetName();
                rankOne.score = FjData.g_Fj[0].Scores;
                rankOne.level = gameLevel;
                //
                int rank = FjData.rankList.AddOne(rankOne);
                RankList.SaveRankList(Set.gameName[(int)Main.playerMode], FjData.rankList);
                //
                gameUI.rankList.UpdateValue(rank, FjData.rankList);

                if (audioSource_BackG.clip != MusicManager.instance.audioClip_EndEff)
                {
                    audioSource_BackG.clip = MusicManager.instance.audioClip_EndEff;
                    audioSource_BackG.Stop();
                }
                if (audioSource_BackG.isPlaying == false)
                {
                    audioSource_BackG.Play();
                }
                break;

            case en_Game01_Sta.End:
                break;

            case en_Game01_Sta.Out:
                break;
        }
    }

    void PlaySound(AudioClip audioClip)
    {
        audioSource_Others.Stop();
        audioSource_Others.volume = (float)Set.setVal.MainSoundVolume / 10;
        audioSource_Others.clip = audioClip;
        audioSource_Others.Play();
    }



    // -----------------------------------------------------------------------------
    void NextPlayerId()
    {
        for (; ; )
        {
            playerId++;
            if (playerId >= FjData.g_Fj.Length)
                break;
            if (playerId >= playerNum)
                break;
            if (Main.playerMode != en_PlayerMode.PassLevel)
            {
                break;
            }
            if (FjData.g_Fj[playerId].Result > 0)
            {
                break;
            }
        }
    }
    bool IsGameOver()
    {
        //if (Main.playerMode != en_PlayerMode.Free && gameLevel + 1 >= maxLevel)
        //    return true;
        // 有1个玩家失败，游戏结束
        for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
        {
            if (FjData.g_Fj[i].Result == 0)
            {
                return true;
            }
        }
        if (playerId < playerNum)
            return false;
        return false;
    }
    // 对战模式下：
    bool IsGameOver_ByBatlle()
    {
        if (gameLevel + 1 >= maxLevel)
            return true;
        if (Main.playerMode == en_PlayerMode.PassLevel)
        {
            if (playerId < playerNum)
                return false;
            // 剩余玩家少于2，游戏结束
            int remainPlayer = 0;
            for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
            {
                if (FjData.g_Fj[i].Result > 0)
                {
                    remainPlayer++;
                }
            }
            if (remainPlayer < 2)
            {
                return true;
            }
        }
        else
        {
            // 有1个玩家失败，游戏结束
            for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
            {
                if (FjData.g_Fj[i].Result == 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
    bool AllPlayerPass()
    {
        for (int i = 0; i < FjData.g_Fj.Length && i < playerNum; i++)
        {
            if (FjData.g_Fj[i].Result == 0)
            {
                return false;
            }
        }
        return true;
    }


    // 震屏效果 ----------------------------------------------------
    int shakeCnt;
    float shakeTime;
    float shakePower;
    public void ShakeStart(float power, int cnt)
    {
        shakePower = power;
        shakeCnt = cnt;
    }
    public void ShakeStop()
    {
        shakeMain_Obj.transform.localPosition = new Vector3(0, 0, 0);
        shakeCnt = 0;
    }
    public void ShakeRun()
    {
        if (shakeCnt > 0)
        {
            shakeTime += Time.deltaTime;
            if (shakeTime >= 0.05f)
            {
                shakeTime = 0;
                //
                shakeCnt--;
                if (shakeCnt == 0)
                {
                    shakeMain_Obj.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    shakeMain_Obj.transform.localPosition = new Vector3(Random.Range(-0.8f, 0.8f), Random.Range(-0.8f, 0.8f), 0) * shakePower;
                }
            }
        }
    }

    static float startButtonLedTime = 0;
    static uint startButtonLedSta = 0;

    public static void StartButtonLedOut(uint color)
    {
        Framebuffer.Update_TargetLedColor(Set.ChannelLength[4] - 2, color);
    }
    public static void StartButtonLed_Run()
    {
        startButtonLedTime += Time.deltaTime;
        if (startButtonLedTime >= 0.5f)
        {
            startButtonLedTime = 0;
            if (startButtonLedSta == 0)
            {
                startButtonLedSta = 0xa0a0a0;
            }
            else
            {
                startButtonLedSta = 0;
            }
            StartButtonLedOut(startButtonLedSta);
        }
    }

    void Update_CoinsTips()
    {
        coins = FjData.g_Fj[0].Coins;
        if (coins >= Set.setVal.StartCoins)
        {
            gameUI.pleaseCoin_Obj.SetActive(false);
        }
        else
        {
            gameUI.pleaseCoin_Obj.SetActive(true);
        }
    }


#if DEBUG_TEST
    void OnGUI()
    {
        //GUI.color = Color.yellow;

        //GUI.Label(new Rect(550, 100, 200, 20), "MonsterNum: " + monsterNum.ToString());
        //GUI.Label(new Rect(550, 130, 200, 20), "RemainNum: " + monsterRemainNum.ToString());
        //GUI.Label(new Rect(550, 160, 200, 20), "AliveNum: " + monsterAliveNum.ToString());
        ////GUI.Label(new Rect(550, 270, 200, 20), "monsterFreshPosId: " + gamePlay.monsterFreshPosId_Out.ToString());

        //GUI.Label(new Rect(550, 200, 200, 20), "BloodDcTime: " + daoJuDcTime[DAOJU_ID_BLOOD].ToString());
        //GUI.Label(new Rect(550, 220, 200, 20), "DaoDanDcTime: " + daoJuDcTime[DAOJU_ID_DAODAN].ToString());
    }
#endif

    public void CheckPass()
    {


        Framebuffer.Update_TargetLedColor(0, 0xff0000);

      

         
    }
}
