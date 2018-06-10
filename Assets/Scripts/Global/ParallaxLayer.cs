using UnityEngine;

public class ParallaxLayer : MonoBehaviour {
 
    public Vector2 speed;
    public bool moveInOppositeDirection;
    Transform mainCamera;

    void Start() {
        mainCamera = GameObject.Find("Main Camera").transform;
    }
 
    void Update ()
    { 
        if (!Application.isPlaying)
        {
            return;
        }
 
        Vector3 distance = mainCamera.position;
        float direction = (moveInOppositeDirection) ? -1f : 1f;
        transform.position = Vector3.Scale(distance, new Vector3(speed.x, speed.y)) * direction;
    }
}