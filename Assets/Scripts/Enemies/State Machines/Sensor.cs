using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {

	protected Animator animator;
	protected GameObject player;
	protected Enemy e;

	protected void Start() {
		e = GetComponent<Enemy>();
		player = e.playerObject;
		animator = GetComponent<Animator>();
	}
}
