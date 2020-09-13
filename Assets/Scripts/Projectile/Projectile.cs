using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public bool impactShake;
	public GameObject hitEffect;

	public LayerMask collisionLayers;
	public List<string> collisionTags;
	public bool spawnEffectOnGroundOnly;

	void OnTriggerEnter2D(Collider2D other) {
		if (
			(collisionTags.Count == 0 || !collisionTags.Contains(other.tag))
			|| (collisionLayers != (collisionLayers | (1 << other.gameObject.layer)))
		){
			return;
		}

		if (impactShake) CameraShaker.TinyShake();

		if (hitEffect != null && (!spawnEffectOnGroundOnly || other.gameObject.layer.Equals(LayerMask.NameToLayer(Layers.Ground)))) {
			GameObject g = Instantiate(hitEffect, this.transform.position, this.transform.rotation, this.transform);
			g.transform.parent = null;
		}

		Destroy(this.gameObject);

	}
}