using UnityEngine;
using UnityEngine.UI;

public class ScrollRectCenter : MonoBehaviour {
    ScrollRect scrollView;

    void Start() {
        scrollView = GetComponent<ScrollRect>();
    }

    public void SnapTo(RectTransform target) {
        scrollView.content.localPosition = scrollView.GetSnapToPositionToBringChildIntoView(target);
    }
}