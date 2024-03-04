using System;
using System.Collections.Generic;
using UnityEngine;


public class MenuUI : MonoBehaviour {

	[SerializeField] private GameObject tutorialButton;
	public GameObject startUI;

	private bool isTutorialCompleted = false;

	public List<GameObject> menuUIs;
	
	private void Awake() {
		if(tutorialButton != null) {
			isTutorialCompleted = PersistentStorage.GetBool("TUTORIAL_COMPLETED");


			if (isTutorialCompleted) {
				tutorialButton.SetActive(true);
			} else {
				tutorialButton.SetActive(false);
			}
		}
	}


	private void Start() {
		Cursor.lockState = CursorLockMode.Confined;
		OpenUI(startUI);
	}


	public void OpenUI(GameObject ui) {
		foreach (var menu in menuUIs) {
			menu.gameObject.SetActive(menu == ui);
		}
		
		if(ui.TryGetComponent(out OnActivateSetTargetSelected activateSelected)) activateSelected.SetTarget();
	}


	public void StartGameOrTutorial() {
		if (isTutorialCompleted) {
			StartGame();
		} else {
			StartTutorial();
		}
	}


	public void Toggle() {
		gameObject.SetActive(!gameObject.activeSelf);
	}
	

	private static void StartGame() {
		SceneHandler.LoadScene("Game");
	}


	private static void StartTutorial() {
		SceneHandler.LoadScene("Game Tutorial");
	}
}