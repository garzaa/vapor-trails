#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteRigger))]
public class SpriteRiggerEditor : Editor {

    SpriteRigger s;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        s = (SpriteRigger) target;
        if (GUILayout.Button("Apply atlas")) {
            s.ApplyAtlas();
        }
    }
}
#endif