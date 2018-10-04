using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
 
    public Vector2 speed;
    Transform mainCamera;
    Vector3 originalPos;
    Vector3 originalCamPos;

    public bool parallaxEnabled = true;

    public void Start() {
        ExtendedStart();
        if (parallaxEnabled) {
            mainCamera = GameObject.Find("Main Camera").transform;
            originalCamPos = new Vector3(
                0,
                0,
                this.transform.position.z
            );
            originalPos = this.transform.position;
        }
    }
 
    void Update () {
        if (parallaxEnabled) {
            Vector3 camPos = new Vector3(
                mainCamera.position.x,
                mainCamera.position.y,
                this.transform.position.z
            );
            Vector3 totalCamDelta = camPos - originalCamPos;
            transform.position = originalPos + Vector3.Scale(totalCamDelta, new Vector3(speed.x, speed.y));
        }
    }

    public virtual void ExtendedUpdate() {

    }

    public virtual void ExtendedStart() {
        
    }
}