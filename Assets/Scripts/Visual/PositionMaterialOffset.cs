using UnityEngine;

public class PositionMaterialOffset : MaterialBlockEditor {
    
    [SerializeField] Vector2 ratio;
    
    void Update() {
        GetBlock();
        block.SetVector("_Offset", (Vector4) (transform.position * ratio));
        SetBlock();
    }
}