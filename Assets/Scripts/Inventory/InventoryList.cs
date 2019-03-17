using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryList : MonoBehaviour {
    public List<InventoryItem> items;

    public InventoryItem GetItem(string itemName) {
        foreach (InventoryItem i in items) {
            if (i.itemName.Equals(itemName)) {
                return i;
            }
        }
        return null;
    }

    public InventoryItem GetItem(InventoryItem item) {
        foreach (InventoryItem i in items) {
            //always check via name - items can have differing counts
            if (i.itemName.Equals(item.itemName)) {
                Debug.Log(i.count);
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
        if (HasItem(item)) {
            if (item.IsAbility()) { 
                return;
		    }
            if (item.stackable) {
                Debug.Log(item.count);
                Debug.Log(GetItem(item).count);
                GetItem(item).count += item.count;
            }
        } else {
            items.Add(item);
        }
    }

    public void AddAll(InventoryList inventoryList) {
        foreach (InventoryItem i in inventoryList.items) {
            AddItem(i);
        }
    }
    
    public SerializableInventoryList MakeSerializableInventory() {
        return new SerializableInventoryList(items);
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