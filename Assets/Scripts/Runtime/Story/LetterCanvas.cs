using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;


public class LetterCanvas : MonoBehaviour {
	private static LetterCanvas instance;

	private Canvas canvas;
	
	public TMP_Text letterText;
	public PlayableAsset enter;
	public PlayableAsset exit;

	public Scrollbar scrollbar;

	private PlayableDirector director;
	
	private PlayerInputs inputs;

	private bool letterOpen = false;

	private void Awake() {
		instance = this;
		canvas = GetComponent<Canvas>();
		director = GetComponent<PlayableDirector>();
		canvas.enabled = false;
	}


	private void OnEnable() {
		if (inputs == null) {
			inputs = new PlayerInputs();
		}
		
		scrollbar.onValueChanged.AddListener(UpdatePage);
		
		inputs.UI.EscapeAction.performed += EscapeActionOnperformed;
		inputs.UI.Enable();
	}
	
	
	private void OnDisable() {
		scrollbar.onValueChanged.RemoveListener(UpdatePage);
		
		inputs.UI.EscapeAction.performed -= EscapeActionOnperformed;
		inputs.UI.Disable();
	}


	public void UpdatePage(float newVec) {
		letterText.pageToDisplay = Mathf.FloorToInt(newVec * letterText.textInfo.pageCount);
	}
	

	private void EscapeActionOnperformed(InputAction.CallbackContext obj) {
		CloseLetter();
	}

	private IEnumerator showLetterCoro(StoryLetter letter) {
		canvas.enabled = true;
		director.playableAsset = enter;
		director.Play();
		Cursor.lockState = CursorLockMode.Confined;
		yield return null;
	}
	
	private IEnumerator closeLetterCoro() {
		director.playableAsset = exit;
		director.Play();
		Cursor.lockState = CursorLockMode.Locked;
		yield return new WaitForSecondsRealtime((float) director.duration);
		canvas.enabled = false;
		
		PauseScreen.UnpauseGame();
		PauseScreen.canPause = true;
	}


	private void showLetter(StoryLetter letter) {
		if (letterOpen) return;
		PauseScreen.canPause = false;
		PauseScreen.PauseGame();
		letterOpen = true;
		
		SetupText(letter.Content);
		
		StopAllCoroutines();
		StartCoroutine(instance.showLetterCoro(letter));
	}


	private void closeLetter() {
		if (!letterOpen) return;
		letterOpen = false;
		
		StopAllCoroutines();
		StartCoroutine(instance.closeLetterCoro());
	}


	public static void ShowLetter(StoryLetter letter) {
		instance.showLetter(letter);
	}


	public static void CloseLetter() {
		instance.closeLetter();
	}


	private void SetupText(string content) {
		letterText.text = content;
		letterText.pageToDisplay = 1;
		
		letterText.ForceMeshUpdate();
		var pageCount = letterText.GetTextInfo(content).pageCount;

		if (pageCount <= 1) {
			scrollbar.gameObject.SetActive(false);
		} else {
			scrollbar.gameObject.SetActive(true);
			scrollbar.numberOfSteps = pageCount;
		}
	}
}