using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/Tutorial Step")]
public class TutorialStep : ScriptableObject {
	
	public List<TutorialCondition> Conditions;
	public string emitOnStart;
	public string emitOnCompleted;
	
	[NonSerialized]
	public List<TutorialCondition> conditions = new List<TutorialCondition>();

	private Tutorial tutorial;

	public void CompleteStep() {
		tutorial.CompleteStep(this);
		if(emitOnCompleted.Trim() != "") GlobalEvents.Emit(emitOnCompleted.Trim());
	}

	public void SetTutorial(Tutorial newTutorial) {
		tutorial = newTutorial;
	}

	public void OpenStep() {
		foreach (var condition in Conditions) {
			var newInstance = Instantiate(condition);
			conditions.Add(newInstance);
		}
		
		if(emitOnStart.Trim() != "") GlobalEvents.Emit(emitOnStart.Trim());

		foreach (var condition in conditions) {
			condition.Setup();
		}
	}


	public void UpdateStep() {
		var done = true;
		
		foreach (var condition in conditions) {
			condition.UpdateCondition();
			done = done && (condition.isCompleted || condition.ignoreComplete);
		}
		
		if (done) {
			CompleteStep();
		}
	}
	

	public void CloseStep() {
		foreach (var condition in conditions) {
			Destroy(condition);
		}
		
		conditions.Clear();
	}
}