using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


public class PauseScreen : MonoBehaviour {

	public static bool canPause = true;
	public static bool isPaused = false;

	public GameObject startFocus;
	
	[CustomSeparator]
	
	public PlayableAsset enterAnimation;
	public PlayableAsset exitAnimation;

	
	private static float prePauseTimescale = 1;
	private static CursorLockMode prePauseLockmode = CursorLockMode.Locked;

	private PlayerInputs inputs;
	
	private PlayableDirector director;
	private Canvas canvas;

	private bool isClosing = false;

	private void Awake() {
		if(inputs == null) {
			inputs = new PlayerInputs();
			inputs.UI.EscapeAction.performed += EscPressed;
			inputs.Enable();
		}

		canvas = GetComponent<Canvas>();
		director = GetComponent<PlayableDirector>();
		canvas.enabled = false;
	}


	private void OnDestroy() {
		if (inputs != null) {
			inputs.Disable();
			inputs.UI.EscapeAction.performed -= EscPressed;
			inputs = null;
		}
	}


	public void EscPressed(InputAction.CallbackContext callbackContext) {
		if (!canPause) return;
		if(isPaused) ClosePauseMenu();
		else OpenPauseMenu();
	}


	private void OnApplicationFocus(bool hasFocus) {
		if(!hasFocus && canPause) OpenPauseMenu();
	}
	
	
	public static void PauseGame() {
		if (isPaused) return;
		isPaused = true;
		
		FMODUnity.RuntimeManager.PauseAllEvents(true);
		AudioListener.pause = true;

		prePauseTimescale = Time.timeScale;
		Time.timeScale = 0;
	}
	
	
	public static void UnpauseGame() {
		if (!isPaused) return;
		
		
		FMODUnity.RuntimeManager.PauseAllEvents(false);
		AudioListener.pause = false;
		
		Time.timeScale = prePauseTimescale;
		
		isPaused = false;
	}


	public void OpenPauseMenu() {
		if (!canPause) return;
		if (isPaused) return;
		
		
		Cursor.lockState = CursorLockMode.Confined;
		
		StopAllCoroutines();
		StartCoroutine(openPauseMenu());
	}


	public void ClosePauseMenu() {
		if (!canPause) return;
		if (!isPaused) return;
		if (isClosing) return;
		isClosing = true;
		
		Cursor.lockState = CursorLockMode.Locked;
		
		StopAllCoroutines();
		StartCoroutine(closePauseMenu());
	}
	

	private IEnumerator openPauseMenu() {
		PauseGame();

		EventSystem.current.SetSelectedGameObject(startFocus);

		director.Stop();
		director.playableAsset = enterAnimation;
		director.Play();
		
		canvas.enabled = true;
		yield return new WaitForSecondsRealtime((float) exitAnimation.duration);
	}


	private IEnumerator closePauseMenu() {
		director.Stop();
		director.playableAsset = exitAnimation;
		director.Play();

		yield return new WaitForSecondsRealtime((float) exitAnimation.duration);
		canvas.enabled = false;
		
		UnpauseGame();
		isClosing = false;
	}
}