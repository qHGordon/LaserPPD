using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class Pair<TFirst, TSecond>
{
	public TFirst First;

	public TSecond Second;

	public Pair(TFirst first, TSecond second)
	{
		First = first;
		Second = second;
	}
}

