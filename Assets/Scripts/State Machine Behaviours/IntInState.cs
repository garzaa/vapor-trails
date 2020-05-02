using UnityEngine;

public class IntInState : StateMachineBehaviour {

    public int stateNum;
    public bool isStateMachine;
    public bool onUpdate;

    int lastEntryState = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (isStateMachine) return;
        lastEntryState = animator.GetInteger("SubState");
        animator.SetInteger("SubState", stateNum);
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        if (!isStateMachine) return;
        lastEntryState = animator.GetInteger("SubState");
        animator.SetInteger("SubState", stateNum);
    }


    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        // if the state was overriden in the substate machine
       //if (isStateMachine && animator.GetInteger("SubState") == stateNum) animator.SetInteger("SubState", 0);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //if (!isStateMachine) animator.SetInteger("SubState", lastEntryState); 
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (onUpdate) animator.SetInteger("SubState", stateNum);    
    }
}