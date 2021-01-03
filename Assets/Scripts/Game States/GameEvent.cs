using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class GameEvent : ScriptableObject {
    List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise() {
        for (int i=0; i<listeners.Count; i++) {
            listeners[i].OnEventRaised();
        }
    }

    public void Register(GameEventListener listener) {
        listeners.Add(listener);
    }

    public void Deregister(GameEventListener listener) {
        listeners.Remove(listener);
    }
}