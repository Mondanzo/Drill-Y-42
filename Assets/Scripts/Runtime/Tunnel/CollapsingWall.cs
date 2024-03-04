 using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.VFX;


[RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
public class CollapsingWall : MonoBehaviour {
	private float cachedDistance = 0;
	private BoxCollider collider;

	public bool isDeadly = true;
	private VisualEffect[] visualEffects;
	private StudioEventEmitter emitter;
	[SerializeField] private ShakeCamInTrigger camShakeZone;

	private void Start() {
		visualEffects = GetComponentsInChildren<VisualEffect>();
		emitter = GetComponent<StudioEventEmitter>();
		SetDeadly(isDeadly);
	}

	public void SetDeadly(bool newDeadly) {
		isDeadly = newDeadly;
		
		if (camShakeZone) camShakeZone.enabled = isDeadly;
		
		if(isDeadly) {
			emitter.Play();
		} else {
			emitter.Stop();
		}
		
		foreach (var vfx in visualEffects) {
			if (isDeadly) {
				vfx.Play();
				
			} else {
				vfx.Stop();
			}
		}
	}
	
	public void Awake() {
		collider = GetComponent<BoxCollider>();
	}

	public void OnTriggerEnter(Collider other) {
		if (!isDeadly) return;

		if (other.TryGetComponent(out PlayerDeath death)) {
			death.Kill("Buried in debris.");
			death.ShowDeathScreen();
		}
	}

	[ContextMenu("Refresh Distance")]
	public float GetDistance() {
		if (cachedDistance == 0) {
			collider = GetComponent<BoxCollider>();
			cachedDistance = collider.size.z;
		}

		return cachedDistance;
	}

	public Vector3 GetSize() {
		if(collider == null) {
			return GetComponent<BoxCollider>().size;
		}
		
		return collider.size;
	}

	public Vector3 GetCenter() {
		if(collider == null) {
			return GetComponent<BoxCollider>().center;
		}
		
		return collider.center;
	}
}