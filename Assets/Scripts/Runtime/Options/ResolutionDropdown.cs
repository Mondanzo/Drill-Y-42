using System;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(TMP_Dropdown))]
public class ResolutionDropdown : MonoBehaviour {

	private TMP_Dropdown dropdown;
	
	public void Awake() {
		dropdown = GetComponent<TMP_Dropdown>();
		
		foreach (var res in Screen.resolutions) {
			dropdown.options.Add(new TMP_Dropdown.OptionData(res.width  + " x " + res.height + " (" + res.refreshRateRatio.value.ToString("0.00") + "Hz)"));
		}
		
		dropdown.value = GraphicsOptions.FindIdxForResolution(Screen.currentResolution);
		
		dropdown.onValueChanged.AddListener(OnValueChanged);
	}


	public void OnDestroy() {
		dropdown.onValueChanged.RemoveListener(OnValueChanged);
	}

	public void OnValueChanged(int value) {
		GraphicsOptions.SetResolutionByIndex(value);
		GraphicsOptions.SaveOptions();
	}
}