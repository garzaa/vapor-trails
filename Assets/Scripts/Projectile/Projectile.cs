using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public bool impactShake;
	public Transform burstPrefab;

	public bool ignorePlayer = false;

	void Start() {
		this.transform.parent = null;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (ignorePlayer && other.tag.ToLower().Contains("player")) {
			return;
		}

		if (impactShake) {
			CameraShaker.TinyShake();
		}
		OnImpact();
	}

	public void OnImpact() {
		if (burstPrefab != null) {
			Instantiate(burstPrefab, transform.position, Quaternion.identity);
		}
		//remove the collider/sprites/etc and stop particle emission
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		var e = GetComponent<ParticleSystem>().emission;
		e.rateOverDistance = 0;
		e.rateOverTime = 0;
		GetComponent<SelfDestruct>().Destroy(2f);
	}
}
