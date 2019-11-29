using UnityEngine;

[ExecuteInEditMode]
public class SpriteRigger : MonoBehaviour {
    public Texture2D spriteAtlas;

    public void ApplyAtlas() {
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>()) {
            Sprite oldSprite = spriteRenderer.sprite;
            Debug.Log($"{oldSprite.name}, {oldSprite.pivot}");
            Sprite newSprite = Sprite.Create(
                spriteAtlas,
                oldSprite.rect,
                oldSprite.pivot / oldSprite.rect.size,
                oldSprite.pixelsPerUnit,
                1,
                SpriteMeshType.Tight,
                oldSprite.border,
                false
            );
            spriteRenderer.sprite = newSprite;
            
        }

    }

    Vector2 convertPivot(Vector2 pivot, Rect rect) {
        // assume it's a pixel-based pivot, convert it to normalized
        return pivot / rect.size;
    }
}
