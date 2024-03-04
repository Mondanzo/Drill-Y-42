using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class ItemVacuum : MonoBehaviour {

	[SerializeField]
	private float vacuumForce = 15f;
	[SerializeField]
	private float collectDistance = 1.5f;

	private Dictionary<string, float> collectedItems = new Dictionary<string, float>();
	private Dictionary<string, int> collectedItemsAmount = new Dictionary<string, int>();
	private List<GameObject> collectedItemsInstances = new List<GameObject>();

	private Popup popup;

	private void Start() {
		GameObject player = transform.parent.gameObject;
		
		GameObject UserInterface = player.transform.Find("UserInterface").gameObject;
		GameObject HUD = UserInterface.transform.Find("HUD").gameObject;
		popup = HUD.GetComponent<Popup>();
	}

	private void vacuumItems(Collider other) {
		// TODO: maybe use layer instead because apparently this is the fastest comparison
		if (other.gameObject.CompareTag("Items")) {
			var rigidBody = other.GetComponent<Rigidbody>();

			Vector3 relativeDirection = transform.position - other.transform.position;
			rigidBody.AddForce(relativeDirection.normalized * vacuumForce);

			if (relativeDirection.sqrMagnitude <= collectDistance && !collectedItemsInstances.Contains(other.gameObject)) {
				if (other.TryGetComponent<DroppedItem>(out var droppedItem)) {
					ItemData itemData = droppedItem.GetItemData();
					
					collectedItemsInstances.Add(other.gameObject);

					if (collectedItems.ContainsKey(itemData.itemName)) {
						collectedItems[itemData.itemName] = 3;
						collectedItemsAmount[itemData.itemName] += 1;
					} else {
						collectedItems.Add(itemData.itemName, 3);
						collectedItemsAmount.Add(itemData.itemName, 1);
					}

					popup.PopupEvent("+" + collectedItemsAmount[itemData.itemName].ToString() + " " + itemData.itemName, 0, itemData.itemName);

					droppedItem.pickUpItem();
				}
			}
		}
	}

	private void Update() {
		foreach (var item in collectedItems.ToList()) {
			collectedItems[item.Key] -= Time.deltaTime;
			if (collectedItems[item.Key] <= 0) {
				collectedItems.Remove(item.Key);
				collectedItemsAmount.Remove(item.Key);
				collectedItemsInstances.Clear();
				popup.DeletePopupId(item.Key);
			}
		}
	}

	private void OnTriggerStay(Collider other) {
		vacuumItems(other);
	}
}
