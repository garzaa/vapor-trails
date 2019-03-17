using UnityEngine;
using System;

public class ItemWrapper : MonoBehaviour {
    public InventoryItem item;

    virtual public InventoryItem GetItem() {
        return item.Clone();
    }

    public ItemWrapper(InventoryItem item) {
        this.item = item;
    }    
}