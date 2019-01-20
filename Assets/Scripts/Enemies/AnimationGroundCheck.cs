using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AnimationGroundCheck : GroundCheck {

	Animator animator;

	new void Start() {
		base.Start();
		animator = GetComponent<Animator>();
		animator.logWarnings = false;
	}
	
	void Update() {
		animator.SetBool("Grounded", IsGrounded());
		animator.SetBool("TouchingLedge", OnLedge());
	}
}
