using UnityEngine;

public class PositionMover : MonoBehaviour {
    public Transform[] destinations;
    public float speed = 1f;
    public bool loop = true;
    public bool closeLoop = true;
    public Activatable callback;
    public bool flipToMovement = false;
    public Transform target;
    public bool sendSpeedToAnimator;
    
    [SerializeField] int currentDestination = 0;
    Animator animator;

    void Start() {
        if (target == null) { target = this.transform; }
        if (sendSpeedToAnimator) {
            animator = target.GetComponent<Animator>();
        }
    }

    void OnDisable() {
        currentDestination = 0;
    }

    void FixedUpdate() {
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
        if (Vector2.Distance(target.transform.position, dt.transform.position) < Vector2.kEpsilon) {
            currentDestination += 1;
            if (currentDestination >= destinations.Length && loop) {
                if (closeLoop) {
                    currentDestination = 0;
                } else {
                    // just move back to the starting point
                    target.transform.position = destinations[0].position;
                    currentDestination = 1;
                }
            }
            if (callback != null) {
                callback.Activate();
            }
        }

        if (sendSpeedToAnimator) {
            float currentSpeed = speed;
            if (Vector2.Distance(target.transform.position, dt.transform.position) < Vector2.kEpsilon) {
                currentSpeed = 0;
            }
            float angle = Vector2.Angle(dt.transform.position - target.transform.position, Vector2.right) * Mathf.Deg2Rad;
            animator.SetFloat("SpeedX", Mathf.Abs(currentSpeed * Mathf.Cos(angle)));
		    animator.SetFloat("SpeedY", Mathf.Abs(currentSpeed * Mathf.Sin(angle)));
        }
    }
}
