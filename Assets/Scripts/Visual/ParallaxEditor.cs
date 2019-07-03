using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParallaxLayer))]
public class ObjectBuilderEditor : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();
        ParallaxLayer p = (ParallaxLayer) target;

        if (GUILayout.Button("Set Position")) {
            //p.SetPosition();
        }
    }
}