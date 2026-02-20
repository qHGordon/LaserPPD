using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace LaserPPD.Core
{


[Serializable]
public class UserOne {
    
    [NonSerialized]
    public uint cardId;         // 手环卡号
    //
    public uint userId;         // 用户ID
    public string userName;     // 用户名字
    public string phoneNumber;  // 手机号码
    public int playCnt;         // 游戏次数
    public int maxScore;        // 最高分
    public int averageScore;    // 平均分

    public List<UserRecordOne> list_RecordUse;
    public List<UserRecordOne> list_ScoresRank;

    public void AddUseRecordOne (int level, int scores) {        
        UserRecordOne recordOne = new UserRecordOne ();
        recordOne.level = level;
        recordOne.scores = scores;
        if (list_RecordUse == null) {
            list_RecordUse = new List<UserRecordOne> ();
        }
        list_RecordUse.Add (recordOne);
        for ( ; list_RecordUse.Count > 10; ) {
            list_RecordUse.RemoveAt (0);
        }
    }

    public int AddScoresRank (int level, int scores) {
        UserRecordOne recordNew = new UserRecordOne ();
        recordNew.level = level;
        recordNew.scores = scores;
        //
        if (list_ScoresRank == null) {
            list_ScoresRank = new List<UserRecordOne> ();
        }
        // 排序
        int rank = -1;
        if (rank < 0 && list_ScoresRank.Count < 10) {
            rank = list_ScoresRank.Count;
            list_ScoresRank.Add (recordNew);            
        }
        for (int i = list_ScoresRank.Count - 1; i >= 0; i--) {
            if (scores > list_ScoresRank[i].scores) {
                if (i + 1 < list_ScoresRank.Count) {
                    list_ScoresRank[i + 1] = list_ScoresRank[i];
                }
                list_ScoresRank[i] = recordNew;
                rank = i;
            }
        }

        for (int i = list_ScoresRank.Count - 1; i >= 10; i--) {
            list_ScoresRank.RemoveAt (i);
        }
        return rank;
    }
}
}
