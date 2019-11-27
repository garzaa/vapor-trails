using UnityEngine;

public class BlockBehaviour : StateMachineBehaviour {
    
    PlayerController player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = animator.GetComponent<PlayerController>();
        player.StartParryWindow();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.ResetTrigger(Buttons.BLOCK);
    }

}