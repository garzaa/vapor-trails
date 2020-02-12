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
    static bool firstClose;

    public List<StationSceneMapping> stationMappings;

    void Start() {
        if (sm == null) sm = this;
        mapUI = GlobalController.gc.GetComponentInChildren<SubwayMapUI>(true);
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
        localSubway = Object.FindObjectOfType<LocalSubwayController>();
        GlobalController.MovePlayerTo(localSubway.transform.position + playerOffset);
        localSubway.HidePlayer();
        localSubway.OffsetPlayerFollower(playerOffset);
        localSubway.GetComponent<Animator>().SetTrigger("Arrive");
    }

    public static void SetPlayerOffset(Vector2 newOffset) {
        playerOffset = newOffset;
    }

    public static void OpenMapUI(LocalSubwayController lc) {
        GlobalController.pc.EnterCutscene();
        localSubway = lc;
        mapUI.gameObject.SetActive(true);
        mapUI.UpdateDiscoveredStops();
        mapUI.PropagateCurrentStopInfo(lc.stop);
        mapUI.SelectFirstChild();
    }

    public static void CloseMapUI() {
        mapUI.gameObject.SetActive(false);
    }

    public static void ReactToStationSelect(SubwayStop stop) {
        if (stop == localSubway.stop) return;
        destination = stop;
        CloseMapUI();
        localSubway.GetComponent<Animator>().SetTrigger("Depart");
    }
}