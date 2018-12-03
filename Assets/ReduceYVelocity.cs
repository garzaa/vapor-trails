using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceYVelocity : StateMachineBehaviour {

	Rigidbody2D rb2d;

	public float velocityClamp;

	public float velocityReduction;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		rb2d = animator.GetComponentInParent<Rigidbody2D>();
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (rb2d.velocity.y < velocityClamp && velocityReduction == 0) {
			rb2d.velocity = new Vector2(
				rb2d.velocity.x,
				velocityClamp
			);
		} else {
			rb2d.velocity = new Vector2(
				rb2d.velocity.x * velocityReduction,
				0
			);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
