using UnityEngine;

public class EnableOnGameState : MonoBehaviour {
    [SerializeField] GameState wantedState;
    public bool immediate = false;

    public bool setDisabled = false;

    void Start() {
        CheckState();
    }

    public void CheckState() {
        if (GlobalController.HasState(wantedState)) gameObject.SetActive(setDisabled);
        else gameObject.SetActive(!setDisabled);
    }
}