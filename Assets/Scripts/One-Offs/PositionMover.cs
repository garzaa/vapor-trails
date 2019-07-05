using UnityEngine;

public class PositionMover : MonoBehaviour {
    public Transform[] destinations;
    public float speed = 1f;
    public bool loop = true;
    public Activatable callback;

    int currentDestination = 0;

    void Update() {
        if (currentDestination >= destinations.Length) {
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, destinations[currentDestination].position, speed * Time.deltaTime);
        if (transform.position.Equals(destinations[currentDestination].position)) {
            currentDestination += 1;
            if (loop && currentDestination >= destinations.Length) {
                currentDestination = 0;
            }
            if (callback != null) {
                callback.Activate();
            }
        }
    }    
}