using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : MonoBehaviour {
    public AudioClip changeSound;
    public string prefName;
    public bool defaultValue;

    bool quiet;

    void OnEnable() {
        quiet = true;
        GetComponentInChildren<Toggle>().isOn = GameOptions.LoadBool(prefName);
        quiet = false;
    }

    public void HandleValueChanged(bool val) {
        SetBool(prefName, val);
        if (!quiet) {
            // called from UI and not auto-settings change
            StateChangeRegistry.PushStateChange();
            SoundManager.UISound(changeSound);
        }
    }

    static void SetBool(string boolName, bool val) {
        PlayerPrefs.SetInt(boolName, val ? 1 : 0);
    }
}
