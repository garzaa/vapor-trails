using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomTransition : StateMachineBehaviour {

    [Header("State Names")]
    public string[] transitions;

    [Header("Exit Triggers")]
    public string[] triggerNames;
    public float triggerSetTime;

    bool startedExit = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        startedExit = false;
        if (triggerNames.Length > 0) return;
        animator.Play(transitions[Random.Range(0, transitions.Length)]);
        if (triggerNames.Length > 0) {
            foreach (string t in triggerNames) {
                animator.ResetTrigger(t);
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateInfo.normalizedTime >= triggerSetTime && !startedExit) {
            startedExit = true;
            animator.SetTrigger(triggerNames[Random.Range(0, triggerNames.Length)]);
        }
    }
}