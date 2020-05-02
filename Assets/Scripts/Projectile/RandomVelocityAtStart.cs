using UnityEngine;

public class RandomVelocityAtStart : MonoBehaviour {
    
    public float speed = 1f;

    void Start() {
        GetComponent<Rigidbody2D>().velocity = Random.onUnitSphere * speed;
    }
}