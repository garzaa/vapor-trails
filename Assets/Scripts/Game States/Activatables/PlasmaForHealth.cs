using UnityEngine;

public class PlasmaForHealth : Activatable {
    readonly int healthPerPlasma = 4;
    public Item plasma;

    override public void ActivateSwitch(bool b) {
        if (b) {
            InventoryList playerInventory = GlobalController.inventory.items;
            if (playerInventory.HasItem(plasma)) {
                StoredItem playerPlasma = playerInventory.GetItem(plasma);
                int plasmaCount = playerPlasma.count;
                playerInventory.RemoveItem(playerPlasma);
                GlobalController.BoostStat(StatType.HEALTH, plasmaCount*healthPerPlasma);
            }
        }
    }
}