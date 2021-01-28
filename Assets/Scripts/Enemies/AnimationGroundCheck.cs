using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AnimationGroundCheck : GroundCheck {

	Animator animator;
	bool touchingLedgeLastFrame;
	public bool flipOnLedgeStep;

	public float lastFlipTime;
	float flipInterval = 0.5f;

	new void Start() {
		base.Start();
		animator = GetComponent<Animator>();
		animator.logWarnings = false;
		entity = animator.GetComponent<Entity>();
	}
	
	void Update() {
		bool onLedge = OnLedge();
		if (!touchingLedgeLastFrame && onLedge) {
			animator.SetTrigger("LedgeStep");
		}
		animator.SetBool("Grounded", IsGrounded());
		animator.SetBool("TouchingLedge", onLedge);
		if (flipOnLedgeStep && onLedge && (Time.unscaledTime> lastFlipTime+flipInterval)) {
			entity.Flip();
			lastFlipTime = Time.unscaledTime;
		}
		touchingLedgeLastFrame = onLedge;
	}
}
