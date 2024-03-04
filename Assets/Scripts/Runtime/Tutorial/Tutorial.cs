using System;
using System.Collections.Generic;
using UnityEngine;


public class Tutorial : MonoBehaviour {

	public List<TutorialStep> Steps;
	
	[NonSerialized]
	public List<TutorialStep> steps = new List<TutorialStep>();
	public int currentStep = 0;
	public bool activateOnStart = false;

	private void Awake() {
		foreach (var step in Steps) {
			steps.Add(Instantiate(step));
		}
	}

	private void Start() {
		if (activateOnStart) {
			TutorialDisplay.INSTANCE.SetTutorialToDisplay(this);
			SetupStep(GetCurrentStep());
		}
	}
	

	[ContextMenu("Go to next step")]
	public void GoToNextStep() {
		CleanupStep(GetCurrentStep());
		currentStep++;
		SetupStep(GetCurrentStep());
	}

	
	[ContextMenu("Go to previous step")]
	public void GoToPreviousStep() {
		CleanupStep(GetCurrentStep());
		currentStep--;
		SetupStep(GetCurrentStep());
	}
	
	
	public TutorialStep GetCurrentStep() {
		if(currentStep < 0 || currentStep >= steps.Count) return null;
		return steps[currentStep];
	}


	public void Update() {
		var step = GetCurrentStep();
		if(step) step.UpdateStep();
	}


	void CleanupStep(TutorialStep step) {
		if (step == null) return;
		step.CloseStep();
		TutorialDisplay.INSTANCE.ClearTutorialDisplay();
	}

	void SetupStep(TutorialStep step) {
		if (step == null) return;
		step.OpenStep();
		step.SetTutorial(this);
		TutorialDisplay.INSTANCE.SetupTutorialDisplay();
	}

	public void CompleteStep(TutorialStep tutorialStep) {
		if (tutorialStep == GetCurrentStep()) {
			Debug.Log("Completed current step.");
			GoToNextStep();
		} else {
			Debug.Log("Completed non-current step?");
		}
	}
}