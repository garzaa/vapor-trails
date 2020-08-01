using UnityEngine;

[System.Serializable]
public class GameOptions {
    public bool shortHop = true;

    public void Load() {
        shortHop = LoadBool("shortHop");
    }

    public void Apply() {
        PlayerPrefs.Save();
        Load();
    }

    bool LoadBool(string boolName) {
        return PlayerPrefs.GetInt(boolName) == 1;
    }

    void SetBool(string boolName, bool val) {
        PlayerPrefs.SetInt(boolName, val ? 1 : 0);
    }
}