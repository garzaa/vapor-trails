using UnityEngine;

public class UILerper : MonoBehaviour {

    [SerializeField]
    float lerpSpeed;

    RectTransform target;
    Vector3 originalPos;
    RectTransform selfTransform;

    void Start() {
        selfTransform = GetComponent<RectTransform>();
        originalPos = selfTransform.position;
    }

    public void SetTarget(RectTransform transform) {
        this.target = transform;
    }

    void Update() {
        if (target != null) {
            selfTransform.position = Vector2.Lerp(
                selfTransform.position,
                originalPos - target.localPosition,
                lerpSpeed
            );
        }
    }

}