using UnityEngine;
using System.Collections.Generic;

public class InteractableItemWanter : Interactable {
    public ItemWanter itemWanter;

    void Start() {
        if (itemWanter == null) {
            itemWanter = GetComponent<ItemWanter>();
        }
    }

    override public void Interact(GameObject player) {
        itemWanter.CheckForItem(GlobalController.inventory.items);
    }
}