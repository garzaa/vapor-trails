using UnityEngine;
using UnityEngine.EventSystems;

public class CenterOnSelect : MonoBehaviour, ISelectHandler {

    UILerper lerper;

    void Start() {
        lerper = GetComponentInParent<UILerper>();
    }

    public void OnSelect(BaseEventData eventData) {
        if (lerper == null) Start();
        lerper.SetTarget(this.GetComponent<RectTransform>());
    }
}