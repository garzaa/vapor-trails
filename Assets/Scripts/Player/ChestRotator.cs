using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ChestRotator : MonoBehaviour {
    public int skinNum;
    public int spriteIndex;
    [SerializeField]
    public List<ChestSprites> skins;

    int skinLastFrame;
    int indexLastFrame;
    SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if (!Application.isEditor) return;
        if (skinLastFrame != skinNum || indexLastFrame != spriteIndex) {
            if (skinNum >= skins.Count || spriteIndex >= skins[skinNum].sprites.Count) {
                return;
            }
        }
        spriteRenderer.sprite = skins[skinNum].sprites[spriteIndex];
        skinLastFrame = skinNum;                                                                                                                                                                                    
        indexLastFrame = spriteIndex;
    }

    void LateUpdate() {
        Update();
    }
}

[System.Serializable]
public class ChestSprites {
    public List<Sprite> sprites;
}