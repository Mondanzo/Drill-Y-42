using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveBack : MonoBehaviour {
	public List<Transform> ObjectsToMoveBack;

	public void MoveBackByAmount(float amount, List<Transform> additionalObjects) {
		var totalList = new List<Transform>();
		totalList.AddRange(ObjectsToMoveBack);
		totalList.AddRange(additionalObjects);
		foreach (var obj in totalList) {
			CharacterController controller;

			if (obj.TryGetComponent(out controller)) {
				controller.enabled = false;
			}
			if(!ObjectsToMoveBack.Contains(obj.parent)) {
				obj.position += Vector3.back * amount;
			}

			if (controller != null) {
				controller.enabled = true;
			}
		}
	}
}