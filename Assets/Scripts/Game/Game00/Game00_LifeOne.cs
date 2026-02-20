using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserPPD.Core;

public class Game00_LifeOne : MonoBehaviour {
	public GameObject lifeValue_Obj;

	public void ShowValue (bool value) {
		lifeValue_Obj.SetActive (value);
	}
}
