using UnityEngine;

public class IntInState : StateMachineBehaviour {

    public int stateNum;
    public bool isStateMachine;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (isStateMachine) return;
        animator.SetInteger("SubState", stateNum);
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        if (isStateMachine) animator.SetInteger("SubState", stateNum);
    }

}