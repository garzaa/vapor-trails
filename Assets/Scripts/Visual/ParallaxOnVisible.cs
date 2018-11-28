using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxOnVisible : MonoBehaviour {
    SpriteRenderer r;
    public Vector2 speed;
    Transform mainCamera;
    Vector3 originalPos;
    Vector3 originalCamPos;
    bool parallaxEnabled = false;

    void Start() {
        r = GetComponent<SpriteRenderer>();
        mainCamera = GameObject.Find("Main Camera").transform;
        originalPos = this.transform.position;
    }

    void Update() {
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

    void OnBecameVisible() {
        print("ENABLED");
        this.parallaxEnabled = true;
        this.originalCamPos = mainCamera.position;
    }

    void OnBecameInvisible() {
        print("DISABLED");
        this.parallaxEnabled = false;
        this.transform.position = this.originalPos;
    }
}
