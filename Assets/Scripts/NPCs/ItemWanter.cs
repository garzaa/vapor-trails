using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ItemWanter : PersistentObject {
    public List<ItemWrapper> wantedItems;
    public NPC acceptedNPC;
    public NPC rejectedNPC;

    bool acceptedItemBefore;

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

    public bool CheckForItem() {
        InventoryList playerInventory = GlobalController.inventory.items;
        List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
        foreach (InventoryItem wantedItem in actualWantedItems) {
            print(wantedItem.itemName);
            InventoryItem i = playerInventory.GetItem(wantedItem);
            if (!(i != null && i.count >= wantedItem.count)) {
                print("pingas");
                return false;
            }
        }
        return true;
    }

    protected override void UpdateObjectState() {
        persistentProperties.Add("Accepted", acceptedItemBefore);
        SaveObjectState();
    }

    public void TakeItems() {
        List<InventoryItem> actualWantedItems = wantedItems.Select(x => x.GetItem()).ToList();
        foreach (InventoryItem wantedItem in actualWantedItems) {
            GlobalController.inventory.items.RemoveItem(wantedItem);
        }
        acceptedItemBefore = true;
        UpdateObjectState();
    }

    public NPC GetNPC(bool accepted) {
        return accepted ? acceptedNPC : rejectedNPC;
    }
}