using UnityEngine;

public class ActivatedItemWanter : Activatable {
    public ItemWanter itemWanter;

    override public void Activate() {
        itemWanter.CheckForItem(GlobalController.inventory.items);
    }
}