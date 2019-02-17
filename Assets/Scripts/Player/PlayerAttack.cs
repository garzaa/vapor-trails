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
			this.EmitHitParticles(otherCol);
		}
	} 

	void EmitHitParticles(Collider2D otherCol) {
		// get angle to target and average distance
		Vector2 halfwayPoint = this.transform.position + ((otherCol.transform.position - this.transform.position)/2);
		var dir = otherCol.transform.position - this.transform.position;
		var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		//rotate hit particles
		GameObject hitparticles = attackerParent.GetComponent<PlayerController>().impactParticles;
		hitparticles.transform.position = otherCol.transform.position;
		hitparticles.transform.rotation = Quaternion.AngleAxis(angle+90, Vector3.forward);

		//emit 2 from each
		foreach (ParticleSystem ps in hitparticles.GetComponentsInChildren<ParticleSystem>()) {
			ps.Emit(1);
		}
	}

	public void OnDeflect() {
		attackerParent.GetComponent<PlayerController>().GainEnergy(1);
		attackerParent.GetComponent<PlayerController>().Parry();
	}
}
