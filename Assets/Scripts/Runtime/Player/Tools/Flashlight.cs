using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour {
	private PlayerInputs playerInputs;

	private StudioEventEmitter studioEventEmitter;
	private Light lightsource;

	private void Awake() {
		playerInputs = new PlayerInputs();
		playerInputs.Interactions.Flashlight.performed += onPerformed;
	}

	private void Start() {
		lightsource = GetComponent<Light>();
		studioEventEmitter = GetComponent<StudioEventEmitter>();
	}

	private void onPerformed(InputAction.CallbackContext context) {
		if (lightsource.enabled) {
			flashlightOff();
		} else {
			flashlightOn();
		}
	}

	private void OnEnable() {
		playerInputs.Interactions.Flashlight.Enable();
	}

	private void OnDisable() {
		playerInputs.Interactions.Flashlight.Disable();
	}


	private void flashlightOn() {
		if (!lightsource.enabled) {
			lightsource.enabled = true;
			studioEventEmitter.Play();
		}
	}

	private void flashlightOff() {
		if (lightsource.enabled) {
			lightsource.enabled = false;
			studioEventEmitter.Stop();
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Drill") && other.GetComponent<KeepPlayerOnMovingObject>()) {
			flashlightOff();
		}
	}
}
