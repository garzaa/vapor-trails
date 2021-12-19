using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TEXTURES THIS USES HAVE TO BE READABLE
// https://www.scale2x.it/algorithm
public class SpriteSmoother : MonoBehaviour {
	const float scaleFactor = 4f;

	// build a mapping of sprites to their upscaled versions
	public Dictionary<string, Texture2D> upscaledTextures = new Dictionary<string, Texture2D>();
	public Dictionary<string, Sprite> upscaledSprites = new Dictionary<string, Sprite>();

	void Start() {
		if (!GlobalController.save.options.upsample) {
			return;
		}
		foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) {
			SmoothSpriteChild smoothSprite = s.gameObject.AddComponent<SmoothSpriteChild>();
			smoothSprite.Initialize(this);
		}
	}

	Texture2D UpscaleTexture(Texture2D input) {
		if (!GlobalController.save.options.upsample) {
			return input;
		}
		return Scale4x(input);
	}

	Texture2D Scale4x(Texture2D input) {
		return Scale2x(Scale2x(input));
	}

	Texture2D Scale8x(Texture2D input) {
		return Scale2x(Scale2x(Scale2x(input)));
	}

	Texture2D Scale2x(Texture2D input) {
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

	Texture2D Scale3x(Texture2D input) {
		Texture2D output = new Texture2D(input.width*3, input.height*3);
		output.filterMode = FilterMode.Point;
		output.mipMapBias = -10;

		for (int x=0; x<input.width; x++) {
			for (int y=0; y<input.height; y++) {
				/*
					A B C
					D E F
					G H I

					E0 E1 E2
					E3 E4 E5
					E6 E7 E8
				*/
				Color a = input.GetPixel(x-1, y+1);
				Color b = input.GetPixel(x, y-1);
				Color c = input.GetPixel(x+1, y+1);
                Color d = input.GetPixel(x-1, y);
				Color e = input.GetPixel(x, y);
                Color f = input.GetPixel(x+1, y);
				Color g = input.GetPixel(x-1, y-1);
                Color h = input.GetPixel(x, y+1);
				Color i = input.GetPixel(x+1, y-1);
               
				Color e0, e1, e2, e3, e4, e5, e6, e7, e8;

				if (!SameColor(b, h) && !SameColor(d, f)) {
					e0 = (SameColor(d, b)) ? d : e;
					e1 = (SameColor(d, b) && !SameColor(e, c)) || (SameColor(b, f) && !SameColor(e, a)) ? b : e;
					e2 = SameColor(b, f) ? f : e;
					e3 = (SameColor(d, b) && !SameColor(e, g)) || (SameColor(d, h) && !SameColor(e, a)) ? d : e;
					e4 = e;
					e5 = (SameColor(b, f) && !SameColor(e, i)) || (SameColor(h, f) && !SameColor(e, c)) ? f : e;
					e6 = SameColor(d, h) ? d : e;
					e7 = (SameColor(d, h) && !SameColor(e, i)) || (SameColor(h, f) && !SameColor(e, g)) ? h : e;
					e8 = SameColor(h, f) ? f : e;
				} else {
					e0 = e1 = e2 = e3 = e4 = e5 = e6 = e7 = e8 = e;
				}

				output.SetPixel(2*x-1, 2*y+1, e0);
				output.SetPixel(2*x, 2*y-1, e1);
				output.SetPixel(2*x+1, 2*y+1, e2);
				output.SetPixel(2*x-1, 2*y, e3);
				output.SetPixel(2*x, 2*y, e4);
				output.SetPixel(2*x+1, 2*y, e5);
				output.SetPixel(2*x-1, 2*y-1, e6);
				output.SetPixel(2*x, 2*y-1, e7);
				output.SetPixel(2*x+1, 2*y+1, e8);
			}
		}

		output.Apply(false, false);
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
