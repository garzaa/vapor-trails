using UnityEngine;

public class StartFightInState : StateMachineBehaviour {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.GetComponent<Boss>().StartFight();
    }

    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        animator.GetComponent<Boss>().StartFight();
    }
}