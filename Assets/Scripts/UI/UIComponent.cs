using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : MonoBehaviour {

	public virtual void Show() {
		SetEnabledRecursive(this.transform, true);
	}

	public virtual void Hide() {
		SetEnabledRecursive(this.transform, false);
	}

	public void SetEnabledRecursive(Transform t, bool e) {
		if (t.GetComponent<UIComponent>() != null) {
			if (e) {
				t.GetComponent<UIComponent>().Show();
			} else {
				t.GetComponent<UIComponent>().Hide();
			}
		}
		foreach (Transform child in transform) {
			SetEnabledRecursive(child, e);
		}
	}

}
