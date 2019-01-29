using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {

	public Animator animator;
	public Enemy e;
	protected GameObject player;
	protected PlayerController pc;

	protected void Start() {
		if (e == null) {
			e = GetComponent<Enemy>();
			if (e != null) {
				player = e.playerObject;
				pc = e.playerObject.GetComponent<PlayerController>();
			}
		}
		if (animator == null) {
			animator = GetComponent<Animator>();
		}
	}
}
