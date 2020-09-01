using UnityEngine;
using XNode;

[NodeWidth(270)]
public class UnlockableGate : CombatNode {
    public Item requiredItem;

    [Input(backingValue=ShowBackingValue.Never)]
    public AttackLink input;

    [Output(backingValue=ShowBackingValue.Never, connectionType=ConnectionType.Override)]
    public AttackLink unlockedBranch;

    override public bool Enabled() {
        return GlobalController.inventory.items.HasItem(requiredItem);
    }

    override public void OnNodeEnter() {
        base.OnNodeEnter();
        attackGraph.MoveNode(GetPort("unlockedBranch").Connection.node as CombatNode);
    }
}