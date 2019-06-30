using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

	public int damage = 1;
	[Range(0, 2f)]
	public float cameraShakeIntensity = .1f;
	[Range(0, 2f)]
	public float cameraShakeTime = 0.1f;
	public bool selfKnockBack = false;
	public Vector2 selfKnockBackVector = Vector2.zero;
	public GameObject hitmarker;
	public bool flipHitmarker = false;
	public List<string> attackedTags;
	public Entity attackerParent;
	public float stunLength = 0.2f;
	public bool knockBack = true;
	public Vector2 knockbackVector = Vector2.zero;
	public bool attackLandTrigger = false;

	public virtual int GetDamage() {
		return this.damage;
	}

	public void OnAttackLand(Entity victim) {
		if (cameraShakeTime>0f && victim.GetComponent<PlayerController>() != null) {
			CameraShaker.Shake(cameraShakeIntensity, cameraShakeTime);
		}
		if (attackLandTrigger) {
			attackerParent.GetComponent<Animator>().SetTrigger("AttackLand");
		}
		ExtendedAttackLand(victim);
	}

	public virtual void MakeHitmarker(Transform pos) {
		GameObject h = Instantiate(hitmarker, pos);
        h.transform.position = pos.position;
	}

	public void OnTriggerEnter2D(Collider2D otherCol) {
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
		return new Vector2(
			x:knockbackVector.x * attackerParent.ForwardScalar(), 
			y:knockbackVector.y
		);
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
