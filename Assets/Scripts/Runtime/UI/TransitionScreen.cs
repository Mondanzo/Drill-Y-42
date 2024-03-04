using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class TransitionScreen : MonoBehaviour {
	public static TransitionScreen instance;

	public PlayableAsset enterAnimation;
	public PlayableAsset exitAnimation;

	private PlayableDirector director;
	private Canvas canvas;
	[SerializeField] private Slider progressBar;

	private float preTransitionTimescale = 1;

	private float progressToSet;

	public void Awake() {
		if (instance != null) {
			DestroyImmediate(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);

		canvas = GetComponent<Canvas>();
		director = GetComponent<PlayableDirector>();
		canvas.enabled = false;
	}


	public void SetProgress(float progress, bool withLerp = true) {
		progressToSet = progress;
		if (!withLerp) progressBar.value = progress;
	}


	public void Update() {
		progressBar.value = Mathf.Lerp(progressBar.value, Mathf.Clamp01(progressToSet), 0.1f);
	}


	public IEnumerator OpenTransitionScreen() {
		director.Stop();
		director.playableAsset = enterAnimation;
		director.Play();

		canvas.enabled = true;
		yield return new WaitForSecondsRealtime((float) enterAnimation.duration);
	}


	public IEnumerator CloseTransitionScreen() {
		director.Stop();
		director.playableAsset = exitAnimation;
		director.Play();

		yield return new WaitForSecondsRealtime((float) exitAnimation.duration);
		canvas.enabled = false;
	}
}