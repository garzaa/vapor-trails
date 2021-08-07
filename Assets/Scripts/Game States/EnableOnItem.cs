using UnityEngine;
using System.Collections.Generic;

public class EnableOnItem : StateChangeReactor {
    public Item wanted;
    public List<Item> wantedItems;
    public bool immediate = true;

    public bool setDisabled = false;

    override public void React(bool fakeSceneLoad) {
        if (!immediate && !fakeSceneLoad) return;

        if (wanted != null) {
            StoredItem i = GlobalController.inventory.items.GetItem(wanted);
            bool hasItem = (i != null);
            if (setDisabled) gameObject.SetActive(!hasItem);
            else gameObject.SetActive(hasItem);
        } else {
            bool satisfied = true;
            foreach (Item i in wantedItems) {
                if (!GlobalController.inventory.items.HasItem(i)) {
                    satisfied = false;
                    break;
                }
            }
            if (setDisabled) gameObject.SetActive(!satisfied);
            else gameObject.SetActive(satisfied);
        }
    }

}
