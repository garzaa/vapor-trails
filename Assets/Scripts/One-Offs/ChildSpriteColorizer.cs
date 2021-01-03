using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class ChildSpriteColorizer : MonoBehaviour {
    public Color color;

    Color lastColor;

    SpriteRenderer[] sprites;


#if UNITY_EDITOR
    void OnEnable() {
        sprites = GetComponentsInChildren<SpriteRenderer>(includeInactive: true)
            .Where(x => x.GetComponent<IgnoreSpriteColorization>()==null)
            .ToArray();
    }

    void Update() {
        if (lastColor != null && color != lastColor) {
            for (int i=0; i<sprites.Length; i++) {
                sprites[i].color = color;
            }
        }
        lastColor = color;
    }
#endif
}
