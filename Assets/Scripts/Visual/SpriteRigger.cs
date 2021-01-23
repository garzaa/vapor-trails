using UnityEngine;

[ExecuteInEditMode]
public class SpriteRigger : MonoBehaviour {
    public Texture2D spriteAtlas;
    
    void OnEnable() {
        if (spriteAtlas != null) ApplyAtlas();gs`
    }

    public void ApplyAtlas() {
        foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>()) {
            Sprite oldSprite = spriteRenderer.sprite;
            Sprite newSprite = Sprite.Create(
                spriteAtlas,
                oldSprite.rect,
                // assume it's a pixel-based pivot, convert it to normalized like the constructor expects
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
}
