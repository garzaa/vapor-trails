using UnityEngine;

public class EnableOnGameState : MonoBehaviour {
    [SerializeField] GameState wantedState;
    public bool immediate = false;

    void Start() {
        CheckState();
    }

    public void CheckState() {
        gameObject.SetActive(GlobalController.HasState(wantedState));
    }
}