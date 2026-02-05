using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game13_AudioManger : MonoBehaviour {

	public static Game13_AudioManger Instance;

	public AudioClip AttackClip, HurtClip, BgmClip;

	public AudioClip[] ReadyClip = new AudioClip[2];

	public Dictionary<string, Action> AudioDic = new Dictionary<string, Action>();

	protected AudioSource AudioSource;

	private void Awake()
	{
		if (!Instance) Instance = this;
		AudioSource = GetComponent<AudioSource>();
	}

	protected void PlayClipEvent(AudioClip clip)
	{
		if (!clip) return;
		AudioSource.PlayOneShot(clip);
	}

	// Use this for initialization
	void Start () {
		AudioDic["Attack"] = () => PlayClipEvent(AttackClip);
		AudioDic["Hurt"] = () => PlayClipEvent(HurtClip);
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
