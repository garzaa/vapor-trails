using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;

public class AudioSlider : SettingsSlider {
    public AudioMixerGroup mixerGroup;

    override public void HandleValueChanged(float val) {
        base.HandleValueChanged(val);
        // 5 should map to no change, and log(1) = 0
        val /= 5;
        mixerGroup.audioMixer.SetFloat(prefName, Mathf.Log(Mathf.Max(val, 0.0001f), 10) * 20f);
    }
}