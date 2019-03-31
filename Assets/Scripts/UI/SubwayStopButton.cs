using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubwayStopButton : MonoBehaviour, ISelectHandler {
    public SubwayStop stop;
    SubwayMapUI mapUI;

    void Start() {
        mapUI = GetComponentInParent<SubwayMapUI>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick() {
        SubwayManager.ReactToStationSelect(this.stop);
    }

    public void OnSelect(BaseEventData eventData) {
        mapUI.ReactToItemHover(this.stop);
    }
}