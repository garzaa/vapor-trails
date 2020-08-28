using UnityEngine;
using XNode;

public class InitialBranchNode : AttackNode {
    public float speedBarrier;

    [Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
    public AttackLink[] speedLinks;

    override public AttackNode GetNextNode(AttackBuffer buffer) {
        AttackNode next = null;

        if ((this.graph as PlayerAttackGraph).rb2d.velocity.magnitude >= speedBarrier) {
            next = MatchAttackNode(buffer, speedLinks, portListName:"speedLinks");
        }

        if (next == null) {
            next = MatchAttackNode(buffer, links);
        }

        return next;
    }
}