using UnityEngine;

public class DynamicCanvasScaler : MonoBehaviour {
    static int pixelScale = 3;
    Canvas targetCanvas;

    void Start() {
        targetCanvas = GetComponent<Canvas>();
    }

    void LateUpdate() {
        pixelScale = ComputePixelScale();
        targetCanvas.scaleFactor = pixelScale;
    }

    int ComputePixelScale() {
        return Mathf.CeilToInt((float)Screen.height / 720f);
    }

    public static int GetPixelScale() {
        return pixelScale;
    }
}
