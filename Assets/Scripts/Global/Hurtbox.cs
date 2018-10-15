using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

	public GameObject parentObject;

	void Start() {
		if (parentObject == null) parentObject = this.gameObject.transform.parent.gameObject;
	}

	public Entity GetParent() {
		return parentObject.GetComponent<Entity>();
	}

	public virtual void OnHit(Attack a) {
		if (parentObject != null) {
			parentObject.GetComponent<Entity>().OnHit(a);
		}
	}
}
