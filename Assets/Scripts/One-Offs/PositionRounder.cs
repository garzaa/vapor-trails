using UnityEngine;

public class PositionRounder : MonoBehaviour {
    [SerializeField] float precision = 1.0f;
    [SerializeField] bool onStart = true;
    [SerializeField] bool onUpdate = false;

    void Start() {
        if (onStart) RoundPosition();
    }

    void LateUpdate() {
        if (onUpdate) RoundPosition();
    }

    void RoundPosition() {
        transform.position = new Vector2(
            Mathf.Round(transform.position.x * precision) / precision,
            Mathf.Round(transform.position.y * precision) / precision
        );
    }
}