using UnityEngine;

public class AttackSequence : StateMachineBehaviour {
	CombatAI combatAI;

	public override void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
		animator.GetComponent<CombatAI>().OnAttackSequenceFinish();
	}
}
