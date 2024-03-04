using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnscaledTime : MonoBehaviour {

	private Material material;


	private void Start() {
		if (TryGetComponent(out RawImage image)) {
			material = image.material;
		}

		if (TryGetComponent(out Renderer renderer)) {
			material = renderer.material;
		}
	}

	// Update is called once per frame
	void Update() {
		if(material)
		material.SetFloat("_time", Time.unscaledTime);
	}
}