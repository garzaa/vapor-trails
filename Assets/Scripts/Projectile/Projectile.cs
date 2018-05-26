using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	Rigidbody2D rb2d;

	public bool impactShake;
	public Transform burstPrefab;

	public bool ignorePlayer = false;

	void Start() {
		rb2d = GetComponent<Rigidbody2D>();
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
		Destroy(this.gameObject);
	}
}
