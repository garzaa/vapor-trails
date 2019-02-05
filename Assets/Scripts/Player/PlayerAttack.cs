using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack {

	public bool gainsEnergy = false;
	int energyGained = 1;

	public bool costsEnergy = false;
	int energyCost = 1;

	public float hitstopLength = 0.2f;

	void Start() {
		attackerParent = GameObject.Find("Player").GetComponent<PlayerController>();
		attackedTags = new List<string>();
		attackedTags.Add(Tags.EnemyHurtbox);
	}

	public override void ExtendedAttackLand(Entity e) {
		if (e == null) {
			return;
		}
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

		SoundManager.HitSound();
		
		//run hitstop if it's a player attack
		if (hitstopLength > 0.0f && this.gameObject.CompareTag(Tags.PlayerHitbox)) {
			Hitstop.Run(this.hitstopLength);
		}
	}

	new void OnTriggerEnter2D(Collider2D otherCol) {
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

	public void OnDeflect() {
		attackerParent.GetComponent<PlayerController>().GainEnergy(1);
		//spawn an effect, maybe go into a random pose
	}
}
