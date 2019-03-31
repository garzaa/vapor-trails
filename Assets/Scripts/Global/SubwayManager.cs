using UnityEngine;

public class SubwayManager : MonoBehaviour {
    public static SubwayManager sm;
    public static SubwayMapUI mapUI;

    static Vector2 offset;

    void Start() {
        sm = this;
    }

    public static void Depart() {

    }

    public static void Arrive() {

    }

    public static void OpenMapUI() {
        mapUI.Show();
    }

    public static void CloseMapUI() {
        mapUI.Hide();
    }

    public static void ReactToStationSelect(SubwayStopButton stop) {

    }
}