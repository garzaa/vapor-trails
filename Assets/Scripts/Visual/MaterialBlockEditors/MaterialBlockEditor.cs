using UnityEngine;

[ExecuteInEditMode]
public class MaterialBlockEditor : MonoBehaviour {

    public string valueName;

    private Renderer r;
    protected MaterialPropertyBlock block;

    virtual protected void Start() {
        block = new MaterialPropertyBlock();
        r = GetComponent<Renderer>();
        r.GetPropertyBlock(block);
    }

    protected void SetBlock() {
        r.SetPropertyBlock(block);
    }

    protected void GetBlock() {
        r.GetPropertyBlock(block);
    }
}
