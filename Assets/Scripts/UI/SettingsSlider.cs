using UnityEngine;
using UnityEngine.UI;

public class SettingsSlider : MonoBehaviour {
    public string prefName;
    public AudioClip changeSound;
    public Text valueLabel;
    public int defaultValue;

    protected bool quiet;

    virtual protected void OnEnable() {
        quiet = true;
        GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt(prefName, defaultValue);
        // force an update
        HandleValueChanged(GetComponentInChildren<Slider>().value);
        quiet = false;
    }

    void OnDisable() {
        OnEnable();
    }

    virtual public void HandleValueChanged(float val) {
        if (!quiet) SoundManager.UISound(changeSound);
        PlayerPrefs.SetInt(prefName, (int) val);
        valueLabel.text = ((int) val).ToString();
    }
}