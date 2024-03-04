using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDeath : MonoBehaviour {
	private PlayerMovement playerMovement;
	private PlayerOxygen playerOxygen;
	private PlayerSprint playerSprint;
	private Stimpack playerStimpack;
	private PlayerItems playerItems;
	private PlayerInteractions playerInteractions;
	public DeathScreen deathScreenToUse;

	private string causeOfDeath = "";

	private DeathScreen deathScreenInstance;
	private bool showingDeathScreen = false;
	private bool killedByOffset = false;

	// Start is called before the first frame update
	void Start() {
		playerMovement = GetComponent<PlayerMovement>();
		playerOxygen = GetComponent<PlayerOxygen>();
		playerSprint = GetComponent<PlayerSprint>();
		// playerStimpack = GetComponent<Stimpack>();
		playerItems = GetComponent<PlayerItems>();
		playerInteractions = GetComponent<PlayerInteractions>();
	}


	private void Update() {
		if(!killedByOffset) {
			if(transform.position.y <= -1000) Kill("glitched out of the world");
			killedByOffset = true;
		}
	}


	// Intentionally without a default value to be able to pick either from the UnityEvent system.
	public void Kill() {
		Kill("unknown.");
	}
	
	
	public void Kill(string deathCause) {
		causeOfDeath = deathCause;
		playerMovement.Disable();
		playerOxygen.enabled = false;
		playerSprint.enabled = false;
		// playerStimpack.enabled = false;
		playerItems.enabled = false;
		playerInteractions.enabled = false;
	}

	private void CreateDeathScreen() { deathScreenInstance = Instantiate(deathScreenToUse); }

	public void ShowDeathScreenDelayed(float delay) {
		Invoke("ShowDeathScreen", delay);
	}

	public void ShowDeathScreen() {
		if (showingDeathScreen) return;
		if(deathScreenInstance == null) CreateDeathScreen();
		var drill = FindObjectOfType<DrillController>();

		if (drill) {
			deathScreenInstance.travelledDistance = drill.drillDistance;
		}
		
		deathScreenInstance.causeOfDeath = causeOfDeath;
		showingDeathScreen = true;
		StartCoroutine(deathScreenInstance.ShowDeathScreen());
	}

	public void Utility_AnimatorUnscaled(Animator animator) {
		animator.updateMode = AnimatorUpdateMode.UnscaledTime;
	}
}