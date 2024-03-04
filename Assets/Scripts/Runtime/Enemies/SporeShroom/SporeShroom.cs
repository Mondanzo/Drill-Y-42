using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SporeShroom : MonoBehaviour {

	private Animator animator;
	private Transform target;

	[Header("References")]
	[SerializeField]
	private GameObject sporeCloudPrefab;
	[SerializeField]
	private Transform cloudSpawnPoint;
	[SerializeField]
	private VisualEffect sporeBreathingVfx;

	[Header("Settings")]
	[SerializeField]
	private float cooldown = 20f;
	private bool isOnCooldown = false;

	private bool isInRange = false;

	private Vector3 originalPosition;

	[SerializeField]
	private float spawnCloudDelay = 1.5f;

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

	private async void shake() {
		currentShakeStrength += (shakeStrength / timeUntilExplosion) * Time.deltaTime;
		if (currentShakeStrength < shakeStrength) {
			transform.position = Vector3.Lerp(transform.position, originalPosition + UnityEngine.Random.insideUnitSphere * currentShakeStrength, 0.5f);
		} else {
			currentShakeStrength = 0;
			transform.position = originalPosition;
			StartCoroutine(spawnCloud());
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

	private IEnumerator spawnCloud() {
		sporeBreathingVfx.Stop();
		animator.SetTrigger("SporeShroomAttack");

		yield return new WaitForSeconds(spawnCloudDelay);
		var cloudInstance = Instantiate(sporeCloudPrefab, cloudSpawnPoint);
		cloudInstance.transform.parent = null;

		isOnCooldown = true;
		yield return new WaitForSeconds(cooldown);
		isOnCooldown = false;
		sporeBreathingVfx.Play();
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			target = other.transform;
			isInRange = true;

			if (isOnCooldown) return;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			target = null;
			isInRange = false;

			if (isOnCooldown) return;
		}
	}
}
