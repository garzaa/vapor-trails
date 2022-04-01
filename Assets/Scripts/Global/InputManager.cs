using UnityEngine;
using System.Collections.Generic;
using Rewired;

public class InputManager : MonoBehaviour {

    static Player rewiredPlayer = null;
    static InputManager im;
    static bool polling = false;

    public Vector2 ls;

    [System.Serializable]
    public class KeyImage {
        public string keyName;
        public Sprite glyph;
    }

    // TODO: make this a string-sprite pairing and use a hashmap for acceleration
    public List<KeyImage> keyImages;

    void Update() {
        ls = InputManager.LeftStick();
    }

    public static Sprite GetGlyph(ActionName action) {
        // given an action name like "down"
        // get the controller
        Controller currentController = rewiredPlayer.controllers.GetLastActiveController();
        if (currentController == null) {
            if (rewiredPlayer.controllers.joystickCount > 0) {
                currentController = rewiredPlayer.controllers.Joysticks[0];
            } else {
                currentController = rewiredPlayer.controllers.Keyboard;
            }
        }

        // find the controller button name that corresponds to the action
        // https://guavaman.com/projects/rewired/docs/HowTos.html#display-glyph-for-action
        ActionElementMap aem = rewiredPlayer.controllers.maps.GetFirstElementMapWithAction(currentController, action.actionId, true);
        if (aem == null) {
            Debug.LogWarning("Nothing mapped on current controller for action "+action.name);
            return null;
        }
        Debug.Log(aem.actionDescriptiveName);

        if (currentController.type == ControllerType.Joystick) {
            Debug.Log("Getting glyph for "+currentController.hardwareName+" at element id "+aem.elementIdentifierId+" with range "+aem.axisRange);
            return ControllerGlyphs.GetGlyph(currentController.hardwareTypeGuid, aem.elementIdentifierId, aem.axisRange);
        }

        // just draw UI with the button text
        return null;
    }

    public static string GetLastControllerName() {
        Rewired.Controller c = rewiredPlayer.controllers.GetLastActiveController();
        if (c != null) {
            return c.hardwareName;
        } else {
            // this will happen if there's no input at all
            return "keyboard";
        }
    }

    void OnEnable() {
        im = this;
        rewiredPlayer = ReInput.players.GetPlayer(0);
    }

    public static bool HasHorizontalInput() {
        return HorizontalInput() != 0;
    }

    public static float HorizontalInput() {
        return rewiredPlayer.GetAxis(Buttons.H_AXIS);
    }

    public static Vector2 UINav() {
        return new Vector2(
            rewiredPlayer.GetAxis(Buttons.UI_X),
            rewiredPlayer.GetAxis(Buttons.UI_Y)
        );
    }

    public static float VerticalInput() {
        return rewiredPlayer.GetAxis(Buttons.V_AXIS);
    }

    public static bool ButtonDown(string buttonName) {
        return !polling && rewiredPlayer.GetButtonDown(buttonName);
    }

    public static bool Button(string buttonName) {
        return !polling && rewiredPlayer.GetButton(buttonName);
    }

    public static bool ButtonUp(string buttonName) {
        return !polling && rewiredPlayer.GetButtonUp(buttonName);
    }

    public static bool GenericContinueInput() {
        return (
            ButtonDown(Buttons.JUMP) || GenericEscapeInput() || AttackInput()
        );
    }

    public static bool AttackInput() {
        return (
            ButtonDown(Buttons.PUNCH)
            || ButtonDown(Buttons.KICK)
        );
    }

    public static bool GenericEscapeInput() {
        return (
            InputManager.ButtonDown(Buttons.SPECIAL)
            || InputManager.ButtonDown(Buttons.INVENTORY)
            || InputManager.Button(Buttons.PAUSE)
            || InputManager.ButtonDown(Buttons.UI_CANCEL)
        );
    }

    public static Vector2 RightStick() {
        return new Vector2(
            rewiredPlayer.GetAxis("Right-Horizontal"),
            rewiredPlayer.GetAxis("Right-Vertical")
        );
    }

    public static Vector2 LeftStick() {
        return new Vector2(
            rewiredPlayer.GetAxis(Buttons.H_AXIS),
            rewiredPlayer.GetAxis(Buttons.V_AXIS)
        );
    }

    public static bool TauntInput() {
        return false;
    }

    public static Vector2 MoveVector() {
        return new Vector2(HorizontalInput(), VerticalInput());
    }

    public void OnButtonPollStart() {
        InputManager.polling = true;    
    }

    public void OnButtonPollEnd() {
        InputManager.polling = false;
    }

    public static float GetInputBufferDuration() {
        return SaveManager.save.options.inputBuffer * (1f/16f);
    }
}
