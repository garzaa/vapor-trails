using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : MonoBehaviour {

	public void Show() {
		SetEnabledRecursive(this.transform, true);
	}

	public void Hide() {
		SetEnabledRecursive(this.transform, false);
	}

	public void SetEnabledRecursive(Transform t, bool e) {
		if (t.GetComponent<Image>() != null) {
			t.GetComponent<Image>().enabled = e;
		}
		foreach (Transform child in transform) {
			SetEnabledRecursive(child, e);
		}
	}

}
