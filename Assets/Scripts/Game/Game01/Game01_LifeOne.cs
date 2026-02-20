using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserPPD.Core;

public class Game01_LifeOne : MonoBehaviour {
	public GameObject lifeValue_Obj;

	public void ShowValue (bool value) {
		lifeValue_Obj.SetActive (value);
	}
}
