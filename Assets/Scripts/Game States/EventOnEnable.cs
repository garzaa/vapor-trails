using UnityEngine;

public class EventOnEnable : MonoBehaviour {
    [SerializeField] GameEvent gameEvent;

    void OnEnable() {
        gameEvent.Raise();
    }
}