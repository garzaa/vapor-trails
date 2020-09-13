using UnityEngine;

public class RandomRotation : MonoBehaviour {
    public float lowBound = 0;
    public float highBound = 360f;

    void Start() {
        transform.rotation = Quaternion.Euler(
            0,
            0,
            transform.rotation.eulerAngles.z + Random.Range(lowBound, highBound)
        );
    }
}