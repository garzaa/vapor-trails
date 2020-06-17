using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameEventListener : MonoBehaviour {
    public GameEvent gameEvent;
    public UnityEvent[] responses;

    void OnEnable() {
        gameEvent.Register(this);
    }

    void OnDisable() {
        gameEvent.Deregister(this);
    }

    public void OnEventRaised() {
        foreach (UnityEvent response in responses) {
            response.Invoke();
        }
    }
}