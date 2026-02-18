using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MusicManager : MonoBehaviour {
	public GameObject musicOne_Prefab;

	public AudioClip audioClip_ShowLevel;
	public AudioClip audioClip_OpenLed;
	public AudioClip audioClip_Correct;	// 正确
	public AudioClip audioClip_Fails;   // 错误
	public AudioClip audioClip_Bomb;    // 墙灯
	public AudioClip audioClip_EndEff;

	public AudioClip[] audioClip_TalkCN;
	public AudioClip[] audioClip_TalkEN;
	//
	public AudioClip[] audioClip_BGM;
	public AudioClip[] audioClip_Leishe_BGM;
	public AudioClip audioClip_Idle;

	public const string MusicDirectory_Idle = "Music/Idle/";
	public const string MusicDirectory_Game = "Music/Game/";

	public static MusicManager instance;
	void Awake () {
		instance = this;
	}

	public void PlayOne (AudioClip audioClip, float delay) {
		MusicOne musicOne = Instantiate (musicOne_Prefab, transform).GetComponent<MusicOne> ();

		musicOne.Play (audioClip, delay);
 
	}

	public void Play_ShowLevel () {
		PlayOne (audioClip_ShowLevel, 0);
	}
	public void Play_OpenLed () {
		PlayOne (audioClip_OpenLed, 0);
	}
	public void Play_Correct () {
		PlayOne (audioClip_Correct, 0);
	}
	public void Play_Fails () {
		PlayOne (audioClip_Fails, 0);
	}
	public void Play_Bomb () {
		PlayOne (audioClip_Bomb, 0);
	}


	public void Play_Talk (int id, float delay) {
		AudioClip[] audioClips = audioClip_TalkCN;
		if (Set.setVal.Language == (int)en_Language.English) {
			audioClips = audioClip_TalkEN;
		}
		if (id >= 0 && id < audioClips.Length) {
			PlayOne (audioClips[id], delay);
		}
	}

	int idleMusicIndex = 0;
	int gameMusicIndex = 0;


	AudioClip LoadLoaclMusic (string fileName) {
		WWW www = new WWW (fileName);
		if (www == null)
			return null;
		float time = Time.time;
		while (www.isDone == false) {
			if (Time.time - time > 1) {
				www.Dispose ();
				return null;
			}
		}
		AudioClip audioClip = www.GetAudioClip ();
		www.Dispose ();
		return audioClip;
	}
	AudioClip LoadLoaclMusic (string subDirectory, ref int index) {
		string directory = Application.persistentDataPath + "/" + subDirectory;
		if (Directory.Exists (directory) == false) {
			return null;
		}
		AudioClip audioClip;
		string[] files = Directory.GetFiles (directory);
		int id = index;
		for (int i = 0; i < files.Length; i++) {
			if (++id >= files.Length) {
				id = 0;
			}
			audioClip = LoadLoaclMusic (files[id]);
			if (audioClip != null) {
				index = id;
				return audioClip;
			}
		}
		return null;
	}

	public AudioClip GetAudioClip_Idle () {

        AudioClip audioClip;
        int a = 0;
        if (Set.setVal.GameChoose == 1)
        {
            a = Random.Range(0, audioClip_Leishe_BGM.Length);
            audioClip = audioClip_Leishe_BGM[a];
        }
        else
        {
            a = Random.Range(0, audioClip_BGM.Length);
            audioClip = audioClip_BGM[a];
        }
  
          audioClip = LoadLoaclMusic (MusicDirectory_Idle, ref idleMusicIndex);
        if (audioClip != null)
        {
            return audioClip;
        }

        if (++idleMusicIndex >= audioClip_BGM.Length)
        {
            gameMusicIndex = 0;
        }
        return audioClip_Idle;
	}
	public AudioClip GetAudioClip_Game () {
		AudioClip audioClip = LoadLoaclMusic (MusicDirectory_Game, ref gameMusicIndex);
		if (audioClip != null) {
			return audioClip;
		}
		//
		if (++gameMusicIndex >= audioClip_BGM.Length) {
			gameMusicIndex = 0;
		}
		return audioClip_BGM[gameMusicIndex];

	}
    public AudioClip GetAudioClip_LeiShe_Game () {
		AudioClip audioClip = LoadLoaclMusic (MusicDirectory_Game, ref gameMusicIndex);
		if (audioClip != null) {
			return audioClip;
		}
		//
		if (++gameMusicIndex >= audioClip_Leishe_BGM.Length) {
			gameMusicIndex = 0;
		}
		return audioClip_Leishe_BGM[gameMusicIndex];
	}
}
