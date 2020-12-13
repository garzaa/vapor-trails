using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridTilemapUtils : MonoBehaviour {
    public string sortingLayer = "Default";
    public int sortingOrder;

    public void AddTilemap() {
        float parallaxAmount = (GetComponent<ParallaxLayer>()!=null) ? GetComponent<ParallaxLayer>().speed.x : 0;
        string tilemapName = $"{parallaxAmount}x {sortingOrder} {sortingLayer}";
        GameObject t = new GameObject(tilemapName);
        t.transform.parent = this.transform;
        t.AddComponent<Tilemap>();
        TilemapRenderer tr = t.AddComponent<TilemapRenderer>();
        tr.sortingLayerName = sortingLayer;
        tr.sortingOrder = sortingOrder;
        t.transform.position = Vector3.zero;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridTilemapUtils))]
public class GridTilemapUtilsInspector : Editor {
    public override void OnInspectorGUI() {
        base.DrawDefaultInspector();
        GridTilemapUtils tilemapUtils = (GridTilemapUtils) target;

        if (GUILayout.Button("Add Tilemap")) {
            tilemapUtils.AddTilemap();  
        }
    }
}
#endif
