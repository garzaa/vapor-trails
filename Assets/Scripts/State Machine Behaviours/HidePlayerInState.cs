using UnityEngine;

public class HidePlayerInState : StateMachineBehaviour {

    public bool stateMachine;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateMachine) return;
        GlobalController.HidePlayer();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateMachine) return;
        GlobalController.ShowPlayer();
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        if (stateMachine) {
            GlobalController.HidePlayer();
        }
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        if (stateMachine) {
            Debug.Log("showing player");
            GlobalController.ShowPlayer();
        }
    }

}