using UnityEngine;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour
{
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    SpriteRenderer spriteRenderer;
    Material mpb;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (Application.isEditor) {
            mpb = GetComponent<Renderer>().material;
        } else {
            mpb = GetComponent<Renderer>().sharedMaterial;
        }
        UpdateOutline(true);
    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void LateUpdate()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    {
        Sprite sprite = spriteRenderer.sprite;
        Vector4 result = new Vector4(sprite.textureRect.min.x / sprite.texture.width,
            sprite.textureRect.min.y / sprite.texture.height,
            sprite.textureRect.max.x / sprite.texture.width,
            sprite.textureRect.max.y / sprite.texture.height);

        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        mpb.SetVector("_Rect", result);

    }
}