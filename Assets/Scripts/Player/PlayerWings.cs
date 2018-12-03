using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWings : MonoBehaviour {

	Animator anim;
	bool ledgeBoosting;

	public void Start() {
		anim = this.GetComponent<Animator>();
	}

	//sprite changing
	public void Open() {
		anim.SetBool("Open", true);
	}

	public void Close() {
		anim.SetBool("Open", false);
	}

	public void EnableJets() {
		anim.SetBool("Jets", true);
	}

	public void DisableJets() {
		anim.SetBool("Jets", false);
	}

	public void DisableJetTrails() {
		foreach (TrailRenderer t in  GetComponentsInChildren<TrailRenderer>()) {
			t.emitting = false;
		}
	}

	public void EnableJetTrails() {
		foreach (TrailRenderer t in  GetComponentsInChildren<TrailRenderer>()) {
			t.emitting = true;
		}
	}


	//wing movement
	public void Dash() {
		anim.SetTrigger("Dash");
	}

	public void Airbrake() {
		anim.SetTrigger("Airbrake");
	}

	public void Jump() {
		anim.SetTrigger("Jump");
	}

	public void Meteor() {
		anim.SetTrigger("Meteor");
	}

	public void LandMeteor() {
		anim.SetTrigger("LandMeteor");
	}

	public void LedgeBoost() {
		if (ledgeBoosting) return;
		ledgeBoosting = true;
		anim.SetTrigger("LedgeBoost");
	}

	public void Supercruise() {
		anim.SetTrigger("Supercruise");
	}

	public void SupercruiseMid() {
		anim.SetTrigger("SupercruiseMid");
	}

	//called by player and self at some animation ends
	public void FoldIn() {
		ledgeBoosting = false;
		DisableJets();
		Close();
	}

	public bool HasOpenJets() {
		return anim.GetBool("Jets");
	}

}
