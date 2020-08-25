using UnityEngine;
using Rewired;

public class InputManager : MonoBehaviour {

    static Player rewiredPlayer;

    static readonly float INPUT_TOLERANCE = 0.2f;

    public static bool dpadUp;
    public static bool dpadDown;
    public static bool dpadLeft;
    public static bool dpadRight;

    float lastX;
    float lastY;

    static InputManager im;

    static bool polling = false;

    void Start() {
        if (im == null) im = this;
        rewiredPlayer = ReInput.players.GetPlayer(0);
    }
 
    void Update() {
        dpadRight = (Input.GetAxis(Buttons.XTAUNT) == 1 && lastX != 1);
        dpadLeft = (Input.GetAxis (Buttons.XTAUNT) == -1 && lastX != -1);
        dpadUp = (Input.GetAxis (Buttons.YTAUNT) == 1 && lastY != 1);
        dpadDown = (Input.GetAxis (Buttons.YTAUNT) == -1 && lastY != -1);

        lastX = Input.GetAxis(Buttons.XTAUNT);
        lastY = Input.GetAxis(Buttons.YTAUNT);
    }

    public static bool HasHorizontalInput() {
        return Mathf.Abs(rewiredPlayer.GetAxis(Buttons.H_AXIS)) > 0.01f;
    }

    public static float HorizontalInput() {
        return rewiredPlayer.GetAxis(Buttons.H_AXIS);
    }

    public static float VerticalInput() {
        return Input.GetAxis(Buttons.V_AXIS);
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
        return dpadDown || dpadLeft || dpadRight || dpadUp;
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

    public static float GetInputBuffer() {
        return GlobalController.save.options.inputBuffer * (1f/16f);
    }
}