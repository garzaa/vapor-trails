using UnityEngine;
using UnityEngine.Events;

public class EventOnDisable : MonoBehaviour {
    public UnityEvent unityEvent;
    public bool skipFirstDisable;

    bool firstDisable = true;

    void OnDisable() {
        if (!(skipFirstDisable && firstDisable)) unityEvent.Invoke();
        firstDisable = false;
    }
}