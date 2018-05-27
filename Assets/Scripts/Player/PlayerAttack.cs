using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack {

	public float stunLength = 0.2f;
	public bool knockBack = true;
	public Vector2 knockbackVector = Vector2.zero;
	public bool flipHitmarker = false;

	public bool gainsEnergy = false;
	int energyGained = 1;

	public bool costsEnergy = false;
	int energyCost = 1;

	void Start() {
		attackerParent = GameObject.Find("Player").GetComponent<PlayerController>();
		attackedTags = new List<string>();
		attackedTags.Add(Tags.EnemyHurtbox);
	}

	public Vector2 GetKnockback() {
		return new Vector2(
			x:knockbackVector.x * attackerParent.GetForwardScalar(), 
			y:knockbackVector.y
		);
	}
	
	public float GetStunLength() {
		return this.stunLength;
	}

	public override void ExtendedAttackLand() {
		//run self knockback
		if (selfKnockBack) {
			attackerParent.GetComponent<Rigidbody2D>().velocity = selfKnockBackVector;
		}
		//give the player some energy
		if (gainsEnergy) {
			attackerParent.GetComponent<PlayerController>().GainEnergy(this.energyGained);
		}
		//deplete energy if necessary
		if (costsEnergy) {
			attackerParent.GetComponent<PlayerController>().LoseEnergy(this.energyCost);
		}
	}

	new void OnTriggerEnter2D(Collider2D otherCol) {
		print(otherCol.name);
		print("bens");
		if (attackedTags.Contains(otherCol.tag)) {
			//call enemy on hit first to avoid race condition with hitstop
			//if it takes energy to inflict damage, don't run any of the hit code
			if (this.costsEnergy && this.energyCost > attackerParent.GetComponent<PlayerController>().CheckEnergy()) {
				return;
			}
			otherCol.GetComponent<Hurtbox>().OnHit(this);
			this.OnAttackLand(otherCol.GetComponent<Hurtbox>().GetParent());
		}
	}
}
