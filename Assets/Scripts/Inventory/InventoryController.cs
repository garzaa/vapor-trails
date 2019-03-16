using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {
    public InventoryList items;   
    InventoryUI inventoryUI;
    bool inInventory = false;`

    void Start() {
        inventoryUI = GetComponent<InventoryUI>();
    }

    void Update() {
        if (Input.GetButtonDown("Inventory")) {
            if (inInventory) {
                inInventory = false;
                if (GlobalController.pc.inCutscene) {
                    return;
                }
                GlobalController.pc.Freeze();
                GlobalController.pc.inCutscene = true;
            }
        }
    }

    int GetItemCount() {
        return debugItemCount;
    }

    public virtual void ReactToItemSelect(InventoryItem item) {
        
    }

    public void GetItem(InventoryItem item) {
        if (item.IsAbility() && items.HasItem(item)) {
			return;
		}
		items.AddItem(item);
		item.OnPickup();
    }
}