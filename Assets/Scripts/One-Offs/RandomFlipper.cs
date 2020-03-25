using UnityEngine;

public class RandomFlipper : MonoBehaviour {
    void Start() {
        if (Random.value > 0.5f) {
            this.transform.localScale.Scale(new Vector2(-1, 1));
        }
    }
}