using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
 using System.Diagnostics;
using System.IO;
using System.IO.Ports;
 

public class MainRun : MonoBehaviour
{
    //  串口
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    //   public static UnityIO uartIO = new UnityIO();
    // public static UnityIO uartYDGZ = new UnityIO( );


    public static Uart_Windows uartYDGZ = new Uart_Windows();
    public static Uart_Windows uartWx = new Uart_Windows();
    public static Uart_Windows uartIO = new Uart_Windows();

    //public static Uart_Windows uartIO01 = new Uart_Windows();
#elif UNITY_ANDROID
    public static Uart_Android uartIO = new Uart_Android ();
    public static Uart_Android uartYDGZ = new Uart_Android ();
     public static Uart_Android uartWx = new Uart_Android ();
#endif


    Process[] processPro;
    float proTime = 0;
    bool isFocus = true;

#if FPS_TEST
    static void UartSendData(byte[] buf, int len) {

    }
#endif

    //
    int CmdIO_Cnt = 3;
    int CmdIO_YDGZ_using = 3;
    int CmdIO_WeChat_using = 3;
    void Awake()
    {
        SetCode setCode = null;
        CmdIO_Cnt = 3;
#if IO_PPL
        setCode = CmdIO_PPL.LAN_SetCode;
        CmdIO_PPL.Init (uartIO.SendData);
#elif IO_PPLJL
        setCode = CmdIO_PPLJL.LAN_SetCode;
        CmdIO_PPLJL.Init(uartIO.SendData);
#else
        setCode = CmdIO.LAN_SetCode;
        CmdIO.Init (uartIO.SendData);
#endif
        CmdIO_YDGZ.Init(uartYDGZ.SendData);
        CmdIOUpdate.Init(uartYDGZ.SendData);
        CmdIO_WeChat.Init(uartWx.SendData);

        // 串口和协议初始化: 
        // 串口初始化
#if FPS_TEST
#elif UNITY_EDITOR|| UNITY_STANDALONE_WIN
        uartIO.Init(1, 38400, setCode);
        uartYDGZ.Init(3, 115200, CmdIO_YDGZ.LAN_SetCode);
        uartWx.Init(4, 9600, CmdIO_WeChat.LAN_SetCode);
        // uartIO01.Init(1, 115200, CmdIOGift.LAN_SetCode

#elif VER_A33
        uartIO.Init(2, 38400, setCode);
        uartYDGZ.Init (3, 115200, CmdIO_YDGZ.LAN_SetCode);
       // uartIO01.Init(3, 115200, CmdIOGift.LAN_SetCode);

#elif VER_S905
        uartIO.Init(4, 38400, setCode);
        uartYDGZ.Init (2, 115200, CmdIO_YDGZ.LAN_SetCode);
       // uartIO01.Init(3, 115200, CmdIOGift.LAN_SetCode);
#elif VER_S905D3
        uartIO.Init(2, 38400, setCode);
        uartYDGZ.Init (3, 115200, CmdIO_YDGZ.LAN_SetCode);
       // uartIO01.Init(3, 115200, CmdIOGift.LAN_SetCode);
#elif VER_205
        uartIO.Init(2, 38400, setCode);
        uartYDGZ.Init (1, 115200, CmdIO_YDGZ.LAN_SetCode);
       // uartIO01.Init(1, 115200, CmdIOGift.LAN_SetCode);

#elif VER_H6
        uartIO.Init(1, 38400, setCode);
        uartYDGZ.Init (2, 115200, CmdIO_YDGZ.LAN_SetCode);
       // uartIO01.Init(1, 115200, CmdIOGift.LAN_SetCode);

#elif VER_3288
        uartIO.Init(2, 38400, setCode);
      //  uartIO01.Init(1, 115200, CmdIOGift.LAN_SetCode);
#else
        // #error  "*****未初始化串口*****"
#endif

    }

    // Use this for initialization
    private void OnGUI()
    {
       
    }
    void OnDestroy()
    {
        //关闭串口
        uartIO.Close();
        uartYDGZ.Close();
          uartWx.Close();
        //
        Resources.UnloadUnusedAssets();
    }

    // Update is called once per frame
    void Update()
    {
        
        // 加密板串口
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        //        Main.uartData.CheckRead();
        
#endif

#if FPS_TEST
#else
        uartIO.CheckRead();
        uartYDGZ.CheckRead();
        uartWx.CheckRead();
        CmdIO_YDGZ.CheckConnect();
        CmdIOUpdate.CheckConnect();

        CmdIO_WeChat.CheckConnect();
#endif
        //IO.CheckSend();
        //	pAction.CodeTableCheck ();

#if UNITY_EDITOR
        Key.KeyTest_ForKeybord();
#endif
        PAction.Check();


#if DEBUG_TEST || FPS_TEST // 测试模式
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
            return;
        }
#endif
    }

}
