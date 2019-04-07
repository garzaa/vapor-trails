using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialBlockEditor : MonoBehaviour
{
    protected Material material;
    public string valueName;

    void Start() {
        material = GetComponent<Renderer>().material;
    }
}
