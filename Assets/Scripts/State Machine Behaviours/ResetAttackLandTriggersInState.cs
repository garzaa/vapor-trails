using UnityEngine;

public class ResetAttackLandTriggersInState : StateMachineBehaviour {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.ResetTrigger("AttackLand");
	}
}