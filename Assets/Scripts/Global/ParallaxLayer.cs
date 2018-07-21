using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
 
    public Vector2 speed;
    Transform mainCamera;
    Vector3 originalPos;
    Vector3 originalCamPos;

    void Start() {
        mainCamera = GameObject.Find("Main Camera").transform;
        originalCamPos = new Vector3(
            mainCamera.position.x,
            mainCamera.position.y,
            this.transform.position.z
        );
        originalPos = this.transform.position;
    }
 
    void Update () { 
        Vector3 camPos = new Vector3(
            mainCamera.position.x,
            mainCamera.position.y,
            this.transform.position.z
        );
        Vector3 totalCamDelta = camPos - originalCamPos;
        transform.position = originalPos + Vector3.Scale(totalCamDelta, new Vector3(speed.x, speed.y));
    }
}