using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteOrderOffset : MonoBehaviour {

    public int offset;
    int offsetLastFrame;
    List<SpriteRenderer> sprites;
    List<int> originalOrders; 

    void Start() {
        sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        foreach (SpriteRenderer s in sprites) {
            originalOrders.Add(s.sortingOrder);
        }
    }

    void Update() {
        if (offsetLastFrame != offset) {
            UpdateSpritesOffset(offset);
        }
        offset = offsetLastFrame;
    }

    void UpdateSpritesOffset(int newOffset) {
        for (int i=0; i<sprites.Count; i++) {
            sprites[i].sortingOrder = originalOrders[i];
        }
    }

    void OnDisable() {
        UpdateSpritesOffset(0);
    }

}
