using UnityEngine;


[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour {
 
    public Vector2 speed;
   	public bool moveInOppositeDirection;

	private Transform cameraTransform;
	private Vector3 previousCameraPosition;
	private bool previousMoveParallax;
	private ParallaxOption options;

	void Start() {
		GameObject gameCamera = GameObject.Find("Main Camera");
		if (gameCamera == null) return;
		options = gameCamera.GetComponent<ParallaxOption>();
		cameraTransform = gameCamera.transform;
		previousCameraPosition = cameraTransform.position;
	}

	void Update () {
		if (cameraTransform == null) return;
		if(options.moveParallax && !previousMoveParallax)
			previousCameraPosition = cameraTransform.position;

		previousMoveParallax = options.moveParallax;

		if(!Application.isPlaying && !options.moveParallax)
			return;

		Vector3 distance = cameraTransform.position - previousCameraPosition;
		float direction = (moveInOppositeDirection) ? -1f : 1f;
		transform.position += Vector3.Scale(distance, new Vector3(speed.x, speed.y)) * direction;

		previousCameraPosition = cameraTransform.position;
		RoundChildren();
	}

    public virtual void ExtendedStart() {
        
    }

    void RoundChildren() {
        this.transform.position = this.transform.position.Round(2);
        foreach (Transform child in transform) {
            child.position = child.position.Round(2);
        }
    }
}