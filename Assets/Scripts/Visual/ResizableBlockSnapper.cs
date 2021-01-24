using UnityEngine;

// put the object's pivot on a corner for best results
[ExecuteInEditMode]
public class ResizableBlockSnapper : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    float gridSize;

    void OnEnable() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridSize = GetComponentInParent<Grid>().cellSize.x;
    }

    void Update() {
        spriteRenderer.size = new Vector2(
            RoundToGridSize(spriteRenderer.size.x),
            RoundToGridSize(spriteRenderer.size.y)
        );

        transform.localPosition = new Vector2(
            RoundToGridSize(transform.localPosition.x),
            RoundToGridSize(transform.localPosition.y)
        );
    }

    float RoundToGridSize(float f) {
        return Mathf.Round(f / gridSize) * gridSize;
    }
}
