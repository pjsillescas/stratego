using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public enum AudioType { EXPLOSION, BOMBDEACTIVATION, ATTACK, WALK }

	[Serializable]
	public class AudioNode
	{
		public AudioType Type;
		public AudioSource Source;
	}

	[SerializeField]
	private List<AudioNode> Sources;

	AudioSource walkSource;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		walkSource = GetSource(AudioType.WALK);
	}

	// Update is called once per frame
	void Update()
	{

	}

	private AudioSource GetSource(AudioType type)
	{
		return Sources.Where(n => n.Type == type).Select(n => n.Source).First();
	}

	public void PlayAudio(AudioType type)
	{
		GetSource(type).Play();
	}

	public void StartWalk()
	{
		walkSource.Play();
	}

	public void StopWalk()
	{
		walkSource.Stop();
	}

}
