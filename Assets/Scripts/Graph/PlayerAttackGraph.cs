using UnityEngine;
using XNode;

public class PlayerAttackGraph : NodeGraph {
    AttackBuffer buffer;
    const int attackFramerate = 16;
    bool active;

    Animator anim;
    float clipTime;
    float clipLength;
    int currentFrame; // 16 fps

    AttackNode currentNode = null;

    public void Initialize(Animator anim, AttackBuffer buffer) {
        this.anim = anim;
        this.buffer = buffer;
        active = false;
    }

    public void EnterGraph() {
        Debug.Log("Entering Graph internally");
        currentNode = GetRootNode();
        currentNode.OnNodeEnter();
        active = true;
    }

    public void ExitGraph() {
        currentNode.OnNodeExit();
        currentNode = null;
        active = false;
        Debug.Log("Exiting graph internally");
    }

    public void Update() {
        // if player is out of the attack graph
        if (!IsActive()) return;

        // assume there aren't any blend states on the animator
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(layerIndex:0);
        clipLength = (clipInfo.Length > 0 ? clipInfo[0].clip.length : 1);
        clipTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        currentFrame = (int) ((clipTime/clipLength) * 16f);

        Debug.Log("Buffer ready: "+buffer.ready);
        Debug.Log(currentFrame);

        if (buffer.ready && (currentNode.IASA==0 || currentFrame>=currentNode.IASA)) {
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
        Debug.Log("Switching node from "+currentNode.attackName);
        buffer.Clear();

        currentNode.OnNodeExit();
        currentNode = node;
        currentNode.OnNodeEnter();

        anim.Play(GetStateName(currentNode));
        Debug.Log("Playing animation "+GetStateName(currentNode));
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

    public bool IsActive() {
        return active;
    }

    public void OnAttackLand() {
        MoveNextNode();
    }

}