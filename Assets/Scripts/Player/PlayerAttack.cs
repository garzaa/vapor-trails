using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack {
	
	int energyGained = 1;
	int energyCost = 1;

	public bool gainsEnergy = false;
	public bool costsEnergy = false;
	public float hitstopLength = 0.2f;
	public bool rotateHitmarker = true;

	void Start() {
		attackerParent = GameObject.Find("Player").GetComponent<PlayerController>();
		attackedTags = new List<string>();
		attackedTags.Add(Tags.EnemyHurtbox);
		rb2d = attackerParent.GetComponent<Rigidbody2D>();
	}

	public override void ExtendedAttackLand(Entity e) {
		if (e == null) {
			return;
		}
		//run self knockback
		if (selfKnockBack) {
			attackerParent.GetComponent<Rigidbody2D>().velocity = new Vector2(
				forceX ? selfKnockBackVector.x * attackerParent.ForwardScalar() : attackerParent.GetComponent<Rigidbody2D>().velocity.x,
				selfKnockBackVector.y
			);
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
			//Hitstop.Run(this.hitstopLength);
			CameraShaker.TinyShake();
		}
	}

	override protected void OnTriggerEnter2D(Collider2D otherCol) {
		if (attackedTags.Contains(otherCol.tag)) {
			//if it takes energy to inflict damage, don't run any of the hit code
			if (costsEnergy && energyCost > attackerParent.GetComponent<PlayerController>().CheckEnergy()) {
				return;
			}
			if (otherCol.GetComponent<Hurtbox>() != null) {
				otherCol.GetComponent<Hurtbox>().OnHit(this);
				OnAttackLand(otherCol.GetComponent<Hurtbox>().GetParent());
			}
			EmitHitParticles(otherCol);
		}
	}

	override public void MakeHitmarker(Transform pos) {
		GameObject h = Instantiate(hitmarker, attackerParent.transform);
        h.transform.position = pos.position;
        if (rotateHitmarker) {
            h.transform.eulerAngles = new Vector3(
                0,
                0,
                Vector2.Angle(Vector2.right, knockbackVector)
            );
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
		attackerParent.GetComponent<PlayerController>().Parry(this);
	}

	override public int GetDamage() {
		return this.damage * attackerParent.GetComponent<PlayerController>().baseDamage;
	}
}
