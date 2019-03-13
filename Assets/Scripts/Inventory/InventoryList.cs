using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryList : System.Object {
    List<InventoryItem> items;

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
            //always check via name - items can have differing counts
            if (i.name.Equals(item.name)) {
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

    public void AddItem(InventoryItem item) {
        if (HasItem(item) && item.stackable) {
            GetItem(item).count += item.count;
        } else if (!HasItem(item)) {
            items.Add(item);
        }
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