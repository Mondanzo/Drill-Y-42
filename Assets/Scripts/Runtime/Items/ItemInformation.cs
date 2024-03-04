using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemInformation : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();

    public ItemData getItemData(string itemName)
    {
        foreach (ItemData item in items)
        {
            if (item.itemName == itemName)
            {
                return item;
            }
        }

        return null;
    }
}
