using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ChildColorEditor : MaterialBlockEditor {
    public Color color;

    private MaterialPropertyBlock[] blocks;
    private Renderer[] renderers;

    override protected void Start() {
        GetChildren();
    }

    void GetChildren() {
        renderers = GetComponentsInChildren<Renderer>();
        blocks = new MaterialPropertyBlock[renderers.Length];

        for (int i=0; i<renderers.Length; i++) {
            print(renderers[i].name);
            blocks[i] = new MaterialPropertyBlock();
            renderers[i].GetPropertyBlock(blocks[i]);
        }
    }

    public void UpdateColors() {
        GetChildren();
        for (int i=0; i<renderers.Length; i++) {
            MaterialPropertyBlock b = new MaterialPropertyBlock(); 
            renderers[i].GetPropertyBlock(b);
            b.SetColor(valueName, color);
            blocks[i] = b;
            print(renderers[i].name);
        }
    }
}

[CustomEditor(typeof(ChildColorEditor))]
class ChildColorEditorInspector : Editor {
    ChildColorEditor colorEditor;

    void Awake() {
        colorEditor = (ChildColorEditor) target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Update Child Colors")) {
            colorEditor.UpdateColors();
        }
    }
}