using UnityEngine;
using XNode;

public class PlayerAttackGraph : NodeGraph {
    AttackBuffer buffer;
    const int attackFramerate = 16;

    Animator anim;
    float clipTime;
    float clipLength;
    int currentFrame; // 16 fps

    AttackNode currentNode = null;

    public void Initialize(Animator anim, AttackBuffer buffer) {
        this.anim = anim;
        this.buffer = buffer;
    }

    public void EnterGraph() {
        currentNode = GetRootNode();
        currentNode.OnNodeEnter();
    }

    public void ExitGraph() {
        currentNode.OnNodeExit();
        currentNode = null;
        GlobalController.pc.ExitAttackGraph(); // bad
    }

    public void Update() {
        // assume there aren't any blend states on the animator
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(layerIndex:0);
        clipLength = (clipInfo.Length > 0 ? clipInfo[0].clip.length : 1);
        clipTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        currentFrame = (int) ((clipTime/clipLength) * 16f);

        if (buffer.ready && currentFrame>=currentNode.IASA) {
            MoveNextNode();
        }

        if (clipTime/clipLength > 1) {
            ExitGraph();
        }
    }

    void MoveNextNode() {
        AttackNode next = currentNode.GetNextAttack(buffer);
        if (next != null) {
            MoveNode(next);
            return;
        }
    }

    void MoveNode(AttackNode node) {
        buffer.Clear();

        currentNode.OnNodeExit();
        currentNode = node;
        currentNode.OnNodeEnter();

        anim.Play(GetStateName(currentNode));
    }

    AttackNode GetRootNode() {
        foreach (Node i in nodes) {
            if (i is InitialBranchNode) return i as AttackNode;
        }
        return null;
    }

    string GetStateName(AttackNode node) {
        return node.attackName;
    }

    public void OnAttackLand() {
        MoveNextNode();
    }

}