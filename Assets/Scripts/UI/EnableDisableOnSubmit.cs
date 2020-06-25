using UnityEngine;
using UnityEngine.EventSystems;

public class EnableDisableOnSubmit : MonoBehaviour, ISubmitHandler, IDeselectHandler, ISelectHandler {
    
    public GameObject target;

    public bool onDeselect;
    public bool onSelect;
    
    public void OnSubmit(BaseEventData eventData) {
        target.SetActive(!target.activeSelf);
    }

    public void OnSelect(BaseEventData eventData) {
        if (onSelect) target.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData) {
        if (onDeselect) target.SetActive(false);
    }

    public void ButtonClick() {
        target.SetActive(!target.activeSelf);
    }

    public void OnDisable() {
        target.SetActive(false);
    }
}