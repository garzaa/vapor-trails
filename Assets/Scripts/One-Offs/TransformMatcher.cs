using UnityEngine;

[ExecuteInEditMode]
public class TransformMatcher : MonoBehaviour {
    public Transform toMatchWith;
    
    void Update() {
        transform.localScale = toMatchWith.localScale;
        transform.localRotation = toMatchWith.localRotation;
        transform.localPosition = toMatchWith.localPosition;
    }
}