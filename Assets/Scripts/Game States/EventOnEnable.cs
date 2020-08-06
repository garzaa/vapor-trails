using UnityEngine;
using UnityEngine.Events;

public class EventOnEnable : MonoBehaviour {
    public GameEvent gameEvent;
    public UnityEvent unityEvent;
    public bool waitForStart;

    bool started;

    void Start() {
        started = true;
        if (waitForStart) OnEnable();
    }

    void OnEnable() {
        if (waitForStart && !started) return;
    
        if (gameEvent != null) gameEvent.Raise();
        if (unityEvent != null) unityEvent.Invoke();
    }
}