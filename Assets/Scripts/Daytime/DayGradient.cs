using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[ExecuteInEditMode]
public class DayGradient : DayWatcher {
	public GradientContainer gradient;
	Tilemap[] tilemaps;
	SpriteRenderer[] spriteRenderers;

	void Start() {
		tilemaps = GetComponentsInChildren<Tilemap>();
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
	}

	override public void OnDayUpdate(float t) {
		Color c = gradient.Evaluate(t);
		for (int i=0; i<tilemaps.Length; i++) {
			tilemaps[i].color = c;
		}
		for (int i=0; i<spriteRenderers.Length; i++) {
			spriteRenderers[i].color = c;
		}
	}
}
