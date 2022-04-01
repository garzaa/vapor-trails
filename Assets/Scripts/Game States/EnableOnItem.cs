using UnityEngine;
using System.Collections.Generic;

public class EnableOnItem : StateChangeReactor {
    public Item wanted;
    public List<Item> wantedItems;
    public bool immediate = true;
    public int amount = 1;

    public bool setDisabled = false;

    override public void React(bool fakeSceneLoad) {
        if (!immediate && !fakeSceneLoad) return;

        if (wanted != null) {
            StoredItem i = GlobalController.inventory.items.GetItem(wanted);
            bool hasItem = (i != null && i.count >= amount);
            if (setDisabled) {
                gameObject.SetActive(!hasItem);
            } else {
                gameObject.SetActive(hasItem);
            }
        } else {
            bool satisfied = true;
            foreach (Item i in wantedItems) {
                if (GlobalController.inventory.items.GetItemCount(wanted) < amount) {
                    satisfied = false;
                    break;
                }
            }
            if (setDisabled) gameObject.SetActive(!satisfied);
            else gameObject.SetActive(satisfied);
        }
    }
}
