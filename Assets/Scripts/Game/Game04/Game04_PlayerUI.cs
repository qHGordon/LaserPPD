using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game04_PlayerUI : MonoBehaviour
{
    public GameObject Scores_Obj;
    public Text text_Scores;
    public GameObject life_Obj;
    public Text text_Life;
    public GameObject remainPoint_Obj;
    public Image image_RemainPointValue;
    public Text text_RemainPoint;       // 剩余点个数
    public Image image_Result;
    public Sprite[] sprite_Result;
    public Game00_ResultScore resultScore;
    public Game00_ResultWinner resultWinner;

    //public GameObject result_Obj;
    //public Text text_Result;

    //Game00_LifeOne[] lifeOne;

    public int playerId;
    int scores;
    int life;
    int remainPoint;
    int maxLife;
    int maxPoint;

    public void Awake0(int no)
    {
        playerId = 0;

        //if (lifeOne == null) {
        //    int len = GameLevelSetting.tab_life.Length - 1;
        //    len = GameLevelSetting.tab_life[len - 1];
        //    lifeOne = new Game00_LifeOne[len];
        //    for (int i = 0; i < len; i++) {
        //        lifeOne[i] = Instantiate (Game02_Main.instance.lifeOne_Prefab, life_Layer).GetComponent<Game00_LifeOne> ();
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
    public void GameStart(int fulllife)
    {
        CheckLanguage();

        maxLife = fulllife;
        maxPoint = 0;
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
        if (resultWinner != null)
        {
            resultWinner.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (life != FjData.g_Fj[0].Life)
        {
            Update_Life();
        }
        if (scores != Game04_Main.instance.Score_LinShi)
        {
            Update_Scores();
        }
      
            Update_RemainPoint();
        

    }

    //void Life_Init (int playernum) {
    //    float dx = -170;
    //    float dy = 170;
    //    int maxOneRol = 10;

    //    if (playernum < 2) {
    //        life_Layer.transform.localScale = Vector3.one * 1f;
    //    } else {
    //        life_Layer.transform.localScale = Vector3.one * 0.5f;
    //        maxOneRol = 7;
    //    }
    //    if (playerId == 0) {
    //        dx *= -1;
    //    }
    //    for (int i = 0; i < lifeOne.Length; i++) {
    //        lifeOne[i].transform.localPosition = new Vector3 ((i % maxOneRol) * dx, -(i / maxOneRol) * dy);
    //    }
    //}
    public void Update_Life()
    {
        life = FjData.g_Fj[0].Life;
        if (true)//Set.setVal.GameMode == (int)en_GameMode.PlayLife
        {
            text_Life.gameObject.SetActive(true);
            text_Life.text = "x" + life.ToString();
        }
        else
        {
            text_Life.gameObject.SetActive(false);
        }

        //for (int i = 0; i < lifeOne.Length; i++) {
        //    if (i >= maxLife) {
        //        lifeOne[i].gameObject.SetActive (false);
        //        continue;
        //    }
        //    lifeOne[i].gameObject.SetActive (true);
        //    if (i < life) {
        //        lifeOne[i].ShowValue (true);
        //    } else {
        //        lifeOne[i].ShowValue (false);
        //    }
        //}

    }

    public void Update_Scores()
    {
        scores = Game04_Main.instance.Score_LinShi;
        text_Scores.text = scores.ToString();
    }

    public void Update_RemainPoint()
    {
     
 
            if (Game04_Main.instance.isClearTarage)
            {
                if (remainPoint_Obj.gameObject.activeSelf)
                {
                    remainPoint_Obj.gameObject.SetActive(false);
                }
                return;
            }
            else
            {
              
                if (!remainPoint_Obj.gameObject.activeSelf )
                {
                    remainPoint_Obj.gameObject.SetActive(true);
                }
            }
        remainPoint = Game_Map04.instance.remainPoint;
        text_RemainPoint.text = remainPoint.ToString();
    }

    public void Update_Result(byte pass)
    {
        //if (remainPoint_Obj != null) {
        //    remainPoint_Obj.SetActive (false);
        //}
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
