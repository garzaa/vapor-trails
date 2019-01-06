using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviroDamage : Attack {

	public bool particlesOnImpact;
	public bool returnPlayerToSafety = false;
	public int numParticles;
	public Transform particleObject;
	ParticleSystem ps;

	void Start() {
		attackerParent = this.gameObject.AddComponent<Entity>();
		attackedTags = new List<string>();
		attackedTags.Add(Tags.EnemyHurtbox);
		attackedTags.Add(Tags.PlayerHurtbox);
		if (particlesOnImpact) {
			ps = GetComponentInChildren<ParticleSystem>();
		}
	}

	//the main thing for the env damage check is that it ignores invincibility and just checks for susceptibility to env damage
	//meteor & dash both make the player invincible, but only dash lets them bypass enviro damage
	public override bool ExtendedAttackCheck(Collider2D col) {
		if (col.GetComponent<Hurtbox>() == null) {
			return false;
		}
		Entity e = col.GetComponent<Hurtbox>().GetParent().GetComponent<Entity>();
		if (e == null) {
			return false;
		}
		if (e.envDmgSusceptible && !e.stunned) {
			return true;
		} else {
			return false;
		}
	}

	public override void ExtendedAttackLand(Entity e) {
		if (particlesOnImpact) {
			particleObject.transform.position = e.transform.position;
			ps.Emit(numParticles);
		}
	}
}
