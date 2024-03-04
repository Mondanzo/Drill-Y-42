using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class OnActivateSetTargetSelected : MonoBehaviour {
	public GameObject activateOnEnable;

	// Start is called before the first frame update
	public void SetTarget() {
		EventSystem.current.SetSelectedGameObject(activateOnEnable);
	}
}