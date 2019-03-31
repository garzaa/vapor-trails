using UnityEngine;

public class SubwayManager : MonoBehaviour {
    public static SubwayManager sm;
    public static SubwayMapUI mapUI;

    static Vector3 playerOffset;

    void Start() {
        sm = this;
        mapUI = Object.FindObjectOfType<SubwayMapUI>();
        CloseMapUI();
    }

    public static void DepartWithPlayer() {

    }

    public static void ArriveWithPlayer() {
        LocalSubwayController lc = Object.FindObjectOfType<LocalSubwayController>();
        GlobalController.MovePlayerTo(lc.transform.position + playerOffset);
    }

    public static void SetPlayerOffset(Vector2 newOffset) {
        playerOffset = newOffset;
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
        LocalSubwayController lc = Object.FindObjectOfType<LocalSubwayController>();
        CloseMapUI();
        lc.GetComponent<Animator>().SetTrigger("Depart");
    }
}