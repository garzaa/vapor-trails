using System.Collections.Generic;

public static class Inputs {
    public static readonly string H_AXIS = "Horizontal";
    public static readonly string V_AXIS = "Vertical";
    public static readonly string JUMP   = "Jump";
    public static readonly string ATTACK = "Attack";

    public static readonly string SPECIAL    = "Special";
    public static readonly string PROJECTILE = "Projectile";
    public static readonly string INTERACT   = "Interact";

    public static readonly string CONFIRM = "Confirm";
    public static readonly string PAUSE   = "Pause";

    private static readonly List<string> moveInputs = new List<string> {
        H_AXIS,
        V_AXIS,
        JUMP
    };

    private static readonly List<string> actionInputs = new List<string> {
        ATTACK, 
        SPECIAL, 
        PROJECTILE,
        INTERACT
    };

    private static readonly List<string> metaInputs = new List<string>{
        CONFIRM,
        PAUSE
    };

    private static readonly Dictionary<InputType, List<string>> inputMap = new Dictionary<InputType, List<string>> {
        { InputType.MOVE, moveInputs },
        { InputType.ACTION, actionInputs },
        { InputType.META, metaInputs }
    };

    public static bool IsType(string inputName, InputType inputType) {
        return inputMap[inputType].Contains(inputName);
    }
}

public enum InputType {
    MOVE,
    ACTION,
    META
}