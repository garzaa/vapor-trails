using UnityEngine;

[ExecuteInEditMode]
public class SpriteMatcher : MonoBehaviour {
    public SpriteRenderer toMatchWith;

    SpriteRenderer sprite;

    void OnEnable() {
        sprite = GetComponent<SpriteRenderer>();
    }
    
    void LateUpdate() {
        sprite.sprite = toMatchWith.sprite;
    }
}
