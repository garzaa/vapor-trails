using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmoothSpriteChild : MonoBehaviour {
	SpriteRenderer spriteRenderer;
	SpriteSmoother spriteSmoother;
	string spriteLastFrame;

	public void Initialize(SpriteSmoother s) {
		this.spriteSmoother = s;
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = spriteSmoother.GetUpscaledSprite(spriteRenderer.sprite, this);
		spriteLastFrame = spriteRenderer.sprite.name;
	}

	void LateUpdate() {
		// if the sprites differ, get the upscaled version from spritesmoother
		// sprites may differ every frame due to some animator keying, or they may not
		// so for performance reasons just check every frame, and optimize the check if needed
		if (spriteRenderer.sprite.name != spriteLastFrame) {
			spriteRenderer.sprite = spriteSmoother.GetUpscaledSprite(spriteRenderer.sprite, this);
		}

		spriteLastFrame = spriteRenderer.sprite.name;
	}

	public void ForceUpscaledSprite() {
		spriteRenderer.sprite = spriteSmoother.GetUpscaledSprite(spriteRenderer.sprite, this);
		spriteLastFrame = spriteRenderer.sprite.name;
	}
}
