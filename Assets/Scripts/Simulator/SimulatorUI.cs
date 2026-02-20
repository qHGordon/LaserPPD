using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Game_LaserTouch; // LaserTouch 命名空间
using LaserPPD.Core;

public class SimulatorUI : MonoBehaviour
{
    [Header("UI Settings")]
    public RectTransform gridContainer;
    public GameObject ledCellPrefab; // 如果为空，代码会自动创建
    public Vector2 cellSize = new Vector2(20, 20);
    public Vector2 cellSpacing = new Vector2(2, 2);

    [Header("Grid Config")]
    public int rows = 10; // 根据实际游戏矩阵调整
    public int columns = 10; // 根据实际游戏矩阵调整

    [Header("Color Settings")]
    public Color defaultColor = new Color(0.1f, 0.1f, 0.1f);
    public Color clickHighlightColor = Color.green;

    [Header("Input Simulation")]
    public bool enableInputSimulation = true;
    public LaserTouch laserTouchRef; // 拖入场景里的 LaserTouch

    private Image[] ledCells;
    private Dictionary<int, Image> indexToImageMap = new Dictionary<int, Image>();

    void Start()
    {
        InitGrid();
        
        // 订阅事件 (根据实际类名修改)
        #if IO_YDGZ
        CmdIO_WeChat.OnLedDataSent += HandleLedData;
        #endif
    }

    void OnDestroy()
    {
        #if IO_YDGZ
        CmdIO_WeChat.OnLedDataSent -= HandleLedData;
        #endif
    }

    // 初始化网格
    void InitGrid()
    {
        if (gridContainer == null) gridContainer = GetComponent<RectTransform>();

        // 设置布局组件
        GridLayoutGroup grid = gridContainer.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = cellSize;
        grid.spacing = cellSpacing;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        ledCells = new Image[rows * columns];

        for (int i = 0; i < rows * columns; i++)
        {
            GameObject cellObj = null;
            if (ledCellPrefab != null)
            {
                cellObj = Instantiate(ledCellPrefab, gridContainer);
            }
            else
            {
                cellObj = new GameObject($"Cell_{i}", typeof(Image));
                cellObj.transform.SetParent(gridContainer, false);
            }

            Image img = cellObj.GetComponent<Image>();
            img.color = defaultColor;
            ledCells[i] = img;

            // 添加点击事件监听
            int index = i; // 闭包捕获
            EventTrigger trigger = cellObj.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => OnCellClicked(index));
            trigger.triggers.Add(entry);

            // 计算硬件索引映射 (S形扫描或其他逻辑)
            // 这里简单假设 i 就是 index，实际可能需要根据 S 形公式转换
            indexToImageMap[index] = img; 
        }
        
        Debug.Log($"[硬件模拟器UI] 初始化完成，生成了 {ledCells.Length} 个格子");
    }

    // 处理硬件发来的 LED 数据
    void HandleLedData(int channel, uint colorUint, uint[] indices, int length)
    {
        // 颜色转换: uint -> Color
        // Bit 17-24: R, Bit 9-16: G, Bit 1-8: B
        float r = ((colorUint >> 17) & 0xFF) / 255f;
        float g = ((colorUint >> 9) & 0xFF) / 255f;
        float b = ((colorUint >> 1) & 0xFF) / 255f;
        Color newColor = new Color(r, g, b);

        foreach (var idx in indices)
        {
            // 注意：这里的 idx 是硬件协议里的索引，可能需要映射回 UI 网格的索引
            // 简单演示：假设 idx 直接对应 UI 列表
            if (indexToImageMap.ContainsKey((int)idx))
            {
                indexToImageMap[(int)idx].color = newColor;
            }
        }
    }

    // 模拟鼠标点击
    void OnCellClicked(int uiIndex)
    {
        if (!enableInputSimulation) return;

        // 高亮一下
        if (indexToImageMap.ContainsKey(uiIndex))
            StartCoroutine(FlashCell(indexToImageMap[uiIndex]));

        Debug.Log($"[硬件模拟器] 点击了格子: {uiIndex}");

        // 发送信号给游戏逻辑
        if (laserTouchRef != null)
        {
            List<int> signals = new List<int> { uiIndex }; // 发送被点击的索引
            laserTouchRef.UpdateSignalsInput(signals);
        }
        else
        {
            Debug.LogWarning("[硬件模拟器] 未绑定 LaserTouch 引用！");
        }
    }

    IEnumerator FlashCell(Image img)
    {
        Color original = img.color;
        img.color = clickHighlightColor;
        yield return new WaitForSeconds(0.2f);
        img.color = original;
    }
}