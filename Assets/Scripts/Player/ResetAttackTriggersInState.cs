using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetAttackTriggersInState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponent<PlayerController>().ResetAttackTriggers();
	}
}
