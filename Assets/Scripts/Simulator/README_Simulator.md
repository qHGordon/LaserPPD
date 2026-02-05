# 硬件模拟器使用指南

## 概述

硬件模拟器用于在没有真实硬件的情况下测试游戏流程和灯光效果。它提供了：
- ✅ 模拟硬件连接状态
- ✅ 可视化 LED 矩阵
- ✅ 鼠标点击模拟输入

## 文件说明

- **SimulatorManager.cs**: 模拟器核心逻辑，管理连接状态和心跳
- **SimulatorUI.cs**: 可视化界面，显示 LED 矩阵并处理输入

## Unity 场景设置步骤

### 1. 创建模拟器管理器

1. 在场景中创建一个空的 GameObject，命名为 `SimulatorManager`
2. 添加 `SimulatorManager` 组件
3. 在 Inspector 中配置：
   - ✅ **Enable Simulator**: 勾选以启用模拟器
   - **Heartbeat Interval**: 心跳间隔（默认 1.0 秒）
   - **Force Connected**: 强制连接状态（默认 true）

### 2. 创建模拟器 UI

#### 方法 A：使用现有 Canvas（推荐）

1. 找到场景中的主 Canvas（通常是 `gameUI_Panel`）
2. 在 Canvas 下创建一个空的 GameObject，命名为 `SimulatorUI`
3. 添加 `SimulatorUI` 组件
4. 配置组件：

**UI 设置**：
- **Grid Container**: 留空（会自动创建）或手动指定一个 RectTransform
- **LED Cell Prefab**: 
  - 可选：创建一个预制体（Image 组件）
  - 如果留空，会自动创建默认格子
- **Cell Size**: 20（像素）
- **Cell Spacing**: 2（像素）

**颜色设置**：
- **Default Color**: 默认颜色（未点亮）
- **Click Highlight Color**: 点击高亮颜色
- **Highlight Duration**: 0.2（秒）

**输入模拟设置**：
- ✅ **Enable Input Simulation**: 勾选以启用鼠标点击输入
- **Click Signal Value**: 1（按下信号值）
- **Laser Touch**: 拖入场景中的 `LaserTouch` 组件引用

#### 方法 B：创建独立 Canvas

1. 创建新的 Canvas：
   - `GameObject` → `UI` → `Canvas`
   - 命名为 `SimulatorCanvas`
   - 设置 `Render Mode` 为 `Screen Space - Overlay`
   - 设置 `Sort Order` 为 100（确保在最上层）

2. 在 Canvas 下创建 `SimulatorUI` GameObject，添加 `SimulatorUI` 组件

3. 配置组件（同方法 A）

### 3. 创建 LED 格子预制体（可选）

如果需要自定义 LED 格子外观：

1. 创建预制体：
   - `GameObject` → `UI` → `Image`
   - 命名为 `LEDCellPrefab`
   - 设置 `Color` 为深灰色（例如 RGB: 25, 25, 25）
   - 可选：添加 `Outline` 组件（边框效果）

2. 将预制体保存到 `Assets/Prefabs/` 目录

3. 在 `SimulatorUI` 组件的 `LED Cell Prefab` 字段中拖入该预制体

## 使用说明

### 启动模拟器

1. 确保 `SimulatorManager` 的 `Enable Simulator` 已勾选
2. 运行游戏，模拟器会自动：
   - 强制设置硬件连接状态为 `true`
   - 开始模拟心跳包
   - 初始化 UI 网格

### 查看 LED 效果

- 当游戏发送 LED 数据时，对应的 UI 格子会改变颜色
- 颜色值根据游戏发送的 RGB 数据自动转换

### 模拟输入

1. 确保 `SimulatorUI` 的 `Enable Input Simulation` 已勾选
2. 确保 `Laser Touch` 引用已设置
3. 点击 UI 网格上的任意格子
4. 系统会：
   - 计算对应的 LED 索引
   - 调用 `LaserTouch.UpdateSignalsInput()` 发送信号
   - 显示点击高亮效果

## 调试技巧

### 查看日志

模拟器会在 Console 中输出调试信息：
- `[硬件模拟器]` - SimulatorManager 日志
- `[硬件模拟器UI]` - SimulatorUI 日志

### 常见问题

**Q: UI 网格没有显示**
- 检查 `Grid Container` 是否已创建
- 检查 Canvas 的 `Sort Order` 是否足够高
- 检查 `Cell Size` 和 `Cell Spacing` 是否合理

**Q: 点击没有反应**
- 检查 `Enable Input Simulation` 是否勾选
- 检查 `Laser Touch` 引用是否设置
- 检查 EventSystem 是否存在（UI 点击需要）

**Q: LED 颜色不对**
- 检查颜色转换逻辑（uint → Color32）
- 检查通道映射是否正确

**Q: 连接状态仍然为 false**
- 确保 `SimulatorManager` 的 `Force Connected` 已勾选
- 检查 `Enable Simulator` 是否启用
- 检查 `#if IO_YDGZ` 编译条件是否满足

## 技术细节

### 事件系统

`CmdIO_YDGZ` 已添加 `OnLedDataSent` 事件：
```csharp
public static event Action<int, uint, uint[], int> OnLedDataSent;
```

当调用 `CMD0_SendCmd_LedOne()` 或 `CMD0_SendCmd_LedAll()` 时，会自动触发该事件。

### 索引映射

LED 索引映射规则：
- **镭射设备**: 序号 80~159（80 个镭射点）
- **拍拍灯通道 5**: 序号 160~221（220/221 为开始/结束按钮）
- **拍拍灯通道 6**: 序号 222~281

矩阵扫描方式：**S 形扫描**（蛇形扫描）
- 第 0 行：从左到右
- 第 1 行：从右到左
- 第 2 行：从左到右
- ...

### 颜色格式

硬件颜色格式（uint）：
```
Bit 17-24: R (红色)
Bit 9-16:  G (绿色)
Bit 1-8:   B (蓝色)
Bit 0:     保留
```

转换公式：
```csharp
byte r = (byte)((color >> 17) & 0xFF);
byte g = (byte)((color >> 9) & 0xFF);
byte b = (byte)((color >> 1) & 0xFF);
```

## 扩展功能

### 添加更多可视化效果

可以在 `SimulatorUI` 中添加：
- 动画效果（闪烁、渐变）
- 粒子效果
- 音效反馈

### 支持多通道显示

当前实现主要针对通道 0。如需支持多通道：
1. 修改 `UpdateLedCell()` 方法
2. 为每个通道创建独立的网格容器
3. 实现通道切换 UI

## 注意事项

⚠️ **编译条件**: 模拟器功能需要 `#if IO_YDGZ` 编译条件满足

⚠️ **性能**: 大量 LED 更新可能影响性能，建议：
- 限制更新频率
- 使用对象池
- 批量更新 UI

⚠️ **测试**: 模拟器仅用于开发测试，发布版本应禁用或移除
