using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryItemEvent : MonoBehaviour {

	public UnityEvent OnItemsCollected;

	public bool destroyOnFire = true;
	public int requiredAmount = 5;
	public ItemData itemToCheckFor;

	private bool fired = false;

	// Update is called once per frame
	void Update() {
		PlayerItems items = FindObjectOfType<PlayerItems>();
		if (items) {
			if (items.inventory.GetItemQuantity(itemToCheckFor) >= requiredAmount && !fired) {
				fired = true;
				if (destroyOnFire) {
					if(OnItemsCollected != null) OnItemsCollected.Invoke();
					Destroy(this);
				}
			} else if (items.inventory.GetItemQuantity(itemToCheckFor) < requiredAmount) {
				fired = false;
			}
		}
	}
}