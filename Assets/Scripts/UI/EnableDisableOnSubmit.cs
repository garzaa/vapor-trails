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
    public AudioClip sound;
    
    public void OnSubmit(BaseEventData eventData) {
        PlaySound();
        target.SetActive(!target.activeSelf);
    }

    public void OnSelect(BaseEventData eventData) {
        if (onSelect) {
            PlaySound();
            target.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        if (onDeselect){
            PlaySound();
            target.SetActive(false);   
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        PlaySound();
        target.SetActive(!target.activeSelf);
    }

    public void OnDisable() {
        target.SetActive(false);
    }

    void PlaySound() {
        if (sound != null) SoundManager.PlaySound(sound);
    }
}