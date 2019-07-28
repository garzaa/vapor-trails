using UnityEngine;

public class PositionMover : MonoBehaviour {
    public Transform[] destinations;
    public float speed = 1f;
    public bool loop = true;
    public Activatable callback;
    public bool flipToMovement = false;

    int currentDestination = 0;

    void Update() {
        if (currentDestination >= destinations.Length) {
            return;
        }
        Transform dt = destinations[currentDestination];
        if (flipToMovement) {
            this.transform.localScale.Scale(new Vector3(
                Mathf.Sign(dt.position.x - transform.position.x),
                1, 
                1
            ));
        }
        transform.position = Vector2.MoveTowards(transform.position, dt.position, speed * Time.deltaTime);
        if (transform.position.Equals(dt.position)) {
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