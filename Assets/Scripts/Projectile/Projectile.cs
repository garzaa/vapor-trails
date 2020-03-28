using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public bool impactShake;
	public GameObject hitEffect;

	public LayerMask collisionLayers;
	public List<string> collisionTags;

	void OnTriggerEnter2D(Collider2D other) {
		if (
			(collisionTags.Count == 0 || !collisionTags.Contains(other.tag))
			|| (collisionLayers != (collisionLayers | (1 << other.gameObject.layer)))
		){
			return;
		}

		if (impactShake) CameraShaker.TinyShake();

		if (hitEffect != null) {
			// first, try to get an impact point
			RaycastHit2D hit = Physics2D.CircleCast(
				this.transform.position, 
				0.5f, 
				Vector2.up, 
				0, 
				collisionLayers);
			if (hit.transform != null) {
				Vector2 originalMotion = this.GetComponent<Rigidbody2D>().velocity;
				Vector2 flipped = Vector2.Reflect(originalMotion, hit.normal);
				float newAngle = Vector2.Angle(Vector2.left, flipped);
				Instantiate(hitEffect, hit.point, Quaternion.Euler(0, 0, newAngle), null);
			} else {
				Instantiate(hitEffect, this.transform.position, Quaternion.identity, null);
			}
		}

		Destroy(this.gameObject);

	}
}