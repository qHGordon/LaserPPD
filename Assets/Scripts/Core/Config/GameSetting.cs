using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LaserPPD.Core
{


[Serializable]
public class GameSetting
{
    public readonly static string[] tab_GamePath = {
        "/YueDongGeZi/",
        "/LeiSheDeng/",
        "/PanYan/",
    };

    public readonly static int[][] tab_MaxLevel = {
       // new int[]{ 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30 },   //"/YueDongGeZi/",
        new int[]{ 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15  },   //"/YueDongGeZi/",
        new int[]{ 2, 3, 4, 5, 6, 7, 8, 9, 10 },                    //"/LeiSheDeng/",
        new int[]{ 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },   //"/PanYan/",
    };

    public int maxLevel;        //
    public string descript;

    public GameLevelSetting[] gameLevelSetting = new GameLevelSetting[30];
    public GameSetting()
    {
        maxLevel = AppConst.MAX_LEVEL;
        for (int i = 0; i < gameLevelSetting.Length; i++)
        {
            if (gameLevelSetting[i] == null)
            {
                gameLevelSetting[i] = new GameLevelSetting();
            }
        }
    }

    public void Default(int gameId, int playerMode)
    {
        maxLevel = 8;  //AppConst.MAX_LEVEL;
        if (gameLevelSetting.Length < AppConst.MAX_LEVEL)
        {
            gameLevelSetting = new GameLevelSetting[AppConst.MAX_LEVEL];
        }
        for (int i = 0; i < gameLevelSetting.Length; i++)
        {
            if (gameLevelSetting[i] == null)
            {
                gameLevelSetting[i] = new GameLevelSetting();
            }
            gameLevelSetting[i].Default(gameId, playerMode, i);
        }
    }


    // 保存，读取-----------------------------------------------------------
    readonly static string[][] tab_DefaultFiles = {
        new string[]{
            "D-001",
            "D-001",
            "D-001",
        },
        new string[]{
            "D-001",
            "D-002",
            "D-003",
        },
        new string[]{
            "D-001",
            "D-002",
            "D-002",
        },
    };
    public static bool IsDefaultFile(string filename)
    {
        int gameId = Mathf.Clamp(Set.setVal.GameChoose, 0, tab_DefaultFiles.Length - 1);
        string[] tabDefaultFiles = tab_DefaultFiles[gameId];
        for (int i = 0; i < tabDefaultFiles.Length; i++)
        {
            if (filename == tabDefaultFiles[i])
            {
                return true;
            }
        }
        return false;
    }
    public static string[] GetAllFiles()
    {

        string[] userFiles = Directory.GetFiles(GetDirectory());

        int gameId = Mathf.Clamp(Set.setVal.GameChoose, 0, tab_DefaultFiles.Length - 1);
        string[] tabDefaultFiles = tab_DefaultFiles[gameId];
        string[] files = new string[tabDefaultFiles.Length + userFiles.Length];

        for (int i = 0; i < files.Length; i++)
        {
            // Debug.LogError(files[i]);
            if (i < tabDefaultFiles.Length)
            {
                files[i] = tabDefaultFiles[i];
            }
            else
            {
                files[i] = Path.GetFileName(userFiles[i - tabDefaultFiles.Length]);
            }
        }
        return files;
    }
    public static string GetDefaultDirectory()
    {
        int gameId = Mathf.Clamp(Set.setVal.GameChoose, 0, tab_GamePath.Length - 1);

        return Application.streamingAssetsPath + tab_GamePath[gameId] + "GameSetting/";
    }
    //获取路径
    public static string GetDirectory()
    {
        int gameId = Mathf.Clamp(Set.setVal.GameChoose, 0, tab_GamePath.Length - 1);
        string directory = Application.persistentDataPath + tab_GamePath[gameId] + "GameSetting/";
        if (Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }
        return directory;
    }

    public static void SaveSetting(string fileName, GameSetting setting)
    {
        string directory = GetDirectory();
        string path = directory + fileName;
        //        
        if (Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        //Jsion保存
#if UNITY_EDITOR
        Debug.Log("SaveGame: " + path);
#endif
        string json = JsonUtility.ToJson(setting);
        StreamWriter fs = new StreamWriter(path);
        fs.Write(json);
        //
        fs.Close();
    }

    public static GameSetting LoadGameSetting(string fileName)
    {
        GameSetting setting = null;


        if (IsDefaultFile(fileName))
        {
            string path = GetDefaultDirectory() + fileName;
            WWW www = new WWW(path);
            long waitTime = 0;
            while (www.isDone == false)
            {
                if (++waitTime >= 10000000)
                {
                    waitTime = -1;  // 超时
                    break;
                }
            }
            if (string.IsNullOrEmpty(www.error))
            {
                try
                {
                    //Jsion保存
                    string json = www.text;
                    setting = JsonUtility.FromJson<GameSetting>(json);
                }
                catch (Exception e)
                {
                    setting = null;
                    Debug.LogError("GameSetting LoadLoacl Error: " + e);
                }
            }
        }
        else
        {
            string directory = GetDirectory();
            string path = directory + fileName; 
            if (File.Exists(path))
            {
                //Debug.Log ("LoadedGame: ");
                try
                {
                    //Jsion保存
                    string json = File.ReadAllText(path);
                    setting = JsonUtility.FromJson<GameSetting>(json);
                }
                catch (Exception e)
                {
                    setting = null;
                    Debug.LogError("GameSetting Load Error: " + e);
                }
            }
        }
        if (setting == null)
        {
            return null;
        }
        for (int i = 0; i < setting.gameLevelSetting.Length; i++)
        {
            if (setting.gameLevelSetting[i] == null)
                continue;
            // 读图案
            setting.gameLevelSetting[i].picSetting = PresetPic.LoadSetting(setting.gameLevelSetting[i].presetPicName);
            // 读动画
            setting.gameLevelSetting[i].animSetting = AnimSet.LoadSetting(setting.gameLevelSetting[i].animInfoName);
        }
        if (setting.maxLevel > setting.gameLevelSetting.Length)
        {
            setting.maxLevel = setting.gameLevelSetting.Length;
        }
        return setting;
    }
    public static void DeleteGameSetting(string filename)
    {
        string directory = GetDirectory();
        File.Delete(directory + filename);
    }
    public static void CreateGameSetting(int gameId, string filename)
    {
        GameSetting gameSetting = new GameSetting();
        gameSetting.Default(gameId, 0);
        SaveSetting(filename, gameSetting);
    }
}
}
