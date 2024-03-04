using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShakeCamInTrigger : MonoBehaviour {

	public AnimationCurve distanceShake = AnimationCurve.Linear(0, 1, 10, 0);
	private List<CamShaker> shakers = new();


	private void OnDisable() {
		foreach (var shaker in shakers) {
			shaker.shakeImpact = 0;
		}
		
		shakers.Clear();
	}


	private void OnTriggerStay(Collider other) {
		if (!enabled) return;
		if (other.TryGetComponent(out CamShaker shaker)) {
			shaker.shakeImpact = Mathf.Clamp01(distanceShake.Evaluate(Vector3.Distance(other.transform.position, transform.position)));
			if(!shakers.Contains(shaker)) shakers.Add(shaker);
		}
	}

	
	private void OnTriggerExit(Collider other) {
		if(other.TryGetComponent(out CamShaker shaker)) {
			shaker.shakeImpact = 0;
			shakers.Remove(shaker);
		}
	}
}