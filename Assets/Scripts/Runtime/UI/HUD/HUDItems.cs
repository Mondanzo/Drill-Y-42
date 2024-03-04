using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDItems : MonoBehaviour
{
	public GameObject itemBarParent;
	public GameObject itemBar;
	
	private List<GameObject> itemBars = new List<GameObject>();

	public void OnEnable() {
		Inventory.onItemAdded += updateItems;
		Inventory.onItemRemoved += updateItems;
	}

	public void OnDisable() {
		Inventory.onItemAdded -= updateItems;
		Inventory.onItemRemoved -= updateItems;
	}

	private void addItemBar(ItemInstance item) {
		GameObject newItemBar = Instantiate(itemBar, itemBarParent.transform);
		newItemBar.name = item.itemType.name;
		
		itemBars.Add(newItemBar);
		
		GameObject Icon = newItemBar.transform.Find("Icon").gameObject;
		GameObject Amount = newItemBar.transform.Find("Amount").gameObject;
		
		TextMeshProUGUI AmountLabel = Amount.GetComponent<TextMeshProUGUI>();
		AmountLabel.text = item.quantity.ToString();
		
		Image IconImage = Icon.GetComponent<Image>();
		IconImage.sprite = item.itemType.itemImage;
	}
	
	private void removeItemBar(ItemInstance item) {
		foreach (GameObject itemBar in itemBars) {
			if (itemBar.name == item.itemType.name) {
				itemBars.Remove(itemBar);
				Destroy(itemBar);
				break;
			}
		}
	}
	
	public void updateItems(ItemInstance item, List<ItemInstance> items) {
		if (item.itemType.hiddenFromHUD) {
			return;
		}
		
		if (item.quantity == 0) {
			removeItemBar(item);
			return;
		}

		if (itemBars.Count == 0) {
			addItemBar(item);
		} else {
			foreach (GameObject itemBar in itemBars) {
				if (itemBar.name == item.itemType.name) {
					GameObject Amount = itemBar.transform.Find("Amount").gameObject;
					TextMeshProUGUI AmountLabel = Amount.GetComponent<TextMeshProUGUI>();
					AmountLabel.text = item.quantity.ToString();
					break;
				}
				
				if (itemBars.IndexOf(itemBar) == itemBars.Count - 1) {
					addItemBar(item);
					break;
				}
			}
		}
	}
}
