using UnityEngine;
 
public class AudioFadeIn : MonoBehaviour {
    public float fadeTime;

    float targetVolume;
    AudioSource audioSource;
    float startTime;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        // it might have been set from the editor to a non-1 value
        targetVolume = audioSource.volume;
        audioSource.volume = 0;
        startTime = Time.time;
    }

    void FixedUpdate() {
        if (audioSource.volume < targetVolume) {
            audioSource.volume = ((Time.time-startTime) / fadeTime) * targetVolume;
        } else {
            this.enabled = false;
        }
    } 
}