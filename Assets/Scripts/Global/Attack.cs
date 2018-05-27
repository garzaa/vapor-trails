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
	[HideInInspector] public Entity attackerParent;

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
		//run hitstop
		if (hitstopLength > 0.0f) {
			Hitstop.Run(this.hitstopLength, e, attackerParent);
		}
		ExtendedAttackLand();
	}

	public void OnTriggerEnter2D(Collider2D otherCol) {
		if (attackedTags.Contains(otherCol.gameObject.tag)) {
			otherCol.GetComponent<Hurtbox>().OnHit(this);
			this.OnAttackLand(otherCol.GetComponent<Hurtbox>().GetParent());
		}
	}

	//called on the entity that the attack lands on
	public virtual void ExtendedAttackLand() {

	}
}
