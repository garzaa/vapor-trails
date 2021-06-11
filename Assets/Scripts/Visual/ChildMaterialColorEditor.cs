using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class ChildMaterialColorEditor : MonoBehaviour {
    [Header("Use context menu to find children")]
    public string fieldName;
    public Color color;

    List<MaterialColorEditor> editors;

    [ContextMenu("Find Children")]
    void Start() {
        editors = GetComponentsInChildren<MaterialColorEditor>(includeInactive: true)
            .Where(x => x.valueName.Equals(this.fieldName))
            .ToList();
    }

    void Update() {
        for (int i=0; i<editors.Count; i++) {
            editors[i].color = this.color;
        }
    }
}
