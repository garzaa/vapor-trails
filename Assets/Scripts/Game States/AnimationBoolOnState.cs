using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBoolOnState : MonoBehaviour {

	public string boolName;
	public GameFlag gameFlag;
	public bool invertPresence;

	private bool toSet;

	void Start() {
		toSet = SaveManager.HasFlag(gameFlag) && !invertPresence;
	}

	//to play nice with animators that get activated and deactivated
	protected virtual void Update() {
		GetComponent<Animator>().SetBool(boolName, toSet);
	}
}
