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
        originalPos = selfTransform.anchoredPosition;
    }

    void OnEnable() {
        this.target = null;
    }

    public void SetTarget(RectTransform transform) {
        this.target = transform;
    }

    void Update() {
        if (target != null) {
            selfTransform.anchoredPosition = Vector2.Lerp(
                selfTransform.anchoredPosition,
                originalPos - target.localPosition,
                lerpSpeed
            );
        }
        if (xOnly) {
            selfTransform.anchoredPosition = new Vector2(
                selfTransform.anchoredPosition.x,
                originalPos.y
            );
        } else if (yOnly) {
            selfTransform.anchoredPosition = new Vector2(
                originalPos.x,
                selfTransform.anchoredPosition.y
            );
        }
    }
}
