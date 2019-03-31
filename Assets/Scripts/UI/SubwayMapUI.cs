using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class SubwayMapUI : UIComponent {

    AudioSource audioSource;
    EventSystem eventSystem;

    bool firstEnable = true;

    void OnEnable() {
        if (firstEnable) {
            firstEnable = false;
            return;
        }
        audioSource = GetComponent<AudioSource>();
        eventSystem = GetComponent<EventSystem>();
    }

    public void SelectFirstChild() {
        Button b = GetComponentsInChildren<Button>().Where(
            x => x.interactable
        ).First();
        b.Select();
        b.OnSelect(new BaseEventData(eventSystem));
    }

    public void ReactToItemHover(SubwayStop stop) {
        if (audioSource != null) audioSource.PlayOneShot(audioSource.clip);
    }

    public void PropagateCurrentStopInfo(SubwayStop stop) {
        SubwayStopButton[] buttons = GetComponentsInChildren<SubwayStopButton>();
        foreach (SubwayStopButton b in buttons) {
            // reset from the last station
            b.GetComponent<Animator>().SetBool("ThisStop", false);
            if (b.stop == stop) {
                b.GetComponent<Animator>().SetBool("ThisStop", true);
            }
        }
    }
}