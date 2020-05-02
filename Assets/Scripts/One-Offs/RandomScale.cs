using UnityEngine;

public class RandomScale : MonoBehaviour {
    public float min = 0f;
    public float max = 1f;

    void Start() {
        transform.localScale *= Random.Range(min, max);
    }
}