using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ChildSpriteColorizer : MonoBehaviour {
    public Color color;

    Color lastColor;

    SpriteRenderer[] sprites;

    void OnEnable() {
        sprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
    }

    void Update() {
        if (lastColor != null && color != lastColor) {
            for (int i=0; i<sprites.Length; i++) {
                sprites[i].color = color;
            }
        }
        lastColor = color;
    }
}