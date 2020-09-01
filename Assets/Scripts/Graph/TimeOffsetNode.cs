using UnityEngine;
using XNode;

public class TimeOffsetNode : CombatNode {
    public float normalizedOffset;

    [Input(backingValue=ShowBackingValue.Never)]
    public AttackLink input;

    [Output(backingValue=ShowBackingValue.Never, connectionType=ConnectionType.Override)]
    public AttackLink output;

    override public void OnNodeEnter() {
        base.OnNodeEnter();
        AttackNode next = GetPort("output").Connection.node as AttackNode;
        next.timeOffset = this.normalizedOffset;
        attackGraph.MoveNode(next);
    }
}