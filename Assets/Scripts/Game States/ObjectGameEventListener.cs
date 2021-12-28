using UnityEngine;

public class ObjectGameEventListener : MonoBehaviour {
    public ObjectGameEvent gameEvent;
    public bool priority;
    public ObjectEvent response;

    void OnEnable() {
        gameEvent.Register(this);
    }

    void OnDisable() {
        gameEvent.Deregister(this);
    }

    public void OnEventRaised(Object eventObject) {
        response.Invoke(eventObject);
    }
}
