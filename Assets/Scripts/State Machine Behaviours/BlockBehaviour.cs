using UnityEngine;

public class BlockBehaviour : StateMachineBehaviour {
    
    PlayerController player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = animator.GetComponent<PlayerController>();
        // parry window comes out two frames after the stance is initiated
        // two actual frames, not two animation frames (slightly before the white flash)
        player.Invoke("StartParryWindow", 2/60f);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.ResetTrigger(Buttons.BLOCK);
    }

}