using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomTransition : StateMachineBehaviour {

    public string[] transitions;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.Play(transitions[Random.Range(0, transitions.Length)]);
    }
}