using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack {
	
	int energyGained = 1;
	int energyCost = 1;

	public bool gainsEnergy = false;
	public bool costsEnergy = false;
	public float hitstopLength = 0.2f;
	public bool rotateHitmarker = true;
	public bool pullInEntity = true;

	public bool attackLandEvent = false;

	PlayerController player;
	BoxCollider2D bc2d;

	void Start() {
		player = GameObject.Find("Player").GetComponent<PlayerController>();
		attackerParent = player;
		attackedTags = new List<string>();
		attackedTags.Add(Tags.EnemyHurtbox);
		rb2d = attackerParent.GetComponent<Rigidbody2D>();
		bc2d = GetComponent<BoxCollider2D>();
	}

	public override void ExtendedAttackLand(Entity e) {
		if (e == null) {
			return;
		}

		// the succ
		if (pullInEntity && e.staggerable) {
			e.transform.position = this.transform.position;
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
			player.GainEnergy(this.energyGained);
		}
		//deplete energy if necessary
		if (costsEnergy) {
			player.LoseEnergy(this.energyCost);
		}

		SoundManager.HitSound();

		if (attackLandEvent) {
			player.OnAttackLand(this);
		}
		
		//run hitstop if it's a player attack
		if (hitstopLength > 0.0f && this.gameObject.CompareTag(Tags.PlayerHitbox)) {
			Hitstop.Run(this.hitstopLength);
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
		}
	}

	override public void MakeHitmarker(Transform pos) {
		Vector2 midpoint = Vector2.MoveTowards(this.transform.position, pos.position, Vector2.Distance(this.transform.position, pos.position)/2f);
		GameObject h = Instantiate(
			hitmarker,
			this.transform.position,
			Quaternion.identity,
			this.transform
		);
		// hitmarker is currently facing the correct direction (left, internally)
		// so, rotate to match the angle between its initial rotation and the knockback vector
		float angleDiff = Vector2.Angle(Vector2.left, knockbackVector * attackerParent.ForwardVector());
		// and then throw all logic out the window because this is what makes it work
		// for negative x knockback
		if (angleDiff == 180) {
			angleDiff -= 180;
		}
		angleDiff = (angleDiff == 0 ? angleDiff : angleDiff-90f);
		h.transform.eulerAngles = new Vector3(
			0,
			0,
			angleDiff
		);
		h.transform.parent = null;
	}

	public void OnDeflect() {
		player.GainEnergy(1);
		player.Parry();
	}

	override public int GetDamage() {
		return this.damage * attackerParent.GetComponent<PlayerController>().baseDamage;
	}
}
