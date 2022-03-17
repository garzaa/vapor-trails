using UnityEngine;
using XNode;

[NodeWidth(270)]
public class UnlockableGate : CombatNode {
    public Item requiredItem;

    [Input(backingValue=ShowBackingValue.Never)]
    public AttackLink input;

    [Output(backingValue=ShowBackingValue.Never, connectionType=ConnectionType.Override)]
    public AttackLink unlockedBranch;

    CombatNode GetUnlockedNode() {
        return (GetPort("unlockedBranch").Connection.node as CombatNode);
    }

    override public bool Enabled() {
        return base.Enabled() 
        && GlobalController.inventory.items.HasItem(requiredItem)
        && GetUnlockedNode().Enabled();
    }

    override public void OnNodeEnter() {
        base.OnNodeEnter();
        attackGraph.MoveNode(GetUnlockedNode());
    }
}
