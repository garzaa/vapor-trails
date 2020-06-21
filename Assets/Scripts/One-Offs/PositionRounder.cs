using UnityEngine;

public class PositionRounder : MonoBehaviour {
    [SerializeField] float precision = 1.0f;

    void Start() {
        RoundPosition();
    }

    void RoundPosition() {
        transform.position = new Vector2(
            Mathf.Round(transform.position.x * precision) / precision,
            Mathf.Round(transform.position.y * precision) / precision
        );
    }
}