using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;


public class HUDStim : MonoBehaviour
{
    public GameObject stimPackObject;
	private Stimpack stimpack;

	public GameObject stimBarParent;
	public GameObject stimBar;

	public EventReference stimSound;

	private List<GameObject> stimBars = new List<GameObject>();
	private GameObject currentStimbar;
	
	public void Start() {
		stimpack = stimPackObject.GetComponent<Stimpack>();
	}
	
	public void addStimpackBar() {
		if (stimBars.Count == 0 && stimBarParent != null && stimBar != null) {
			GameObject newStimBar = Instantiate(stimBar, stimBarParent.transform);
			stimBars.Add(newStimBar);
		}

		foreach (GameObject stimbar in stimBars) {
			if (stimbar && stimbar.GetComponent<StimbarInternal>().stimCount < 3) {
				currentStimbar = stimbar;
				break;
			}
			
			if (stimBars.IndexOf(stimbar) == stimBars.Count - 1 && stimBarParent != null && stimBar != null) {
				GameObject newStimBar = Instantiate(stimBar, stimBarParent.transform);
				stimBars.Add(newStimBar);
				currentStimbar = newStimBar;
				break;
			}
		}

		if (currentStimbar != null) {
			currentStimbar.GetComponent<StimbarInternal>().AddStim();
		}
	}
	
	public void removeStimpackBar() {
		GameObject currentStimbar = null;
		foreach (GameObject stimbar in stimBars) {
			if (stimbar.GetComponent<StimbarInternal>().stimCount > 0) {
				currentStimbar = stimbar;
				
				if (stimbar.GetComponent<StimbarInternal>().stimCount == 1) {
					stimBars.Remove(stimbar);
					Destroy(stimbar);
				} else {
					currentStimbar.GetComponent<StimbarInternal>().RemoveStim();
				}
				
				break;
			}
		}
	}

	public void removeStimpack() {
		GameObject currentStimbar = null;
		foreach (GameObject stimbar in stimBars) {
			if (stimbar.GetComponent<StimbarInternal>().stimCount > 0) {
				currentStimbar = stimbar;
				
				stimBars.Remove(stimbar);
				Destroy(stimbar);
				
				break;
			}
		}
	}

	private IEnumerator shakeStimbarObject(GameObject stimBar) {
		for (int i = 0; i < 50; i++) {
			if (stimBar == null) {
				break;
			}
				
			stimBar.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(stimBar.transform.rotation.z, 0 + Random.Range(-3, 3), 0.5f));
			yield return new WaitForSeconds(0.01f);
			stimBar.transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(stimBar.transform.rotation.z, 0 + Random.Range(-3, 3), 0.5f));
			yield return new WaitForSeconds(0.01f);
		}
		
		stimBar.transform.rotation = Quaternion.Euler(0, 0, 0);
	}
	public void Shake() {
		RuntimeManager.PlayOneShot(stimSound);

		foreach (GameObject stimbar in stimBars) {
			StartCoroutine(shakeStimbarObject(stimbar));
			
		}
	}
}
