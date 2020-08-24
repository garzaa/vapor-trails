[System.Serializable]
public class AttackLink {
    public AttackType type;
    public AttackDirection direction;

    public AttackLink(AttackType type, AttackDirection direction) {
        this.type = type;
        this.direction = direction;
    }
}

public enum AttackDirection {
    ANY = 15,
    UP = 1,
    FORWARD = 2,
    DOWN = 4,
    BACKWARD = 8
}

public enum AttackType {
    PUNCH = 1,
    KICK = 2
}