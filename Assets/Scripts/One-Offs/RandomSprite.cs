using UnityEngine;

public class RandomSprite : MonoBehaviour {
    public Sprite[] sprites;

    void OnEnable() {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}