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

    string exitNodeName = "Idle";

    public void Initialize(Animator anim, AttackBuffer buffer) {
        this.anim = anim;
        this.buffer = buffer;
}

    public void EnterGraph() {
        currentNode = GetRootNode();
        currentNode.OnNodeEnter();
        exitNodeName = (currentNode as InitialBranchNode).exitNode;
    }

    public void ExitGraph() {
        currentNode.OnNodeExit();
        currentNode = null;
        anim.Play(exitNodeName, 0);
        GlobalController.pc.ExitAttackGraph(); // bad
    }

    public void Update() {
        // assume there aren't any blend states on the animator
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(layerIndex:0);
        // if for some reason the current state has no animation in it
        clipLength = (clipInfo.Length > 0 ? clipInfo[0].clip.length : 0.25f);
        clipTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        currentFrame = (int) ((clipTime * clipLength) * 16f);

        if (buffer.ready && (currentFrame>=currentNode.IASA || currentNode.cancelable)) {
            MoveNextNode();
        }
        else if (currentFrame>=currentNode.IASA && InputManager.HasHorizontalInput()) {
            ExitGraph();
        }
        else if (clipTime >= 1) {
            ExitGraph();
        }
    }

    void MoveNextNode() {
        AttackNode next = currentNode.GetNextAttack(buffer);
        if (next != null) {
            MoveNode(next);
        }
    }

    void MoveNode(AttackNode node) {
        buffer.Clear();

        currentNode.OnNodeExit();
        currentNode = node;
        currentNode.OnNodeEnter();

        anim.Play(GetStateName(currentNode), 0, 0);
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
        currentNode.cancelable = true;
    }

}