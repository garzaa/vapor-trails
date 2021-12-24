using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundCheck))]
public class AnimationGroundCheck : MonoBehaviour {

	Animator animator;
	bool touchingLedgeLastFrame;
	public bool flipOnLedgeStep = true;
	GroundData groundData;

	float lastFlipTime;
	float flipInterval = 0.5f;
	Entity entity;

	void Start() {
		animator = GetComponent<Animator>();
		animator.logWarnings = false;
		entity = animator.GetComponent<Entity>();
		groundData = GetComponent<GroundCheck>().groundData;
	}
	
	void Update() {
		if (groundData.ledgeStep) {
			animator.SetTrigger("LedgeStep");
			if (flipOnLedgeStep && groundData.onLedge && (Time.unscaledTime > lastFlipTime+flipInterval)) {
				entity.Flip();
				lastFlipTime = Time.unscaledTime;
			}
		}
		animator.SetBool("Grounded", groundData.grounded);
	}
}
