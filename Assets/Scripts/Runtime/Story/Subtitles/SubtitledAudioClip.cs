using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct Subtitle {
	public string subtitleText;
	public float startTime;
	public float endTime;
}

[CreateAssetMenu(menuName = "Story/Subtitle Clip", fileName = "Subtitled Clip")]
public class SubtitledAudioClip : ScriptableObject {

	public AudioClip clip;
	public List<Subtitle> subtitles;
	
	public AudioClip GetClip() {
		return clip;
	}

	public string GetCurrentSubtitle(float time) {
		foreach (var subtitle in subtitles) {
			if (subtitle.startTime <= time && subtitle.endTime > time) return subtitle.subtitleText;
		}
		return "";
	}

	public float GetLength() {
		if (clip.loadType == AudioClipLoadType.CompressedInMemory) {
			return 0;
		}

		return clip.length;
	}
}