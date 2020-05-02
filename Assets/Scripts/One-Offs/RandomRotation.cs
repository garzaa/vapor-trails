using UnityEngine;

public class RandomRotation : MonoBehaviour {
    void Start() {
        transform.Rotate(0f, 0f, Random.Range(0f, 360f));
    }
}