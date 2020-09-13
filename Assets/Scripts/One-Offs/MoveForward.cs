using UnityEngine;

public class MoveForward : MonoBehaviour {
    Rigidbody2D rb2d;
    public float speed;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float degZ = transform.eulerAngles.z;
        rb2d.velocity = (Vector2.right * transform.lossyScale.x).Rotate(degZ) * speed;
    }
}