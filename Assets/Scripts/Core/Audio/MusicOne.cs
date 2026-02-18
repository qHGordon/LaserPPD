using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicOne : MonoBehaviour {
    public AudioSource audioSource;
    float playTime = 0.3f;
    public void Play (AudioClip audioClip, float delay) {
        audioSource.clip = audioClip;
        audioSource.volume = (float)Set.setVal.MainSoundVolume / 10;
        audioSource.PlayDelayed (delay);
        playTime = 0.3f + delay;
    }

    void Update () {
        if (playTime > 0) {
            playTime -= Time.deltaTime;
            return;
        }
        if (audioSource == null) {
            Destroy (gameObject);
            playTime = 1;
            return;
        }
        if (audioSource.isPlaying == false) {
            Destroy (gameObject);
            playTime = 1;
            return;
        }
    }
}
