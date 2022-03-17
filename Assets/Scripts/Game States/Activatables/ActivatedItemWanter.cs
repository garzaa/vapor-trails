using UnityEngine;

public class ActivatedItemWanter : Activatable {
    public ItemWanter itemWanter;

    override public void Activate() {
        itemWanter.CheckForItem(GlobalController.inventory);
    }

    override public void ActivateSwitch(bool b) {
        if (b) Activate();
    }
}
