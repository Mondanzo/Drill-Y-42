using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Tutorial/Conditions/Base", fileName = "TutorialStepBasic")]
public class TutorialCondition : ScriptableObject {
	public string tutorialDescription = "Condition to complete step.";
	public bool hideInList = false;
	public bool isCompleted = false;
	public bool ignoreComplete = false;
	public bool cantBeUncompleted = false;

	public virtual void Setup() {}
	public virtual void UpdateCondition() {}

	public virtual void Complete() {
		isCompleted = true;
	}

	public virtual void Uncomplete() {
		if (cantBeUncompleted) return;
		isCompleted = false;
	}
}