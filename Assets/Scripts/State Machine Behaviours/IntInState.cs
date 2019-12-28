using UnityEngine;

public class IntInState : StateMachineBehaviour {

    public int stateNum;
    public bool isStateMachine;

    int lastEntryState = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (isStateMachine) return;
        lastEntryState = animator.GetInteger("SubState");
        animator.SetInteger("SubState", stateNum);
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        if (!isStateMachine) return;
        animator.SetInteger("SubState", stateNum);
        lastEntryState = animator.GetInteger("SubState");
    }


    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        if (isStateMachine) animator.SetInteger("SubState", lastEntryState);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!isStateMachine) animator.SetInteger("SubState", lastEntryState);
    }
}