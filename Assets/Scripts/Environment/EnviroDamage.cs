using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviroDamage : Attack {

	public bool particlesOnImpact;
	public bool returnPlayerToSafety = true;
	public int numParticles;
	public Transform particleObject;
	public AudioSource impactSound;
	// this was added later
	public AudioClip impactSoundClip;
	ParticleSystem ps;
	Renderer thisRenderer;

	Collider2D thisCollider;

	void Start() {
		attackerParent = this.gameObject.AddComponent<Entity>();
		attackedTags = new List<string>();
		attackedTags.Add(Tags.EnemyHurtbox);
		attackedTags.Add(Tags.PlayerHurtbox);
		if (particlesOnImpact) {
			ps = GetComponentInChildren<ParticleSystem>();
		}
		thisRenderer = GetComponentInChildren<Renderer>();
		thisCollider = GetComponent<Collider2D>();
	}

	//the main thing for the env damage check is that it ignores invincibility and just checks for susceptibility to env damage
	public override bool ExtendedAttackCheck(Collider2D col) {
		if (col.GetComponent<Hurtbox>() == null) {
			return false;
		}
		Entity e = col.GetComponent<Hurtbox>().GetParent().GetComponent<Entity>();
	if (e == null) {
			return false;
		}
		if (e.envDmgSusceptible) {
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

		if (impactSound) {
			impactSound.PlayOneShot(impactSound.clip);
		}

		if (impactSoundClip) {
			SoundManager.PlaySound(impactSoundClip);
		}

		thisCollider.enabled = false;
		// keep the player from walking through spikes
		Invoke("ReenableCollider", 0.2f);
	}

	void ReenableCollider() {
		thisCollider.enabled = true;
	}
}
