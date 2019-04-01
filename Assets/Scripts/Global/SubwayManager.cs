using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SubwayManager : MonoBehaviour {
    public static SubwayManager sm;
    public static SubwayMapUI mapUI;

    static LocalSubwayController localSubway;
    static Vector3 playerOffset;
    static SubwayStop destination;
    public static bool playerOnSubway;

    public List<StationSceneMapping> stationMappings;

    void Start() {
        sm = this;
        mapUI = Object.FindObjectOfType<SubwayMapUI>();
        CloseMapUI();
    }

    public static void DepartWithPlayer() {
        playerOnSubway = true;
        GlobalController.LoadScene(GetStopScene(destination));
    }

    public static string GetStopScene(SubwayStop stop) {
        return sm.stationMappings.Where(
            x => x.stop == stop
        ).First().scene.SceneName;
    }

    public static void ArriveWithPlayer() {
        LocalSubwayController lc = Object.FindObjectOfType<LocalSubwayController>();
        GlobalController.MovePlayerTo(lc.transform.position + playerOffset);
        lc.HidePlayer();
        lc.OffsetPlayerFollower(playerOffset);
        lc.GetComponent<Animator>().SetTrigger("Arrive");
    }

    public static void SetPlayerOffset(Vector2 newOffset) {
        playerOffset = newOffset;
    }

    public static void OpenMapUI(LocalSubwayController lc) {
        GlobalController.pc.EnterDialogue();
        localSubway = lc;
        mapUI.gameObject.SetActive(true);
        mapUI.UpdateDiscoveredStops();
        mapUI.PropagateCurrentStopInfo(lc.stop);
        mapUI.SelectFirstChild();
    }

    public static void CloseMapUI() {
        GlobalController.pc.ExitDialogue();
        mapUI.gameObject.SetActive(false);
    }

    public static void ReactToStationSelect(SubwayStop stop) {
        if (stop == localSubway.stop) return;
        CloseMapUI();
        localSubway.GetComponent<Animator>().SetTrigger("Depart");
    }
}