using UnityEngine;

public class MapCamera : MonoBehaviour {

    float moveSpeed = 15f;

    public void ResetPosition() {
        transform.localPosition = Vector2.zero;
    }

    void Update() {
        Vector3 nav = (Vector3) InputManager.UINav();
        transform.localPosition += moveSpeed * Time.unscaledDeltaTime * nav;
    }
}
