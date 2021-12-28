using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName="Game Event", menuName = "Data Containers/Object Game Event")]
public class ObjectGameEvent : ScriptableObject {

    List<ObjectGameEventListener> listeners = new List<ObjectGameEventListener>();
    List<ObjectGameEventListener> priorityListeners = new List<ObjectGameEventListener>();

    public void Raise(Object eventObject) {
        for (int i=0; i<priorityListeners.Count; i++) {
            priorityListeners[i].OnEventRaised(eventObject);
        }
        for (int i=0; i<listeners.Count; i++) {
            listeners[i].OnEventRaised(eventObject);
        }
    }

    public void Register(ObjectGameEventListener listener) {
        if (listener.priority) {
            priorityListeners.Add(listener);
        } else {
            listeners.Add(listener);
        }
    }

    public void Deregister(ObjectGameEventListener listener) {
        if (listener.priority) {
            priorityListeners.Remove(listener);
        } else {
            listeners.Remove(listener);
        }
    }
}
