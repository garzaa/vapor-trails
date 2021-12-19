using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AsyncTexture {
	public int width { get; private set; }
	public int height { get; private set; }
	Color[] pixels;

	public AsyncTexture(Color[] pixels, Vector2Int size) {
		this.pixels = pixels;
		this.width = size.x;
		this.height = size.y;
	}

	public AsyncTexture(Vector2Int size) {
		this.pixels = new Color[size.x * size.y];
		this.width = size.x;
		this.height = size.y;
	}

	public Color GetPixel(int x, int y) {
		// if it's within bounds, then return x and y
		// otherwise, return a clamped value

		if (x>=width) x = width-1;
		else if (x<0) x = 0;
		if (y>=height) y = height-1;
		else if (y<0) y = 0;

		// origin is at the bottom left
		return pixels[y*width + x];
	}

	public void SetPixel(int x, int y, Color color) {
		if (x>=width) x = width-1;
		else if (x<0) x = 0;
		if (y>=height) y = height-1;
		else if (y<0) y = 0;

		pixels[y*width + x] = color;
	}

	public Color[] GetPixels() {
		return pixels;
	}
}
