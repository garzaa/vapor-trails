# if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorCameraFollower : MonoBehaviour {

    Vector3 lastPos = Vector3.zero;

    void OnEnable() {
        EditorApplication.update += Update;
    }

    void Update() {
        if (!Application.isPlaying && Camera.current != null && Camera.current.transform != null) {
            Vector3 currentPos = Camera.current.transform.position;
            if (!currentPos.Equals(lastPos)) {
                this.transform.position = (Vector2) Camera.current.transform.position;
            }
            lastPos = Camera.current.transform.position;
        }
    }

    void OnDisable() {
        EditorApplication.update -= Update;
        this.transform.position = Vector2.zero;
    }

}

# endif
