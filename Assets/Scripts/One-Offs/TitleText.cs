using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleText : MonoBehaviour {
	
	public Text title;
	public Text subTitle;

	Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
	}

}
