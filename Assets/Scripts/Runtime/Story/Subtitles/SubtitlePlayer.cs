using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProgrammerInstrumentPlayer))]
public class SubtitlePlayer : MonoBehaviour {
	private static SubtitlePlayer instance;
	public static SubtitlePlayer SubtitlePlayerInstance;


	public static ProgrammerInstrumentPlayer player;

	public TMPro.TMP_Text textContainer;

	private Queue<SubtitledAudioClip> clipsToPlay;

	private SubtitledAudioClip currentClip;

	private bool startedPlaying = false;
	
	public void Start() {
		if (instance != null) {
			DestroyImmediate(gameObject);
		}

		player = GetComponent<ProgrammerInstrumentPlayer>();
		clipsToPlay = new Queue<SubtitledAudioClip>();
		instance = this;
		DontDestroyOnLoad(gameObject);
	}


	public void Update() {
		if (!player.IsPlaying() && startedPlaying) {
			currentClip = null;
			startedPlaying = false;
			
			// Is not playing.
			// ...
			if (clipsToPlay.Count > 0) {
				playClip(clipsToPlay.Dequeue());
			}
		}

		if (player.IsPlaying()) {
			startedPlaying = true;
			textContainer.text = currentClip.GetCurrentSubtitle(player.GetCurrentPosition());
		} else {
			textContainer.text = "";
		}
		
	}

	public void playClip(SubtitledAudioClip clip, bool cancelAnyCurrentClip = false) {
		if (clipsToPlay.Count <= 0 && currentClip == null) {
			currentClip = clip;
			player.EasyPlayClip(clip.GetClip());
			return;
		}
		
		if (cancelAnyCurrentClip) {
			currentClip = clip;
			player.Cancel();
			player.EasyPlayClip(clip.GetClip());
		} else {
			clipsToPlay.Enqueue(clip);
		}
	}

	public static void PlayClip(SubtitledAudioClip clip, bool cancelAnyCurrentclip = false) {
		if(instance) instance.playClip(clip, cancelAnyCurrentclip);
	}
}