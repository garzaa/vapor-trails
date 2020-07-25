using UnityEngine;

public class EventOnEnable : MonoBehaviour {
    public GameEvent gameEvent;

    void OnEnable() {
        gameEvent.Raise();
    }
}