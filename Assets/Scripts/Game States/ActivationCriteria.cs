using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivationCriteria : MonoBehaviour {

	[HideInInspector]
	public bool isSatisfied = false;

	public abstract bool CheckSatisfied();
}
