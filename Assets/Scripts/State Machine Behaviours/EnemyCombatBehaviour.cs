using UnityEngine;

public class EnemyCombatBehaviour : StateMachineBehaviour {

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.ResetTrigger("AttackLand");
    } 

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.ResetTrigger("AttackLand");
    }

}