using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	public string attackName;
	public int damage = 1;
	[Range(0, 2f)]
	public float cameraShakeIntensity = .1f;
	[Range(0, 2f)]
	public float cameraShakeTime = 0.1f;
	public bool selfKnockBack = false;
	public Vector2 selfKnockBackVector = Vector2.zero;
	public bool forceX = false;
	public GameObject hitmarker;
	public bool flipHitmarker = false;
	public List<string> attackedTags;
	public Entity attackerParent;
	public float stunLength = 0.2f;
	public bool knockBack = true;
	public Vector2 knockbackVector = Vector2.zero;
	public bool inheritMomentum = false;
	public bool attackLandTrigger = false;

	protected Rigidbody2D rb2d;

	void Start() {
		if (attackerParent == null) {
			attackerParent = GetComponentInParent<Entity>();
		}
		rb2d = attackerParent.GetComponent<Rigidbody2D>();
	}

	public virtual int GetDamage() {
		return this.damage;
	}

	public void OnAttackLand(Entity victim) {
		ExtendedAttackLand(victim);
		if (attackLandTrigger) {
			attackerParent.GetComponent<Animator>().SetTrigger("AttackLand");
		}
	}

	public virtual void MakeHitmarker(Transform pos) {
		GameObject h = Instantiate(hitmarker, pos);
        h.transform.position = pos.position;
	}

	virtual protected void OnTriggerEnter2D(Collider2D otherCol) {
		if (!ExtendedAttackCheck(otherCol)) {
			return;
		}
		if (attackedTags.Contains(otherCol.gameObject.tag)) {
			if (otherCol.GetComponent<Hurtbox>() == null) {
				return;
			}
			otherCol.GetComponent<Hurtbox>().OnHit(this);
			this.OnAttackLand(otherCol.GetComponent<Hurtbox>().GetParent());
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

	public virtual bool ExtendedAttackCheck(Collider2D col) {
		if (col.GetComponent<Hurtbox>() == null) {
			return false;
		}
		return !col.GetComponent<Hurtbox>().GetParent().invincible;
	}

	public virtual void ExtendedAttackLand(Entity e) {

	}
}
