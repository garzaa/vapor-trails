public class UnlockableAttackNode : AttackNode {
    public Item requiredItem;

    override public bool Enabled() {
        return GlobalController.inventory.items.HasItem(requiredItem);
    }
}