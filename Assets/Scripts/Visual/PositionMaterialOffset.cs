using UnityEngine;

public class PositionMaterialOffset : MaterialBlockEditor {
    
    #pragma warning disable 0649
    [SerializeField] Vector2 ratio;
	#pragma warning restore 0649
    
    void Update() {
        GetBlock();
        block.SetVector("_Offset", (Vector4) (transform.position * ratio));
        SetBlock();
    }
}
