using UnityEngine;

public class AbilityItemWrapper : ItemEditorWrapper {
    public AbilityItem abilityItem;

    override public InventoryItem GetItem() {
        return this.abilityItem;
    }
}