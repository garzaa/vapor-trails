[System.Serializable]
public class AttackLink {
    public AttackType type = AttackType.NONE;
    public AttackDirection direction = AttackDirection.ANY;

    public AttackLink(AttackType type, AttackDirection direction) {
        this.type = type;
        this.direction = direction;
    }
}