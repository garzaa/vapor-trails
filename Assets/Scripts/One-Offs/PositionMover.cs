using UnityEngine;

public class PositionMover : MonoBehaviour {
    public Transform[] destinations;
    public float speed = 1f;
    public bool loop = true;
    public Activatable callback;
    public bool flipToMovement = false;
    public Transform target;
    public bool sendSpeedToAnimator;

    int currentDestination = 0;
    Animator animator;
    
    void Start() {
        if (target == null) { target = this.transform; }
        if (sendSpeedToAnimator) {
            animator = target.GetComponent<Animator>();
        }
    }

    void Update() {
        if (currentDestination >= destinations.Length) {
            return;
        }
        Transform dt = destinations[currentDestination];
        if (flipToMovement) {
            target.transform.localScale = new Vector3(
                Mathf.Sign(dt.position.x - target.transform.position.x),
                1, 
                1
            );
        }
        target.transform.position = Vector2.MoveTowards(target.transform.position, dt.position, speed * Time.deltaTime);
        if (target.transform.position.Equals(dt.position)) {
            currentDestination += 1;
            if (loop && currentDestination >= destinations.Length) {
                currentDestination = 0;
            }
            if (callback != null) {
                callback.Activate();
            }
        }

        if (sendSpeedToAnimator) {
            float angle = Vector2.Angle(dt.transform.position - target.transform.position, Vector2.right);
            animator.SetFloat("SpeedX", Mathf.Abs(speed * Mathf.Cos(angle)));
		    animator.SetFloat("SpeedY", Mathf.Abs(speed * Mathf.Sin(angle)));
        }
    }    
}