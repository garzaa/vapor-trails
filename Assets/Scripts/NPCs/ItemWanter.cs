using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemWanter : PersistentObject {
    public List<ItemWrapper> wantedItems;
    public Activatable yesActivation;
    public Activatable noActivation;

    bool acceptedItemBefore;
    public bool persistent = false;
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
        List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
        foreach (InventoryItem wantedItem in actualWantedItems) {
            InventoryItem i = inventoryToCheck.GetItem(wantedItem);
            if (!(i != null && i.count >= wantedItem.count)) {
                //reject if even one item is missing
                RejectItems();
                return;
            }
        }
        AcceptItems();
    }

    protected override void UpdateObjectState() {
        if (!persistent) return;
        persistentProperties.Add("Accepted", acceptedItemBefore);
        SaveObjectState();
    }

    void AcceptItems() {
        if (consumesItems) {
            List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
            foreach (InventoryItem wantedItem in actualWantedItems) {
                Debug.Log(wantedItem.itemName);
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