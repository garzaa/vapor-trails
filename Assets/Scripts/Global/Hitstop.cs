using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitstop : MonoBehaviour{

	//statically calling instances
	public static Hitstop instance;

	void Awake() {
		instance = this;
	}

	public static void Run(float seconds, Enemy enemy, PlayerController pc) {
		instance.StartCoroutine(DoHitstop(seconds, enemy, pc));
	}

	static IEnumerator DoHitstop(float seconds, Enemy enemy, PlayerController pc) {
		//pause animations for both entities
		bool frozePlayer = false;
		
		//store the last velocities
		Rigidbody2D rb2d = enemy.GetComponent<Rigidbody2D>();
		Vector2 lastPlayerV = pc.GetComponent<Rigidbody2D>().velocity;

		Animator enemyAnim = enemy.GetComponent<Animator>();
		Vector2 lastEnemyV = enemy.rb2d.velocity;

		//freeze the positions, don't want to repeatedly freeze the player if they're hitting multiple enemies
		enemy.inHitstop = true;
		if (!pc.inHitstop) {
			pc.LockInSpace();
			frozePlayer = true;
			pc.inHitstop = true;
		}

		//freeze the animations
		if (enemyAnim != null) {
			enemyAnim.speed = 0;
		}
		pc.GetComponent<Animator>().speed = 0;

		Vector2 lastV = Vector2.zero;
		if (rb2d != null) {
			lastV = rb2d.velocity;
		}

		//don't want to unfreeze the enemy afterwards if they're already frozen for some reason
		//so if they're not already frozen, then freeze them and store that info
		//also unfreeze them if they're in hitstop?
		bool frozenEnemy = false;
		if (!enemy.lockedInSpace) {
			enemy.LockInSpace();
			frozenEnemy = true;
		}
		yield return new WaitForSeconds(seconds);

		//then undo everything
		if (enemyAnim != null) {
			enemyAnim.speed = 1;
		}
		pc.GetComponent<Animator>().speed = 1;

		//the enemy might have died
		if (enemy != null) {
			//also then unfreeze them if they were frozen from hitstop
			if (frozenEnemy) {
				enemy.UnLockInSpace();
			}
			print("giving enemy velocity " + lastEnemyV);
			enemy.rb2d.velocity = lastEnemyV;
		
			if (rb2d != null) rb2d.velocity = lastV;
			enemy.inHitstop = false;
		}

		if (frozePlayer) {
			pc.GetComponent<Rigidbody2D>().velocity = lastPlayerV;
			pc.inHitstop = false;
			pc.UnLockInSpace();
		}
	}
}