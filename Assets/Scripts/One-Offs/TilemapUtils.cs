using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TilemapUtils: MonoBehaviour {
    Tilemap tilemap;

    void OnEnable() {
        tilemap = GetComponent<Tilemap>();
    }

    public void ClearTilemap() {
        tilemap.ClearAllTiles();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TilemapUtils))]
public class TilemapUtilsInspector : Editor {
    public override void OnInspectorGUI() {
        base.DrawDefaultInspector();

        TilemapUtils tilemapUtils = (TilemapUtils) target;
        if (GUILayout.Button("Clear Tilemap")) {
            tilemapUtils.ClearTilemap();
        }
    }
}
#endif
