using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomTransition : StateMachineBehaviour {

    public string[] transitions;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetTrigger(transitions[Random.Range(0, transitions.Length)]);
    }
}