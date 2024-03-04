using FMODUnity;
using UnityEngine;

public class StartLever : MonoBehaviour {

	private DrillController drillController;
	private DrillData drillData;
	private Animator animator;
	private StudioEventEmitter studioEventEmitter;

	private void Start() {
		drillController = FindObjectOfType<DrillController>();
		drillData = FindObjectOfType<DrillData>();
		animator = GetComponent<Animator>();
		studioEventEmitter = GetComponent<StudioEventEmitter>();
	}

	public void LeverActivate() {
		// only allow activating lever if drill is halted
		if (drillController.DrillHalted == true && drillData.IsDrillRepaired) {
			animator.SetTrigger("LeverActivate");
			studioEventEmitter.Play();
		}
	}
}
