using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using LaserPPD.Core;

public enum en_LoadType
{
    Download = 0,
    Export,
}
public enum en_FileType
{
    PresetPicSet = 0,
    AnimSet,
    GameSetting,
    MusicIdle,
    MusicGame,
}
public enum en_GameLoadSta
{
    Idle = 0,
    DownLoad,
    Export,
}

public class Menu_FileDownLoad : MonoBehaviour
{
    public Text text_Title;
    public Menu_Button[] button;
    public Toggle toggle_AllSelect;
    public GameObject gameOne_Prefab;
    public Transform gameName_Layer;
    public GameObject copyTips_Obj;
    public Text text_CopyTips;


#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public const string uDiskPath = "D:/LeiSheWu/";
#elif UNITY_ANDROID
#if VER_A33
    public const string uDiskPath = "storage/usbhost1/";      // A33
#elif VER_3368
    //public const string uDiskPath = "mnt/external_sd/";      // 3368
    public const string uDiskPath = "mnt/media_rw/external_usb/";
#elif VER_H3
    public const string uDiskPath = "mnt/usbhost/Storage01/";      // H3
#else
    public const string uDiskPath = "mnt/usb_storage/USB_DISK1/udisk0/";
#endif    
#endif

    public en_GameLoadSta statue;
    float runTime;
    en_LoadType loadType;
    en_FileType fileType;
    string diskPath;
    string fileDirectory;
    //string[] srcPath;

    int copyId;
    bool showedName;
    bool confirmSta;
    //
    int selectId;
    float gameNameLayerLimitUp;

    // 传入的变量
    Menu menu;
    Menu_Tips menuTips;
    //
    public void Awake0(Menu mmenu)
    {
        //
        menu = mmenu;
        menuTips = menu.menuTips;
        for (int i = 0; i < button.Length; i++)
        {
            button[i].Init(i, OnClick_Button);
        }

        toggle_AllSelect.onValueChanged.AddListener(OnChange_AllSelect);
    }
    // Use this for initialization

    public void GameStart(en_LoadType loadtype, en_FileType filetype)
    {
        gameObject.SetActive(true);
        loadType = loadtype;
        fileType = filetype;
        switch (filetype)
        {
            case en_FileType.PresetPicSet:
                fileDirectory = "PresetPic/";
                break;
            case en_FileType.AnimSet:
                fileDirectory = "AnimSetting/";
                break;
            case en_FileType.GameSetting:
                fileDirectory = "GameSetting/";
                break;
            case en_FileType.MusicIdle:
                fileDirectory = MusicManager.MusicDirectory_Idle;
                break;
            case en_FileType.MusicGame:
                fileDirectory = MusicManager.MusicDirectory_Game;
                break;
            default:
                fileDirectory = "xx/";
                break;
        }
        //
        Update_Language(filetype);
        if (loadType == en_LoadType.Download)
        {
            Update_UDisk_FileList();
        }
        else
        {
            Update_LoaclFileList();
        }
        ChangeStatue(en_GameLoadSta.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameName_Layer.transform.localPosition.y < -15)
        {
            gameName_Layer.transform.localPosition = new Vector3(0, -15);
        }
        else if (gameName_Layer.transform.localPosition.y > gameNameLayerLimitUp)
        {
            gameName_Layer.transform.localPosition = new Vector3(0, gameNameLayerLimitUp);
        }

        if (menuTips.gameObject.activeSelf)
            return;


        switch (statue)
        {
            case en_GameLoadSta.DownLoad:
                if (list_GameName == null)
                {
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                if (copyId >= list_GameName.Count)
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        menuTips.Init("下载完成", 3, true);
                    }
                    else
                    {
                        menuTips.Init("Download complete", 3, true);
                    }
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                if (list_GameName[copyId].isOn == false)
                {
                    NextFile();
                    break;
                }
                string filename = list_GameName[copyId].GetComponentInChildren<Text>().text;
                // 源路径:U盘
                string uDiskRootPath = UDiskPath();
                if (string.IsNullOrEmpty(uDiskRootPath))
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        menuTips.Init("设备不存在，下载失败", 3, true);
                    }
                    else
                    {
                        menuTips.Init("Device does not exist, download failed", 3, true);
                    }
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                string srcpath = uDiskRootPath + fileDirectory + filename;
                if (File.Exists(srcpath) == false)
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        menuTips.Init(filename + " 不存在，下载失败", 3, true);
                    }
                    else
                    {
                        menuTips.Init(filename + " does not exist, download failed", 3, true);
                    }
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                // 目标路径:本地
                string targetpath = GetLoaclDirectory() + filename;
                if (confirmSta == false)
                {
                    confirmSta = true;
                    if (File.Exists(targetpath))
                    {
                        if (Set.setVal.Language == (int)en_Language.Chinese)
                        {
                            menuTips.Init(filename + " 已存在，是否替换", ReplaceFile);
                        }
                        else
                        {
                            menuTips.Init(filename + " already exists, do you want to replace it", ReplaceFile);
                        }
                        break;
                    }
                }
                if (showedName == false)
                {
                    CopyTips(filename);
                    break;
                }
                //Debug.Log ("下载路径： " + srcPath[copyId]);
                File.Copy(srcpath, targetpath, true);
                NextFile();
                break;

            case en_GameLoadSta.Export:
                if (list_GameName == null)
                {
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                if (copyId >= list_GameName.Count)
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        menuTips.Init("导出完成", 3, true);
                    }
                    else
                    {
                        menuTips.Init("Export complete", 3, true);
                    }
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                if (list_GameName[copyId].isOn == false)
                {
                    NextFile();
                    break;
                }
                filename = list_GameName[copyId].GetComponentInChildren<Text>().text;
                // 源路径:本地
                srcpath = GetLoaclDirectory() + filename;
                if (File.Exists(srcpath) == false)
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        menuTips.Init(filename + " 不存在，导出失败", 3, true);
                    }
                    else
                    {
                        menuTips.Init(filename + " Device does not exist, export failed", 3, true);
                    }
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                // 目标路径:U盘
                uDiskRootPath = UDiskPath();
                if (Directory.Exists(uDiskRootPath) == false)
                {
                    if (Set.setVal.Language == (int)en_Language.Chinese)
                    {
                        menuTips.Init("移动设备不存在，导出失败", 3, true);
                    }
                    else
                    {
                        menuTips.Init("Device does not exist, export failed", 3, true);
                    }
                    ChangeStatue(en_GameLoadSta.Idle);
                    break;
                }
                //
                string directory = uDiskRootPath + fileDirectory;
                if (Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }
                //
                targetpath = directory + filename;
                if (confirmSta == false)
                {
                    confirmSta = true;
                    if (File.Exists(targetpath))
                    {
                        if (Set.setVal.Language == (int)en_Language.Chinese)
                        {
                            menuTips.Init(filename + " 已存在，是否替换", ReplaceFile);
                        }
                        else
                        {
                            menuTips.Init(filename + " already exists, do you want to replace it", ReplaceFile);
                        }
                        break;
                    }
                }

                if (showedName == false)
                {
                    showedName = true;
                    CopyTips(filename);
                    break;
                }
                File.Copy(srcpath, targetpath, true);
                NextFile();
                break;
        }
    }

    void ChangeStatue(en_GameLoadSta sta)
    {
        statue = sta;
        runTime = 0;
        copyId = 0;
        showedName = false;
        confirmSta = false;

        copyTips_Obj.SetActive(false);
        switch (statue)
        {
            case en_GameLoadSta.Idle:
                break;
            case en_GameLoadSta.DownLoad:
                break;
            case en_GameLoadSta.Export:
                break;
        }
    }

    string GetLoaclDirectory()
    {
    //    Debug.LogError(Application.persistentDataPath + "/" + fileDirectory);
        return Application.persistentDataPath + "/" + fileDirectory;
    }

    void CopyTips(string tips)
    {
        if (copyTips_Obj.activeSelf == false)
            copyTips_Obj.SetActive(true);
        showedName = true;
        text_CopyTips.text = tips;
    }

    // 替换文件？
    void ReplaceFile(bool isOk)
    {
        if (isOk == false)
        {
            NextFile();
        }
    }
    void NextFile()
    {
        copyId++;
        showedName = false;
        confirmSta = false;
    }
    void StartDownload(bool isOk)
    {
        if (isOk)
        {
            ChangeStatue(en_GameLoadSta.DownLoad);
        }
        else
        {
            ChangeStatue(en_GameLoadSta.Idle);
        }
    }
    void StartExport(bool isOk)
    {
        if (isOk)
        {
            ChangeStatue(en_GameLoadSta.Export);
        }
        else
        {
            ChangeStatue(en_GameLoadSta.Idle);
        }
    }

    void Update_Language(en_FileType filetype)
    {
        if (Set.setVal.Language == (int)en_Language.Chinese)
        {
            string titleName = "";
            switch (filetype)
            {
                case en_FileType.PresetPicSet:
                    titleName = "图 案";
                    break;
                case en_FileType.AnimSet:
                    titleName = "动 画";
                    break;
                case en_FileType.GameSetting:
                    titleName = "游 戏";
                    break;
                case en_FileType.MusicIdle:
                    titleName = "待 机 音 乐";
                    break;
                case en_FileType.MusicGame:
                    titleName = "游 戏 音 乐";
                    break;
            }
            if (loadType == en_LoadType.Download)
            {
                text_Title.text = titleName + " 下 载";
                button[0].SetName("下 载");
            }
            else if (loadType == en_LoadType.Export)
            {
                text_Title.text = titleName + " 导 出";
                button[0].SetName("导出");
            }
            toggle_AllSelect.GetComponentInChildren<Text>().text = "全选";
            button[1].SetName("返 回");
        }
        else
        {
            string titleName = "";
            switch (filetype)
            {
                case en_FileType.PresetPicSet:
                    titleName = "Picture";
                    break;
                case en_FileType.AnimSet:
                    titleName = "Animation";
                    break;
                case en_FileType.GameSetting:
                    titleName = "Game";
                    break;
                case en_FileType.MusicIdle:
                    titleName = "Idle Music";
                    break;
                case en_FileType.MusicGame:
                    titleName = "Game Music";
                    break;
            }
            if (loadType == en_LoadType.Download)
            {
                text_Title.text = titleName + " Download";
                button[0].SetName("Download");
            }
            else if (loadType == en_LoadType.Export)
            {
                text_Title.text = titleName + " Export";
                button[0].SetName("Export");
            }
            toggle_AllSelect.GetComponentInChildren<Text>().text = "Select All";
            button[1].SetName("Back");
        }
    }

    List<Toggle> list_GameName = new List<Toggle>();
    void ClearFileList()
    {
        for (int i = 0; i < list_GameName.Count; i++)
        {
            if (list_GameName[i] != null)
            {
                Destroy(list_GameName[i].gameObject);
            }
        }
        list_GameName.Clear();
    }
    void UpdateFileNameList(string[] gamePath)
    {
        //srcPath = gamePath;
        //
        string fileName;
        for (int i = 0; i < gamePath.Length; i++)
        {
            fileName = Path.GetFileName(gamePath[i]);
            if (string.IsNullOrEmpty(fileName))
                continue;
            if (fileType == en_FileType.AnimSet || fileType == en_FileType.PresetPicSet || fileType == en_FileType.GameSetting)
            {
                if (fileName[0] != 'U')
                    continue;
            }
            Toggle toggle = Instantiate(gameOne_Prefab, gameName_Layer).GetComponent<Toggle>();
            toggle.GetComponentInChildren<Text>().text = fileName;
            list_GameName.Add(toggle);
        }
        gameNameLayerLimitUp = Mathf.Max(70 * gamePath.Length + 15 - 600, -15);
    }
    void Update_LoaclFileList()
    {
        string directory = GetLoaclDirectory();
        Debug.LogError(directory);
        if (File.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }
        string[] paths = Directory.GetFiles(directory);
        //
        ClearFileList();
        UpdateFileNameList(paths);
    }
    void Update_UDisk_FileList()
    {
        ClearFileList();
        //
        string uDiskRootPath = UDiskPath();
        if (Directory.Exists(uDiskRootPath) == false)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("移动设备不存在", 3, true);
            }
            else
            {
                menuTips.Init("Mobile device does not exist", 3, true);
            }
            return;
        }
        string directory = uDiskRootPath + fileDirectory;
        if (Directory.Exists(directory) == false)
        {
            return;
        }
        string[] paths = Directory.GetFiles(directory);
        UpdateFileNameList(paths);
    }

    public void OnChange_AllSelect(bool isOn)
    {
        for (int i = 0; i < list_GameName.Count; i++)
        {
            list_GameName[i].isOn = isOn;
        }
    }

    public void OnClick_Button(int id)
    {
        if (menuTips.gameObject.activeSelf)
            return;
        if (id == 0)
        {
            OnClick_Ok();
        }
        else if (id == 1)
        {
            OnClick_Back();
        }
    }
    public void OnClick_Ok()
    {
        if (statue != en_GameLoadSta.Idle)
            return;

        string uDiskRootPath = UDiskPath();
     
        if (Directory.Exists(uDiskRootPath) == false)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("移动设备不存在", 3, true);
            }
            else
            {
                menuTips.Init("Mobile device does not exist", 3, true);
            }
            return;
        }

        int selectCount = 0;
        for (int i = 0; i < list_GameName.Count; i++)
        {
            if (list_GameName[i].isOn)
            {
                selectCount++;
            }
        }
        if (selectCount == 0)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("请选择游戏", 3, true);
            }
            else
            {
                menuTips.Init("Please choose games", 3, true);
            }
            return;
        }
        if (loadType == en_LoadType.Download)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("确认下载", StartDownload);
            }
            else
            {
                menuTips.Init("Confirm download", StartDownload);
            }
        }
        else if (loadType == en_LoadType.Export)
        {
            if (Set.setVal.Language == (int)en_Language.Chinese)
            {
                menuTips.Init("确认导出", StartExport);
            }
            else
            {
                menuTips.Init("Confirm Export", StartExport);
            }
        }
    }
    public void OnClick_Back()
    {
        //menu.ChangeStatue (en_MenuStatue.MenuSta_SysSet);
        gameObject.SetActive(false);
        Key.Clear();
    }


    public static string UDiskPath()
    {

#if UNITY_EDITOR|| UNITY_STANDALONE_WIN
        return uDiskPath;
#elif VER_S905 || VER_H6
        return GetDiskPath() + "/";
#else
        return uDiskPath;
#endif
    }


#if VER_H6
    static string GetDiskPath () {
        string path;
        try {
            AndroidJavaObject context = new AndroidJavaClass ("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject> ("currentActivity");
            AndroidJavaClass jc = new AndroidJavaClass ("com.jm.jarpag.MainClass");
            path = jc.CallStatic<string> ("GetStoragePath", context, true);
        } catch (System.Exception e) {
            path = "";
            Debug.LogError ("U盘路径出错： " + e);
        }
        return path;
    }
#elif VER_S905
    static string GetDiskPath()
    {

        string path;
        try
        {
            // 3288----------------------------------
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");   //固定的，获取UnityPlayer类
            var currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");   //通过UnityPlayer类，获取Activity


            AndroidJavaClass launcher = new AndroidJavaClass("ZtlApi.ZtlManager");      //获取java类

            AndroidJavaObject appController = launcher.CallStatic<AndroidJavaObject>("GetInstance"); //获取java类的实例 和unity单例一样，这是自己写的

            // AndoridInterface callback = new AndoridInterface(callbackClass);


            appController.Call("setContext", currentActivity);  //初始化java类，把activity传进去，很多android方法调用都要用到，callback是一个回调，就是java调用Unity要用到的
            //string str = appController.CallStatic<string>("getUsbStoragePath");
            path = appController.Call<string>("getUsbStoragePath");//非静态类就用这
        }
        catch (System.Exception e)
        {
            path = "";
            Debug.LogError("U盘路径出错： " + e);
        }
        //Debug.Log("GetUDiskPath: " + path);
        return path;
    }
#endif
}
