using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimbarInternal : MonoBehaviour
{
	public int stimCount = 0;
	public float width = 0;
	public float height = 0;
	
	IEnumerator Start() {
		width = GetComponent<RectTransform>().rect.width;
		height = GetComponent<RectTransform>().rect.height;
		
		GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
		GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
		
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime;
			t = Mathf.Clamp01(t);
			t = Mathf.Sin(t * Mathf.PI * 0.5f);

			GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0, width, t));
			GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(0, height, t));

			yield return null;
		}
	}

	public void AddStim() {
		if (stimCount < 3) {
			stimCount++;
		}

		UpdateStim();
	}
	
	public void RemoveStim() {
		if (stimCount > 0) {
			stimCount--;
		}

		UpdateStim();
	}

	IEnumerator stimbarSize(GameObject stimbar, float size) {
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime;
			stimbar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(stimbar.GetComponent<RectTransform>().rect.height, size, t));
		}
		
		if (size == 0) {
			stimbar.SetActive(false);
		}
		
		yield return null;
	}
	private void UpdateStim() {
		for (int i = 0; i < transform.childCount; i++) {
			if (i < stimCount) {
				transform.GetChild(i).gameObject.SetActive(true);
				StartCoroutine(stimbarSize(transform.GetChild(i).gameObject, 35));
			}
		}
	}
}
