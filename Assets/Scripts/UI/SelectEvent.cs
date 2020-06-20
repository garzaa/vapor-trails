using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectEvent : MonoBehaviour, ISelectHandler {

    [SerializeField]
    UnityEvent onSelect;

    public void OnSelect(BaseEventData eventData) {
        onSelect.Invoke();
    }
}
