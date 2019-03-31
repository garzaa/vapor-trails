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

    public static void OpenMapUI(LocalSubwayController lc) {
        GlobalController.pc.EnterDialogue();
        mapUI.gameObject.SetActive(true);
        print(lc.stop);
        mapUI.PropagateCurrentStopInfo(lc.stop);
        mapUI.SelectFirstChild();
    }

    public static void CloseMapUI() {
        GlobalController.pc.ExitDialogue();
        mapUI.gameObject.SetActive(false);
    }

    public static void ReactToStationSelect(SubwayStop stop) {
        LocalSubwayController lc = Object.FindObjectOfType<LocalSubwayController>();
        if (stop == lc.stop) return;
        CloseMapUI();
        lc.GetComponent<Animator>().SetTrigger("Depart");
    }
}