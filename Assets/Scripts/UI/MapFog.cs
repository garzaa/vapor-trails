using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class MapFog : PersistentObject {
    [SerializeField] Texture2D fog;
    [SerializeField] GameObject cameraTarget;

    string currentScene;
    string mapProp = "Map";
    SpriteRenderer spriteRenderer;

    float texturePPU = 1f/12f;
    float updateInterval = 0.5f;

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
        StopUpdatingMap();

        if (!string.IsNullOrEmpty(currentScene)) {
            SaveCurrentMap();
        } 

        currentScene = scene.name;

        SerializedPersistentObject savedState = LoadObjectState();
        if (savedState != null) {
            DecodeAndUpdateMap((bool[]) savedState.persistentProperties[mapProp]);
        } else {
            WipeMap();
        }

        StartUpdatingMap();
    }

    bool[] EncodeMap() {
        Color32[] pixels = fog.GetPixels32();
        bool[] alphas = new bool[pixels.Length];
        for (int i=0; i<pixels.Length; i++) {
            alphas[i] = pixels[i].a > 0;
        }
        return alphas;
    }

    void DecodeAndUpdateMap(bool[] map) {
        Color32[] colors = new Color32[map.Length];
        for (int i=0; i<map.Length; i++) {
            colors[i] = new Color(0, 0, 0, ToByte(map[i]));
        }
        fog.SetPixels32(colors);
        fog.Apply();
    }

    byte ToByte(bool b) {
        return b ? (byte) 1 : (byte) 0;
    }

    void WipeMap() {
        Color32[] colors = new Color32[fog.width*fog.height];
        for (int i=0; i<colors.Length; i++) {
            colors[i] = new Color(0, 0, 0, 1);
        }
        fog.SetPixels32(colors);
        fog.Apply();
    }

    void StopUpdatingMap() {
        StopAllCoroutines();
    }

    void StartUpdatingMap() {
        StartCoroutine(UpdateMap());
    }

    IEnumerator UpdateMap() {
        yield return new WaitForSeconds(updateInterval);

        Vector2 pos = cameraTarget.transform.position;
        pos *= texturePPU;
        pos += new Vector2(fog.width/2, fog.height/2);

        fog.SetPixel(
            Mathf.FloorToInt(pos.x),
            Mathf.FloorToInt(pos.y),
            new Color32(0, 0, 0, 0)
        );
        fog.Apply();

        StartCoroutine(UpdateMap());
    }

    override public string GetID() {
        return "MapFog/"+currentScene;
    }    
}