using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserPPD.Core;

public class Game14_AudioManger : MonoBehaviour {

	public static Game14_AudioManger Instance;

	public Dictionary<string, Action> AudioDic = new Dictionary<string, Action>();

	public AudioClip ChangeClip, BgmClip;

	public AudioClip[] ReadyClip = new AudioClip[2];

	protected AudioSource AudioSource;

	protected void PlayClipEvent(AudioClip clip)
	{
		if (!clip) return;
		AudioSource.PlayOneShot(clip);
	}

	private void Awake()
	{
		if (!Instance) Instance = this;
		AudioSource = GetComponent<AudioSource>();
	}

	// Use this for initialization
	void Start () {
		AudioDic["Change"] = () => AudioSource.PlayOneShot(ChangeClip);
		AudioDic["Readying"] = () => PlayClipEvent(ReadyClip[0]);
		AudioDic["ReadyEnd"] = () =>
		{
			PlayClipEvent(ReadyClip[1]);
			AudioSource.clip = BgmClip;
			AudioSource.Play();
			AudioSource.loop = true;
		};
	}
	
	// Update is called once per frame
	void Update () {

	}
}
