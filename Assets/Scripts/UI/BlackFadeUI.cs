using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackFadeUI : UIComponent {

	Animator anim;

	void Awake() {
		anim = GetComponent<Animator>();
	}

	public override void Show() {
		anim.SetBool("Shown", true);
	}

	public override void Hide() {
		anim.SetBool("Shown", false);
	}
}
