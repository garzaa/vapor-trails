using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

	public Sprite brokenSprite;
	ParticleSystem particles;
	bool particlesOnBreak = false;
	bool broken = false;

	void Start() {
		if (GetComponentInChildren<ParticleSystem>() != null) {
			particles = GetComponentInChildren<ParticleSystem>();
			particlesOnBreak = true;
		}
		int xScale = Random.Range(-1, 1) > 0 ? 1 : -1;
		this.transform.localScale = new Vector3(
			xScale,
			this.transform.localScale.y,
			this.transform.localScale.z
		);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (!broken && other.CompareTag(Tags.PlayerHitbox)) {
			Break(other);
		}
	}

	void Break(Collider2D other) {
		broken = true;
		if (brokenSprite != null) {
			GetComponent<SpriteRenderer>().sprite = brokenSprite;
		}
		if (particlesOnBreak) {
			//flip the direction of the emitted particles depending on what side the attck is coming from
			int xScale = other.transform.position.x > this.transform.position.x ? -1 : 1;
			particles.transform.localScale = Vector3.Scale(particles.transform.localScale, new Vector3(xScale, 1, 1));
			particles.Emit(10);
		}

		PlayerAttack a = other.GetComponent<PlayerAttack>();
		if (a != null) {
			CameraShaker.Shake(a.cameraShakeIntensity, a.cameraShakeTime);
			//instantiate the hitmarker
			if (a.hitmarker != null) {
				Instantiate(a.hitmarker, this.transform.position, Quaternion.identity);
			}
		}
	}

}
