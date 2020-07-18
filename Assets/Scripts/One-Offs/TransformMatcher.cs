using UnityEngine;

[ExecuteInEditMode]
public class TransformMatcher : MonoBehaviour {
    public Transform toMatchWith;

    public bool matchScale = true;
    public bool matchRotation = true;
    public bool matchPosition = true;
    
    void LateUpdate() {
        if (matchScale) transform.localScale = toMatchWith.localScale;
        if (matchRotation) transform.localRotation = toMatchWith.localRotation;
        if (matchPosition) transform.localPosition = toMatchWith.localPosition;
    }
}