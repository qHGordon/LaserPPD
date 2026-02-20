using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace LaserPPD.Core
{


[Serializable]
public class PresetPic
{

    [NonSerialized]
    public const int PIC_WIDTH = 100;
    [NonSerialized]
    public const int PIC_HEIGHT = 40;
    [NonSerialized]


    const int DATABUF_SIZE = PIC_WIDTH * PIC_HEIGHT;
    //
    public string descript;
    public byte[] dataBuff;    
    //public byte[,] dataBuff;// = new byte[PIC_WIDTH, PIC_WIDTH];
    //
    
    public PresetPic () {

        //dataBuff = new byte[AppConst.MAX_LED];
        dataBuff = new byte[DATABUF_SIZE];
    }
    public void Default () {
        descript = "";

        //if (dataBuff == null) {
        //    dataBuff = new byte[AppConst.MAX_LED];
        //} else if (dataBuff.Length != AppConst.MAX_LED) {
        //    dataBuff = new byte[AppConst.MAX_LED];
        //}
        //for (int i = 0; i < dataBuff.Length; i++) {
        //    dataBuff[i] = 0;
        //}
        if (dataBuff == null) {
            dataBuff = new byte[DATABUF_SIZE];
        } else if (dataBuff.Length != DATABUF_SIZE) {
            Debug.LogError ("PicLenError:" + dataBuff.Length);
            dataBuff = new byte[DATABUF_SIZE];
        }
        for (int i = 0; i < dataBuff.Length; i++) {
            dataBuff[i] = 0;
        }
        //for (int i = 0; i < PIC_WIDTH; i++) {
        //    for (int j = 0; j < PIC_WIDTH; j++) {
        //        dataBuff[i,j] = 0;
        //    }
        //}
    }

    //发送指令----------------------------------------------------------------------
    public readonly static enPointSta[] tab_PointSta = { enPointSta.Rest, enPointSta.Die, enPointSta.Target };
    public static void SendPresetPoint (int gameId, PresetPic picSetting, ref int setIndex) {
        if (setIndex >= tab_PointSta.Length)
            return;
        byte[] buf = new byte[128];
        byte pointType = (byte)tab_PointSta[setIndex];
        int bufLen = Mathf.Min (Set.setVal.Width * Set.setVal.Height, picSetting.dataBuff.Length);
        int pointLen = 0;
        int bitn = 0;
        int len = 0;

        int id;
        for (int y = 0; y < PIC_HEIGHT && y < Set.setVal.Height; y++) {
            for (int x = 0; x < PIC_WIDTH && x < Set.setVal.Width; x++) {
                if (bitn == 0) {
                    buf[len] = 0;
                }
                id = x + y * PIC_WIDTH;
                if (id >= picSetting.dataBuff.Length)
                    continue;
                if (picSetting.dataBuff[id] == pointType) {
                    buf[len] |= (byte)(1 << bitn);
                    pointLen++;
                }
                if (++bitn >= 7) {
                    bitn = 0;
                    len++;
                }
            }
        }

        if (bitn > 0) {
            len++;
        }
        if (pointLen == 0) {
            setIndex++;
            return;
        }
        //
        //if (CmdIO_YDGZ.CMD0_SendCmd_PresetPoint (gameId, setIndex, pointType, buf, (byte)len) == false) {
        //    setIndex++;
        //}
    }

    // 保存，读取-------------------------------------------------------------------
    public readonly static string[][] tab_DefaultFiles = {
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
            "D-013",
            "D-014",
            "D-015",
            "D-016",
            "D-017",
            "D-018",
            "D-019",
            "D-020",
            "D-021",
            "D-022",
            "D-023",
            "D-024",
            "D-025",
            "D-026",
            "D-027",
            "D-028",
            "D-029",
            "D-030",
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
            "D-013",
            "D-014",
            "D-015",
        },
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
        string[] files = new string[tabDefaultFiles.Length + userFiles.Length + 1];

        if (Set.setVal.Language == (int)en_Language.Chinese) {
            files[0] = "随机";
        } else {
            files[0] = "Random";
        }
        for (int i = 1; i < files.Length; i++) {
            if (i - 1 < tabDefaultFiles.Length) {
                files[i] = tabDefaultFiles[i - 1];
            } else {
                files[i] = Path.GetFileName (userFiles[i - 1 - tabDefaultFiles.Length]);
            }
        }
        return files;
    }
    public static string GetDefaultDirectory () {
        int gameId = Mathf.Clamp (Set.setVal.GameChoose, 0, GameSetting.tab_GamePath.Length - 1);
        return Application.streamingAssetsPath + GameSetting.tab_GamePath[gameId] + "PresetPic/";
    }
    // 获取文件夹：
    public static string GetDirectory () {
        int gameId = Mathf.Clamp (Set.setVal.GameChoose, 0, GameSetting.tab_GamePath.Length - 1);
        string directory = Application.persistentDataPath + GameSetting.tab_GamePath[gameId] + "PresetPic/";
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }
        return directory;
    }
    public static void SaveSetting (string fileName, PresetPic setting) {
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
       Debug.LogError ("SavePresetPic: " + path + "!");
#endif
        string json = JsonUtility.ToJson (setting);
        StreamWriter fs = new StreamWriter (path);
        fs.Write (json);
        //
        fs.Close ();
    }

    public static PresetPic LoadSetting (string fileName) {
        PresetPic setting = null;
        if (fileName == null || fileName == "") {

            setting = new PresetPic ();
            return setting;
        }
        if (IsDefaultFile (fileName)) {
            string path = GetDefaultDirectory () + fileName;
#if UNITY_EDITOR 
//            Debug.Log ("LoadLoaclPresetPic: " + path);
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
                    setting = JsonUtility.FromJson<PresetPic> (json);
                } catch (Exception e) {
                    setting = null;
                    Debug.LogError ("PresetPicSetting LoadLoacl Error: " + e);
                }
            }
        } else {
            string directory = GetDirectory ();
            string path = directory + fileName;
#if !UNITY_EDITOR
            Debug.LogError("LoadPresetPic: " + path);

#endif
            if (File.Exists (path)) {
                //Debug.Log ("LoadedGame: ");
                try {
                    //Jsion保存
                    string json = File.ReadAllText (path);
                    setting = JsonUtility.FromJson<PresetPic> (json);
                } catch (Exception e) {
                    setting = null;
#if UNITY_EDITOR
                    Debug.LogError("PresetPicSetting Load Error: " + e);
#endif

                }
            }
        }
        if (setting == null) {
            setting = new PresetPic ();
            setting.Default ();
//            Debug.LogError("PicLenError01:" + setting.dataBuff.Length);
            return setting;

        }
        //if (setting.dataBuff.Length != AppConst.MAX_LED) {
        //    setting.dataBuff = new byte[AppConst.MAX_LED];
        //}
        if (setting.dataBuff == null) {
             
            setting.dataBuff = new byte[DATABUF_SIZE];// new byte[PIC_WIDTH, PIC_WIDTH];
        } else if (setting.dataBuff.Length != DATABUF_SIZE) {

#if UNITY_EDITOR
//            Debug.LogError("PicLenError2:" + setting.dataBuff.Length);
#endif
            //  setting.dataBuff = new byte[DATABUF_SIZE];//new byte[PIC_WIDTH, PIC_WIDTH];

        }

#if UNITY_EDITOR
//        Debug.LogError("no");
#endif
        return setting;
    }
}
}
