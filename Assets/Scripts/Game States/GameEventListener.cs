using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class GameEventListener : MonoBehaviour {
    public GameEvent gameEvent;
    public bool priority;
    public UnityEvent response;

    void OnEnable() {
        gameEvent.Register(this);
    }

    void OnDisable() {
        gameEvent.Deregister(this);
    }

    public void OnEventRaised() {
        response.Invoke();
    }
}
