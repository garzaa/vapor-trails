using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeInState : StateMachineBehaviour {

	public bool onEnter = true;
	public bool during;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerController pc = animator.GetComponent<PlayerController>();
		if (!pc.inCutscene && onEnter) {
			animator.GetComponent<PlayerController>().Freeze();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerController pc = animator.GetComponent<PlayerController>();
		if (!pc.inCutscene && during) {
			animator.GetComponent<PlayerController>().Freeze();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerController pc = animator.GetComponent<PlayerController>();
		if (!pc.inCutscene) {
			animator.GetComponent<PlayerController>().UnFreeze();
		}
	}
}
