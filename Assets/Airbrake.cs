using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airbrake : StateMachineBehaviour {

	Rigidbody2D rb2d;

	public float velocityClamp;

	public float velocityReduction;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		rb2d = animator.GetComponentInParent<Rigidbody2D>();
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (rb2d.velocity.y < velocityClamp && velocityReduction == 0) {
			rb2d.velocity = new Vector2(
				rb2d.velocity.x,
				velocityClamp
			);
		} else if (Mathf.Abs(rb2d.velocity.x) != 0) {
			rb2d.velocity = new Vector2(
				rb2d.velocity.x - (Mathf.Sign(rb2d.velocity.x) * velocityReduction),
				0
			);
		} else {
			rb2d.velocity = new Vector2(
				rb2d.velocity.x,
				0
			);
		}
	}
}
