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
			anim.SetTrigger("squash");
		} else {
			StartCoroutine(JumpTimeout());
		}
	}

	public void Jump() {
		//move towards the player
		//first, get where they are
		anim.SetTrigger("jump");

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

	void OnCollisionEnter2D(Collision2D c) {
		if (c.gameObject.tag.Contains("platform") && rb2d.velocity.y == 0) {
			if (!grounded) { 
				anim.SetTrigger("land");
				grounded = true;
				StartCoroutine(JumpTimeout());
			} 
		}
	}

	public void OnGroundHit() {
		anim.SetBool("Grounded", true);
		grounded = true;
		StartCoroutine(JumpTimeout());
	}

	public void OnGroundLeave() {
		anim.SetBool("Grounded", false);
	}

}
