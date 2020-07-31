using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryList : MonoBehaviour {
    public List<Item> items;

    public InventoryList() {
        this.items = new List<Item>();
    }

    public Item GetItem(string itemName) {
        foreach (Item i in items) {
            if (i.name.Equals(itemName)) {
                return i;
            }
        }
        return null;
    }

    public void Empty() {
        items.Clear();
    }

    public Item GetItem(Item item) {
        return GetItem(item.name);
    }

    public bool HasItem(string itemName) {
        return GetItem(itemName) != null;
    }

    public bool HasItem(Item item) {
        return GetItem(item) != null;
    }

    public Item GetItemByIndex(int index) {
        return items[index];
    }

    // don't double items when added to the inventory
    public void AddItem(Item item) {
        Item instance = item.Instance();
        if (instance.stackable && HasItem(instance)) {
            GetItem(instance).count += instance.count;
        } else {
            items.Add(instance);
        }
    }

    public void AddAll(InventoryList inventoryList) {
        AddAll(inventoryList.items);
    }

    public void AddAll(List<Item> items) {
        foreach (Item i in items) {
            AddItem(i);
        }
    }
    
    public SerializableInventoryList MakeSerializableInventory() {
        return new SerializableInventoryList(items);
    }

    public void LoadFromSerializableInventoryList(SerializableInventoryList i) {
        this.items = i.items.Select(x => ItemDB.GetItem(x)).ToList();
    }

    public void RemoveItem(Item toRemove) {
        if (GetItem(toRemove) == null) {
            Debug.Log("RemoveItem isn't nullsafe you brainlet");
        }
        if (toRemove.stackable) {
            GetItem(toRemove).count -= Mathf.Max(toRemove.count, 1);
            if (GetItem(toRemove).count == 0) {
                items.Remove(GetItem(toRemove));
            }
        } else {
            items.Remove(GetItem(toRemove));
        } 

    }
}

[System.Serializable]
public class SerializableInventoryList {
    public List<SerializableItem> items;
    
    public SerializableInventoryList(List<Item> items) {
        this.items = items.Select(x => x.MakeSerialized()).ToList();
    }
}