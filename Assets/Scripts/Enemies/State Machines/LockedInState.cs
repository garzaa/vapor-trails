using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedInState : StateMachineBehaviour {

	Entity e;
	Rigidbody2D rb2d;

	public bool overrideInState;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		e = animator.GetComponent<Entity>();
		rb2d = animator.GetComponent<Rigidbody2D>();
		if (e) {
			e.LockInSpace();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (overrideInState) {
			e.LockInSpace();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (e) {
			e.UnLockInSpace();
		}
	}
}
