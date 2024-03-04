using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Tutorial/Conditions/Stimpack Used", fileName = "Tutorial Condition Stimpack Used")]
public class TutorialConditionStimpackUsed : TutorialCondition {

	public int requiredUses = 1;
	private int currentUses = 0;

	private Stimpack stimpack;

	public override void Setup() {
		stimpack = FindObjectOfType<Stimpack>();
		stimpack.StimpackUsed += OnUse;
	}


	private void OnUse() {
		currentUses++;

		if (requiredUses <= currentUses) {
			Complete();
			stimpack.StimpackUsed -= OnUse;
		}
	}
}