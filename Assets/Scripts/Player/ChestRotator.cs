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
        // only call Update if we're modifying things in the editor
        if (Application.isEditor && !Application.isPlaying) ChangeSkin();
    }

    void LateUpdate() {
        ChangeSkin();
    }

    void ChangeSkin() {
        spriteRenderer.sprite = skins[skinNum].sprites[spriteIndex];
        skinLastFrame = skinNum;                                                                                                                                                                                    
        indexLastFrame = spriteIndex; 
    }
}

[System.Serializable]
public class ChestSprites {
    public List<Sprite> sprites;
}