using UnityEngine;
using System.Collections;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

// TEXTURES THIS USES HAVE TO BE READABLE IN IMPORT SETTINGS
public class SpriteSmoother : MonoBehaviour {
	const float scaleFactor = 4f;

	Dictionary<string, Texture2D> upscaledTextures = new Dictionary<string, Texture2D>();
	Dictionary<string, Sprite> upscaledSprites = new Dictionary<string, Sprite>();
	HashSet<string> queuedTextures = new HashSet<string>();

	List<SmoothSpriteChild> children = new List<SmoothSpriteChild>();

	void Start() {
		if (!GlobalController.save.options.upsample) {
			return;
		}
		foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>()) {
			if (s.GetComponent<NoSmoothSprite>()) {
				continue;
			}
			SmoothSpriteChild smoothSprite = s.gameObject.AddComponent<SmoothSpriteChild>();
			smoothSprite.Initialize(this);
			children.Add(smoothSprite);
		}
	}

	Task<AsyncTexture> UpscaleTexture(AsyncTexture input) {
		return Scale4x(input);
	}

	async Task<AsyncTexture> Scale4x(AsyncTexture input) {
		AsyncTexture t = await Task.Run(() => Scale2x(Scale2x(input)));
		return t;
	}

	AsyncTexture Scale2x(AsyncTexture input) {
		AsyncTexture output = new AsyncTexture(new Vector2Int(input.width*2, input.height*2));
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
		return output;
	}

	bool SameColor(Color a, Color b) {
		return 
			Mathf.Approximately(a.r, b.r)
			&& Mathf.Approximately(a.g, b.g)
			&& Mathf.Approximately(a.b, b.b)
			&& Mathf.Approximately(a.a, b.a);
	}

	public Sprite GetUpscaledSprite(Sprite s, SmoothSpriteChild caller) {
		if (upscaledSprites.ContainsKey(s.name)) {
 			return upscaledSprites[s.name];
		}

		string textureName = s.texture.name;
		if (!upscaledTextures.ContainsKey(textureName)) {
			if (!queuedTextures.Contains(textureName)) {
				queuedTextures.Add(textureName);
				Debug.Log("upscaling texture "+textureName+" from "+caller.gameObject.name);
				AddUpscaledTexture(
					new AsyncTexture(s.texture.GetPixels(),
					new Vector2Int(s.texture.width, s.texture.height)),
					textureName
				);
			}
			return s;
		} else {
			Sprite upscaled = ExtractSprite(s, upscaledTextures[textureName]);
			upscaledSprites.Add(s.name, upscaled);
			return upscaled;
		}
	}

	async void AddUpscaledTexture(AsyncTexture texture, string name) {
		AsyncTexture upscaledAsync = await UpscaleTexture(texture);
		Texture2D upscaled = new Texture2D(upscaledAsync.width, upscaledAsync.height);
		upscaled.filterMode = FilterMode.Point;
		upscaled.mipMapBias = -10;
		upscaled.SetPixels(upscaledAsync.GetPixels());
		upscaled.Apply(true, false);
		upscaled.name = name;
		upscaledTextures[name] = upscaled;
		queuedTextures.Remove(name);

		// abort the loop if the editor exits play mode
		if (!Application.isPlaying) return;

		foreach (SmoothSpriteChild c in children) {
			c.ForceUpscaledSprite();
		}
	}

	Sprite ExtractSprite(Sprite original, Texture2D upscaledAtlas) {
		Rect spriteRect = original.rect;
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

		upscaled.name = original.name;

		return upscaled;
	}
}
