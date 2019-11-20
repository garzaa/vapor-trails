using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextureOutline : MonoBehaviour
{
    public Color color = Color.white;
    public bool outline = true;

    [Range(0, 16)]
    public int outlineSize = 1;

    RawImage rawImage;
    Material mpb;

    void OnEnable()
    {
        rawImage = GetComponent<RawImage>();
        if (!Application.isEditor) {
            mpb = GetComponent<RawImage>().material;
        } else {
            mpb = GetComponent<RawImage>().material;
        }
        UpdateOutline();
        Texture texture = rawImage.mainTexture;
        Vector4 result = new Vector4(
            rawImage.uvRect.x / texture.width,
            rawImage.uvRect.y / texture.height,
            rawImage.uvRect.width / texture.width,
            rawImage.uvRect.height / texture.height
        );
        Debug.Log(rawImage.uvRect);
        Debug.Log(texture.width);
        Debug.Log(texture.height);
    }

    void OnDisable()
    {
        UpdateOutline();
    }

    void LateUpdate()
    {
        UpdateOutline();
    }

    void UpdateOutline()
    {
        /*
        Sprite sprite = spriteRenderer.sprite;
        Vector4 result = new Vector4(sprite.textureRect.min.x / sprite.texture.width,
            sprite.textureRect.min.y / sprite.texture.height,
            sprite.textureRect.max.x / sprite.texture.width,
            sprite.textureRect.max.y / sprite.texture.height);
            */
        
        Texture texture = rawImage.mainTexture;
        Vector4 result = new Vector4(
            rawImage.uvRect.min.x / texture.width,
            rawImage.uvRect.min.y / texture.height,
            rawImage.uvRect.max.x / texture.width,
            rawImage.uvRect.max.y / texture.height
        );
        mpb.SetFloat("_Outline", this.outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        mpb.SetVector("_Rect", result);

    }
}