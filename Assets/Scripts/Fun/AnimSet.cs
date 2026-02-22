using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class AnimSet {
    public static readonly int[] tab_animCount = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    // 1关动画设置
    public string descript;		// 描述
    public int animCount;       // 动画个数
	//
	public AnimOne[] animOne;  // 设置

    public AnimSet () {
        animOne = new AnimOne[Main.MAX_ANIM];
    }

    public void Default () {
        bool isNew = false;
        if (animOne == null) {
            isNew = true;
        } else if (animOne.Length != Main.MAX_ANIM) {
            isNew = true;
        }
        if (isNew) {
            animOne = new AnimOne[Main.MAX_ANIM];
        }
        for (int i = 0; i < animOne.Length; i++) {
            if (animOne[i] == null) {
                animOne[i] = new AnimOne ();
            }
            animOne[i].Default ();
        }
        //
        descript = "默认设置";
        animCount = 2;
    }

    // 保存，读取-------------------------------------------------------------------
    readonly static string[][] tab_DefaultFiles = {
        new string[]{ 
            "D-001",
            "D-002",
            "D-003",
            "D-004",
            "D-005",
            "D-006",
            "D-007",
            "D-008",
            "D-009",
            "D-010",
            "D-011",
        },
        new string[]{
            "D-001",
            "D-002",
            "D-003",
            "D-004",
            "D-005",
            "D-006",
            "D-007",
            "D-008",
            "D-009",
            "D-010",
        },
        new string[]{
            "D-001",
            "D-002",
            "D-003",
            "D-004",
            "D-005",
            "D-006",
            "D-007",
            "D-008",
            "D-009",
            "D-010",
            "D-011",
            "D-012",
        },
        //"D-012",
        //"D-013",
        //"D-014",
        //"D-015",
        //"D-016",
        //"D-017",
        //"D-018",
        //"D-019",
        //"D-020",
    };
    public static bool IsDefaultFile (string filename) {
        int gameId = Mathf.Clamp (Set.setVal.GameChoose, 0, tab_DefaultFiles.Length - 1);
        string[] tabDefaultFiles = tab_DefaultFiles[gameId];
        for (int i = 0; i < tabDefaultFiles.Length; i++) {
            if (filename == tabDefaultFiles[i]) {
                return true;
            }
        }
        return false;
    }
    public static string[] GetAllFiles () {
        string[] userFiles = Directory.GetFiles (GetDirectory ());

        int gameId = Mathf.Clamp (Set.setVal.GameChoose, 0, tab_DefaultFiles.Length - 1);
        string[] tabDefaultFiles = tab_DefaultFiles[gameId];
        string[] files = new string[tabDefaultFiles.Length + userFiles.Length];
        for (int i = 0; i < files.Length; i++) {
            if (i < tabDefaultFiles.Length) {
                files[i] = tabDefaultFiles[i];
            } else {
                files[i] = Path.GetFileName (userFiles[i - tabDefaultFiles.Length]);
            }
        }
        return files;
    }
    public static string GetDefaultDirectory () {
        int gameId = Mathf.Clamp (Set.setVal.GameChoose, 0, GameSetting.tab_GamePath.Length - 1);
        return Application.streamingAssetsPath + GameSetting.tab_GamePath[gameId] + "AnimSetting/";
    }
    // 获取文件夹：
    public static string GetDirectory () {
        int gameId = Mathf.Clamp (Set.setVal.GameChoose, 0, GameSetting.tab_GamePath.Length - 1);
        string directory = Application.persistentDataPath + GameSetting.tab_GamePath[gameId] + "AnimSetting/";
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }
        return directory;
    }
    public static void SaveSetting (string fileName, AnimSet setting) {
        if (fileName == null || fileName == "")
            return;
        string directory = GetDirectory ();
        string path = directory + fileName;
        //        
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }
    
        //Jsion保存
#if UNITY_EDITOR
        Debug.Log ("SaveAnim: " + path);
#endif
        string json = JsonUtility.ToJson (setting);
        StreamWriter fs = new StreamWriter (path);
        fs.Write (json);
        //
        fs.Close ();
    }

    public static AnimSet LoadSetting (string fileName) {
        AnimSet setting = null;
        if (fileName == null || fileName == "") {
            return null;
        }
        if (IsDefaultFile(fileName)) {
            string path = GetDefaultDirectory () + fileName;
#if UNITY_EDITOR
//            Debug.Log ("LoadLoaclAnim: " + path);
#endif
            WWW www = new WWW (path);
            long waitTime = 0;
            while (www.isDone == false) {
                if (++waitTime >= 10000000) {
                    waitTime = -1;  // 超时
                    break;
                }
            }
            if (string.IsNullOrEmpty (www.error)) {
                try {
                    //Jsion保存
                    string json = www.text;
                    setting = JsonUtility.FromJson<AnimSet> (json);
                } catch (Exception e) {
                    setting = null;
                    Debug.LogError ("AnimSetting LoadLoacl Error: " + e);
                }
            }
        } else {
            string directory = GetDirectory ();
            string path = directory + fileName;
#if UNITY_EDITOR
//            Debug.Log ("LoadAnim: " + path);
#endif
            if (File.Exists (path)) {
                //Debug.Log ("LoadedGame: ");
                try {
                    //Jsion保存
                    string json = File.ReadAllText (path);
                    setting = JsonUtility.FromJson<AnimSet> (json);
                } catch (Exception e) {
                    setting = null;
                    Debug.LogError ("AnimSetting Load Error: " + e);
                }
            }
        }
        
        if (setting == null) {
            return null;
        }
        for (int i = 0; i < setting.animOne.Length; i++) {
            if (setting.animOne[i] == null) {
                setting.animOne[i] = new AnimOne ();
            }
        }
        return setting;
    }
}
