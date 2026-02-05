using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum en_ArryType
{
    LeftUp = 0,
    LeftCenter,
    LeftDown,
    RightUp,
    RightCenter,
    RightDown,
    CenterUp,
    Center,
    CenterDown,
}

public class Menu_SizeSet : MonoBehaviour
{
    public Text text_Title;
    public Menu_SetOne[] setOne;
    public GameObject ledOne_Prefab;
    public Transform chLen_Layer;
    public Button button_UpdateSize;
    public Button button_Save;
    public Button button_Default;
    public Button button_Back;


    const int SETID_Width = 0;
    const int SETID_Height = 1;
    const int SETID_ChannelLength = 2;
    const int SETID_WallLedNum = 8;
    const int SETID_WallLedNum_Widch = 9;
    const int SETID_WallLedNum_Height = 10;
    const int SETID_PPDWidth = 11;
    const int SETID_PPDHeight = 12;

    const int CH_ONE_WIDTH = 50;

    public static List<Menu_LedOne> list_ChannleLen = new List<Menu_LedOne>();
    byte[,] ledMask = new byte[PresetPic.PIC_WIDTH, PresetPic.PIC_HEIGHT];


    int selectId;
    int width;
    int height;
    int[] channelLen = new int[Main.MAX_CH];

    float ledLayerLimitUp;
    float ledLayerLimitX;
    Vector2 ledLayerPos;

    Menu menu;
    public void Awake0(Menu mmenu)
    {
        menu = mmenu;

        button_UpdateSize.onClick.AddListener(OnClick_UpdateSize);
        button_Save.onClick.AddListener(OnClick_Save);
        button_Default.onClick.AddListener(OnClick_Default);
        button_Back.onClick.AddListener(OnClick_Back);

        setOne[SETID_Width].SetValueInit(Set.SET_c_Size);
        setOne[SETID_Height].SetValueInit(Set.SET_c_Size);
        setOne[SETID_WallLedNum].SetValueInit(Set.SET_c_WallLedNum);
        for (int i = 0; i < setOne.Length; i++)
        {
            setOne[i].Init(i, OnClick_SetOne);
        }
        for (int i = 0; i < Set.ChannelLength.Length; i++)
        {
            setOne[SETID_ChannelLength + i].SetValueInit(Set.SET_c_ChannelLength);
        }
        setOne[SETID_WallLedNum_Height].SetValueInit(Set.SET_c_Size);
        setOne[SETID_WallLedNum_Widch].SetValueInit(Set.SET_c_Size);
        setOne[SETID_PPDWidth].SetValueInit(Set.SET_c_Size);
        setOne[SETID_PPDHeight].SetValueInit(Set.SET_c_Size);
    }

    void OnEnable()
    {
        GameStart();
    }
    // Use this for initialization
    public void GameStart()
    {
        for (int i = 0; i < PresetPic.PIC_WIDTH; i++)
        {
            for (int j = 0; j < PresetPic.PIC_HEIGHT; j++)
            {
                ledMask[i, j] = Framebuffer.ledMask[i, j];
            }
        }
        for (int i = 0; i < channelLen.Length; i++)
        {
            channelLen[i] = Set.ChannelLength[i];
        }
        selectId = -1;
        Update_Languae();
        Update_SetValue();
        Update_SelectId();
        OnClick_UpdateSize();

#if LiuGuang
        setOne[8].gameObject.SetActive(false);
        setOne[9].gameObject.SetActive(true);
        setOne[10].gameObject.SetActive(true);

        setOne[0].transform.localPosition = new Vector2(-650, 256);
        setOne[1].transform.localPosition = new Vector2(-209, 256);
        setOne[9].transform.localPosition = new Vector2(232, 256);
        setOne[10].transform.localPosition = new Vector2(667, 256);


#else
        setOne[8].gameObject.SetActive(true);
        setOne[9].gameObject.SetActive(false);
        setOne[10].gameObject.SetActive(false);
        //setOne[0].transform.localPosition = new Vector2(-500, 260);
        //setOne[1].transform.localPosition = new Vector2(-0, 260);
        //setOne[8].transform.localPosition = new Vector2(500, 260);
    
#endif
    }

    // Update is called once per frame
    void Update()
    {
        //if (chLen_Layer.parent.localPosition.y < 0) {
        //    chLen_Layer.parent.localPosition = new Vector3 (0, 0);
        //} else if (chLen_Layer.parent.localPosition.y > ledLayerLimitUp) {
        //    chLen_Layer.parent.localPosition = new Vector3 (0, chLenLayerLimitUp);
        //}

        ledLayerPos = chLen_Layer.transform.parent.localPosition;
        ledLayerPos.x = Mathf.Clamp(ledLayerPos.x, -ledLayerLimitX, 0);
        ledLayerPos.y = Mathf.Clamp(ledLayerPos.y, 0, ledLayerLimitUp);
        chLen_Layer.transform.parent.localPosition = ledLayerPos;
    }

    void Update_Languae()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            text_Title.text = "布 局 设 置";
            button_UpdateSize.GetComponentInChildren<Text>().text = "更 新 布 局";
            button_Save.GetComponentInChildren<Text>().text = "保 存 设 置";
            button_Default.GetComponentInChildren<Text>().text = "默 认 值";
            button_Back.GetComponentInChildren<Text>().text = "返 回";

            setOne[SETID_Width].SetName("宽度");
            setOne[SETID_Height].SetName("高度");
            setOne[SETID_WallLedNum].SetName("墙灯个数");
            for (int i = 0; i < Set.ChannelLength.Length; i++)
            {
                setOne[SETID_ChannelLength + i].SetName("通道" + (i + 1) + "长度");
            }
            setOne[SETID_WallLedNum_Widch].SetName("墙灯宽度");
            setOne[SETID_WallLedNum_Height].SetName("墙灯高度");
            setOne[SETID_PPDWidth].SetName("拍拍灯宽度");
            setOne[SETID_PPDHeight].SetName("拍拍灯高度");

        }
        else
        {
            text_Title.text = "Size Setting";
            button_UpdateSize.GetComponentInChildren<Text>().text = "Update size";
            button_Save.GetComponentInChildren<Text>().text = "Save";
            button_Default.GetComponentInChildren<Text>().text = "Default";
            button_Back.GetComponentInChildren<Text>().text = "Return";

            setOne[SETID_Width].SetName("Width");
            setOne[SETID_Height].SetName("Height");
            setOne[SETID_WallLedNum].SetName("Number of button");

            for (int i = 0; i < Set.ChannelLength.Length; i++)
            {
                setOne[SETID_ChannelLength + i].SetName("Channel " + (i + 1) + " Length");
            }
            setOne[SETID_WallLedNum_Widch].SetName("WallLED_Width+");
            setOne[SETID_WallLedNum_Height].SetName("WallLED_Heigt+");
            setOne[SETID_PPDWidth].SetName("Touch_Width");
            setOne[SETID_PPDHeight].SetName("Touch_Height");
        }
    }

    void Update_SetValue()
    {
        setOne[SETID_Width].UpdateValue(Set.setVal.Width);
        setOne[SETID_Height].UpdateValue(Set.setVal.Height);
        setOne[SETID_WallLedNum].UpdateValue(Set.setVal.WallLedNum);
        for (int i = 0; i < Set.ChannelLength.Length; i++)
        {
            setOne[SETID_ChannelLength + i].UpdateValue(Set.ChannelLength[i]);
        }
        setOne[SETID_WallLedNum_Widch].UpdateValue(Set.setVal.WallNum_Width);
        setOne[SETID_WallLedNum_Height].UpdateValue(Set.setVal.WallNum_Height);
        setOne[SETID_PPDWidth].UpdateValue(Set.setVal.PPDWidth);
        setOne[SETID_PPDHeight].UpdateValue(Set.setVal.PPDHeight);
    }

    void Update_SelectId()
    {
        for (int i = 0; i < setOne.Length; i++)
        {
            if (selectId == i)
            {
                setOne[i].SetSelect(true);
            }
            else
            {
                setOne[i].SetSelect(false);
            }
        }
    }

    readonly static Color[] tab_Color = { Color.red, Color.green, Color.blue, Color.yellow, Color.gray, new Color(0, 0.5f, 0.5f) };
    void Update_ChannleLen()
    {
        for (int i = 0; i < Set.ChannelLength.Length; i++)
        {
            channelLen[i] = setOne[SETID_ChannelLength + i].GetValue();
        }

        int dir = 0;
        int x = 0;
        int y = 0;
        int len = channelLen[0];
        int id = 0;

        for (int i = 0; i < channelLen.Length && id < list_ChannleLen.Count; i++)
        {
            for (int j = 0; j < channelLen[i] && id < list_ChannleLen.Count; j++)
            {
                list_ChannleLen[id].Init(i, j);
                list_ChannleLen[id].SetColor(tab_Color[i]);
                //
                if (dir == 0)
                {
                    list_ChannleLen[id].transform.localPosition = new Vector3(x * CH_ONE_WIDTH, y * CH_ONE_WIDTH);
                }
                else
                {
                    list_ChannleLen[id].transform.localPosition = new Vector3((width - 1 - x) * CH_ONE_WIDTH, y * CH_ONE_WIDTH);
                }
                id++;
                if (++x >= width)
                {
                    x = 0;
                    y++;
                    dir = (dir + 1) % 2;
                }
            }
        }
        for (int i = id; i < list_ChannleLen.Count; i++)
        {
            list_ChannleLen[i].Init(0, 0);
            list_ChannleLen[i].SetColor(Color.black);
            //
            if (dir == 0)
            {
                list_ChannleLen[id].transform.localPosition = new Vector3(x * CH_ONE_WIDTH, -y * CH_ONE_WIDTH);
            }
            else
            {
                list_ChannleLen[id].transform.localPosition = new Vector3((width - 1 - x) * CH_ONE_WIDTH, -y * CH_ONE_WIDTH);
            }
            id++;
            if (++x >= width)
            {
                x = 0;
                y++;
                dir = (dir + 1) % 2;
            }
        }

        ledLayerLimitUp = y * 50 + 50 - 460;
        if (ledLayerLimitUp < 0)
        {
            ledLayerLimitUp = 0;
        }

        ledLayerLimitX = Set.setVal.Width * CH_ONE_WIDTH - 1500 + CH_ONE_WIDTH;
        if (ledLayerLimitX < 0)
        {
            ledLayerLimitX = 0;
        }
        ledLayerLimitUp = Set.setVal.Height * CH_ONE_WIDTH - 400 + CH_ONE_WIDTH;
        if (ledLayerLimitUp < 0)
        {
            ledLayerLimitUp = 0;
        }
    }


    public static void Update_LedPos(List<Menu_LedOne> listLedOne, en_ConnectOrder connectOrder, int ledWidth, int width, int height, en_ArryType arryType)
    {
        int x, y;
        int dir = 0;
        float offsetx = 0;
        float offsety = 0;
        //Debug.LogError("connectOrder: " + connectOrder);

        switch (arryType)
        {
            case en_ArryType.LeftUp:
                offsetx = 0;// 0.5f * ledWidth;
                offsety = -(height - 1) * ledWidth;
                break;
            case en_ArryType.LeftCenter:
                break;
            case en_ArryType.LeftDown:
                offsetx = 0;// 0.5f * ledWidth;
                offsety = 0;// 0.5f * height;
                break;
            case en_ArryType.RightUp:
                break;
            case en_ArryType.RightCenter:
                break;
            case en_ArryType.RightDown:
                break;
            case en_ArryType.CenterUp:
                break;
            case en_ArryType.Center:
                offsetx = -(width - 1) * ledWidth * 0.5f;
                offsety = -(height - 1) * ledWidth * 0.5f;
                break;
            case en_ArryType.CenterDown:
                break;
        }

        switch (connectOrder)
        {
            case en_ConnectOrder.LeftDown_ToRight:
                x = 0;
                y = 0;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (x + 1 < width)
                        {
                            x++;
                        }
                        else
                        {
                            y++;
                            dir = 1;
                        }
                    }
                    else if (x > 0)
                    {
                        x--;
                    }
                    else
                    {
                        y++;
                        dir = 0;
                    }
                }
                break;

            case en_ConnectOrder.LeftDown_ToUp:
                x = 0;
                y = 0;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (y + 1 < height)
                        {
                            y++;
                        }
                        else
                        {
                            x++;
                            dir = 1;
                        }
                    }
                    else if (y > 0)
                    {
                        y--;
                    }
                    else
                    {
                        x++;
                        dir = 0;
                    }
                }
                break;

            case en_ConnectOrder.LeftUp_ToRight:
                x = 0;
                y = height - 1;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (x + 1 < width)
                        {
                            x++;
                        }
                        else
                        {
                            y--;
                            dir = 1;
                        }
                    }
                    else if (x > 0)
                    {
                        x--;
                    }
                    else
                    {
                        y--;
                        dir = 0;
                    }
                }
                break;

            case en_ConnectOrder.LeftUp_ToDown:
                x = 0;
                y = height - 1;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (y > 0)
                        {
                            y--;
                        }
                        else
                        {
                            x++;
                            dir = 1;
                        }
                    }
                    else if (y + 1 < height)
                    {
                        y++;
                    }
                    else
                    {
                        x++;
                        dir = 0;
                    }
                }
                break;


            case en_ConnectOrder.RightUp_ToDown:
                x = width - 1;
                y = height - 1;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (y > 0)
                        {
                            y--;
                        }
                        else
                        {
                            x--;
                            dir = 1;
                        }
                    }
                    else if (y + 1 < height)
                    {
                        y++;
                    }
                    else
                    {
                        x--;
                        dir = 0;
                    }
                }
                break;

            case en_ConnectOrder.RightUp_ToLeft:
                x = width - 1;
                y = height - 1;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (x > 0)
                        {
                            x--;
                        }
                        else
                        {
                            y--;
                            dir = 1;
                        }
                    }
                    else if (x + 1 < width)
                    {
                        x++;
                    }
                    else
                    {
                        y--;
                        dir = 0;
                    }
                }
                break;


            case en_ConnectOrder.RightDown_ToUp:
                x = width - 1;
                y = 0;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (y + 1 < height)
                        {
                            y++;
                        }
                        else
                        {
                            x--;
                            dir = 1;
                        }
                    }
                    else if (y > 0)
                    {
                        y--;
                    }
                    else
                    {
                        x--;
                        dir = 0;
                    }
                }
                break;
            case en_ConnectOrder.RightDown_ToLeft:
                x = width - 1;
                y = 0;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (x > 0)
                        {
                            x--;
                        }
                        else
                        {
                            y++;
                            dir = 1;
                        }
                    }
                    else if (x + 1 < width)
                    {
                        x++;
                    }
                    else
                    {
                        y++;
                        dir = 0;
                    }
                }
                break;
            case en_ConnectOrder.LeftUp_ToDown_LEIShe:
                x = 0;
                y = height - 1;
                for (int i = 0; i < listLedOne.Count; i++)
                {
                    listLedOne[i].transform.localPosition = new Vector3(x * ledWidth + offsetx, y * ledWidth + offsety);
                    listLedOne[i].SetPosXY(x, y);
                    if (dir == 0)
                    {
                        if (y > 0)
                        {
                            y--;
                        }
                        else
                        {
                            x++;
                            y = height - 1;
                        }
                    }
                    //else if (y + 1 < height)
                    //{
                    //    y++;
                    //}
                    //else
                    //{
                    //    x++;
                    //    dir = 0;
                    //}
                }
                break;
        }
    }

    void Update_LedStaOne(int id, byte sta)
    {
        if (id < list_ChannleLen.Count)
        {
            if (sta == 0)
            {
                list_ChannleLen[id].SetColor(Color.green, sta);
            }
            else
            {
                list_ChannleLen[id].SetColor(Color.gray, sta);
            }
        }
    }
    void Update_LedSta()
    {
        for (int i = 0; i < list_ChannleLen.Count; i++)
        {
            Update_LedStaOne(i, ledMask[list_ChannleLen[i].x, list_ChannleLen[i].y]);
        }
    }

    public void OnClick_UpdateSize()
    {
        width = setOne[SETID_Width].GetValue();
        height = setOne[SETID_Height].GetValue();
        for (int i = 0; i < Set.ChannelLength.Length; i++)
        {
            channelLen[i] = setOne[SETID_ChannelLength + i].GetValue();
        }

        for (int i = 0; i < list_ChannleLen.Count; i++)
        {
            if (list_ChannleLen[i] == null)
                continue;
            Destroy(list_ChannleLen[i].gameObject);
        }
        list_ChannleLen.Clear();
        // new
        int ch = 0;
        int id = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Menu_LedOne ledOne = Instantiate(ledOne_Prefab, chLen_Layer).GetComponent<Menu_LedOne>();
                ledOne.Init(list_ChannleLen.Count, OnClick_LedOne);
                ledOne.Init(ch, id);
                list_ChannleLen.Add(ledOne);
                if (ch < channelLen.Length)
                {
                    if (++id >= channelLen[ch])
                    {
                        ch++;
                        id = 0;
                    }
                }
            }
        }
        //Update_ChannleLen ();
        Update_LedPos(list_ChannleLen, (en_ConnectOrder)Set.setVal.Index_AnZhuang, 50, width, height, en_ArryType.LeftUp);
        Update_LedSta();

        ledLayerLimitX = Set.setVal.Width * CH_ONE_WIDTH - 1600 + CH_ONE_WIDTH;
        if (ledLayerLimitX < 0)
        {
            ledLayerLimitX = 0;
        }
        ledLayerLimitUp = Set.setVal.Height * CH_ONE_WIDTH - 460 + CH_ONE_WIDTH;
        if (ledLayerLimitUp < 0)
        {
            ledLayerLimitUp = 0;
        }
    }
    public void OnClick_SetOne(int id)
    {
        selectId = id;
        Update_SelectId();
    }

    public void OnClick_LedOne(int id)
    {
        if (list_ChannleLen[id].pointType == 0)
        {
            Update_LedStaOne(id, 1);
        }
        else
        {
            Update_LedStaOne(id, 0);
        }
    }

    void SaveSet(bool isOk)
    {
        if (isOk == false)
            return;
        Set.setVal.Width = setOne[SETID_Width].GetValue();
        Set.setVal.Height = setOne[SETID_Height].GetValue();
        Set.setVal.WallLedNum = setOne[SETID_WallLedNum].GetValue();
        Set.setVal.WallLedNum = 0;
        for (int i = 0; i < Set.ChannelLength.Length; i++)
        {
            Set.ChannelLength[i] = setOne[SETID_ChannelLength + i].GetValue();
        }
        Set.setVal.WallNum_Width = setOne[SETID_WallLedNum_Widch].GetValue();
        Set.setVal.WallNum_Height = setOne[SETID_WallLedNum_Height].GetValue();
        Set.setVal.PPDWidth = setOne[SETID_PPDWidth].GetValue();
        Set.setVal.PPDHeight = setOne[SETID_PPDHeight].GetValue();
        Set.SaveSize();

        for (int i = 0; i < list_ChannleLen.Count; i++)
        {
            if (list_ChannleLen[i].x < PresetPic.PIC_WIDTH && list_ChannleLen[i].y < PresetPic.PIC_HEIGHT)
            {
                ledMask[list_ChannleLen[i].x, list_ChannleLen[i].y] = list_ChannleLen[i].pointType;
            }
        }
        Framebuffer.SaveSetting(ledMask);

        Main.VerifySpeedFromSize();
        Framebuffer.MapTabInit();
        //
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("保存成功", 3, true);
        }
        else
        {
            Menu_Tips.instance.Init("Saved successfully", 3, true);
        }
    }
    public void OnClick_Save()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("是否保存设置", SaveSet);
        }
        else
        {
            Menu_Tips.instance.Init("Save setting?", SaveSet);
        }
    }

    void DefaultSet(bool isOk)
    {
        if (isOk == false)
            return;
        for (int i = 0; i < PresetPic.PIC_WIDTH; i++)
        {
            for (int j = 0; j < PresetPic.PIC_HEIGHT; j++)
            {
                ledMask[i, j] = 1;
            }
        }
        Set.DefaultSize();
        Framebuffer.defaultSetting();
        GameStart();
        //
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("恢复默认值成功", 3, true);
        }
        else
        {
            Menu_Tips.instance.Init("Restore Default Success", 3, true);
        }
    }
    public void OnClick_Default()
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            Menu_Tips.instance.Init("是否恢复默认值", DefaultSet);
        }
        else
        {
            Menu_Tips.instance.Init("Restore default setting?", DefaultSet);
        }
    }
    public void OnClick_Back()
    {
        //gameObject.SetActive (false);
        menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
    }
}
