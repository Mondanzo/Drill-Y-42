using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


[RequireComponent(typeof(Slider))]
public class BusSlider : MonoBehaviour {
	
	public string bus;
	
	private Slider slider;
	
	// Start is called before the first frame update
	void Awake() {
		slider = GetComponent<Slider>();

		var targetBus = FMODUnity.RuntimeManager.GetBus(bus);

		if (targetBus.isValid()) {
			if (targetBus.getVolume(out var volume) == RESULT.OK) {
				Debug.Log(bus + " adjusted volume");
				slider.value = volume;
			}
		} else {
			Debug.Log(bus +  " not valid");
		}
		
		slider.onValueChanged.AddListener(VolumeValueChanged);
	}

	void VolumeValueChanged(float newVal) {
		var targetBus = FMODUnity.RuntimeManager.GetBus(bus);

		if (targetBus.isValid()) {
			targetBus.setVolume(newVal);
			AudioOptions.SetVolume(bus, newVal);
		}
	}
}