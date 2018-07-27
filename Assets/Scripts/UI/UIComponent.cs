using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : MonoBehaviour {

	public virtual void Show() {
		if (GetComponent<Image>() != null) {
			GetComponent<Image>().enabled = true;
		}
		if (GetComponent<Text>() != null) {
			GetComponent<Text>().enabled = true;
		}
	}

	public virtual void Hide() {
		if (GetComponent<Image>() != null) {
			GetComponent<Image>().enabled = false;
		}
		if (GetComponent<Text>() != null) {
			GetComponent<Text>().enabled = false;
		}
	}
}
