using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTable
{
	public enum ColorType
	{
		Color_White = 0xFFFFFF,
		Color_Red = 0xff0000,
		Color_Blue = 0x0000ff,
		Color_Green = 0x00ff00,
		Color_Black = 0x000000,
	}

	public static Vector4 GetRGBA(ColorType color)
	{
		Vector4 vec4 = new Vector4(255, 255, 255, 255);
		switch (color)
		{
			case ColorType.Color_White:
				vec4 = new Vector4(255, 255, 255, 255);
				break;
			case ColorType.Color_Red:
				vec4 = new Vector4(255, 0, 0, 255);
				break;
			case ColorType.Color_Blue:
				vec4 = new Vector4(0, 0, 255, 255);
				break;
			case ColorType.Color_Green:
				vec4 = new Vector4(0, 255, 0, 255);
				break;
			case ColorType.Color_Black:
				vec4 = new Vector4(0, 0, 0, 255);
				break;
		}
		return vec4;
	}

	//public static char GetRGBA_ToString(ColorType color)
	//{
	//	char ch = ' ';
	//	switch (color)
	//	{
	//		case ColorType.Color_White:
	//			ch = '0';
	//			break;
	//		case ColorType.Color_Red:
	//			ch = 'R';
	//			break;
	//		case ColorType.Color_Blue:
	//			ch = 'E';
	//			break;
	//		case ColorType.Color_Green:
	//			ch = 'G';
	//			break;
	//		case ColorType.Color_Black:
	//			ch = 'B';
	//			break;
	//	}
	//	return ch;
	//}

}

//public class Pair<TFirst, TSecond>
//{
//	public TFirst First;

//	public TSecond Second;

//	public Pair(TFirst first, TSecond second)
//	{
//		First = first;
//		Second = second;
//	}
//}

