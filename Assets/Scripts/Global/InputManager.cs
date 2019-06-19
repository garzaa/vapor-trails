using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    static readonly float INPUT_TOLERANCE = 0.1f;

    static bool frozenInputs = false;

    public static bool HasHorizontalInput() {
        return frozenInputs ? false : Mathf.Abs(Input.GetAxis(Inputs.H_AXIS)) > INPUT_TOLERANCE;
    }

    public static float HorizontalInput() {
        return frozenInputs ? 0 : Input.GetAxis(Inputs.H_AXIS);
    }

    public static float VerticalInput() {
        return frozenInputs ? 0 : Input.GetAxis(Inputs.V_AXIS);
    }

    public static bool ButtonDown(string buttonName) {
        if (
            frozenInputs
            && (
                Inputs.IsType(buttonName, InputType.MOVE)
                || Inputs.IsType(buttonName, InputType.ACTION)
            )
        ) {
            return false;
        }
        return Input.GetButtonDown(buttonName);
    }

    public static void FreezeInputs() {
        frozenInputs = true;
    }

    public static void UnfreezeInputs() {
        frozenInputs = false;
    }
}