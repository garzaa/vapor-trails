using UnityEngine;
using UnityEngine.EventSystems;

public class CenterOnSelect : MonoBehaviour, ISelectHandler {

    UILerper lerper;

    void Awake() {
        lerper = GetComponentInParent<UILerper>();
    }

    public void OnSelect(BaseEventData eventData) { 
        if (GetComponent<RectTransform>() != null)
            lerper.SetTarget(GetComponent<RectTransform>());
    }
}
