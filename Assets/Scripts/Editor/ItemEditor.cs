using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
class ItemEditor : Editor {
    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
        return ((Item) target).detailedIcon.texture;
    }
}