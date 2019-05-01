using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SpriteOrderOffset : MonoBehaviour {
    public int offset;

    private SpriteRenderer[] renderers;

    public void Start() {
        GetChildren();
    }

    public void GetChildren() {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ApplyOffset() {
        for (int i=0; i<renderers.Length; i++) {
            renderers[i].sortingOrder += offset;
        }
    }
}

[CustomEditor(typeof(SpriteOrderOffset))]
class SpriteOrderOffsetEditor : Editor {
    SpriteOrderOffset spriteOrderOffset;

    void Awake() {
        spriteOrderOffset = (SpriteOrderOffset) target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Apply Offset")) {
            spriteOrderOffset.GetChildren();
            spriteOrderOffset.ApplyOffset();
        }
    }
}