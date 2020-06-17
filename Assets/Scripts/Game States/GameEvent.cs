using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class GameEvent : ScriptableObject {
    List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise() {
        foreach (GameEventListener listener in listeners) {
            listener.OnEventRaised();
        }
    }

    public void Register(GameEventListener listener) {
        listeners.Add(listener);
    }

    public void Deregister(GameEventListener listener) {
        listeners.Remove(listener);
    }
}