using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class MiningBeam : Tool {
	private PlayerInputs playerInputs;

	[Header("References")]
	[SerializeField]
	private GameObject playerObject;
	[SerializeField]
	private Animator handAnimator;
	[SerializeField]
	private GameObject transmutationCircleVfx;
	[SerializeField]
	private VisualEffect miningVfx;
	[SerializeField]
	private LayerMask layermask;

	private StudioEventEmitter miningSoundEvent;

	[Header("Settings")]
	[SerializeField]
	private float raycastDistance = 4f;
	[Range(0f, 4)]
	[SerializeField]
	private float beamFadeInOutDuration = 0.5f;

	private float miningVfxScaleMultiplier;
	private bool isMining = false;
	private InputAction.CallbackContext miningCallbackContext;

	private Coroutine fadeInOutCoroutine;

	private PlayerMovement playerMovement;

	private void Awake() {
		playerInputs = new PlayerInputs();
		playerInputs.Interactions.Primary.performed += startMining;
		playerInputs.Interactions.Primary.canceled += stopMining;
		
		playerMovement = GetComponentInParent<PlayerMovement>();
	}

	private void Start() {
		miningVfxScaleMultiplier = miningVfx.GetFloat("Length");
		miningSoundEvent = GetComponent<StudioEventEmitter>();
	}

	private void Update() {
		RaycastHit miningBeamHit;
		var isHit = rayCastMiningBeam(out miningBeamHit);

		if (isMining && isHit) {
			if (miningBeamHit.collider.gameObject.TryGetComponent<Interaction>(out var interaction)) {
				interaction.executeInput(miningCallbackContext, playerObject);
			}
		}
	}

	private void startMining(InputAction.CallbackContext context) {
		miningCallbackContext = context;
		isMining = true;
		transmutationCircleVfx.gameObject.SetActive(true);
		miningVfx.Play();

		beamFadeInOut(1, 0, beamFadeInOutDuration);

		handAnimator.SetBool("FirstPersonDrillBool", true);
		miningSoundEvent.Play();
	}

	private void stopMining(InputAction.CallbackContext context) {
		miningCallbackContext = context;
		isMining = false;
		transmutationCircleVfx.gameObject.SetActive(false);
		miningVfx.Stop();

		beamFadeInOut(0, 1, beamFadeInOutDuration);

		handAnimator.SetBool("FirstPersonDrillBool", false);
		miningSoundEvent.Stop();
	}

	private void beamFadeInOut(float from, float to, float duration) {
		if (fadeInOutCoroutine != null) StopCoroutine(fadeInOutCoroutine);
		fadeInOutCoroutine = StartCoroutine(lerpAlphaOverTime(from, to, duration));
	}

	private IEnumerator lerpAlphaOverTime(float from, float to, float duration) {
		float time = 0;
		while (time < duration) {
			time += Time.deltaTime;
			var retVal = Mathf.Lerp(from, to, time / duration);
			miningVfx.SetFloat("AlphaClip", retVal);

			yield return null;
		}
	}

	private Vector3 getCenterRaycastEndpoint(float length) {
		return Camera.main.transform.position + Camera.main.transform.forward * length;
	}

	private bool rayCastMiningBeam(out RaycastHit centerRayHit) {
		// find out & look at the endpoint of the main raycast

		float distance;
		Vector3 mainRayCastEndPoint;

		var isCenterRayHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out centerRayHit, raycastDistance, layermask);

		if (isCenterRayHit) {
			miningVfx.SetBool("IsSparkOn", true);
			miningSoundEvent.EventInstance.setParameterByNameWithLabel("Collision", "Collide");
			if (centerRayHit.distance < 1.7f) {
				mainRayCastEndPoint = getCenterRaycastEndpoint(1.7f);
				distance = Vector3.Distance(miningVfx.transform.position, mainRayCastEndPoint);
			} else {
				mainRayCastEndPoint = centerRayHit.point;
				distance = Vector3.Distance(miningVfx.transform.position, mainRayCastEndPoint);
			}
		} else {
			miningVfx.SetBool("IsSparkOn", false);
			miningSoundEvent.EventInstance.setParameterByNameWithLabel("Collision", "No Collision");
			mainRayCastEndPoint = getCenterRaycastEndpoint(raycastDistance);
			distance = Vector3.Distance(miningVfx.transform.position, mainRayCastEndPoint);
		}
		miningVfx.transform.LookAt(mainRayCastEndPoint);

		var maxDistance = Vector3.Distance(miningVfx.transform.position, getCenterRaycastEndpoint(raycastDistance));
		miningVfx.SetFloat("Length", (miningVfxScaleMultiplier / maxDistance) * distance);

		Debug.DrawRay(miningVfx.transform.position, miningVfx.transform.forward * maxDistance, Color.cyan);
		Debug.DrawRay(miningVfx.transform.position, miningVfx.transform.forward * distance, Color.magenta);
		return isCenterRayHit;
	}
	
	public void Disable() {
		playerInputs.Interactions.Primary.Disable();
	}

	public void Enable() {
		playerInputs.Interactions.Primary.Enable();
	}

	private void OnEnable() {
		//handAnimator.SetBool("FirstPersonWalk", true);
		playerInputs.Interactions.Primary.Enable();
	}

	private void OnDisable() {
		//handAnimator.SetBool("FirstPersonWalk", false);
		playerInputs.Interactions.Primary.Disable();
	}
}
