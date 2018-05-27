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

	public bool gainsEnergy = false;
	int energyGained = 1;

	public bool costsEnergy = false;
	int energyCost = 1;

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
			CameraShaker.TinyShake();
		}
		//instantiate the hitmarker
		if (this.hitmarker != null) {
			Instantiate(hitmarker, enemy.transform.position, Quaternion.identity);
		}
		//run self knockback
		if (selfKnockBack) {
			pc.GetComponent<Rigidbody2D>().velocity = selfKnockBackVector;
		}
		//run hitstop
		if (hitstopLength > 0.0f) {
			Hitstop.Run(this.hitstopLength, enemy, pc);
		}
		//give the player some energy
		if (gainsEnergy) {
			pc.GainEnergy(this.energyGained);
		}
		//deplete energy if necessary
		if (costsEnergy) {
			pc.LoseEnergy(this.energyCost);
		}
	}

	void OnTriggerEnter2D(Collider2D enemyCol) {
		if (enemyCol.gameObject.CompareTag(Tags.EnemyHurtbox)) {
			//call enemy on hit first to avoid race condition with hitstop
			//if it takes energy to inflict damage, don't run any of the hit code
			if (this.costsEnergy && this.energyCost > pc.CheckEnergy()) {
				return;
			}
			enemyCol.GetComponent<EnemyHurtbox>().OnHit(this);
			this.OnAttackLand(enemyCol.GetComponent<EnemyHurtbox>().GetParent());
		}
	}
}
