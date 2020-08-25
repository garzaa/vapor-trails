using System.Collections.Generic;
using System;
using UnityEngine;
using XNode;

#if UNITY_EDITOR
using XNodeEditor;
using UnityEditor;
#endif

[NodeWidth(270)]
public class AttackNode : Node {
    public string attackName;
    // TODO: also make horizontal input trigger buffer
    public int IASA = 7;

    [HideInInspector]
    public bool cancelable = false;

    [HideInInspector]
    public bool active = false;

    [Input(backingValue=ShowBackingValue.Never)] public AttackLink input;
    [Output(dynamicPortList=true, connectionType=ConnectionType.Override)] public AttackLink[] links;

    List<Tuple<AttackLink, AttackNode>> directionalLinks = new List<Tuple<AttackLink, AttackNode>>();
    AttackNode anyDirectionNode = null;

    // directional attacks are prioritized in order, otherwise the first any-directional link is used
    public AttackNode GetNextAttack(AttackBuffer buffer) {
        directionalLinks.Clear();
        anyDirectionNode = null;

        for (int i=0; i<links.Length; i++) {
            AttackLink link = links[i];
            if (link.type==buffer.type && buffer.HasDirection(link.direction)) {
                AttackNode next = GetPort("links "+i).Connection.node as AttackNode;
                if (next.Enabled()) {
                    if (link.direction == AttackDirection.ANY) {
                        anyDirectionNode = next;
                    } else {
                        directionalLinks.Add(new Tuple<AttackLink, AttackNode>(link, next));
                    }
                }
            }
        }

        // return first encountered directional match
        if (directionalLinks.Count > 0) {
            return directionalLinks[0].Item2;
        }

        if (anyDirectionNode != null) {
            return anyDirectionNode;
        }

        return null;
    }

    private void Reset() {
        name = attackName;
    }

    public virtual void OnNodeEnter() {
        active = true;
    }

    public virtual void OnNodeExit() {
        cancelable = false;
        active = false;
    }

    public virtual bool Enabled() {
        return true;
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