using UnityEngine;

public class MapCamera : MonoBehaviour {

    float moveSpeed = 15f;

    public void ResetPosition() {
        transform.localPosition = Vector2.zero;
    }

    void Update() {
        transform.localPosition += moveSpeed * Time.deltaTime * (Vector3) InputManager.RightStick();
    }
}