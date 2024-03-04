using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu]
public class Inventory : ScriptableObject {
	public delegate void OnItemAdded(ItemInstance item, List<ItemInstance> items);
	public static event OnItemAdded onItemAdded;
	public delegate void OnItemRemoved(ItemInstance item, List<ItemInstance> items);
	public static event OnItemRemoved onItemRemoved;

	public List<ItemInstance> items = new();
    public void AddItem(ItemData item, int quantity)
    {
        ItemInstance itemInstance = items.Find(i => i.itemType == item);
        if (itemInstance != null)
        {
            itemInstance.quantity += quantity;
			
			if (onItemAdded != null) {
				onItemAdded(itemInstance, items);
			}
        } else {
			ItemInstance createdItem = new ItemInstance(item);
			items.Add(createdItem);

			if (onItemAdded != null) {
				onItemAdded(createdItem, items);
			}
		}
	}

    public void RemoveItem(ItemData item, int quantity)
    {
        ItemInstance itemInstance = items.Find(i => i.itemType == item);
        if (itemInstance != null)
        {
            itemInstance.quantity -= quantity;
            if (itemInstance.quantity <= 0)
            {
                items.Remove(itemInstance);
            }

			if (onItemRemoved != null) {
				onItemRemoved(itemInstance, items);
			}
		}
    }

	public ItemInstance GetItem(ItemData item) {
		var itemInstance = items.Find(i => i.itemType == item);
		return itemInstance;
	}

    public int GetItemQuantity(ItemData item)
    {
        ItemInstance itemInstance = items.Find(i => i.itemType == item);
        if (itemInstance != null)
        {
            return itemInstance.quantity;
        }
        else
        {
            return 0;
        }
    }
}
