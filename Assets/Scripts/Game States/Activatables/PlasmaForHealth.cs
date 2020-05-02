using UnityEngine;

public class PlasmaForHealth : Activatable {
    readonly int healthPerPlasma = 4;
    [SerializeField] ItemWrapper plasmaItem;

    override public void ActivateSwitch(bool b) {
        if (b) {
            InventoryList playerInventory = GlobalController.inventory.items;
            InventoryItem plasma = plasmaItem.GetItem();
            if (playerInventory.HasItem(plasma)) {
                InventoryItem playerPlasma = playerInventory.GetItem(plasma);
                int plasmaCount = playerPlasma.count;
                playerInventory.RemoveItem(playerPlasma);
                GlobalController.BoostStat(StatType.HEALTH, plasmaCount*healthPerPlasma);
            }
        }
    }
}