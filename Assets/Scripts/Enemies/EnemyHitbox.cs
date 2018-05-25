using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour {

	GameObject parentObject;

	void Start() {
		parentObject = this.gameObject.transform.parent.gameObject;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag(Tags.PlayerHurtbox)) {
			//parentObject.GetComponent<Enemy>().OnHit(other.gameObject.GetComponent<PlayerAttack>());
		}
	}

	public Enemy GetParent() {
		return parentObject.GetComponent<Enemy>();
	}

	public void OnHit(PlayerAttack a) {
		parentObject.GetComponent<Enemy>().OnHit(a);
	}

}