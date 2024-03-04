using UnityEngine;

public class ClimbLadder : MonoBehaviour {
	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			other.GetComponent<PlayerMovement>().CurrentState = PlayerMovement.State.Climbing;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			other.GetComponent<PlayerMovement>().CurrentState = PlayerMovement.State.Walking;
		}
	}
}
