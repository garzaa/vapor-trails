# if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorCameraFollower : MonoBehaviour {

    void OnEnable() {
        EditorApplication.update += Update;
    }

    void Update() {
        if (!Application.isPlaying && Camera.current != null && Camera.current.transform != null) {
            this.transform.position = (Vector2) Camera.current.transform.position;
        }
    }

    void OnDisable() {
        EditorApplication.update -= Update;
        this.transform.position = Vector2.zero;
    }

}

# endif