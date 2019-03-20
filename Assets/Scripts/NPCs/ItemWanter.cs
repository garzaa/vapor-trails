using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemWanter : PersistentObject {
    public List<ItemWrapper> wantedItems;
    public Activatable yesActivation;
    public Activatable noActivation;

    bool acceptedItemBefore;
    bool persistent;
    public bool consumesItems = false;

    override public void Start() {
        persistentProperties = new Hashtable();
        SerializedPersistentObject o = LoadObjectState();
        ConstructFromSerialized(o);
    }

    override public void ConstructFromSerialized(SerializedPersistentObject s) {
        if (s == null) {
			return;
		}
		this.persistentProperties = s.persistentProperties;
        acceptedItemBefore = (bool) this.persistentProperties["Accepted"];
    }

    public void CheckForItem(InventoryList inventoryToCheck) {
        if (acceptedItemBefore) {
            return;
        }
        List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
        foreach (InventoryItem wantedItem in actualWantedItems) {
            InventoryItem i = inventoryToCheck.GetItem(wantedItem);
            if (!(i != null && i.count >= wantedItem.count)) {
                //reject if even one item is missing
                RejectItems();
            }
        }
        AcceptItems();
    }

    protected override void UpdateObjectState() {
        persistentProperties.Add("Accepted", acceptedItemBefore);
        if (persistent) SaveObjectState();
    }

    void AcceptItems() {
        List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
        if (consumesItems) {
            foreach (InventoryItem wantedItem in actualWantedItems) {
                GlobalController.inventory.items.RemoveItem(wantedItem);
            }
        }
        acceptedItemBefore = true;
        UpdateObjectState();
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