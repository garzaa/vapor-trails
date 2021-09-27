using UnityEngine;
using System.Collections.Generic;

public class DayGradient : DayWatcher {
	public Gradient gradient;
	TilemapRenderer[] tilemapRenderers;
	SpriteRenderer[] spriteRenderers;

	void Start() {
		renderers = GetComponentsInChildren<Renderer>();
	}

	override void OnDayUpdate(float t) {
		Color c = gradient.Evaluate(t);
		for (int i=0; i<tilemapRenderers.Length; i++) {
			tilemapRenderers[i].color = c;
		}
		for (int i=0; i<spriteRenderers.Length; i++) {
			spriteRenderers[i].color = c;
		}
	}
}
