using UnityEngine;
using System.Collections;
 
public class AudioFadeIn : MonoBehaviour {
    public float fadeTime;

    float targetVolume;
    AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        targetVolume = audioSource.volume;
        audioSource.volume = 0;
    }

    void Update() {
        if (audioSource.volume < targetVolume) {
            // this was ripped from stackoverflow, the math is bad but *40 works
            audioSource.volume += Time.deltaTime / (this.fadeTime * 40);
        } else {
            // this script's job is done
            Destroy(this);
        }
    } 
}