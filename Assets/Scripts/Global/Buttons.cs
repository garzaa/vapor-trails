using System.Collections.Generic;

public static class Buttons {
    public static readonly string H_AXIS = "Horizontal";
    public static readonly string V_AXIS = "Vertical";
    public static readonly string JUMP   = "Jump";
    public static readonly string ATTACK = "Attack";
    public static readonly string PUNCH  = "Punch";
    public static readonly string KICK   = "Kick";
    public static readonly string XTAUNT = "Horizontal Taunt";
    public static readonly string YTAUNT = "Vertical Taunt";
    public static readonly string INVENTORY = "Inventory";

    public static readonly string SPECIAL    = "Special";
    public static readonly string PROJECTILE = "Projectile";
    public static readonly string INTERACT   = "Interact";
    public static readonly string BLOCK      = "Block";
    public static readonly string SURF       = "Surf";

    public static readonly string CONFIRM = "Confirm";
    public static readonly string PAUSE   = "Start";

    private static readonly List<string> moveInputs = new List<string> {
        H_AXIS,
        V_AXIS,
        JUMP
    };

    private static readonly List<string> actionInputs = new List<string> {
        ATTACK,
        PUNCH,
        KICK,
        SPECIAL, 
        PROJECTILE,
        INTERACT,
        BLOCK,
        XTAUNT,
        YTAUNT
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