using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerOxygen : MonoBehaviour {
	public UnityEvent onOxygenZero;

	[SerializeField]
	private float passiveOxygenRegeneration = 10;
	[SerializeField]
	private int oxygenRegenDelay = 2;
	private bool isLosingOxygen;

	[HideInInspector]
	public float Oxygen;
	public float MaxOxygen = 100f; // TODO: get from playerdata for possible upgrade modifications to it

	private void Start() {
		Oxygen = MaxOxygen;
	}

	private void Update() {
		if (!isLosingOxygen) {
			GainOxygen(passiveOxygenRegeneration * Time.deltaTime);
		}
	}

	public void LoseOxygen(float damage) {
		Oxygen -= damage;
		Oxygen = Oxygen < 0 ? 0 : Oxygen;

		if (Oxygen <= 0) {
			onOxygenZero.Invoke();
		}
		StopAllCoroutines();
		StartCoroutine(delayOxygenRegen());
	}

	public void GainOxygen(float amount) {
		Oxygen += amount;
		Oxygen = Oxygen > MaxOxygen ? MaxOxygen : Oxygen;
	}

	private IEnumerator delayOxygenRegen() {
		isLosingOxygen = true;
		yield return new WaitForSeconds(oxygenRegenDelay);
		isLosingOxygen = false;
	}
}
