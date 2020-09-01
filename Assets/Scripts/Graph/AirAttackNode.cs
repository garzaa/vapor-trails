using UnityEngine;
using XNode;

public class AirAttackNode : AttackNode {
    [Output(backingValue=ShowBackingValue.Never, connectionType=ConnectionType.Override)]
    public AttackLink onLand;

    override public void NodeUpdate(int currentFrame, float clipTime, AttackBuffer buffer) {
        if (buffer.ready && (currentFrame>=IASA || cancelable)) {
            MoveNextNode(buffer);
        } else if (clipTime >= 1) {
            attackGraph.ExitGraph();
        }
    }

    override public void OnGroundHit() {
        if (GetPort("onLand").ConnectionCount>0) attackGraph.MoveNode(GetPort("onLand").Connection.node as CombatNode);
    }    
}