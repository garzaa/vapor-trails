using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class AudioResource : ScriptableObject {
	#pragma warning disable 0649
	[SerializeField] List<AudioClip> sounds;
	[SerializeField] public AudioType audioType = AudioType.WORLD;
	#pragma warning restore 0649

	public void PlayFrom(GameObject caller) {
		int idx = Random.Range(0, sounds.Count);
		caller.GetComponent<AudioSource>().PlayOneShot(sounds[idx]);
	}

	public void Play() {
		if (audioType.Equals(AudioType.WORLD)) {
			SoundManager.WorldSound(GetRandomSound());
		} else if (audioType.Equals(AudioType.UI)) {
			SoundManager.UISound(GetRandomSound());
		} else {
			Debug.LogWarning("AudioResource "+this.name+" called with a MUSIC type, it's time to write that bit");
		}
	}

	public AudioClip GetRandomSound() {
		return sounds[Random.Range(0, sounds.Count)];
	}
}

public enum AudioType {
	WORLD = 1,
	MUSIC = 2,
	UI    = 3
}
