using UnityEngine;

public class DynamicCanvasScaler : MonoBehaviour {
    int pixelScale = 3;
    public Camera _camera;
    public Canvas targetCanvas;

    void Update() {
        int lastPixelScale = pixelScale;
        pixelScale = GetPixelScale(); 
        if (pixelScale != lastPixelScale) {
            targetCanvas.scaleFactor = pixelScale;
        }
    }

    int GetPixelScale() {
        return (Screen.height / 720) + 1;
    }
}
