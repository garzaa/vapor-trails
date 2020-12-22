using UnityEngine;
using System.Collections;

public class GenericContinueListener : MonoBehaviour {

    public bool waitOneFrame = false;
    public Activatable activatable;

    void Update() {
        if (InputManager.GenericContinueInput()) {
            StartCoroutine(CallActivatable());
        }
    }

    IEnumerator CallActivatable() {
        if (waitOneFrame) {
            yield return new WaitForEndOfFrame();
        } else {
            yield return null;
        }
        activatable.Activate();
    }
}
