using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Logo : MonoBehaviour
{
    public Image heyi;
    public Image tishi;
    public Image image_He;
    public Num num_GameNum;
    public GameObject NumHeOne;
    private Vector3 scale = new Vector3(0.8f, 0.8f, 0.8f);
    //logo
    public Image Image_logo;
    [HideInInspector]
    public string String_Company;//根据公司类别区分路径

    void OnEnable()
    {
        //切换数字
        int gameNum = Main.tab_GameId.Length;
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            image_He.sprite = Instantiate(Resources.Load<Sprite>("Pic/Game97/Idle/SeveralCombineOne_cn/91")) as Sprite;
            num_GameNum.spritePath = "Pic/Game97/Idle/SeveralCombineOne_cn";
        }
        else
        {
            image_He.sprite = Instantiate(Resources.Load<Sprite>("Pic/Game97/Idle/SeveralCombineOne_en/91")) as Sprite;
            num_GameNum.spritePath = "Pic/Game97/Idle/SeveralCombineOne_en";
        }

        num_GameNum.UpdateShow(gameNum);
        if (gameNum >= 10)
        {
            NumHeOne.transform.localPosition = new Vector3(20, 0, 0);
        }
        else
        {
            NumHeOne.transform.localPosition = new Vector3(0, 0, 0);
        }

        //切换logo
        switch (Main.COMPANY_NUM)
        {
            case 0: //公版
                String_Company = "Company_00";
                break;
            case 1: //海燕电子
                String_Company = "Company_01";
                Image_logo.transform.localPosition = new Vector3(transform.localPosition.x, 150, transform.localPosition.z);
                NumHeOne.transform.localPosition = new Vector3(transform.localPosition.x, -80, transform.localPosition.z);
                break;
            case 2:
                String_Company = "Company_02";
                Image_logo.transform.localPosition = new Vector3(transform.localPosition.x, 84, transform.localPosition.z);
                NumHeOne.transform.localPosition = new Vector3(transform.localPosition.x, -20, transform.localPosition.z);
                break;
            case 3:
                String_Company = "Company_03";
                Image_logo.transform.localPosition = new Vector3(transform.localPosition.x, 84, transform.localPosition.z);
                NumHeOne.transform.localPosition = new Vector3(transform.localPosition.x, -20, transform.localPosition.z);
                break;
            case 4:
                String_Company = "Company_04";
                //Image_logo.transform.localPosition = new Vector3(0, 84, 0);
                NumHeOne.transform.localPosition = new Vector3(0, -50, 0);
                break;
            case 5:
                String_Company = "Company_05";
                NumHeOne.transform.localPosition = new Vector3(0, -5, 0);
                break;
            case 6:
                String_Company = "Company_06";
                NumHeOne.transform.localPosition = new Vector3(0, -5, 0);
                break;
            case 7:
                String_Company = "Company_07";
                NumHeOne.transform.localPosition = new Vector3(-57, 72, 0);
                if (Main.statue == en_MainStatue.LoadScene)
                {
                    transform.localPosition = new Vector3(94, 60, 0);
                }

                //else {
                //    transform.localPosition = new Vector3(94, -138, 0);
                //}
                break;
            case 8:
                String_Company = "Company_08";

                NumHeOne.transform.localPosition = new Vector3(0, -100, 0);
                if (Main.statue == en_MainStatue.LoadScene)
                {
                    this.transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
                    Debug.Log("设置位置");
                }
                break;
            case 9:
                String_Company = "Company_09";

                NumHeOne.transform.localPosition = new Vector3(0, -100, 0);
                if (Main.statue == en_MainStatue.LoadScene)
                {
                    this.transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
                    Debug.Log("设置位置");
                }
                break;
            case 10:
                String_Company = "Company_10";

                NumHeOne.transform.localPosition = new Vector3(0, -100, 0);

                if (Main.statue == en_MainStatue.LoadScene)
                {
                    this.transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
                    Debug.Log("设置位置");
                }

                Sprite sptite;
                if (Set.setVal.Language == (int)en_Language.Chinese)
                {
                    sptite = Instantiate(Resources.Load<Sprite>(String_Company + "/Pic/Game97/Idle/BackG_cn/0002")) as Sprite;
                    heyi.sprite = sptite;
                    sptite = Instantiate(Resources.Load<Sprite>(String_Company + "/Pic/Game97/Idle/BackG_cn/0001")) as Sprite;
                    tishi.sprite = sptite;
           

                }
                else
                {
                    heyi.sprite = Instantiate(Resources.Load<Sprite>(String_Company + "/Pic/Game97/Idle/BackG_en/0002")) as Sprite;
                    tishi.sprite = Instantiate(Resources.Load<Sprite>(String_Company + "/Pic/Game97/Idle/BackG_en/0001")) as Sprite;
         
                }
                heyi.SetNativeSize();
                tishi.transform.localPosition = new Vector3(0, -95, 0);
                heyi.transform.localPosition = new Vector3(0, 100, 0);
                tishi.SetNativeSize();

                break;
            default:
                String_Company = "Company_00";
                break;
        }
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            // 枪神
            Image_logo.sprite = Instantiate(Resources.Load<Sprite>(String_Company + "/Pic/Game97/Idle/BackG_cn/0000")) as Sprite;

        }
        else
        {
            // 枪神
            Image_logo.sprite = Instantiate(Resources.Load<Sprite>(String_Company + "/Pic/Game97/Idle/BackG_en/0000")) as Sprite;
        }

        Image_logo.SetNativeSize();

        if (Main.COMPANY_NUM == 10)
        {
            NumHeOne.gameObject.SetActive(false);

        }
    }
    Vector3 nums;
    private void Update()
    {
        if (Main.COMPANY_NUM == 8 || Main.COMPANY_NUM == 9|| Main.COMPANY_NUM == 10)
        {

            if (transform.localScale == scale)
            {
                nums = Vector3.one;
            }
            else if (transform.localScale == Vector3.one)
            {
                nums = scale;
            }
            transform.localScale = Vector3.MoveTowards(transform.localScale, nums, 0.01f);

        }

    }
}
