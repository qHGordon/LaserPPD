# Game01 技术开发文档

## 1. 模块概述

### 1.1 玩法说明

Game01 是一款**激光镭射交互游戏**，核心玩法包括：

- **目标点（Target）与危险点（Die）机制**：
  - 玩家需要踩踏/按键触发**蓝色目标点**获取分数（+10分）
  - 避免触发**红色危险点**，否则扣分（-5分）
  - 通过**墙灯（WallLED）**按键击打目标墙灯获取分数（+5分）

- **激光接收检测**：
  - 游戏过程中，LED 点阵会显示预设图案或动态移动点位
  - 玩家需要使用激光枪照射这些点位
  - 当激光接收器**未检测到激光**（`LedKey.KeyStatus == false`）且超过保护时间时，会触发扣血逻辑

- **阶段（JieDuan）系统**：
  - 游戏分为多个阶段（`currJieDuan: 0-3`）
  - 每个阶段有不同的目标点分布和玩法
  - 完成所有阶段后判定为通过（Pass），否则失败（Loss）

- **地图系统**：
  - 支持 12 种不同的地图配置（`Initmap_Maze_00` ~ `Initmap_Maze_11`）
  - 地图中包含动态移动的 LED 点位（`Game01_Ledone`），具有多种移动模式（Stay、Shake、OneWay、Down、Up）

### 1.2 核心入口脚本

**核心入口脚本：`Game01_Main.cs`**

- 位置：`Assets/Scripts/Game/Game01/Game01_Main.cs`
- 继承：`MonoBehaviour`
- 初始化流程：
  1. `Main.cs` 中通过 `Resources.Load<GameObject>(companyPath + "Prefabs/Game01_" + tab_Language[language])` 加载预制体
  2. 调用 `Game01_Main.Awake0(Main mainn)` 进行初始化
  3. 调用 `Game01_Main.GameStart()` 开始游戏流程

---

## 2. 核心类结构 (Class Structure)

### 2.1 主要脚本列表

#### 核心逻辑脚本

| 脚本名称 | 路径 | 职责说明 |
|---------|------|---------|
| `Game01_Main` | `Assets/Scripts/Game/Game01/Game01_Main.cs` | 主控制器，管理游戏状态机、流程切换、UI调度、计时与结算 |
| `Game01_Player` | `Assets/Scripts/Game/Game01/Game01_Player.cs` | 玩家逻辑控制器，管理玩家状态机、目标点生成、激光/踩踏检测、扣分扣血逻辑 |
| `Game_Map01` | `Assets/Scripts/Game/Game_Map/Game_Map01.cs` | 地图初始化与动态LED生成，创建 `Game01_Ledone` 列表 |
| `Game01_Ledone` | `Assets/Scripts/Game/Game_Map/Game01_Ledone.cs` | 单个移动LED点逻辑，结合激光检测触发扣血 |

#### LED/激光底层脚本

| 脚本名称 | 路径 | 职责说明 |
|---------|------|---------|
| `GameLeiSheBase` | `Assets/Scripts/Game/Game01/GameLeiSheBase.cs` | 维护 `GamePoint` 数组，封装LED点位颜色更新（传输LED） |
| `GameLeiSheLedControl` | `Assets/Scripts/Game/Game01/GameLeiSheLedControl.cs` | LED动画/预设图逻辑，管理点阵动画与预设图案绘制 |

#### UI 相关脚本

| 脚本名称 | 路径 | 职责说明 |
|---------|------|---------|
| `Game01_GameUI` | `Assets/Scripts/Game/Game01/Game01_GameUI.cs` | 主UI控制器：倒计时、关卡显示、继续、结算、排行榜 |
| `Game01_PlayerUI` | `Assets/Scripts/Game/Game01/Game01_PlayerUI.cs` | 玩家UI显示（分数、生命、目标点等） |
| `Game01_ResultScore` | `Assets/Scripts/Game/Game01/Game01_ResultScore.cs` | 结果分数显示 |
| `Game01_ResultWinner` | `Assets/Scripts/Game/Game01/Game01_ResultWinner.cs` | 获胜者显示（对战模式） |
| `Game01_ResultWins` | `Assets/Scripts/Game/Game01/Game01_ResultWins.cs` | 胜利结算（单人模式） |
| `Game01_ResultOne` | `Assets/Scripts/Game/Game01/Game01_ResultOne.cs` | 单个结果项 |
| `Game01_LifeOne` | `Assets/Scripts/Game/Game01/Game01_LifeOne.cs` | 生命值显示组件 |
| `Game01_GameUIComm` | `Assets/Scripts/Game/Game01/Game01_GameUIComm.cs` | 通用UI组件 |

### 2.2 继承关系说明

**所有核心脚本均直接继承自 `MonoBehaviour`**，未发现继承 `Main` 或 `GameComm` 的结构。

- `Game01_Main` → `MonoBehaviour`
- `Game01_Player` → `MonoBehaviour`
- `Game_Map01` → `MonoBehaviour`
- `Game01_Ledone` → `MonoBehaviour`
- `GameLeiSheBase` → 普通类（非 MonoBehaviour）
- `GameLeiSheLedControl` → 普通类（非 MonoBehaviour）

### 2.3 关键方法说明

#### Game01_Main 关键方法

- `GameStart()`：游戏启动入口，初始化地图、玩家、统计
- `Update()`：主循环，驱动状态机更新
- `ChangeStatue(en_Game01_Sta sta)`：状态切换，处理各状态逻辑
- `StartButtonLed_Run()`：开始按钮LED闪烁
- `ShakeStart()` / `ShakeStop()` / `ShakeRun()`：震屏效果

#### Game01_Player 关键方法

- `GameStart(int no)`：玩家初始化
- `PlayStart()`：开始游戏流程
- `ChangeStatue(en_Player01Sta sta)`：玩家状态切换
- `CheckRxKey()`：检查激光接收键（激光输入检测）
- `CheckLedKey(int ch)`：检查LED按键（踩踏输入检测）
- `CheckWallLedKey()`：检查墙面LED按键
- `GetRandom_Tarage()`：随机生成目标点
- `GetOtherLed()`：生成危险点

---

## 3. 核心流程图解 (Logic Flow)

### 3.1 游戏如何开始 (GameStart)

```
Main.ChangeStatue(en_MainStatue.Game_01)
    ↓
加载预制体 Resources/Prefabs/Game01_CN.prefab 或 Game01_EN.prefab
    ↓
Game01_Main.Awake0(Main mainn)
    ├─ instance = this
    ├─ player.Awake0(this)
    └─ gameUI.Awake0()
    ↓
Game01_Main.GameStart()
    ├─ Game_Map01.instance.Initmap()  // 初始化地图
    ├─ player.GameStart(0)            // 初始化玩家
    ├─ gameUI.GameStart()             // 初始化UI
    ├─ 重置统计数据（Scores、Result）
    └─ ChangeStatue(en_Game01_Sta.Tips)  // 进入提示状态
```

**关键初始化步骤：**

1. **地图初始化** (`Game_Map01.Initmap()`)：
   - 根据 `Main.MapIndex % 12` 选择地图（00-11）
   - 创建动态移动的 LED 点位（`Game01_Ledone`）
   - 初始化保护时间 `protectTime = 5`

2. **玩家初始化** (`Game01_Player.GameStart(0)`)：
   - 设置生命值 `FjData.g_Fj[Id].Life = 100`（硬编码）
   - 初始化激光接收键ID：`rxKeyStartId = LedKey.GetLeiSheKeyStartId()`
   - 初始化目标按键ID：`targetKeyId = LedKey.GetLeiSheTargetKeyId()`
   - 初始化墙灯按键ID：`wallKeyId = LedKey.GetLeiSheWallLedKeyId()`
   - 初始化LED控制：`ledControl.LedInit()`

### 3.2 游戏循环 (Update / Play 状态)

#### Game01_Main 主状态机流程

```
Tips (2秒)
    ↓
WaitStart (等待开始按钮)
    ↓
ShowLevel (1.5秒)
    ↓
ShowPlayer (2秒，多人模式)
    ↓
Ready (倒计时 3-2-1)
    ↓
Play (主游戏循环)
    ├─ 倒计时 gameTime 递减
    ├─ 检测玩家状态（Pass/Loss）
    └─ 时间到或玩家结束 → ShowResult
    ↓
ShowResult (3秒)
    ↓
ShowResultScore (加分动画)
    ↓
WaitNextLevel / ShowWins / ShowWiner / RankList / End
    ↓
Out → 返回 Main.Game_97
```

#### Game01_Player 玩家状态机流程

```
Idle
    ↓
ReadyTargetLed (准备目标灯，逐列点亮)
    ↓
PlayTargetLed (击打目标灯阶段)
    ├─ CheckRxKey()  // 检测激光
    ├─ CheckWallLedKey()  // 检测墙灯
    └─ TargetLed <= 0 → ReadyPlayLed
    ↓
ReadyPlayLed (准备游戏灯，播放预设图案)
    ├─ CheckRxKey()
    └─ 图案播放完成 → Play
    ↓
Play (主游戏阶段)
    ├─ currJieDuan = 0: 击打通道4的目标点（10个）
    ├─ currJieDuan = 1: 击打目标按键
    ├─ currJieDuan = 2: 击打通道5的目标点（10个）
    ├─ currJieDuan = 3: 击打目标按键
    ├─ CheckLedKey()  // 检测踩踏输入
    ├─ CheckRxKey()   // 检测激光接收
    └─ 完成所有阶段 → Pass，否则 → Loss
    ↓
WaitPass (阶段完成等待，清屏动画)
    ↓
Pass / Loss
    ↓
ResultScore (加分动画)
```

**Play 状态详细逻辑：**

- **阶段 0** (`currJieDuan = 0`)：
  - 在通道4（`ch = 4`）随机生成10个蓝色目标点
  - 其他点位设为红色危险点
  - 玩家需要踩踏蓝色目标点，全部完成后进入阶段1

- **阶段 1** (`currJieDuan = 1`)：
  - 检测目标按键按下（`targetKeyId`）
  - 按下后进入阶段2

- **阶段 2** (`currJieDuan = 2`)：
  - 在通道5（`ch = 5`）随机生成10个蓝色目标点
  - 其他点位设为红色危险点
  - 全部完成后进入阶段3

- **阶段 3** (`currJieDuan = 3`)：
  - 检测目标按键按下（`targetKeyId`）
  - 按下后判定为通过（Pass）

### 3.3 游戏如何判定输赢 (HitTarget / HitDie)

#### 目标点判定（HitTarget）

**触发位置：** `Game01_Player.CheckLedKey(int ch)`

```csharp
if (LedKey.KeyPressed(id))  // 检测踩踏输入
{
    if (Framebuffer.led[id].statue == enPointSta.Target)
    {
        // 命中目标点
        FjData.g_Fj[0].Scores += 10;  // 加分
        MusicManager.instance.Play_Correct();
        tarageNum--;  // 目标点数量减1
    }
}
```

#### 危险点判定（HitDie）

**触发位置：** `Game01_Player.CheckLedKey(int ch)`

```csharp
if (Framebuffer.led[id].statue == enPointSta.Die)
{
    if (errorCD <= 0)  // 错误冷却时间
    {
        // 扣分（注意：扣血逻辑被注释掉了）
        FjData.g_Fj[0].Scores -= 5;
        errorCD = 1;  // 设置冷却
        MusicManager.instance.Play_Fails();
    }
}
```

#### 激光接收判定（扣血逻辑）

**触发位置：** `Game01_Player.CheckRxKey()`

```csharp
// 检测激光接收状态
if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID) == false && statue == en_Player01Sta.Play)
{
    protectTime[JieShou_ID] -= Time.deltaTime;  // 保护时间递减
    
    if (protectTime[JieShou_ID] <= 0)
    {
        // 保护时间耗尽，触发扣血
        // 注意：实际扣血代码被注释掉了（#if !UNITY_EDITOR）
        GameLeiSheBase.gamePoint[i].bindCnt = 10;  // 闪烁反馈
        MusicManager.instance.Play_Fails();
    }
}
```

#### 移动点位扣血（Game01_Ledone）

**触发位置：** `Game01_Ledone.Moving()`

```csharp
// 在移动点位上检测激光
if (LedKey.KeyStatus(startId + Framebuffer.MappingId(x, y)) == false && 
    Game01_Main.instance.statue == en_Game01_Sta.Play)
{
    if (Game_Map01.instance.protectTime <= 0)
    {
        FjData.g_Fj[0].Life--;  // 扣血（仅在非编辑器模式）
        Game_Map01.instance.protectTime = 3f;  // 重置保护时间
    }
}
```

#### 地面扣血（Game_Map01）

**触发位置：** `Game_Map01.Update()`（仅在阶段0和2）

```csharp
// 检测地面第一列（i==0）的激光接收状态
if (LedKey.KeyStatus(startId + Framebuffer.MappingId(i, k)) == false)
{
    if (protectTime <= 0)
    {
        FjData.g_Fj[0].Life--;  // 扣血
        protectTime = 1;  // 重置保护时间
    }
}
```

### 3.4 游戏如何结束

#### 正常结束流程

```
Play 状态中：
    ├─ 玩家状态变为 Pass → ShowResult
    ├─ 玩家状态变为 Loss → ShowResult
    └─ 时间耗尽（gameTime <= 0）→ ShowResult
    ↓
ShowResult (3秒)
    ↓
ShowResultScore (加分动画)
    ├─ 如果游戏结束（isGameOver）：
    │   ├─ 上榜 → InputName → RankList
    │   └─ 未上榜 → End
    └─ 如果未结束：
        └─ WaitNextLevel → ShowLevel（下一关）
    ↓
End (1秒)
    ↓
Out (0.3秒)
    ↓
返回 Main.Game_97
```

#### 失败判定

- **生命值归零**：`FjData.g_Fj[Id].Life <= 0` → `Die` → `Loss`
- **时间耗尽**：`gameTime <= 0` → `ShowResult`
- **未完成所有阶段**：`currJieDuan < maxJieDuan` 且游戏结束 → `Loss`

#### 通过判定

- **完成所有阶段**：`currJieDuan >= maxJieDuan` 且完成最后阶段 → `Pass`
- **剩余点数归零**：`FjData.g_Fj[Id].RemainPoint <= 0` → `Pass`（但代码中 `RemainPoint` 固定为1）

---

## 4. 硬件与输入交互

### 4.1 激光接收输入处理

#### 激光接收键ID获取

```csharp
// Game01_Player.GameStart()
rxKeyStartId = LedKey.GetLeiSheKeyStartId();  // 获取激光接收键起始ID
```

#### 激光接收状态检测

**主要检测方法：** `Game01_Player.CheckRxKey()`

```csharp
// 遍历所有LED点位
for (int i = 0; i < len && i < GameLeiSheBase.gamePoint.Length; i++)
{
    // 计算接收键ID
    int JieShou_ID = i;
    if (Framebuffer.isNewLeiShe)
    {
        JieShou_ID = GameLeiSheBase.tab_Point[i];  // 新镭射系统使用映射表
    }
    
    // 检测激光接收状态
    if (LedKey.KeyStatus(rxKeyStartId + JieShou_ID) == false)
    {
        // 未检测到激光，保护时间递减
        protectTime[JieShou_ID] -= Time.deltaTime;
        
        if (protectTime[JieShou_ID] <= 0)
        {
            // 触发扣血逻辑（实际代码被注释）
            GameLeiSheBase.gamePoint[i].bindCnt = 10;  // 闪烁反馈
        }
    }
    else
    {
        // 检测到激光，重置保护时间
        protectTime[JieShou_ID] = 2f - 0.02f * Set.setVal.LeiShe_LMD;
    }
}
```

**保护时间计算：**
- 初始值：`protectTime[i] = 2f - 0.02f * Set.setVal.LeiShe_LMD`
- `LeiShe_LMD` 越大，保护时间越短

#### 移动点位激光检测

**位置：** `Game01_Ledone.Moving()`

```csharp
// 检测移动点位上的激光接收状态
if (LedKey.KeyStatus(startId + Framebuffer.MappingId(pos_group[i].x, pos_group[i].y)) == false)
{
    // 未检测到激光，触发扣血
    if (Game_Map01.instance.protectTime <= 0)
    {
        FjData.g_Fj[0].Life--;
        Game_Map01.instance.protectTime = 3f;
    }
}
```

### 4.2 踩踏/按键输入处理

#### 开始按钮检测

**位置：** `Game01_Main.Update()` - `WaitStart` 状态

```csharp
startButtonId = LedKey.GetLeiSheStartButtonId();

if (LedKey.KeyPressed(startButtonId) || gameUI.levelStarted || Input.GetKeyDown(KeyCode.O))
{
    ChangeStatue(en_Game01_Sta.ShowLevel);
}
```

#### LED按键检测（踩踏输入）

**主要检测方法：** `Game01_Player.CheckLedKey(int ch)`

```csharp
// 遍历PPD（点阵）区域
for (int x = 0; x < Set.setVal.PPDWidth; x++)
{
    for (int y = 0; y < Set.setVal.PPDHeight; y++)
    {
        int id = x + Set.setVal.PPDWidth * y;
        int pointId = id;
        
        // 计算通道偏移
        for (int m = 0; m < ch; m++)
        {
            id += Set.ChannelLength[m];
        }
        
        // 检测按键按下
        if (LedKey.KeyPressed(id))
        {
            // 判断点位类型并处理
            if (Framebuffer.led[id].statue == enPointSta.Target)
            {
                // 目标点：加分
            }
            else if (Framebuffer.led[id].statue == enPointSta.Die)
            {
                // 危险点：扣分
            }
        }
    }
}
```

#### 目标按键检测

**位置：** `Game01_Player.Update()` - `Play` 状态（阶段1和3）

```csharp
// 阶段1：最后一个目标按键
targetKeyId = (Set.ChannelLength[0] + ... + Set.ChannelLength[4]) - 1;

// 阶段3：倒数第二个目标按键
targetKeyId = (Set.ChannelLength[0] + ... + Set.ChannelLength[4]) - 2;

if (LedKey.KeyPressed(targetKeyId) || Input.GetKeyDown(KeyCode.Q))
{
    // 进入下一阶段
}
```

#### 墙灯按键检测

**位置：** `Game01_Player.CheckWallLedKey()`

```csharp
wallKeyId = LedKey.GetLeiSheWallLedKeyId();

for (int i = 0; i < Set.setVal.WallLedNum; i++)
{
    if (LedKey.KeyStatus(wallKeyId + i) == false)
        continue;
    
    // 击打墙灯，扣减目标灯数量并加分
    FjData.g_Fj[Id].TargetLed--;
    FjData.g_Fj[0].Scores += 5;
}
```

### 4.3 LED硬件协议接口

#### 传输LED更新

**主要接口：** `Framebuffer.Update_TransmitLedColor()`

```csharp
// 全屏更新
GameLeiSheBase.Update_ColorFull(uint color, enPointSta sta);

// 单点更新
GameLeiSheBase.Update_PointColor(int id, uint color, enPointSta sta);
GameLeiSheBase.Update_PointColor(int x, int y, uint color, enPointSta sta);

// 直接调用 Framebuffer
Framebuffer.Update_TransmitLedColor(int id, uint color, enPointSta sta);
Framebuffer.Update_TransmitLedColor(int x, int y, uint color, enPointSta sta);
```

#### 目标LED更新

**主要接口：** `Framebuffer.Update_TargetLedColor()`

```csharp
// 全屏更新
GameLeiSheBase.Update_TargetLedColorAll(uint color, enPointSta sta);

// 单点更新
GameLeiSheBase.Update_TargetLedColor(int id, uint color, enPointSta sta);
Framebuffer.Update_TargetLedColor(int ch, int id, uint color);
```

#### 坐标映射

```csharp
// 坐标到LED ID映射
int id = Framebuffer.MappingId(x, y);
int id = Framebuffer.tab_Mapping[picId];  // 使用映射表
```

### 4.4 特殊硬件协议处理

#### 枪械输出控制

```csharp
// Game01_Player.ChangeStatue()
IO.GunOut(0);  // 关闭枪械输出
IO.GunOut(1);  // 开启枪械输出（在 PlayTargetLed 状态）
```

#### 开始按钮LED闪烁

**位置：** `Game01_Main.StartButtonLed_Run()`

```csharp
// 每0.5秒切换一次颜色
startButtonLedTime += Time.deltaTime;
if (startButtonLedTime >= 0.5f)
{
    startButtonLedTime = 0;
    startButtonLedSta = (startButtonLedSta == 0) ? 0xa0a0a0 : 0;
    Framebuffer.Update_TargetLedColor(Set.ChannelLength[4] - 2, startButtonLedSta);
}
```

#### 目标按键LED闪烁

**位置：** `Game01_Player.TargetLed_Run()`

```csharp
// 每0.5秒切换一次颜色
targetLedTime += Time.deltaTime;
if (targetLedTime >= 0.5f)
{
    targetLedTime = 0;
    targetLedSta = (targetLedSta == 0) ? 0xa0a0a0 : 0;
    TargetLedOut(targetLedSta);  // 根据阶段选择不同的LED位置
}
```

---

## 5. 注意事项

### 5.1 魔术数字与硬编码

#### Game01_Main 中的硬编码

| 位置 | 数值 | 说明 | 建议 |
|------|------|------|------|
| `GameStart()` | `playerNum = 1` | 固定为单人模式 | 应使用配置或 `Main.playerNum` |
| `Update()` - Tips | `2f` | 提示显示时间 | 建议提取为常量 |
| `Update()` - ShowLevel | `1.5f` | 关卡显示时间 | 建议提取为常量 |
| `Update()` - ShowPlayer | `2f` | 玩家显示时间 | 建议提取为常量 |
| `ChangeStatue()` - Ready | `4` | 准备阶段总时长 | 建议提取为常量 |
| `ChangeStatue()` - Ready | `3` | readyTime初始值 | 建议提取为常量 |
| `Update()` - ShowResult | `3` | 结果显示等待时间 | 建议提取为常量 |
| `Update()` - ShowWins | `4` | ShowWins显示时间 | 建议提取为常量 |
| `Update()` - ShowWiner | `8` | ShowWiner显示时间 | 建议提取为常量 |
| `Update()` - RankList | `8` | RankList显示时间 | 建议提取为常量 |
| `Update()` - End | `1` | End状态等待时间 | 建议提取为常量 |
| `Update()` - Out | `0.3f` | Out状态等待时间 | 建议提取为常量 |
| `WaitNextLevel` | `gameLevel >= 10` | 关卡上限硬编码 | 应使用 `maxLevel` |

#### Game01_Player 中的硬编码

| 位置 | 数值 | 说明 | 建议 |
|------|------|------|------|
| `GameStart()` | `Life = 100` | 生命值硬编码 | 应使用 `Main.gameSetting.gameLevelSetting[gameLevel].life` |
| `GameStart()` | `RemainPoint = 1` | 剩余点数硬编码 | 应使用配置 |
| `GameStart()` | `maxJieDuan = 4` | 最大阶段数硬编码 | 应使用配置 |
| `PlayStart()` | `2f - 0.02f * Set.setVal.LeiShe_LMD` | 保护时间计算 | 建议提取为方法 |
| `Update()` - ReadyTargetLed | `0.03f` | 目标LED更新间隔 | 建议提取为常量 |
| `Update()` - PlayTargetLed | `3` | 目标LED显示超时时间 | 建议提取为常量 |
| `Update()` - ReadyPlayLed | `0.03f` | 游戏LED更新间隔 | 建议提取为常量 |
| `Update()` - Play | `0.2f` | Play状态初始延迟 | 建议提取为常量 |
| `Update()` - Play | `3` | 保护时间（多处） | 建议提取为常量 |
| `Update()` - WaitPass | `0.5f` | WaitPass状态等待时间 | 建议提取为常量 |
| `Update()` - Die | `0.5f` | Die状态等待时间 | 建议提取为常量 |
| `CheckLedKey()` | `10` | 击中目标得分 | 建议提取为常量 |
| `CheckLedKey()` | `5` | 错误扣分 | 建议提取为常量 |
| `CheckLedKey()` | `1` | 错误冷却时间 | 建议提取为常量 |
| `GetRandom_Tarage()` | `300` | 最大尝试次数 | 建议提取为常量 |
| `GetRandom_Tarage()` | `10` | 随机目标数量 | 应使用参数或配置 |
| `InitMap_FindSame()` | `20` | FindSame模式目标数量 | 建议提取为常量 |
| `ShowTargetLed()` | `Random.Range(3, 6)` | 目标LED数量范围 | 建议提取为常量 |
| `ShowTargetLed()` | `63` | 目标LED亮度值 | 建议提取为常量 |
| `CheckRxKey()` | `JieShou_ID == 79` | 特殊LED跳过检测 | **风险点**：可能跳过关键检测 |

#### Game_Map01 中的硬编码

| 位置 | 数值 | 说明 | 建议 |
|------|------|------|------|
| `Initmap()` | `Main.MapIndex % 12` | 地图数量硬编码 | 如果地图数量变化会出错 |
| `Initmap()` | `protectTime = 5` | 初始保护时间 | 建议提取为常量 |
| `Update()` | `0.02f` | 清屏更新间隔 | 建议提取为常量 |
| `Update()` | `1` | 地面扣血保护时间 | 建议提取为常量 |

#### Game01_Ledone 中的硬编码

| 位置 | 数值 | 说明 | 建议 |
|------|------|------|------|
| `Init()` | `protectTime = 4` | 保护时间初始化 | 建议提取为常量 |
| `Moving()` | `0.4f` | 保护时间 | 建议提取为常量 |
| `Moving()` | `1f` | 停留时间 | 建议提取为常量 |
| `Moving()` | `3f` | 保护时间 | 建议提取为常量 |

### 5.2 潜在问题与风险点

#### 严重问题

1. **扣血逻辑被注释**
   - **位置：** `Game01_Player.CheckRxKey()` 第973行
   - **问题：** `FjData.g_Fj[0].Life--` 被 `#if !UNITY_EDITOR` 包裹，实际未执行
   - **影响：** 激光接收检测不会真正扣血，可能导致玩法不一致
   - **建议：** 确认是否需要启用扣血逻辑

2. **特殊LED跳过检测**
   - **位置：** `Game01_Player.CheckRxKey()` 第966行
   - **问题：** `if (JieShou_ID == 79) return;` 直接跳过特定LED检测
   - **影响：** 可能导致该LED点位无法触发扣血
   - **建议：** 确认是否为特殊设计，否则应移除

3. **硬编码条件判断**
   - **位置：** `Game01_Main.Update()` 第303行、`Game01_Player` 多处
   - **问题：** `if (/*Main.playerMode != en_PlayerMode.Free*/true)` 硬编码为 `true`
   - **影响：** 无法根据玩家模式调整逻辑
   - **建议：** 恢复条件判断或明确注释原因

4. **地图数量硬编码**
   - **位置：** `Game_Map01.Initmap()` 第36行
   - **问题：** `Main.MapIndex % 12` 硬编码地图数量
   - **影响：** 如果地图数量变化（增加或减少），会导致地图选择错误
   - **建议：** 使用动态地图数量或配置

#### 代码质量问题

1. **乱码字符**
   - **位置：** `Game01_Ledone.cs` 第215行、`Game_Map01.cs` 第494行
   - **问题：** Debug日志包含乱码字符（`"¿ÛÑª×ø±ê£º"`、`"地面扣血坐标："`）
   - **建议：** 修正编码或使用英文日志

2. **注释掉的代码路径**
   - **位置：** 多处存在注释掉的代码
   - **问题：** 影响代码可读性和维护性
   - **建议：** 清理无用注释或使用版本控制管理

3. **语法错误**
   - **位置：** `Game01_Ledone.cs` 第31行
   - **问题：** `protectTime = 4     ;` 多余空格
   - **建议：** 修正格式

4. **数组大小硬编码**
   - **位置：** `Game01_Player.cs` 第656行
   - **问题：** `int cnt = 300` 可能与实际LED数量不匹配
   - **建议：** 使用 `Set.setVal.Width * Set.setVal.Height` 或配置

#### 逻辑风险

1. **状态机耦合度高**
   - **问题：** `Game01_Player.currJieDuan` 依赖 `Game01_Main.instance.index_JieDuan`
   - **影响：** 状态管理分散，容易出现不一致
   - **建议：** 统一状态管理或明确职责划分

2. **资源加载风险**
   - **位置：** `Main.cs` 第530行
   - **问题：** `Resources.Load` 如果路径不存在会导致空引用异常
   - **建议：** 添加空值检查

3. **UI资源路径依赖**
   - **位置：** `Game01_GameUI.cs` 第57行等
   - **问题：** `Resources.LoadAll<Sprite>("UI/ReadyTime")` 依赖路径存在
   - **建议：** 添加资源存在性检查

### 5.3 待优化项

1. **配置化改造**
   - 将所有硬编码的数值提取为配置或常量
   - 使用 `ScriptableObject` 或配置文件管理游戏参数

2. **代码重构**
   - 统一状态管理，减少耦合
   - 清理注释掉的代码
   - 修正编码问题

3. **性能优化**
   - `CheckRxKey()` 和 `CheckLedKey()` 每帧遍历所有点位，可考虑优化
   - 使用对象池管理 `Game01_Ledone` 实例

4. **测试覆盖**
   - 添加单元测试覆盖核心逻辑
   - 添加集成测试验证硬件交互

---

## 6. 相关资源与预制体

### 6.1 预制体文件

- `Assets/Resources/Prefabs/Game01_CN.prefab` - 中文版预制体
- `Assets/Resources/Prefabs/Game01_EN.prefab` - 英文版预制体
- `Assets/Resources/Company_04/Prefabs/Game01_CN.prefab` - 公司04中文版
- `Assets/Resources/Company_04/Prefabs/Game01_EN.prefab` - 公司04英文版
- `Assets/Resources/Company_04/Prefabs/Game01_EN1.prefab` - 公司04英文版变体

### 6.2 UI资源路径

- `Resources.LoadAll<Sprite>("UI/ReadyTime")` - 倒计时数字图片
- `Resources.LoadAll<Sprite>("UI/Result_" + tab_Language[language])` - 结果图片
- `Resources.LoadAll<Sprite>("UI/Winner_CN")` - 获胜者图片

### 6.3 预制体引用

- `lifeOne_Prefab` - 生命值预制体引用（`Game01_Main` 第37行）

---

## 7. 总结

Game01 是一个复杂的激光镭射交互游戏，涉及多个状态机、硬件交互和动态地图系统。核心玩法是通过踩踏目标点和激光照射完成游戏目标。

**关键要点：**
- 主控制器 `Game01_Main` 管理整体流程
- 玩家控制器 `Game01_Player` 管理玩家状态和输入检测
- 地图系统 `Game_Map01` 和 `Game01_Ledone` 提供动态点位
- 硬件交互通过 `LedKey` 和 `Framebuffer` 接口实现

**维护建议：**
- 优先解决扣血逻辑被注释的问题
- 提取硬编码数值为配置
- 清理注释掉的代码和乱码字符
- 添加资源加载的空值检查

---

**文档版本：** 1.0  
**最后更新：** 2026-02-07  
**维护者：** 技术团队
