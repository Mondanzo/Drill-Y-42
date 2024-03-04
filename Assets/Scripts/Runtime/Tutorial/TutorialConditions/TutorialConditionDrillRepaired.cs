using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Tutorial/Conditions/Drill Repaired", fileName = "Tutorial Condition Drill Repaired")]
public class TutorialConditionDrillRepaired : TutorialCondition {

	public override void Setup() {
		if (FindObjectOfType<DrillData>().IsDrillRepaired) {
			Complete();
			return;
		}
		
		var controller = FindObjectOfType<DrillController>();
		controller.OnDrillRepairedFirstTime.AddListener(OnRepair);
	}


	private void OnRepair() {
		Complete();
	}
}