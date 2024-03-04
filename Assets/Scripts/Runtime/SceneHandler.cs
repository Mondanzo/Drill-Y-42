#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneHandler : MonoBehaviour {

	public static SceneHandler instance;

	private void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	public static void RestartScene() {
        LoadScene(SceneManager.GetActiveScene().name);
		Time.timeScale = 1;
	}

	
	public static void QuitGame() {
		#if UNITY_EDITOR
		EditorApplication.ExitPlaymode();
		#else
		Application.Quit();
		#endif
	}


	private IEnumerator loadSceneAsync(string scene) {
		FMODUnity.RuntimeManager.MuteAllEvents(true);
		PauseScreen.canPause = false;
		PauseScreen.PauseGame();
		
		TransitionScreen.instance.SetProgress(0, false);
		yield return TransitionScreen.instance.OpenTransitionScreen();

		var operation = SceneManager.LoadSceneAsync(scene);

		operation.allowSceneActivation = false;

		while(operation.progress < 0.9f) {
			TransitionScreen.instance.SetProgress(operation.progress);
			yield return null;
		}

		operation.allowSceneActivation = true;
		TransitionScreen.instance.SetProgress(operation.progress, false);
		
		PauseScreen.UnpauseGame();
		yield return operation;
		FMODUnity.RuntimeManager.MuteAllEvents(false);
		yield return new WaitForSecondsRealtime(0.5f);
		yield return TransitionScreen.instance.CloseTransitionScreen();

		PauseScreen.canPause = true;
	}


	private void loadScene(string scene) {
		if (TransitionScreen.instance == null) {
			SceneManager.LoadScene(scene);
			Time.timeScale = 1;
		} else {
			StartCoroutine(loadSceneAsync(scene));
		}
	}


	public static void LoadScene(string scene) {
		instance.loadScene(scene);
	}
}