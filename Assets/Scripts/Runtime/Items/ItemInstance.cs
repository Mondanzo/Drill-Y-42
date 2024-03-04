using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemData itemType;
    public int quantity;
    public ItemInstance(ItemData itemData) {
        itemType = itemData;
        quantity = 1;
    }
}