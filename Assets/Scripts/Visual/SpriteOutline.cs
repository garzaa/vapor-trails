using UnityEngine;

[ExecuteInEditMode]
public class SpriteOutline : MonoBehaviour
{
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

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

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        mpb.SetVector("_Rect", result);
        spriteRenderer.SetPropertyBlock(mpb);

    }
}