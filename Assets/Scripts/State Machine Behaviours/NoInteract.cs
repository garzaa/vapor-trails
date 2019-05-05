using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoInteract : StateMachineBehaviour {
    PlayerController e;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        e = animator.GetComponent<PlayerController>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        e.canInteract = false;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        e.canInteract = true;
    }
}