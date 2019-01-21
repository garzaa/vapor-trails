using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInState : StateMachineBehaviour {

	public Vector2 direction;
	public bool forceZero;

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Rigidbody2D rb2d = animator.GetComponent<Rigidbody2D>();
		Entity e = animator.GetComponent<Entity>();
		Vector2 newDirection = direction;
		// if tthe x or y component of velocity is zero, do you clamp it at zero or leave it alone
		if (!forceZero) {
			newDirection = new Vector2(
				direction.x != 0 ? direction.x : rb2d.velocity.x,
				direction.y != 0 ? direction.y : rb2d.velocity.y
			);
		}

		rb2d.velocity = newDirection * e.ForwardVector();
	}
}
