using System;
using System.Collections;
using UnityEngine;

public class SporePlant : MonoBehaviour {

	private Animator animator;
	private Transform target;

	[Header("References")]
	[SerializeField]
	private GameObject sporeCloudPrefab;
	[SerializeField]
	private Transform cloudSpawnPoint;

	[Header("Settings")]
	[SerializeField]
	private float cooldown = 20f;
	private bool isOnCooldown = false;

	private bool isInRange = false;

	private Vector3 originalPosition;
	[SerializeField]
	[Range(0f, 0.5f)]
	private float shakeStrength = 0.1f;
	private float currentShakeStrength;
	[SerializeField]
	private float timeUntilExplosion = 4f;

	private void Start() {
		animator = GetComponent<Animator>();
		originalPosition = transform.position;
	}

	private void Update() {
		if (isOnCooldown) return;

		if (isInRange) {
			shake();
		} else {
			unShake();
		}
	}

	// triggered via animation event
	private void onPlantOpened() {
		var cloudInstance = Instantiate(sporeCloudPrefab, cloudSpawnPoint);
		cloudInstance.transform.parent = null;
		StartCoroutine(startCooldown());
	}

	// triggered via animation event
	private void onPlantClosed() {
		if (target && !isOnCooldown) {
			animator.SetBool("isOpen", true);
		}
	}

	private IEnumerator startCooldown() {
		isOnCooldown = true;
		yield return new WaitForSeconds(cooldown);
		isOnCooldown = false;
		animator.SetBool("isOpen", false);
	}

	private void shake() {
		currentShakeStrength += (shakeStrength / timeUntilExplosion) * Time.deltaTime;
		if (currentShakeStrength < shakeStrength) {
			transform.position = Vector3.Lerp(transform.position, originalPosition + UnityEngine.Random.insideUnitSphere * currentShakeStrength, 0.5f);
		} else {
			currentShakeStrength = 0;
			transform.position = originalPosition;
			animator.SetBool("isOpen", true);
		}
	}

	private void unShake() {
		currentShakeStrength -= (shakeStrength / timeUntilExplosion) * 1.5f * Time.deltaTime;
		if (currentShakeStrength > 0) {
			transform.position = Vector3.Lerp(transform.position, originalPosition + UnityEngine.Random.insideUnitSphere * currentShakeStrength, 0.5f);
		} else {
			currentShakeStrength = 0;
			transform.position = originalPosition;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			target = other.transform;
			isInRange = true;

			if (isOnCooldown) return;

			//animator.SetBool("isOpen", true);
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			target = null;
			isInRange = false;

			if (isOnCooldown) return;

			//animator.SetBool("isOpen", false);
		}
	}
}
