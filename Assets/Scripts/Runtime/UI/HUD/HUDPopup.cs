using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Popup : MonoBehaviour
{ 
	[SerializeField]
	private GameObject popup;

	[SerializeField]
	private GameObject parent;
	
	private Dictionary<string, GameObject> popupIds = new Dictionary<string, GameObject>();
	private List<IEnumerator> popupCoroutines = new List<IEnumerator>();

	public IEnumerator DeletePopup(GameObject popupInstance, float waitTime) {
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(FadeOutPopup(popupInstance));
		yield return new WaitForSeconds(1);
		Destroy(popupInstance);
	}
	
	private IEnumerator FadeInPopup(GameObject popupInstance) {
		TextMeshProUGUI popupText = popupInstance.GetComponent<TextMeshProUGUI>();
		for (float i = 0; i < 1; i += 0.05f) {
			popupText.color = new Color(1, 1, 1, i);
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	private IEnumerator FadeOutPopup(GameObject popupInstance) {
		TextMeshProUGUI popupText = popupInstance.GetComponent<TextMeshProUGUI>();
		for (float i = 1; i > -.5f; i -= 0.05f) {
			popupText.color = new Color(1, 1, 1, i);
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	float easeOutCubic(float x) {
		return 1 - Mathf.Pow(1 - x, 3);
	}
	
	private IEnumerator FadeOutTextSize(GameObject popupInstance, float duration, float size, float delay = 0) {
		TextMeshProUGUI popupText = popupInstance.GetComponent<TextMeshProUGUI>();

		float startSize = popupText.fontSize;
		float endSize = size;
		float time = 0;

		if (delay != 0) {
			yield return new WaitForSeconds(delay);
		}
		
		while (time < duration) {
			time += Time.deltaTime;
			popupText.fontSize = easeOutCubic(time / duration) * (endSize - startSize) + startSize;

			yield return null;
		}
	}
	
	public void PopupEvent(string text, float waitTime = 0, string id = "") {
		if (id != "") {
			if (popupIds.ContainsKey(id)) {
				GameObject popupInstance = parent.transform.Find(id).gameObject;
				
				EditPopup(popupInstance, text);
			} else {
				GameObject popupInstance = Instantiate(popup, parent.transform);
				TextMeshProUGUI popupText = popupInstance.GetComponent<TextMeshProUGUI>();
				popupText.color = new Color(1, 1, 1, 0);
				popupText.text = text;
				
				StartCoroutine(FadeInPopup(popupInstance));
				
				popupIds.Add(id, popupInstance);
				popupInstance.name = id;
				EditPopup(popupInstance, text);
			}
		} else {
			GameObject popupInstance = Instantiate(popup, parent.transform);
			TextMeshProUGUI popupText = popupInstance.GetComponent<TextMeshProUGUI>();
			popupText.color = new Color(1, 1, 1, 0);
			popupText.text = text;

			StartCoroutine(FadeInPopup(popupInstance));
			
			if (waitTime > 0) {
				StartCoroutine(DeletePopup(popupInstance, waitTime));
			}
		}
	}
	
	public void DeletePopupId(string id) {
		if (popupIds.ContainsKey(id)) {
			GameObject popupInstance = parent.transform.Find(id).gameObject;
			StartCoroutine(DeletePopup(popupInstance, 1));
			popupIds.Remove(id);
		}
	}
	
	public void EditPopup(GameObject popupInstance, string text) {
		TextMeshProUGUI popupText = popupInstance.GetComponent<TextMeshProUGUI>();
		
		foreach (IEnumerator coroutine in popupCoroutines.ToList()) {
			StopCoroutine(coroutine);
			popupCoroutines.Remove(coroutine);
		}
		
		IEnumerator corountine1 = FadeOutTextSize(popupInstance, 0.1f, 22, 0);
		IEnumerator corountine2 = FadeOutTextSize(popupInstance, 0.5f, 15.9f, 0.105f);
		popupCoroutines.Add(corountine1);
		popupCoroutines.Add(corountine2);
		
		StartCoroutine(corountine1);
		StartCoroutine(corountine2);
		
		popupText.text = text;
	}
}
