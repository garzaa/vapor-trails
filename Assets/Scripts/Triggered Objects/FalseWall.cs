using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FalseWall : PlayerTriggeredObject {

	public override void OnPlayerEnter() {
		GetComponent<Animator>().SetBool("Hidden", true);
	}

	public override void OnPlayerExit() {
		GetComponent<Animator>().SetBool("Hidden", false);
	}
}
