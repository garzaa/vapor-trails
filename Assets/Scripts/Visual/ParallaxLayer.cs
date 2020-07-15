using UnityEngine;


[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour {
 
    public Vector2 speed;
   	public bool moveInOppositeDirection;

	private Transform cameraTransform;
	private Vector3 previousCameraPosition;
	private bool activeLastFrame;
	private ParallaxOption options;

	public bool zeroOnStart = true;

	void Start() {
		GameObject gameCamera = GameObject.Find("Main Camera");
		if (gameCamera == null) return;
		options = gameCamera.GetComponent<ParallaxOption>();
		cameraTransform = gameCamera.transform;
		previousCameraPosition = Vector2.zero;
		RoundChildren(this.transform);
		if (zeroOnStart) transform.localPosition = Vector2.zero;
	}

	public void LateUpdate() {

		if (cameraTransform == null ) {
			return;
		}

		// if parallax wasn't active last frame
		if (options.moveParallax && !activeLastFrame) {
			Start();
			activeLastFrame = options.moveParallax;
			return;
		}

		activeLastFrame = options.moveParallax;

		if (!Application.isPlaying && !options.moveParallax){
			return;
		}

		Move();
		previousCameraPosition = cameraTransform.position;
	}

    public virtual void ExtendedStart() {
        
    }

    void RoundChildren(Transform t) {
		t.position = t.position.Round(2);
        foreach (Transform child in t) {
            child.position = child.position.Round(2);
			RoundChildren(child);
        }
    }

	void Move() {
		Vector3 distance = cameraTransform.position - previousCameraPosition;
		float direction = (moveInOppositeDirection) ? -1f : 1f;
		transform.localPosition += Vector3.Scale(distance, new Vector3(speed.x, speed.y)) * direction;
	}
}