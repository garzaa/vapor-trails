using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWings : MonoBehaviour {

	Animator anim;

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


	//wing movement
	public void Dash() {
		anim.SetTrigger("Dash");
	}

	public void Jump() {
		anim.SetTrigger("Jump");
	}

	public void Meteor() {
		anim.SetBool("Meteor", true);
	}

	public void EndMeteor() {
		anim.SetBool("Meteor", false);
	}

	//called by player at the end of a jump animation or whatever
	public void FoldIn() {
		DisableJets();
		Close();
	}

}
