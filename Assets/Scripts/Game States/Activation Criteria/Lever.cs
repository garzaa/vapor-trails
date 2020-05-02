using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ActivationCriteria {
	public bool flipped = false;
	public bool active = true;
	public bool flipToInactive = false;
	Animator animator;

	bool hitTimeout = false;

	void Start() {
		animator = GetComponent<Animator>();
	}

	void Update() {
		if (animator != null) {
			animator.SetBool("Active", active);
		}
	}

	void ReEnableHitting() {
		hitTimeout = false;
		satisfied = false;
		UpdateSatisfied();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (active && (other.GetComponent<PlayerAttack>() != null) && !hitTimeout) {
			hitTimeout = true;
			Invoke("ReEnableHitting", 1);
			PlayerAttack a = other.GetComponent<PlayerAttack>();
			if (a != null) {
				CameraShaker.Shake(a.cameraShakeIntensity, a.cameraShakeTime);
				SoundManager.HitSound();
				//instantiate the hitmarker
				if (a.hitmarker != null) {
					a.MakeHitmarker(this.transform);
				}
			}

			Flip();

			//then at the end of the flip, reset
			satisfied = true;
			base.UpdateSatisfied();
		}
	}

	public void Flip() {
		flipped = !flipped;
		
		if (animator != null) {
			animator.SetBool("Flipped", flipped);
			if (flipToInactive) {
				animator.SetBool("Active", false);
				active = false;
			}
		}
	}
}
