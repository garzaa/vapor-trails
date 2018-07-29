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
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		SoundManager.ExplosionSound();
		//remove the collider/sprites/etc and stop particle emission
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<SelfDestruct>().Destroy(2f);
	}
}
