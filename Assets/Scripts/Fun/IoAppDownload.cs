using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class IoAppDownload
{
    public const string ioAppDirectory = "IoAPP/";
    public const string ioAppFileName = "IoAPP.bin";


    public static void GetFileInfo () {
        byte res;
        long len;

        if (Main.statue == en_MainStatue.Game_98 && Menu.statue == en_MenuStatue.MenuSta_UpdateIoApp) {
            return;
        }

        string filename = Application.persistentDataPath + "/" + ioAppDirectory + ioAppFileName;
        FileInfo fileInfo = new FileInfo (filename);
        if (fileInfo.Exists == false) {
            res = 2;
            len = 0;
#if UNITY_EDITOR
            Debug.Log ("文件不存在: " + filename);
#endif
        } else if (fileInfo.Length >= 500000) {
            // 大于500K出错
            res = 3;
            len = 0;
#if UNITY_EDITOR
            Debug.Log ("文件过大: " + fileInfo.Length);
#endif
        } else {
            res = 1;
            len = fileInfo.Length;
#if UNITY_EDITOR
            Debug.Log ("文件大小: " + len);
#endif
        }
        CmdIOUpdate.SendCmd_GetInfoRet (res, len);
    }

    const int READ_LENGTH = 4096;
    static byte[] ReadBuf = new byte[READ_LENGTH];
    public static void GetFileData (int offset, int len) {
#if UNITY_EDITOR
        Debug.Log ("下载数据 offset：" + offset + ", len: " + len);
#endif
        if (Main.statue == en_MainStatue.Game_98 && Menu.statue == en_MenuStatue.MenuSta_UpdateIoApp) {
            return;
        }

        if (len > READ_LENGTH)
            return;

        //int res;
        string filename = Application.persistentDataPath + "/" + ioAppDirectory + ioAppFileName;
        if (File.Exists (filename) == false) {
            // 文件不存在
            CmdIOUpdate.SendCmd_Error (2);
            return;
        }
        int rlen = 0;
        FileStream fStream = File.Open (filename, FileMode.Open);
        if (offset >= fStream.Length) {
            fStream.Close ();
            CmdIOUpdate.SendCmd_Error (3);    // 已超过文件大小
            return;
        }
        fStream.Seek (offset, SeekOrigin.Begin);
        rlen = fStream.Read (ReadBuf, 0, len);//READ_LENGTH
        fStream.Close ();
        if (rlen <= 0) {
            CmdIOUpdate.SendCmd_Error (4);
            return;
        }
        CmdIOUpdate.SendCmd_GetDataRet (offset, ReadBuf, rlen);
    }
}
