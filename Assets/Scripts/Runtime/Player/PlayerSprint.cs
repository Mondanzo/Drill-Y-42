using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerSprint : MonoBehaviour {
	[Header("References")]
	[SerializeField]
	private StudioEventEmitter sprintAudioSource;

	[Header("Settings")]
	[SerializeField]
	private float sprintSpeedMultiplier = 1.5f;
	[SerializeField]
	private float defaultFOV = 60;
	[SerializeField]
	private float sprintFOV = 70;
	[SerializeField]
	private float oxygenUsage = 10;

	private bool isSprinting = false;
	private float resSprintMultiplier = 1f;
	private float animationMoveSpeedMultiplier = 0.5f;

	private PlayerOxygen playerOxygen;

	private void Start() {
		playerOxygen = GetComponent<PlayerOxygen>();
		playerOxygen.onOxygenZero.AddListener(onOxygenZero);
	}

	private void Update() {
		if (isSprinting) {
			playerOxygen.LoseOxygen(oxygenUsage * Time.deltaTime);
		}
	}

	private void onOxygenZero() {
		StopSprinting();
	}

	public void StartSprinting() {
		StopAllCoroutines();
		StartCoroutine(animateFov(Camera.main.fieldOfView, sprintFOV, 0.25f));

		isSprinting = true;
		animationMoveSpeedMultiplier = 1f;
		if (sprintAudioSource) sprintAudioSource.Play();
	}

	public void StopSprinting() {
		StopAllCoroutines();
		StartCoroutine(animateFov(Camera.main.fieldOfView, defaultFOV, 0.25f));

		isSprinting = false;
		animationMoveSpeedMultiplier = 0.5f;
		if (sprintAudioSource) sprintAudioSource.Stop();
	}

	public float GetResSprintSpeedMultiplier(Vector3 playerVelocity) {
		if (isSprinting) {
			var forwardMovementFactor = Mathf.Clamp01(Vector3.Dot(transform.forward, playerVelocity.normalized));
			resSprintMultiplier = Mathf.Lerp(1f, sprintSpeedMultiplier, forwardMovementFactor);
		} else {
			resSprintMultiplier = 1f;
		}

		return resSprintMultiplier;
	}

	public float GetAnimationMoveSpeedMultiplier() {
		return animationMoveSpeedMultiplier;
	}

	private IEnumerator<float> animateFov(float number, float endNumber, float duration) {
		float t = 0f;
		float fin = 0f;
		while (t < duration) {
			t += Time.deltaTime;
			float currentNumber = easeOut(number, endNumber, t / duration);

			Camera.main.fieldOfView = currentNumber;

			fin = currentNumber;
			yield return fin;
		}
		yield return fin;
	}

	private float easeOut(float x, float y, float t) {
		float diff = y - x;
		float pow = Mathf.Pow(t - 1, 5) + 1;
		return diff * pow + x;
	}
}
