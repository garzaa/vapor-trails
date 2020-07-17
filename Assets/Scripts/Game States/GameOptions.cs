using UnityEngine;

[System.Serializable]
public class GameOptions {
    public bool shortHop = true;

    public void Refresh() {
        shortHop = LoadBool("shortHop");
    }

    public void Save() {
        PlayerPrefs.Save();
        Refresh();
    }

    bool LoadBool(string boolName) {
        return PlayerPrefs.GetInt(boolName) == 1;
    }

    void SetBool(string boolName, bool val) {
        PlayerPrefs.SetInt(boolName, val ? 1 : 0);
    }
}