using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {
	
	public Transform bullet;
	public float startSpeed;
	public TargetingSystem targetingSystem;

	public Transform extraBulletPos;

	public void Fire(float forwardScalar = 1, Transform bulletPos = null) {
		Transform target = null;

		if (this.targetingSystem != null) {
			target = targetingSystem.GetClosestTarget(this.transform);
		}

		if (bulletPos == null) {
			if (extraBulletPos == null) {
				extraBulletPos = this.transform;
			} else {
				bulletPos = extraBulletPos.transform;
			}
		}

		Vector3 startPos = this.transform.position;
		if (bulletPos != null) {
			startPos = bulletPos.position;
		}
		GameObject b = Instantiate(bullet, startPos, Quaternion.identity, null).gameObject;

		//flip according to player scale
		b.transform.localScale = Vector3.Scale(b.transform.localScale, new Vector3(forwardScalar * -1, 1, 1));
		if (target != null) {
			//aim it and send it off
			var dir = target.transform.position - this.transform.position;
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			b.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
			b.GetComponent<Rigidbody2D>().velocity = b.transform.right.normalized * startSpeed;
		} else {
			b.GetComponent<Rigidbody2D>().velocity = new Vector2(
				startSpeed * forwardScalar,
				0
			);
		}
	}
}
