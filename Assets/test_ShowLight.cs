using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public class test_ShowLight : MonoBehaviour
{

    int id = 0;
    Button bt;
    Menu_AdcVerify menu_adc;
    public bool istouch = false;
    // Use this for initialization
    public void Init(int _id, Menu_AdcVerify _menu_adc)
    {
        id = _id;
        bt = GetComponent<Button>();
        bt.onClick.AddListener(bt_OnClick);
        menu_adc = _menu_adc;
        istouch = false;
    }
    private void Update()
    {
        if (istouch)
        {
            GetComponent<Image>().color = new Color(1, 1, 0);
        }
    }
    // Update is called once per frame
    readonly Color[] tab_PointColor = { Color.black, Color.blue, Color.red, Color.green };

    void bt_OnClick()
    {
        istouch = !istouch;
        if (istouch)
        {
            Framebuffer.led[Framebuffer.tab_Mapping[id]].statue = enPointSta.Rest;
            Framebuffer.Update_PointColor(Framebuffer.tab_Mapping[id], 0xffff00, enPointSta.Rest);

        }
        else
        {
            Framebuffer.led[Framebuffer.tab_Mapping[id]].statue = enPointSta.None;
            Framebuffer.Update_PointColor(Framebuffer.tab_Mapping[id], 0, enPointSta.None);

        }

    }
}
