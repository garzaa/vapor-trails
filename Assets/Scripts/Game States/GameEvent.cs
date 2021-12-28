using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Game Event", menuName = "Data Containers/Game Event")]
public class GameEvent : ScriptableObject {

    List<GameEventListener> listeners = new List<GameEventListener>();
    List<GameEventListener> priorityListeners = new List<GameEventListener>();

    public void Raise() {
        for (int i=0; i<priorityListeners.Count; i++) {
            priorityListeners[i].OnEventRaised();
        }
        for (int i=0; i<listeners.Count; i++) {
            listeners[i].OnEventRaised();
        }
    }

    public void Register(GameEventListener listener) {
        if (listener.priority) {
            priorityListeners.Add(listener);
        } else {
            listeners.Add(listener);
        }
    }

    public void Deregister(GameEventListener listener) {
        if (listener.priority) {
            priorityListeners.Remove(listener);
        } else {
            listeners.Remove(listener);
        }
    }
}
