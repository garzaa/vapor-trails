using UnityEngine;

public class GameEventOnDisable : MonoBehaviour {
    public GameEvent gameEvent;

    void OnDisable() {
        gameEvent.Raise();
    }
}