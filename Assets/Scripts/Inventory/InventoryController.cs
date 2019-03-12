using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryController : MonoBehaviour {
    List<InventoryItem> items;

    //for debugging
    public int debugItemCount;

    int GetItemCount() {
        return debugItemCount;
    }

    public void AddItem(InventoryItem item) {
        if (item.stackable) {
            
        }
    }

    public InventoryItem GetItem(string itemName) {
        foreach (InventoryItem i in items) {
            if (i.name.Equals(itemName)) {
                return i;
            }
        }
        return null;
    }

    public bool HasItem(string itemName) {
        return GetItem(itemName) != null;
    }
}
