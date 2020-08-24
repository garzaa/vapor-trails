using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item))]
class ItemEditor : Editor {
    /*
    commented because it throws a nullref every time the editor reloads the assets

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {

        // hopefully if you do it like this it doesn't crash
        Texture2D baseTex = ((Item) target).detailedIcon.texture;
        Texture2D newTex = new Texture2D(baseTex.width, baseTex.height);
        EditorUtility.CopySerialized(baseTex, newTex);

        return newTex;
    }
    
    */
}