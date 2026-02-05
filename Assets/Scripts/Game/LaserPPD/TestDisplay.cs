using System;
using UnityEngine;

public class TestDisplay : MonoBehaviour
{
    public static TestDisplay Instance;
    public RectTransform gridParent; // 网格父物体
    public GameObject gridPrefab; // 网格预制体
    public int[,] laserMatrix; // 镭射矩阵
    public int width = 10; // 网格宽度
    public int height = 10; // 网格高度
    public int currentChannel = 0; // 当前通道

    void Awake()
    {
        Instance = this;
    }
    //触发返回的序号 镭射的序号是80~159 80个镭射，拍拍灯是通道5 序号位160~221（220和221分别是开始和结束） ，通道6 为222到281
    public Color Uint2Color(uint color)
    {
        byte r = (byte)((color >> 17) & 0xFF);
        byte g = (byte)((color >> 9) & 0xFF);
        byte b = (byte)((color >> 1) & 0xFF);
        return new Color32(r, g, b, 255);
    }
    public void InitMatrix()
    {
        height = Set.setVal.WallNum_Height;
        laserMatrix = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                laserMatrix[i, j] = 0; //初始化为0
            }
        }
        CreateGrid();
    }

    private void CreateGrid()
    {
        
    }
}