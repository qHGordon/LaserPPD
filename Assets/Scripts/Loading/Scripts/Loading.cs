using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum en_LoadingStatue {
    Logo_Company = 0,   // 公司LOGO
    Logo_Face,          // 笑脸
    Logo_Agent,         // 代理商LOGO
    LoadData,           // 加载数据
    LoadScene,          // 加载场景
}

public class Loading : MonoBehaviour {
    public Image image_PlaneBackground;
    public Image image_Logo;
    public GameObject progress_Obj;
    public Image image_progressValue;
    //
    Sprite[] sprite_Logo;

    //动画贴图

    //异步对象
    AsyncOperation async = null;

    Image[] image_numErrorCode;

    public en_LoadingStatue statue;
    float time;
    float runTime;
    float errorTime;

    void Awake() {
        // 隐藏鼠标光标
      //  Screen.SetResolution((int)Main.TARGET_SCREEN_WIDTH, (int)Main.TARGET_SCREEN_HEIGHT, true);
    }
    // Use this for initialization
    void Start() {
        //
        Set.LoadAll();
        
        //
        string str_Company = "UI";
        //if (Main.COMPANY == 1) {
        //    str_Company = "Company_" + Main.COMPANY.ToString("D2");
        //}
        //
        string str_Language;
        if (Set.setVal.Language == (int)en_Language.Chinese) {
            str_Language = "CN";
        } else {
            str_Language = "EN";
        }
        //if (Main.COMPANY_NUM == 1) {
        //    image_Logo.sprite = Resources.Load<Sprite>("Company_" + Main.COMPANY_NUM.ToString("D2") + "/Logo/Logo_CN");
        //    image_Logo.transform.localPosition = new Vector3(0, 130);
        //} else {
        //    image_Logo.sprite = Resources.Load<Sprite>("UI/Logo/Logo_" + str_Language);
        //}
        image_Logo.SetNativeSize();

#if FPS_TEST || (UNITY_EDITOR && !SAVE_FILE)
        ChangeStatue(en_LoadingStatue.LoadScene);
#else
        ChangeStatue(en_LoadingStatue.LoadData);
#endif
    }

    IEnumerator LoadScene() {
        string sceneName;
        yield return new WaitForEndOfFrame();

        sceneName = "Game_97";
        async = SceneManager.LoadSceneAsync(sceneName);

        //读取完备后返回
        yield return async;
    }

    // Update is called once per frame
    void Update() {
        switch (statue) {
        case en_LoadingStatue.Logo_Company:     // 显示 JM logo
            if (Time.time - time >= 1) {
                ChangeStatue(en_LoadingStatue.LoadData);
            }
            break;
        case en_LoadingStatue.Logo_Face:       // 显示 笑脸 logo
            if (Time.time - time >= 1) {
                ChangeStatue(en_LoadingStatue.Logo_Agent);
            }
            break;
        case en_LoadingStatue.Logo_Agent:       // 显示 图瑞 logo
            if (Time.time - time >= 1) {
#if (UNITY_EDITOR && !SAVE_FILE)
                ChangeStatue(en_LoadingStatue.LoadScene);
                //ChangeStatue(en_LoadingStatue.LoadData);
#else
                ChangeStatue(en_LoadingStatue.LoadData);
#endif
            }
            break;

        case en_LoadingStatue.LoadData:
            if (Main.Load_Run()) {
                ChangeStatue(en_LoadingStatue.LoadScene);
            }
            break;

        case en_LoadingStatue.LoadScene:        // 显示加载进度
            if (async != null) {
                //在这里计算读取的进度，  
                //progress 的取值范围在0.1 - 1之间， 但是它不会等于1  
                //也就是说progress可能是0.9的时候就直接进入新场景了  
                //所以在写进度条的时候需要注意一下。  
                //为了计算百分比 所以直接乘以100即可  

                //有了读取进度的数值，大家可以自行制作进度条啦。  
                image_progressValue.fillAmount = async.progress;
            }
            break;
        }
    }

    void ChangeStatue(en_LoadingStatue sta) {
        //print(" : " + sta);
        statue = sta;
        time = Time.time;
        runTime = 0;
        switch (statue) {
        case en_LoadingStatue.Logo_Company:     // 显示 JM logo
            image_Logo.rectTransform.sizeDelta = new Vector2(Main.TARGET_SCREEN_WIDTH, Main.TARGET_SCREEN_HEIGHT);
            //    image_Logo.sprite = Resources.Load<Sprite>("Pic/Logo/logo_0_JIAMI");
            image_Logo.sprite = Resources.Load<Sprite>("Pic/Logo/logo_0_LaoWanTong");
            break;

        case en_LoadingStatue.Logo_Face:       // 显示 笑脸 logo
            image_Logo.sprite = Resources.Load<Sprite>("Pic/Logo/logo_1_Face");
            break;
        case en_LoadingStatue.Logo_Agent:       // 显示 图瑞 logo
            image_Logo.sprite = Resources.Load<Sprite>("Pic/Logo/logo_1_JY");
            break;

        case en_LoadingStatue.LoadData:
            //image_Logo.sprite = Resources.Load<Sprite>("Pic/Game_" + Set.setVal.GameId.ToString("D2") + "/GameLogo_Text/000" + Set.setVal.Language.ToString());
            //Main.FormatImageSizeFollowSprite(image_Logo);
            Main.Load_Start();
            break;

        case en_LoadingStatue.LoadScene:        // 显示加载进度
            //logo.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 600);
            //image_Logo.sprite = Resources.Load<Sprite>("Pic/Logo/logo_2_" + Set.setVal.Language.ToString());
            image_progressValue.fillAmount = 0;
            //开启一个异步任务
            //进入loadScene方法
            StartCoroutine(LoadScene());
            break;
        }
    }
}
