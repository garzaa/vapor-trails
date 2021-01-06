using UnityEngine;
using XNode;

[CreateAssetMenu]
public class PlayerAttackGraph : NodeGraph {
    const int attackFramerate = 16;

    float clipTime;
    float clipLength;

    int currentFrame;

    // expose these to nodes
    public Rigidbody2D rb2d;
    public Animator anim;
    public AttackBuffer buffer;
    public AirAttackTracker airAttackTracker;
    public PlayerController playerController;

    CombatNode currentNode = null;

    public int stateNum;
    public string exitNodeName = "Idle 100";

    public void Initialize(Animator anim, AttackBuffer buffer, Rigidbody2D rb, AirAttackTracker airAttackTracker) {
        this.anim = anim;
        this.buffer = buffer;
        this.rb2d = rb;
        this.airAttackTracker = airAttackTracker;
        this.playerController = anim.GetComponent<PlayerController>();
    }

    public void EnterGraph(Node entryNode) {
        currentNode = (entryNode == null) ? GetRootNode() : entryNode as CombatNode;
        anim.SetInteger("SubState", this.stateNum);
        currentNode.OnNodeEnter();
    }

    public void ExitGraph(bool quiet=false) {
        if (currentNode != null) {
            currentNode.OnNodeExit();
        }
        currentNode = null;
        GlobalController.pc.ExitAttackGraph(); // bad
        if (!quiet) anim.Play(exitNodeName, 0);
    }

    public void Update() {
        // assume there aren't any blend states on the animator
        AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(layerIndex:0);

        // if for some reason the current state has no animation in it
        clipLength = (clipInfo.Length > 0 ? clipInfo[0].clip.length : 0.25f);
        clipTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        currentFrame = (int) ((clipTime * clipLength) * 16f);

        currentNode.NodeUpdate(currentFrame, clipTime, buffer);

        if (anim.GetInteger("SubState") != stateNum) {
            ExitGraph(quiet:true);
        }
    }

    public void MoveNode(CombatNode node) {
        buffer.Clear();

        currentNode.OnNodeExit();
        currentNode = node;
        currentNode.OnNodeEnter();
    }
    
    int GetStateHash() {
        return anim.GetCurrentAnimatorStateInfo(layerIndex:0).fullPathHash;
    }

    CombatNode GetRootNode() {
        foreach (Node i in nodes) {
            if (i is InitialBranchNode) return i as CombatNode;
        }
        return null;
    }

    public void OnAttackLand() {
        currentNode.OnAttackLand();
    }

    public void OnGroundHit() {
        if (currentNode != null) {
            currentNode.OnGroundHit();
        }
    }

}
