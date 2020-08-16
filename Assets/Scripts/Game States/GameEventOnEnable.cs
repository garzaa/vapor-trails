using UnityEngine;

public class GameEventOnEnable : MonoBehaviour {
    public GameEvent gameEvent;

    void OnEnable() {
        gameEvent.Raise();
    }
}