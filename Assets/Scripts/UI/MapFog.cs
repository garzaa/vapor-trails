using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class MapFog : PersistentObject {
    [SerializeField] Texture2D fog;

    string scene;
    string mapProp = "Map";
    Color transparent = new Color(0, 0, 0, 0);

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public void SaveCurrentMap() {
        Debug.Log("Saving current map");
        persistentProperties[mapProp] = EncodeMap();
        SaveObjectState();
        Debug.Log("Done");
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        StopUpdating();

        // save if it's not the first load
        if (!string.IsNullOrEmpty(this.scene)) {
            SaveCurrentMap();
        } else {
            WipeMap();
        }

        // then initialize for a new scene
        this.scene = scene.name;

        // try to load a texture from the current path
        SerializedPersistentObject o = LoadObjectState();
        if (o != null) {
            Debug.Log("Texture found, loading it in");
            // update the texture with these
            DecodeAndUpdateMap((bool[]) o.persistentProperties[mapProp]);
            Debug.Log("Done");
        } else {
            Debug.Log("No texture found, loading a black texture");
            WipeMap();
        }

        Debug.Log("Updating map");
        StartUpdating();
    }

    bool[] EncodeMap() {
        // array of bits
        Color32[] pixels = fog.GetPixels32();
        bool[] alphas = new bool[pixels.Length];

        for (int i=0; i<pixels.Length; i++) {
            alphas[i] = ToBool(pixels[i].a);
        }

        return alphas;
    }

    void DecodeAndUpdateMap(bool[] map) {
        // from a bool array to an image
        Color32[] colors = new Color32[map.Length];
        for (int i=0; i<map.Length; i++) {
            colors[i] = new Color32(0, 0, 0, ToByte(map[i]));
        }
        fog.SetPixels32(colors);
        fog.Apply();
    }

    void WipeMap() {
        int numPixels = fog.width * fog.height;
        Color32[] colors = new Color32[numPixels];
        for (int i=0; i<numPixels; i++) {
            colors[i] = Color.black;
        }
        fog.SetPixels32(colors, 0);
        fog.Apply();
    }

    void StopUpdating() {
        StopAllCoroutines();
    }

    void StartUpdating() {
        StartCoroutine(UpdateMap());
    }

    IEnumerator UpdateMap() {
        Vector2 pos = GlobalController.pc.transform.position;
        Debug.Log("player is currently at "+pos);

        // divide by texture ppu
        pos /= 8f;

        // texture (0, 0) is actually size/2, size/2
        pos += new Vector2(fog.width/2, fog.height/2);
        
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);

        Debug.Log($"Updating pixel {x}, {y} with player position");
        fog.SetPixel(x, y, transparent);
        fog.Apply();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(UpdateMap());
    }

    override public string GetID() {
        if (id == null) {
            id = "MapFog/"+scene;
        }
        return id;
    }

    byte ToByte(bool b) {
        return b ? (byte) 1 : (byte) 0;
    }
    
    bool ToBool(byte b) {
        return b > 0;
    }
}