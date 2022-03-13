using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemWanter : PersistentObject {
    public List<Item> wanted;
    public Activatable yesActivation;
    public Activatable noActivation;

    bool acceptedItemBefore;
    public bool persistent = false;
    public bool consumesItems = false;

    override protected void Start() {
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
        foreach (Item wantedItem in wanted) {
            StoredItem i = inventoryToCheck.GetItem(wantedItem.name);
            if (i == null) {
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
            foreach (Item wantedItem in wanted) {
                GlobalController.inventory.items.RemoveItem(new StoredItem(wantedItem));
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
