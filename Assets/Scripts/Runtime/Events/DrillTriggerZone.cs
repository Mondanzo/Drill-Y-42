using UnityEngine;
using UnityEngine.Events;

public class DrillTriggerZone : MonoBehaviour {
	public bool OneShot = false;
	private bool oneShot = false;

	public UnityEvent DrillEntered;
	public UnityEvent DrillStay;
	public UnityEvent DrillExited;


	public void Destroy(GameObject obj) {
		GameObject.Destroy(obj);
	}

	private void OnTriggerEnter(Collider other) {
		if (oneShot) return;

		if (other.CompareTag("Drill")) {
			DrillEntered.Invoke();
			oneShot = OneShot;
		}
	}
	private void OnTriggerStay(Collider other) {
		if (oneShot) return;

		if (other.CompareTag("Drill")) {
			DrillStay.Invoke();
			oneShot = OneShot;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (oneShot) return;

		if (other.CompareTag("Drill")) {
			DrillExited.Invoke();
			oneShot = OneShot;
		}
	}
}
