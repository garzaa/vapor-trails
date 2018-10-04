using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour {

	public virtual void ActivateSwitch(bool b) {
		this.gameObject.SetActive(b);
	}

	public virtual void Activate() {
		this.ActivateSwitch(true);
	}
}
