using UnityEngine;

public class EnableOnGameState : StateChangeReactor {
    public GameState wantedState;
    public bool immediate = true;

    public bool setDisabled = false;

    override public void React(bool fakeSceneLoad) {
        if (!immediate) return;
        bool hasState = SaveManager.HasState(wantedState);

        if (setDisabled) {
            gameObject.SetActive(!hasState);
        } else {
            gameObject.SetActive(hasState);
        }
    }
}
