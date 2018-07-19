using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeferredAttack : MonoBehaviour {

	//say the player dashes into spikes and their dash ends amid the spikes.
	//no OnHit() is called since it's only on collision entry.
	//this script fixes that.
	PlayerController pc;
	bool invincibleLastFrame = false;
	List<GameObject> deferredAttacks;

	void FixedUpdate() {
		if (pc.invincible && !invincibleLastFrame) {
			if (deferredAttacks.Count > 0) TakeDamage();
		}
		invincibleLastFrame = pc.invincible;
	}

	void Start() {
		pc = GetComponentInParent<PlayerController>();
		deferredAttacks = new List<GameObject>();
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (pc.invincible && col.GetComponent<Attack>() != null) {
			deferredAttacks.Add(col.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (deferredAttacks.Contains(col.gameObject)) {
			deferredAttacks.Remove(col.gameObject);
		}
	}

	void TakeDamage() {
		if (deferredAttacks.Count > 0) {
			GameObject g = deferredAttacks[0];
			if (g != null) {
				pc.OnHit(g.GetComponent<Attack>());
			}
			deferredAttacks.Clear();
		}
	}
}
