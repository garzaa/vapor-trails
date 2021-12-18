using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TEXTURES THIS USES HAVE TO BE READABLE
// ok this is slow as fuck, what's the holdup here
// try a single 3x
public class SpriteSmoother : MonoBehaviour {
	const float scaleFactor = 8f;

	// build a mapping of sprites to their upscaled versions
	// can you get the names of them? have to upscale the entire texture
	// ok map texture names to their upscaled versions
	public Dictionary<string, Texture2D> upscaledTextures = new Dictionary<string, Texture2D>();

	// maybe also build a cache of extracted sprites? more memory, but better framewise performance
	public Dictionary<string, Sprite> upscaledSprites = new Dictionary<string, Sprite>();

	void Start() {
		foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) {
			SmoothSpriteChild smoothSprite = s.gameObject.AddComponent<SmoothSpriteChild>();
			smoothSprite.Initialize(this);
		}
	}

	Texture2D UpscaleTexture(Texture2D input) {
		Debug.Log("upscaling texture "+input.name);
		return Scale2X(Scale2X(Scale2X(input)));
	}

	Texture2D Scale2X(Texture2D input) {
		Texture2D output = new Texture2D(input.width*2, input.height*2);
		output.filterMode = FilterMode.Point;
		output.mipMapBias = -10;
		for (int x=0; x<input.width; x++) {
			for (int y=0; y<input.height; y++) {
				/*
					A B C
					D E F
					G H I

					E0 E1
					E2 E3
				*/
				Color b = input.GetPixel(x, y-1);
                Color h = input.GetPixel(x, y+1);
                Color d = input.GetPixel(x-1, y);
                Color f = input.GetPixel(x+1, y);
               
                Color e = input.GetPixel(x, y);

				if (!SameColor(b, h) && !SameColor(d, f)) {
					output.SetPixel(2*x,   2*y,   SameColor(d, b) ? d : e);
                    output.SetPixel(2*x+1, 2*y,   SameColor(b, f) ? f : e);
                    output.SetPixel(2*x,   2*y+1, SameColor(d, h) ? d : e);
                    output.SetPixel(2*x+1, 2*y+1, SameColor(h, f) ? f : e);
				} else {
					output.SetPixel(2*x,   2*y,   e);
                    output.SetPixel(2*x+1, 2*y,   e);
                    output.SetPixel(2*x,   2*y+1, e);
                    output.SetPixel(2*x+1, 2*y+1, e);
				}
			}
		}
		output.Apply(true, false);
		return output;
	}

	bool SameColor(Color a, Color b) {
		return Mathf.Approximately(a.r, b.r) && Mathf.Approximately(a.g, b.g) && Mathf.Approximately(a.b, b.b) && Mathf.Approximately(a.a, b.a);
	}

	public Sprite GetUpscaledSprite(Sprite s) {
		// if the sprite is upscaled, then return its counterpart
		if (upscaledSprites.ContainsKey(s.name)) {
 			return upscaledSprites[s.name];
		}

		string textureName = s.texture.name;
		if (!upscaledTextures.ContainsKey(textureName)) {
			upscaledTextures.Add(textureName, UpscaleTexture(s.texture));
		}
		Sprite upscaled = ExtractSprite(s, upscaledTextures[textureName]);
		upscaledSprites.Add(s.name, upscaled);

		return upscaled;
	}

	Sprite ExtractSprite(Sprite original, Texture2D upscaledAtlas) {
		Rect spriteRect = original.rect;
		// upscale the bits individually
		spriteRect.position *= scaleFactor;
		spriteRect.size *= scaleFactor;

		Sprite upscaled = Sprite.Create(
			upscaledAtlas,
			spriteRect,
			original.pivot*scaleFactor / spriteRect.size,
			original.pixelsPerUnit * scaleFactor,
			1,
			SpriteMeshType.FullRect,
			original.border,
			false
		);

		return upscaled;
	}
}
