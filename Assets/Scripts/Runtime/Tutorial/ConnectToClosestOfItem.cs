using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ConnectToClosestOfItem : MonoBehaviour {

	public ItemData itemToTrack;
	public Vector3 offset = new Vector3(0, 3f, 0);

	private Transform currentTarget;

	// Update is called once per frame
	void Update() {
		if (currentTarget) {
			transform.position = currentTarget.position + offset;
		} else {
			transform.position = Vector3.down * 10f;
		}
		var potentialItems = FindObjectsOfType<DroppedItem>();
		foreach (var item in potentialItems) {
			if (item.itemToGive == itemToTrack) {
				if (currentTarget) {
					if (Camera.main != null && Vector3.Distance(currentTarget.transform.position, Camera.main.transform.position) > Vector3.Distance(item.transform.position, Camera.main.transform.position)) {
						currentTarget = item.transform;
					}
				}
				else {
					currentTarget = item.transform;
				}
			}
		}
	}
}