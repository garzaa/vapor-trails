using UnityEngine;

public class UILerper : MonoBehaviour {

    [SerializeField]
    float lerpSpeed = 0.2f;
    public bool xOnly = false;
    public bool yOnly = false;

    RectTransform target;
    Vector3 originalPos;
    RectTransform selfTransform;

    public bool resetOnEnable = false;

    void Awake() {
        selfTransform = GetComponent<RectTransform>();
        originalPos = selfTransform.position;
    }

    void OnEnable() {
        selfTransform.position = originalPos;
        if (originalPos != Vector3.zero) Debug.LogWarning(gameObject.name + " isn't starting at (0,0)! Pain will occur when resizing the window.");
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
        } else if (yOnly) {
            selfTransform.position = new Vector2(
                originalPos.x,
                selfTransform.position.y
            );
        }
    }
}
