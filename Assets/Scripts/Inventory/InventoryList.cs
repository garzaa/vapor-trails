using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryList : MonoBehaviour {
    public List<ItemWrapper> startingItems;
    public List<InventoryItem> items;

    public InventoryList() {
        this.items = new List<InventoryItem>();
    }

    void Start() {
        // will be overwritten by deserialization, so it's OK
        if (startingItems == null || this.items.Count > 0) return;
        foreach (ItemWrapper itemWrapper in startingItems) {
            items.Add(itemWrapper.item);
        }
    }

    public InventoryItem GetItem(string itemName) {
        foreach (InventoryItem i in items) {
            if (i.itemName.Equals(itemName)) {
                return i;
            }
        }
        return null;
    }

    public InventoryItem GetItem(InventoryItem item) {
        return GetItem(item.itemName);
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
                GetItem(item).count += item.count;
            }
        } else {
            items.Add(item);
        }
    }

    public void AddAll(InventoryList inventoryList) {
        AddAll(inventoryList.items);
    }

    public void AddAll(List<InventoryItem> items) {
        foreach (InventoryItem i in items) {
            AddItem(i);
        }
    }
    
    public SerializableInventoryList MakeSerializableInventory() {
        return new SerializableInventoryList(items);
    }

    public void LoadFromSerializableInventoryList(SerializableInventoryList i) {
        this.items = i.items.Select(x => new InventoryItem(x)).ToList();
    }

    public void RemoveItem(InventoryItem toRemove) {
        if (GetItem(toRemove) == null) {
            Debug.Log("RemoveItem isn't nullsafe you brainlet");
        }
        if (toRemove.stackable) {
            GetItem(toRemove).count -= Mathf.Max(toRemove.count, 1);
        } else {
            items.Remove(GetItem(toRemove));
        } 

    }
}

[System.Serializable]
public class SerializableInventoryList {
    public List<SerializableItem> items;
    
    public SerializableInventoryList(List<InventoryItem> items) {
        this.items = items.Select(x => x.MakeSerialized()).ToList();
    }
}