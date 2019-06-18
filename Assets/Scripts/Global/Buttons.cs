public class Inputs {
    public static readonly string H_AXIS = "Horizontal";
    public static readonly string V_AXIS = "VERTICAL";
    public static readonly string JUMP = "Jump";
    public static readonly string ATTACK = "Attack";

    public static readonly string SPECIAL = "Special";
    public static readonly string PROJECTILE = "Projectile";
    public static readonly string INTERACT = "Interact";

    public static readonly string CONFIRM = "Confirm";
    public static readonly string PAUSE = "Pause";

    private string[] moveInputs = {
        H_AXIS,
        V_AXIS,
        JUMP
    };

    private string[] actionInputs = {
        ATTACK, 
        SPECIAL, 
        PROJECTILE,
        INTERACT
    };

    private string[] metaInputs = {

    };
}

public enum InputType {
    MOVE,
    ACTION,
    META
}