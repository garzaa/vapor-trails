using UnityEngine;

public class DynamicCanvasScaler : MonoBehaviour {
    static int pixelScale = 3;
    Canvas targetCanvas;

    public float multiplier = 1;

    void Start() {
        targetCanvas = GetComponent<Canvas>();
    }

    void LateUpdate() {
        pixelScale = ComputePixelScale();
        targetCanvas.scaleFactor = Mathf.Max(1, pixelScale * multiplier);
    }

    int ComputePixelScale() {
        return Mathf.CeilToInt((float)Screen.height / 720f);
    }

    public static int GetPixelScale() {
        return pixelScale;
    }
}
