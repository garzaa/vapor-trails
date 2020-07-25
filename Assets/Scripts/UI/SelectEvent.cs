using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectEvent : MonoBehaviour, ISelectHandler {

    public UnityEvent onSelect;

    public void OnSelect(BaseEventData eventData) {
        onSelect.Invoke();
    }
}
