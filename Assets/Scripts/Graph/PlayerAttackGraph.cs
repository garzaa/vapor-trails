using UnityEngine;
using XNode;

public class PlayerAttackGraph : NodeGraph {
    const float buffer = 0.1f;
    const int attackFramerate = 16;

    Animator anim;
    float clipTime;
    float clipLength;
    int currentFrame; // 16 fps

    AttackNode currentNode;
    AttackLink bufferedInput;

    public void Initialize(Animator animator) {
        anim = animator;
    }

    public void Enter() {
        currentNode = GetRootNode();
    }

    public void Exit() {
        currentNode = null;
        bufferedInput = null;
    }

    public void Update() {
        // if player is out of the attack graph
        if (currentNode == null) return;

        clipLength = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        clipTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        currentFrame = (int) ((clipTime/clipLength) * 16f);        

        if (bufferedInput != null && currentFrame >= currentNode.IASA) {
            // if possible, consume input and switch nodes
        }
        
        // grab and buffer inputs

    }

    void UpdateInputs() {
        bool punch = Input.GetButtonDown(Buttons.PUNCH);
        bool kick = Input.GetButtonDown(Buttons.KICK);
        if (punch | kick) {
            bufferedInput = new AttackLink(
                AttackType.PUNCH,
                AttackDirection.FORWARD
            );
        }
    }

    void ClearBuffer() {

    }

    void SwitchNode(AttackNode node) {
        bufferedInput = null;
        anim.Play(GetAnimationName(node));
    }

    AttackNode GetRootNode() {
        foreach (Node i in nodes) {
            if (i is InitialBranchNode) return i as AttackNode;
        }
        return null;
    }

    string GetAnimationName(AttackNode node) {
        return "Player"+node.attackName.Replace(" ", "");
    }

}