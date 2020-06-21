using UnityEngine;

public class InputManager : MonoBehaviour {

    static readonly float INPUT_TOLERANCE = 0.2f;

    static bool frozenInputs = false;

    private static bool CheckFrozenInputs(string buttonName) {
        return !(frozenInputs
            && (
                Buttons.IsType(buttonName, InputType.MOVE)
                || Buttons.IsType(buttonName, InputType.ACTION)
            ));
    }

    public static bool dpadUp;
    public static bool dpadDown;
    public static bool dpadLeft;
    public static bool dpadRight;

    float lastX;
    float lastY;
 
    void Update() {
        dpadRight = (Input.GetAxis(Buttons.XTAUNT) == 1 && lastX != 1);
        dpadLeft = (Input.GetAxis (Buttons.XTAUNT) == -1 && lastX != -1);
        dpadUp = (Input.GetAxis (Buttons.YTAUNT) == 1 && lastY != 1);
        dpadDown = (Input.GetAxis (Buttons.YTAUNT) == -1 && lastY != -1);

        lastX = Input.GetAxis(Buttons.XTAUNT);
        lastY = Input.GetAxis(Buttons.YTAUNT);
    }

    public static bool HasHorizontalInput() {
        return frozenInputs ? false : Mathf.Abs(Input.GetAxis(Buttons.H_AXIS)) > INPUT_TOLERANCE/4f;
    }

    public static float HorizontalInput() {
        return frozenInputs ? 0 : Input.GetAxis(Buttons.H_AXIS);
    }

    public static float VerticalInput() {
        return frozenInputs ? 0 : Input.GetAxis(Buttons.V_AXIS);
    }

    public static bool BlockInput() {
        return frozenInputs ? false : Input.GetAxis(Buttons.BLOCK) > 0.5f;
    }

    public static bool ButtonDown(string buttonName) {
        return CheckFrozenInputs(buttonName) && Input.GetButtonDown(buttonName);
    }

    public static bool Button(string buttonName) {
        return CheckFrozenInputs(buttonName) && Input.GetButton(buttonName);
    }

    public static bool ButtonUp(string buttonName) {
        return CheckFrozenInputs(buttonName) && Input.GetButtonUp(buttonName);
    }

    public static void FreezeInputs() {
        frozenInputs = true;
    }

    public static void UnfreezeInputs() {
        frozenInputs = false;
    }

    public static bool GenericContinueInput() {
        return (
            Input.GetButtonDown(Buttons.JUMP) || GenericEscapeInput() || AttackInput()
        );
    }

    public static bool AttackInput() {
        return (
            Input.GetButtonDown(Buttons.PUNCH)
            || Input.GetButtonDown(Buttons.KICK)
        );
    }

    public static bool GenericEscapeInput() {
        return (
            Input.GetButtonDown(Buttons.SPECIAL)
            || Input.GetButtonDown(Buttons.INVENTORY)
        );
    }

    public static Vector2 RightStick() {
        // cant push all the way to the edge on a joystick for some reason
        return new Vector2(
            Input.GetAxis("Right-Horizontal"),
            -Input.GetAxis("Right-Vertical")
        ).normalized;
    }

    public static bool TauntInput() {
        return dpadDown || dpadLeft || dpadRight || dpadUp;
    }

    public static Vector2 MoveVector() {
        return new Vector2(HorizontalInput(), VerticalInput());
    }
}