using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PlayerMovement : MonoBehaviour, PlayerInputs.IPlayerMovementActions {

	[Header("Player Parameters")]
	public float PlayerJumpStrength = 5f;
	public float PlayerTurnRate = 8f;
	[SerializeField]
	private float gravityMultiplier = 2;

	[Header("Camera Controls")]
	public float minPitchRotation = -45f;
	public float maxPitchRotation = 45f;

	public float viewBobbingSpeed = 0.05f;
	public float viewBobbingAmount = 0.05f;
	[Header("References")]
	public GameObject hand;
	public GameObject hammerTool;

	private Vector2 inputMotion;
	private Vector2 inputRotation;

	public PlayerInputs playerInputs;
	private bool isEnabled = true;

	private Vector3 playerVelocity;

	private CharacterController controller;
	private Camera camera;
	private Animator handAnimator;

	[SerializeField]
	private float animatorDampTime = 0.1f;

	private float currentCameraRotation;

	private PlayerData playerData;

	public UnityEvent playerStoppedHammerDashing;

	private PlayerSprint playerSprint;
	private Coroutine sprintCoroutine;

	// player push variables (for hammer tool)
	private Vector3 pushVelocity;
	private float slowMultiplier = 1.0f;
	private float pushStopThreshold = 1.0f;
	private float pushMaxSpeed = 50.0f;
	private float pushDecelerationMultiplier = 1.0f;
	private float playerTurnRateMultiplier = 1.0f;
	private bool pushPlayerActive = false;

	[SerializeField]
	private float climbingSpeed = 5;

	[FormerlySerializedAs("isMoving")] public bool isWalking;
	public bool isClimbing;

	private State state;

	public State CurrentState {
		get => state;
		set {
			state = value;
			playerVelocity = Vector3.zero; // reset velocity if state changes
		}
	}

	public enum State{
		Walking,
		Climbing
	}

	// Start is called before the first frame update
	private void Start() {
		if (playerInputs == null) {
			playerInputs = new PlayerInputs();

			playerInputs.PlayerMovement.AddCallbacks(this);
		}

		playerInputs.Enable();

		controller = GetComponent<CharacterController>();
		camera = GetComponentInChildren<Camera>();
		currentCameraRotation = camera.transform.eulerAngles.x;
		handAnimator = hand.GetComponent<Animator>();

		Cursor.lockState = CursorLockMode.Locked;

		playerData = GetComponent<PlayerData>();

		playerSprint = GetComponent<PlayerSprint>();
	}

	public PlayerInputs getPlayerInputs() {
		return playerInputs;
	}

	private void OnDisable() {
		playerInputs.Disable();
	}

	public void Disable() {
		isEnabled = false;
	}

	public void Enable() {
		isEnabled = true;
	}

	// Update is called once per frame
	private void Update() {
		if (PauseScreen.isPaused) return;
		switch (CurrentState) {
			case State.Walking:
				HandleRotation();
				HandleMovement();	
				break;
			case State.Climbing:
				HandleRotation();
				HandleClimbing();
				break;
		}
	}

	private void HandleRotation() {
		if (!isEnabled) {
			return;
		}

		transform.Rotate(Vector3.up, inputRotation.x * PlayerTurnRate * playerTurnRateMultiplier * Time.deltaTime);

		currentCameraRotation += inputRotation.y * PlayerTurnRate * playerTurnRateMultiplier * Time.deltaTime;

		currentCameraRotation = Mathf.Clamp(currentCameraRotation, minPitchRotation, maxPitchRotation);

		float delta = Mouse.current.delta.ReadValue().x;

		var cameraEulerAngles = camera.transform.eulerAngles;
		camera.transform.eulerAngles =
			new Vector3(currentCameraRotation, cameraEulerAngles.y, Mathf.Lerp(0, delta / 3, 0.025f));

		hand.transform.eulerAngles = new Vector3(0, cameraEulerAngles.y - 90f, (-cameraEulerAngles.x));
		hand.transform.localPosition = new Vector3(0.182f, 0.467f, 0);

		hammerTool.transform.eulerAngles = new Vector3(cameraEulerAngles.x, cameraEulerAngles.y, 0);
	}

	private void HandleMovement() {
		if (!isEnabled) {
			isWalking = false;
			return;
		}

		var motion = new Vector3(inputMotion.x, 0, inputMotion.y);

		handAnimator.SetFloat("FirstPersonMoveSpeed", motion.magnitude * playerSprint.GetAnimationMoveSpeedMultiplier(), animatorDampTime, Time.deltaTime); // the float goes from 0-1, 0 -> no movement, 0.5 -> normal running, 1 -> sprint
		if (handAnimator.GetFloat("FirstPersonMoveSpeed") < 0.01) {
			handAnimator.SetFloat("FirstPersonMoveSpeed", 0);
		}

		var adjustedMotion = transform.rotation * motion;

		adjustedMotion *= playerData.PlayerSpeed * playerSprint.GetResSprintSpeedMultiplier(playerVelocity) * slowMultiplier;

		playerVelocity.x = adjustedMotion.x;
		playerVelocity.z = adjustedMotion.z;

		pushPlayer();

		// Jump Logic
		if (!controller.isGrounded) {
			playerVelocity += gravityMultiplier * Time.deltaTime * Physics.gravity;
		}

		isWalking = Vector3.Scale(playerVelocity, new Vector3(1, 0, 1)).magnitude > 0.1f && controller.isGrounded;
		controller.Move(playerVelocity * Time.deltaTime);
	}

	private void HandleClimbing() {
		if (!isEnabled) {
			return;
		}

		// this moves the player as if they are flying
		var adjustedMotion = new Vector3();
		adjustedMotion += camera.transform.forward * inputMotion.y;
		adjustedMotion += camera.transform.right * inputMotion.x;
		adjustedMotion = Vector3.ClampMagnitude(adjustedMotion, 1);
		adjustedMotion *= climbingSpeed;

		var forwardMovementFactor = Vector3.Dot(transform.forward, adjustedMotion.normalized);
		if (forwardMovementFactor > 0) {
			// at certain up/down angle move player up or downwards with full climbing speed
			if (Mathf.Abs(adjustedMotion.y) > .3f) {
				adjustedMotion.y = Mathf.Sign(adjustedMotion.y) * climbingSpeed;
			}
		} else { // in case player moves backward don't apply vertical movement
			adjustedMotion.y = 0;
		}
		playerVelocity = adjustedMotion;

		isClimbing = Mathf.Abs(playerVelocity.y) > 0.1f && !controller.isGrounded;
		controller.Move(playerVelocity * Time.deltaTime);
	}

	public void Kill() {
		GetComponent<PlayerDeath>().Kill();
	}

	private void pushPlayer() {
		if (!pushPlayerActive) return;

		// stop pushing if player collided with wall
		if ((controller.collisionFlags & CollisionFlags.Sides) != 0) { // touching sides
			pushVelocity = Vector3.zero;
			playerVelocity.y = 0f;
		}

		if (pushVelocity.magnitude > pushStopThreshold) {
			playerVelocity += Vector3.ClampMagnitude(pushVelocity, pushMaxSpeed);

			// deccelerate push velocity
			pushVelocity = Vector3.Lerp(pushVelocity, Vector3.zero, pushDecelerationMultiplier * Time.deltaTime);

			// only apply upwards force once.. otherwise player will fly into the sky and never return
			if (pushVelocity.y > 0) pushVelocity.y = 0;

			playerInputs.PlayerMovement.Move.Disable();
		} else {
			pushVelocity = Vector3.zero;
			playerInputs.PlayerMovement.Move.Enable();
			playerTurnRateMultiplier = 1.0f;
			playerStoppedHammerDashing.Invoke();
			pushPlayerActive = false;
		}
	}

	public bool ReturnIsEnabled() {
		return isEnabled;
	}

	public void PushPlayer(Vector3 pushVelocity, float pushStopThreshold, float pushMaxSpeed, float pushDecelerationMultiplier) {
		pushPlayerActive = true;
		this.pushVelocity = pushVelocity;
		this.pushStopThreshold = pushStopThreshold;
		this.pushMaxSpeed = pushMaxSpeed;
		this.pushDecelerationMultiplier = pushDecelerationMultiplier;
	}

	public void SetSlowMultiplier(float multiplier) {
		if (PauseScreen.isPaused) return;
		slowMultiplier = multiplier;
	}

	public void SetPlayerTurnRateMultiplier(float mult) {
		playerTurnRateMultiplier = mult;
	}

	public void OnMove(InputAction.CallbackContext context) {
		inputMotion = context.ReadValue<Vector2>();
		if (inputMotion.y <= 0) {
			playerSprint.StopSprinting();
		}
	}

	public void OnRotate(InputAction.CallbackContext context) {
		inputRotation = context.ReadValue<Vector2>();
	}

	public void OnJump(InputAction.CallbackContext context) {
		if (PauseScreen.isPaused) return;
 		if (controller.isGrounded) {
			playerVelocity.y = PlayerJumpStrength;
		}
	}

	public void OnSprint(InputAction.CallbackContext context) {
		if (context.started) {
			sprintCoroutine = StartCoroutine(startSprint());
		} else if (context.canceled) {
			if (sprintCoroutine != null) StopCoroutine(sprintCoroutine);
			playerSprint.StopSprinting();
		}
	}

	// added short delay before checking if sprinting is possible so pressing shift and forward at the same time also makes the player sprint
	private IEnumerator startSprint() {
		yield return new WaitForSeconds(0.05f);
		if (inputMotion.y > 0) {
			playerSprint.StartSprinting();
		}
	}
}