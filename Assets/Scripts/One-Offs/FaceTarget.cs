using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : MonoBehaviour {

	public bool facingRight;
	public GameObject target;
	public float threshold = 0.2f;

	bool isEntity;
	Entity e;

	void Start() {
		if (target == null) {
			target = GlobalController.pc.gameObject;
		}
		e = GetComponent<Entity>();
		isEntity = e != null;
	}

	void Update() {
		if (Mathf.Abs(target.transform.position.x - transform.position.x) < threshold) {
			return;
		}
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
