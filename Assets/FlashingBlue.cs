using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingBlue : StateMachineBehaviour {
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponent<PlayerController>().FlashCyan();
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.GetComponent<PlayerController>().StopFlashingCyan();
	}
}
