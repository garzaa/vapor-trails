using UnityEngine;

public class FlashWhiteOnEnable : MonoBehaviour {
    Renderer r;
    Material defaultMaterial;
    static Material whiteMaterial;

    public float flashTime = 0.1f;

    void Awake() {
        if (r == null) r = GetComponent<Renderer>();
        if (defaultMaterial == null) defaultMaterial = r.material;
        if (whiteMaterial == null) {
            whiteMaterial = Resources.Load<Material>("Shaders/WhiteFlash");
        }
    }

    void WhiteSprite() {
        r.material = whiteMaterial;
        Invoke("NormalSprite", flashTime);
    }

    void NormalSprite() {
        r.material = defaultMaterial;
    }

    void OnEnable() {
        WhiteSprite();
    }
}