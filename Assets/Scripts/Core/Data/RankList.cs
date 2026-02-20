using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LaserPPD.Core
{


[Serializable]
public struct RankOne{
	public string playerName;
	public int score;
	public int level;
}

[Serializable]
public class RankList {
	const int MAX_RANK = 10;

	public RankOne[] rankOne = new RankOne[MAX_RANK];

    public bool UpList (int score) {
        for (int i = 0; i < rankOne.Length; i++) {
            if (score > rankOne[i].score) {
                return true;
            }
        }
        return false;
    }
    // 加入排行榜：返回-1，表示没上榜，正数表示第几名
    public int AddOne (RankOne rank) {
        if (rank.score == 0)
            return -1;
        int top = -1;
        for (int i = rankOne.Length - 1; i >= 0; i--) {
            if (rank.score <= rankOne[i].score)
                continue;
            // 加入：
            if (i + 1 < rankOne.Length) {
                rankOne[i + 1] = rankOne[i];
            }
            rankOne[i] = rank;
            top = i;
        }
        return top;
    }


    public void Clear () {
        for (int i = 0; i < rankOne.Length; i++) {
            rankOne[i].playerName = "";
            rankOne[i].score = 0;
            rankOne[i].level = 0;
        }
        //
        rankOne[0].playerName = "AAA";
        rankOne[0].score = 90;
        rankOne[0].level = 1;
        //
        rankOne[1].playerName = "BBB";
        rankOne[1].score = 80;
        rankOne[1].level = 1;
        //
        rankOne[2].playerName = "CCC";
        rankOne[2].score = 70;
        rankOne[2].level = 1;
    }
    public static void ClearOne (string fileName, RankList rankList) {
        rankList.Clear ();
        SaveRankList (fileName, rankList);
    }

    public static void ClearAll () {
        string directory = Application.persistentDataPath + "/RankList/";
        string[] files = Directory.GetFiles (directory);
        RankList rankList = new RankList ();
        
        for (int i = 0; i < files.Length; i++) {
            ClearOne (Path.GetFileName (files[i]), rankList);
        }
    }

    public static void DeleteOne (string fileName) {
        string directory = Application.persistentDataPath + "/RankList/";
        string path = directory + fileName;
        if (File.Exists (path)) {
            File.Delete (path);
        }
    }

    public static void SaveRankList (string fileName, RankList rankList) {
        string directory = Application.persistentDataPath + "/RankList/";
        string path = directory + fileName;
        //        
        if (Directory.Exists (directory) == false) {
            Directory.CreateDirectory (directory);
        }
        //Jsion保存
#if UNITY_EDITOR
        Debug.Log ("Save RankList: " + path);
#endif
        string json = JsonUtility.ToJson (rankList);
        StreamWriter fs = new StreamWriter (path);
        fs.Write (json);
        //
        fs.Close ();
    }

    public static RankList LoadRankList (string fileName) { 
        RankList rankList = null;
        if (fileName == "") {
            rankList = new RankList ();
            rankList.Clear ();
            return rankList;
        }
        string directory = Application.persistentDataPath + "/RankList/";
        string path = directory + fileName;
        if (File.Exists (path)) {
            try {
                string json = File.ReadAllText (path);
                rankList = JsonUtility.FromJson<RankList> (json);
            } catch (Exception e) {
                rankList = null;
                Debug.LogError ("RankList Load Error: " + e);
            }
        }
        bool saveFlag = false;
        if (rankList == null) {
            rankList = new RankList ();
            rankList.Clear ();
            saveFlag = true;
        }
        
        if (saveFlag) {
            SaveRankList (fileName, rankList);
        }
        return rankList;
    }
}
}
