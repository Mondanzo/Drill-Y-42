using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(TMP_Dropdown))]
public class QualityDropdown : MonoBehaviour {

	private TMP_Dropdown dropdown;
	
	public void Awake() {
		dropdown = GetComponent<TMP_Dropdown>();

		foreach (var quality in QualitySettings.names) {
			dropdown.options.Add(new TMP_Dropdown.OptionData(quality));
		}

		dropdown.value = QualitySettings.GetQualityLevel();
		
		dropdown.onValueChanged.AddListener(OnValueChanged);
	}


	public void OnDestroy() {
		dropdown.onValueChanged.RemoveListener(OnValueChanged);
	}

	public void OnValueChanged(int value) {
		QualitySettings.SetQualityLevel(value);
		GraphicsOptions.SaveOptions();
	}
}