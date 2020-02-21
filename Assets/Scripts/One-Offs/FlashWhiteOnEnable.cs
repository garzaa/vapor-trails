using UnityEngine;

public class FlashWhiteOnEnable : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    Material defaultMaterial;
    static Material whiteMaterial;

    public float flashTime = 0.1f;

    void Awake() {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (defaultMaterial == null) defaultMaterial = spriteRenderer.material;
        if (whiteMaterial == null) {
            whiteMaterial = Resources.Load<Material>("Shaders/WhiteFlash");
        }
    }

    void WhiteSprite() {
        spriteRenderer.material = whiteMaterial;
        Invoke("NormalSprite", flashTime);
    }

    void NormalSprite() {
        spriteRenderer.material = defaultMaterial;
    }

    void OnEnable() {
        WhiteSprite();
    }
}