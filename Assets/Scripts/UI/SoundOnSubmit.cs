using UnityEngine;
using UnityEngine.EventSystems;

public class SoundOnSubmit : MonoBehaviour, ISubmitHandler {
    public AudioClip sound;

    public void OnSubmit(BaseEventData eventData) {
        SoundManager.UISound(sound);
    }
}