using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	Rigidbody2D rb2d;

	void Start() {
		rb2d = GetComponent<Rigidbody2D>();
		this.transform.parent = null;
	}
}
