using UnityEngine;
using UnityEngine.EventSystems;

public class CenterInScrollView : MonoBehaviour, ISelectHandler {

    ScrollRectCenter rectCenter;

    public RectTransform optionalAnchor;

    void Start() {
        rectCenter = GetComponentInParent<ScrollRectCenter>();
    }

    public void OnSelect(BaseEventData eventData) {
        rectCenter.SnapTo(optionalAnchor != null ? optionalAnchor : this.GetComponent<RectTransform>());
    }
}