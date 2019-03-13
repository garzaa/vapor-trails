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
            GetItem(item).count += item.count;
        } else if (!HasItem(item.name)) {
            items.Add(item);
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

    public InventoryItem GetItem(InventoryItem item) {
        foreach (InventoryItem i in items) {
            if (i.Equals(item)) {
                return i;
            }
        }
        return null;
    }

    public bool HasItem(string itemName) {
        return GetItem(itemName) != null;
    }
    
    public SerializableInventory MakeSerializableInventory() {
        return new SerializableInventory(this.items);
    }

    public void LoadFromSerializableInventory(SerializableInventory i) {
        this.items = i.items;
    }
}

[System.Serializable]
public class SerializableInventory {
    public List<InventoryItem> items;
    
    public SerializableInventory(List<InventoryItem> items) {
        this.items = items;
    }
}