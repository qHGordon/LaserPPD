using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Game00_CoinIn : MonoBehaviour {
    public Image image_Coins;
    public Text text_CoinIn;
    
    int playerId = 0;
    int coins = -1;

    public void Init(int no) {
        playerId = no;
        Update_CoinIn();
    }
    void OnEnable () {
        Update_CoinIn ();
    }

	// Update is called once per frame
	void Update () {
        if (coins != FjData.g_Fj[playerId].Coins) {
            Update_CoinIn();
        }
	}

    void Update_CoinIn() {
        coins = FjData.g_Fj[playerId].Coins;
        text_CoinIn.text = coins.ToString("D2") + "/" + Set.setVal.StartCoins.ToString("D2");
    }
}
