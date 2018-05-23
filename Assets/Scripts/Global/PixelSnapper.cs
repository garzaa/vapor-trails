using UnityEngine;
[ExecuteInEditMode]
public class PixelSnapper : MonoBehaviour {
    int pixelScale = 3;

    private Camera _camera;

    void Update()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;
        }
        if (Screen.height < 720) {
            pixelScale = 2;
        } 
        else if (Screen.height <= 1080) {
            pixelScale = 3;
        }
        _camera.orthographicSize = Screen.height * (0.005f / pixelScale);
    }
}
