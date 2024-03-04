using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(DrillTriggerZone))]
public class DrillHaltPoint : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {
		GetComponent<DrillTriggerZone>().DrillEntered.AddListener(HaltDrill);
	}

	private void HaltDrill() {
		GameObject.FindObjectOfType<DrillController>().SetDrillHalt(true);
	}
}