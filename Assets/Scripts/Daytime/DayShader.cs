using UnityEngine;

[ExecuteInEditMode]
public class DayShader : DayWatcher {
	Renderer r;
	MaterialPropertyBlock block;

	void Start() {
		r = GetComponent<Renderer>();
		block = new MaterialPropertyBlock();
	}

	public override void OnDayUpdate(float t) {
		if (block == null) block = new MaterialPropertyBlock();
		r.GetPropertyBlock(block);
		block.SetFloat("_Daytime", t);
		r.SetPropertyBlock(block);
	}
}
