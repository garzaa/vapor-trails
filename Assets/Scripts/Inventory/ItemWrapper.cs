using UnityEngine;

public class ItemWrapper : MonoBehaviour {
    public InventoryItem item {
        get { return GetItem(); }
        set { item = value; }
    }

    virtual public InventoryItem GetItem() {
        return this.item;
    }

    public ItemWrapper(InventoryItem item) {
        this.item = item;
    }    
}