using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoFlip : StateMachineBehaviour {

    Entity e;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        e = animator.GetComponent<Entity>();
        e.canFlip = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        e.canFlip = true;
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        e.canFlip = false;
    }
}
