using UnityEngine;
using UnityEngine.EventSystems;

public class SoundOnSelect : MonoBehaviour, ISelectHandler {
    public AudioClip selectSound;

    public void OnSelect(BaseEventData eventData) { 
        SoundManager.UISound(selectSound);
    }
}