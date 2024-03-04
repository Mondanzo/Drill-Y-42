using System;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TMP_Dropdown))]
public class FullscreenDropdown : MonoBehaviour {

	private TMP_Dropdown dropdown;
	
	public void Awake() {
		dropdown = GetComponent<TMP_Dropdown>();
		
		dropdown.value = GraphicsOptions.GetIndexByFullscreenMode(Screen.fullScreenMode);
		
		dropdown.onValueChanged.AddListener(OnValueChanged);

	}


	public void OnDestroy() {
		dropdown.onValueChanged.RemoveListener(OnValueChanged);
	}

	public void OnValueChanged(int value) {
		var mode = GraphicsOptions.GetFullscreenModeByIndex(value);

		Screen.fullScreenMode = mode;
		GraphicsOptions.SaveOptions();
	}
}