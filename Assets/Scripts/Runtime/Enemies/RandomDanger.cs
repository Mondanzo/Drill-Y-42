using System;
using UnityEngine;


class RandomDanger : MonoBehaviour {
	private void Awake() {
		gameObject.SetActive(false);
	}

	public void Place() {
		gameObject.SetActive(true);
	}
}