using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteMask))]
public class SpriteMaskLink : MonoBehaviour {
    public SpriteRenderer otherSpriteRenderer;
    SpriteMask mask;
    Sprite spriteLastFrame = null;

    void Start() {
        this.mask = GetComponent<SpriteMask>();
        if (otherSpriteRenderer == null) {
            otherSpriteRenderer = GetComponentInParent<SpriteRenderer>();
        }
    }

    void Update() {
        if (!otherSpriteRenderer.enabled) {
            mask.sprite = null;
            return;
        }

        if (spriteLastFrame != otherSpriteRenderer.sprite) {
            this.mask.sprite = otherSpriteRenderer.sprite;
            spriteLastFrame = otherSpriteRenderer.sprite;
        }
    }
}
