using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubwayStopButton : MonoBehaviour, ISelectHandler {
    public SubwayStop stop;
    SubwayMapUI mapUI;
    Animator anim;

    public GameFlag requiredGameFlag = GameFlag.None;
    public string originalText;

    void Start() {
        mapUI = GetComponentInParent<SubwayMapUI>();
        GetComponent<Button>().onClick.AddListener(OnClick);
        originalText = GetComponentInChildren<Text>().text;
        anim = GetComponent<Animator>();
        CheckDiscovery();
    }

    public void CheckDiscovery() {
        if (requiredGameFlag != GameFlag.None && !GlobalController.HasFlag(requiredGameFlag)) {
            GetComponentInChildren<Text>().text = "???";
            anim.SetBool("Interactable", false);
        } else {
            GetComponentInChildren<Text>().text = originalText;
            anim.SetBool("Interactable", true);
        }
    }

    void OnClick() {
        SubwayManager.ReactToStationSelect(this.stop);
    }

    public void OnSelect(BaseEventData eventData) {
        mapUI.ReactToItemHover(this.stop);
    }
}