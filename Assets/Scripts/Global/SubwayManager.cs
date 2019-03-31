using UnityEngine;

public class SubwayManager : MonoBehaviour {
    public static SubwayManager sm;
    public static SubwayMapUI mapUI;

    static Vector2 offset;

    void Start() {
        sm = this;
        mapUI = Object.FindObjectOfType<SubwayMapUI>();
        CloseMapUI();
    }

    public static void Depart() {

    }

    public static void Arrive() {

    }

    public static void OpenMapUI() {
        GlobalController.pc.EnterDialogue();
        mapUI.gameObject.SetActive(true);
    }

    public static void CloseMapUI() {
        GlobalController.pc.ExitDialogue();
        mapUI.gameObject.SetActive(false);
    }

    public static void ReactToStationSelect(SubwayStop stop) {
        print(stop);
    }
}