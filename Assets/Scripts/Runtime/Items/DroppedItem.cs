using UnityEngine;

public class DroppedItem : MonoBehaviour {
	public ItemData itemToGive;
	private GameObject player;

	[SerializeField]
	private AudioClip audioClip;

	private bool isCollected = false;

	public ItemData getItemData() {
		return itemToGive;
	}

	void Start() {
		player = GameObject.Find("Player");
	}

	public void pickUpItem() {
		if (isCollected) return;

		player.GetComponent<PlayerItems>().inventory.AddItem(itemToGive, 1);
		
		if (audioClip) {
			AudioSource.PlayClipAtPoint(audioClip, transform.position);
		}

		isCollected = true; // vacuum gets triggered on physics frame which leads two instances of the item getting collected without this bool

		Destroy(gameObject);
	}
	
	public ItemData GetItemData() {
		return itemToGive;
	}
}
