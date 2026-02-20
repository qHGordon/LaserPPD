using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace LaserPPD.Core
{


[Serializable]
public struct UserRecordOne
{    
    public int gameType;
    public int deviceId;
    public int level;
    public int scores;
}

[Serializable]
public class UserRecordList
{
    public uint cardId;
    public List<UserRecordOne> list_UserRecord;

    public UserRecordList () {
        list_UserRecord = new List<UserRecordOne> ();
    }
    public void AddRecordOne (int score) {
        UserRecordOne recordOne = new UserRecordOne ();
        recordOne.scores = score;
    }
}

public class UserRecord {

    public static string[] GetAllFiles () {
        return Directory.GetFiles (GetDirectory ());
    }
    // 获取文件夹：
    public static string GetDirectory () {
        string directory = Application.persistentDataPath + "/UserRecord/";
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }
        return directory;
    }
    public static void SaveData (uint cardId, UserRecordList userRecord) {
        string fileName = cardId.ToString ("D10");
        string directory = GetDirectory ();
        string path = directory + fileName;
        //        
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }

        //Jsion保存
#if UNITY_EDITOR
        Debug.Log ("SaveUserRecord: " + path);
#endif
        string json = JsonUtility.ToJson (userRecord);
        StreamWriter fs = new StreamWriter (path);
        fs.Write (json);
        //
        fs.Close ();
    }

    public static UserRecordList LoadUserRecord (uint cardId) {
        UserRecordList userRecord = null;
        string fileName = cardId.ToString ("D10");
        string directory = GetDirectory ();
        string path = directory + fileName;
#if UNITY_EDITOR
        Debug.Log ("LoadUserRecord: " + path);
#endif
        if (File.Exists (path)) {
            try {
                string json = File.ReadAllText (path);
                userRecord = JsonUtility.FromJson<UserRecordList> (json);
            } catch (Exception e) {
                userRecord = null;
                Debug.LogError ("AnimSetting Load Error: " + e);
            }
        }
        if (userRecord == null) {
            userRecord = new UserRecordList ();
        }
        if (userRecord != null) {
            userRecord.cardId = cardId;
        }
        return userRecord;
    }
    public static bool DeleteUserRecord (uint cardId) {
        string fileName = cardId.ToString ("D10");
        string directory = GetDirectory ();
        string path = directory + fileName;
        if (File.Exists (path) == false) {
            return false;
        }
        File.Delete (path);
        return true;
    }
}
}
