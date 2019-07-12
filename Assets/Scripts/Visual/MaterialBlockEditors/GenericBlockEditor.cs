using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GenericBlockEditor : MonoBehaviour {

    public string valueName;

    private Renderer r;
    protected MaterialPropertyBlock block;

    public FloatParams floatParams;
    public ColorParams colorParams;

    virtual protected void Start() {
        block = new MaterialPropertyBlock();
        r = GetComponent<Renderer>();
    }

    void LateUpdate() {
        GetBlock();
        foreach (KeyValuePair<string, float> kv in floatParams) {
            block.SetFloat(kv.Key, kv.Value);
        }

        foreach (KeyValuePair<string, Color> kv in colorParams) {
            block.SetColor(kv.Key, kv.Value);
        }
        SetBlock();
    }

    protected void SetBlock() {
        r.SetPropertyBlock(block);
    }

    protected void GetBlock() {
        if (block == null) block = new MaterialPropertyBlock();
        r.GetPropertyBlock(block);
    }
}

[System.Serializable]
public class FloatParams : SerializableDictionaryBase<string, float> {}

[System.Serializable]
public class ColorParams : SerializableDictionaryBase<string, Color> {}