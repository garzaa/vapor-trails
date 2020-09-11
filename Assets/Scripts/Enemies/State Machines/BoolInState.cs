using UnityEngine;

// use sparingly, this will be ludicrous to debug
public class BoolInState : StateMachineBehaviour {
    public string boolName;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(boolName, true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool(boolName, false);
    }
}