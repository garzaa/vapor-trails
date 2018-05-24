using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekPlayerX : EnemyBehavior {

	public override void Move() {
		if (mainController.frozen || playerDistance > maxSeekThreshold || mainController.IsStunned()) {
			return;
		}
		//move towards the player
		//first, get where they are
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

		if (Mathf.Abs(mainController.rb2d.velocity.x) < mainController.maxSpeed) {
			mainController.rb2d.AddForce(new Vector2(mainController.moveForce * moveScale, mainController.rb2d.velocity.y));
		}
	}
}