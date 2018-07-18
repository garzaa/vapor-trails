using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

	GameObject parentObject;

	void Start() {
		parentObject = this.gameObject.transform.parent.gameObject;
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
