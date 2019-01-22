using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : MonoBehaviour {

	public bool facingRight;
	public GameObject target;

	bool isEntity;
	Entity e;

	void Start() {
		if (target == null) {
			target = GlobalController.pc.gameObject;
		}
		e = GetComponent<Entity>();
		isEntity = e != null;
		if (isEntity) {
			facingRight = e.facingRight;
		}
	}

	void Update() {
		if (isEntity) {	
			facingRight = e.facingRight;
		}
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
			e.Flip();
		} else {
			this.transform.localScale = new Vector2(this.transform.localScale.x * -1, this.transform.localScale.y);
			facingRight = !facingRight;
		}
	}

}
