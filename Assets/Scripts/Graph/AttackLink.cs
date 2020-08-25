[System.Serializable]
public class AttackLink {
    [NodeEnum]
    public AttackType type = AttackType.NONE;
    [NodeEnum]
    public AttackDirection direction = AttackDirection.ANY;

    public AttackLink(AttackType type, AttackDirection direction) {
        this.type = type;
        this.direction = direction;
    }
}