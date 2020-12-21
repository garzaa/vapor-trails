using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class GridTilemapUtils : MonoBehaviour {
    public string sortingLayer = "Default";
    public int sortingOrder;
    public bool addCollider = false;

    void OnEnable() {
        if (GetComponent<Grid>() == null) {
            Grid g = gameObject.AddComponent<Grid>();
            g.cellSize = new Vector3(0.64f, 0.64f, 1);
        }
    }

    public void AddTilemap() {
        float parallaxAmount = (GetComponent<ParallaxLayer>()!=null) ? GetComponent<ParallaxLayer>().speed.x : 0;
        string tilemapName = $"{parallaxAmount}x {sortingOrder} {sortingLayer}";
    
        GameObject t = new GameObject(tilemapName);
        t.transform.parent = this.transform;
        t.AddComponent<Tilemap>();
        t.transform.position = Vector3.zero;

        TilemapRenderer tr = t.AddComponent<TilemapRenderer>();
        tr.sortingLayerName = sortingLayer;
        tr.sortingOrder = sortingOrder;

        if (addCollider) AddCollider(t);
    }

    void AddCollider(GameObject tilemap) {
        TilemapCollider2D collider = tilemap.AddComponent<TilemapCollider2D>();
        collider.usedByComposite = true;
        
        Rigidbody2D rb2d = tilemap.AddComponent<Rigidbody2D>();
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;

        CompositeCollider2D composite = tilemap.AddComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
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
