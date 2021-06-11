using UnityEngine;
using UnityEngine.Events;

public class EventOnPlayerEnter : PlayerTriggeredObject {
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public override void OnPlayerEnter() {
        onEnter.Invoke();
    }

    public override void OnPlayerExit() {
        onExit.Invoke();
    }
}
