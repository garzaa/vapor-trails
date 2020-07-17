using UnityEngine;

public class BlockBehaviour : StateMachineBehaviour {
    
    PlayerController player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        player = animator.GetComponent<PlayerController>();
        // parry window comes out two frames after the stance is initiated
        // two actual frames, not two animation frames (slightly before the white flash)
        player.Invoke("StartParryWindow", 2/60f);
        // call this here, otherwise it activates as long as the block button is held
        // "why does the block button need to be held" because it's a trigger and I don't want to code triggerDown logic
        player.currentEnergy--;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.ResetTrigger(Buttons.BLOCK);
    }

}