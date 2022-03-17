using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryList : PersistentObject {
    private List<StoredItem> _items = null;

    public List<StoredItem> items  {
        get {
            if (_items == null) {
                base.OnEnable();
            }
            return _items;
        }
    }

    protected override void SetDefaults() {
        SetDefault(nameof(items), new List<StoredItem>());
        _items = GetList<StoredItem>(nameof(items));
    }

    public StoredItem GetItem(string itemName) {
        foreach (StoredItem i in items) {
            if (i.name.Equals(itemName)) {
                return i;
            }
        }
        return null;
    }

    public void Clear() {
        items.Clear();
    }

    public bool IsEmpty() {
        return items != null && items.Count > 0;
    }

    public List<StoredItem> GetAll() {
        return items;
    }

    public StoredItem GetItem(Item item) {
        return GetItem(item.name);
    }

    public StoredItem GetItem(StoredItem item) {
        return GetItem(item.name);
    }

    public bool HasItem(string itemName) {
        return GetItem(itemName) != null;
    }

    public bool HasItem(Item item) {
        return HasItem(item.name);
    }
    
    public int GetItemCount(Item item) {
        if (!HasItem(item)) {
            return 0;
        } else {
            return GetItem(item.name).count;
        }
    }

    public bool HasItem(StoredItem stored) {
        return HasItem(stored.name);
    }

    public void AddItem(StoredItem s) {
        if (s.item.stackable && HasItem(s)) {
            GetItem(s).count += s.count;
        } else {
            items.Add(s);
        }
    }

    public void AddItem(Item s) {
        AddItem(new StoredItem(s));
    }

    public void AddAll(InventoryList inventoryList) {
        AddAll(inventoryList.items);
    }

    public void AddAll(List<Item> items) {
        foreach (Item i in items) {
            AddItem(i);
        }
    }

    public void AddAll(List<StoredItem> items) {
        foreach (StoredItem i in items) {
            AddItem(i.item);
        }
    }

    public void RemoveItem(StoredItem toRemove) {
        if (GetItem(toRemove) == null) {
            Debug.Log("RemoveItem isn't nullsafe you brainlet");
        }
        if (toRemove.item.stackable) {
            GetItem(toRemove).count -= Mathf.Max(toRemove.count, 1);
            if (GetItem(toRemove).count == 0) {
                items.Remove(GetItem(toRemove));
            }
        } else {
            items.Remove(GetItem(toRemove));
        } 

    }
}
