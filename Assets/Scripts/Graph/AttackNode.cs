using UnityEngine;
using XNode;

[NodeWidth(250)]
public class AttackNode : Node {
    public string attackName;
    public int damage = 1;
    public int IASA;

    [Output(dynamicPortList=true)] public AttackLink[] links;
    [Input] public AttackLink input;

    private void Reset() {
        this.name = attackName;
    }
}