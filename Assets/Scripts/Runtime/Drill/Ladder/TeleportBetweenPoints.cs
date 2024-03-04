using UnityEngine;
using UnityEngine.Events;

public class TeleportBetweenPoints : MonoBehaviour {

	[SerializeField]
	private Transform pointA;
	[SerializeField]
	private Transform pointB;
	[SerializeField]
	private UnityEvent teleportedToA;
	[SerializeField]
	private UnityEvent teleportedToB;

	public void Teleport(GameObject other) {
		var controller = other.GetComponent<CharacterController>();

		var distanceToA = Vector3.Distance(pointA.position, other.transform.position);
		var distanceToB = Vector3.Distance(pointB.position, other.transform.position);

		controller.enabled = false;
		if (distanceToA < distanceToB) {
			other.transform.position = pointB.position;
			teleportedToB.Invoke();
		} else {
			other.transform.position = pointA.position;
			teleportedToA.Invoke();
		}
		controller.enabled = true;
	}
}
