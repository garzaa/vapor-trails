using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {

	public Animator animator;
	protected Entity e;
	protected GameObject player;
	protected PlayerController pc;

	virtual protected void Start() {
		if (e == null) {
			e = GetComponent<Entity>();
		}
		if (animator == null) {
			animator = GetComponent<Animator>();
		}
		player = GlobalController.pc.gameObject;
		pc = GlobalController.pc;
	}
}
