using UnityEngine;

public class PlayerCombatBehaviour : StateMachineBehaviour {
    PlayerController player;
    public PlayerAttackGraph attackGraph;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Enter(animator);
    } 

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.ResetTrigger("AttackLand");
    }

    void Enter(Animator animator) {
        if (player == null) player = animator.GetComponent<PlayerController>();
        player.StartCombatStanceCooldown();
        if (attackGraph != null) {
            player.EnterAttackGraph(attackGraph);
        }
    }
}