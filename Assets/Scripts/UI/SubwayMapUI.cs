using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubwayMapUI : UIComponent {

    AudioSource audioSource;
    EventSystem eventSystem;

    bool firstEnable = true;

    void OnEnable() {
        if (firstEnable) {
            firstEnable = false;
            return;
        }
        SelectFirstChild();
        audioSource = GetComponent<AudioSource>();
        eventSystem = GetComponent<EventSystem>();
    }

    void SelectFirstChild() {
        Button b = GetComponentsInChildren<Button>()[0];
        b.Select();
        b.OnSelect(new BaseEventData(eventSystem));
    }

    public void ReactToItemHover(SubwayStop stop) {
        if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }
}