using UnityEngine;
using XNode;

public class MoveGraphNode : CombatNode {
    public CombatNode targetNode;

    [Input(backingValue=ShowBackingValue.Never)]
    public AttackLink input;

    override public void OnNodeEnter() {
        base.OnNodeEnter();
        GlobalController.pc.EnterAttackGraph(
            targetNode.graph as PlayerAttackGraph,
            targetNode
        );
    }
}