#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteOrderOffset))]
public class SpriteOrderOffsetEditor : Editor {

    SpriteOrderOffset s;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        s = (SpriteOrderOffset) target;
        if (GUILayout.Button("Apply Offset")) {
            s.ApplyOffset();
        }
    }
}
#endif