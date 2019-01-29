using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeOnExit : StateMachineBehaviour {

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponent<PlayerController>().UnFreeze();
	}
}
