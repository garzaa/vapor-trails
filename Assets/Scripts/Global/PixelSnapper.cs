using UnityEngine;
[ExecuteInEditMode]
public class PixelSnapper : MonoBehaviour {
    int pixelScale = 3;

    private Camera _camera;

    void Start() {
        if (_camera == null) {
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;
        }
    }

    void Update()
    {
        int lastPixelScale = pixelScale;
        if (Screen.height < 720) {
            pixelScale = 2;
        } 
        else if (Screen.height <= 1080) {
            pixelScale = 3;
        }
        _camera.orthographicSize = Screen.height * (0.005f / pixelScale);
        if (pixelScale != lastPixelScale) {
            UpdateUI(pixelScale);
        }
    }

    void UpdateUI(int ratio) {
        GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor = pixelScale;
    }
}
