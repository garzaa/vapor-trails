using UnityEngine;
using UnityEngine.UI;

public class SettingsSlider : MonoBehaviour {
    public string prefName;
    public AudioClip changeSound;
    public Text valueLabel;
    public int defaultValue;

    bool quiet;

    void OnEnable() {
        quiet = true;
        GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt(prefName, defaultValue);
        quiet = false;
    }

    public void HandleValueChanged(float val) {
        if (!quiet) SoundManager.PlaySound(changeSound);
        PlayerPrefs.SetInt(prefName, (int) val);
        valueLabel.text = ((int) val).ToString();
    }
}