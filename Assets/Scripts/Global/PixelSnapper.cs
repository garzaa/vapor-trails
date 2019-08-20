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
        //pixelScale = (Screen.height / 720) + 1;

        if (pixelScale != lastPixelScale) {
            _camera.orthographicSize = Screen.height * (0.005f / pixelScale);
            UpdateUI(pixelScale);
        }
    }

    void UpdateUI(int ratio) {
        GameObject.Find("PixelCanvas").GetComponent<Canvas>().scaleFactor = pixelScale;
    }
}
