using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WidthFitter : MonoBehaviour
{
    public RectTransform target;
    RectTransform thisTransform;

    void Start() {
        thisTransform = GetComponent<RectTransform>();
    }

    void Update() {
        if (target != null) {
            thisTransform.sizeDelta = new Vector2(
                target.rect.size.x,
                thisTransform.sizeDelta.y
            );
        }
    }
}
