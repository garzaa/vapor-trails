using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemWanter : PersistentObject {
    public List<ItemWrapper> wantedItems;
    public Activatable toActivate;

    bool acceptedItemBefore;
    bool persistent;
    bool consumesItems = true;

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

    public bool CheckForItem(InventoryList inventoryToCheck) {
        List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
        foreach (InventoryItem wantedItem in actualWantedItems) {
            print(wantedItem.itemName);
            InventoryItem i = inventoryToCheck.GetItem(wantedItem);
            if (!(i != null && i.count >= wantedItem.count)) {
                return false;
            }
        }
        return true;
    }

    protected override void UpdateObjectState() {
        persistentProperties.Add("Accepted", acceptedItemBefore);
        if (persistent) SaveObjectState();
    }

    public void AcceptItems() {
        List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
        if (consumesItems) {
            foreach (InventoryItem wantedItem in actualWantedItems) {
                GlobalController.inventory.items.RemoveItem(wantedItem);
            }
        }
        acceptedItemBefore = true;
        UpdateObjectState();
        if (toActivate != null) {
            toActivate.Activate();
        }
    }
}