using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePlayerForDuration : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GlobalController.pc.EnterCutscene();
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GlobalController.pc.ExitCutscene();
	}
}
