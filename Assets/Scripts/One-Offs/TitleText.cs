using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleText : MonoBehaviour {
	
	public Text title;
	public Text subTitle;

	Animator anim;

	void Awake() {
		anim = GetComponent<Animator>();
	}

	public void ShowText(string title, string subTitle = null) {
		this.title.text = title;
		if (!string.IsNullOrEmpty(subTitle)) {
			this.subTitle.text = subTitle;
		} else {
			this.subTitle.text = "";
		}
		anim.SetTrigger("ShowText");
	}

}
