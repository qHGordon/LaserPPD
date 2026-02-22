using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
public class UnityIO : MonoBehaviour
{



    public string portName = "COM3";//串口号
    public int baudRate = 115200;//波特率
    public Parity parity = Parity.None;//效验位
    public int dataBits = 8;//数据位
    public StopBits stopBits = StopBits.One;//停止位

    SetCode setCode;
    int uartNo;
    int baudRatio;


    public int errorSta;
    float repaireTime;
    //
    const int MAX_RECIVE_BUF_SIZE = 8192;
    byte[] reciveBuf = new byte[MAX_RECIVE_BUF_SIZE];
    int readIn;
    int readOut;

    const int MAX_SENDBUF_SIZE = 65536;
    byte[] writeBuf = new byte[MAX_SENDBUF_SIZE];
    int writeIn;
    int writeOut;

    Thread uartReadThread;
    Thread uartSendThread;
    bool threadSta;

    // 测试数据
    public int sendDataNum;
    public int receiveDataNum;
    public int maxSendBufLen;
    SerialPort hCom = null;
    public void Init(int no, int baud, SetCode setCodeFun)
    {
        setCode = setCodeFun;
        uartNo = no;
        baudRatio = baud;
        errorSta = 1;
        repaireTime = 0;

        Open();
        //if (hCom == 0)
        //    return;

        threadSta = true;
        uartReadThread = new Thread(new ThreadStart(ReadDataThread));
        uartReadThread.IsBackground = true;
        uartReadThread.Start();
       

        uartSendThread = new Thread(new ThreadStart(SendDataThread));
        uartSendThread.IsBackground = true;
        uartSendThread.Start();
    }

    void Open()
    {
        hCom = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
        hCom.ReadTimeout = 400;
        try
        {
            hCom.Open();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        //try
        //{
        //    hCom = UART_Open(uartNo, baudRatio);
        //    Main.Log("hCom_" + uartNo + ": " + hCom.ToString());
        //}
        //catch (ArgumentException e)
        //{
        //    Main.Log("UART_Open Error: " + e.Message);
        //}
        if (hCom != null)
        {
            errorSta = 0;
            repaireTime = 0;
        }
        readIn = 0;
        readOut = 0;
        writeIn = 0;
        writeOut = 0;
    }
    public void ClosePort()
    {
        try
        {
            hCom.Close();
            uartReadThread.Abort();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    public void Close()
    {
        Debug.LogError("@@");
        ClosePort();
        uartReadThread.Abort();
        uartSendThread.Abort();
        threadSta = false;
    }

    const int BUF_LENGTH = 2048;      // 单次最多发送 128 字节
    byte[] bufTemp = new byte[BUF_LENGTH];

    // 读线程
    int ReCnt = 0;
    void ReadDataThread()
    {

        int len;

        //byte[] bufTemp = new byte[BUF_LENGTH];
        while (threadSta) //threadSta
        {
           // Debug.LogError("!!!");

            Thread.Sleep(20);

            if (hCom == null)
            { 
                continue;
            }
            len = 0;
            try
            {
                Debug.LogError("???");
                // len = hCom.Read(bufTemp, 0, 128);
                if (hCom.BytesToRead > 0)
                {
                    len = hCom.Read(bufTemp, 0, 128);

                }
  
            }
            catch (ArgumentException e)
            {
                //Main.strMsg = "UartRead" + e.Message;
                Close();
                hCom = null;
                connectTime = 0;
                Debug.LogError("Uart_" + uartNo + " Error");
                continue;
            }
            if (len < 0)
            {
                Close();
                hCom = null;
                connectTime = 0;
                Debug.LogError("Uart_" + uartNo + " Error: " + len);
                continue;
            }
            if (len > 0)
            {
                ReCnt++;
                receiveDataNum += len;
                Debug.LogError("接受次数" + receiveDataNum);

                for (int i = 0; i < len; i++)
                {
                    reciveBuf[readIn] = bufTemp[i];
                    readIn++;
                    if (readIn >= reciveBuf.Length)
                    {
                        readIn = 0;
                    }
                }
            }
        }
          //  Debug.LogError("接受结束" + receiveDataNum);
        }
        // 写线程
        int SendCnt = 0;
        void SendDataThread()
        {
            const int ONCE_SENDD_LENGTH = 128;      // 单次最多发送 128 字节
            int len;
            byte[] bufTemp = new byte[ONCE_SENDD_LENGTH];

            while (threadSta)
            {

                Thread.Sleep(20);
                //
                if (hCom == null)
                    continue;

                // 单个发送缓存
                if (writeIn != writeOut)
                {
                    //发送: 单次最多发送 128 字节
                    if (writeOut < writeIn)
                        len = writeIn - writeOut;
                    else
                        len = writeBuf.Length - writeOut;
                    if (len > ONCE_SENDD_LENGTH)
                        len = ONCE_SENDD_LENGTH;
                    // 不知道会有什么影响
                    //len =
                    SendCnt++;
                                Debug.LogError("发送次数" + sendDataNum);
                    hCom.Write(writeBuf, writeOut, len);
                    if (len > 0)
                    {
                        sendDataNum += len;
                        writeOut = (writeOut + len) % writeBuf.Length;
                    }
                }
            }
        }

        //
        float lastTime;
        float connectTime = 0;
        public void CheckRead()
        {
            if (hCom == null)
            {
                if (connectTime > 0)
                {
                    connectTime -= Time.deltaTime;
                }
                else
                {
                    connectTime = 1.0f;
                    //
                    Open();
                }
                if (errorSta == 0)
                {
                    repaireTime += Time.deltaTime;
                    if (repaireTime >= 5)
                    {
                        errorSta = 1;
                    }
                }
            }
            for (; ; )
            {
                if (readIn != readOut)
                {
                    setCode(reciveBuf[readOut]);
                    readOut++;
                    if (readOut >= reciveBuf.Length)
                    {
                        readOut = 0;
                    }
                }
                else
                {
                    break;
                }
            }

            //int len;
            //int rxCnt = 0;
            //for (;;) {
            //    len = 0;
            //    try {
            //        len = UART_Read(hCom, reciveBuf, 0, 128);
            //    } catch (ArgumentException e) {
            //        Main.strMsg = "UartRead" + e.Message;
            //        return;
            //    }
            //    if (len > 0) {
            //        rxCnt += len;
            //        if (uartNo == MainRun.UART_ID_IO) {
            //            Main.receiveDataNum[0] += len;
            //            Main.connectByteTimeoutTime[0] = 1.0f;
            //        } else if (uartNo == MainRun.UART_ID_DATA) {
            //            Main.receiveDataNum[1] += len;
            //            Main.connectByteTimeoutTime[1] = 3.0f;
            //        }
            //        for (int i = 0; i < len; i++) {
            //            setCode(reciveBuf[i]);
            //        }
            //        //          Main.Log("reciveLen: "+ len.ToString());
            //    } else {
            //        break;
            //    }
            //}
            //float delayTime = Time.time - lastTime;
            //lastTime = Time.time;
            //if (uartNo == MainRun.UART_ID_IO) {
            //    if (Main.maxReceveNum[0] < rxCnt) {
            //        Main.maxReceveNum[0] = rxCnt;
            //    }
            //    if (Main.maxDelayTime[0] < delayTime) {
            //        Main.maxDelayTime[0] = delayTime;
            //    }
            //} else if (uartNo == MainRun.UART_ID_DATA) {
            //    if (Main.maxReceveNum[1] < rxCnt) {
            //        Main.maxReceveNum[1] = rxCnt;
            //    }
            //    if (Main.maxDelayTime[1] < delayTime) {
            //        Main.maxDelayTime[1] = delayTime;
            //    }
            //}        
        }

        //
        public void SendData(byte[] buf, int len)
        {

            //len = UART_Write(hCom, buf, 0, len);
            //if (uartNo == MainRun.UART_ID_IO)
            //    Main.sendDataNum[0] += len;
            //else
            //    Main.sendDataNum[1] += len;

            //if (uartNo == MainRun.UART_ID_DATA) {
            //    Main.perSendDataNum += len;
            //}
            for (int i = 0; i < len; i++)
            {
                writeBuf[writeIn] = buf[i];
                writeIn++;
                if (writeIn >= writeBuf.Length)
                    writeIn = 0;
            }
        }

        public int GetSendBufCount()
        {
            if (writeIn >= writeOut)
            {
                return writeIn - writeOut;
            }
            else
            {
                return writeIn + writeBuf.Length - writeOut;
            }
        }

    }
