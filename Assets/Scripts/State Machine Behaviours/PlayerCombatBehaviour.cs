using UnityEngine;

public class PlayerCombatBehaviour : StateMachineBehaviour {
    PlayerController player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Enter(animator);
    } 

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        Enter(animator);
    }

    void Enter(Animator animator) {
        if (player == null) player = animator.GetComponent<PlayerController>();
        player.StartCombatStanceCooldown();
    }
}