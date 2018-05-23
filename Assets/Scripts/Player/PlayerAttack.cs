using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	public int damage = 1;
	public float hitstop = .04f;
	public bool cameraShake = false;
	public Vector2 knockbackVector = Vector2.zero;
	public GameObject hitmarker;

	public bool flipHitmarker = false;

	PlayerController pc;

	public Vector2 selfKnockback;

	void Start() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}

	public int GetDamage() {
		return this.damage * pc.baseAttackDamage;
	}

	void OnAttackLand() {
		//camera shake?
		//instantiate the hitmarker
		//start knockback
		//run hitstop
	}

	void OnTriggerEnter2D(Collider2D enemyCol) {
		if (enemyCol.gameObject.CompareTag("EnemyHitbox")) {
			OnAttackLand();
		}
	}
}
