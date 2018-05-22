using UnityEngine;
[ExecuteInEditMode]
public class PixelSnapper : MonoBehaviour {
    [Range(1, 4)]
    public int pixelScale = 1;

    private Camera _camera;

    void Update()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;
        }
        _camera.orthographicSize = Screen.height * (0.005f / pixelScale);
    }
}
