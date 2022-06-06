using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SpriteRigger : MonoBehaviour {
    // the atlas to be applied
    public Texture2D spriteAtlas;
    
    void Awake() {
        ApplyAtlas();
    }

    public void ApplyAtlas() {
        if (spriteAtlas == null) return;
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

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteRigger))]
public class SpriteRiggerInspector : Editor {
    public override void OnInspectorGUI() {
        base.DrawDefaultInspector();
        SpriteRigger spriteRigger = target as SpriteRigger;

        if (GUILayout.Button("Apply Atlas")) {
            spriteRigger.ApplyAtlas();  
        }
    }
}
#endif
