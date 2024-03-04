using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAllDestroyed : MonoBehaviour {

	public UnityEvent AllDestroyed;

	public List<GameObject> ObjectsThatNeedToBeDestroyed;

	public void Update() {
		foreach (var destroyItem in ObjectsThatNeedToBeDestroyed) {
			if (destroyItem != null) return;
		}
		
		Destroy(this);
		if(AllDestroyed != null) AllDestroyed.Invoke();
	}
}