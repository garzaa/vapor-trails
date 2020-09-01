using System.Collections.Generic;
using System;
using UnityEngine;
using XNode;

/* 
#if UNITY_EDITOR
using XNodeEditor;
using UnityEditor;
#endif
*/

[NodeWidth(270)]
public class AttackNode : CombatNode {
    public string attackName;
    public int IASA = 7;

    [Input(backingValue=ShowBackingValue.Never)] 
    public AttackLink input;

    [Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
    public AttackLink[] links;

    List<Tuple<AttackLink, CombatNode>> directionalLinks = new List<Tuple<AttackLink, CombatNode>>();
    CombatNode anyDirectionNode = null;

    [HideInInspector]
    public float timeOffset = 0;

    virtual public CombatNode GetNextNode(AttackBuffer buffer) {
        return MatchAttackNode(buffer, this.links);
    }

    override public void NodeUpdate(int currentFrame, float clipTime, AttackBuffer buffer) {
        if (buffer.ready && (currentFrame>=IASA || cancelable)) {
            MoveNextNode(buffer);
        } else if (currentFrame>=IASA && InputManager.HasHorizontalInput()) {
            attackGraph.ExitGraph();
        } else if (clipTime >= 1) {
            attackGraph.ExitGraph();
        }
    }
 
    protected void MoveNextNode(AttackBuffer buffer) {
        CombatNode next = GetNextNode(buffer);
        if (next != null) {
            attackGraph.MoveNode(next);
        }
    }

    // directional attacks are prioritized in order, then the first any-directional link is used
    protected CombatNode MatchAttackNode(AttackBuffer buffer, AttackLink[] attackLinks, string portListName="links") {
        directionalLinks.Clear();
        anyDirectionNode = null;

        for (int i=0; i<attackLinks.Length; i++) {
            AttackLink link = attackLinks[i];
            if (link.type==buffer.type && buffer.HasDirection(link.direction)) {
                CombatNode next = GetPort(portListName+" "+i).Connection.node as CombatNode;
                if (next.Enabled()) {
                    if (anyDirectionNode==null && link.direction==AttackDirection.ANY) {
                        anyDirectionNode = next;
                    } else {
                        directionalLinks.Add(new Tuple<AttackLink, CombatNode>(link, next));
                    }
                }
            }
        }

        if (directionalLinks.Count > 0) {
            return directionalLinks[0].Item2;
        }

        if (anyDirectionNode != null) {
            return anyDirectionNode;
        }

        return null;
    }

    void Awake() {
        name = attackName;
    }

    override public void OnNodeEnter() {
        base.OnNodeEnter();
        attackGraph.anim.Play(attackName, layer:0, normalizedTime:timeOffset);
        timeOffset = 0;
    }
}

/*
#if UNITY_EDITOR

// highlight the current node
// unfortunately doesn't always update in time, but oh well
[CustomNodeEditor(typeof(AttackNode))]
public class AttackNodeEditor : NodeEditor {
    private AttackNode attackNode;
    private static GUIStyle editorLabelStyle;

    public override void OnBodyGUI() {
        attackNode = target as AttackNode;

        if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        if (attackNode.active) EditorStyles.label.normal.textColor = Color.cyan;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;
    }
}

#endif
*/