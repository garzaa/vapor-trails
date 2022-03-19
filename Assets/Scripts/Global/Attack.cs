using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	public string attackName;
	public int damage = 1;
	public AudioResource attackLandSound;
	[Range(0, 2f)]
	public float cameraShakeIntensity = .1f;
	[Range(0, 2f)]
	public float cameraShakeTime = 0.1f;
	public bool selfKnockBack = false;
	public Vector2 selfKnockBackVector = Vector2.zero;
	public bool forceX = false;
	public GameObject hitmarker;
	public bool flipHitmarker = false;
	[HideInInspector]
	public List<string> attackedTags;
	public Entity attackerParent;
	public float stunLength = 0.2f;
	public bool knockBack = true;
	public Vector2 knockbackVector = Vector2.zero;
	public bool knockbackAway = false;
	public bool inheritMomentum = false;
	public bool instakill = false;
	HashSet<Entity> entitiesHitThisActive = new HashSet<Entity>();

	protected Rigidbody2D rb2d;

	void Start() {
		if (attackerParent == null) {
			attackerParent = GetComponentInParent<Entity>();
		}
		rb2d = attackerParent.GetComponent<Rigidbody2D>();
	}

	void OnDisable() {
		entitiesHitThisActive.Clear();
	}

	public virtual int GetDamage() {
		return this.damage;
	}

	public void OnAttackLand(Entity victim, Hurtbox hurtbox) {
		entitiesHitThisActive.Add(victim);

		ExtendedAttackLand(victim);

		if (attackLandSound && !hurtbox.overrideHitSound) {
			SoundManager.PlayIfClose(attackLandSound, victim.gameObject);
		}

		Animator a;
		if ((a = attackerParent.GetComponent<Animator>()) != null) {
			a.SetTrigger("AttackLand");
		}
	}

	public virtual void MakeHitmarker(Transform pos) {
		GameObject h = Instantiate(hitmarker, pos);
        h.transform.position = pos.position;
	}

	virtual protected void OnTriggerEnter2D(Collider2D otherCol) {
		Hurtbox hurtbox = otherCol.GetComponent<Hurtbox>();
		if (!hurtbox || !ExtendedAttackCheck(hurtbox)) {	
			return;
		}

		Entity entity = hurtbox.GetParent();

		if (entitiesHitThisActive.Contains(entity)) {
			return;
		}

		if (attackedTags.Contains(hurtbox.gameObject.tag)) {
			if (hurtbox.OnHit(this)) {
				OnAttackLand(entity, hurtbox);
			}
		}
	}

	public virtual Vector2 GetKnockback() {
		Vector2 baseKnockback = new Vector2(
			x:knockbackVector.x * attackerParent.ForwardScalar(),
			y:knockbackVector.y
		);
		if (inheritMomentum) {
			baseKnockback += rb2d.velocity;
		}
		return baseKnockback;
	}
	
	public float GetStunLength() {
		return this.stunLength;
	}

	public virtual bool ExtendedAttackCheck(Hurtbox hurtbox) {
		if (hurtbox == null) {
			return false;
		}
		return !hurtbox.GetParent().invincible;
	}

	public virtual void ExtendedAttackLand(Entity e) {

	}
}
