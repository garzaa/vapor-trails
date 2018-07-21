using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAtPlayer : EnemyBehavior {

	public float jumpCooldown = 2f;
	public float scaleX = 3f;
	public float scaleY = 5f;

	public bool grounded = false;

	IEnumerator JumpTimeout() {
		yield return new WaitForSeconds(jumpCooldown);
		if (!(mainController.frozen || playerDistance > maxSeekThreshold)) {
			anim.SetTrigger("Squash");
		} else {
			StartCoroutine(JumpTimeout());
		}
	}

	public void Jump() {
		//move towards the player
		//first, get where they are
		anim.SetTrigger("Jump");

		if (Mathf.Abs(playerObject.transform.position.x - this.transform.position.x) < minSeekThreshold) {
			return;
		}
		int moveScale;
		if (playerObject.transform.position.x > this.transform.position.x) {
			moveScale = 1;
			mainController.movingRight = true;
		} else {
			moveScale = -1;
			mainController.movingRight = false;
		}
		mainController.rb2d.velocity = new Vector2(scaleX * moveScale, scaleY);
	}

	public override void OnGroundHit() {
		anim.SetBool("Grounded", true);
		grounded = true;
		StartCoroutine(JumpTimeout());
	}

	public override void OnGroundLeave() {
		anim.SetBool("Grounded", false);
		grounded = false;
	}

}
