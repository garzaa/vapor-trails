using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubwayStopButton : MonoBehaviour, ISelectHandler {
    public SubwayStop stop;
    SubwayMapUI mapUI;
    Animator anim;

    public GameState requiredState;
    bool startedBefore = false;

    string originalText;
    bool discovered;

    public bool IsDiscovered() {
        return discovered;
    }

    void Start() {
        mapUI = GetComponentInParent<SubwayMapUI>();
        GetComponent<Button>().onClick.AddListener(OnClick);
        originalText = GetComponentInChildren<Text>().text;
        anim = GetComponent<Animator>();
        startedBefore = true;
        CheckDiscovery();
    }

    public void CheckDiscovery() {
        if (!startedBefore) {
            Start();
        }
        anim = anim ?? GetComponent<Animator>();
        if (requiredState!=null && !GlobalController.HasState(requiredState)) {
            GetComponentInChildren<Text>().text = "???";
            // anim.SetBool("Interactable", false);
            discovered = false;
        } else {
            GetComponentInChildren<Text>().text = originalText;
            // anim.SetBool("Interactable", true);
            discovered = true;
        }
    }

    void OnClick() {
        SubwayManager.ReactToStationSelect(this);
    }

    public void OnSelect(BaseEventData eventData) {
        if (mapUI == null) mapUI = GetComponentInParent<SubwayMapUI>();
        mapUI.ReactToItemHover(this.stop);
    }
}
