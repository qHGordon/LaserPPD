using System;
using System.Collections.Generic;
using UnityEngine;
namespace Game_LaserTouch
{
    public enum GameState_LaserTouch
    {
        Idle = 0,
        Ready,
        Touch1,
        Laser,
        Touch2,
        Over
    }
    public class LaserTouch : MonoBehaviour
    {
        /// <summary>
        /// 通信信号输入
        /// </summary>
        public List<int> signalsInput = new List<int>();
        public TouchArea touchAreaStartEnd;
        public TouchArea touchArea1;
        public TouchArea touchArea2;
        public LaserArea LaserArea;
        public GameState_LaserTouch state = GameState_LaserTouch.Idle;
        public void Init()
        {
            touchArea1 = new TouchArea(4,0,60);
        }
        public void UpdateSignalsInput(List<int> signalsInput)
        {
            signalsInput.Clear();
            for (int i = 0; i < signalsInput.Count; i++)
            {
                if (signalsInput[i] == 1)
                {
                    signalsInput[i] = 0;
                }
                else
                {
                    signalsInput[i] = 1;
                }
            }
        }
    }
    /// <summary>
    /// 灯光硬件
    /// </summary>
    public class LightHardware
    {
        protected uint channel;
        protected uint startIndex;
        protected uint endIndex;
        public LightHardware(uint channel, uint startIndex, uint endIndex)
        {
            this.channel = channel;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }
        public LightHardware(uint channel, uint Length)
        {
            this.channel = channel;
            startIndex = 0;
            endIndex = Length;
        }
    }
    public class LaserArea : LightHardware
    {
        // uint 
        public LaserArea(uint channel, uint startIndex, uint endIndex)
            : base(channel, startIndex, endIndex) { }
        public void UpdateLaserOne(uint index, bool isOpen)
        {
            if (index >= (endIndex - startIndex) || index < 0)
            {
                Debug.Log("输入的TouchIndex超出范围");
                return;
            }
            CmdIO_YDGZ.CMD0_SendCmd_LedOne((int)channel, (uint)(isOpen ? 63 : 0), new uint[] { startIndex + index}, 1);
        }
        public void UpdateLasers(uint[] indexArray, bool isOpen)
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne((int)channel, (uint)(isOpen ? 63 : 0), indexArray, indexArray.Length);
        }
        public void UpdateLaserAll(bool open)
        {
            uint[] indexArray = new uint[endIndex - startIndex];
            for (uint i = 0; i < (endIndex - startIndex); i++)
            {
                indexArray[i] = i + startIndex;
            }
            CmdIO_YDGZ.CMD0_SendCmd_LedOne((int)channel, (uint)(open ? 63 : 0), indexArray, indexArray.Length);
        }
    }
    public class TouchArea : LightHardware
    {
        public TouchArea(uint channel, uint startIndex, uint endIndex)
            : base(channel, startIndex, endIndex){}
        public void UpdateColorOne(uint index, Color color)
        {
            if (index >= (endIndex - startIndex) || index < 0)
            {
                Debug.Log("输入的TouchIndex超出范围");
                return;
            }
            CmdIO_YDGZ.CMD0_SendCmd_LedOne((int)channel, new GameColor(color).ToUintColor(), new uint[] { index + startIndex }, 1);
        }
        public void UpdateColors(uint[] indexArray, Color color)
        {
            CmdIO_YDGZ.CMD0_SendCmd_LedOne((int)channel, new GameColor(color).ToUintColor(), indexArray, indexArray.Length);
        }
        public void UpdateColorAll(Color color)
        {
            uint[] indexArray = new uint[endIndex - startIndex];
            for (uint i = 0; i < (endIndex - startIndex); i++)
            {
                indexArray[i] = i + startIndex;
            }
            CmdIO_YDGZ.CMD0_SendCmd_LedOne((int)channel, new GameColor(color).ToUintColor(), indexArray, indexArray.Length);
        }
    }
    public class GameColor {
        [Range(0, 255)]
        public int r;
        [Range(0, 255)]
        public int g;
        [Range(0, 255)]
        public int b;

        public GameColor()
        {
            r = 0;
            g = 0;
            b = 0;
        }
        public GameColor(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public GameColor(Color color)
        {
            this.r = (int)(color.r * 255);
            this.g = (int)(color.g * 255);
            this.b = (int)(color.b * 255);
        }
        public static uint Color2Uint(byte r, byte g, byte b)
        {
            int temp = 0;
            temp = ((r & 0xFF) << 17) | ((g & 0xFF) << 9) | ((b & 0xFF) << 1);
            return (uint)temp;
        }
        public static Color GameColor2UnityColor(GameColor color)
        {
            return new Color(color.r / 255f, color.g / 255f, color.b / 255f);
        }
        public static GameColor UnityColor2GameColor(Color color)
        {
            return new GameColor((int)color.r * 255, (int)color.g * 255, (int)color.b * 255);
        }
        public static Color Uint2Color(uint color)
        {
            byte r = (byte)((color >> 17) & 0xFF);
            byte g = (byte)((color >> 9) & 0xFF);
            byte b = (byte)((color >> 1) & 0xFF);
            return new Color32(r, g, b, 255);
        }
            public uint ToUintColor()
        {
            return Color2Uint((byte)r, (byte)g, (byte)b);
        }
    }
}
