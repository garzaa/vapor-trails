using UnityEngine;

[ExecuteInEditMode]
public class SpriteOrderOffset : MonoBehaviour {
    public int offset;

    [Header("Current range")]
    public int front;
    public int back;

    private Renderer[] renderers;

    public void Start() {
        GetChildren();
    }

    public void GetChildren() {
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void ApplyOffset() {
        int max = int.MinValue;
        int min = int.MaxValue;
        for (int i=0; i<renderers.Length; i++) {
            Renderer s = renderers[i];
            s.sortingOrder += offset;
            max = s.sortingOrder > max ? s.sortingOrder : max;
            min = s.sortingOrder < min ? s.sortingOrder : min;
        }
        front = max;
        back = min;
    }
}