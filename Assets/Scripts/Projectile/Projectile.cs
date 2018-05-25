using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	Rigidbody2D rb2d;
	GameObject player;

	public bool impactShake;
	public float shakeDistance;
	public Transform burstPrefab;

	void Start() {
		rb2d = GetComponent<Rigidbody2D>();
		player = GameObject.Find("Player");
		this.transform.parent = null;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(Tags.Player)) {
			return;
		}
		if (impactShake && Vector2.Distance(transform.position, player.transform.position) < shakeDistance) {
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
