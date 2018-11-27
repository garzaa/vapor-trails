using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustController : MonoBehaviour {

	public WallCheck groundCheck;
	public ParticleSystem dustParticles;

	public List<TrailRenderer> trails;

	public PlayerController pc;

	void FixedUpdate() {
		var emission = dustParticles.emission;
		if (pc.IsSpeeding() && groundCheck.TouchingWall()) {
			RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5, 1 << LayerMask.NameToLayer(Layers.Ground));
			if (hit.collider != null) {
				var pos = dustParticles.transform.position;
				pos.y = hit.point.y;
				dustParticles.transform.position = pos;
			}
			emission.enabled = true;
		} else {
			emission.enabled = false;
		}

		if (pc.IsSpeeding() && trails.Count > 0) {
			foreach (TrailRenderer t in trails) {
				t.emitting = true;
			}
		} else if (!pc.IsSpeeding() && trails.Count  > 0) {
			foreach (TrailRenderer t in trails) {
				t.emitting = false;
			}
		}
	}
}
