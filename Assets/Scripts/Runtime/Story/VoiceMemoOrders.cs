using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MemosSet", menuName = "Story/Voice Memo Set")]
public class VoiceMemoOrders : ScriptableObject {
	public List<SubtitledAudioClip> voiceMemos;

	public static Dictionary<VoiceMemoOrders, Queue<SubtitledAudioClip>> voiceMemosDict;

	public SubtitledAudioClip GetNextMemo() {

		SubtitledAudioClip clip;
		if(voiceMemosDict[this].TryDequeue(out clip)) return clip;
		
		return null;
	}


	public void OnEnable() {
		Setup();
	}


	public void OnDisable() {
		if (voiceMemosDict != null && voiceMemosDict.ContainsKey(this)) voiceMemosDict.Remove(this);
	}


	private void Setup() {
		if (voiceMemosDict == null) {
			voiceMemosDict = new Dictionary<VoiceMemoOrders, Queue<SubtitledAudioClip>>();
		}

		if (!voiceMemosDict.ContainsKey(this)) {
			var tQueue = new Queue<SubtitledAudioClip>();

			foreach (var memo in voiceMemos) {
				tQueue.Enqueue(memo);
			}

			voiceMemosDict.Add(this, tQueue);
		}
	}


	public bool HasMemosLeft() {
		Queue<SubtitledAudioClip> tQueue;
		if (voiceMemosDict.TryGetValue(this, out tQueue)) return tQueue.Count > 0;
		return false;
	}

	public bool Exists() {
		return voiceMemosDict.ContainsKey(this);
	}
}