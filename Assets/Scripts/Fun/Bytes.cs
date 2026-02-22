using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bytes {

    public static int Bit7ToBit8(byte[] outBuf, byte[] inBuf, int offset, int len) {        
        int inId = 0;
        int outId = 0;
        int size;
        byte xor;

        while (inId < len) {
            if (inId + 8 > len) {
                size = len - inId - 1;
            } else {
                size = 7;
            }
            xor = inBuf[offset + inId + size];
            for (int i = 0; i < size; i++) {
                outBuf[outId] = inBuf[offset + inId];
                if ((xor & (1 << i)) != 0) {
                    outBuf[outId] |= 0x80;
                }
                inId++;
                outId++;
            }
            inId++;
        }
        return outId;
    }

    public static int Bit8ToBit7(byte[] outBuf, int offset, byte[] inBuf, int len) {
        int inId = 0;
        int outId = 0;
        byte xor;

        while (inId < len) {
            xor = 0;
            for (int i = 0; i < 7 && inId < len; i++) {
                outBuf[offset + outId] = inBuf[inId];
                if ((inBuf[inId] & 0x80) != 0) {
                    xor |= (byte)(1 << i);
                }
                inId++;
                outId++;
            }
            outBuf[offset + outId] = xor;
            outId++;
        }
        return outId;
    }
}
