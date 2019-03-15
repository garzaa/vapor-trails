using UnityEngine;

public class ItemEditorWrapper : MonoBehaviour {
    public InventoryItem item;

    virtual public InventoryItem GetItem() {
        return this.item;
    }
    
}