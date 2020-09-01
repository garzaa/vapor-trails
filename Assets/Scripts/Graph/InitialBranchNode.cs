using UnityEngine;
using XNode;

public class InitialBranchNode : AttackNode {
    public float speedBarrier;

    [Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
    public AttackLink[] speedLinks;

    override public void OnNodeEnter() {
        base.OnNodeEnter();
        CombatNode next = GetNextNode(attackGraph.buffer);
        if (next != null) {
            attackGraph.MoveNode(GetNextNode(attackGraph.buffer));
        } else {
            attackGraph.ExitGraph();
        }
    }

    override public CombatNode GetNextNode(AttackBuffer buffer) {
        CombatNode next = null;

        if (speedBarrier > 0 && ((this.graph as PlayerAttackGraph).rb2d.velocity.magnitude >= speedBarrier)) {
            next = MatchAttackNode(buffer, speedLinks, portListName:"speedLinks");
        }

        if (next == null) {
            next = MatchAttackNode(buffer, links);
        }

        return next;
    }
}