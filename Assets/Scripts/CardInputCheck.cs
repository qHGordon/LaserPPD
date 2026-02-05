using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardInputCheck :MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Set.setVal.GameMode == (int)en_GameMode.CardId) {
            CardIdInputCheck ();
        }
        if (cardInputed > 0) {
            cardInputed--;
        }
    }

    float inCdTime = 0;
    public static uint inCardId;
    public static int cardIdLen = 0;
    public static int cardInputed = 0;
    void CardIdInputOne (int c) {
        if (c < 0 || c >= 10)
            return;
        if (inCdTime <= 0) {
            inCardId = 0;
            cardIdLen = 0;
        }
        inCdTime = 0.2f;
        inCardId *= 10;
        inCardId += (uint)c;
        cardIdLen++;
    }
    void CardIdEnd () {
        if (inCdTime > 0 && cardIdLen >= 10) {
            inCdTime = 0;
            cardIdLen = 0;
            cardInputed = 2;
#if UNITY_EDITOR
            string format = "D" + cardIdLen;
            Debug.Log ("CardId: " + inCardId.ToString(format));
#endif
            //inCardId = 0;
        } else {
#if UNITY_EDITOR
            string format = "D" + cardIdLen;
            Debug.Log ("CardError: " + inCardId.ToString (format));
#endif
        }
    }


    public static bool GetEnterKeyCardId () {
#if UNITY_EDITOR
        if (Input.GetKeyDown (KeyCode.Return)) {
            return true;
        }
#else
        if (Input.GetKeyDown (KeyCode.JoystickButton0)) {
            return true;
        }
#endif
        return false;
    }
    void CardIdInputCheck () {
        if (inCdTime > 0) {
            inCdTime -= Time.fixedDeltaTime;
        }
#if DEBUG_TEST //&& false
        //if (Input.anyKeyDown) {
        //    Event e = Event.current;
        //    if (e.isKey) {
        //        Debug.Log ("Key: " + Input.key);
        //    }
        //}
        //for (int i = 0; i < 600; i++) {
        //    if (Input.GetKeyDown ((KeyCode)i)) {
        //        Debug.Log ("Key: " + (KeyCode)i);
        //    }
        //}
#endif
        //for (int i = 0; i < 10; i++) {
        //    if (Input.GetKeyDown (KeyCode.Alpha0 + i)) {
        //        CardIdInputOne (i);
        //    }
        //}
        //if (GetEnterKeyCardId ()) {

        //}
    }

    void OnGUI () {
        for (int i = 0; i < 10; i++) {
            string str = "Alpha" + i.ToString ();
            if (Event.current.Equals (Event.KeyboardEvent (str))) {
                CardIdInputOne (i);
#if UNITY_EDITOR || DEBUG_TEST
                Debug.Log (str);
#endif
            }
        }
#if UNITY_EDITOR
        if (Event.current.Equals (Event.KeyboardEvent ("Return"))) {
            CardIdEnd ();
        }
#else
        if (Event.current.Equals (Event.KeyboardEvent ("JoystickButton0"))) {
            CardIdEnd ();
        }
#endif
    }


}
