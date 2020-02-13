using UnityEngine;

public class AudioFade : MonoBehaviour {

    public bool disableAtStart;
    public bool combatMusic;

    float targetVolume;
    AudioSource audioSource;
    float startTime;

    bool fadingOut;
    bool fadingIn;
    float fadeTime;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        // it might have been set from the editor to a non-1 value
        targetVolume = audioSource.volume;
        if (disableAtStart) audioSource.volume = 0;
    }

    public void FadeIn(float time) {
        fadingOut = false;
        fadingOut = true;
        fadeTime = time;
        startTime = Time.time;
    }

    public void FadeOut(float time) {
        fadingOut = true;
        fadingIn = false;
        fadeTime = time;
        startTime = Time.time;
    }

    void FixedUpdate() {
        if (fadingIn) {
            if (audioSource.volume < targetVolume) {
                audioSource.volume = ((Time.time-startTime) / fadeTime) * targetVolume;
            } else {
                fadingIn = false;
            }
        } else if (fadingOut) {
            if (audioSource.volume > 0) {
                audioSource.volume = targetVolume - (((Time.time-startTime) / fadeTime) * targetVolume);
            } else {
                fadingOut = false;
            }
        }
    }
}