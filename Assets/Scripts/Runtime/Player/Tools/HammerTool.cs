using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class HammerTool : Tool {
	private PlayerInputs playerInputs;

	private PlayerMovement playerMovement;

	[Header("References")]
	[SerializeField]
	private Animator handAnimator;
	[SerializeField]
	private Animator hammerAnimator;
	[SerializeField]
	private GameObject hammerObject;
	[SerializeField]
	private AnimationClip chargeAnimationClip;
	[SerializeField]
	private AnimationClip chargeHandAnimationClip;
	[SerializeField]
	private Renderer hammerMaterial;
	[SerializeField]
	private VisualEffect hitEffect;
	[SerializeField]
	private VisualEffect chargeEffect;
	[SerializeField]
	private EventReference charge;
	[SerializeField]
	private EventReference cooldown;
	[SerializeField]
	private EventReference hit;

	private Collider hammerCollider;

	[Header("Settings")]
	[SerializeField]
	private float stopThreshold = 1f;
	[SerializeField]
	private float maxSpeed = 50f;
	[SerializeField]
	private float decelerationMultiplier = 1f;
	[SerializeField]
	private bool allowVerticalPush = true;
	[SerializeField]
	private float slowTurnRateMultiplier = 0.1f;
	[SerializeField]
	private float maxEmissionStrength = 1f;
	[SerializeField]
	private float equippedSlowDown = 0.9f;

	[Header("Charge")]
	[SerializeField]
	private float chargeThreshold = 1f;
	[SerializeField]
	private float chargeForce = 40f;
	[SerializeField]
	private float chargeSlowDown = 0.5f;

	[Header("Overcharge")]
	[SerializeField]
	private float overChargeThreshold = 3f;
	[SerializeField]
	private float overChargeReleaseThreshold = 5f;
	[SerializeField]
	private float overChargeForce = 100f;
	[SerializeField]
	private float overChargeSlowDown = 0.1f;
	[SerializeField]
	private float overChargeCooldown = 15f;

	private float timer = 0f;
	private bool isOnCooldown = false;
	private bool isCharging = false;
	
	private EventInstance chargeInstance;
	private EventInstance cooldownInstance;
	private EventInstance hitInstance;

	private void Awake() {
		playerInputs = new PlayerInputs();
		playerInputs.Interactions.Primary.performed += started;
		playerInputs.Interactions.Primary.canceled += canceled;

		playerMovement = GetComponentInParent<PlayerMovement>();
		hammerCollider = GetComponent<Collider>();
		hammerCollider.enabled = false;
	}

	private void Start() {
		chargeInstance = RuntimeManager.CreateInstance(charge);
		cooldownInstance = RuntimeManager.CreateInstance(cooldown);
		hitInstance = RuntimeManager.CreateInstance(hit);
	}

	private void started(InputAction.CallbackContext context) {
		if (isOnCooldown) return;

		isCharging = true;
	}

	private void canceled(InputAction.CallbackContext context) {
		if (isOnCooldown) return;

		if (timer < chargeThreshold) { // do normal punch
			hammerMaterial.material.SetFloat("_EmissionStrength", 0);
			StartCoroutine(disablePunchCollider()); // disable punch collider after short time for normal puch; otherwise it gets disabled by the player stopping his charge movement (playermovent.cs)
		} else if (timer < overChargeThreshold) { // charge puunch
			playerMovement.PushPlayer(getPushVelocity(chargeForce), stopThreshold, maxSpeed, decelerationMultiplier);
			playerMovement.SetPlayerTurnRateMultiplier(slowTurnRateMultiplier);
			hammerMaterial.material.SetFloat("_EmissionStrength", 0);
		} else if (timer < overChargeReleaseThreshold) { // do overcharge punch
			playerMovement.PushPlayer(getPushVelocity(overChargeForce), stopThreshold, maxSpeed, decelerationMultiplier);
			playerMovement.SetPlayerTurnRateMultiplier(slowTurnRateMultiplier);
			isOnCooldown = true;
		}
		punch();
	}

	private void Update() {
		chargeInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
		cooldownInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
		hitInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));

		adjustChargeAnimationDuration();
		if (isOnCooldown) {
			timer += Time.deltaTime;
			hammerMaterial.material.SetFloat("_EmissionStrength", maxEmissionStrength - (timer * (maxEmissionStrength / overChargeCooldown)));
			cooldownInstance.getPlaybackState(out var playbackstate);
			if (playbackstate == PLAYBACK_STATE.STOPPED) cooldownInstance.start();

			if (timer >= overChargeCooldown) {
				timer = 0;
				isOnCooldown = false;
				hammerMaterial.material.SetFloat("_EmissionStrength", 0);
				cooldownInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			}
			return;
		}

		if (isCharging) {
			timer += Time.deltaTime;
			hammerMaterial.material.SetFloat("_EmissionStrength", timer * (maxEmissionStrength / overChargeReleaseThreshold));
			chargeEffect.SetFloat("SparkesRate", timer * (1 / overChargeReleaseThreshold));
			chargeInstance.setParameterByName("PitchCharge", timer * (1 / overChargeReleaseThreshold));

			if (timer >= overChargeReleaseThreshold) { // force release punch
				playerMovement.PushPlayer(getPushVelocity(overChargeForce), stopThreshold, maxSpeed, decelerationMultiplier);
				playerMovement.SetPlayerTurnRateMultiplier(slowTurnRateMultiplier);
				isOnCooldown = true;
				punch();
			} else if (timer > overChargeThreshold) {
				handAnimator.SetBool("HammerOvercharge", true);
				hammerAnimator.SetBool("Overcharge", true);
				playerMovement.SetSlowMultiplier(overChargeSlowDown);
			} else if (timer > chargeThreshold) {
				chargeInstance.getPlaybackState(out var playbackstate);
				if (playbackstate == PLAYBACK_STATE.STOPPED) chargeInstance.start();

				handAnimator.SetBool("HammerCharge", true);
				hammerAnimator.SetBool("Charge", true);
				playerMovement.SetSlowMultiplier(chargeSlowDown);

				chargeEffect.SetBool("IsOn", true);
			}
		}
	}

	private void adjustChargeAnimationDuration() {
		var chargeDuration = overChargeThreshold - chargeThreshold;

		// calculate speed multiplier to change charge animation to have the same length as charge duration
		var speedMultiplier = chargeDuration / chargeAnimationClip.length;
		// invert speed multiplier so it is greater than 1 and therefore increases playback speed and therefore lowers duration it takes to play
		var resMultiplier = 1 / speedMultiplier;
		hammerAnimator.SetFloat("ChargeSpeedMultiplier", resMultiplier);

		speedMultiplier = chargeDuration / chargeHandAnimationClip.length;
		resMultiplier = 1 / speedMultiplier;
		handAnimator.SetFloat("ChargeSpeedMultiplier", resMultiplier);
	}

	private void punch() {
		hammerCollider.enabled = true;

		chargeInstance.setParameterByName("PitchCharge", 0);
		chargeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		hitInstance.start();

		isCharging = false;
		timer = 0f;
		playerMovement.SetSlowMultiplier(equippedSlowDown);
		handAnimator.SetBool("HammerCharge", false);
		handAnimator.SetBool("HammerOvercharge", false);
		handAnimator.SetTrigger("HammerPunch");

		hammerAnimator.SetBool("Charge", false);
		hammerAnimator.SetBool("Overcharge", false);
		hammerAnimator.SetTrigger("Punch");

		//hitEffect.Play();
		chargeEffect.SetBool("IsOn", false);
		chargeEffect.SetFloat("SparkesRate", 0);
	}

	private IEnumerator disablePunchCollider() {
		yield return new WaitUntil(() => hammerCollider.enabled);

		yield return new WaitForSeconds(0.1f);
		hammerCollider.enabled = false;
	}

	private Vector3 getPushVelocity(float magnitude) {
		if (allowVerticalPush) {
			return Camera.main.transform.forward * magnitude;
		} else {
			var val = Camera.main.transform.forward * magnitude;
			val.y = 0;
			return val;
		}
	}

	private void OnEnable() {
		hammerObject.SetActive(true);

		playerMovement.SetSlowMultiplier(equippedSlowDown);
		playerInputs.Interactions.Primary.Enable();
	}

	private void OnDisable() {
		hammerObject.SetActive(false);

		playerMovement.SetSlowMultiplier(1.0f);
		playerInputs.Interactions.Primary.Disable();
	}

	private void OnTriggerEnter(Collider other) {
		var ore = other.gameObject.GetComponentInParent<Ores>();
		if (ore) {
			ore.InstantMine();
		}
	}
}
