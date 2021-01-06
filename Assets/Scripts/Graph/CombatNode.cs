using UnityEngine;
using XNode;
using System.Collections.Generic;
     
public class CombatNode : Node {
    [HideInInspector]
    bool active;

    [HideInInspector]
    protected bool cancelable;

    protected PlayerAttackGraph attackGraph;

    virtual public void OnNodeEnter() {
        attackGraph = this.graph as PlayerAttackGraph;
        active = true;
        attackGraph.playerController.OnAttackNodeEnter();
    }

    virtual public void NodeUpdate(int currentFrame, float clipTime, AttackBuffer buffer) {

    }

    virtual public void OnNodeExit() {
        cancelable = false;
        active = false;
    }

    virtual public bool Enabled() {
        return true;
    }

    virtual public void OnGroundHit() {

    }

    public bool IsActive() {
        return active;
    }

    virtual public void OnAttackLand() {
        cancelable = true;
    }
}
