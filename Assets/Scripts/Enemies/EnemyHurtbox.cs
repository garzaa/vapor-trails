using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtbox : MonoBehaviour {

	GameObject parentObject;

	void Start() {
		parentObject = this.gameObject.transform.parent.gameObject;
	}

	public Enemy GetParent() {
		return parentObject.GetComponent<Enemy>();
	}

	public void OnHit(PlayerAttack a) {
		parentObject.GetComponent<Enemy>().OnHit(a);
	}

}