using UnityEngine;

public class MoveForward : MonoBehaviour {
    Rigidbody2D rb2d;
    public float speed;

    void Start() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float degZ = transform.eulerAngles.z;
        rb2d.velocity = new Vector3(
            speed * Mathf.Cos(degZ) * transform.localScale.x,
            speed * Mathf.Sin(degZ),
            0
        );
    }
}