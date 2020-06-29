using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class MapFog : PersistentObject {
    [SerializeField] Texture2D fog;
    [SerializeField] GameObject cameraTarget;

    string scene;
    string mapProp = "Map";
    SpriteRenderer spriteRenderer;

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    override public void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SaveCurrentMap() {
        if (persistentProperties == null) persistentProperties = new Hashtable();
        persistentProperties[mapProp] = EncodeMap();
        SaveObjectState();
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        StopUpdating();

        // save if it's not the first load
        if (!string.IsNullOrEmpty(this.scene)) {
            SaveCurrentMap();
        } 

        // then initialize for a new scene
        WipeMap();
        this.scene = SceneManager.GetActiveScene().name;

        // try to load a texture from the current path
        SerializedPersistentObject o = LoadObjectState();
        if (o != null) {
            // update the texture with these
            DecodeAndUpdateMap((bool[]) o.persistentProperties[mapProp]);
        } 

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
            colors[i] = new Color(0, 0, 0, ToByte(map[i]));
        }
        fog.SetPixels32(colors, 0);
        fog.Apply();
    }

    void WipeMap() {
        int numPixels = fog.width * fog.height;
        Color32[] colors = new Color32[numPixels];
        for (int i=0; i<numPixels; i++) {
            colors[i] = new Color(0, 0, 0, 1);
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
        // put this at the start to deal with camera snapping (everyone starts at (0,0))
        yield return new WaitForSeconds(0.5f);

        Vector2 pos = cameraTarget.transform.position;

        // divide by texture ppu
        pos /= 8f;

        // texture (0, 0) is actually size/2, size/2
        pos += new Vector2(fog.width/2, fog.height/2);
        
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);

        fog.SetPixel(x, y, new Color32(0, 0, 0, 0));
        fog.Apply();

        StartCoroutine(UpdateMap());
    }

    override public string GetID() {
        return "MapFog/"+scene;
    }

    byte ToByte(bool b) {
        return b ? (byte) 1 : (byte) 0;
    }
    
    bool ToBool(byte b) {
        return b > 0;
    }
}