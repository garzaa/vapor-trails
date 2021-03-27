using UnityEngine;

public class ItemCheck : CheckNode {
    public Item item;

    protected override bool Check() {
        return GlobalController.inventory.items.HasItem(item.name);
    }
}
