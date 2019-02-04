using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeOnExit : StateMachineBehaviour {

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		PlayerController pc = animator.GetComponent<PlayerController>();
		if (!pc.inCutscene) {
			animator.GetComponent<PlayerController>().UnFreeze();
		}
	}
}
