using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class Menu_AdcVerify : MonoBehaviour
{
    PresetPic presetPic;
    public Transform presetPic_Layer;
    List<Image> list_PresetPic;
    public test_ShowLight[] Led_test = new test_ShowLight[1000];
    public GameObject picOne_Prefab;
    Menu menu;
    public Button button_Back;
    public Button button_ReSet;
    public Button button_Show;
    float runTime = 0;
    int ch = 0;
    int led_Now = 0;
    int led_End = 0;
    bool startShow = false;
    public void Back()
    {

        for (int i = 0; i < presetPic_Layer.childCount; i++)
        {
            Destroy(presetPic_Layer.GetChild(i).gameObject);
        }
        menu.ChangeStatue(en_MenuStatue.MenuSta_SysSet);
    }

    public void Awake0(Menu mmenu)
    {
        //
        menu = mmenu;
        button_Back.onClick.AddListener(Back);
        button_ReSet.onClick.AddListener(ResetMap);
        button_Show.onClick.AddListener(Show);

    }
    public void GameStart()
    {
        startShow = false;
        runTime = 0;
        ch = 0;
        led_Now = 0;
        PresetPic_Init();
        ResetMap();
        GameLedControl.Stop();
        if (Set.setVal.Language == 0)
        {
            button_Back.GetComponent<Menu_Button>().text.text = "返回";
            button_ReSet.GetComponent<Menu_Button>().text.text = "清空";
            button_Show.GetComponent<Menu_Button>().text.text = "更新";

        }
        else if (Set.setVal.Language == 1)
        {
            button_Back.GetComponent<Menu_Button>().text.text = "Back";
            button_ReSet.GetComponent<Menu_Button>().text.text = "ReSet";
            button_Show.GetComponent<Menu_Button>().text.text = "Show";


        }
        ///   GameLedControl.ReadyStart(Set.gameSetting.gameLevelSetting[0], false);
        //   GameLedControl.PlayStart(Set.gameSetting.gameLevelSetting[0]);
    }
    public void ResetMap()
    {
        runTime = 0;
        startShow = false;
        ch = 0;
        Framebuffer.Update_ColorFull(0, enPointSta.None);
        for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
        {
            GameLedControl.gamePoint[i].statue = enPointSta.None;

        }
        for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
        {

            list_PresetPic[i].color = tab_PointColor[0];
            Led_test[i].istouch = false;
        }
    }

    void Show()
    {
        startShow = true;
        runTime = 0.1f;
        int startpos = 0;

        for (int i = 0; i < ch; i++)
        {
            startpos += Set.ChannelLength[i];
        }

        led_Now = startpos; led_End = led_Now + Set.ChannelLength[ch];

        for (int i = led_Now; i < led_End; i++)
        {
            GameLedControl.gamePoint[i].statue = enPointSta.Target;
            DrawPic.DrawPointId(i, 0x0000ff, enPointSta.Target);
        }
        ch++;
        if (ch > 6)
        {
            ch = 0;
        }
        //

    }
    private void Update()
    {

        for (int i = 0; i < Set.setVal.Width * Set.setVal.Height; i++)
        {


            switch (GameLedControl.gamePoint[i].statue)
            {
                case enPointSta.None:
                    Led_test[Framebuffer.tab_Mapping[i]].GetComponent<Image>().color = tab_PointColor[0];

                    break;
                case enPointSta.Target:
                    Led_test[Framebuffer.tab_Mapping[i]].GetComponent<Image>().color = tab_PointColor[1];

                    break;
                case enPointSta.Die:
                    Led_test[Framebuffer.tab_Mapping[i]].GetComponent<Image>().color = tab_PointColor[2];

                    break;
                case enPointSta.Rest:
                    Led_test[Framebuffer.tab_Mapping[i]].GetComponent<Image>().color = tab_PointColor[3];

                    break;
                case enPointSta.MoveRest:
                    break;
                case enPointSta.MoveDie:
                    break;
                case enPointSta.Dieing:
                    break;
                default:
                    break;
            }
            if (LedKey.KeyStatus(i))
            {
                Led_test[Framebuffer.tab_Mapping[i]].GetComponent<Image>().color = new Color(1, 0, 0);
                DrawPic.DrawPointId(i, 0xff0000, enPointSta.Die);

            }
        }
        if (startShow)
        {
            runTime -= Time.deltaTime;
            if (runTime <= 0)
            {
                runTime = 0.1f;
                GameLedControl.gamePoint[led_Now].statue = enPointSta.Rest;
                DrawPic.DrawPointId(led_Now, 0x00ff00, enPointSta.Rest);

                if (led_Now < led_End)
                {
                    led_Now++;
                }
            }
        }
    }
    void PresetPic_Init()
    {
        list_PresetPic = new List<Image>();

        int len = Set.setVal.Width * Set.setVal.Height;
        // 去除多余的

        for (; list_PresetPic.Count > len;)
        {
            Destroy(list_PresetPic[0].gameObject);
            list_PresetPic.RemoveAt(0);
        }
        // 添加不足的
        for (int i = list_PresetPic.Count; i < len; i++)
        {
            Image image = Instantiate(picOne_Prefab, presetPic_Layer).GetComponent<Image>();

            list_PresetPic.Add(image);
        }
        int widthOne = 10;
        if (Set.setVal.Width > 0)
        {
            widthOne = 400 / Set.setVal.Width;
        }
        int heightOne = 10;
        if (Set.setVal.Height > 0)
        {
            heightOne = 500 / Set.setVal.Height;
        }
        if (widthOne > heightOne)
        {
            widthOne = heightOne;
        }
        if (widthOne > 50)
        {
            widthOne = 50;
        }
        int width = widthOne * Set.setVal.Width;
        int height = widthOne * Set.setVal.Height;
        int x = 0;
        int y = 0;
        int ID = 0;
        int num = 0;

        for (int i = 0; i < list_PresetPic.Count; i++)
        {
            list_PresetPic[i].rectTransform.sizeDelta = new Vector2(widthOne, widthOne);
            list_PresetPic[i].transform.localPosition = new Vector3(x * widthOne - width * 0.5f, -y * widthOne + height * 0.5f);
            list_PresetPic[i].color = Color.black;
            if (list_PresetPic[i].gameObject.GetComponent<Button>() == null)
            {
                list_PresetPic[i].gameObject.AddComponent<Button>();
            }

            list_PresetPic[i].gameObject.AddComponent<test_ShowLight>();
            ID = (Set.setVal.Height - 1 - y) * Set.setVal.Width + x;
            num = Framebuffer.tab_Mapping[ID];
            list_PresetPic[i].name = ID.ToString() + "        " + num.ToString();

            test_ShowLight bt = list_PresetPic[i].gameObject.GetComponent<test_ShowLight>();
            bt.Init(ID, this);
            Led_test[ID] = bt;

            //  bt.onClick.AddListener(bt_OnClick(ID));
            if (++x >= Set.setVal.Width)
            {
                x = 0;
                y++;
            }

        }
        presetPic = PresetPic.LoadSetting("D-001");
        Update_CurrPresetPic();

    }

    void Update_CurrPresetPic()
    {


        Update_PresetPic(presetPic.dataBuff);
    }
    readonly Color[] tab_PointColor = { Color.black, Color.blue, Color.red, Color.green };

    void Update_PresetPic(byte[] dataBuf)
    {
        if (dataBuf == null)
        {
            presetPic_Layer.gameObject.SetActive(false);
            return;
        }

        presetPic_Layer.gameObject.SetActive(true);

        int id;
        int bufId;
        for (int i = 0; i < Set.setVal.Width && i < PresetPic.PIC_WIDTH; i++)
        {
            for (int j = 0; j < Set.setVal.Height && j < PresetPic.PIC_HEIGHT; j++)
            {
                id = j * Set.setVal.Width + i;
                bufId = j * PresetPic.PIC_WIDTH + i;
                if (bufId >= dataBuf.Length)
                    continue;
                if (id >= list_PresetPic.Count)
                    return;
                if (dataBuf[bufId] >= tab_PointColor.Length)
                {
                    list_PresetPic[id].color = Color.black;
                }
                else if (id < dataBuf.Length)
                {
                    list_PresetPic[id].color = tab_PointColor[dataBuf[bufId]];
                }
                else
                {
                    list_PresetPic[id].color = Color.black;
                }
            }
        }
    }
}
