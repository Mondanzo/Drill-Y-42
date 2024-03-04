using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TutorialDisplay : MonoBehaviour {
	[SerializeField] private TutorialDisplayStep templateStep;
	[SerializeField] private Transform tutorialStepContainer;
	private Dictionary<TutorialCondition, TutorialDisplayStep> tutorialSteps = new ();

	private Tutorial currentTutorial;

	public static TutorialDisplay INSTANCE;

	private void Awake() {
		if (INSTANCE != null) {
			Destroy(gameObject);
			return;
		}

		INSTANCE = this;
		DontDestroyOnLoad(gameObject);
	}

	
	public void SetTutorialToDisplay(Tutorial tutorial) {
		if (currentTutorial == tutorial) return;

		if (currentTutorial) ClearTutorialDisplay();

		currentTutorial = tutorial;
	}
	
	
	public void SetupTutorialDisplay() {
		if (currentTutorial == null) return;
		var step = currentTutorial.GetCurrentStep();

		foreach (var condition in step.conditions) {
			var stepDisplay = Instantiate(templateStep, tutorialStepContainer);
			stepDisplay.stepDescription.text = condition.tutorialDescription;
			stepDisplay.stepDoneBox.isOn = condition.isCompleted;
			tutorialSteps.Add(condition, stepDisplay);
		}
	}


	public void Update() {
		if (currentTutorial == null) return;
		var step = currentTutorial.GetCurrentStep();

		if (step) {
			foreach (var condition in step.conditions) {
				if (tutorialSteps.TryGetValue(condition, out TutorialDisplayStep display)) {
					display.stepDescription.text = condition.tutorialDescription;
					display.stepDoneBox.isOn = condition.isCompleted;
				}
			}
		}
	}


	public void ClearTutorialDisplay() {
		foreach (var key in tutorialSteps.Values) {
			Destroy(key.gameObject);
		}
		
		tutorialSteps.Clear();
	}
}