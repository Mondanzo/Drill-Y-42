using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(ProgrammerInstrumentPlayer))]
public class SubtitlePositionalPlayer : MonoBehaviour {

	public TMPro.TMP_Text textContainer;

	private Queue<SubtitledAudioClip> clipsToPlay;

	private ProgrammerInstrumentPlayer player;
	private SphereCollider triggerZone;

	private SubtitledAudioClip currentClip;

	public bool subtitlesFadeout = true;
	public AnimationCurve subtitlesFadeoutCurve = AnimationCurve.EaseInOut(0, 1, 0, 1);
	
	private bool startedPlaying = false;
	private float percentageToSource = 0f;

	public float minDistance = 5f;
	public float maxDistance = 100f;
	
	public void Start() {
		clipsToPlay = new Queue<SubtitledAudioClip>();
		player = GetComponent<ProgrammerInstrumentPlayer>();

		triggerZone = GetComponent<SphereCollider>();
		triggerZone.isTrigger = true;
	}


	public void OnTriggerStay(Collider other) {
		if (other.GetComponentInChildren<FMODUnity.StudioListener>()) {
			percentageToSource = Vector3.Distance(other.transform.position, transform.position) / triggerZone.radius;
		}
	}

	public void OnTriggerExit(Collider other) {
		if (other.GetComponentInChildren<FMODUnity.StudioListener>()) {
			percentageToSource = 1;
		}
	}


	public void Update() {
		if (!player.IsPlaying() && startedPlaying) {
			currentClip = null;
			startedPlaying = false;
			
			// Is not playing.
			// ...
			if (clipsToPlay.Count > 0) {
				PlayClip(clipsToPlay.Dequeue());
			}
		}

		if (player.IsPlaying()) {
			startedPlaying = true;
			Debug.Log(currentClip);
			textContainer.text = currentClip.GetCurrentSubtitle(player.GetCurrentPosition());
			if(subtitlesFadeout)
				textContainer.alpha = subtitlesFadeoutCurve.Evaluate(1 - Mathf.Clamp01(percentageToSource));
			else {
				textContainer.alpha = 1;
			}
		} else {
			textContainer.text = "";
		}
		
	}

	public void PlayClip(SubtitledAudioClip clip, bool cancelAnyCurrentClip = false) {
		if (clipsToPlay.Count <= 0 && currentClip == null) {
			currentClip = clip;
			player.overrideMinDistance = minDistance;
			player.overrideMaxDistance = maxDistance;
			player.EasyPlayClip(clip.GetClip());
			return;
		}
		
		if (cancelAnyCurrentClip) {
			player.Cancel();
			currentClip = clip;
			player.overrideMinDistance = minDistance;
			player.overrideMaxDistance = maxDistance;
			player.EasyPlayClip(clip.GetClip());
		} else {
			clipsToPlay.Enqueue(clip);
		}
	}
	
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, minDistance);
		
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, maxDistance);
	}
}