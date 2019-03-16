using UnityEngine;

public class AbilityItemWrapper : ItemWrapper {
    public AbilityItem abilityItem;

    override public InventoryItem GetItem() {
        return this.abilityItem;
    }

    public AbilityItemWrapper(AbilityItem abilityItem, InventoryItem item) : base(item) {
        this.item = item;
        this.abilityItem = abilityItem;
    }
}