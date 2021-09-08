using UnityEngine;
using Rewired;

public class InputManager : MonoBehaviour {

    static Player rewiredPlayer = null;

    static InputManager im;

    static bool polling = false;

    public static string ControllerLastInput() {
        return rewiredPlayer.controllers.GetLastActiveController().hardwareName;
    }

    void Start() {
        im = this;
        rewiredPlayer = ReInput.players.GetPlayer(0);
    }

    void FixedUpdate() {
        if (rewiredPlayer != null) Debug.Log(ControllerLastInput());
    }

    public static bool HasHorizontalInput() {
        return Mathf.Abs(rewiredPlayer.GetAxis(Buttons.H_AXIS)) > 0.01f;
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
        return GlobalController.save.options.inputBuffer * (1f/16f);
    }
}
