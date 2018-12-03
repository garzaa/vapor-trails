using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePlayerForDuration : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponentInParent<PlayerController>().Freeze();
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponentInParent<PlayerController>().UnFreeze();
	}
}
