using UnityEngine;

public class KeepPlayerOnMovingObject : MonoBehaviour {
	[SerializeField]
	private Transform movingObject;
	private Transform player;

	private int counter = 0;

	private void Start() {
		if (!movingObject) {
			movingObject = transform.parent.transform;
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			counter++;
			player = other.transform;
			player.parent = movingObject;
		}
	}

	public void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			counter--;
			if (counter <= 0) {
				player = other.transform;
				player.parent = null;
			}
		}
	}
}
