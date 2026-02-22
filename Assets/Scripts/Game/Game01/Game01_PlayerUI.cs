using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game01_PlayerUI : MonoBehaviour
{
    public GameObject Scores_Obj;
    public Text text_Scores;
    public GameObject life_Obj;
    public Text text_Life;
    public GameObject[] lifeOne_Obj;
    public Image[] image_LifeOne;
    public GameObject target_Obj;
    public GameObject remainPoint_Obj;
    public Image image_RemainPointValue;
    public Text text_RemainPoint;       // 剩余点个数
    public GameObject targetButton_Obj;
    public Image image_Result;
    public Sprite[] sprite_Result;
    public Game01_ResultScore resultScore;
    //public Game00_ResultWinner resultWinner;

    //public GameObject result_Obj;
    //public Text text_Result;

    //Game00_LifeOne[] lifeOne;

    public int playerId;
    int scores;
    int life;
    int remainPoint;


    public void Awake0(int no)
    {
        playerId = no;

        //if (lifeOne == null) {
        //    int len = GameLevelSetting.tab_life.Length - 1;
        //    len = GameLevelSetting.tab_life[len - 1];
        //    lifeOne = new Game00_LifeOne[len];
        //    for (int i = 0; i < len; i++) {
        //        lifeOne[i] = Instantiate (Game01_Main.instance.lifeOne_Prefab, life_Layer).GetComponent<Game00_LifeOne> ();
        //        lifeOne[i].transform.localScale = Vector3.one * 0.85f;
        //    }
        //}
    }

    readonly string[] tab_Language = { "CN", "EN" };
    int language = -1;
    void CheckLanguage()
    {
        if (language != Set.setVal.Language || sprite_Result == null)
        {
            language = Set.setVal.Language;
            sprite_Result = Resources.LoadAll<Sprite>("UI/Result_" + tab_Language[language]);
        }
    }
    public void GameStart(int no)
    {
        playerId = no;
        CheckLanguage();
        //修改图标--语言
        //Life_Init (playernum);
        Update_Life();
        Update_Scores();
        Update_RemainPoint();
        //
        if (remainPoint_Obj != null)
        {
            remainPoint_Obj.SetActive(true);
        }
        image_Result.gameObject.SetActive(false);
        life_Obj.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (life != FjData.g_Fj[playerId].Life)
        {
            Update_Life();
        }
        if (scores != FjData.g_Fj[playerId].Scores)
        {
            Update_Scores();
        }
        if (remainPoint != FjData.g_Fj[playerId].TargetLed)
        {
            Update_RemainPoint();
        }

    }

    public void Update_MaxLife(int value)
    {
        int lifeWithOne = 1;
        if (Main.playerMode == en_PlayerMode.Free)
        {
            lifeWithOne = 4;
        }
        int count = value / lifeWithOne;
        if (value % lifeWithOne > 0)
            count++;
        int rol = 1;    // 排数
        if (count > 10)
        {
            rol = 2;
        }
        int col = count / rol;  // 列数
        if (count % rol > 0)
            col++;
        float widthOne = 200;
        float twidth = (col - 1) * widthOne;
        float maxWidth;
        float startx;
        float starty = 0;
        //if (Main.playerMode == en_PlayerMode.Free) {
        //    // 不显示时间：
        //    life_Obj.transform.localPosition = new Vector3 (0, 0);
        //    maxWidth = 1700;
        //    startx = -twidth * 0.5f;
        //    starty = (rol - 1) * widthOne * 0.5f;
        //} else 
        if (Main.playerNum > 1)
        {
            // 显示玩家号：
            life_Obj.transform.localPosition = new Vector3(0, 288);
            maxWidth = 1600;
            startx = -twidth * 0.5f;
        }
        else
        {
            // 不显示玩家号：
            life_Obj.transform.localPosition = new Vector3(-840, 433);
            maxWidth = 1240;
            startx = 0;
        }
        if (twidth <= 1)
        {
            life_Obj.transform.localScale = Vector3.one;
        }
        else
        {
            life_Obj.transform.localScale = Vector3.one * Mathf.Clamp(maxWidth / twidth, 0f, 1f);
        }

        for (int i = 0; i < lifeOne_Obj.Length; i++)
        {
            if (i < count)
            {
                lifeOne_Obj[i].SetActive(true);
                lifeOne_Obj[i].transform.localPosition = new Vector3(startx + (i % col) * widthOne, starty - (i / col) * widthOne);
            }
            else
            {
                lifeOne_Obj[i].SetActive(false);
            }
        }
    }
    public void Update_Life()
    {
        life = FjData.g_Fj[playerId].Life;
        text_Life.text = "x" + life.ToString();

        int lifeWithOne = 1;
        if (Main.playerMode == en_PlayerMode.Free)
        {
            lifeWithOne = 4;
        }
        int remainLife = life;
        for (int i = 0; i < image_LifeOne.Length; i++)
        {
            if (remainLife > 0)
            {
                image_LifeOne[i].fillAmount = Mathf.Clamp((float)remainLife / lifeWithOne, 0f, 1f);
            }
            else
            {
                image_LifeOne[i].fillAmount = 0;
            }
            remainLife -= lifeWithOne;
        }
    }

    public void Update_Scores()
    {
        scores = FjData.g_Fj[playerId].Scores;
        text_Scores.text = scores.ToString();
    }

    public void Update_RemainPoint()
    {
        remainPoint = FjData.g_Fj[playerId].TargetLed;
        text_RemainPoint.text = remainPoint.ToString();
        if (FjData.g_Fj[playerId].TargetPoint > 0)
        {
            image_RemainPointValue.fillAmount = (float)remainPoint / FjData.g_Fj[playerId].TargetPoint;
        }
    }

    public void Update_Result(byte pass)
    {
        image_Result.gameObject.SetActive(true);
        if (pass != 0)
        {
            //text_Result.text = "闯关成功";
            image_Result.sprite = sprite_Result[1];
        }
        else
        {
            //text_Result.text = "闯关失败";
            image_Result.sprite = sprite_Result[0];
        }
        image_Result.SetNativeSize();
    }
}
