using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryList : System.Object {
    public List<InventoryItem> items;

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

    public bool HasItem(InventoryItem item) {
        return GetItem(item) != null;
    }

    public InventoryItem GetItemByIndex(int index) {
        return items[index];
    }
    
    public SerializableInventoryList MakeSerializableInventory() {
        return new SerializableInventoryList(this.items);
    }

    public void LoadFromSerializableInventoryList(SerializableInventoryList i) {
        this.items = i.items;
    }
}

[System.Serializable]
public class SerializableInventoryList {
    public List<InventoryItem> items;
    
    public SerializableInventoryList(List<InventoryItem> items) {
        this.items = items;
    }
}