using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LaserPPD.Core
{


public class UserManager
{
    public static string[] GetAllFiles () {
        return Directory.GetFiles (GetDirectory ());
    }
    // 获取文件夹：
    public static string GetDirectory () {
        string directory = Application.persistentDataPath + "/UserData/";
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }
        return directory;
    }
    public static void SaveData (uint cardId, UserOne userData) {        
        string fileName = cardId.ToString ("D10");
        string directory = GetDirectory ();
        string path = directory + fileName;
        //        
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }

        //Jsion保存
#if UNITY_EDITOR
        Debug.Log ("SaveUser: " + path);
#endif
        string json = JsonUtility.ToJson (userData);
        StreamWriter fs = new StreamWriter (path);
        fs.Write (json);
        //
        fs.Close ();
    }

    public static UserOne LoadUserData (uint cardId) {
        UserOne userData = null;
        string fileName = cardId.ToString ("D10");
        string directory = GetDirectory ();
        string path = directory + fileName;
#if UNITY_EDITOR
        Debug.Log ("LoadUser: " + path);
#endif
        if (File.Exists (path) == false) {
#if UNITY_EDITOR
            Debug.Log ("FileUnExsits: " + fileName);
#endif
            return null;
        }
        try {
            string json = File.ReadAllText (path);
            userData = JsonUtility.FromJson<UserOne> (json);
        } catch (Exception e) {
            userData = null;
            Debug.LogError ("AnimSetting Load Error: " + e);
        }
        if (userData != null) {
            userData.cardId = cardId;
            if (userData.list_RecordUse == null) {
                userData.list_RecordUse = new List<UserRecordOne> ();
            }
            if (userData.list_ScoresRank == null) {
                userData.list_ScoresRank = new List<UserRecordOne> ();
            }
        }
        return userData;
    }
    public static bool DeleteUser (uint cardId) {
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
