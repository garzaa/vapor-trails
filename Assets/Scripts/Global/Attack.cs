using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	public int damage = 1;
	public float hitstopLength = .02f;
	public bool cameraShake = false;
	public bool selfKnockBack = false;
	public Vector2 selfKnockBackVector = Vector2.zero;
	public GameObject hitmarker;
	public List<string> attackedTags;
	public Entity attackerParent;
	public float stunLength = 0.2f;
	public bool knockBack = true;
	public Vector2 knockbackVector = Vector2.zero;

	public int GetDamage() {
		return this.damage;
	}

	public void OnAttackLand(Entity e) {
		if (cameraShake) {
			CameraShaker.TinyShake();
		}
		//instantiate the hitmarker
		if (this.hitmarker != null) {
			Instantiate(hitmarker, e.transform.position, Quaternion.identity);
		}
		ExtendedAttackLand(e);
		//run hitstop
		if (hitstopLength > 0.0f) {
			Hitstop.Run(this.hitstopLength, e, attackerParent);
		}
	}

	public void OnTriggerEnter2D(Collider2D otherCol) {
		if (!ExtendedAttackCheck(otherCol)) {
			return;
		}
		if (attackedTags.Contains(otherCol.gameObject.tag)) {
			if (otherCol.GetComponent<Hurtbox>() == null) {
				return;
			}
			if (otherCol.GetComponent<Hurtbox>().GetParent().invincible && !this.CompareTag(Tags.EnviroDamage)) {
				return;
			}
			otherCol.GetComponent<Hurtbox>().OnHit(this);
			this.OnAttackLand(otherCol.GetComponent<Hurtbox>().GetParent());
		}
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

	public virtual bool ExtendedAttackCheck(Collider2D col) {
		return true;
	}

	//called on the entity that the attack lands on
	public virtual void ExtendedAttackLand(Entity e) {

	}
}
