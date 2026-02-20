using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserPPD.Core;

public class PlayOnce : MonoBehaviour {
    //
    public bool destroy = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayEnd() {
        if (destroy) {
            Destroy(gameObject);
        } else {
            gameObject.SetActive(false);
        }        
    }
}
