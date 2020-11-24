using UnityEngine;

public class GenericContinueListener : MonoBehaviour {
    public Activatable activatable;

    void Update() {
        if (InputManager.GenericContinueInput()) {
            activatable.ActivateSwitch(true);
        }
    }
}