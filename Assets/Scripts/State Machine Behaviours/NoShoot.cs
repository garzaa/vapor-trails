using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoShoot : StateMachineBehaviour {

    PlayerController pc;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		pc = animator.GetComponent<PlayerController>();
		pc.DisableShooting();
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		pc.EnableShooting();
	}

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
        pc = animator.GetComponent<PlayerController>();
		pc.DisableShooting();
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash) {
		pc.EnableShooting();
    }
}