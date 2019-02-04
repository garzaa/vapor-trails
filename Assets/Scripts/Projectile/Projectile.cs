using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public bool impactShake;
	public GameObject burstPrefab;

	GameObject impactDust;

	public bool ignorePlayer = false;

	void Start() {
		this.transform.parent = null;
		impactDust = (GameObject) Resources.Load("ImpactDustPrefab");
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (ignorePlayer && other.tag.ToLower().Contains("player")) {
			return;
		}

		if (other.CompareTag(Tags.EnemyHitbox)) {
			return;
		}

		if (impactShake) {
			CameraShaker.TinyShake();
		}
		OnImpact(other);
	}

	public void OnImpact(Collider2D other) {
		print(LayerMask.LayerToName(other.gameObject.layer));
		print(other.gameObject.name);
		if (burstPrefab != null) {
			GameObject go = Instantiate(burstPrefab, transform.position, Quaternion.identity);
		}

		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, other.transform.position, 4, 1 << LayerMask.NameToLayer(Layers.Ground));
		if (hit.transform != null) {
			Vector2 originalMotion = this.GetComponent<Rigidbody2D>().velocity;
			Vector2 flipped = Vector2.Reflect(originalMotion, hit.normal);
			float newAngle = Vector2.Angle(Vector2.left, flipped);
			GameObject g = (GameObject) Instantiate(impactDust, hit.point, Quaternion.Euler(0, 0, newAngle+180), null);
		}

		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		SoundManager.ExplosionSound();
		//remove the collider/sprites/etc and stop particle emission
		GetComponent<Collider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<SelfDestruct>().Destroy(2f);
	}
}
