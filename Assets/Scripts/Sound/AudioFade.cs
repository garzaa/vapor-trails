using UnityEngine;

public class AudioFade : MonoBehaviour {

    public bool disableAtStart;
    public bool combatMusic;
    public bool restartOnFadeIn;

    float maxVolume;
    AudioSource audioSource;
    float startTime;

    bool fadingOut;
    bool fadingIn;
    float fadeTime;


    void Start() {
        audioSource = GetComponent<AudioSource>();
        // it might have been set from the editor to a non-1 value
        maxVolume = audioSource.volume;
        if (disableAtStart) audioSource.volume = 0;
    }

    public void FadeIn(float time) {
        fadingOut = false;
        fadingIn = true;
        fadeTime = time;
        startTime = Time.time;
        if (restartOnFadeIn) audioSource.time = 0;
    }

    public void FadeOut(float time) {
        fadingOut = true;
        fadingIn = false;
        fadeTime = time;
        startTime = Time.time;
    }

    void FixedUpdate() {
        if (fadingIn) {
            if (audioSource.volume < maxVolume) {
                audioSource.volume = ((Time.time-startTime) / fadeTime) * maxVolume;
            } else {
                fadingIn = false;
            }
        } else if (fadingOut) {
            if (audioSource.volume > 0) {
                audioSource.volume = maxVolume - (((Time.time-startTime) / fadeTime) * maxVolume);
            } else {
                fadingOut = false;
            }
        }
    }
}