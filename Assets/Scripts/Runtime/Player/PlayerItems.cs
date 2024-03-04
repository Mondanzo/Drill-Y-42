using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : MonoBehaviour {
    [Header("Item Controls")]
    public float maxQuantityOfItems = 5f;

    [Header("References")]
    public Inventory inventory;
    public ItemInformation itemInformation;
    public GameObject crystalUIPanel;

    void Start() {
        inventory = Inventory.CreateInstance<Inventory>();
    }
    void Update() {
        //crystalUIPanel.GetComponent<CrystalVisualizer>().setCrystalAmount(inventory.GetItem(itemInformation.getItemData("Crystal")));
        // no :)
    }

#if UNITY_EDITOR
	private void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 50, 700, 700));
        // GUILayout.Label("max quantity of each item: " + maxQuantityOfItems, GUILayout.Height(20f));
        for (int i = 0; i < inventory.items.Count; i++) {
            GUILayout.Label(inventory.items[i].itemType.itemName + ": " + inventory.items[i].quantity, GUILayout.Height(20f));
        }
        GUILayout.EndArea();
    }
#endif
}
