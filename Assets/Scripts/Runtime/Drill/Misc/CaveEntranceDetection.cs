using UnityEngine;
using UnityEngine.Events;

public class CaveEntranceDetection : MonoBehaviour {
	[SerializeField]
	private UnityEvent caveDetected;
	[SerializeField]
	private UnityEvent caveNoLongerDetected;

	private void OnTriggerEnter(Collider other) {
		caveDetected.Invoke();
	}

	private void OnTriggerExit(Collider other) {
		caveNoLongerDetected.Invoke();
	}
}
