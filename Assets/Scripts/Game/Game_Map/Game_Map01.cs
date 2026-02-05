using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Game_Map01 : MonoBehaviour
{
    public bool isclearall = false;
    int maxJieduan = 4;
    public int Jieduan_Now = 0;
    public List<Game01_Ledone> list_LightPos = new List<Game01_Ledone>();

    private void Awake()
    {
        instance = this;
    }
    public static Game_Map01 instance;
    public Game01_Ledone obj_Led;
    public float max_runtime = 0;
    public float protectTime = 5;

    public void Initmap()
    {
        protectTime = 5;

        for (int i = 0; i < list_LightPos.Count; i++)
        {
            Destroy(list_LightPos[i].gameObject);
        }
        isclearall = false;
        maxJieduan = 4;

        list_LightPos = new List<Game01_Ledone>();
        GameLeiSheLedControl.PrePic_Copy = new int[Set.setVal.Width * Set.setVal.Height];
        GameLeiSheLedControl.LedCoordinate();
        Invoke("Initmap_Maze_" + (Main.MapIndex % 12).ToString("D2"), 0);
        // Initmap_Maze_01();
    }
    Game01_Ledone New_LedOne(int x, int y, int dir, Game01_Ledone.en_Move_Type move)
    {
        if (x < 0 || x >= Set.setVal.Width || y < 0 || y >= Set.setVal.Height)
        {
            return null;
        }
        Game01_Ledone one = Instantiate(obj_Led, this.transform);
        one.Init(x, y, dir, move);

        list_LightPos.Add(one);
        return one;

    }
    Game01_Ledone New_LedOne(List<Vector2Int> _pos, int dir, Game01_Ledone.en_Move_Type move)
    {
        Game01_Ledone one = Instantiate(obj_Led, this.transform);
        one.Init(_pos, dir, move);

        list_LightPos.Add(one);
        return one;
    }
    void Initmap_Maze_00()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        //     New_LedOne(0, 0, 0, 0);
        //     New_LedOne(2, 0, 0, Game01_Ledone.en_Move_Type.OneWay);
        New_LedOne(5, 0, 0, 0);
        New_LedOne(Set.setVal.Width - 2, 3, 0, 0);
        New_LedOne(7, Set.setVal.Height - 2, 0, 0);
        New_LedOne(9, Set.setVal.Height - 2, 0, 0);
        New_LedOne(Set.setVal.Width / 2, 3, 0, 0);

        switch (Jieduan_Now)
        {

            case 1:
                List<Vector2Int> pos1 = new List<Vector2Int>();
                ;
                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                        pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(pos1, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
                New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);


                break;
            case 2:
                pos = new List<Vector2Int>();
                for (int i = 2; i < Set.setVal.Width - 2; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                break;
            case 3:
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 3; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                pos = new List<Vector2Int>();

                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                        pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);

                break;
        }

    }
    void Initmap_Maze_01()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        List<Vector2Int> pos1 = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

            }
        }
        New_LedOne(pos, 0, 0);
        New_LedOne(pos1, 0, 0);
        New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
        New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);

        switch (Jieduan_Now)
        {

            case 1:

                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                        pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(pos1, 0, Game01_Ledone.en_Move_Type.OneWay);
                New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
                New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);


                break;
            case 2:
                pos = new List<Vector2Int>();
                for (int i = 2; i < Set.setVal.Width - 2; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                break;
            case 3:
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 3; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                pos = new List<Vector2Int>();

                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                        pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);

                break;
        }

    }
    void Initmap_Maze_02()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        List<Vector2Int> pos1 = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

            }
        }
        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
        New_LedOne(pos1, 0, Game01_Ledone.en_Move_Type.OneWay);
        New_LedOne(Set.setVal.Width / 4, 0, 0, 0);
        New_LedOne(Set.setVal.Width * 3 / 4, 0, 0, 0);

        switch (Jieduan_Now)
        {

            case 1:




                break;
            case 2:
                pos = new List<Vector2Int>();
                for (int i = 2; i < Set.setVal.Width - 2; i++)
                {
                    for (int k = 2; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                break;
            case 3:
                for (int i = 0; i < Set.setVal.Width; i++)
                {
                    for (int k = 3; k < Set.setVal.Height - 1; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                    }
                }
                New_LedOne(pos, 0, 0);
                pos = new List<Vector2Int>();

                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pos.Add(new Vector2Int(i, k));
                        pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
                    }
                }
                New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);

                break;
        }

    }
    void Initmap_Maze_03()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;

        List<Vector2Int> pos1 = new List<Vector2Int>();
        for (int i = 0; i < 3; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));
                pos1.Add(new Vector2Int(Set.setVal.Width / 2 + i, Set.setVal.Height - 1 - k));

            }
        }
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 3; k < Set.setVal.Height - 1; k++)
            {
                pos.Add(new Vector2Int(i, k));
            }
        }
        New_LedOne(pos, 0, 0);
        pos = new List<Vector2Int>();

        for (int i = 0; i < 2; i++)
        {
            for (int k = 0; k < 3; k++)
            {
                pos.Add(new Vector2Int(i, k));
                pos.Add(new Vector2Int(Set.setVal.Width - 1 - i, k));
            }
        }
        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);


    }
    void Initmap_Maze_04()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));

            }
        }
        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Stay);
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            if (i % 7 == 0)
            {
                New_LedOne(i, 0, 0, Game01_Ledone.en_Move_Type.OneWay);

            }
        }

    }
    void Initmap_Maze_05()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        pos = new List<Vector2Int>();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            if (i % 8 == 0)
            {
                for (int k = 0; k < 3; k++)
                {
                    pos.Add(new Vector2Int(i, k));

                }

            }
        }

        New_LedOne(pos, 0, 0);
        pos = new List<Vector2Int>();
        for (int i = Set.setVal.Width - 3; i < Set.setVal.Width; i++)
        {

            for (int k = 0; k < 2; k++)
            {
                pos.Add(new Vector2Int(i, Set.setVal.Height - 1 - k));

            }

        }

        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);

    }
    void Initmap_Maze_06()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = 0; k < Set.setVal.Height; k++)
            {
                if (i % 8 == 0)
                {
                    pos.Add(new Vector2Int(i, k));

                }
            }
        }

        New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);
        max_runtime = 0.5f;
    }
    void Initmap_Maze_07()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        Game01_Ledone _one;
        pos = new List<Vector2Int>();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = Set.setVal.Height - 3; k < Set.setVal.Height; k++)
            {
                pos.Add(new Vector2Int(i, k));

            }
        }


        _one = New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Stay);
        _one.donw_Y = Set.setVal.Height - 3;
        for (int i = 0; i < 4; i++)
        {
            New_LedOne(Set.setVal.Width / 2 - i * 2, 0, 0, Game01_Ledone.en_Move_Type.OneWay);
            New_LedOne(Set.setVal.Width / 2 + i * 2, 0, 0, Game01_Ledone.en_Move_Type.OneWay);

        }
        max_runtime = 1f;

    }
    void Initmap_Maze_09()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        Game01_Ledone _one;
        pos = new List<Vector2Int>();
        for (int i = 0; i < Set.setVal.Width; i++)
        {
            for (int k = Set.setVal.Height - 3; k < Set.setVal.Height; k++)
            {
                pos.Add(new Vector2Int(i, k));

            }
        }
        for (int k = 0; k < Set.setVal.Height; k++)
        {
            pos.Add(new Vector2Int(0, k));
            pos.Add(new Vector2Int(Set.setVal.Width / 2, k));
            pos.Add(new Vector2Int(Set.setVal.Width - 1, k));

        }
        _one = New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.Shake);
        max_runtime = 2;

    }
    void Initmap_Maze_08()
    {
        List<Vector2Int> pos = new List<Vector2Int>();
        max_runtime = 1.5f;
        Game01_Ledone _one;
        for (int n = 0; n < 3; n++)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int k = Set.setVal.Height - 3; k < Set.setVal.Height; k++)
                {

                    pos.Add(new Vector2Int(n * 8 + i, k));

                }
            }
        }
        _one = New_LedOne(pos, 0, Game01_Ledone.en_Move_Type.OneWay);

        for (int i = 0; i < 3; i++)
        {
            New_LedOne(0 + i * 8, 0, 0, 0);

        }
    }
    float runtime = 0;
    int startId = LedKey.GetLeiSheKeyStartId();
    private void Update()
    {
        if (Game01_Main.instance.player.statue != en_Player01Sta.Play)
        {
            return;
        }
        if (protectTime > 0)
        {
            protectTime -= Time.deltaTime;
        }
        if (Game01_Main.instance.player.currJieDuan== 0 || Game01_Main.instance.player.currJieDuan == 2)
        {
            if (Game01_Main.instance.player.statue==en_Player01Sta.Play)
            {
                for (int i = 0; i < Set.setVal.Width ; i++)
                {
                    for (int k = 0; k <  Set.setVal.Height; k++)
                    {
                        Framebuffer.Update_TransmitLedColor(i,k, 255, enPointSta.Target);
                        if (i==0)
                        {
                            if (LedKey.KeyStatus(startId + Framebuffer.MappingId(i,k)) == false)
                            {
                                if (protectTime <= 0)
                                {
                                    MusicManager.instance.Play_Fails();
                                    protectTime = 1;
                                    if (FjData.g_Fj[0].Life > 0)
                                    {
                                        Debug.LogError("地面扣血坐标：" +i + "  " + k);

                                        FjData.g_Fj[0].Life--;
                                    }
                                }
                            }
                        }
                     
                    }
                    
                }

            }
        }
        runtime += Time.deltaTime;
        if (runtime > 0.02f)
        {
            runtime = 0;
            Framebuffer.Update_TransmitLedClearAll();
        }


    }
}
