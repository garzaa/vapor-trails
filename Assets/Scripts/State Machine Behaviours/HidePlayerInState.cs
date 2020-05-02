using UnityEngine;

public class HidePlayerInState : StateMachineBehaviour {

    public bool stateMachine;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GlobalController.HidePlayer();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GlobalController.pc.EnterCutscene();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GlobalController.ShowPlayer();
        if (GlobalController.dialogueOpen) {
            GlobalController.pc.EnterCutscene();
        }
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
            if (GlobalController.dialogueOpen) {
                GlobalController.pc.EnterCutscene();
            }
        }
    }

}