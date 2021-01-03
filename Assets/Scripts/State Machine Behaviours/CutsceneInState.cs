using UnityEngine;

public class CutsceneInState : StateMachineBehaviour {

    public bool stateMachine;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateMachine) return;
        GlobalController.pc.EnterCutscene();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (stateMachine) return;
        GlobalController.pc.ExitCutscene();
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        if (stateMachine) {
            GlobalController.pc.EnterCutscene();
        }
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
        if (stateMachine) {
            GlobalController.pc.ExitCutscene();
        }
    }

}
