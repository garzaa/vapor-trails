using UnityEngine;


[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour {
 
    public Vector2 speed;
   	public bool moveInOppositeDirection;

	private Transform cameraTransform;
	private Vector3 previousCameraPosition;
	private bool activeLastFrame;
	private ParallaxOption options;

	void Start() {
		GameObject gameCamera = GameObject.Find("Main Camera");
		if (gameCamera == null) return;
		options = gameCamera.GetComponent<ParallaxOption>();
		cameraTransform = gameCamera.transform;
		previousCameraPosition = Vector2.zero;
		RoundChildren();
	}

	public void Update() {
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

    void RoundChildren() {
        foreach (Transform child in transform) {
            child.localPosition = child.localPosition.Round(2);
        }
    }

	void Move() {
		Vector3 distance = cameraTransform.position - previousCameraPosition;
		float direction = (moveInOppositeDirection) ? -1f : 1f;
		transform.position += Vector3.Scale(distance, new Vector3(speed.x, speed.y)) * direction;
	}
}