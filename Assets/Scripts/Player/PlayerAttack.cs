using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

	public int damage = 1;
	public float hitstopLength = .04f;
	public float stunLength = 0.2f;
	public bool cameraShake = false;
	public Vector2 knockbackVector = Vector2.zero;
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
		//run hitstop
		Hitstop.Run(this.hitstopLength, enemy, pc);
	}

	void OnTriggerEnter2D(Collider2D enemyCol) {
		if (enemyCol.gameObject.CompareTag(Tags.EnemyHitbox)) {
			OnAttackLand(enemyCol.GetComponent<EnemyHitbox>().GetParent());
		}
	}
}
