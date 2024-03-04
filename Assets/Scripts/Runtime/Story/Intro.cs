using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;


public class Intro : MonoBehaviour {

	public float resumeOffset = -3;
	
	private PlayableDirector director;

	// Start is called before the first frame update
	IEnumerator Start() {
		director = GetComponent<PlayableDirector>();
		
		Time.timeScale = 0;
		PauseScreen.isPaused = true;
		PauseScreen.canPause = false;
		
		director.Play();
		yield return new WaitForSecondsRealtime((float) director.duration + resumeOffset);

		Time.timeScale = 1;
		PauseScreen.isPaused = false;
		PauseScreen.canPause = true;
		
		Destroy(gameObject);
	}
}