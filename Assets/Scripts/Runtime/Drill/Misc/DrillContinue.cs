using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DrillContinue : MonoBehaviour {
	public void RunDrill() {
		FindObjectOfType<DrillController>().SetDrillHalt(false);
	}
}