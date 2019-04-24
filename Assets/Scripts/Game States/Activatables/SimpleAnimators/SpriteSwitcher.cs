using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteSwitcher : SimpleAnimator {
    [Range(0, 1)]
    public float frameDelay;
    public List<Sprite> sprites;

    int currentSprite;
    SpriteRenderer spriteRenderer;
    bool hasInvoke = false;

    override protected void Init() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    override protected void Draw() {
        if (!hasInvoke) {
            hasInvoke = true;
            Invoke("IncrementSprite", frameDelay);
        }
    }

    void IncrementSprite() {
        if (++currentSprite >= sprites.Count) {
            currentSprite = 0;
        }
        spriteRenderer.sprite = sprites[currentSprite];
        hasInvoke = false;
    }
}