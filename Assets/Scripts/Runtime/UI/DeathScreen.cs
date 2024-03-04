using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DeathScreen : MonoBehaviour {

	[Header("References")]
	public TMP_Text causeOfDeathText;
	public TMP_Text distanceTravelledText;
	public Image image;
	
	public string causeOfDeath;
	public float travelledDistance;


	public void Awake() {
		gameObject.SetActive(false);
		image.material.color = Color.clear;
	}
	

	public IEnumerator ShowDeathScreen() {
		Time.timeScale = 0;
		PauseScreen.isPaused = true;
		
		gameObject.SetActive(true);
		Cursor.lockState = CursorLockMode.Confined;

		for (float i = 0; i <= 1; i += 0.1f) {
			image.material.color = Color.Lerp(Color.clear, Color.white, i);
			yield return null;
		}

		image.material.color = Color.white;
		
		yield return new WaitForSecondsRealtime(0.2f);
		yield return PrintText(causeOfDeathText, "Cause of death: " + causeOfDeath);
		yield return new WaitForSecondsRealtime(0.4f);
		yield return PrintText(distanceTravelledText, "Distance travelled: " + travelledDistance.ToString("N0") + "m");
	}
	
	
	public IEnumerator PrintText(TMP_Text textContainer, string textToPrint) {
		textContainer.text = "";
		
		foreach (var c in textToPrint) {
			textContainer.text += c;
			switch (c) {
				case ':':
					yield return new WaitForSecondsRealtime(1f);
					break;

				default:
					yield return new WaitForSecondsRealtime(0.1f);
					break;
			}
		}
	}
}