using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemWanter : MonoBehaviour {
    public List<Item> wanted;
    public Activatable yesActivation;
    public Activatable noActivation;

    const string accepted = "accepted";
    public bool consumesItems = false;

    public void CheckForItem(InventoryController inventoryToCheck) {
        foreach (Item wantedItem in wanted) {
            StoredItem i = inventoryToCheck.items.GetItem(wantedItem.name);
            if (i == null) {
                //reject if even one item is missing
                RejectItems();
                return;
            }
        }
        AcceptItems();
    }

    void AcceptItems() {
        if (consumesItems) {
            foreach (Item wantedItem in wanted) {
                GlobalController.inventory.items.RemoveItem(new StoredItem(wantedItem));
            }
        }
        if (yesActivation != null) {
            yesActivation.Activate();
        }
    }

    void RejectItems() {
        if (noActivation != null) {
            noActivation.Activate();
        }
    }
}
