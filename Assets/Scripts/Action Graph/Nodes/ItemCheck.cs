using UnityEngine;

public class ItemCheck : CheckNode {
    public Item item;
    public int count = 1;

    protected override bool Check() {
        StoredItem i = GlobalController.inventory.items.GetItem(item.name);
        return (i != null) && i.count >= count;
    }
}
