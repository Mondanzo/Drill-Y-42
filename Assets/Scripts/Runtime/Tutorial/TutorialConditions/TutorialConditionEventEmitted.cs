using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Tutorial/Conditions/Global Event Emitted", fileName = "TC Global Event Emitted")]
public class TutorialConditionEventEmitted : TutorialCondition {
	public string eventName;
	[NonSerialized] private string finalEventName;

	[NonSerialized] private bool isEmitted = false;

	private void Awake() {
		finalEventName = eventName;
	}


	public override void UpdateCondition() {
		if(isEmitted) Complete();
	}


	private void OnEnable() {
		GlobalEvents.RegisterListener(finalEventName, Emit);
	}

	private void OnDisable() {
		GlobalEvents.UnregisterListener(finalEventName, Emit);
	}


	private void Emit() {
		isEmitted = true;
	}
}