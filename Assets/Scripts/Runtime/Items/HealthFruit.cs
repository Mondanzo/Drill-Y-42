using UnityEngine;

public class HealthFruit : MonoBehaviour {
	[SerializeField]
	private ItemData itemToGive;

	public void pickUp(GameObject other) {
		if (other.CompareTag("Player")) {
			if (other.GetComponentInChildren<Stimpack>().StimpacksFull()) {
				other.GetComponentInChildren<HUDStim>().Shake();
			} else {
				other.GetComponent<PlayerItems>().inventory.AddItem(itemToGive, 1);
				Destroy(gameObject);
			}
		}
	}
}
