using UnityEngine;

[ExecuteInEditMode]
public class SpriteOrderOffset : MonoBehaviour {
    public int offset;

    [Header("Current range")]
    public int front;
    public int back;

    private SpriteRenderer[] renderers;

    public void Start() {
        GetChildren();
    }

    public void GetChildren() {
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void ApplyOffset() {
        int max = int.MinValue;
        int min = int.MaxValue;
        for (int i=0; i<renderers.Length; i++) {
            SpriteRenderer s = renderers[i];
            max = s.sortingOrder > max ? s.sortingOrder : max;
            min = s.sortingOrder < min ? s.sortingOrder : min;
            s.sortingOrder += offset;
        }
        front = max;
        back = min;
    }
}