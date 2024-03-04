using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling.LowLevel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour {
	private PlayerInputs playerInputs;

	[Header("References")]
	[SerializeField]
	private Camera cameraObject;
	[SerializeField]
	private Material highlightMaterial;
	[SerializeField]
	private Animator handAnimator;
	[SerializeField]
	private GameObject interactionPrompt;
	[SerializeField]
	private GameObject miningPrompt;

	[Header("Settings")]
	[SerializeField]
	private float raycastDistance = 4f;

	private List<GameObject> selectedObject = new List<GameObject>();

	private Interaction lastInteraction;

	private void Awake() {
		playerInputs = new PlayerInputs();
		playerInputs.Interactions.Interaction.performed += onInteraction;
		playerInputs.Interactions.Interaction.started += onInteraction;
		playerInputs.Interactions.Interaction.canceled += onInteractionCanceled;
	}

	void Update() {
		RaycastHit hit;
		Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, raycastDistance);
		Debug.DrawRay(cameraObject.transform.position, cameraObject.transform.forward * raycastDistance);

		displayHighlight(hit);
		displayInteractionPrompts(hit);
	}

	private void onInteraction(InputAction.CallbackContext context) {
		RaycastHit hit;
		var isHit = Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, raycastDistance);

		if (isHit) {
			if (hit.collider.gameObject.TryGetComponent<Interaction>(out var interaction)) {
				interaction.executeInput(context, gameObject);
				lastInteraction = interaction;
				// TODO: move animation onto object that gets interacted with?
				if (hit.collider.gameObject.CompareTag("Items")) {
					handAnimator.SetTrigger("FirstPersonGrab");
				}
			}
		}
	}

	// Fixes the problem of interactions never canceling if player stops aiming at the same obj. TODO: rework interaction system to work more elegantly
	private void onInteractionCanceled(InputAction.CallbackContext context) {
		if (lastInteraction) {
			lastInteraction.executeInput(context, gameObject);
			lastInteraction = null;
		}
	}

	private void displayHighlight(RaycastHit hit) {
		if (hit.collider && !hit.collider == selectedObject.Find(obj => obj == hit.collider.gameObject)) {
			Interaction interaction = hit.collider.gameObject.GetComponent<Interaction>();
			if ((interaction)) {
				if (interaction.toHighlight.Count > 0 && !selectedObject.Contains(hit.collider.gameObject)) {
					foreach (GameObject obj in interaction.toHighlight) {
						if (obj) {
							if (!selectedObject.Contains(obj)) {
								Renderer mesh = obj.GetComponent<Renderer>();

								if (mesh) {
									Material[] matArray = new Material[2];
									matArray[0] = mesh.materials[0];
									matArray[1] = highlightMaterial;

									selectedObject.Add(obj);

									mesh.materials = matArray;
								}
							}
						}
					}
				} else {
					if (!selectedObject.Contains(hit.collider.gameObject)) {
						selectedObject.Add(hit.collider.gameObject);

						Renderer mesh = hit.collider.gameObject.GetComponent<Renderer>();

						Material[] matArray = new Material[2];
						matArray[0] = mesh.materials[0];
						matArray[1] = highlightMaterial;

						mesh.materials = matArray;
					}
				}
			}
		}

		foreach (GameObject obj in selectedObject.ToList()) {
			if (!obj) {
				selectedObject.Remove(obj);
				continue;
			};

			if (hit.collider) {
				Interaction interaction = hit.collider.gameObject.GetComponent<Interaction>();

				if (interaction && interaction.toHighlight.Count > 0 && interaction.toHighlight.Contains(obj)) {
					if (!hit.collider.gameObject.GetComponent<Interaction>().toHighlight.Contains(obj)) {
						Renderer mesh = obj.GetComponent<Renderer>();

						if (mesh) {
							Material[] matArray = new Material[1];
							matArray[0] = mesh.materials[0];

							mesh.materials = matArray;
						}

						selectedObject.Remove(obj);
					}
				} else {
					if (obj != hit.collider.gameObject) {
						Renderer mesh = obj.GetComponent<Renderer>();

						if (mesh) {
							Material[] matArray = new Material[1];
							matArray[0] = mesh.materials[0];

							mesh.materials = matArray;
						}

						selectedObject.Remove(obj);
					}
				}
			}
		}

		if (hit.collider == null) {
			foreach (GameObject obj in selectedObject.ToList()) {
				if (!obj) {
					selectedObject.Remove(obj);
					continue;
				};

				Renderer mesh = obj.GetComponent<Renderer>();

				if (mesh) {
					Material[] matArray = new Material[1];
					matArray[0] = mesh.materials[0];

					mesh.materials = matArray;
				}

				selectedObject.Remove(obj);
			}
		}
	}

	private void displayInteractionPrompts(RaycastHit hit) {
		if (hit.collider) {
			if (hit.collider.gameObject.TryGetComponent<Interaction>(out var interaction)) {

				// in case of Interaction interaction show corresponding prompt
				if (interaction.InteractionType == Interaction.Clicks.Interaction) {
					var prompt = interactionPrompt.GetComponentInParent<HUDPrompt>();
					if (prompt) prompt.Show();
				}
				if (interaction.InteractionType == Interaction.Clicks.Primary) {
					var prompt = miningPrompt.GetComponentInParent<HUDPrompt>();
					if (prompt) prompt.Show();
				}
			}
		}
	}

	private void OnEnable() {
		playerInputs.Interactions.Interaction.Enable();
	}

	void OnDisable() {
		playerInputs.Interactions.Interaction.Disable();
	}
}
