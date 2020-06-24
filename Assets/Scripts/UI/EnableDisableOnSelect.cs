using UnityEngine;
using UnityEngine.EventSystems;

public class EnableDisableOnSelect : MonoBehaviour, ISelectHandler, IDeselectHandler {
    
    public GameObject target;
    
    public void OnSelect(BaseEventData eventData) {
        target.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData) {
        target.SetActive(false);        
    }
}