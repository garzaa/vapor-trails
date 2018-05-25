using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	public int damage = 1;
	public float hitstopLength = .02f;
	public float stunLength = 0.2f;
	public bool cameraShake = false;
	public bool knockBack = true;
	public Vector2 knockbackVector = Vector2.zero;
	public bool selfKnockBack = false;
	public Vector2 selfKnockBackVector = Vector2.zero;
	public GameObject hitmarker;

	public bool flipHitmarker = false;

	PlayerController pc;

	void Start() {
		pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}

	public int GetDamage() {
		return this.damage * pc.baseAttackDamage;
	}

	public float GetStunLength() {
		return stunLength;
	}

	public Vector2 GetKnockback() {
		return new Vector2(
			x:knockbackVector.x * pc.GetForwardScalar(), 
			y:knockbackVector.y
		);
	}

	void OnAttackLand(Enemy enemy) {
		//camera shake?
		if (cameraShake) {
			CameraShaker.SmallShake();
		}
		//instantiate the hitmarker
		Instantiate(hitmarker, enemy.transform.position, Quaternion.identity);
		//run self knockback
		if (selfKnockBack) {
			pc.GetComponent<Rigidbody2D>().velocity = selfKnockBackVector;
		}
		//run hitstop
		if (hitstopLength > 0.0f) {
			Hitstop.Run(this.hitstopLength, enemy, pc);
		}
	}

	void OnTriggerEnter2D(Collider2D enemyCol) {
		if (enemyCol.gameObject.CompareTag(Tags.EnemyHitbox)) {
			//call enemy on hit first to avoid race condition with hitstop
			enemyCol.GetComponent<EnemyHitbox>().OnHit(this);
			this.OnAttackLand(enemyCol.GetComponent<EnemyHitbox>().GetParent());
		}
	}
}
