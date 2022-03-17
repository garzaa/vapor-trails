using UnityEngine;

public class PlasmaForHealth : Activatable {
    readonly int healthPerPlasma = 4;
    public Item plasma;

    override public void ActivateSwitch(bool b) {
        if (b) {
            InventoryController playerInventory = GlobalController.inventory;
            if (playerInventory.items.HasItem(plasma)) {
                StoredItem playerPlasma = playerInventory.items.GetItem(plasma);
                int plasmaCount = playerPlasma.count;
                playerInventory.items.RemoveItem(playerPlasma);
                GlobalController.BoostStat(StatType.HEALTH, plasmaCount*healthPerPlasma);
            }
        }
    }
}
