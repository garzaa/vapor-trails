using UnityEngine;

public class TouchActivator : MonoBehaviour {
    public Activatable activatable;

    void OnColliderEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(Tags.Player)) {
            activatable.Activate();
        }
    }
}