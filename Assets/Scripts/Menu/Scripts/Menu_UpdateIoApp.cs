using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
 

public class Menu_UpdateIoApp : MonoBehaviour {
    public Text text_Title;
    public Menu_Button[] button;
    public Menu_Button[] button_FileName;
    public GameObject fileNameList_Obj;
    public GameObject ioAppOne_Prefab;
    public Transform ioAppName_Layer;

    float runTime;
    string currSrcFilePath;
    string[] srcPath;

    public const string ioAppDirectory = IoAppDownload.ioAppDirectory;
    public const string ioAppFileName = IoAppDownload.ioAppFileName;
    //
    int selectId;
    float ioAppLayerLimitUp;

    // 传入的变量
    Menu menu;
    Menu_Tips menuTips;
    //
    public void Awake0(Menu mmenu) {
        //
        menu = mmenu;
        menuTips = menu.menuTips;
        for (int i = 0; i < button.Length; i++) {
            button[i].Init(i, OnClick_Button);
        }
        for (int i = 0; i < button_FileName.Length; i++) {
            button_FileName[i].Init (i, OnClick_FileNameOk);
        }
    }
    // Use this for initialization

    public void GameStart() {
        //
        fileNameList_Obj.SetActive (false);
        Update_Language ();
    }

    // Update is called once per frame
    void Update() {
        if (ioAppName_Layer.transform.localPosition.y < -15) {
            ioAppName_Layer.transform.localPosition = new Vector3 (0, -15);
        } else if (ioAppName_Layer.transform.localPosition.y > ioAppLayerLimitUp) {
            ioAppName_Layer.transform.localPosition = new Vector3 (0, ioAppLayerLimitUp);
        }

        if (menuTips.gameObject.activeSelf)
            return;
        
    }



    void Update_Language() {
        if (Set.setVal.Language == (int)en_Language.Chinese) {

            text_Title.text = "控 制 板 程 序";
            button[0].SetName("更 新");
            button[1].SetName("删 除");
            button[2].SetName("返 回");
            //
            button_FileName[0].SetName ("确定");
            button_FileName[1].SetName ("取消");
        } else {
            text_Title.text = "Control app";
            button[0].SetName("Update");
            button[1].SetName("Delete");
            button[2].SetName("Back");
            //
            button_FileName[0].SetName ("OK");
            button_FileName[1].SetName ("Cancel");
        }
    }



    List<Menu_SelectOne> list_IoAppFileName = new List<Menu_SelectOne> ();
    void UpdateSelectFile (int id) {
        for (int i = 0; i < list_IoAppFileName.Count; i++) {
            if (id != i) {
                list_IoAppFileName[i].SetIsOn (false);
            }
        }
    }
    void UpdateGameName (string[] filePath) {
        srcPath = filePath;

        for (int i = 0; i < list_IoAppFileName.Count; i++) {
            if (list_IoAppFileName[i] != null) {
                Destroy (list_IoAppFileName[i].gameObject);
            }
        }
        list_IoAppFileName.Clear ();
        //
        for (int i = 0; i < filePath.Length; i++) {
            Menu_SelectOne selectOne = Instantiate (ioAppOne_Prefab, ioAppName_Layer).GetComponent<Menu_SelectOne> ();
            selectOne.Init (i, OnValueChanged_FileNameSelect);
            selectOne.SetName (Path.GetFileName (filePath[i]));            
            list_IoAppFileName.Add (selectOne);
        }
        UpdateSelectFile (-1);
        ioAppLayerLimitUp = Mathf.Max (70 * filePath.Length + 15 - 600, -15);
    }


    void CopyFile(bool isOk) {
        if (isOk == false)
            return;
        //string uDiskRootPath = Menu_GameDownLoad.UDiskPath();
        //if (Directory.Exists(uDiskRootPath) == false) {
        //    menuTips.Init(en_MenuTipsType.Tips, "移动设备不存在", 3, true);
        //    return;
        //}
        //string scrPath = uDiskRootPath + ioAppDirectory + ioAppFileName;
        //if (File.Exists(scrPath) == false) {
        //    menuTips.Init(en_MenuTipsType.Tips, "没有控制板程序", 3, true);
        //    return;
        //}
        if (File.Exists (currSrcFilePath) == false) {
            //Debug.Log ("下载路径：" + currSrcFilePath);
            if (Set.setVal.Language == (int)en_Language.Chinese) {
                menuTips.Init ("控制板程序未找到", 3, true);
            } else {
                menuTips.Init ("Control board program not found", 3, true);
            }
            return;
        }

        string directory = Application.persistentDataPath + "/" + ioAppDirectory;
        if (Directory.Exists(directory) == false) {
            Directory.CreateDirectory(directory);
        }
        string targetpath = directory + ioAppFileName;
      
        File.Copy(currSrcFilePath, targetpath, true);
        //
        if (Set.setVal.Language == (int)en_Language.Chinese) {
            menuTips.Init("控制板程序更新完成", 3, true);
        } else {
            menuTips.Init("Control board program update complete", 3, true);
        }
    }

    void DeleteFile(bool isOk) {
        if (isOk == false) 
            return;

        string targetPath = Application.persistentDataPath + "/" + ioAppDirectory + ioAppFileName;
        File.Delete(targetPath);
        //
        if (Set.setVal.Language == (int)en_Language.Chinese) {
            menuTips.Init("删除成功", 3, true);
        } else {
            menuTips.Init("Delete success", 3, true);
        }
    }

    int GetFileNameSelectId () {
        for (int i = 0; i < list_IoAppFileName.Count; i++) {
            if (list_IoAppFileName[i].toggle.isOn) {
                return i;
            }
        }
        return - 1;
    }

    public void OnClick_FileNameOk (int id) {        
        if (id == 0) {
            // 确定
            if (srcPath == null)
                return;
            int selectId = GetFileNameSelectId ();
            if (selectId < 0 || selectId >= srcPath.Length)
                return;
            //
            currSrcFilePath = srcPath[selectId];

            fileNameList_Obj.SetActive (false);
            //
            //string uDiskRootPath = Menu_GameDownLoad.UDiskPath ();
            //if (Directory.Exists (uDiskRootPath) == false) {
            //    menuTips.Init (en_MenuTipsType.Tips, "移动设备不存在", 3, true);
            //    return;
            //}            
            if (File.Exists (currSrcFilePath) == false) {
                //Debug.Log ("下载路径：" + currSrcFilePath);
                if (Set.setVal.Language == (int)en_Language.Chinese) {
                    menuTips.Init ("控制板程序未找到", 3, true);
                } else {
                    menuTips.Init ("Control board program not found", 3, true);
                }
                return;
            }
            string targetPath = Application.persistentDataPath + "/" + ioAppDirectory + ioAppFileName;
            if (File.Exists (targetPath)) {
                if (Set.setVal.Language == (int)en_Language.Chinese) {
                    menuTips.Init ("是否替换控制板程序", CopyFile);
                } else {
                    menuTips.Init ("Do you want to replace the control board program", CopyFile);
                }
            } else {
                CopyFile (true);
            }
        } else if (id == 1) {
            // 取消
            fileNameList_Obj.SetActive (false);
        }
    }

    public void OnValueChanged_FileNameSelect (int id, bool isOne) {
        if (isOne) {
            UpdateSelectFile (id);
        }
    }

    public void OnClick_Button(int id) {
        if (menuTips.gameObject.activeSelf)
            return;
        if (id == 0) {
            // 下载
            OnClick_Update();
        } else if (id == 1) {
            // 删除
            OnClick_Delete();
        } else if (id == 2) {
            // 返回
            OnClick_Back();
        }
    }
    public void OnClick_Update() {
        
        // 显示文件列表
        string uDiskRootPath = Menu_FileDownLoad.UDiskPath ();
//        Debug.LogError(uDiskRootPath);

        if (Directory.Exists (uDiskRootPath) == false) {
            if (Set.setVal.Language == (int)en_Language.Chinese) {
                menuTips.Init ("移动设备不存在", 3, true);
            } else {
                menuTips.Init ("Mobile device does not exist", 3, true);
            }
            return;
        }

        string directory = uDiskRootPath + ioAppDirectory;
      //  Debug.LogError(directory);
        if (Directory.Exists (directory) == false) {
            if (Set.setVal.Language == (int)en_Language.Chinese) {
                menuTips.Init ("没有目标文件", 3, true);
            } else {
                menuTips.Init ("No target file", 3, true);
            }
            return;
        }
        fileNameList_Obj.SetActive (true);

        string[] paths = Directory.GetFiles (directory);
        UpdateGameName (paths);
    }

    public void OnClick_Delete() {
        string targetPath = Application.persistentDataPath + "/" + ioAppDirectory + ioAppFileName;
        if (File.Exists(targetPath)) {
            if (Set.setVal.Language == (int)en_Language.Chinese) {
                menuTips.Init ("是否删除控制板程序", DeleteFile);
            } else {
                menuTips.Init ("Do you want to delete the control board program", DeleteFile);
            }
        } else {
            if (Set.setVal.Language == (int)en_Language.Chinese) {
                menuTips.Init ("控制板程序不存在", 3, true);
            } else {
                menuTips.Init ("The control board program does not exist", 3, true);
            }
        }
    }

    public void OnClick_Back() {
        menu.ChangeStatue (en_MenuStatue.MenuSta_SysSet);
        //gameObject.SetActive(false);
        //Key.Clear();
    }
}
