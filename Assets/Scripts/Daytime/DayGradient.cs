using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

[ExecuteInEditMode]
public class DayGradient : DayWatcher {
	public GradientContainer gradient;
	Tilemap[] tilemaps;
	SpriteRenderer[] spriteRenderers;
	MaterialColorEditor[] colorEditors;

	void Start() {
		tilemaps = GetComponentsInChildren<Tilemap>();
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		colorEditors = GetComponentsInChildren<MaterialColorEditor>();
	}

	override public void OnDayUpdate(float t) {
		Color c = gradient.Evaluate(t);
		UpdateTilemaps(tilemaps, c);
		UpdateSprites(spriteRenderers, c);
		UpdateMaterials(colorEditors, c);
	}

	void UpdateTilemaps(Tilemap[] tilemaps, Color c) {
		for (int i=0; i<tilemaps.Length; i++) {
			tilemaps[i].color = c;
		}
	}

	void UpdateSprites(SpriteRenderer[] spriteRenderers, Color c) {
		for (int i=0; i<spriteRenderers.Length; i++) {
			spriteRenderers[i].color = c;
		}
	}

	void UpdateMaterials(MaterialColorEditor[] materialColorEditors, Color c) {
		for (int i=0; i<colorEditors.Length; i++) {
			colorEditors[i].color = c;
		}
	}
}
