using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EnableDisableOnSubmit : MonoBehaviour,
    ISubmitHandler,
    IDeselectHandler,
    ISelectHandler,
    IPointerDownHandler {
    
    public GameObject target;

    public bool onDeselect;
    public bool onSelect;
    public bool onDisable = true;
    public AudioClip sound;
    
    public void OnSubmit(BaseEventData eventData) {
        UISound();
        target.SetActive(!target.activeSelf);
    }

    public void OnSelect(BaseEventData eventData) {
        if (onSelect) {
            UISound();
            target.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        if (onDeselect){
            UISound();
            target.SetActive(false);   
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        UISound();
        target.SetActive(!target.activeSelf);
    }

    public void OnDisable() {
        if (onDisable) target.SetActive(false);
    }

    void UISound() {
        if (sound != null) SoundManager.UISound(sound);
    }
}