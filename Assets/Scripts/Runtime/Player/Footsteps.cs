using FMODUnity;
using UnityEngine;


public class Footsteps : MonoBehaviour {
	private CharacterController controller;
	private PlayerMovement playerMovement;

	[Header("References")]
	public EventReference footstepsEvent;
	public EventReference ladderClimbEvent;

	[Header("Settings")]
	public float footstepOffset = 0.35f;
	public float ladderstepOffset = 0.35f;
	public bool isInDrill;

	private float currentFootstepoffset = 0f;
	private float currentLadderoffset = 0f;

	public void Start() {
		controller = GetComponent<CharacterController>();
		playerMovement = GetComponent<PlayerMovement>();
		currentFootstepoffset = footstepOffset;
		currentLadderoffset = ladderstepOffset;
	}

	public void Update() {
		handleFootsteps();
		handleLaddersteps();
	}

	private void handleFootsteps() {
		if (playerMovement.CurrentState != PlayerMovement.State.Walking) return;

		isInDrill = transform.GetComponentInParent<DrillController>() != null;

		if (playerMovement.isWalking) {
			currentFootstepoffset -= Time.deltaTime;
		}

		if (controller.isGrounded && currentFootstepoffset <= 0) {
			var instance = RuntimeManager.CreateInstance(footstepsEvent);
			RuntimeManager.AttachInstanceToGameObject(instance, transform);
			instance.setParameterByName("footsteps", isInDrill ? 1f : 0f);
			instance.start();
			instance.release();
		}

		if (currentFootstepoffset <= 0) currentFootstepoffset = footstepOffset;
	}
	
	private void handleLaddersteps() {
		if (playerMovement.CurrentState != PlayerMovement.State.Climbing) return;

		if (playerMovement.isClimbing) {
			currentLadderoffset -= Time.deltaTime;
		}

		if (currentLadderoffset <= 0) {
			var instance = RuntimeManager.CreateInstance(ladderClimbEvent);
			RuntimeManager.AttachInstanceToGameObject(instance, transform);
			instance.start();
			instance.release();
		}

		if (currentLadderoffset <= 0) currentLadderoffset = ladderstepOffset;
	}
}