using UnityEngine;
using XNode;

public class InitialBranchNode : AttackNode {
    public float speedBarrier;

    [Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
    public AttackLink[] speedLinks;

    override public AttackNode GetNextNode(AttackBuffer buffer) {
        AttackNode next = null;

        if ((this.graph as PlayerAttackGraph).rb2d.velocity.magnitude >= speedBarrier){
            Debug.Log("looking at speed links");
            next = MatchAttackNode(buffer, speedLinks);
        }

        if (next == null) {
            next = MatchAttackNode(buffer, links);
        } else {
            Debug.Log("found next speed node "+next.attackName);
        }

        return next;
    }
}