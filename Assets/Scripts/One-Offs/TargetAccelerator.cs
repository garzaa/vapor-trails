using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TargetAccelerator : AcceleratorController {
	GameObject target;

	void OnEnable() {
		if (!target) {
			target = new GameObject("BoostTarget");
			target.transform.parent = this.transform;
			target.transform.localPosition = this.transform.localPosition + Vector3.up;
		}
	}

	public override Vector2 GetBoostVector() {
		// V0 = √(2G * Hmax)
		// currently only works for vertical jumps
		// put the player ABOVE it with extra room for their feet
		// why...why do I have to subtract 1 here instead of adding it
		float hMax = target.transform.position.y - 1.0f - this.transform.position.y;
		float vY = Mathf.Sqrt(2f*Physics2D.gravity.y * hMax);
		return new Vector2(0, vY);
	}

}
