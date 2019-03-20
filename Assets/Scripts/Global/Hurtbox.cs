using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour {

	public GameObject parentObject;
	public bool overrideTargetPosition;

	void Start() {
		if (parentObject == null) parentObject = GetComponentInParent<Entity>().gameObject;
	}

	public Entity GetParent() {
		return parentObject.GetComponent<Entity>();
	}

	public virtual void OnHit(Attack a) {
		if (parentObject != null) {
			parentObject.GetComponent<Entity>().OnHit(a);
		}
		if (a.hitmarker != null) a.MakeHitmarker(this.transform.position);
	}
}
