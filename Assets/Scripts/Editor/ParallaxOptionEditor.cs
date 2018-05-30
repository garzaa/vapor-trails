using UnityEngine;
using UnityEditor;
 
[CustomEditor(typeof(ParallaxOption))]
class ParallaxOptionEditor : Editor
{
    private ParallaxOption options;
 
    void Awake()
    {
        options = (ParallaxOption)target;
    }
 
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
 
        if(GUILayout.Button("Save Position"))
        {
            options.SavePosition();
            EditorUtility.SetDirty(options);
        }
 
        if(GUILayout.Button("Restore Position"))
        {
            options.RestorePosition();
        }
    }
}