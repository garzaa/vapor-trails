using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntInState : StateMachineBehaviour {

    public int stateNum;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetInteger("SubState", stateNum);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetInteger("SubState", -1);
    }
}