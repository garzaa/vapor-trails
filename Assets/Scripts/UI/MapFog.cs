using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class MapFog : PersistentObject {
    #pragma warning disable 0649
    [SerializeField] Texture2D fog;
    [SerializeField] GameObject cameraTarget;
    #pragma warning restore 0649

    const string MAP_PROPERTY = "FogTexture";

    float texturePPU = 1f/12f;
    float updateInterval = 0.2f;

    override protected void SetDefaults() {
        if (HasProperty(MAP_PROPERTY)) {
            DecodeAndUpdateMap(GetProperty<byte[]>(MAP_PROPERTY));
        } else {
            ResetMap();
        }
    }

    void ResetMap() {
        Color32[] colors = new Color32[fog.width*fog.height];
        for (int i=0; i<colors.Length; i++) {
            colors[i] = new Color(0, 0, 0, 1);
        }
        fog.SetPixels32(colors);
        fog.Apply();
    }

    void Start() {
       StartCoroutine(UpdateMap()); 
    }

    public void SaveCurrentMap() {
        SetProperty(MAP_PROPERTY, EncodeMap());
    }

    byte[] EncodeMap() {
        Color32[] pixels = fog.GetPixels32();
        byte[] alphas = new byte[pixels.Length];
        for (int i=0; i<pixels.Length; i++) {
            alphas[i] = pixels[i].a > 0 ? (byte) 1 : (byte) 0;
        }
        return alphas;
    }

    void DecodeAndUpdateMap(byte[] map) {
        Color32[] colors = new Color32[map.Length];
        for (int i=0; i<map.Length; i++) {
            colors[i] = new Color(0, 0, 0, map[i]);
        }
        fog.SetPixels32(colors);
        fog.Apply();
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
        SaveCurrentMap();

        StartCoroutine(UpdateMap());
    }

    override public string GetID() {
        return "MapFog/" + SceneManager.GetActiveScene().name;
    }
}
