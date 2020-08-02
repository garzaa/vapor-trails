using UnityEngine;
using UnityEngine.Events;

public class EventOnDisable : MonoBehaviour {
    public UnityEvent unityEvent;

    void OnEnable() {
        unityEvent.Invoke();
    }
}