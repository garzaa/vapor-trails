using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : MonoBehaviour {

	public bool facingRight;
	public GameObject target;

	bool isEntity;

	void Start() {
		if (target == null) {
			target = GlobalController.pc.gameObject;
		}
		isEntity = GetComponent<Entity>() != null;
		if (isEntity) {
			facingRight = GetComponent<Entity>().facingRight;
		}
	}

	void Update() {
		if (facingRight) {
			if (target.transform.position.x < this.transform.position.x) {
				Flip();
			}
		} else {
			if (target.transform.position.x > this.transform.position.x) {
				Flip();
			}
		}
	}

	void Flip() {
		if (isEntity) {
			GetComponent<Entity>().Flip();
			facingRight = !facingRight;
		} else {
			this.transform.localScale = new Vector2(this.transform.localScale.x * -1, this.transform.localScale.y);
			facingRight = !facingRight;
		}
	}

}
