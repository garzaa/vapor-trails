using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {
    public InventoryList items;   
    public InventoryUI inventoryUI;
    bool inInventory = false;

    void Start () {
        inventoryUI.PopulateItems(this.items);
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

    public virtual void ReactToItemSelect(InventoryItem item) {
        
    }

    public void AddItem(InventoryItem item) {
        SoundManager.ItemGetSound();
		items.AddItem(item);
        inventoryUI.PopulateItems(this.items);
		item.OnPickup();
    }
}