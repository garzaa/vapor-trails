using UnityEngine;

public class UILerper : MonoBehaviour {

    [SerializeField]
    float lerpSpeed = 0.2f;
    public bool xOnly = false;

    RectTransform target;
    Vector3 originalPos;
    RectTransform selfTransform;

    void Awake() {
        selfTransform = GetComponent<RectTransform>();
        originalPos = selfTransform.position;
    }

    void OnEnable() {
        selfTransform.position = originalPos;
        this.target = null;
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
        if (xOnly) {
            selfTransform.position = new Vector2(
                selfTransform.position.x,
                originalPos.y
            );
        }
    }
}